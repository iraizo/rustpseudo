using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;

public class RidableHorse : BaseRidableAnimal
{
	[ServerVar(Help = "Population active on the server, per square km", ShowInAdminUI = true)]
	public static float Population = 2f;

	public string distanceStatName = "";

	public HorseBreed[] breeds;

	public SkinnedMeshRenderer[] bodyRenderers;

	public SkinnedMeshRenderer[] hairRenderers;

	private int currentBreed = -1;

	private ProtectionProperties riderProtection;

	private ProtectionProperties baseHorseProtection;

	public const Flags Flag_HideHair = Flags.Reserved4;

	public const Flags Flag_WoodArmor = Flags.Reserved5;

	public const Flags Flag_RoadsignArmor = Flags.Reserved6;

	private float equipmentSpeedMod;

	private int numStorageSlots;

	private static Material[] breedAssignmentArray = (Material[])(object)new Material[2];

	private float distanceRecordingSpacing = 5f;

	private HitchTrough currentHitch;

	private float totalDistance;

	private float kmDistance;

	private float tempDistanceTravelled;

	private int numEquipmentSlots = 4;

	public override float RealisticMass => 550f;

	protected override float PositionTickRate => 0.05f;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("RidableHorse.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public int GetStorageSlotCount()
	{
		return numStorageSlots;
	}

	public void ApplyBreed(int index)
	{
		if (currentBreed != index)
		{
			if (index >= breeds.Length || index < 0)
			{
				Debug.LogError((object)("ApplyBreed issue! index is " + index + " breed length is : " + breeds.Length));
			}
			else
			{
				ApplyBreedInternal(breeds[index]);
				currentBreed = index;
			}
		}
	}

	protected void ApplyBreedInternal(HorseBreed breed)
	{
		if (base.isServer)
		{
			SetMaxHealth(StartHealth() * breed.maxHealth);
			base.health = MaxHealth();
		}
	}

	public HorseBreed GetBreed()
	{
		if (currentBreed == -1 || currentBreed >= breeds.Length)
		{
			return null;
		}
		return breeds[currentBreed];
	}

	public override float GetTrotSpeed()
	{
		float num = equipmentSpeedMod / (base.GetRunSpeed() * GetBreed().maxSpeed);
		return base.GetTrotSpeed() * GetBreed().maxSpeed * (1f + num);
	}

	public override float GetRunSpeed()
	{
		float num = base.GetRunSpeed();
		HorseBreed breed = GetBreed();
		return num * breed.maxSpeed + equipmentSpeedMod;
	}

	public override void SetupCorpse(BaseCorpse corpse)
	{
		base.SetupCorpse(corpse);
		HorseCorpse component = ((Component)corpse).GetComponent<HorseCorpse>();
		if (Object.op_Implicit((Object)(object)component))
		{
			component.breedIndex = currentBreed;
		}
		else
		{
			Debug.Log((object)"no horse corpse");
		}
	}

	public override void ScaleDamageForPlayer(BasePlayer player, HitInfo info)
	{
		base.ScaleDamageForPlayer(player, info);
		riderProtection.Scale(info.damageTypes);
	}

	public override void OnKilled(HitInfo hitInfo = null)
	{
		TryLeaveHitch();
		base.OnKilled(hitInfo);
	}

	public void SetBreed(int index)
	{
		ApplyBreed(index);
		SendNetworkUpdate();
	}

	public override void LeadingChanged()
	{
		if (!IsLeading())
		{
			TryHitch();
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		SetBreed(Random.Range(0, breeds.Length));
		baseHorseProtection = baseProtection;
		riderProtection = ScriptableObject.CreateInstance<ProtectionProperties>();
		baseProtection = ScriptableObject.CreateInstance<ProtectionProperties>();
		baseProtection.Add(baseHorseProtection, 1f);
	}

	public override void PlayerMounted(BasePlayer player, BaseMountable seat)
	{
		base.PlayerMounted(player, seat);
		((FacepunchBehaviour)this).InvokeRepeating((Action)RecordDistance, distanceRecordingSpacing, distanceRecordingSpacing);
		TryLeaveHitch();
	}

	public override void PlayerDismounted(BasePlayer player, BaseMountable seat)
	{
		base.PlayerDismounted(player, seat);
		((FacepunchBehaviour)this).CancelInvoke((Action)RecordDistance);
		TryHitch();
	}

	public bool IsHitched()
	{
		return (Object)(object)currentHitch != (Object)null;
	}

	public void SetHitch(HitchTrough Hitch)
	{
		currentHitch = Hitch;
		SetFlag(Flags.Reserved3, (Object)(object)currentHitch != (Object)null);
	}

	public override float ReplenishRatio()
	{
		return 1f;
	}

	public override void EatNearbyFood()
	{
		if (Time.get_time() < nextEatTime || (StaminaCoreFraction() >= 1f && base.healthFraction >= 1f))
		{
			return;
		}
		if (IsHitched())
		{
			Item foodItem = currentHitch.GetFoodItem();
			if (foodItem != null && foodItem.amount > 0)
			{
				ItemModConsumable component = ((Component)foodItem.info).GetComponent<ItemModConsumable>();
				if (Object.op_Implicit((Object)(object)component))
				{
					float amount = component.GetIfType(MetabolismAttribute.Type.Calories) * currentHitch.caloriesToDecaySeconds;
					AddDecayDelay(amount);
					ReplenishFromFood(component);
					foodItem.UseItem();
					nextEatTime = Time.get_time() + Random.Range(2f, 3f) + Mathf.InverseLerp(0.5f, 1f, StaminaCoreFraction()) * 4f;
					return;
				}
			}
		}
		base.EatNearbyFood();
	}

	public void TryLeaveHitch()
	{
		if (Object.op_Implicit((Object)(object)currentHitch))
		{
			currentHitch.Unhitch(this);
		}
	}

	public void TryHitch()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		List<HitchTrough> list = Pool.GetList<HitchTrough>();
		Vis.Entities(((Component)this).get_transform().get_position(), 2.5f, list, 256, (QueryTriggerInteraction)1);
		foreach (HitchTrough item in list)
		{
			if (!(Vector3.Dot(Vector3Ex.Direction2D(((Component)item).get_transform().get_position(), ((Component)this).get_transform().get_position()), ((Component)this).get_transform().get_forward()) < 0.4f) && !item.isClient && item.HasSpace() && item.ValidHitchPosition(((Component)this).get_transform().get_position()) && item.AttemptToHitch(this))
			{
				break;
			}
		}
		Pool.FreeList<HitchTrough>(ref list);
	}

	public void RecordDistance()
	{
		BasePlayer driver = GetDriver();
		if ((Object)(object)driver == (Object)null)
		{
			tempDistanceTravelled = 0f;
			return;
		}
		kmDistance += tempDistanceTravelled / 1000f;
		if (kmDistance >= 1f)
		{
			driver.stats.Add(distanceStatName + "_km", 1, (Stats)5);
			kmDistance -= 1f;
		}
		driver.stats.Add(distanceStatName, Mathf.FloorToInt(tempDistanceTravelled));
		driver.stats.Save();
		totalDistance += tempDistanceTravelled;
		tempDistanceTravelled = 0f;
	}

	public override void MarkDistanceTravelled(float amount)
	{
		tempDistanceTravelled += amount;
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.horse = Pool.Get<Horse>();
		info.msg.horse.staminaSeconds = staminaSeconds;
		info.msg.horse.currentMaxStaminaSeconds = currentMaxStaminaSeconds;
		info.msg.horse.breedIndex = currentBreed;
		info.msg.horse.numStorageSlots = numStorageSlots;
		if (!info.forDisk)
		{
			info.msg.horse.runState = (int)currentRunState;
			info.msg.horse.maxSpeed = GetRunSpeed();
		}
	}

	public override void OnInventoryDirty()
	{
		EquipmentUpdate();
	}

	public override bool CanAnimalAcceptItem(Item item, int targetSlot)
	{
		ItemModAnimalEquipment component = ((Component)item.info).GetComponent<ItemModAnimalEquipment>();
		if (targetSlot == -1 && !Object.op_Implicit((Object)(object)component))
		{
			return true;
		}
		if (targetSlot < numEquipmentSlots)
		{
			if ((Object)(object)component == (Object)null)
			{
				return false;
			}
			if (component.slot == ItemModAnimalEquipment.SlotType.Basic)
			{
				return true;
			}
			for (int i = 0; i < numEquipmentSlots; i++)
			{
				Item slot = inventory.GetSlot(i);
				if (slot != null)
				{
					ItemModAnimalEquipment component2 = ((Component)slot.info).GetComponent<ItemModAnimalEquipment>();
					if (!((Object)(object)component2 == (Object)null) && component2.slot == component.slot)
					{
						Debug.Log((object)("rejecting because slot same, found : " + (int)component2.slot + " new : " + (int)component.slot));
						return false;
					}
				}
			}
		}
		return true;
	}

	public int GetStorageStartIndex()
	{
		return numEquipmentSlots;
	}

	public void EquipmentUpdate()
	{
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		SetFlag(Flags.Reserved4, b: false, recursive: false, networkupdate: false);
		SetFlag(Flags.Reserved5, b: false, recursive: false, networkupdate: false);
		SetFlag(Flags.Reserved6, b: false, recursive: false, networkupdate: false);
		riderProtection.Clear();
		baseProtection.Clear();
		equipmentSpeedMod = 0f;
		numStorageSlots = 0;
		for (int i = 0; i < numEquipmentSlots; i++)
		{
			Item slot = inventory.GetSlot(i);
			if (slot == null)
			{
				continue;
			}
			ItemModAnimalEquipment component = ((Component)slot.info).GetComponent<ItemModAnimalEquipment>();
			if (Object.op_Implicit((Object)(object)component))
			{
				SetFlag(component.WearableFlag, b: true, recursive: false, networkupdate: false);
				if (component.hideHair)
				{
					SetFlag(Flags.Reserved4, b: true);
				}
				if (Object.op_Implicit((Object)(object)component.riderProtection))
				{
					riderProtection.Add(component.riderProtection, 1f);
				}
				if (Object.op_Implicit((Object)(object)component.animalProtection))
				{
					baseProtection.Add(component.animalProtection, 1f);
				}
				equipmentSpeedMod += component.speedModifier;
				numStorageSlots += component.additionalInventorySlots;
			}
		}
		for (int j = GetStorageStartIndex(); j < inventory.capacity; j++)
		{
			if (j >= GetStorageStartIndex() + numStorageSlots)
			{
				Item slot2 = inventory.GetSlot(j);
				if (slot2 != null)
				{
					slot2.RemoveFromContainer();
					slot2.Drop(((Component)this).get_transform().get_position() + Vector3.get_up() + Random.get_insideUnitSphere() * 0.25f, Vector3.get_zero());
				}
			}
		}
		inventory.capacity = GetStorageStartIndex() + numStorageSlots;
		SendNetworkUpdate();
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.horse != null)
		{
			staminaSeconds = info.msg.horse.staminaSeconds;
			currentMaxStaminaSeconds = info.msg.horse.currentMaxStaminaSeconds;
			numStorageSlots = info.msg.horse.numStorageSlots;
			ApplyBreed(info.msg.horse.breedIndex);
		}
	}

	protected override bool CanPushNow(BasePlayer pusher)
	{
		return false;
	}

	[ServerVar]
	public static void setHorseBreed(Arg arg)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = arg.Player();
		if ((Object)(object)basePlayer == (Object)null || !basePlayer.IsDeveloper)
		{
			return;
		}
		int @int = arg.GetInt(0, 0);
		List<RidableHorse> list = Pool.GetList<RidableHorse>();
		Vis.Entities(basePlayer.eyes.position, basePlayer.eyes.position + basePlayer.eyes.HeadForward() * 5f, 0f, list, -1, (QueryTriggerInteraction)2);
		foreach (RidableHorse item in list)
		{
			item.SetBreed(@int);
		}
		Pool.FreeList<RidableHorse>(ref list);
	}
}
