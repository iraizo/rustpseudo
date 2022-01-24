using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class BaseRidableAnimal : BaseVehicle
{
	public enum RunState
	{
		stopped = 1,
		walk,
		run,
		sprint,
		LAST
	}

	public ItemDefinition onlyAllowedItem;

	public ItemContainer.ContentsType allowedContents = ItemContainer.ContentsType.Generic;

	public int maxStackSize = 1;

	public int numSlots;

	public string lootPanelName = "generic";

	public bool needsBuildingPrivilegeToUse;

	public bool isLootable = true;

	protected ItemContainer inventory;

	public const Flags Flag_ForSale = Flags.Reserved2;

	private Vector3 lastMoveDirection;

	public GameObjectRef saddlePrefab;

	public EntityRef saddleRef;

	public Transform movementLOSOrigin;

	public SoundPlayer sprintSounds;

	public SoundPlayer largeWhinny;

	public const Flags Flag_Lead = Flags.Reserved7;

	public const Flags Flag_HasRider = Flags.On;

	[Header("Purchase")]
	public ItemDefinition purchaseToken;

	public GameObjectRef eatEffect;

	public GameObjectRef CorpsePrefab;

	[Header("Obstacles")]
	public Transform animalFront;

	public float obstacleDetectionRadius = 0.25f;

	public float maxWaterDepth = 1.5f;

	public float roadSpeedBonus = 2f;

	public float maxWallClimbSlope = 53f;

	public float maxStepHeight = 1f;

	public float maxStepDownHeight = 1.35f;

	[Header("Movement")]
	public RunState currentRunState = RunState.stopped;

	public float walkSpeed = 2f;

	public float trotSpeed = 7f;

	public float runSpeed = 14f;

	public float turnSpeed = 30f;

	public float maxSpeed = 5f;

	public Transform[] groundSampleOffsets;

	[Header("Dung")]
	public ItemDefinition Dung;

	public float CaloriesToDigestPerHour = 100f;

	public float DungProducedPerCalorie = 0.001f;

	private float pendingDungCalories;

	private float dungProduction;

	[Header("Stamina")]
	public float staminaSeconds = 10f;

	public float currentMaxStaminaSeconds = 10f;

	public float maxStaminaSeconds = 20f;

	public float staminaCoreLossRatio = 0.1f;

	public float staminaCoreSpeedBonus = 3f;

	public float staminaReplenishRatioMoving = 0.5f;

	public float staminaReplenishRatioStanding = 1f;

	public float calorieToStaminaRatio = 0.1f;

	public float hydrationToStaminaRatio = 0.5f;

	public float maxStaminaCoreFromWater = 0.5f;

	public bool debugMovement = true;

	private const float normalOffsetDist = 0.15f;

	private Vector3[] normalOffsets = (Vector3[])(object)new Vector3[7]
	{
		new Vector3(0.15f, 0f, 0f),
		new Vector3(-0.15f, 0f, 0f),
		new Vector3(0f, 0f, 0.15f),
		new Vector3(0f, 0f, 0.3f),
		new Vector3(0f, 0f, 0.6f),
		new Vector3(0.15f, 0f, 0.3f),
		new Vector3(-0.15f, 0f, 0.3f)
	};

	[ServerVar(Help = "How long before a horse dies unattended")]
	public static float decayminutes = 180f;

	public float currentSpeed;

	public float desiredRotation;

	public float animalPitchClamp = 90f;

	public float animalRollClamp;

	public static Queue<BaseRidableAnimal> _processQueue = new Queue<BaseRidableAnimal>();

	[ServerVar]
	[Help("How many miliseconds to budget for processing ridable animals per frame")]
	public static float framebudgetms = 1f;

	[ServerVar]
	[Help("Scale all ridable animal dung production rates by this value. 0 will disable dung production.")]
	public static float dungTimeScale = 1f;

	private BaseEntity leadTarget;

	private float nextDecayTime;

	private float lastMovementUpdateTime = -1f;

	private bool inQueue;

	protected float nextEatTime;

	private float lastEatTime = float.NegativeInfinity;

	private float lastInputTime;

	private float forwardHeldSeconds;

	private float backwardHeldSeconds;

	private float sprintHeldSeconds;

	private float lastSprintPressedTime;

	private float lastForwardPressedTime;

	private float lastBackwardPressedTime;

	private float timeInMoveState;

	protected bool onIdealTerrain;

	private float nextIdealTerrainCheckTime;

	private float nextStandTime;

	private InputState aiInputState;

	private Vector3 currentVelocity;

	private Vector3 averagedUp = Vector3.get_up();

	private float nextGroundNormalUpdateTime;

	private Vector3 targetUp = Vector3.get_up();

	private float nextObstacleCheckTime;

	private float cachedObstacleDistance = float.PositiveInfinity;

	private const int maxObstacleCheckSpeed = 10;

	private float timeAlive;

	private TimeUntil dropUntilTime;

	public override bool IsNpc => true;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("BaseRidableAnimal.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 2333451803u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_Claim "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_Claim", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(2333451803u, "RPC_Claim", this, player, 3f))
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
							rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							RPCMessage msg2 = rPCMessage;
							RPC_Claim(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_Claim");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3653170552u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_Lead "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_Lead", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3653170552u, "RPC_Lead", this, player, 3f))
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
							rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							RPCMessage msg3 = rPCMessage;
							RPC_Lead(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_Lead");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 331989034 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_OpenLoot "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_OpenLoot", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(331989034u, "RPC_OpenLoot", this, player, 3f))
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
							rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							RPCMessage rpc2 = rPCMessage;
							RPC_OpenLoot(rpc2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in RPC_OpenLoot");
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

	public bool IsForSale()
	{
		return HasFlag(Flags.Reserved2);
	}

	public void ContainerServerInit()
	{
		if (inventory == null)
		{
			CreateInventory(giveUID: true);
			OnInventoryFirstCreated(inventory);
		}
	}

	public void CreateInventory(bool giveUID)
	{
		inventory = new ItemContainer();
		inventory.entityOwner = this;
		inventory.allowedContents = ((allowedContents == (ItemContainer.ContentsType)0) ? ItemContainer.ContentsType.Generic : allowedContents);
		inventory.SetOnlyAllowedItem(onlyAllowedItem);
		inventory.maxStackSize = maxStackSize;
		inventory.ServerInitialize(null, numSlots);
		inventory.canAcceptItem = ItemFilter;
		if (giveUID)
		{
			inventory.GiveUID();
		}
		inventory.onItemAddedRemoved = OnItemAddedOrRemoved;
		inventory.onDirty += OnInventoryDirty;
	}

	public void SaveContainer(SaveInfo info)
	{
		if (info.forDisk)
		{
			if (inventory != null)
			{
				info.msg.storageBox = Pool.Get<StorageBox>();
				info.msg.storageBox.contents = inventory.Save();
			}
			else
			{
				Debug.LogWarning((object)("Storage container without inventory: " + ((object)this).ToString()));
			}
		}
	}

	public virtual void OnInventoryFirstCreated(ItemContainer container)
	{
	}

	public virtual void OnInventoryDirty()
	{
	}

	public virtual void OnItemAddedOrRemoved(Item item, bool added)
	{
	}

	public bool ItemFilter(Item item, int targetSlot)
	{
		return CanAnimalAcceptItem(item, targetSlot);
	}

	public virtual bool CanAnimalAcceptItem(Item item, int targetSlot)
	{
		return true;
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	private void RPC_OpenLoot(RPCMessage rpc)
	{
		if (inventory != null)
		{
			BasePlayer player = rpc.player;
			if (Object.op_Implicit((Object)(object)player) && player.CanInteract() && CanOpenStorage(player) && (!needsBuildingPrivilegeToUse || player.CanBuild()) && player.inventory.loot.StartLootingEntity(this))
			{
				player.inventory.loot.AddContainer(inventory);
				player.inventory.loot.SendImmediate();
				player.ClientRPCPlayer(null, player, "RPC_OpenLootPanel", lootPanelName);
				SendNetworkUpdate();
			}
		}
	}

	public virtual void PlayerStoppedLooting(BasePlayer player)
	{
	}

	public virtual bool CanOpenStorage(BasePlayer player)
	{
		if (!HasFlag(Flags.On))
		{
			return true;
		}
		if (PlayerIsMounted(player))
		{
			return true;
		}
		return false;
	}

	public void LoadContainer(LoadInfo info)
	{
		if (info.fromDisk && info.msg.storageBox != null)
		{
			if (inventory != null)
			{
				inventory.Load(info.msg.storageBox.contents);
				inventory.capacity = numSlots;
			}
			else
			{
				Debug.LogWarning((object)("Storage container without inventory: " + ((object)this).ToString()));
			}
		}
	}

	public float GetBreathingDelay()
	{
		return currentRunState switch
		{
			RunState.walk => 8f, 
			RunState.run => 5f, 
			RunState.sprint => 2.5f, 
			_ => -1f, 
		};
	}

	public bool IsLeading()
	{
		return HasFlag(Flags.Reserved7);
	}

	public static float UnitsToKPH(float unitsPerSecond)
	{
		return unitsPerSecond * 60f * 60f / 1000f;
	}

	public static void ProcessQueue()
	{
		float realtimeSinceStartup = Time.get_realtimeSinceStartup();
		float num = framebudgetms / 1000f;
		while (_processQueue.get_Count() > 0 && Time.get_realtimeSinceStartup() < realtimeSinceStartup + num)
		{
			BaseRidableAnimal baseRidableAnimal = _processQueue.Dequeue();
			if ((Object)(object)baseRidableAnimal != (Object)null)
			{
				baseRidableAnimal.BudgetedUpdate();
				baseRidableAnimal.inQueue = false;
			}
		}
	}

	public void SetLeading(BaseEntity newLeadTarget)
	{
		leadTarget = newLeadTarget;
		SetFlag(Flags.Reserved7, (Object)(object)leadTarget != (Object)null);
	}

	public override float GetNetworkTime()
	{
		return lastMovementUpdateTime;
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		SaveContainer(info);
	}

	private void OnPhysicsNeighbourChanged()
	{
		((FacepunchBehaviour)this).Invoke((Action)DelayedDropToGround, Time.get_fixedDeltaTime());
	}

	public void DelayedDropToGround()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		DropToGround(((Component)this).get_transform().get_position(), force: true);
		UpdateGroundNormal(force: true);
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		LoadContainer(info);
	}

	public override void AttemptMount(BasePlayer player, bool doMountChecks = true)
	{
		if (!IsForSale())
		{
			base.AttemptMount(player, doMountChecks);
		}
	}

	public virtual void LeadingChanged()
	{
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_Claim(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!((Object)(object)player == (Object)null) && IsForSale())
		{
			Item item = GetPurchaseToken(player);
			if (item != null)
			{
				item.UseItem();
				SetFlag(Flags.Reserved2, b: false);
				AttemptMount(player, doMountChecks: false);
			}
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_Lead(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!((Object)(object)player == (Object)null) && !HasDriver() && !IsForSale())
		{
			bool num = IsLeading();
			bool flag = msg.read.Bit();
			if (num != flag)
			{
				SetLeading(flag ? player : null);
				LeadingChanged();
			}
		}
	}

	public override void PlayerMounted(BasePlayer player, BaseMountable seat)
	{
		base.PlayerMounted(player, seat);
		SetFlag(Flags.On, b: true, recursive: true);
	}

	public override void PlayerDismounted(BasePlayer player, BaseMountable seat)
	{
		base.PlayerDismounted(player, seat);
		SetFlag(Flags.On, b: false, recursive: true);
	}

	public void SetDecayActive(bool isActive)
	{
		if (isActive)
		{
			((FacepunchBehaviour)this).InvokeRandomized((Action)AnimalDecay, Random.Range(30f, 60f), 60f, 6f);
		}
		else
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)AnimalDecay);
		}
	}

	public float TimeUntilNextDecay()
	{
		return nextDecayTime - Time.get_time();
	}

	public void AddDecayDelay(float amount)
	{
		if (nextDecayTime < Time.get_time())
		{
			nextDecayTime = Time.get_time() + 5f;
		}
		nextDecayTime += amount;
		if (Global.developer > 0)
		{
			Debug.Log((object)("Add Decay Delay ! amount is " + amount + "time until next decay : " + (nextDecayTime - Time.get_time())));
		}
	}

	public override void Hurt(HitInfo info)
	{
		if (!IsForSale())
		{
			base.Hurt(info);
		}
	}

	public void AnimalDecay()
	{
		if (base.healthFraction == 0f || base.IsDestroyed || Time.get_time() < lastInputTime + 600f || Time.get_time() < lastEatTime + 600f || IsForSale())
		{
			return;
		}
		if (Time.get_time() < nextDecayTime)
		{
			if (Global.developer > 0)
			{
				Debug.Log((object)"Skipping animal decay due to hitching");
			}
		}
		else
		{
			float num = 1f / decayminutes;
			float num2 = ((!IsOutside()) ? 1f : 0.5f);
			Hurt(MaxHealth() * num * num2, DamageType.Decay, this, useProtection: false);
		}
	}

	public void UseStamina(float amount)
	{
		if (onIdealTerrain)
		{
			amount *= 0.5f;
		}
		staminaSeconds -= amount;
		if (staminaSeconds <= 0f)
		{
			staminaSeconds = 0f;
		}
	}

	public bool CanInitiateSprint()
	{
		return staminaSeconds > 4f;
	}

	public bool CanSprint()
	{
		return staminaSeconds > 0f;
	}

	public void ReplenishStamina(float amount)
	{
		float num = 1f + Mathf.InverseLerp(maxStaminaSeconds * 0.5f, maxStaminaSeconds, currentMaxStaminaSeconds);
		amount *= num;
		amount = Mathf.Min(currentMaxStaminaSeconds - staminaSeconds, amount);
		float num2 = Mathf.Min(currentMaxStaminaSeconds - staminaCoreLossRatio * amount, amount * staminaCoreLossRatio);
		currentMaxStaminaSeconds = Mathf.Clamp(currentMaxStaminaSeconds - num2, 0f, maxStaminaSeconds);
		staminaSeconds = Mathf.Clamp(staminaSeconds + num2 / staminaCoreLossRatio, 0f, currentMaxStaminaSeconds);
	}

	public virtual float ReplenishRatio()
	{
		return 1f;
	}

	public void ReplenishStaminaCore(float calories, float hydration)
	{
		float num = calories * calorieToStaminaRatio;
		float num2 = hydration * hydrationToStaminaRatio;
		float num3 = ReplenishRatio();
		num2 = Mathf.Min(maxStaminaCoreFromWater - currentMaxStaminaSeconds, num2);
		if (num2 < 0f)
		{
			num2 = 0f;
		}
		float num4 = num + num2 * num3;
		currentMaxStaminaSeconds = Mathf.Clamp(currentMaxStaminaSeconds + num4, 0f, maxStaminaSeconds);
		staminaSeconds = Mathf.Clamp(staminaSeconds + num4, 0f, currentMaxStaminaSeconds);
	}

	public void UpdateStamina(float delta)
	{
		if (currentRunState == RunState.sprint)
		{
			UseStamina(delta);
		}
		else if (currentRunState == RunState.run)
		{
			ReplenishStamina(staminaReplenishRatioMoving * delta);
		}
		else
		{
			ReplenishStamina(staminaReplenishRatioStanding * delta);
		}
	}

	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		RiderInput(inputState, player);
	}

	public void DismountHeavyPlayers()
	{
		if (HasFlag(Flags.On))
		{
			BasePlayer driver = GetDriver();
			if (Object.op_Implicit((Object)(object)driver) && IsPlayerTooHeavy(driver))
			{
				DismountAllPlayers();
			}
		}
	}

	public BaseMountable GetSaddle()
	{
		if (!saddleRef.IsValid(base.isServer))
		{
			return null;
		}
		return ((Component)saddleRef.Get(base.isServer)).GetComponent<BaseMountable>();
	}

	public void BudgetedUpdate()
	{
		DismountHeavyPlayers();
		UpdateOnIdealTerrain();
		UpdateStamina(Time.get_fixedDeltaTime());
		if (currentRunState == RunState.stopped)
		{
			EatNearbyFood();
		}
		if (lastMovementUpdateTime == -1f)
		{
			lastMovementUpdateTime = Time.get_realtimeSinceStartup();
		}
		float delta = Time.get_realtimeSinceStartup() - lastMovementUpdateTime;
		UpdateMovement(delta);
		lastMovementUpdateTime = Time.get_realtimeSinceStartup();
		UpdateDung(delta);
	}

	public void ApplyDungCalories(float calories)
	{
		pendingDungCalories += calories;
	}

	private void UpdateDung(float delta)
	{
		if (!((Object)(object)Dung == (Object)null) && !Mathf.Approximately(dungTimeScale, 0f))
		{
			float num = Mathf.Min(pendingDungCalories * delta, CaloriesToDigestPerHour / 3600f * delta) * DungProducedPerCalorie;
			dungProduction += num;
			pendingDungCalories -= num;
			if (dungProduction >= 1f)
			{
				DoDung();
			}
		}
	}

	private void DoDung()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		dungProduction -= 1f;
		ItemManager.Create(Dung, 1, 0uL).Drop(((Component)this).get_transform().get_position() + -((Component)this).get_transform().get_forward() + Vector3.get_up() * 1.1f + Random.get_insideUnitSphere() * 0.1f, -((Component)this).get_transform().get_forward());
	}

	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		timeAlive += Time.get_fixedDeltaTime();
		if (!inQueue)
		{
			_processQueue.Enqueue(this);
			inQueue = true;
		}
	}

	public float StaminaCoreFraction()
	{
		return Mathf.InverseLerp(0f, maxStaminaSeconds, currentMaxStaminaSeconds);
	}

	public void DoEatEvent()
	{
		ClientRPC(null, "Eat");
	}

	public void ReplenishFromFood(ItemModConsumable consumable)
	{
		if (Object.op_Implicit((Object)(object)consumable))
		{
			ClientRPC(null, "Eat");
			lastEatTime = Time.get_time();
			float ifType = consumable.GetIfType(MetabolismAttribute.Type.Calories);
			float ifType2 = consumable.GetIfType(MetabolismAttribute.Type.Hydration);
			float num = consumable.GetIfType(MetabolismAttribute.Type.Health) + consumable.GetIfType(MetabolismAttribute.Type.HealthOverTime);
			ApplyDungCalories(ifType);
			ReplenishStaminaCore(ifType, ifType2);
			Heal(num * 4f);
		}
	}

	public virtual void EatNearbyFood()
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		if (Time.get_time() < nextEatTime)
		{
			return;
		}
		float num = StaminaCoreFraction();
		nextEatTime = Time.get_time() + Random.Range(2f, 3f) + Mathf.InverseLerp(0.5f, 1f, num) * 4f;
		if (num >= 1f)
		{
			return;
		}
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities(((Component)this).get_transform().get_position() + ((Component)this).get_transform().get_forward() * 1.5f, 2f, list, 67109377, (QueryTriggerInteraction)2);
		list.Sort((BaseEntity a, BaseEntity b) => (b is DroppedItem).CompareTo(a is DroppedItem));
		foreach (BaseEntity item in list)
		{
			if (item.isClient)
			{
				continue;
			}
			DroppedItem droppedItem = item as DroppedItem;
			if (Object.op_Implicit((Object)(object)droppedItem) && droppedItem.item != null && droppedItem.item.info.category == ItemCategory.Food)
			{
				ItemModConsumable component = ((Component)droppedItem.item.info).GetComponent<ItemModConsumable>();
				if (Object.op_Implicit((Object)(object)component))
				{
					ReplenishFromFood(component);
					droppedItem.item.UseItem();
					if (droppedItem.item.amount <= 0)
					{
						droppedItem.Kill();
					}
					break;
				}
			}
			CollectibleEntity collectibleEntity = item as CollectibleEntity;
			if (Object.op_Implicit((Object)(object)collectibleEntity) && collectibleEntity.IsFood())
			{
				collectibleEntity.DoPickup(null);
				break;
			}
			GrowableEntity growableEntity = item as GrowableEntity;
			if (Object.op_Implicit((Object)(object)growableEntity) && growableEntity.CanPick())
			{
				growableEntity.PickFruit(null);
				break;
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
	}

	public void SwitchMoveState(RunState newState)
	{
		if (newState != currentRunState)
		{
			currentRunState = newState;
			timeInMoveState = 0f;
			SetFlag(Flags.Reserved8, currentRunState == RunState.sprint, recursive: false, networkupdate: false);
			MarkObstacleDistanceDirty();
		}
	}

	public void UpdateOnIdealTerrain()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (!(Time.get_time() < nextIdealTerrainCheckTime))
		{
			nextIdealTerrainCheckTime = Time.get_time() + Random.Range(1f, 2f);
			onIdealTerrain = false;
			if ((Object)(object)TerrainMeta.TopologyMap != (Object)null && ((uint)TerrainMeta.TopologyMap.GetTopology(((Component)this).get_transform().get_position()) & 0x800u) != 0)
			{
				onIdealTerrain = true;
			}
		}
	}

	public float MoveStateToVelocity(RunState stateToCheck)
	{
		float num = 0f;
		return stateToCheck switch
		{
			RunState.walk => GetWalkSpeed(), 
			RunState.run => GetTrotSpeed(), 
			RunState.sprint => GetRunSpeed(), 
			_ => 0f, 
		};
	}

	public float GetDesiredVelocity()
	{
		return MoveStateToVelocity(currentRunState);
	}

	public RunState StateFromSpeed(float speedToUse)
	{
		if (speedToUse <= MoveStateToVelocity(RunState.stopped))
		{
			return RunState.stopped;
		}
		if (speedToUse <= MoveStateToVelocity(RunState.walk))
		{
			return RunState.walk;
		}
		if (speedToUse <= MoveStateToVelocity(RunState.run))
		{
			return RunState.run;
		}
		return RunState.sprint;
	}

	public void ModifyRunState(int dir)
	{
		if ((currentRunState != RunState.stopped || dir >= 0) && (currentRunState != RunState.sprint || dir <= 0))
		{
			RunState newState = currentRunState + dir;
			SwitchMoveState(newState);
		}
	}

	public bool CanStand()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (nextStandTime > Time.get_time())
		{
			return false;
		}
		if ((Object)(object)mountPoints[0].mountable == (Object)null)
		{
			return false;
		}
		bool flag = false;
		List<Collider> list = Pool.GetList<Collider>();
		Vis.Colliders<Collider>(((Component)mountPoints[0].mountable.eyePositionOverride).get_transform().get_position() - ((Component)this).get_transform().get_forward() * 1f, 2f, list, 2162689, (QueryTriggerInteraction)2);
		if (list.Count > 0)
		{
			flag = true;
		}
		Pool.FreeList<Collider>(ref list);
		return !flag;
	}

	public void DoDebugMovement()
	{
		if (aiInputState == null)
		{
			aiInputState = new InputState();
		}
		if (!debugMovement)
		{
			InputMessage current = aiInputState.current;
			current.buttons &= -3;
			InputMessage current2 = aiInputState.current;
			current2.buttons &= -9;
			InputMessage current3 = aiInputState.current;
			current3.buttons &= -129;
		}
		else
		{
			InputMessage current4 = aiInputState.current;
			current4.buttons |= 2;
			InputMessage current5 = aiInputState.current;
			current5.buttons |= 8;
			InputMessage current6 = aiInputState.current;
			current6.buttons |= 0x80;
			RiderInput(aiInputState, null);
		}
	}

	public virtual void RiderInput(InputState inputState, BasePlayer player)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		float num = Time.get_time() - lastInputTime;
		lastInputTime = Time.get_time();
		num = Mathf.Clamp(num, 0f, 1f);
		Vector3.get_zero();
		timeInMoveState += num;
		if (inputState == null)
		{
			return;
		}
		if (inputState.IsDown(BUTTON.FORWARD))
		{
			lastForwardPressedTime = Time.get_time();
			forwardHeldSeconds += num;
		}
		else
		{
			forwardHeldSeconds = 0f;
		}
		if (inputState.IsDown(BUTTON.BACKWARD))
		{
			lastBackwardPressedTime = Time.get_time();
			backwardHeldSeconds += num;
		}
		else
		{
			backwardHeldSeconds = 0f;
		}
		if (inputState.IsDown(BUTTON.SPRINT))
		{
			lastSprintPressedTime = Time.get_time();
			sprintHeldSeconds += num;
		}
		else
		{
			sprintHeldSeconds = 0f;
		}
		if (inputState.IsDown(BUTTON.DUCK) && CanStand() && (currentRunState == RunState.stopped || (currentRunState == RunState.walk && currentSpeed < 1f)))
		{
			ClientRPC(null, "Stand");
			nextStandTime = Time.get_time() + 3f;
			currentSpeed = 0f;
		}
		if (Time.get_time() < nextStandTime)
		{
			forwardHeldSeconds = 0f;
			backwardHeldSeconds = 0f;
		}
		if (forwardHeldSeconds > 0f)
		{
			if (currentRunState == RunState.stopped)
			{
				SwitchMoveState(RunState.walk);
			}
			else if (currentRunState == RunState.walk)
			{
				if (sprintHeldSeconds > 0f)
				{
					SwitchMoveState(RunState.run);
				}
			}
			else if (currentRunState == RunState.run && sprintHeldSeconds > 1f && CanInitiateSprint())
			{
				SwitchMoveState(RunState.sprint);
			}
		}
		else if (backwardHeldSeconds > 1f)
		{
			ModifyRunState(-1);
			backwardHeldSeconds = 0.1f;
		}
		else if (backwardHeldSeconds == 0f && forwardHeldSeconds == 0f && timeInMoveState > 1f && currentRunState != RunState.stopped)
		{
			ModifyRunState(-1);
		}
		if (currentRunState == RunState.sprint && (!CanSprint() || Time.get_time() - lastSprintPressedTime > 5f))
		{
			ModifyRunState(-1);
		}
		if (inputState.IsDown(BUTTON.RIGHT))
		{
			if (currentRunState == RunState.stopped)
			{
				ModifyRunState(1);
			}
			desiredRotation = 1f;
		}
		else if (inputState.IsDown(BUTTON.LEFT))
		{
			if (currentRunState == RunState.stopped)
			{
				ModifyRunState(1);
			}
			desiredRotation = -1f;
		}
		else
		{
			desiredRotation = 0f;
		}
	}

	public override float MaxVelocity()
	{
		return maxSpeed * 1.5f;
	}

	private float NormalizeAngle(float angle)
	{
		if (angle > 180f)
		{
			angle -= 360f;
		}
		return angle;
	}

	public void UpdateGroundNormal(bool force = false)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		if (Time.get_time() >= nextGroundNormalUpdateTime || force)
		{
			nextGroundNormalUpdateTime = Time.get_time() + Random.Range(0.2f, 0.3f);
			targetUp = averagedUp;
			Transform[] array = groundSampleOffsets;
			for (int i = 0; i < array.Length; i++)
			{
				if (TransformUtil.GetGroundInfo(array[i].get_position() + Vector3.get_up() * 2f, out var _, out var normal, 4f, LayerMask.op_Implicit(413204481)))
				{
					targetUp += normal;
				}
				else
				{
					targetUp += Vector3.get_up();
				}
			}
			targetUp /= (float)(groundSampleOffsets.Length + 1);
		}
		averagedUp = Vector3.Lerp(averagedUp, targetUp, Time.get_deltaTime() * 2f);
	}

	public void MarkObstacleDistanceDirty()
	{
		nextObstacleCheckTime = 0f;
	}

	public float GetObstacleDistance()
	{
		if (Time.get_time() >= nextObstacleCheckTime)
		{
			float desiredVelocity = GetDesiredVelocity();
			if (currentSpeed > 0f || desiredVelocity > 0f)
			{
				cachedObstacleDistance = ObstacleDistanceCheck(Mathf.Max(desiredVelocity, 2f));
			}
			nextObstacleCheckTime = Time.get_time() + Random.Range(0.25f, 0.35f);
		}
		return cachedObstacleDistance;
	}

	public float ObstacleDistanceCheck(float speed = 10f)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).get_transform().get_position();
		int num = Mathf.Max(2, Mathf.Min((int)speed, 10));
		float num2 = 0.5f;
		int num3 = Mathf.CeilToInt((float)num / num2);
		float num4 = 0f;
		Vector3 val = QuaternionEx.LookRotationForcedUp(((Component)this).get_transform().get_forward(), Vector3.get_up()) * Vector3.get_forward();
		Vector3 val2 = ((Component)movementLOSOrigin).get_transform().get_position();
		val2.y = ((Component)this).get_transform().get_position().y;
		Vector3 up = ((Component)this).get_transform().get_up();
		RaycastHit val6 = default(RaycastHit);
		for (int i = 0; i < num3; i++)
		{
			float num5 = num2;
			bool flag = false;
			float num6 = 0f;
			Vector3 pos = Vector3.get_zero();
			Vector3 normal = Vector3.get_up();
			Vector3 val3 = val2;
			Vector3 val4 = val3 + Vector3.get_up() * (maxStepHeight + obstacleDetectionRadius);
			Vector3 val5 = val3 + val * num5;
			float num7 = maxStepDownHeight + obstacleDetectionRadius;
			if (Physics.SphereCast(val4, obstacleDetectionRadius, val, ref val6, num5, 1486954753))
			{
				num6 = ((RaycastHit)(ref val6)).get_distance();
				pos = ((RaycastHit)(ref val6)).get_point();
				normal = ((RaycastHit)(ref val6)).get_normal();
				flag = true;
			}
			if (!flag)
			{
				if (!TransformUtil.GetGroundInfo(val5 + Vector3.get_up() * 2f, out pos, out normal, 2f + num7, LayerMask.op_Implicit(413204481)))
				{
					return num4;
				}
				num6 = Vector3.Distance(val3, pos);
				if (WaterLevel.Test(pos + Vector3.get_one() * maxWaterDepth, waves: true, this))
				{
					normal = -((Component)this).get_transform().get_forward();
					return num4;
				}
				flag = true;
			}
			if (flag)
			{
				float num8 = Vector3.Angle(up, normal);
				float num9 = Vector3.Angle(normal, Vector3.get_up());
				if (num8 > maxWallClimbSlope || num9 > maxWallClimbSlope)
				{
					Vector3 val7 = normal;
					float num10 = pos.y;
					int num11 = 1;
					for (int j = 0; j < normalOffsets.Length; j++)
					{
						Vector3 val8 = val5 + normalOffsets[j].x * ((Component)this).get_transform().get_right();
						float num12 = maxStepHeight * 2.5f;
						if (TransformUtil.GetGroundInfo(val8 + Vector3.get_up() * num12 + normalOffsets[j].z * ((Component)this).get_transform().get_forward(), out var pos2, out var normal2, num7 + num12, LayerMask.op_Implicit(413204481)))
						{
							num11++;
							val7 += normal2;
							num10 += pos2.y;
						}
					}
					num10 /= (float)num11;
					((Vector3)(ref val7)).Normalize();
					float num13 = Vector3.Angle(up, val7);
					num9 = Vector3.Angle(val7, Vector3.get_up());
					if (num13 > maxWallClimbSlope || num9 > maxWallClimbSlope || Mathf.Abs(num10 - val5.y) > maxStepHeight)
					{
						return num4;
					}
				}
			}
			num4 += num6;
			val = QuaternionEx.LookRotationForcedUp(((Component)this).get_transform().get_forward(), normal) * Vector3.get_forward();
			val2 = pos;
		}
		return num4;
	}

	public virtual void MarkDistanceTravelled(float amount)
	{
	}

	public void UpdateMovement(float delta)
	{
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0503: Unknown result type (might be due to invalid IL or missing references)
		//IL_0508: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_056c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0571: Unknown result type (might be due to invalid IL or missing references)
		//IL_0581: Unknown result type (might be due to invalid IL or missing references)
		//IL_0586: Unknown result type (might be due to invalid IL or missing references)
		//IL_058b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
		float num = WaterFactor();
		if (num > 1f && !base.IsDestroyed)
		{
			Kill();
			return;
		}
		if (desiredRotation != 0f)
		{
			MarkObstacleDistanceDirty();
		}
		if (num >= 0.3f && currentRunState > RunState.run)
		{
			currentRunState = RunState.run;
		}
		else if (num >= 0.45f && currentRunState > RunState.walk)
		{
			currentRunState = RunState.walk;
		}
		if (Time.get_time() - lastInputTime > 3f && !IsLeading())
		{
			currentRunState = RunState.stopped;
			desiredRotation = 0f;
		}
		if ((HasDriver() && IsLeading()) || (Object)(object)leadTarget == (Object)null)
		{
			SetLeading(null);
		}
		if (IsLeading())
		{
			Vector3 position = ((Component)leadTarget).get_transform().get_position();
			Vector3 val = Vector3Ex.Direction2D(((Component)this).get_transform().get_position() + ((Component)this).get_transform().get_right() * 1f, ((Component)this).get_transform().get_position());
			Vector3 val2 = Vector3Ex.Direction2D(((Component)this).get_transform().get_position() + ((Component)this).get_transform().get_forward() * 0.01f, ((Component)this).get_transform().get_position());
			Vector3 val3 = Vector3Ex.Direction2D(position, ((Component)this).get_transform().get_position());
			float num2 = Vector3.Dot(val, val3);
			float num3 = Vector3.Dot(val2, val3);
			bool flag = Vector3Ex.Distance2D(position, ((Component)this).get_transform().get_position()) > 2.5f;
			bool num4 = Vector3Ex.Distance2D(position, ((Component)this).get_transform().get_position()) > 10f;
			if (flag || num3 < 0.95f)
			{
				float num5 = Mathf.InverseLerp(0f, 1f, num2);
				float num6 = 1f - Mathf.InverseLerp(-1f, 0f, num2);
				desiredRotation = 0f;
				desiredRotation += num5 * 1f;
				desiredRotation += num6 * -1f;
				if (Mathf.Abs(desiredRotation) < 0.001f)
				{
					desiredRotation = 0f;
				}
				if (flag)
				{
					SwitchMoveState(RunState.walk);
				}
				else
				{
					SwitchMoveState(RunState.stopped);
				}
			}
			else
			{
				desiredRotation = 0f;
				SwitchMoveState(RunState.stopped);
			}
			if (num4)
			{
				SetLeading(null);
				SwitchMoveState(RunState.stopped);
			}
		}
		float obstacleDistance = GetObstacleDistance();
		RunState runState = StateFromSpeed(obstacleDistance * GetRunSpeed());
		if (runState < currentRunState)
		{
			SwitchMoveState(runState);
		}
		float desiredVelocity = GetDesiredVelocity();
		Vector3 val4 = Vector3.get_forward() * Mathf.Sign(desiredVelocity);
		float num7 = Mathf.InverseLerp(0.85f, 1f, obstacleDistance);
		float num8 = Mathf.InverseLerp(1.25f, 10f, obstacleDistance);
		float num9 = 1f - Mathf.InverseLerp(20f, 45f, Vector3.Angle(Vector3.get_up(), averagedUp));
		num8 = num7 * 0.1f + num8 * 0.9f;
		float num10 = Mathf.Min(Mathf.Clamp01(Mathf.Min(num9 + 0.2f, num8)) * GetRunSpeed(), desiredVelocity);
		float num11 = ((num10 < currentSpeed) ? 3f : 1f);
		if (Mathf.Abs(currentSpeed) < 2f && desiredVelocity == 0f)
		{
			currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, delta * 3f);
		}
		else
		{
			currentSpeed = Mathf.Lerp(currentSpeed, num10, delta * num11);
		}
		if (num8 == 0f)
		{
			currentSpeed = 0f;
		}
		float num12 = 1f - Mathf.InverseLerp(2f, 7f, currentSpeed);
		num12 = (num12 + 1f) / 2f;
		if (desiredRotation != 0f)
		{
			((Component)animalFront).get_transform().get_position();
			Quaternion rotation = ((Component)this).get_transform().get_rotation();
			((Component)this).get_transform().Rotate(Vector3.get_up(), desiredRotation * delta * turnSpeed * num12);
			if (!IsLeading() && Vis.AnyColliders(((Component)animalFront).get_transform().get_position(), obstacleDetectionRadius * 0.25f, 1503731969, (QueryTriggerInteraction)1))
			{
				((Component)this).get_transform().set_rotation(rotation);
			}
		}
		Vector3 val5 = ((Component)this).get_transform().TransformDirection(val4);
		Vector3 normalized = ((Vector3)(ref val5)).get_normalized();
		float num13 = currentSpeed * delta;
		Vector3 val6 = ((Component)this).get_transform().get_position() + normalized * num13 * Mathf.Sign(currentSpeed);
		currentVelocity = val5 * currentSpeed;
		UpdateGroundNormal();
		if (!(currentSpeed > 0f) && !(timeAlive < 2f) && !(TimeUntil.op_Implicit(dropUntilTime) > 0f))
		{
			return;
		}
		_ = ((Component)this).get_transform().get_position() + ((Component)this).get_transform().InverseTransformPoint(((Component)animalFront).get_transform().get_position()).y * ((Component)this).get_transform().get_up();
		RaycastHit val7 = default(RaycastHit);
		bool flag2 = Physics.SphereCast(((Component)animalFront).get_transform().get_position(), obstacleDetectionRadius, normalized, ref val7, num13, 1503731969);
		bool flag3 = Physics.SphereCast(((Component)this).get_transform().get_position() + ((Component)this).get_transform().InverseTransformPoint(((Component)animalFront).get_transform().get_position()).y * ((Component)this).get_transform().get_up(), obstacleDetectionRadius, normalized, ref val7, num13, 1503731969);
		if (!Vis.AnyColliders(((Component)animalFront).get_transform().get_position() + normalized * num13, obstacleDetectionRadius, 1503731969, (QueryTriggerInteraction)1) && !flag2 && !flag3)
		{
			if (DropToGround(val6 + Vector3.get_up() * maxStepHeight))
			{
				MarkDistanceTravelled(num13);
			}
			else
			{
				currentSpeed = 0f;
			}
		}
		else
		{
			currentSpeed = 0f;
		}
	}

	public bool DropToGround(Vector3 targetPos, bool force = false)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		float range = (force ? 10000f : (maxStepHeight + maxStepDownHeight));
		if (TransformUtil.GetGroundInfo(targetPos, out var pos, out var _, range, LayerMask.op_Implicit(413204481)))
		{
			if (Physics.CheckSphere(pos + Vector3.get_up() * 1f, 0.2f, 413204481))
			{
				return false;
			}
			((Component)this).get_transform().set_position(pos);
			Quaternion val = QuaternionEx.LookRotationForcedUp(((Component)this).get_transform().get_forward(), averagedUp);
			Vector3 eulerAngles = ((Quaternion)(ref val)).get_eulerAngles();
			if (eulerAngles.z > 180f)
			{
				eulerAngles.z -= 360f;
			}
			else if (eulerAngles.z < -180f)
			{
				eulerAngles.z += 360f;
			}
			eulerAngles.z = Mathf.Clamp(eulerAngles.z, -10f, 10f);
			((Component)this).get_transform().set_rotation(Quaternion.Euler(eulerAngles));
			return true;
		}
		return false;
	}

	public void DoNetworkUpdate()
	{
		SendNetworkUpdate();
	}

	public override void PreServerLoad()
	{
		base.PreServerLoad();
		CreateInventory(giveUID: false);
	}

	public override void ServerInit()
	{
		ContainerServerInit();
		base.ServerInit();
		((FacepunchBehaviour)this).InvokeRepeating((Action)DoNetworkUpdate, Random.Range(0f, 0.2f), 0.333f);
		SetDecayActive(isActive: true);
		if (debugMovement)
		{
			((FacepunchBehaviour)this).InvokeRandomized((Action)DoDebugMovement, 0f, 0.1f, 0.1f);
		}
	}

	public override void OnKilled(HitInfo hitInfo = null)
	{
		Assert.IsTrue(base.isServer, "OnKilled called on client!");
		BaseCorpse baseCorpse = DropCorpse(CorpsePrefab.resourcePath);
		if (Object.op_Implicit((Object)(object)baseCorpse))
		{
			SetupCorpse(baseCorpse);
			baseCorpse.Spawn();
			baseCorpse.TakeChildren(this);
		}
		((FacepunchBehaviour)this).Invoke((Action)base.KillMessage, 0.5f);
		base.OnKilled(hitInfo);
	}

	public virtual void SetupCorpse(BaseCorpse corpse)
	{
		corpse.flags = flags;
		LootableCorpse component = ((Component)corpse).GetComponent<LootableCorpse>();
		if (Object.op_Implicit((Object)(object)component))
		{
			component.TakeFrom(inventory);
		}
	}

	public override Vector3 GetLocalVelocityServer()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return currentVelocity;
	}

	public void UpdateDropToGroundForDuration(float duration)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		dropUntilTime = TimeUntil.op_Implicit(duration);
	}

	public bool PlayerHasToken(BasePlayer player)
	{
		return GetPurchaseToken(player) != null;
	}

	public Item GetPurchaseToken(BasePlayer player)
	{
		int itemid = purchaseToken.itemid;
		return player.inventory.FindItemID(itemid);
	}

	public virtual float GetWalkSpeed()
	{
		return walkSpeed;
	}

	public virtual float GetTrotSpeed()
	{
		return trotSpeed;
	}

	public virtual float GetRunSpeed()
	{
		if (base.isServer)
		{
			_ = runSpeed;
			float num = Mathf.InverseLerp(maxStaminaSeconds * 0.5f, maxStaminaSeconds, currentMaxStaminaSeconds) * staminaCoreSpeedBonus;
			float num2 = (onIdealTerrain ? roadSpeedBonus : 0f);
			return runSpeed + num + num2;
		}
		return runSpeed;
	}

	public bool IsPlayerTooHeavy(BasePlayer player)
	{
		return player.Weight >= 10f;
	}
}
