using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public class InvisibleVendingMachine : NPCVendingMachine
{
	public GameObjectRef buyEffect;

	public NPCVendingOrderManifest vmoManifest;

	public NPCShopKeeper GetNPCShopKeeper()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		List<NPCShopKeeper> list = Pool.GetList<NPCShopKeeper>();
		Vis.Entities(((Component)this).get_transform().get_position(), 2f, list, 131072, (QueryTriggerInteraction)2);
		NPCShopKeeper result = null;
		if (list.Count > 0)
		{
			result = list[0];
		}
		Pool.FreeList<NPCShopKeeper>(ref list);
		return result;
	}

	public void KeeperLookAt(Vector3 pos)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		NPCShopKeeper nPCShopKeeper = GetNPCShopKeeper();
		if (!((Object)(object)nPCShopKeeper == (Object)null))
		{
			nPCShopKeeper.SetAimDirection(Vector3Ex.Direction2D(pos, ((Component)nPCShopKeeper).get_transform().get_position()));
		}
	}

	public override bool HasVendingSounds()
	{
		return false;
	}

	public override float GetBuyDuration()
	{
		return 0.5f;
	}

	public override void CompletePendingOrder()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		Effect.server.Run(buyEffect.resourcePath, ((Component)this).get_transform().get_position(), Vector3.get_up());
		NPCShopKeeper nPCShopKeeper = GetNPCShopKeeper();
		if (Object.op_Implicit((Object)(object)nPCShopKeeper))
		{
			nPCShopKeeper.SignalBroadcast(Signal.Gesture, "victory");
			if ((Object)(object)vend_Player != (Object)null)
			{
				nPCShopKeeper.SetAimDirection(Vector3Ex.Direction2D(((Component)vend_Player).get_transform().get_position(), ((Component)nPCShopKeeper).get_transform().get_position()));
			}
		}
		base.CompletePendingOrder();
	}

	public override bool PlayerOpenLoot(BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		KeeperLookAt(((Component)player).get_transform().get_position());
		return base.PlayerOpenLoot(player, panelToOpen);
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if ((Object)(object)vmoManifest != (Object)null && info.msg.vendingMachine != null)
		{
			info.msg.vendingMachine.vmoIndex = vmoManifest.GetIndex(vendingOrders);
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (vmoManifest.GetIndex(vendingOrders) == -1)
		{
			Debug.LogError((object)"VENDING ORDERS NOT FOUND! Did you forget to add these orders to the VMOManifest?");
		}
	}

	public override void Load(LoadInfo info)
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (!info.fromDisk || !((Object)(object)vmoManifest != (Object)null) || info.msg.vendingMachine == null)
		{
			return;
		}
		if (info.msg.vendingMachine.vmoIndex == -1 && TerrainMeta.Path.Monuments != null)
		{
			foreach (MonumentInfo monument in TerrainMeta.Path.Monuments)
			{
				if (monument.displayPhrase.token.Contains("fish") && Vector3.Distance(((Component)monument).get_transform().get_position(), ((Component)this).get_transform().get_position()) < 100f)
				{
					info.msg.vendingMachine.vmoIndex = 17;
				}
			}
		}
		NPCVendingOrder nPCVendingOrder = (vendingOrders = vmoManifest.GetFromIndex(info.msg.vendingMachine.vmoIndex));
	}
}
