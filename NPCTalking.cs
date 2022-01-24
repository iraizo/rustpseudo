using System;
using System.Collections.Generic;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class NPCTalking : NPCShopKeeper, IConversationProvider
{
	[Serializable]
	public class NPCConversationResultAction
	{
		public string action;

		public int scrapCost;

		public string broadcastMessage;

		public float broadcastRange;
	}

	public ConversationData[] conversations;

	public NPCConversationResultAction[] conversationResultActions;

	[NonSerialized]
	private float maxConversationDistance = 5f;

	private List<BasePlayer> conversingPlayers = new List<BasePlayer>();

	private BasePlayer lastActionPlayer;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("NPCTalking.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 4224060672u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ConversationAction "));
				}
				TimeWarning val2 = TimeWarning.New("ConversationAction", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(4224060672u, "ConversationAction", this, player, 5uL))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(4224060672u, "ConversationAction", this, player, 3f))
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
							ConversationAction(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in ConversationAction");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2112414875 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Server_BeginTalking "));
				}
				TimeWarning val2 = TimeWarning.New("Server_BeginTalking", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(2112414875u, "Server_BeginTalking", this, player, 1uL))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(2112414875u, "Server_BeginTalking", this, player, 3f))
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
							Server_BeginTalking(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in Server_BeginTalking");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1597539152 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Server_EndTalking "));
				}
				TimeWarning val2 = TimeWarning.New("Server_EndTalking", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(1597539152u, "Server_EndTalking", this, player, 1uL))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(1597539152u, "Server_EndTalking", this, player, 3f))
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
							RPCMessage msg4 = rPCMessage;
							Server_EndTalking(msg4);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in Server_EndTalking");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2713250658u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Server_ResponsePressed "));
				}
				TimeWarning val2 = TimeWarning.New("Server_ResponsePressed", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(2713250658u, "Server_ResponsePressed", this, player, 5uL))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(2713250658u, "Server_ResponsePressed", this, player, 3f))
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
							RPCMessage msg5 = rPCMessage;
							Server_ResponsePressed(msg5);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in Server_ResponsePressed");
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

	public int GetConversationIndex(string conversationName)
	{
		for (int i = 0; i < conversations.Length; i++)
		{
			if (conversations[i].shortname == conversationName)
			{
				return i;
			}
		}
		return -1;
	}

	public virtual string GetConversationStartSpeech(BasePlayer player)
	{
		return "intro";
	}

	public ConversationData GetConversation(string conversationName)
	{
		return GetConversation(GetConversationIndex(conversationName));
	}

	public ConversationData GetConversation(int index)
	{
		return conversations[index];
	}

	public virtual ConversationData GetConversationFor(BasePlayer player)
	{
		return conversations[0];
	}

	public bool ProviderBusy()
	{
		return HasFlag(Flags.Reserved1);
	}

	public void ForceEndConversation(BasePlayer player)
	{
		ClientRPCPlayer(null, player, "Client_EndConversation");
	}

	public void ForceSpeechNode(BasePlayer player, int speechNodeIndex)
	{
		if (!((Object)(object)player == (Object)null))
		{
			ClientRPCPlayer(null, player, "Client_ForceSpeechNode", speechNodeIndex);
		}
	}

	public virtual void OnConversationEnded(BasePlayer player)
	{
		if (conversingPlayers.Contains(player))
		{
			conversingPlayers.Remove(player);
		}
	}

	public void CleanupConversingPlayers()
	{
		for (int num = conversingPlayers.Count - 1; num >= 0; num--)
		{
			BasePlayer basePlayer = conversingPlayers[num];
			if ((Object)(object)basePlayer == (Object)null || !basePlayer.IsAlive() || basePlayer.IsSleeping())
			{
				conversingPlayers.RemoveAt(num);
			}
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	[RPC_Server.CallsPerSecond(1uL)]
	public void Server_BeginTalking(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		CleanupConversingPlayers();
		ConversationData conversationFor = GetConversationFor(player);
		if ((Object)(object)conversationFor != (Object)null)
		{
			if (conversingPlayers.Contains(player))
			{
				OnConversationEnded(player);
			}
			conversingPlayers.Add(player);
			UpdateFlags();
			OnConversationStarted(player);
			ClientRPCPlayer(null, player, "Client_StartConversation", GetConversationIndex(conversationFor.shortname), GetConversationStartSpeech(player));
		}
	}

	public virtual void OnConversationStarted(BasePlayer speakingTo)
	{
	}

	public virtual void UpdateFlags()
	{
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	[RPC_Server.CallsPerSecond(1uL)]
	public void Server_EndTalking(RPCMessage msg)
	{
		OnConversationEnded(msg.player);
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	[RPC_Server.CallsPerSecond(5uL)]
	public void ConversationAction(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		string action = msg.read.String(256);
		OnConversationAction(player, action);
	}

	public bool ValidConversationPlayer(BasePlayer player)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (Vector3.Distance(((Component)player).get_transform().get_position(), ((Component)this).get_transform().get_position()) > maxConversationDistance)
		{
			return false;
		}
		if (conversingPlayers.Contains(player))
		{
			return false;
		}
		return true;
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	[RPC_Server.CallsPerSecond(5uL)]
	public void Server_ResponsePressed(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		int num = msg.read.Int32();
		int num2 = msg.read.Int32();
		ConversationData conversationFor = GetConversationFor(player);
		if ((Object)(object)conversationFor == (Object)null)
		{
			return;
		}
		ConversationData.ResponseNode responseNode = conversationFor.speeches[num].responses[num2];
		if (responseNode != null)
		{
			if (responseNode.conditions.Length != 0)
			{
				UpdateFlags();
			}
			bool flag = responseNode.PassesConditions(player, this);
			if (flag && !string.IsNullOrEmpty(responseNode.actionString))
			{
				OnConversationAction(player, responseNode.actionString);
			}
			int speechNodeIndex = conversationFor.GetSpeechNodeIndex(flag ? responseNode.resultingSpeechNode : responseNode.GetFailedSpeechNode(player, this));
			if (speechNodeIndex == -1)
			{
				ForceEndConversation(player);
			}
			else
			{
				ForceSpeechNode(player, speechNodeIndex);
			}
		}
	}

	public BasePlayer GetActionPlayer()
	{
		return lastActionPlayer;
	}

	public virtual void OnConversationAction(BasePlayer player, string action)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (action == "openvending")
		{
			InvisibleVendingMachine vendingMachine = GetVendingMachine();
			if ((Object)(object)vendingMachine != (Object)null && Vector3.Distance(((Component)player).get_transform().get_position(), ((Component)this).get_transform().get_position()) < 5f)
			{
				ForceEndConversation(player);
				vendingMachine.PlayerOpenLoot(player, "vendingmachine.customer", doPositionChecks: false);
				return;
			}
		}
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition("scrap");
		NPCConversationResultAction[] array = conversationResultActions;
		foreach (NPCConversationResultAction nPCConversationResultAction in array)
		{
			if (!(nPCConversationResultAction.action == action))
			{
				continue;
			}
			CleanupConversingPlayers();
			foreach (BasePlayer conversingPlayer in conversingPlayers)
			{
				if (!((Object)(object)conversingPlayer == (Object)(object)player) && !((Object)(object)conversingPlayer == (Object)null))
				{
					int speechNodeIndex = GetConversationFor(player).GetSpeechNodeIndex("startbusy");
					ForceSpeechNode(conversingPlayer, speechNodeIndex);
				}
			}
			int num = nPCConversationResultAction.scrapCost;
			List<Item> list = player.inventory.FindItemIDs(itemDefinition.itemid);
			foreach (Item item in list)
			{
				num -= item.amount;
			}
			if (num > 0)
			{
				int speechNodeIndex2 = GetConversationFor(player).GetSpeechNodeIndex("toopoor");
				ForceSpeechNode(player, speechNodeIndex2);
				break;
			}
			num = nPCConversationResultAction.scrapCost;
			foreach (Item item2 in list)
			{
				int num2 = Mathf.Min(num, item2.amount);
				item2.UseItem(num2);
				num -= num2;
				if (num <= 0)
				{
					break;
				}
			}
			lastActionPlayer = player;
			BroadcastEntityMessage(nPCConversationResultAction.broadcastMessage, nPCConversationResultAction.broadcastRange);
			lastActionPlayer = null;
			break;
		}
	}
}
