using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class SamSite : ContainerIOEntity
{
	public interface ISamSiteTarget
	{
		SamTargetType SAMTargetType { get; }

		bool isClient { get; }

		bool IsValidSAMTarget(bool staticRespawn);

		Vector3 CenterPoint();

		Vector3 GetWorldVelocity();

		bool IsVisible(Vector3 position, float maxDistance = float.PositiveInfinity);
	}

	public class SamTargetType
	{
		public readonly float scanRadius;

		public readonly float speedMultiplier;

		public readonly float timeBetweenBursts;

		public SamTargetType(float scanRadius, float speedMultiplier, float timeBetweenBursts)
		{
			this.scanRadius = scanRadius;
			this.speedMultiplier = speedMultiplier;
			this.timeBetweenBursts = timeBetweenBursts;
		}
	}

	public Animator pitchAnimator;

	public GameObject yaw;

	public GameObject pitch;

	public GameObject gear;

	public Transform eyePoint;

	public float gearEpislonDegrees = 20f;

	public float turnSpeed = 1f;

	public float clientLerpSpeed = 1f;

	public Vector3 currentAimDir = Vector3.get_forward();

	public Vector3 targetAimDir = Vector3.get_forward();

	public float vehicleScanRadius = 350f;

	public float missileScanRadius = 500f;

	public GameObjectRef projectileTest;

	public GameObjectRef muzzleFlashTest;

	public bool staticRespawn;

	public ItemDefinition ammoType;

	public Transform[] tubes;

	[ServerVar(Help = "how long until static sam sites auto repair")]
	public static float staticrepairseconds = 1200f;

	public SoundDefinition yawMovementLoopDef;

	public float yawGainLerp = 8f;

	public float yawGainMovementSpeedMult = 0.1f;

	public SoundDefinition pitchMovementLoopDef;

	public float pitchGainLerp = 10f;

	public float pitchGainMovementSpeedMult = 0.5f;

	public int lowAmmoThreshold = 5;

	public Flags Flag_DefenderMode = Flags.Reserved9;

	public static SamTargetType targetTypeUnknown;

	public static SamTargetType targetTypeVehicle;

	public static SamTargetType targetTypeMissile;

	private ISamSiteTarget currentTarget;

	private SamTargetType mostRecentTargetType;

	private Item ammoItem;

	private float lockOnTime;

	private float lastTargetVisibleTime;

	private int lastAmmoCount;

	private int currentTubeIndex;

	private int firedCount;

	private float nextBurstTime;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("SamSite.OnRpcMessage", 0);
		try
		{
			if (rpc == 3160662357u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ToggleDefenderMode "));
				}
				TimeWarning val2 = TimeWarning.New("ToggleDefenderMode", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(3160662357u, "ToggleDefenderMode", this, player, 1uL))
						{
							return true;
						}
						if (!RPC_Server.IsVisible.Test(3160662357u, "ToggleDefenderMode", this, player, 3f))
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
							RPCMessage msg2 = rPCMessage;
							ToggleDefenderMode(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in ToggleDefenderMode");
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

	public override bool IsPowered()
	{
		if (!staticRespawn)
		{
			return HasFlag(Flags.Reserved8);
		}
		return true;
	}

	public override int ConsumptionAmount()
	{
		return 25;
	}

	public bool IsInDefenderMode()
	{
		return HasFlag(Flag_DefenderMode);
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
	}

	private void SetTarget(ISamSiteTarget target)
	{
		bool num = currentTarget != target;
		currentTarget = target;
		if (!target.IsUnityNull())
		{
			mostRecentTargetType = target.SAMTargetType;
		}
		if (num)
		{
			MarkIODirty();
		}
	}

	private void MarkIODirty()
	{
		if (!staticRespawn)
		{
			lastPassthroughEnergy = -1;
			MarkDirtyForceUpdateOutputs();
		}
	}

	private void ClearTarget()
	{
		SetTarget(null);
	}

	public override void ServerInit()
	{
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		targetTypeUnknown = new SamTargetType(vehicleScanRadius, 1f, 5f);
		targetTypeVehicle = new SamTargetType(vehicleScanRadius, 1f, 5f);
		targetTypeMissile = new SamTargetType(missileScanRadius, 2.25f, 3.5f);
		mostRecentTargetType = targetTypeUnknown;
		ClearTarget();
		((FacepunchBehaviour)this).InvokeRandomized((Action)TargetScan, 1f, 3f, 1f);
		currentAimDir = ((Component)this).get_transform().get_forward();
		if (base.inventory != null && !staticRespawn)
		{
			base.inventory.onItemAddedRemoved = OnItemAddedRemoved;
		}
	}

	private void OnItemAddedRemoved(Item arg1, bool arg2)
	{
		EnsureReloaded();
		if (IsPowered())
		{
			MarkIODirty();
		}
	}

	public override void Save(SaveInfo info)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		info.msg.samSite = Pool.Get<SAMSite>();
		info.msg.samSite.aimDir = GetAimDir();
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (staticRespawn && HasFlag(Flags.Reserved1))
		{
			((FacepunchBehaviour)this).Invoke((Action)SelfHeal, staticrepairseconds);
		}
	}

	public void SelfHeal()
	{
		lifestate = LifeState.Alive;
		base.health = startHealth;
		SetFlag(Flags.Reserved1, b: false);
	}

	public override void Die(HitInfo info = null)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (staticRespawn)
		{
			ClearTarget();
			Quaternion val = Quaternion.LookRotation(currentAimDir, Vector3.get_up());
			val = Quaternion.Euler(0f, ((Quaternion)(ref val)).get_eulerAngles().y, 0f);
			currentAimDir = val * Vector3.get_forward();
			((FacepunchBehaviour)this).Invoke((Action)SelfHeal, staticrepairseconds);
			lifestate = LifeState.Dead;
			base.health = 0f;
			SetFlag(Flags.Reserved1, b: true);
		}
		else
		{
			base.Die(info);
		}
	}

	public void FixedUpdate()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = currentAimDir;
		if (!currentTarget.IsUnityNull() && IsPowered())
		{
			float num = projectileTest.Get().GetComponent<ServerProjectile>().speed * currentTarget.SAMTargetType.speedMultiplier;
			Vector3 val2 = currentTarget.CenterPoint();
			float num2 = Vector3.Distance(val2, ((Component)eyePoint).get_transform().get_position());
			float num3 = num2 / num;
			Vector3 val3 = val2 + currentTarget.GetWorldVelocity() * num3;
			num3 = Vector3.Distance(val3, ((Component)eyePoint).get_transform().get_position()) / num;
			val3 = val2 + currentTarget.GetWorldVelocity() * num3;
			Vector3 val4 = currentTarget.GetWorldVelocity();
			if (((Vector3)(ref val4)).get_magnitude() > 0.1f)
			{
				float num4 = Mathf.Sin(Time.get_time() * 3f) * (1f + num3 * 0.5f);
				Vector3 val5 = val3;
				val4 = currentTarget.GetWorldVelocity();
				val3 = val5 + ((Vector3)(ref val4)).get_normalized() * num4;
			}
			val4 = val3 - ((Component)eyePoint).get_transform().get_position();
			currentAimDir = ((Vector3)(ref val4)).get_normalized();
			if (num2 > currentTarget.SAMTargetType.scanRadius)
			{
				ClearTarget();
			}
		}
		Quaternion val6 = Quaternion.LookRotation(currentAimDir, ((Component)this).get_transform().get_up());
		Vector3 eulerAngles = ((Quaternion)(ref val6)).get_eulerAngles();
		eulerAngles = BaseMountable.ConvertVector(eulerAngles);
		float num5 = Mathf.InverseLerp(0f, 90f, 0f - eulerAngles.x);
		float num6 = Mathf.Lerp(15f, -75f, num5);
		Quaternion localRotation = Quaternion.Euler(0f, eulerAngles.y, 0f);
		yaw.get_transform().set_localRotation(localRotation);
		Quaternion localRotation2 = pitch.get_transform().get_localRotation();
		float x = ((Quaternion)(ref localRotation2)).get_eulerAngles().x;
		localRotation2 = pitch.get_transform().get_localRotation();
		Quaternion localRotation3 = Quaternion.Euler(x, ((Quaternion)(ref localRotation2)).get_eulerAngles().y, num6);
		pitch.get_transform().set_localRotation(localRotation3);
		if (currentAimDir != val)
		{
			SendNetworkUpdate();
		}
	}

	public Vector3 GetAimDir()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return currentAimDir;
	}

	public bool HasValidTarget()
	{
		return !currentTarget.IsUnityNull();
	}

	public override bool CanPickup(BasePlayer player)
	{
		if (!base.CanPickup(player))
		{
			return false;
		}
		if (base.isServer && pickup.requireEmptyInv && base.inventory != null && base.inventory.itemList.Count > 0)
		{
			return false;
		}
		return !HasAmmo();
	}

	public void TargetScan()
	{
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		if (!IsPowered())
		{
			lastTargetVisibleTime = 0f;
			return;
		}
		if (Time.get_time() > lastTargetVisibleTime + 3f)
		{
			ClearTarget();
		}
		if (!staticRespawn)
		{
			int num = ((ammoItem != null && ammoItem.parent == base.inventory) ? ammoItem.amount : 0);
			bool flag = lastAmmoCount < lowAmmoThreshold;
			bool flag2 = num < lowAmmoThreshold;
			if (num != lastAmmoCount && flag != flag2)
			{
				MarkIODirty();
			}
			lastAmmoCount = num;
		}
		if (HasValidTarget() || IsDead())
		{
			return;
		}
		List<ISamSiteTarget> list = Pool.GetList<ISamSiteTarget>();
		if (!IsInDefenderMode())
		{
			AddTargetSet(list, 32768, targetTypeVehicle.scanRadius);
		}
		AddTargetSet(list, 1048576, targetTypeMissile.scanRadius);
		ISamSiteTarget samSiteTarget = null;
		foreach (ISamSiteTarget item in list)
		{
			if (!item.isClient && !(item.CenterPoint().y < ((Component)eyePoint).get_transform().get_position().y) && item.IsVisible(((Component)eyePoint).get_transform().get_position(), item.SAMTargetType.scanRadius * 2f) && item.IsValidSAMTarget(staticRespawn))
			{
				samSiteTarget = item;
				break;
			}
		}
		if (!samSiteTarget.IsUnityNull() && currentTarget != samSiteTarget)
		{
			lockOnTime = Time.get_time() + 0.5f;
		}
		SetTarget(samSiteTarget);
		if (!currentTarget.IsUnityNull())
		{
			lastTargetVisibleTime = Time.get_time();
		}
		Pool.FreeList<ISamSiteTarget>(ref list);
		if (currentTarget.IsUnityNull())
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)WeaponTick);
		}
		else
		{
			((FacepunchBehaviour)this).InvokeRandomized((Action)WeaponTick, 0f, 0.5f, 0.2f);
		}
		void AddTargetSet(List<ISamSiteTarget> allTargets, int layerMask, float scanRadius)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			List<ISamSiteTarget> list2 = Pool.GetList<ISamSiteTarget>();
			Vis.Entities(((Component)eyePoint).get_transform().get_position(), scanRadius, list2, layerMask, (QueryTriggerInteraction)1);
			allTargets.AddRange(list2);
			Pool.FreeList<ISamSiteTarget>(ref list2);
		}
	}

	public virtual bool HasAmmo()
	{
		if (!staticRespawn)
		{
			if (ammoItem != null && ammoItem.amount > 0)
			{
				return ammoItem.parent == base.inventory;
			}
			return false;
		}
		return true;
	}

	public void Reload()
	{
		if (staticRespawn)
		{
			return;
		}
		for (int i = 0; i < base.inventory.itemList.Count; i++)
		{
			Item item = base.inventory.itemList[i];
			if (item != null && item.info.itemid == ammoType.itemid && item.amount > 0)
			{
				ammoItem = item;
				return;
			}
		}
		ammoItem = null;
	}

	public void EnsureReloaded()
	{
		if (!HasAmmo())
		{
			Reload();
		}
	}

	public bool IsReloading()
	{
		return ((FacepunchBehaviour)this).IsInvoking((Action)Reload);
	}

	public void WeaponTick()
	{
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		if (IsDead() || Time.get_time() < lockOnTime || Time.get_time() < nextBurstTime)
		{
			return;
		}
		if (!IsPowered())
		{
			firedCount = 0;
			return;
		}
		if (firedCount >= 6)
		{
			float timeBetweenBursts = mostRecentTargetType.timeBetweenBursts;
			nextBurstTime = Time.get_time() + timeBetweenBursts;
			firedCount = 0;
			return;
		}
		EnsureReloaded();
		if (HasAmmo())
		{
			bool num = ammoItem != null && ammoItem.amount == lowAmmoThreshold;
			if (!staticRespawn && ammoItem != null)
			{
				ammoItem.UseItem();
			}
			firedCount++;
			float speedMultiplier = 1f;
			if (!currentTarget.IsUnityNull())
			{
				speedMultiplier = currentTarget.SAMTargetType.speedMultiplier;
			}
			FireProjectile(tubes[currentTubeIndex].get_position(), currentAimDir, speedMultiplier);
			Effect.server.Run(muzzleFlashTest.resourcePath, this, StringPool.Get("Tube " + (currentTubeIndex + 1)), Vector3.get_zero(), Vector3.get_up());
			currentTubeIndex++;
			if (currentTubeIndex >= tubes.Length)
			{
				currentTubeIndex = 0;
			}
			if (num)
			{
				MarkIODirty();
			}
		}
	}

	public void FireProjectile(Vector3 origin, Vector3 direction, float speedMultiplier)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		BaseEntity baseEntity = GameManager.server.CreateEntity(projectileTest.resourcePath, origin, Quaternion.LookRotation(direction, Vector3.get_up()));
		if (!((Object)(object)baseEntity == (Object)null))
		{
			baseEntity.creatorEntity = this;
			ServerProjectile component = ((Component)baseEntity).GetComponent<ServerProjectile>();
			if (Object.op_Implicit((Object)(object)component))
			{
				component.InitializeVelocity(GetInheritedProjectileVelocity() + direction * component.speed * speedMultiplier);
			}
			baseEntity.Spawn();
		}
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		int result = Mathf.Min(1, GetCurrentEnergy());
		switch (outputSlot)
		{
		case 0:
			if (currentTarget.IsUnityNull())
			{
				return 0;
			}
			return result;
		case 1:
			if (ammoItem == null || ammoItem.amount >= lowAmmoThreshold || ammoItem.parent != base.inventory)
			{
				return 0;
			}
			return result;
		case 2:
			if (HasAmmo())
			{
				return 0;
			}
			return result;
		default:
			return 0;
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	[RPC_Server.CallsPerSecond(1uL)]
	private void ToggleDefenderMode(RPCMessage msg)
	{
		if (staticRespawn)
		{
			return;
		}
		BasePlayer player = msg.player;
		if (!((Object)(object)player == (Object)null) && player.CanBuild())
		{
			bool flag = msg.read.Bit();
			if (flag != IsInDefenderMode())
			{
				SetFlag(Flag_DefenderMode, flag);
			}
		}
	}
}
