using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class CollectibleEntity : BaseEntity, IPrefabPreProcess
{
	public Phrase itemName;

	public ItemAmount[] itemList;

	public GameObjectRef pickupEffect;

	public float xpScale = 1f;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("CollectibleEntity.OnRpcMessage", 0);
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
						if (!RPC_Server.MaxDistance.Test(2778075470u, "Pickup", this, player, 3f))
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

	public bool IsFood()
	{
		for (int i = 0; i < itemList.Length; i++)
		{
			if (itemList[i].itemDef.category == ItemCategory.Food)
			{
				return true;
			}
		}
		return false;
	}

	public void DoPickup(BasePlayer reciever)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		if (itemList == null)
		{
			return;
		}
		ItemAmount[] array = itemList;
		foreach (ItemAmount itemAmount in array)
		{
			Item item = ItemManager.Create(itemAmount.itemDef, (int)itemAmount.amount, 0uL);
			if (item != null)
			{
				if (Object.op_Implicit((Object)(object)reciever))
				{
					reciever.GiveItem(item, GiveItemReason.ResourceHarvested);
				}
				else
				{
					item.Drop(((Component)this).get_transform().get_position() + Vector3.get_up() * 0.5f, Vector3.get_up());
				}
			}
		}
		itemList = null;
		if (pickupEffect.isValid)
		{
			Effect.server.Run(pickupEffect.resourcePath, ((Component)this).get_transform().get_position(), ((Component)this).get_transform().get_up());
		}
		RandomItemDispenser randomItemDispenser = PrefabAttribute.server.Find<RandomItemDispenser>(prefabID);
		if (randomItemDispenser != null)
		{
			randomItemDispenser.DistributeItems(reciever, ((Component)this).get_transform().get_position());
		}
		Kill();
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void Pickup(RPCMessage msg)
	{
		if (msg.player.CanInteract())
		{
			DoPickup(msg.player);
		}
	}

	public override void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(preProcess, rootObj, name, serverside, clientside, bundling);
		if (serverside)
		{
			preProcess.RemoveComponent((Component)(object)((Component)this).GetComponent<Collider>());
		}
	}
}
