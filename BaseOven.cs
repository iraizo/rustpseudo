using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class BaseOven : StorageContainer, ISplashable
{
	public enum TemperatureType
	{
		Normal,
		Warming,
		Cooking,
		Smelting,
		Fractioning
	}

	public TemperatureType temperature;

	public Menu.Option switchOnMenu;

	public Menu.Option switchOffMenu;

	public ItemAmount[] startupContents;

	public bool allowByproductCreation = true;

	public ItemDefinition fuelType;

	public bool canModFire;

	public bool disabledBySplash = true;

	private const float UpdateRate = 0.5f;

	private float cookingTemperature => temperature switch
	{
		TemperatureType.Fractioning => 1500f, 
		TemperatureType.Cooking => 200f, 
		TemperatureType.Smelting => 1000f, 
		TemperatureType.Warming => 50f, 
		_ => 15f, 
	};

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("BaseOven.OnRpcMessage", 0);
		try
		{
			if (rpc == 4167839872u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SVSwitch "));
				}
				TimeWarning val2 = TimeWarning.New("SVSwitch", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(4167839872u, "SVSwitch", this, player, 3f))
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
							SVSwitch(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SVSwitch");
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

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (IsOn())
		{
			StartCooking();
		}
	}

	public override void OnInventoryFirstCreated(ItemContainer container)
	{
		base.OnInventoryFirstCreated(container);
		if (startupContents != null)
		{
			ItemAmount[] array = startupContents;
			foreach (ItemAmount itemAmount in array)
			{
				ItemManager.Create(itemAmount.itemDef, (int)itemAmount.amount, 0uL).MoveToContainer(container);
			}
		}
	}

	public override void OnItemAddedOrRemoved(Item item, bool bAdded)
	{
		base.OnItemAddedOrRemoved(item, bAdded);
		if (item != null && item.HasFlag(Item.Flag.OnFire))
		{
			item.SetFlag(Item.Flag.OnFire, b: false);
			item.MarkDirty();
		}
	}

	public void OvenFull()
	{
		StopCooking();
	}

	private Item FindBurnable()
	{
		if (base.inventory == null)
		{
			return null;
		}
		foreach (Item item in base.inventory.itemList)
		{
			if (Object.op_Implicit((Object)(object)((Component)item.info).GetComponent<ItemModBurnable>()) && ((Object)(object)fuelType == (Object)null || (Object)(object)item.info == (Object)(object)fuelType))
			{
				return item;
			}
		}
		return null;
	}

	public void Cook()
	{
		Item item = FindBurnable();
		if (item == null)
		{
			StopCooking();
			return;
		}
		base.inventory.OnCycle(0.5f);
		BaseEntity slot = GetSlot(Slot.FireMod);
		if (Object.op_Implicit((Object)(object)slot))
		{
			((Component)slot).SendMessage("Cook", (object)0.5f, (SendMessageOptions)1);
		}
		ItemModBurnable component = ((Component)item.info).GetComponent<ItemModBurnable>();
		item.fuel -= 0.5f * (cookingTemperature / 200f);
		if (!item.HasFlag(Item.Flag.OnFire))
		{
			item.SetFlag(Item.Flag.OnFire, b: true);
			item.MarkDirty();
		}
		if (item.fuel <= 0f)
		{
			ConsumeFuel(item, component);
		}
		OnCooked();
	}

	protected virtual void OnCooked()
	{
	}

	private void ConsumeFuel(Item fuel, ItemModBurnable burnable)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		if (allowByproductCreation && (Object)(object)burnable.byproductItem != (Object)null && Random.Range(0f, 1f) > burnable.byproductChance)
		{
			Item item = ItemManager.Create(burnable.byproductItem, burnable.byproductAmount, 0uL);
			if (!item.MoveToContainer(base.inventory))
			{
				OvenFull();
				item.Drop(base.inventory.dropPosition, base.inventory.dropVelocity);
			}
		}
		if (fuel.amount <= 1)
		{
			fuel.Remove();
			return;
		}
		fuel.amount--;
		fuel.fuel = burnable.fuelAmount;
		fuel.MarkDirty();
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	protected virtual void SVSwitch(RPCMessage msg)
	{
		bool flag = msg.read.Bit();
		if (flag != IsOn() && (!needsBuildingPrivilegeToUse || msg.player.CanBuild()))
		{
			if (flag)
			{
				StartCooking();
			}
			else
			{
				StopCooking();
			}
		}
	}

	public void UpdateAttachmentTemperature()
	{
		BaseEntity slot = GetSlot(Slot.FireMod);
		if (Object.op_Implicit((Object)(object)slot))
		{
			((Component)slot).SendMessage("ParentTemperatureUpdate", (object)base.inventory.temperature, (SendMessageOptions)1);
		}
	}

	public virtual void StartCooking()
	{
		if (FindBurnable() != null)
		{
			base.inventory.temperature = cookingTemperature;
			UpdateAttachmentTemperature();
			((FacepunchBehaviour)this).InvokeRepeating((Action)Cook, 0.5f, 0.5f);
			SetFlag(Flags.On, b: true);
		}
	}

	public virtual void StopCooking()
	{
		UpdateAttachmentTemperature();
		if (base.inventory != null)
		{
			base.inventory.temperature = 15f;
			foreach (Item item in base.inventory.itemList)
			{
				if (item.HasFlag(Item.Flag.OnFire))
				{
					item.SetFlag(Item.Flag.OnFire, b: false);
					item.MarkDirty();
				}
			}
		}
		((FacepunchBehaviour)this).CancelInvoke((Action)Cook);
		SetFlag(Flags.On, b: false);
	}

	public bool WantsSplash(ItemDefinition splashType, int amount)
	{
		if (!base.IsDestroyed && IsOn())
		{
			return disabledBySplash;
		}
		return false;
	}

	public int DoSplash(ItemDefinition splashType, int amount)
	{
		StopCooking();
		return Mathf.Min(200, amount);
	}

	public override bool HasSlot(Slot slot)
	{
		if (canModFire && slot == Slot.FireMod)
		{
			return true;
		}
		return base.HasSlot(slot);
	}
}
