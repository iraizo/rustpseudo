using System;
using System.Collections.Generic;
using CompanionServer;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class RelationshipManager : BaseEntity
{
	public enum RelationshipType
	{
		NONE,
		Acquaintance,
		Friend,
		Enemy
	}

	public class PlayerRelationshipInfo : IPooled, IServerFileReceiver
	{
		public string displayName;

		public ulong player;

		public RelationshipType type;

		public int weight;

		public uint mugshotCrc;

		public string notes;

		public float lastSeenTime;

		public float lastMugshotTime;

		public void EnterPool()
		{
			Reset();
		}

		public void LeavePool()
		{
			Reset();
		}

		private void Reset()
		{
			displayName = null;
			player = 0uL;
			type = RelationshipType.NONE;
			weight = 0;
			mugshotCrc = 0u;
			notes = "";
			lastMugshotTime = 0f;
		}

		public void OnServerFileReceived(FileStorage.Type type, uint numId, uint crc, byte[] data)
		{
		}
	}

	public class PlayerRelationships : IPooled
	{
		public bool dirty;

		public ulong ownerPlayer;

		public Dictionary<ulong, PlayerRelationshipInfo> relations;

		public bool Forget(ulong player)
		{
			if (relations.TryGetValue(player, out var value))
			{
				relations.Remove(player);
				if (value.mugshotCrc != 0)
				{
					ServerInstance.DeleteMugshot(ownerPlayer, player, value.mugshotCrc);
				}
				return true;
			}
			return false;
		}

		public PlayerRelationshipInfo GetRelations(ulong player)
		{
			BasePlayer basePlayer = FindByID(player);
			if (relations.TryGetValue(player, out var value))
			{
				if ((Object)(object)basePlayer != (Object)null)
				{
					value.displayName = basePlayer.displayName;
				}
				return value;
			}
			PlayerRelationshipInfo playerRelationshipInfo = Pool.Get<PlayerRelationshipInfo>();
			if ((Object)(object)basePlayer != (Object)null)
			{
				playerRelationshipInfo.displayName = basePlayer.displayName;
			}
			playerRelationshipInfo.player = player;
			relations.Add(player, playerRelationshipInfo);
			return playerRelationshipInfo;
		}

		public PlayerRelationships()
		{
			LeavePool();
		}

		public void EnterPool()
		{
			ownerPlayer = 0uL;
			if (relations != null)
			{
				relations.Clear();
				Pool.Free<Dictionary<ulong, PlayerRelationshipInfo>>(ref relations);
			}
		}

		public void LeavePool()
		{
			ownerPlayer = 0uL;
			relations = Pool.Get<Dictionary<ulong, PlayerRelationshipInfo>>();
			relations.Clear();
		}
	}

	public class PlayerTeam
	{
		public ulong teamID;

		public string teamName;

		public ulong teamLeader;

		public List<ulong> members = new List<ulong>();

		public List<ulong> invites = new List<ulong>();

		public float teamStartTime;

		private List<Connection> onlineMemberConnections = new List<Connection>();

		public float teamLifetime => Time.get_realtimeSinceStartup() - teamStartTime;

		public BasePlayer GetLeader()
		{
			return FindByID(teamLeader);
		}

		public void SendInvite(BasePlayer player)
		{
			if (invites.Count > 8)
			{
				invites.RemoveRange(0, 1);
			}
			BasePlayer basePlayer = FindByID(teamLeader);
			if (!((Object)(object)basePlayer == (Object)null))
			{
				invites.Add(player.userID);
				player.ClientRPCPlayer(null, player, "CLIENT_PendingInvite", basePlayer.displayName, teamLeader, teamID);
			}
		}

		public void AcceptInvite(BasePlayer player)
		{
			if (invites.Contains(player.userID))
			{
				invites.Remove(player.userID);
				AddPlayer(player);
				player.ClearPendingInvite();
			}
		}

		public void RejectInvite(BasePlayer player)
		{
			player.ClearPendingInvite();
			invites.Remove(player.userID);
		}

		public bool AddPlayer(BasePlayer player)
		{
			ulong userID = player.userID;
			if (members.Contains(userID))
			{
				return false;
			}
			if (player.currentTeam != 0L)
			{
				return false;
			}
			if (members.Count >= maxTeamSize)
			{
				return false;
			}
			player.currentTeam = teamID;
			members.Add(userID);
			ServerInstance.playerToTeam.Add(userID, this);
			MarkDirty();
			player.SendNetworkUpdate();
			return true;
		}

		public bool RemovePlayer(ulong playerID)
		{
			if (members.Contains(playerID))
			{
				members.Remove(playerID);
				ServerInstance.playerToTeam.Remove(playerID);
				BasePlayer basePlayer = FindByID(playerID);
				if ((Object)(object)basePlayer != (Object)null)
				{
					basePlayer.ClearTeam();
					basePlayer.BroadcastAppTeamRemoval();
				}
				if (teamLeader == playerID)
				{
					if (members.Count > 0)
					{
						SetTeamLeader(members[0]);
					}
					else
					{
						Disband();
					}
				}
				MarkDirty();
				return true;
			}
			return false;
		}

		public void SetTeamLeader(ulong newTeamLeader)
		{
			teamLeader = newTeamLeader;
			MarkDirty();
		}

		public void Disband()
		{
			ServerInstance.DisbandTeam(this);
			CompanionServer.Server.TeamChat.Remove(teamID);
		}

		public void MarkDirty()
		{
			foreach (ulong member in members)
			{
				BasePlayer basePlayer = FindByID(member);
				if ((Object)(object)basePlayer != (Object)null)
				{
					basePlayer.UpdateTeam(teamID);
				}
			}
			this.BroadcastAppTeamUpdate();
		}

		public List<Connection> GetOnlineMemberConnections()
		{
			if (members.Count == 0)
			{
				return null;
			}
			onlineMemberConnections.Clear();
			foreach (ulong member in members)
			{
				BasePlayer basePlayer = FindByID(member);
				if (!((Object)(object)basePlayer == (Object)null) && basePlayer.Connection != null)
				{
					onlineMemberConnections.Add(basePlayer.Connection);
				}
			}
			return onlineMemberConnections;
		}
	}

	private const int MugshotResolution = 256;

	private const int MugshotMaxFileSize = 65536;

	private const float MugshotMaxDistance = 50f;

	public Dictionary<ulong, PlayerRelationships> relationships = new Dictionary<ulong, PlayerRelationships>();

	private int lastReputationUpdateIndex;

	private const int seenReputationSeconds = 60;

	private int startingReputation;

	[ServerVar]
	public static bool contacts = true;

	[ServerVar]
	public static int forgetafterminutes = 960;

	[ServerVar]
	public static int maxplayerrelationships = 128;

	[ServerVar]
	public static float seendistance = 10f;

	[ServerVar]
	public static float mugshotUpdateInterval = 300f;

	private static List<BasePlayer> _dirtyRelationshipPlayers = new List<BasePlayer>();

	public static int maxTeamSize_Internal = 8;

	public Dictionary<ulong, BasePlayer> cachedPlayers = new Dictionary<ulong, BasePlayer>();

	public Dictionary<ulong, PlayerTeam> playerToTeam = new Dictionary<ulong, PlayerTeam>();

	public Dictionary<ulong, PlayerTeam> teams = new Dictionary<ulong, PlayerTeam>();

	private ulong lastTeamIndex = 1uL;

	[ServerVar]
	public static int maxTeamSize
	{
		get
		{
			return maxTeamSize_Internal;
		}
		set
		{
			maxTeamSize_Internal = value;
			if (Object.op_Implicit((Object)(object)ServerInstance))
			{
				ServerInstance.SendNetworkUpdate();
			}
		}
	}

	public static RelationshipManager ServerInstance { get; private set; }

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("RelationshipManager.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 1684577101 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SERVER_ChangeRelationship "));
				}
				TimeWarning val2 = TimeWarning.New("SERVER_ChangeRelationship", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(1684577101u, "SERVER_ChangeRelationship", this, player, 2uL))
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
							SERVER_ChangeRelationship(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SERVER_ChangeRelationship");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1239936737 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SERVER_ReceiveMugshot "));
				}
				TimeWarning val2 = TimeWarning.New("SERVER_ReceiveMugshot", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(1239936737u, "SERVER_ReceiveMugshot", this, player, 10uL))
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
							SERVER_ReceiveMugshot(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in SERVER_ReceiveMugshot");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2178173141u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SERVER_SendFreshContacts "));
				}
				TimeWarning val2 = TimeWarning.New("SERVER_SendFreshContacts", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(2178173141u, "SERVER_SendFreshContacts", this, player, 1uL))
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
							SERVER_SendFreshContacts(msg4);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in SERVER_SendFreshContacts");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 290196604 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SERVER_UpdatePlayerNote "));
				}
				TimeWarning val2 = TimeWarning.New("SERVER_UpdatePlayerNote", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(290196604u, "SERVER_UpdatePlayerNote", this, player, 10uL))
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
							SERVER_UpdatePlayerNote(msg5);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in SERVER_UpdatePlayerNote");
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
		if (contacts)
		{
			((FacepunchBehaviour)this).InvokeRepeating((Action)UpdateContactsTick, 0f, 1f);
			((FacepunchBehaviour)this).InvokeRepeating((Action)UpdateReputations, 0f, 0.05f);
			((FacepunchBehaviour)this).InvokeRepeating((Action)SendRelationships, 0f, 5f);
		}
	}

	public void UpdateReputations()
	{
		if (contacts && BasePlayer.activePlayerList.get_Count() != 0)
		{
			if (lastReputationUpdateIndex >= BasePlayer.activePlayerList.get_Count())
			{
				lastReputationUpdateIndex = 0;
			}
			BasePlayer basePlayer = BasePlayer.activePlayerList.get_Item(lastReputationUpdateIndex);
			if (basePlayer.reputation != (basePlayer.reputation = GetReputationFor(basePlayer.userID)))
			{
				basePlayer.SendNetworkUpdate();
			}
			lastReputationUpdateIndex++;
		}
	}

	public void UpdateContactsTick()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (!contacts)
		{
			return;
		}
		Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BasePlayer current = enumerator.get_Current();
				UpdateAcquaintancesFor(current, 1f);
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
	}

	public int GetReputationFor(ulong playerID)
	{
		int num = startingReputation;
		foreach (PlayerRelationships value in relationships.Values)
		{
			foreach (KeyValuePair<ulong, PlayerRelationshipInfo> relation in value.relations)
			{
				if (relation.Key != playerID)
				{
					continue;
				}
				if (relation.Value.type == RelationshipType.Friend)
				{
					num++;
				}
				else if (relation.Value.type == RelationshipType.Acquaintance)
				{
					if (relation.Value.weight > 60)
					{
						num++;
					}
				}
				else if (relation.Value.type == RelationshipType.Enemy)
				{
					num--;
				}
			}
		}
		return num;
	}

	[ServerVar]
	public static void wipecontacts(Arg arg)
	{
		BasePlayer basePlayer = arg.Player();
		if (!((Object)(object)basePlayer == (Object)null) && !((Object)(object)ServerInstance == (Object)null))
		{
			ulong userID = basePlayer.userID;
			if (ServerInstance.relationships.ContainsKey(userID))
			{
				Debug.Log((object)("Wiped contacts for :" + userID));
				ServerInstance.relationships.Remove(userID);
				ServerInstance.MarkRelationshipsDirtyFor(userID);
			}
			else
			{
				Debug.Log((object)("No contacts for :" + userID));
			}
		}
	}

	[ServerVar]
	public static void wipe_all_contacts(Arg arg)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = arg.Player();
		if ((Object)(object)basePlayer == (Object)null || (Object)(object)ServerInstance == (Object)null)
		{
			return;
		}
		if (!arg.HasArgs(1) || arg.Args[0] != "confirm")
		{
			Debug.Log((object)"Please append the word 'confirm' at the end of the console command to execute");
			return;
		}
		_ = basePlayer.userID;
		ServerInstance.relationships.Clear();
		Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BasePlayer current = enumerator.get_Current();
				ServerInstance.MarkRelationshipsDirtyFor(current);
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		Debug.Log((object)"Wiped all contacts.");
	}

	public float GetAcquaintanceMaxDist()
	{
		return seendistance;
	}

	public void UpdateAcquaintancesFor(BasePlayer player, float deltaSeconds)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		PlayerRelationships playerRelationships = GetRelationships(player.userID);
		List<BasePlayer> list = Pool.GetList<BasePlayer>();
		BaseNetworkable.GetCloseConnections(((Component)player).get_transform().get_position(), GetAcquaintanceMaxDist(), list);
		foreach (BasePlayer item in list)
		{
			if ((Object)(object)item == (Object)(object)player || item.isClient || !item.IsAlive() || item.IsSleeping())
			{
				continue;
			}
			PlayerRelationshipInfo relations = playerRelationships.GetRelations(item.userID);
			if (!(Vector3.Distance(((Component)player).get_transform().get_position(), ((Component)item).get_transform().get_position()) <= GetAcquaintanceMaxDist()))
			{
				continue;
			}
			relations.lastSeenTime = Time.get_realtimeSinceStartup();
			if ((relations.type == RelationshipType.NONE || relations.type == RelationshipType.Acquaintance) && player.IsPlayerVisibleToUs(item, 1218519041))
			{
				int num = Mathf.CeilToInt(deltaSeconds);
				if (player.InSafeZone() || item.InSafeZone())
				{
					num = 0;
				}
				if (relations.type != RelationshipType.Acquaintance || (relations.weight < 60 && num > 0))
				{
					SetRelationship(player, item, RelationshipType.Acquaintance, num);
				}
			}
		}
		Pool.FreeList<BasePlayer>(ref list);
	}

	public void SetSeen(BasePlayer player, BasePlayer otherPlayer)
	{
		ulong userID = player.userID;
		ulong userID2 = otherPlayer.userID;
		PlayerRelationshipInfo relations = GetRelationships(userID).GetRelations(userID2);
		if (relations.type != 0)
		{
			relations.lastSeenTime = Time.get_realtimeSinceStartup();
		}
	}

	public bool CleanupOldContacts(PlayerRelationships ownerRelationships, ulong playerID, RelationshipType relationshipType = RelationshipType.Acquaintance)
	{
		int numberRelationships = GetNumberRelationships(playerID);
		if (numberRelationships < maxplayerrelationships)
		{
			return true;
		}
		List<ulong> list = Pool.GetList<ulong>();
		foreach (KeyValuePair<ulong, PlayerRelationshipInfo> relation in ownerRelationships.relations)
		{
			if (relation.Value.type == relationshipType && Time.get_realtimeSinceStartup() - relation.Value.lastSeenTime > (float)forgetafterminutes * 60f)
			{
				list.Add(relation.Key);
			}
		}
		int count = list.Count;
		foreach (ulong item in list)
		{
			ownerRelationships.Forget(item);
		}
		Pool.FreeList<ulong>(ref list);
		return numberRelationships - count < maxplayerrelationships;
	}

	public void ForceRelationshipByID(BasePlayer player, ulong otherPlayerID, RelationshipType newType, int weight, bool sendImmediate = false)
	{
		if (!contacts || (Object)(object)player == (Object)null || player.userID == otherPlayerID || player.IsNpc)
		{
			return;
		}
		ulong userID = player.userID;
		if (HasRelations(userID, otherPlayerID))
		{
			PlayerRelationshipInfo relations = GetRelationships(userID).GetRelations(otherPlayerID);
			if (relations.type != newType)
			{
				relations.weight = 0;
			}
			relations.type = newType;
			relations.weight += weight;
			if (sendImmediate)
			{
				SendRelationshipsFor(player);
			}
			else
			{
				MarkRelationshipsDirtyFor(player);
			}
		}
	}

	public void SetRelationship(BasePlayer player, BasePlayer otherPlayer, RelationshipType type, int weight = 1, bool sendImmediate = false)
	{
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		if (!contacts)
		{
			return;
		}
		ulong userID = player.userID;
		ulong userID2 = otherPlayer.userID;
		if ((Object)(object)player == (Object)null || (Object)(object)player == (Object)(object)otherPlayer || player.IsNpc || ((Object)(object)otherPlayer != (Object)null && otherPlayer.IsNpc))
		{
			return;
		}
		PlayerRelationships playerRelationships = GetRelationships(userID);
		if (!CleanupOldContacts(playerRelationships, userID))
		{
			CleanupOldContacts(playerRelationships, userID, RelationshipType.Enemy);
		}
		PlayerRelationshipInfo relations = playerRelationships.GetRelations(userID2);
		bool flag = false;
		if (relations.type != type)
		{
			flag = true;
			relations.weight = 0;
		}
		relations.type = type;
		relations.weight += weight;
		float num = Time.get_realtimeSinceStartup() - relations.lastMugshotTime;
		if (flag || relations.mugshotCrc == 0 || num >= mugshotUpdateInterval)
		{
			bool flag2 = otherPlayer.IsAlive();
			bool num2 = player.SecondsSinceAttacked > 10f && !player.IsAiming;
			float num3 = 100f;
			if (num2)
			{
				Vector3 val = otherPlayer.eyes.position - player.eyes.position;
				Vector3 normalized = ((Vector3)(ref val)).get_normalized();
				bool flag3 = Vector3.Dot(player.eyes.HeadForward(), normalized) >= 0.6f;
				float num4 = Vector3Ex.Distance2D(((Component)player).get_transform().get_position(), ((Component)otherPlayer).get_transform().get_position());
				if (flag2 && num4 < num3 && flag3)
				{
					ClientRPCPlayer(null, player, "CLIENT_DoMugshot", userID2);
					relations.lastMugshotTime = Time.get_realtimeSinceStartup();
				}
			}
		}
		if (sendImmediate)
		{
			SendRelationshipsFor(player);
		}
		else
		{
			MarkRelationshipsDirtyFor(player);
		}
	}

	public PlayerRelationships GetRelationshipSaveByID(ulong playerID)
	{
		PlayerRelationships val = Pool.Get<PlayerRelationships>();
		PlayerRelationships playerRelationships = GetRelationships(playerID);
		if (playerRelationships != null)
		{
			val.playerID = playerID;
			val.relations = Pool.GetList<PlayerRelationshipInfo>();
			{
				foreach (KeyValuePair<ulong, PlayerRelationshipInfo> relation in playerRelationships.relations)
				{
					PlayerRelationshipInfo val2 = Pool.Get<PlayerRelationshipInfo>();
					val2.playerID = relation.Value.player;
					val2.type = (int)relation.Value.type;
					val2.weight = relation.Value.weight;
					val2.mugshotCrc = relation.Value.mugshotCrc;
					val2.displayName = relation.Value.displayName;
					val2.notes = relation.Value.notes;
					val2.timeSinceSeen = Time.get_realtimeSinceStartup() - relation.Value.lastSeenTime;
					val.relations.Add(val2);
				}
				return val;
			}
		}
		return null;
	}

	public void MarkRelationshipsDirtyFor(ulong playerID)
	{
		BasePlayer basePlayer = FindByID(playerID);
		if (Object.op_Implicit((Object)(object)basePlayer))
		{
			MarkRelationshipsDirtyFor(basePlayer);
		}
	}

	public static void ForceSendRelationships(BasePlayer player)
	{
		if (Object.op_Implicit((Object)(object)ServerInstance))
		{
			ServerInstance.MarkRelationshipsDirtyFor(player);
		}
	}

	public void MarkRelationshipsDirtyFor(BasePlayer player)
	{
		if (!((Object)(object)player == (Object)null))
		{
			if (!_dirtyRelationshipPlayers.Contains(player))
			{
				_dirtyRelationshipPlayers.Add(player);
			}
			_ = player.userID;
		}
	}

	public void SendRelationshipsFor(BasePlayer player)
	{
		if (contacts)
		{
			ulong userID = player.userID;
			PlayerRelationships relationshipSaveByID = GetRelationshipSaveByID(userID);
			ClientRPCPlayer<PlayerRelationships>(null, player, "CLIENT_RecieveLocalRelationships", relationshipSaveByID);
		}
	}

	public void SendRelationships()
	{
		if (!contacts)
		{
			return;
		}
		foreach (BasePlayer dirtyRelationshipPlayer in _dirtyRelationshipPlayers)
		{
			if (!((Object)(object)dirtyRelationshipPlayer == (Object)null) && dirtyRelationshipPlayer.IsConnected && !dirtyRelationshipPlayer.IsSleeping())
			{
				SendRelationshipsFor(dirtyRelationshipPlayer);
			}
		}
		_dirtyRelationshipPlayers.Clear();
	}

	public int GetNumberRelationships(ulong player)
	{
		if (relationships.TryGetValue(player, out var value))
		{
			return value.relations.Count;
		}
		return 0;
	}

	public bool HasRelations(ulong player, ulong otherPlayer)
	{
		if (relationships.TryGetValue(player, out var value) && value.relations.ContainsKey(otherPlayer))
		{
			return true;
		}
		return false;
	}

	public PlayerRelationships GetRelationships(ulong player)
	{
		if (relationships.TryGetValue(player, out var value))
		{
			return value;
		}
		PlayerRelationships playerRelationships = Pool.Get<PlayerRelationships>();
		playerRelationships.ownerPlayer = player;
		relationships.Add(player, playerRelationships);
		return playerRelationships;
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(1uL)]
	public void SERVER_SendFreshContacts(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (Object.op_Implicit((Object)(object)player))
		{
			SendRelationshipsFor(player);
		}
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(2uL)]
	public void SERVER_ChangeRelationship(RPCMessage msg)
	{
		ulong userID = msg.player.userID;
		ulong num = msg.read.UInt64();
		int num2 = Mathf.Clamp(msg.read.Int32(), 0, 3);
		PlayerRelationships playerRelationships = GetRelationships(userID);
		playerRelationships.GetRelations(num);
		BasePlayer player = msg.player;
		RelationshipType relationshipType = (RelationshipType)num2;
		if (num2 == 0)
		{
			if (playerRelationships.Forget(num))
			{
				SendRelationshipsFor(player);
			}
			return;
		}
		BasePlayer basePlayer = FindByID(num);
		if ((Object)(object)basePlayer == (Object)null)
		{
			ForceRelationshipByID(player, num, relationshipType, 0, sendImmediate: true);
		}
		else
		{
			SetRelationship(player, basePlayer, relationshipType, 1, sendImmediate: true);
		}
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(10uL)]
	public void SERVER_UpdatePlayerNote(RPCMessage msg)
	{
		ulong userID = msg.player.userID;
		ulong player = msg.read.UInt64();
		string notes = msg.read.String(256);
		GetRelationships(userID).GetRelations(player).notes = notes;
		MarkRelationshipsDirtyFor(userID);
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(10uL)]
	public void SERVER_ReceiveMugshot(RPCMessage msg)
	{
		ulong userID = msg.player.userID;
		ulong num = msg.read.UInt64();
		uint num2 = msg.read.UInt32();
		byte[] array = msg.read.BytesWithSize(65536u);
		if (array != null && ImageProcessing.IsValidJPG(array, 256, 512) && relationships.TryGetValue(userID, out var value) && value.relations.TryGetValue(num, out var value2))
		{
			uint steamIdHash = GetSteamIdHash(userID, num);
			uint num3 = FileStorage.server.Store(array, FileStorage.Type.jpg, net.ID, steamIdHash);
			if (num3 != num2)
			{
				Debug.LogWarning((object)"Client/Server FileStorage CRC differs");
			}
			if (num3 != value2.mugshotCrc)
			{
				FileStorage.server.RemoveExact(value2.mugshotCrc, FileStorage.Type.jpg, net.ID, steamIdHash);
			}
			value2.mugshotCrc = num3;
			MarkRelationshipsDirtyFor(userID);
		}
	}

	private void DeleteMugshot(ulong steamId, ulong targetSteamId, uint crc)
	{
		if (crc != 0)
		{
			uint steamIdHash = GetSteamIdHash(steamId, targetSteamId);
			FileStorage.server.RemoveExact(crc, FileStorage.Type.jpg, net.ID, steamIdHash);
		}
	}

	private static uint GetSteamIdHash(ulong requesterSteamId, ulong targetSteamId)
	{
		return (uint)(((requesterSteamId & 0xFFFF) << 16) | (targetSteamId & 0xFFFF));
	}

	public int GetMaxTeamSize()
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(serverside: true);
		if (Object.op_Implicit((Object)(object)activeGameMode))
		{
			return activeGameMode.GetMaxRelationshipTeamSize();
		}
		return maxTeamSize;
	}

	public void OnEnable()
	{
		if (base.isServer)
		{
			if ((Object)(object)ServerInstance != (Object)null)
			{
				Debug.LogError((object)"Major fuckup! RelationshipManager spawned twice, Contact Developers!");
				Object.Destroy((Object)(object)((Component)this).get_gameObject());
			}
			else
			{
				ServerInstance = this;
			}
		}
	}

	public void OnDestroy()
	{
		if (base.isServer)
		{
			ServerInstance = null;
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.relationshipManager = Pool.Get<RelationshipManager>();
		info.msg.relationshipManager.maxTeamSize = maxTeamSize;
		if (!info.forDisk)
		{
			return;
		}
		info.msg.relationshipManager.lastTeamIndex = lastTeamIndex;
		info.msg.relationshipManager.teamList = Pool.GetList<PlayerTeam>();
		foreach (KeyValuePair<ulong, PlayerTeam> team in teams)
		{
			PlayerTeam value = team.Value;
			if (value == null)
			{
				continue;
			}
			PlayerTeam val = Pool.Get<PlayerTeam>();
			val.teamLeader = value.teamLeader;
			val.teamID = value.teamID;
			val.teamName = value.teamName;
			val.members = Pool.GetList<TeamMember>();
			foreach (ulong member in value.members)
			{
				TeamMember val2 = Pool.Get<TeamMember>();
				BasePlayer basePlayer = FindByID(member);
				val2.displayName = (((Object)(object)basePlayer != (Object)null) ? basePlayer.displayName : (SingletonComponent<ServerMgr>.Instance.persistance.GetPlayerName(member) ?? "DEAD"));
				val2.userID = member;
				val.members.Add(val2);
			}
			info.msg.relationshipManager.teamList.Add(val);
		}
		info.msg.relationshipManager.relationships = Pool.GetList<PlayerRelationships>();
		foreach (ulong key in relationships.Keys)
		{
			_ = relationships[key];
			PlayerRelationships relationshipSaveByID = GetRelationshipSaveByID(key);
			info.msg.relationshipManager.relationships.Add(relationshipSaveByID);
		}
	}

	public void DisbandTeam(PlayerTeam teamToDisband)
	{
		teams.Remove(teamToDisband.teamID);
		Pool.Free<PlayerTeam>(ref teamToDisband);
	}

	public static BasePlayer FindByID(ulong userID)
	{
		BasePlayer value = null;
		if (ServerInstance.cachedPlayers.TryGetValue(userID, out value))
		{
			if ((Object)(object)value != (Object)null)
			{
				return value;
			}
			ServerInstance.cachedPlayers.Remove(userID);
		}
		BasePlayer basePlayer = BasePlayer.FindByID(userID);
		if (!Object.op_Implicit((Object)(object)basePlayer))
		{
			basePlayer = BasePlayer.FindSleeping(userID);
		}
		if ((Object)(object)basePlayer != (Object)null)
		{
			ServerInstance.cachedPlayers.Add(userID, basePlayer);
		}
		return basePlayer;
	}

	public PlayerTeam FindTeam(ulong TeamID)
	{
		if (teams.ContainsKey(TeamID))
		{
			return teams[TeamID];
		}
		return null;
	}

	public PlayerTeam FindPlayersTeam(ulong userID)
	{
		if (playerToTeam.TryGetValue(userID, out var value))
		{
			return value;
		}
		return null;
	}

	public PlayerTeam CreateTeam()
	{
		PlayerTeam playerTeam = Pool.Get<PlayerTeam>();
		playerTeam.teamID = lastTeamIndex;
		playerTeam.teamStartTime = Time.get_realtimeSinceStartup();
		teams.Add(lastTeamIndex, playerTeam);
		lastTeamIndex++;
		return playerTeam;
	}

	[ServerUserVar]
	public static void trycreateteam(Arg arg)
	{
		if (maxTeamSize == 0)
		{
			arg.ReplyWith("Teams are disabled on this server");
			return;
		}
		BasePlayer basePlayer = arg.Player();
		if (basePlayer.currentTeam == 0L)
		{
			PlayerTeam playerTeam = ServerInstance.CreateTeam();
			playerTeam.teamLeader = basePlayer.userID;
			playerTeam.AddPlayer(basePlayer);
		}
	}

	[ServerUserVar]
	public static void promote(Arg arg)
	{
		BasePlayer basePlayer = arg.Player();
		if (basePlayer.currentTeam == 0L)
		{
			return;
		}
		BasePlayer lookingAtPlayer = GetLookingAtPlayer(basePlayer);
		if (!((Object)(object)lookingAtPlayer == (Object)null) && !lookingAtPlayer.IsDead() && !((Object)(object)lookingAtPlayer == (Object)(object)basePlayer) && lookingAtPlayer.currentTeam == basePlayer.currentTeam)
		{
			PlayerTeam playerTeam = ServerInstance.teams[basePlayer.currentTeam];
			if (playerTeam != null && playerTeam.teamLeader == basePlayer.userID)
			{
				playerTeam.SetTeamLeader(lookingAtPlayer.userID);
			}
		}
	}

	[ServerUserVar]
	public static void leaveteam(Arg arg)
	{
		BasePlayer basePlayer = arg.Player();
		if (!((Object)(object)basePlayer == (Object)null) && basePlayer.currentTeam != 0L)
		{
			PlayerTeam playerTeam = ServerInstance.FindTeam(basePlayer.currentTeam);
			if (playerTeam != null)
			{
				playerTeam.RemovePlayer(basePlayer.userID);
				basePlayer.ClearTeam();
			}
		}
	}

	[ServerUserVar]
	public static void acceptinvite(Arg arg)
	{
		BasePlayer basePlayer = arg.Player();
		if (!((Object)(object)basePlayer == (Object)null) && basePlayer.currentTeam == 0L)
		{
			ulong uLong = arg.GetULong(0, 0uL);
			PlayerTeam playerTeam = ServerInstance.FindTeam(uLong);
			if (playerTeam == null)
			{
				basePlayer.ClearPendingInvite();
			}
			else
			{
				playerTeam.AcceptInvite(basePlayer);
			}
		}
	}

	[ServerUserVar]
	public static void rejectinvite(Arg arg)
	{
		BasePlayer basePlayer = arg.Player();
		if (!((Object)(object)basePlayer == (Object)null) && basePlayer.currentTeam == 0L)
		{
			ulong uLong = arg.GetULong(0, 0uL);
			PlayerTeam playerTeam = ServerInstance.FindTeam(uLong);
			if (playerTeam == null)
			{
				basePlayer.ClearPendingInvite();
			}
			else
			{
				playerTeam.RejectInvite(basePlayer);
			}
		}
	}

	public static BasePlayer GetLookingAtPlayer(BasePlayer source)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		RaycastHit hit = default(RaycastHit);
		if (Physics.Raycast(source.eyes.position, source.eyes.HeadForward(), ref hit, 5f, 1218652417, (QueryTriggerInteraction)1))
		{
			BaseEntity entity = hit.GetEntity();
			if (Object.op_Implicit((Object)(object)entity))
			{
				return ((Component)entity).GetComponent<BasePlayer>();
			}
		}
		return null;
	}

	[ServerVar]
	public static void sleeptoggle(Arg arg)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = arg.Player();
		RaycastHit hit = default(RaycastHit);
		if ((Object)(object)basePlayer == (Object)null || !Physics.Raycast(basePlayer.eyes.position, basePlayer.eyes.HeadForward(), ref hit, 5f, 1218652417, (QueryTriggerInteraction)1))
		{
			return;
		}
		BaseEntity entity = hit.GetEntity();
		if (!Object.op_Implicit((Object)(object)entity))
		{
			return;
		}
		BasePlayer component = ((Component)entity).GetComponent<BasePlayer>();
		if (Object.op_Implicit((Object)(object)component) && (Object)(object)component != (Object)(object)basePlayer && !component.IsNpc)
		{
			if (component.IsSleeping())
			{
				component.EndSleeping();
			}
			else
			{
				component.StartSleeping();
			}
		}
	}

	[ServerUserVar]
	public static void kickmember(Arg arg)
	{
		BasePlayer basePlayer = arg.Player();
		if ((Object)(object)basePlayer == (Object)null)
		{
			return;
		}
		PlayerTeam playerTeam = ServerInstance.FindTeam(basePlayer.currentTeam);
		if (playerTeam != null && !((Object)(object)playerTeam.GetLeader() != (Object)(object)basePlayer))
		{
			ulong uLong = arg.GetULong(0, 0uL);
			if (basePlayer.userID != uLong)
			{
				playerTeam.RemovePlayer(uLong);
			}
		}
	}

	[ServerUserVar]
	public static void sendinvite(Arg arg)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = arg.Player();
		PlayerTeam playerTeam = ServerInstance.FindTeam(basePlayer.currentTeam);
		RaycastHit hit = default(RaycastHit);
		if (playerTeam == null || (Object)(object)playerTeam.GetLeader() == (Object)null || (Object)(object)playerTeam.GetLeader() != (Object)(object)basePlayer || !Physics.Raycast(basePlayer.eyes.position, basePlayer.eyes.HeadForward(), ref hit, 5f, 1218652417, (QueryTriggerInteraction)1))
		{
			return;
		}
		BaseEntity entity = hit.GetEntity();
		if (Object.op_Implicit((Object)(object)entity))
		{
			BasePlayer component = ((Component)entity).GetComponent<BasePlayer>();
			if (Object.op_Implicit((Object)(object)component) && (Object)(object)component != (Object)(object)basePlayer && !component.IsNpc && component.currentTeam == 0L)
			{
				playerTeam.SendInvite(component);
			}
		}
	}

	[ServerVar]
	public static void fakeinvite(Arg arg)
	{
		BasePlayer basePlayer = arg.Player();
		ulong uLong = arg.GetULong(0, 0uL);
		PlayerTeam playerTeam = ServerInstance.FindTeam(uLong);
		if (playerTeam != null)
		{
			if (basePlayer.currentTeam != 0L)
			{
				Debug.Log((object)"already in team");
			}
			playerTeam.SendInvite(basePlayer);
			Debug.Log((object)"sent bot invite");
		}
	}

	[ServerVar]
	public static void addtoteam(Arg arg)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = arg.Player();
		PlayerTeam playerTeam = ServerInstance.FindTeam(basePlayer.currentTeam);
		RaycastHit hit = default(RaycastHit);
		if (playerTeam == null || (Object)(object)playerTeam.GetLeader() == (Object)null || (Object)(object)playerTeam.GetLeader() != (Object)(object)basePlayer || !Physics.Raycast(basePlayer.eyes.position, basePlayer.eyes.HeadForward(), ref hit, 5f, 1218652417, (QueryTriggerInteraction)1))
		{
			return;
		}
		BaseEntity entity = hit.GetEntity();
		if (Object.op_Implicit((Object)(object)entity))
		{
			BasePlayer component = ((Component)entity).GetComponent<BasePlayer>();
			if (Object.op_Implicit((Object)(object)component) && (Object)(object)component != (Object)(object)basePlayer && !component.IsNpc)
			{
				playerTeam.AddPlayer(component);
			}
		}
	}

	public static bool TeamsEnabled()
	{
		return maxTeamSize > 0;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (!info.fromDisk || info.msg.relationshipManager == null)
		{
			return;
		}
		lastTeamIndex = info.msg.relationshipManager.lastTeamIndex;
		foreach (PlayerTeam team in info.msg.relationshipManager.teamList)
		{
			PlayerTeam playerTeam = Pool.Get<PlayerTeam>();
			playerTeam.teamLeader = team.teamLeader;
			playerTeam.teamID = team.teamID;
			playerTeam.teamName = team.teamName;
			playerTeam.members = new List<ulong>();
			foreach (TeamMember member in team.members)
			{
				playerTeam.members.Add(member.userID);
			}
			teams[playerTeam.teamID] = playerTeam;
		}
		foreach (PlayerTeam value in teams.Values)
		{
			foreach (ulong member2 in value.members)
			{
				playerToTeam[member2] = value;
				BasePlayer basePlayer = FindByID(member2);
				if ((Object)(object)basePlayer != (Object)null && basePlayer.currentTeam != value.teamID)
				{
					Debug.LogWarning((object)$"Player {member2} has the wrong teamID: got {basePlayer.currentTeam}, expected {value.teamID}. Fixing automatically.");
					basePlayer.currentTeam = value.teamID;
				}
			}
		}
		foreach (PlayerRelationships relationship in info.msg.relationshipManager.relationships)
		{
			ulong playerID = relationship.playerID;
			PlayerRelationships playerRelationships = GetRelationships(playerID);
			playerRelationships.relations.Clear();
			foreach (PlayerRelationshipInfo relation in relationship.relations)
			{
				PlayerRelationshipInfo playerRelationshipInfo = new PlayerRelationshipInfo();
				playerRelationshipInfo.type = (RelationshipType)relation.type;
				playerRelationshipInfo.weight = relation.weight;
				playerRelationshipInfo.displayName = relation.displayName;
				playerRelationshipInfo.mugshotCrc = relation.mugshotCrc;
				playerRelationshipInfo.notes = relation.notes;
				playerRelationshipInfo.player = relation.playerID;
				playerRelationshipInfo.lastSeenTime = Time.get_realtimeSinceStartup() - relation.timeSinceSeen;
				playerRelationships.relations.Add(relation.playerID, playerRelationshipInfo);
			}
		}
	}
}
