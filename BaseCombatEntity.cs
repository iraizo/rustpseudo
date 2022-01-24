using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class BaseCombatEntity : BaseEntity
{
	[Serializable]
	public struct Pickup
	{
		public bool enabled;

		[ItemSelector(ItemCategory.All)]
		public ItemDefinition itemTarget;

		public int itemCount;

		[Tooltip("Should we set the condition of the item based on the health of the picked up entity")]
		public bool setConditionFromHealth;

		[Tooltip("How much to reduce the item condition when picking up")]
		public float subtractCondition;

		[Tooltip("Must have building access to pick up")]
		public bool requireBuildingPrivilege;

		[Tooltip("Must have hammer equipped to pick up")]
		public bool requireHammer;

		[Tooltip("Inventory Must be empty (if applicable) to be picked up")]
		public bool requireEmptyInv;
	}

	[Serializable]
	public struct Repair
	{
		public bool enabled;

		[ItemSelector(ItemCategory.All)]
		public ItemDefinition itemTarget;

		public GameObjectRef repairEffect;

		public GameObjectRef repairFullEffect;

		public GameObjectRef repairFailedEffect;
	}

	public enum ActionVolume
	{
		Quiet,
		Normal,
		Loud
	}

	public enum LifeState
	{
		Alive,
		Dead
	}

	[Serializable]
	public enum Faction
	{
		Default,
		Player,
		Bandit,
		Scientist,
		Horror
	}

	private const float MAX_HEALTH_REPAIR = 50f;

	[NonSerialized]
	public DamageType lastDamage;

	[NonSerialized]
	public BaseEntity lastAttacker;

	public BaseEntity lastDealtDamageTo;

	[NonSerialized]
	public bool ResetLifeStateOnSpawn = true;

	protected DirectionProperties[] propDirection;

	protected float unHostileTime;

	private float lastNoiseTime;

	[Header("BaseCombatEntity")]
	public SkeletonProperties skeletonProperties;

	public ProtectionProperties baseProtection;

	public float startHealth;

	public Pickup pickup;

	public Repair repair;

	public bool ShowHealthInfo = true;

	public LifeState lifestate;

	public bool sendsHitNotification;

	public bool sendsMeleeHitNotification = true;

	public bool markAttackerHostile = true;

	protected float _health;

	protected float _maxHealth = 100f;

	public Faction faction;

	[NonSerialized]
	public float lastAttackedTime = float.NegativeInfinity;

	[NonSerialized]
	public float lastDealtDamageTime = float.NegativeInfinity;

	private int lastNotifyFrame;

	public float TimeSinceLastNoise => Time.get_time() - lastNoiseTime;

	public ActionVolume LastNoiseVolume { get; private set; }

	public Vector3 LastNoisePosition { get; private set; }

	public Vector3 LastAttackedDir { get; set; }

	public float SecondsSinceAttacked => Time.get_time() - lastAttackedTime;

	public float SecondsSinceDealtDamage => Time.get_time() - lastDealtDamageTime;

	public float healthFraction => Health() / MaxHealth();

	public float health
	{
		get
		{
			return _health;
		}
		set
		{
			float num = _health;
			_health = Mathf.Clamp(value, 0f, MaxHealth());
			if (base.isServer && _health != num)
			{
				OnHealthChanged(num, _health);
			}
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("BaseCombatEntity.OnRpcMessage", 0);
		try
		{
			if (rpc == 1191093595 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_PickupStart "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_PickupStart", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(1191093595u, "RPC_PickupStart", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							RPCMessage rpc2 = rPCMessage;
							RPC_PickupStart(rpc2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_PickupStart");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	protected virtual int GetPickupCount()
	{
		return pickup.itemCount;
	}

	public virtual bool CanPickup(BasePlayer player)
	{
		if (pickup.enabled)
		{
			if (!pickup.requireBuildingPrivilege || player.CanBuild())
			{
				if (pickup.requireHammer)
				{
					return player.IsHoldingEntity<Hammer>();
				}
				return true;
			}
			return false;
		}
		return false;
	}

	public virtual void OnPickedUp(Item createdItem, BasePlayer player)
	{
	}

	public virtual void OnPickedUpPreItemMove(Item createdItem, BasePlayer player)
	{
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	private void RPC_PickupStart(RPCMessage rpc)
	{
		if (rpc.player.CanInteract() && CanPickup(rpc.player))
		{
			Item item = ItemManager.Create(pickup.itemTarget, GetPickupCount(), skinID);
			if (pickup.setConditionFromHealth && item.hasCondition)
			{
				item.conditionNormalized = Mathf.Clamp01(healthFraction - pickup.subtractCondition);
			}
			OnPickedUpPreItemMove(item, rpc.player);
			rpc.player.GiveItem(item, GiveItemReason.PickedUp);
			OnPickedUp(item, rpc.player);
			Kill();
		}
	}

	public virtual List<ItemAmount> BuildCost()
	{
		if ((Object)(object)repair.itemTarget == (Object)null)
		{
			return null;
		}
		ItemBlueprint itemBlueprint = ItemManager.FindBlueprint(repair.itemTarget);
		if ((Object)(object)itemBlueprint == (Object)null)
		{
			return null;
		}
		return itemBlueprint.ingredients;
	}

	public virtual float RepairCostFraction()
	{
		return 0.5f;
	}

	public List<ItemAmount> RepairCost(float healthMissingFraction)
	{
		List<ItemAmount> list = BuildCost();
		if (list == null)
		{
			return null;
		}
		List<ItemAmount> list2 = new List<ItemAmount>();
		foreach (ItemAmount item in list)
		{
			int num = Mathf.RoundToInt(item.amount * RepairCostFraction() * healthMissingFraction);
			if (num > 0)
			{
				list2.Add(new ItemAmount(item.itemDef, num));
			}
		}
		return list2;
	}

	public virtual void OnRepair()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Effect.server.Run(repair.repairEffect.isValid ? repair.repairEffect.resourcePath : "assets/bundled/prefabs/fx/build/repair.prefab", this, 0u, Vector3.get_zero(), Vector3.get_zero());
	}

	public virtual void OnRepairFinished()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Effect.server.Run(repair.repairFullEffect.isValid ? repair.repairFullEffect.resourcePath : "assets/bundled/prefabs/fx/build/repair_full.prefab", this, 0u, Vector3.get_zero(), Vector3.get_zero());
	}

	public virtual void OnRepairFailed(BasePlayer player, string reason)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Effect.server.Run(repair.repairFailedEffect.isValid ? repair.repairFailedEffect.resourcePath : "assets/bundled/prefabs/fx/build/repair_failed.prefab", this, 0u, Vector3.get_zero(), Vector3.get_zero());
		if ((Object)(object)player != (Object)null && !string.IsNullOrEmpty(reason))
		{
			player.ChatMessage(reason);
		}
	}

	public virtual void OnRepairFailedResources(BasePlayer player, List<ItemAmount> requirements)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Effect.server.Run(repair.repairFailedEffect.isValid ? repair.repairFailedEffect.resourcePath : "assets/bundled/prefabs/fx/build/repair_failed.prefab", this, 0u, Vector3.get_zero(), Vector3.get_zero());
		if ((Object)(object)player != (Object)null)
		{
			ItemAmountList val = ItemAmount.SerialiseList(requirements);
			try
			{
				player.ClientRPCPlayer<ItemAmountList>(null, player, "Client_OnRepairFailedResources", val);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
	}

	public virtual void DoRepair(BasePlayer player)
	{
		if (!repair.enabled)
		{
			return;
		}
		float num = 30f;
		if (SecondsSinceAttacked <= num)
		{
			OnRepairFailed(player, $"Unable to repair: Recently damaged. Repairable in: {num - SecondsSinceAttacked:N0}s.");
			return;
		}
		float num2 = MaxHealth() - Health();
		float num3 = num2 / MaxHealth();
		if (num2 <= 0f || num3 <= 0f)
		{
			OnRepairFailed(player, "Unable to repair: Not damaged.");
			return;
		}
		List<ItemAmount> list = RepairCost(num3);
		if (list == null)
		{
			return;
		}
		float num4 = Enumerable.Sum<ItemAmount>((IEnumerable<ItemAmount>)list, (Func<ItemAmount, float>)((ItemAmount x) => x.amount));
		if (num4 > 0f)
		{
			float num5 = Enumerable.Min<ItemAmount>((IEnumerable<ItemAmount>)list, (Func<ItemAmount, float>)((ItemAmount x) => Mathf.Clamp01((float)player.inventory.GetAmount(x.itemid) / x.amount)));
			num5 = Mathf.Min(num5, 50f / num2);
			if (num5 <= 0f)
			{
				OnRepairFailedResources(player, list);
				return;
			}
			int num6 = 0;
			foreach (ItemAmount item in list)
			{
				int amount = Mathf.CeilToInt(num5 * item.amount);
				int num7 = player.inventory.Take(null, item.itemid, amount);
				if (num7 > 0)
				{
					num6 += num7;
					player.Command("note.inv", item.itemid, num7 * -1);
				}
			}
			float num8 = (float)num6 / num4;
			health += num2 * num8;
			SendNetworkUpdate();
		}
		else
		{
			health += num2;
			SendNetworkUpdate();
		}
		if (Health() >= MaxHealth())
		{
			OnRepairFinished();
		}
		else
		{
			OnRepair();
		}
	}

	public virtual void InitializeHealth(float newhealth, float newmax)
	{
		_maxHealth = newmax;
		_health = newhealth;
		lifestate = LifeState.Alive;
	}

	public override void ServerInit()
	{
		propDirection = PrefabAttribute.server.FindAll<DirectionProperties>(prefabID);
		if (ResetLifeStateOnSpawn)
		{
			InitializeHealth(StartHealth(), StartMaxHealth());
			lifestate = LifeState.Alive;
		}
		base.ServerInit();
	}

	public virtual void OnHealthChanged(float oldvalue, float newvalue)
	{
	}

	public void Hurt(float amount)
	{
		Hurt(Mathf.Abs(amount), DamageType.Generic);
	}

	public void Hurt(float amount, DamageType type, BaseEntity attacker = null, bool useProtection = true)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("Hurt", 0);
		try
		{
			HitInfo hitInfo = new HitInfo(attacker, this, type, amount, ((Component)this).get_transform().get_position());
			hitInfo.UseProtection = useProtection;
			Hurt(hitInfo);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public virtual void Hurt(HitInfo info)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		Assert.IsTrue(base.isServer, "This should be called serverside only");
		if (IsDead())
		{
			return;
		}
		TimeWarning val = TimeWarning.New("Hurt( HitInfo )", 50);
		try
		{
			float num = health;
			ScaleDamage(info);
			if (info.PointStart != Vector3.get_zero())
			{
				for (int i = 0; i < propDirection.Length; i++)
				{
					if (!((Object)(object)propDirection[i].extraProtection == (Object)null) && !propDirection[i].IsWeakspot(((Component)this).get_transform(), info))
					{
						propDirection[i].extraProtection.Scale(info.damageTypes);
					}
				}
			}
			info.damageTypes.Scale(DamageType.Arrow, ConVar.Server.arrowdamage);
			info.damageTypes.Scale(DamageType.Bullet, ConVar.Server.bulletdamage);
			info.damageTypes.Scale(DamageType.Slash, ConVar.Server.meleedamage);
			info.damageTypes.Scale(DamageType.Blunt, ConVar.Server.meleedamage);
			info.damageTypes.Scale(DamageType.Stab, ConVar.Server.meleedamage);
			info.damageTypes.Scale(DamageType.Bleeding, ConVar.Server.bleedingdamage);
			if (!(this is BasePlayer))
			{
				info.damageTypes.Scale(DamageType.Fun_Water, 0f);
			}
			DebugHurt(info);
			health = num - info.damageTypes.Total();
			SendNetworkUpdate();
			if (Global.developer > 1)
			{
				Debug.Log((object)string.Concat("[Combat]".PadRight(10), ((Object)((Component)this).get_gameObject()).get_name(), " hurt ", info.damageTypes.GetMajorityDamageType(), "/", info.damageTypes.Total(), " - ", health.ToString("0"), " health left"));
			}
			lastDamage = info.damageTypes.GetMajorityDamageType();
			lastAttacker = info.Initiator;
			if ((Object)(object)lastAttacker != (Object)null)
			{
				BaseCombatEntity baseCombatEntity = lastAttacker as BaseCombatEntity;
				if ((Object)(object)baseCombatEntity != (Object)null)
				{
					baseCombatEntity.lastDealtDamageTime = Time.get_time();
					baseCombatEntity.lastDealtDamageTo = this;
				}
			}
			BaseCombatEntity baseCombatEntity2 = lastAttacker as BaseCombatEntity;
			if (markAttackerHostile && (Object)(object)baseCombatEntity2 != (Object)null && (Object)(object)baseCombatEntity2 != (Object)(object)this)
			{
				baseCombatEntity2.MarkHostileFor();
			}
			if (lastDamage.IsConsideredAnAttack())
			{
				lastAttackedTime = Time.get_time();
				if ((Object)(object)lastAttacker != (Object)null)
				{
					Vector3 val2 = ((Component)lastAttacker).get_transform().get_position() - ((Component)this).get_transform().get_position();
					LastAttackedDir = ((Vector3)(ref val2)).get_normalized();
				}
			}
			if (Health() <= 0f)
			{
				Die(info);
			}
			BasePlayer initiatorPlayer = info.InitiatorPlayer;
			if (Object.op_Implicit((Object)(object)initiatorPlayer))
			{
				if (IsDead())
				{
					initiatorPlayer.stats.combat.Log(info, num, health, "killed");
				}
				else
				{
					initiatorPlayer.stats.combat.Log(info, num, health);
				}
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public virtual bool IsHostile()
	{
		return unHostileTime > Time.get_realtimeSinceStartup();
	}

	public virtual void MarkHostileFor(float duration = 60f)
	{
		float num = Time.get_realtimeSinceStartup() + duration;
		unHostileTime = Mathf.Max(unHostileTime, num);
	}

	private void DebugHurt(HitInfo info)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		if (!ConVar.Vis.damage)
		{
			return;
		}
		if (info.PointStart != info.PointEnd)
		{
			ConsoleNetwork.BroadcastToAllClients("ddraw.arrow", 60, Color.get_cyan(), info.PointStart, info.PointEnd, 0.1f);
			ConsoleNetwork.BroadcastToAllClients("ddraw.sphere", 60, Color.get_cyan(), info.HitPositionWorld, 0.01f);
		}
		string text = "";
		for (int i = 0; i < info.damageTypes.types.Length; i++)
		{
			float num = info.damageTypes.types[i];
			if (num != 0f)
			{
				string[] obj = new string[5] { text, " ", null, null, null };
				DamageType damageType = (DamageType)i;
				obj[2] = damageType.ToString().PadRight(10);
				obj[3] = num.ToString("0.00");
				obj[4] = "\n";
				text = string.Concat(obj);
			}
		}
		string text2 = string.Concat("<color=lightblue>Damage:</color>".PadRight(10), info.damageTypes.Total().ToString("0.00"), "\n<color=lightblue>Health:</color>".PadRight(10), health.ToString("0.00"), " / ", (health - info.damageTypes.Total() <= 0f) ? "<color=red>" : "<color=green>", (health - info.damageTypes.Total()).ToString("0.00"), "</color>", "\n<color=lightblue>HitEnt:</color>".PadRight(10), this, "\n<color=lightblue>HitBone:</color>".PadRight(10), info.boneName, "\n<color=lightblue>Attacker:</color>".PadRight(10), info.Initiator, "\n<color=lightblue>WeaponPrefab:</color>".PadRight(10), info.WeaponPrefab, "\n<color=lightblue>Damages:</color>\n", text);
		ConsoleNetwork.BroadcastToAllClients("ddraw.text", 60, Color.get_white(), info.HitPositionWorld, text2);
	}

	public void SetHealth(float hp)
	{
		if (health != hp)
		{
			health = hp;
			SendNetworkUpdate();
		}
	}

	public virtual void Heal(float amount)
	{
		if (Global.developer > 1)
		{
			Debug.Log((object)("[Combat]".PadRight(10) + ((Object)((Component)this).get_gameObject()).get_name() + " healed"));
		}
		health = _health + amount;
		SendNetworkUpdate();
	}

	public virtual void OnKilled(HitInfo info)
	{
		Kill(DestroyMode.Gib);
	}

	public virtual void Die(HitInfo info = null)
	{
		if (IsDead())
		{
			return;
		}
		if (Global.developer > 1)
		{
			Debug.Log((object)("[Combat]".PadRight(10) + ((Object)((Component)this).get_gameObject()).get_name() + " died"));
		}
		health = 0f;
		lifestate = LifeState.Dead;
		if (info != null && Object.op_Implicit((Object)(object)info.InitiatorPlayer))
		{
			BasePlayer initiatorPlayer = info.InitiatorPlayer;
			if ((Object)(object)initiatorPlayer != (Object)null && initiatorPlayer.GetActiveMission() != -1 && !initiatorPlayer.IsNpc)
			{
				initiatorPlayer.ProcessMissionEvent(BaseMission.MissionEventType.KILL_ENTITY, prefabID.ToString(), 1f);
			}
		}
		TimeWarning val = TimeWarning.New("OnKilled", 0);
		try
		{
			OnKilled(info);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void DieInstantly()
	{
		if (!IsDead())
		{
			if (Global.developer > 1)
			{
				Debug.Log((object)("[Combat]".PadRight(10) + ((Object)((Component)this).get_gameObject()).get_name() + " died"));
			}
			health = 0f;
			lifestate = LifeState.Dead;
			OnKilled(null);
		}
	}

	public void UpdateSurroundings()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		StabilityEntity.UpdateSurroundingsQueue updateSurroundingsQueue = StabilityEntity.updateSurroundingsQueue;
		OBB val = WorldSpaceBounds();
		((ObjectWorkQueue<Bounds>)updateSurroundingsQueue).Add(((OBB)(ref val)).ToBounds());
	}

	public void MakeNoise(Vector3 position, ActionVolume loudness)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		LastNoisePosition = position;
		LastNoiseVolume = loudness;
		lastNoiseTime = Time.get_time();
	}

	public bool CanLastNoiseBeHeard(Vector3 listenPosition, float listenRange)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (listenRange <= 0f)
		{
			return false;
		}
		return Vector3.Distance(listenPosition, LastNoisePosition) <= listenRange;
	}

	public virtual bool IsDead()
	{
		return lifestate == LifeState.Dead;
	}

	public virtual bool IsAlive()
	{
		return lifestate == LifeState.Alive;
	}

	public Faction GetFaction()
	{
		return faction;
	}

	public virtual bool IsFriendly(BaseCombatEntity other)
	{
		return false;
	}

	public override void ResetState()
	{
		base.ResetState();
		health = MaxHealth();
		if (base.isServer)
		{
			lastAttackedTime = float.NegativeInfinity;
			lastDealtDamageTime = float.NegativeInfinity;
		}
	}

	public override void DestroyShared()
	{
		base.DestroyShared();
		if (base.isServer)
		{
			UpdateSurroundings();
		}
	}

	public virtual float GetThreatLevel()
	{
		return 0f;
	}

	public override float PenetrationResistance(HitInfo info)
	{
		if (!Object.op_Implicit((Object)(object)baseProtection))
		{
			return 100f;
		}
		return baseProtection.density;
	}

	public virtual void ScaleDamage(HitInfo info)
	{
		if (info.UseProtection && (Object)(object)baseProtection != (Object)null)
		{
			baseProtection.Scale(info.damageTypes);
		}
	}

	public HitArea SkeletonLookup(uint boneID)
	{
		if ((Object)(object)skeletonProperties == (Object)null)
		{
			return (HitArea)(-1);
		}
		return skeletonProperties.FindBone(boneID)?.area ?? ((HitArea)(-1));
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.baseCombat = Pool.Get<BaseCombat>();
		info.msg.baseCombat.state = (int)lifestate;
		info.msg.baseCombat.health = Health();
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (Health() > MaxHealth())
		{
			health = MaxHealth();
		}
		if (float.IsNaN(Health()))
		{
			health = MaxHealth();
		}
	}

	public override void Load(LoadInfo info)
	{
		if (base.isServer)
		{
			lifestate = LifeState.Alive;
		}
		if (info.msg.baseCombat != null)
		{
			lifestate = (LifeState)info.msg.baseCombat.state;
			_health = info.msg.baseCombat.health;
		}
		base.Load(info);
	}

	public override float Health()
	{
		return _health;
	}

	public override float MaxHealth()
	{
		return _maxHealth;
	}

	public virtual float StartHealth()
	{
		return startHealth;
	}

	public virtual float StartMaxHealth()
	{
		return StartHealth();
	}

	public void SetMaxHealth(float newMax)
	{
		_maxHealth = newMax;
		_health = Mathf.Min(_health, newMax);
	}

	public void DoHitNotify(HitInfo info)
	{
		TimeWarning val = TimeWarning.New("DoHitNotify", 0);
		try
		{
			if (sendsHitNotification && !((Object)(object)info.Initiator == (Object)null) && info.Initiator is BasePlayer && !info.isHeadshot && !((Object)(object)this == (Object)(object)info.Initiator) && Time.get_frameCount() != lastNotifyFrame)
			{
				lastNotifyFrame = Time.get_frameCount();
				bool flag = info.Weapon is BaseMelee;
				if (base.isServer && (!flag || sendsMeleeHitNotification))
				{
					bool arg = info.Initiator.net.get_connection() == info.Predicted;
					ClientRPCPlayer(null, info.Initiator as BasePlayer, "HitNotify", arg);
				}
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public override void OnAttacked(HitInfo info)
	{
		TimeWarning val = TimeWarning.New("BaseCombatEntity.OnAttacked", 0);
		try
		{
			if (!IsDead())
			{
				DoHitNotify(info);
			}
			if (base.isServer)
			{
				Hurt(info);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		base.OnAttacked(info);
	}
}
