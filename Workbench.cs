using System;
using ConVar;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class Workbench : StorageContainer
{
	public const int blueprintSlot = 0;

	public const int experimentSlot = 1;

	public bool Static;

	public int Workbenchlevel;

	public LootSpawn experimentalItems;

	public GameObjectRef experimentStartEffect;

	public GameObjectRef experimentSuccessEffect;

	public ItemDefinition experimentResource;

	public TechTreeData techTree;

	public static ItemDefinition blueprintBaseDef;

	private ItemDefinition pendingBlueprint;

	private bool creatingBlueprint;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("Workbench.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 2308794761u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_BeginExperiment "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_BeginExperiment", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(2308794761u, "RPC_BeginExperiment", this, player, 3f))
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
							RPC_BeginExperiment(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_BeginExperiment");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 4127240744u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_TechTreeUnlock "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_TechTreeUnlock", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(4127240744u, "RPC_TechTreeUnlock", this, player, 3f))
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
							RPC_TechTreeUnlock(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_TechTreeUnlock");
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

	public int GetScrapForExperiment()
	{
		if (Workbenchlevel == 1)
		{
			return 75;
		}
		if (Workbenchlevel == 2)
		{
			return 300;
		}
		if (Workbenchlevel == 3)
		{
			return 1000;
		}
		Debug.LogWarning((object)"GetScrapForExperiment fucked up big time.");
		return 0;
	}

	public bool IsWorking()
	{
		return HasFlag(Flags.On);
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_TechTreeUnlock(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		int num = msg.read.Int32();
		TechTreeData.NodeInstance byID = techTree.GetByID(num);
		if (byID == null)
		{
			Debug.Log((object)("Node for unlock not found :" + num));
		}
		else
		{
			if (!techTree.PlayerCanUnlock(player, byID))
			{
				return;
			}
			if (byID.IsGroup())
			{
				foreach (int output in byID.outputs)
				{
					TechTreeData.NodeInstance byID2 = techTree.GetByID(output);
					if (byID2 != null && (Object)(object)byID2.itemDef != (Object)null)
					{
						player.blueprints.Unlock(byID2.itemDef);
					}
				}
				Debug.Log((object)("Player unlocked group :" + byID.groupName));
			}
			else if ((Object)(object)byID.itemDef != (Object)null)
			{
				int num2 = ResearchTable.ScrapForResearch(byID.itemDef);
				int itemid = ItemManager.FindItemDefinition("scrap").itemid;
				if (player.inventory.GetAmount(itemid) >= num2)
				{
					player.inventory.Take(null, itemid, num2);
					player.blueprints.Unlock(byID.itemDef);
				}
			}
		}
	}

	public static ItemDefinition GetBlueprintTemplate()
	{
		if ((Object)(object)blueprintBaseDef == (Object)null)
		{
			blueprintBaseDef = ItemManager.FindItemDefinition("blueprintbase");
		}
		return blueprintBaseDef;
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_BeginExperiment(RPCMessage msg)
	{
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer player = msg.player;
		if ((Object)(object)player == (Object)null || IsWorking())
		{
			return;
		}
		PersistantPlayer persistantPlayerInfo = player.PersistantPlayerInfo;
		int num = Random.Range(0, experimentalItems.subSpawn.Length);
		for (int i = 0; i < experimentalItems.subSpawn.Length; i++)
		{
			int num2 = i + num;
			if (num2 >= experimentalItems.subSpawn.Length)
			{
				num2 -= experimentalItems.subSpawn.Length;
			}
			ItemDefinition itemDef = experimentalItems.subSpawn[num2].category.items[0].itemDef;
			if (Object.op_Implicit((Object)(object)itemDef.Blueprint) && !itemDef.Blueprint.defaultBlueprint && itemDef.Blueprint.userCraftable && itemDef.Blueprint.isResearchable && !itemDef.Blueprint.NeedsSteamItem && !itemDef.Blueprint.NeedsSteamDLC && !persistantPlayerInfo.unlockedItems.Contains(itemDef.itemid))
			{
				pendingBlueprint = itemDef;
				break;
			}
		}
		if ((Object)(object)pendingBlueprint == (Object)null)
		{
			player.ChatMessage("You have already unlocked everything for this workbench tier.");
			return;
		}
		Item slot = base.inventory.GetSlot(0);
		if (slot != null)
		{
			if (!slot.MoveToContainer(player.inventory.containerMain))
			{
				slot.Drop(GetDropPosition(), GetDropVelocity());
			}
			player.inventory.loot.SendImmediate();
		}
		if (experimentStartEffect.isValid)
		{
			Effect.server.Run(experimentStartEffect.resourcePath, this, 0u, Vector3.get_zero(), Vector3.get_zero());
		}
		SetFlag(Flags.On, b: true);
		base.inventory.SetLocked(isLocked: true);
		((FacepunchBehaviour)this).CancelInvoke((Action)ExperimentComplete);
		((FacepunchBehaviour)this).Invoke((Action)ExperimentComplete, 5f);
		SendNetworkUpdate();
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
	}

	public override void OnKilled(HitInfo info)
	{
		base.OnKilled(info);
		((FacepunchBehaviour)this).CancelInvoke((Action)ExperimentComplete);
	}

	public int GetAvailableExperimentResources()
	{
		Item experimentResourceItem = GetExperimentResourceItem();
		if (experimentResourceItem == null || (Object)(object)experimentResourceItem.info != (Object)(object)experimentResource)
		{
			return 0;
		}
		return experimentResourceItem.amount;
	}

	public Item GetExperimentResourceItem()
	{
		return base.inventory.GetSlot(1);
	}

	public void ExperimentComplete()
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		Item experimentResourceItem = GetExperimentResourceItem();
		int scrapForExperiment = GetScrapForExperiment();
		if ((Object)(object)pendingBlueprint == (Object)null)
		{
			Debug.LogWarning((object)"Pending blueprint was null!");
		}
		if (experimentResourceItem != null && experimentResourceItem.amount >= scrapForExperiment && (Object)(object)pendingBlueprint != (Object)null)
		{
			experimentResourceItem.UseItem(scrapForExperiment);
			Item item = ItemManager.Create(GetBlueprintTemplate(), 1, 0uL);
			item.blueprintTarget = pendingBlueprint.itemid;
			creatingBlueprint = true;
			if (!item.MoveToContainer(base.inventory, 0))
			{
				item.Drop(GetDropPosition(), GetDropVelocity());
			}
			creatingBlueprint = false;
			if (experimentSuccessEffect.isValid)
			{
				Effect.server.Run(experimentSuccessEffect.resourcePath, this, 0u, Vector3.get_zero(), Vector3.get_zero());
			}
		}
		SetFlag(Flags.On, b: false);
		pendingBlueprint = null;
		base.inventory.SetLocked(isLocked: false);
		SendNetworkUpdate();
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		SetFlag(Flags.On, b: false);
		if (base.inventory != null)
		{
			base.inventory.SetLocked(isLocked: false);
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		base.inventory.canAcceptItem = ItemFilter;
	}

	public override bool ItemFilter(Item item, int targetSlot)
	{
		if ((targetSlot == 1 && (Object)(object)item.info == (Object)(object)experimentResource) || (targetSlot == 0 && creatingBlueprint))
		{
			return true;
		}
		return false;
	}
}
