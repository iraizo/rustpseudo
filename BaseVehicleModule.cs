using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using Rust.Modular;
using UnityEngine;
using UnityEngine.Assertions;

public class BaseVehicleModule : BaseVehicle, IPrefabPreProcess
{
	public enum VisualGroup
	{
		None,
		Engine,
		Cabin,
		Flatbed
	}

	[Serializable]
	public class LODLevel
	{
		public Renderer[] renderers;
	}

	public Item AssociatedItemInstance;

	private TimeSince timeSinceItemLockRefresh;

	private const float TIME_BETWEEN_LOCK_REFRESH = 1f;

	[Header("Vehicle Module")]
	[SerializeField]
	private Transform centreOfMassTransform;

	[SerializeField]
	private float mass = 100f;

	public VisualGroup visualGroup;

	[SerializeField]
	[HideInInspector]
	private VehicleLight[] lights;

	public LODLevel[] lodRenderers;

	[SerializeField]
	[HideInInspector]
	private List<ConditionalObject> conditionals;

	[Header("Trigger Parent")]
	[SerializeField]
	private TriggerParent[] triggerParents;

	[Header("Sliding Components")]
	[SerializeField]
	private VehicleModuleSlidingComponent[] slidingComponents;

	[SerializeField]
	private VehicleModuleButtonComponent[] buttonComponents;

	private TimeSince TimeSinceAddedToVehicle;

	private float prevRefreshHealth = -1f;

	private bool prevRefreshVehicleIsDead;

	private bool prevRefreshVehicleIsLockable;

	public bool PropagateDamage { get; private set; } = true;


	public BaseModularVehicle Vehicle { get; private set; }

	public int FirstSocketIndex { get; private set; } = -1;


	public Vector3 CentreOfMass => centreOfMassTransform.get_localPosition();

	public float Mass => mass;

	public uint ID => net.ID;

	public bool IsOnAVehicle => (Object)(object)Vehicle != (Object)null;

	public ItemDefinition AssociatedItemDef => repair.itemTarget;

	public virtual bool HasSeating => false;

	public virtual bool HasAnEngine => false;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("BaseVehicleModule.OnRpcMessage", 0);
		try
		{
			if (rpc == 2683376664u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_Use "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_Use", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(2683376664u, "RPC_Use", this, player, 3f))
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
							RPC_Use(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_Use");
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

	public virtual void NonUserSpawn()
	{
	}

	public override void VehicleFixedUpdate()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if (isSpawned && IsOnAVehicle)
		{
			base.VehicleFixedUpdate();
			if (Vehicle.IsEditableNow && AssociatedItemInstance != null && TimeSince.op_Implicit(timeSinceItemLockRefresh) > 1f)
			{
				AssociatedItemInstance.LockUnlock(!CanBeMovedNow());
				timeSinceItemLockRefresh = TimeSince.op_Implicit(0f);
			}
			for (int i = 0; i < slidingComponents.Length; i++)
			{
				slidingComponents[i].ServerUpdateTick(this);
			}
		}
	}

	public override void Hurt(HitInfo info)
	{
		if (IsOnAVehicle)
		{
			Vehicle.ModuleHurt(this, info);
		}
		base.Hurt(info);
	}

	public override void OnHealthChanged(float oldValue, float newValue)
	{
		base.OnHealthChanged(oldValue, newValue);
		if (!base.isServer)
		{
			return;
		}
		if (IsOnAVehicle)
		{
			if (Vehicle.IsDead())
			{
				return;
			}
			if (AssociatedItemInstance != null)
			{
				AssociatedItemInstance.condition = Health();
			}
			if (newValue <= 0f)
			{
				Vehicle.ModuleReachedZeroHealth();
			}
		}
		RefreshConditionals(canGib: true);
	}

	public bool CanBeMovedNow()
	{
		if (IsOnAVehicle)
		{
			return CanBeMovedNowOnVehicle();
		}
		return true;
	}

	protected virtual bool CanBeMovedNowOnVehicle()
	{
		return true;
	}

	public virtual float GetAdjustedDriveForce(float absSpeed, float topSpeed)
	{
		return 0f;
	}

	public void AcceptPropagatedDamage(float amount, DamageType type, BaseEntity attacker = null, bool useProtection = true)
	{
		PropagateDamage = false;
		Hurt(amount, type, attacker, useProtection);
		PropagateDamage = true;
	}

	public virtual void AdminFixUp(int tier)
	{
		SetHealth(MaxHealth());
		SendNetworkUpdate();
	}

	public override void Die(HitInfo info = null)
	{
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void RPC_Use(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!CanBeUsedNowBy(player))
		{
			return;
		}
		string lookingAtColldierName = msg.read.String(256);
		VehicleModuleSlidingComponent[] array = slidingComponents;
		foreach (VehicleModuleSlidingComponent vehicleModuleSlidingComponent in array)
		{
			if (PlayerIsLookingAtUsable(lookingAtColldierName, vehicleModuleSlidingComponent.interactionColliderName))
			{
				vehicleModuleSlidingComponent.Use(this);
				break;
			}
		}
		VehicleModuleButtonComponent[] array2 = buttonComponents;
		foreach (VehicleModuleButtonComponent vehicleModuleButtonComponent in array2)
		{
			if (PlayerIsLookingAtUsable(lookingAtColldierName, vehicleModuleButtonComponent.interactionColliderName))
			{
				vehicleModuleButtonComponent.ServerUse(player, this);
				break;
			}
		}
	}

	public override void AdminKill()
	{
		if (IsOnAVehicle)
		{
			Vehicle.AdminKill();
		}
	}

	public virtual void OnPlayerDismountedVehicle(BasePlayer player)
	{
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.vehicleModule = Pool.Get<VehicleModule>();
		info.msg.vehicleModule.socketIndex = FirstSocketIndex;
	}

	public override void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(process, rootObj, name, serverside, clientside, bundling);
		damageRenderer = ((Component)this).GetComponent<DamageRenderer>();
		RefreshParameters();
		lights = ((Component)this).GetComponentsInChildren<VehicleLight>();
	}

