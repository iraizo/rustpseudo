using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class WorldItem : BaseEntity
{
	private bool _isInvokingSendItemUpdate;

	[Header("WorldItem")]
	public bool allowPickup = true;

	[NonSerialized]
	public Item item;

	protected float eatSeconds = 10f;

	protected float caloriesPerSecond = 1f;

	public override TraitFlag Traits
	{
		get
		{
			if (item != null)
			{
				return item.Traits;
			}
			return base.Traits;
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("WorldItem.OnRpcMessage", 0);
		try
		{
			if (rpc == 2778075470u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Pickup "));
				}
				TimeWarning val2 = TimeWarning.New("Pickup", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(2778075470u, "Pickup", this, player, 3f))
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
							Pickup(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Pickup");
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

	public override void ServerInit()
	{
		base.ServerInit();
		if (item != null)
		{
			((Component)this).BroadcastMessage("OnItemChanged", (object)item, (SendMessageOptions)1);
		}
	}

	private void DoItemNetworking()
	{
		if (!_isInvokingSendItemUpdate)
		{
			_isInvokingSendItemUpdate = true;
			((FacepunchBehaviour)this).Invoke((Action)SendItemUpdate, 0.1f);
		}
	}

	private void SendItemUpdate()
	{
		_isInvokingSendItemUpdate = false;
		if (item != null)
		{
			UpdateItem val = Pool.Get<UpdateItem>();
			try
			{
				val.item = item.Save(bIncludeContainer: false, bIncludeOwners: false);
				ClientRPC<UpdateItem>(null, "UpdateItem", val);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void Pickup(RPCMessage msg)
	{
		if (msg.player.CanInteract() && this.item != null && allowPickup)
		{
			ClientRPC(null, "PickupSound");
			Item item = this.item;
			RemoveItem();
			msg.player.GiveItem(item, GiveItemReason.PickedUp);
			msg.player.SignalBroadcast(Signal.Gesture, "pickup_item");
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (item != null)
		{
			bool forDisk = info.forDisk;
			info.msg.worldItem = Pool.Get<WorldItem>();
			info.msg.worldItem.item = item.Save(forDisk, bIncludeOwners: false);
		}
	}

	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		DestroyItem();
	}

	public override void SwitchParent(BaseEntity ent)
	{
		SetParent(ent, parentBone);
	}

	public override Item GetItem()
	{
		return item;
	}

	public void InitializeItem(Item in_item)
	{
		if (item != null)
		{
			RemoveItem();
		}
		item = in_item;
		if (item != null)
		{
			item.OnDirty += OnItemDirty;
			((Object)this).set_name(item.info.shortname + " (world)");
			item.SetWorldEntity(this);
			OnItemDirty(item);
		}
	}

	public void RemoveItem()
	{
		if (item != null)
		{
			item.OnDirty -= OnItemDirty;
			item = null;
		}
	}

	public void DestroyItem()
	{
		if (item != null)
		{
			item.OnDirty -= OnItemDirty;
			item.Remove();
			item = null;
		}
	}

	protected virtual void OnItemDirty(Item in_item)
	{
		Assert.IsTrue(item == in_item, "WorldItem:OnItemDirty - dirty item isn't ours!");
		if (item != null)
		{
			((Component)this).BroadcastMessage("OnItemChanged", (object)item, (SendMessageOptions)1);
		}
		DoItemNetworking();
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.worldItem != null && info.msg.worldItem.item != null)
		{
			Item item = ItemManager.Load(info.msg.worldItem.item, this.item, base.isServer);
			if (item != null)
			{
				InitializeItem(item);
			}
		}
	}

	public override void Eat(BaseNpc baseNpc, float timeSpent)
	{
		if (!(eatSeconds <= 0f))
		{
			eatSeconds -= timeSpent;
			baseNpc.AddCalories(caloriesPerSecond * timeSpent);
			if (eatSeconds < 0f)
			{
				DestroyItem();
				Kill();
			}
		}
	}

	public override string ToString()
	{
		if (_name == null)
		{
			if (base.isServer)
			{
				_name = string.Format("{1}[{0}] {2}", (net != null) ? net.ID : 0u, base.ShortPrefabName, ((Object)this).get_name());
			}
			else
			{
				_name = base.ShortPrefabName;
			}
		}
		return _name;
	}
}