	public void RefreshParameters()
	{
		for (int num = conditionals.Count - 1; num >= 0; num--)
		{
			ConditionalObject conditionalObject = conditionals[num];
			if ((Object)(object)conditionalObject.gameObject == (Object)null)
			{
				conditionals.RemoveAt(num);
			}
			else if (conditionalObject.restrictOnHealth)
			{
				conditionalObject.healthRestrictionMin = Mathf.Clamp01(conditionalObject.healthRestrictionMin);
				conditionalObject.healthRestrictionMax = Mathf.Clamp01(conditionalObject.healthRestrictionMax);
			}
			if ((Object)(object)conditionalObject.gameObject != (Object)null)
			{
				Gibbable component = conditionalObject.gameObject.GetComponent<Gibbable>();
				if (component != null)
				{
					component.isConditional = true;
				}
			}
		}
	}

	public virtual void ModuleAdded(BaseModularVehicle vehicle, int firstSocketIndex)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Vehicle = vehicle;
		FirstSocketIndex = firstSocketIndex;
		TimeSinceAddedToVehicle = TimeSince.op_Implicit(0f);
		if (base.isServer)
		{
			TriggerParent[] array = triggerParents;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].associatedMountable = vehicle;
			}
			SendNetworkUpdate();
		}
		RefreshConditionals(canGib: false);
	}

	public virtual void ModuleRemoved()
	{
		Vehicle = null;
		FirstSocketIndex = -1;
		if (base.isServer)
		{
			TriggerParent[] array = triggerParents;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].associatedMountable = null;
			}
			SendNetworkUpdate();
		}
	}

	public void OtherVehicleModulesChanged()
	{
		RefreshConditionals(canGib: false);
	}

	public virtual void OnEngineStateChanged(VehicleEngineController<ModularCar>.EngineState oldState, VehicleEngineController<ModularCar>.EngineState newState)
	{
	}

	public override float MaxHealth()
	{
		if ((Object)(object)AssociatedItemDef != (Object)null)
		{
			return AssociatedItemDef.condition.max;
		}
		return base.MaxHealth();
	}

	public override float StartHealth()
	{
		return MaxHealth();
	}

	public int GetNumSocketsTaken()
	{
		if ((Object)(object)AssociatedItemDef == (Object)null)
		{
			return 1;
		}
		return ((Component)AssociatedItemDef).GetComponent<ItemModVehicleModule>().socketsTaken;
	}

	public List<ConditionalObject> GetConditionals()
	{
		List<ConditionalObject> list = new List<ConditionalObject>();
		foreach (ConditionalObject conditional in conditionals)
		{
			if ((Object)(object)conditional.gameObject != (Object)null)
			{
				list.Add(conditional);
			}
		}
		return list;
	}

	public virtual float GetMaxDriveForce()
	{
		return 0f;
	}

	public void RefreshConditionals(bool canGib)
	{
		if (base.IsDestroyed || !IsOnAVehicle || !Vehicle.HasInited)
		{
			return;
		}
		foreach (ConditionalObject conditional in conditionals)
		{
			RefreshConditional(conditional, canGib);
		}
		prevRefreshHealth = Health();
		prevRefreshVehicleIsDead = Vehicle.IsDead();
		prevRefreshVehicleIsLockable = Vehicle.IsLockable;
		PostConditionalRefresh();
	}

	protected virtual void PostConditionalRefresh()
	{
	}

	private void RefreshConditional(ConditionalObject conditional, bool canGib)
	{
		if (conditional == null || (Object)(object)conditional.gameObject == (Object)null)
		{
			return;
		}
		bool flag = true;
		if (conditional.restrictOnHealth)
		{
			flag = ((!Mathf.Approximately(conditional.healthRestrictionMin, conditional.healthRestrictionMax)) ? (base.healthFraction > conditional.healthRestrictionMin && base.healthFraction <= conditional.healthRestrictionMax) : Mathf.Approximately(base.healthFraction, conditional.healthRestrictionMin));
			if (!canGib)
			{
			}
		}
		if (flag && IsOnAVehicle && conditional.restrictOnLockable)
		{
			flag = Vehicle.IsLockable == conditional.lockableRestriction;
		}
		if (flag && conditional.restrictOnAdjacent)
		{
			bool flag2 = false;
			bool flag3 = false;
			if (TryGetAdjacentModuleInFront(out var result))
			{
				flag2 = InSameVisualGroupAs(result, conditional.adjacentMatch);
			}
			if (TryGetAdjacentModuleBehind(out result))
			{
				flag3 = InSameVisualGroupAs(result, conditional.adjacentMatch);
			}
			switch (conditional.adjacentRestriction)
			{
			case ConditionalObject.AdjacentCondition.BothDifferent:
				flag = !flag2 && !flag3;
				break;
			case ConditionalObject.AdjacentCondition.SameInFront:
				flag = flag2;
				break;
			case ConditionalObject.AdjacentCondition.SameBehind:
				flag = flag3;
				break;
			case ConditionalObject.AdjacentCondition.DifferentInFront:
				flag = !flag2;
				break;
			case ConditionalObject.AdjacentCondition.DifferentBehind:
				flag = !flag3;
				break;
			case ConditionalObject.AdjacentCondition.BothSame:
				flag = flag2 && flag3;
				break;
			}
		}
		if (flag)
		{
			if (!IsOnAVehicle)
			{
				for (int i = 0; i < conditional.socketSettings.Length; i++)
				{
					flag = !conditional.socketSettings[i].HasSocketRestrictions;
					if (!flag)
					{
						break;
					}
				}
			}
			else
			{
				for (int j = 0; j < conditional.socketSettings.Length; j++)
				{
					ModularVehicleSocket socket = Vehicle.GetSocket(FirstSocketIndex + j);
					if (socket == null)
					{
						Debug.LogWarning((object)$"{AssociatedItemDef.displayName.get_translated()} module got NULL socket at index {FirstSocketIndex + j}. Total vehicle sockets: {Vehicle.TotalSockets} FirstSocketIndex: {FirstSocketIndex} Sockets taken: {conditional.socketSettings.Length}");
					}
					flag = socket?.ShouldBeActive(conditional.socketSettings[j]) ?? false;
					if (!flag)
					{
						break;
					}
				}
			}
		}
		conditional.gameObject.get_activeInHierarchy();
		conditional.SetActive(flag);
	}

	private bool TryGetAdjacentModuleInFront(out BaseVehicleModule result)
	{
		if (!IsOnAVehicle)
		{
			result = null;
			return false;
		}
		int socketIndex = FirstSocketIndex - 1;
		return Vehicle.TryGetModuleAt(socketIndex, out result);
	}

	private bool TryGetAdjacentModuleBehind(out BaseVehicleModule result)
	{
		if (!IsOnAVehicle)
		{
			result = null;
			return false;
		}
		int num = FirstSocketIndex + GetNumSocketsTaken() - 1;
		return Vehicle.TryGetModuleAt(num + 1, out result);
	}

	private bool InSameVisualGroupAs(BaseVehicleModule moduleEntity, ConditionalObject.AdjacentMatchType matchType)
	{
		if ((Object)(object)moduleEntity == (Object)null)
		{
			return false;
		}
		if (visualGroup == VisualGroup.None)
		{
			if (matchType == ConditionalObject.AdjacentMatchType.GroupNotExact)
			{
				return false;
			}
			return moduleEntity.prefabID == prefabID;
		}
		switch (matchType)
		{
		case ConditionalObject.AdjacentMatchType.GroupOrExact:
			if (moduleEntity.prefabID != prefabID)
			{
				return moduleEntity.visualGroup == visualGroup;
			}
			return true;
		case ConditionalObject.AdjacentMatchType.ExactOnly:
			return moduleEntity.prefabID == prefabID;
		case ConditionalObject.AdjacentMatchType.GroupNotExact:
			if (moduleEntity.prefabID != prefabID)
			{
				return moduleEntity.visualGroup == visualGroup;
			}
			return false;
		default:
			return false;
		}
	}

	private bool CanBeUsedNowBy(BasePlayer player)
	{
		if (!IsOnAVehicle || (Object)(object)player == (Object)null)
		{
			return false;
		}
		if (Vehicle.IsEditableNow || Vehicle.IsDead() || !Vehicle.PlayerIsMounted(player))
		{
			return false;
		}
		return Vehicle.PlayerCanUseThis(player, ModularCarLock.LockType.General);
	}

	public bool PlayerIsLookingAtUsable(string lookingAtColldierName, string usableColliderName)
	{
		return string.Compare(lookingAtColldierName, usableColliderName, ignoreCase: true) == 0;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (base.isClient && info.msg.vehicleModule != null && FirstSocketIndex < 0 && info.msg.vehicleModule.socketIndex >= 0)
		{
			FirstSocketIndex = info.msg.vehicleModule.socketIndex;
		}
	}

	public override bool IsVehicleRoot()
	{
		return false;
	}
}
