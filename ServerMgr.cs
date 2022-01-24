using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CompanionServer;
using ConVar;
using Facepunch;
using Facepunch.Math;
using Ionic.Crc;
using Network;
using Network.Visibility;
using ProtoBuf;
using Rust;
using Steamworks;
using Steamworks.Data;
using UnityEngine;

public class ServerMgr : SingletonComponent<ServerMgr>, IServerCallback
{
	public ConnectionQueue connectionQueue = new ConnectionQueue();

	public TimeAverageValueLookup<Type> packetHistory = new TimeAverageValueLookup<Type>();

	public TimeAverageValueLookup<uint> rpcHistory = new TimeAverageValueLookup<uint>();

	private Stopwatch queryTimer = Stopwatch.StartNew();

	private Dictionary<uint, int> unconnectedQueries = new Dictionary<uint, int>();

	private Stopwatch queriesPerSeconTimer = Stopwatch.StartNew();

	private int NumQueriesLastSecond;

	private MemoryStream queryBuffer = new MemoryStream();

	public const string BYPASS_PROCEDURAL_SPAWN_PREF = "bypassProceduralSpawn";

	private ConnectionAuth auth;

	private bool runFrameUpdate;

	private bool useQueryPort;

	public UserPersistance persistance;

	public PlayerStateManager playerStateManager;

	private AIThinkManager.QueueType aiTick;

	private List<ulong> bannedPlayerNotices = new List<ulong>();

	private string _AssemblyHash;

	private IEnumerator restartCoroutine;

	public static int AvailableSlots => ConVar.Server.maxplayers - BasePlayer.activePlayerList.get_Count();

	private string AssemblyHash
	{
		get
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected O, but got Unknown
			if (_AssemblyHash == null)
			{
				string location = typeof(ServerMgr).Assembly.Location;
				if (!string.IsNullOrEmpty(location))
				{
					byte[] array = File.ReadAllBytes(location);
					CRC32 val = new CRC32();
					val.SlurpBlock(array, 0, array.Length);
					_AssemblyHash = val.get_Crc32Result().ToString("x");
				}
				else
				{
					_AssemblyHash = "il2cpp";
				}
			}
			return _AssemblyHash;
		}
	}

	public bool Restarting => restartCoroutine != null;

	private void Log(Exception e)
	{
		if (Global.developer > 0)
		{
			Debug.LogException(e);
		}
	}

	public void OnNetworkMessage(Message packet)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Invalid comparison between Unknown and I4
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected I4, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected I4, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0569: Unknown result type (might be due to invalid IL or missing references)
		if (ConVar.Server.packetlog_enabled)
		{
			packetHistory.Increment(packet.type);
		}
		Type type = packet.type;
		if ((int)type != 4)
		{
			switch (type - 9)
			{
			default:
				switch (type - 18)
				{
				case 0:
				{
					if (packet.connection.GetPacketsPerSecond(packet.type) >= 1)
					{
						Net.sv.Kick(packet.connection, "Packet Flooding: User Information", packet.connection.connected);
						return;
					}
					TimeWarning val = TimeWarning.New("GiveUserInformation", 20);
					try
					{
						OnGiveUserInformation(packet);
					}
					catch (Exception e4)
					{
						Log(e4);
						Net.sv.Kick(packet.connection, "Invalid Packet: User Information", false);
					}
					finally
					{
						((IDisposable)val)?.Dispose();
					}
					packet.connection.AddPacketsPerSecond(packet.type);
					return;
				}
				case 4:
				{
					TimeWarning val = TimeWarning.New("OnEACMessage", 20);
					try
					{
						EACServer.OnMessageReceived(packet);
					}
					catch (Exception e6)
					{
						Log(e6);
						Net.sv.Kick(packet.connection, "Invalid Packet: EAC", false);
					}
					finally
					{
						((IDisposable)val)?.Dispose();
					}
					return;
				}
				case 6:
				{
					if (!World.Transfer || !packet.connection.connected)
					{
						return;
					}
					if (packet.connection.GetPacketsPerSecond(packet.type) >= (ulong)ConVar.Server.maxpacketspersecond_world)
					{
						Net.sv.Kick(packet.connection, "Packet Flooding: World", packet.connection.connected);
						return;
					}
					TimeWarning val = TimeWarning.New("OnWorldMessage", 20);
					try
					{
						WorldNetworking.OnMessageReceived(packet);
					}
					catch (Exception e5)
					{
						Log(e5);
						Net.sv.Kick(packet.connection, "Invalid Packet: World", false);
					}
					finally
					{
						((IDisposable)val)?.Dispose();
					}
					return;
				}
				case 3:
				{
					if (!packet.connection.connected)
					{
						return;
					}
					if (packet.connection.GetPacketsPerSecond(packet.type) >= (ulong)ConVar.Server.maxpacketspersecond_voice)
					{
						Net.sv.Kick(packet.connection, "Packet Flooding: Disconnect Reason", packet.connection.connected);
						return;
					}
					TimeWarning val = TimeWarning.New("OnPlayerVoice", 20);
					try
					{
						OnPlayerVoice(packet);
					}
					catch (Exception e3)
					{
						Log(e3);
						Net.sv.Kick(packet.connection, "Invalid Packet: Player Voice", false);
					}
					finally
					{
						((IDisposable)val)?.Dispose();
					}
					packet.connection.AddPacketsPerSecond(packet.type);
					return;
				}
				}
				break;
			case 0:
			{
				if (!packet.connection.connected)
				{
					return;
				}
				if (packet.connection.GetPacketsPerSecond(packet.type) >= (ulong)ConVar.Server.maxpacketspersecond_rpc)
				{
					Net.sv.Kick(packet.connection, "Paket Flooding: RPC Message", false);
					return;
				}
				TimeWarning val = TimeWarning.New("OnRPCMessage", 20);
				try
				{
					OnRPCMessage(packet);
				}
				catch (Exception e8)
				{
					Log(e8);
					Net.sv.Kick(packet.connection, "Invalid Packet: RPC Message", false);
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
				packet.connection.AddPacketsPerSecond(packet.type);
				return;
			}
			case 3:
			{
				if (!packet.connection.connected)
				{
					return;
				}
				if (packet.connection.GetPacketsPerSecond(packet.type) >= (ulong)ConVar.Server.maxpacketspersecond_command)
				{
					Net.sv.Kick(packet.connection, "Packet Flooding: Client Command", packet.connection.connected);
					return;
				}
				TimeWarning val = TimeWarning.New("OnClientCommand", 20);
				try
				{
					ConsoleNetwork.OnClientCommand(packet);
				}
				catch (Exception e7)
				{
					Log(e7);
					Net.sv.Kick(packet.connection, "Invalid Packet: Client Command", false);
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
				packet.connection.AddPacketsPerSecond(packet.type);
				return;
			}
			case 5:
			{
				if (!packet.connection.connected)
				{
					return;
				}
				if (packet.connection.GetPacketsPerSecond(packet.type) >= 1)
				{
					Net.sv.Kick(packet.connection, "Packet Flooding: Disconnect Reason", packet.connection.connected);
					return;
				}
				TimeWarning val = TimeWarning.New("ReadDisconnectReason", 20);
				try
				{
					ReadDisconnectReason(packet);
					Net.sv.Disconnect(packet.connection);
				}
				catch (Exception e2)
				{
					Log(e2);
					Net.sv.Kick(packet.connection, "Invalid Packet: Disconnect Reason", false);
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
				packet.connection.AddPacketsPerSecond(packet.type);
				return;
			}
			case 6:
			{
				if (!packet.connection.connected)
				{
					return;
				}
				if (packet.connection.GetPacketsPerSecond(packet.type) >= (ulong)ConVar.Server.maxpacketspersecond_tick)
				{
					Net.sv.Kick(packet.connection, "Packet Flooding: Player Tick", packet.connection.connected);
					return;
				}
				TimeWarning val = TimeWarning.New("OnPlayerTick", 20);
				try
				{
					OnPlayerTick(packet);
				}
				catch (Exception e)
				{
					Log(e);
					Net.sv.Kick(packet.connection, "Invalid Packet: Player Tick", false);
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
				packet.connection.AddPacketsPerSecond(packet.type);
				return;
			}
			case 1:
			case 2:
			case 4:
				break;
			}
			ProcessUnhandledPacket(packet);
		}
		else
		{
			if (!packet.connection.connected)
			{
				return;
			}
			if (packet.connection.GetPacketsPerSecond(packet.type) >= 1)
			{
				Net.sv.Kick(packet.connection, "Packet Flooding: Client Ready", packet.connection.connected);
				return;
			}
			TimeWarning val = TimeWarning.New("ClientReady", 20);
			try
			{
				ClientReady(packet);
			}
			catch (Exception e9)
			{
				Log(e9);
				Net.sv.Kick(packet.connection, "Invalid Packet: Client Ready", false);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
			packet.connection.AddPacketsPerSecond(packet.type);
		}
	}

	public void ProcessUnhandledPacket(Message packet)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (Global.developer > 0)
		{
			Debug.LogWarning((object)("[SERVER][UNHANDLED] " + packet.type));
		}
		Net.sv.Kick(packet.connection, "Sent Unhandled Message", false);
	}

	public void ReadDisconnectReason(Message packet)
	{
		string text = packet.get_read().String(2048);
		string text2 = ((object)packet.connection).ToString();
		if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
		{
			DebugEx.Log((object)(text2 + " disconnecting: " + text), (StackTraceLogType)0);
		}
	}

	private BasePlayer SpawnPlayerSleeping(Connection connection)
	{
		BasePlayer basePlayer = BasePlayer.FindSleeping(connection.userid);
		if ((Object)(object)basePlayer == (Object)null)
		{
			return null;
		}
		if (!basePlayer.IsSleeping())
		{
			Debug.LogWarning((object)"Player spawning into sleeper that isn't sleeping!");
			basePlayer.Kill();
			return null;
		}
		basePlayer.PlayerInit(connection);
		basePlayer.inventory.SendSnapshot();
		DebugEx.Log((object)(((object)basePlayer.net.get_connection()).ToString() + " joined [" + basePlayer.net.get_connection().os + "/" + basePlayer.net.get_connection().ownerid + "]"), (StackTraceLogType)0);
		return basePlayer;
	}

	public BasePlayer SpawnNewPlayer(Connection connection)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer.SpawnPoint spawnPoint = FindSpawnPoint();
		BasePlayer basePlayer = GameManager.server.CreateEntity("assets/prefabs/player/player.prefab", spawnPoint.pos, spawnPoint.rot).ToPlayer();
		basePlayer.health = 0f;
		basePlayer.lifestate = BaseCombatEntity.LifeState.Dead;
		basePlayer.ResetLifeStateOnSpawn = false;
		basePlayer.limitNetworking = true;
		basePlayer.SetFlag(BaseEntity.Flags.Protected, connection == null);
		basePlayer.Spawn();
		basePlayer.limitNetworking = false;
		if (connection != null)
		{
			basePlayer.PlayerInit(connection);
			if (Object.op_Implicit((Object)(object)BaseGameMode.GetActiveGameMode(serverside: true)))
			{
				BaseGameMode.GetActiveGameMode(serverside: true).OnNewPlayer(basePlayer);
			}
			else if (Application.get_isEditor() || (SleepingBag.FindForPlayer(basePlayer.userID, ignoreTimers: true).Length == 0 && !basePlayer.hasPreviousLife))
			{
				basePlayer.Respawn();
			}
			else
			{
				basePlayer.SendRespawnOptions();
			}
			DebugEx.Log((object)$"{basePlayer.displayName} with steamid {basePlayer.userID} joined from ip {basePlayer.net.get_connection().ipaddress}", (StackTraceLogType)0);
			DebugEx.Log((object)$"\tNetworkId {basePlayer.userID} is {basePlayer.net.ID} ({basePlayer.displayName})", (StackTraceLogType)0);
			if (basePlayer.net.get_connection().ownerid != basePlayer.net.get_connection().userid)
			{
				DebugEx.Log((object)$"\t{basePlayer} is sharing the account {basePlayer.net.get_connection().ownerid}", (StackTraceLogType)0);
			}
		}
		return basePlayer;
	}

	private void ClientReady(Message packet)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		if ((int)packet.connection.state != 3)
		{
			Net.sv.Kick(packet.connection, "Invalid connection state", false);
			return;
		}
		packet.connection.decryptIncoming = true;
		ClientReady val = ClientReady.Deserialize((Stream)(object)packet.get_read());
		try
		{
			foreach (ClientInfo item in val.clientInfo)
			{
				packet.connection.info.Set(item.name, item.value);
			}
			connectionQueue.JoinedGame(packet.connection);
			TimeWarning val2 = TimeWarning.New("ClientReady", 0);
			try
			{
				TimeWarning val3 = TimeWarning.New("SpawnPlayerSleeping", 0);
				BasePlayer basePlayer;
				try
				{
					basePlayer = SpawnPlayerSleeping(packet.connection);
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
				if ((Object)(object)basePlayer == (Object)null)
				{
					val3 = TimeWarning.New("SpawnNewPlayer", 0);
					try
					{
						basePlayer = SpawnNewPlayer(packet.connection);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				if ((Object)(object)basePlayer != (Object)null)
				{
					Util.SendSignedInNotification(basePlayer);
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		SendReplicatedVars(packet.connection);
	}

	private void OnRPCMessage(Message packet)
	{
		uint uid = packet.get_read().UInt32();
		uint num = packet.get_read().UInt32();
		if (ConVar.Server.rpclog_enabled)
		{
			rpcHistory.Increment(num);
		}
		BaseEntity baseEntity = BaseNetworkable.serverEntities.Find(uid) as BaseEntity;
		if (!((Object)(object)baseEntity == (Object)null))
		{
			baseEntity.SV_RPCMessage(num, packet);
		}
	}

	private void OnPlayerTick(Message packet)
	{
		BasePlayer basePlayer = packet.Player();
		if (!((Object)(object)basePlayer == (Object)null))
		{
			basePlayer.OnReceivedTick((Stream)(object)packet.get_read());
		}
	}

	private void OnPlayerVoice(Message packet)
	{
		BasePlayer basePlayer = packet.Player();
		if (!((Object)(object)basePlayer == (Object)null))
		{
			basePlayer.OnReceivedVoice(packet.get_read().BytesWithSize(10485760u));
		}
	}

	private void OnGiveUserInformation(Message packet)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if ((int)packet.connection.state != 0)
		{
			Net.sv.Kick(packet.connection, "Invalid connection state", false);
			return;
		}
		packet.connection.state = (State)1;
		if (packet.get_read().UInt8() != 228)
		{
			Net.sv.Kick(packet.connection, "Invalid Connection Protocol", false);
			return;
		}
		packet.connection.userid = packet.get_read().UInt64();
		packet.connection.protocol = packet.get_read().UInt32();
		packet.connection.os = packet.get_read().String(128);
		packet.connection.username = packet.get_read().String(256);
		if (string.IsNullOrEmpty(packet.connection.os))
		{
			throw new Exception("Invalid OS");
		}
		if (string.IsNullOrEmpty(packet.connection.username))
		{
			Net.sv.Kick(packet.connection, "Invalid Username", false);
			return;
		}
		packet.connection.username = packet.connection.username.Replace('\n', ' ').Replace('\r', ' ').Replace('\t', ' ')
			.Trim();
		if (string.IsNullOrEmpty(packet.connection.username))
		{
			Net.sv.Kick(packet.connection, "Invalid Username", false);
			return;
		}
		string text = string.Empty;
		string branch = ConVar.Server.branch;
		if (packet.get_read().get_Unread() >= 4)
		{
			text = packet.get_read().String(128);
		}
		if (branch != string.Empty && branch != text)
		{
			DebugEx.Log((object)string.Concat("Kicking ", packet.connection, " - their branch is '", text, "' not '", branch, "'"), (StackTraceLogType)0);
			Net.sv.Kick(packet.connection, "Wrong Steam Beta: Requires '" + branch + "' branch!", false);
		}
		else if (packet.connection.protocol > 2326)
		{
			DebugEx.Log((object)string.Concat("Kicking ", packet.connection, " - their protocol is ", packet.connection.protocol, " not ", 2326), (StackTraceLogType)0);
			Net.sv.Kick(packet.connection, "Wrong Connection Protocol: Server update required!", false);
		}
		else if (packet.connection.protocol < 2326)
		{
			DebugEx.Log((object)string.Concat("Kicking ", packet.connection, " - their protocol is ", packet.connection.protocol, " not ", 2326), (StackTraceLogType)0);
			Net.sv.Kick(packet.connection, "Wrong Connection Protocol: Client update required!", false);
		}
		else
		{
			packet.connection.token = packet.get_read().BytesWithSize(512u);
			if (packet.connection.token == null || packet.connection.token.Length < 1)
			{
				Net.sv.Kick(packet.connection, "Invalid Token", false);
			}
			else
			{
				auth.OnNewConnection(packet.connection);
			}
		}
	}

	public bool OnUnconnectedMessage(int type, NetRead read, uint ip, int port)
	{
		if (useQueryPort)
		{
			return false;
		}
		if (type == 255)
		{
			if (queriesPerSeconTimer.get_Elapsed().TotalSeconds > 1.0)
			{
				queriesPerSeconTimer.Reset();
				queriesPerSeconTimer.Start();
				NumQueriesLastSecond = 0;
			}
			if (NumQueriesLastSecond > ConVar.Server.queriesPerSecond)
			{
				return false;
			}
			if (read.UInt8() != byte.MaxValue)
			{
				return false;
			}
			if (read.UInt8() != byte.MaxValue)
			{
				return false;
			}
			if (read.UInt8() != byte.MaxValue)
			{
				return false;
			}
			if (queryTimer.get_Elapsed().TotalSeconds > 60.0)
			{
				queryTimer.Reset();
				queryTimer.Start();
				unconnectedQueries.Clear();
			}
			if (!unconnectedQueries.ContainsKey(ip))
			{
				unconnectedQueries.Add(ip, 0);
			}
			int num = unconnectedQueries[ip] + 1;
			unconnectedQueries[ip] = num;
			if (num > ConVar.Server.ipQueriesPerMin)
			{
				return true;
			}
			NumQueriesLastSecond++;
			((Stream)(object)read).Position = 0L;
			int unread = read.get_Unread();
			if (unread > 4096)
			{
				return true;
			}
			if (queryBuffer.Capacity < unread)
			{
				queryBuffer.Capacity = unread;
			}
			int num2 = ((Stream)(object)read).Read(queryBuffer.GetBuffer(), 0, unread);
			SteamServer.HandleIncomingPacket(queryBuffer.GetBuffer(), num2, ip, (ushort)port);
			return true;
		}
		return false;
	}

	public bool Initialize(bool loadSave = true, string saveFile = "", bool allowOutOfDateSaves = false, bool skipInitialSpawn = false)
	{
		persistance = new UserPersistance(ConVar.Server.rootFolder);
		playerStateManager = new PlayerStateManager(persistance);
		SpawnMapEntities();
		if (Object.op_Implicit((Object)(object)SingletonComponent<SpawnHandler>.Instance))
		{
			TimeWarning val = TimeWarning.New("SpawnHandler.UpdateDistributions", 0);
			try
			{
				SingletonComponent<SpawnHandler>.Instance.UpdateDistributions();
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		if (loadSave)
		{
			World.LoadedFromSave = true;
			World.LoadedFromSave = (skipInitialSpawn = SaveRestore.Load(saveFile, allowOutOfDateSaves));
		}
		else
		{
			SaveRestore.SaveCreatedTime = DateTime.UtcNow;
			World.LoadedFromSave = false;
		}
		if (Object.op_Implicit((Object)(object)SingletonComponent<SpawnHandler>.Instance))
		{
			TimeWarning val;
			if (!skipInitialSpawn)
			{
				val = TimeWarning.New("SpawnHandler.InitialSpawn", 200);
				try
				{
					SingletonComponent<SpawnHandler>.Instance.InitialSpawn();
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
			}
			val = TimeWarning.New("SpawnHandler.StartSpawnTick", 200);
			try
			{
				SingletonComponent<SpawnHandler>.Instance.StartSpawnTick();
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		CreateImportantEntities();
		auth = ((Component)this).GetComponent<ConnectionAuth>();
		return World.LoadedFromSave;
	}

	public void OpenConnection()
	{
		useQueryPort = ConVar.Server.queryport > 0 && ConVar.Server.queryport != ConVar.Server.port;
		if (!useQueryPort && !Net.sv.get_AllowPassthroughMessages())
		{
			ConVar.Server.queryport = Math.Max(ConVar.Server.port, RCon.Port) + 1;
			useQueryPort = true;
		}
		Net.sv.ip = ConVar.Server.ip;
		Net.sv.port = ConVar.Server.port;
		StartSteamServer();
		if (!Net.sv.Start())
		{
			Debug.LogWarning((object)"Couldn't Start Server.");
			CloseConnection();
			return;
		}
		Net.sv.callbackHandler = (IServerCallback)(object)this;
		((BaseNetwork)Net.sv).cryptography = (INetworkCryptocraphy)(object)new NetworkCryptographyServer();
		EACServer.DoStartup();
		((MonoBehaviour)this).InvokeRepeating("EACUpdate", 1f, 1f);
		((MonoBehaviour)this).InvokeRepeating("DoTick", 1f, 1f / (float)ConVar.Server.tickrate);
		((MonoBehaviour)this).InvokeRepeating("DoHeartbeat", 1f, 1f);
		runFrameUpdate = true;
		ConsoleSystem.add_OnReplicatedVarChanged((Action<string, string>)OnReplicatedVarChanged);
	}

	private void CloseConnection()
	{
		if (persistance != null)
		{
			persistance.Dispose();
			persistance = null;
		}
		EACServer.DoShutdown();
		Net.sv.callbackHandler = null;
		TimeWarning val = TimeWarning.New("sv.Stop", 0);
		try
		{
			Net.sv.Stop("Shutting Down");
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		val = TimeWarning.New("RCon.Shutdown", 0);
		try
		{
			RCon.Shutdown();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		val = TimeWarning.New("PlatformService.Shutdown", 0);
		try
		{
			IPlatformService instance = PlatformService.Instance;
			if (instance != null)
			{
				instance.Shutdown();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		val = TimeWarning.New("CompanionServer.Shutdown", 0);
		try
		{
			CompanionServer.Server.Shutdown();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		val = TimeWarning.New("NexusServer.Shutdown", 0);
		try
		{
			NexusServer.Shutdown();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		ConsoleSystem.remove_OnReplicatedVarChanged((Action<string, string>)OnReplicatedVarChanged);
	}

	private void OnDisable()
	{
		if (!Application.isQuitting)
		{
			CloseConnection();
		}
	}

	private void OnApplicationQuit()
	{
		Application.isQuitting = true;
		CloseConnection();
	}

	private void CreateImportantEntities()
	{
		CreateImportantEntity<EnvSync>("assets/bundled/prefabs/system/net_env.prefab");
		CreateImportantEntity<CommunityEntity>("assets/bundled/prefabs/system/server/community.prefab");
		CreateImportantEntity<ResourceDepositManager>("assets/bundled/prefabs/system/server/resourcedepositmanager.prefab");
		CreateImportantEntity<RelationshipManager>("assets/bundled/prefabs/system/server/relationship_manager.prefab");
		CreateImportantEntity<TreeManager>("assets/bundled/prefabs/system/tree_manager.prefab");
	}

	public void CreateImportantEntity<T>(string prefabName) where T : BaseEntity
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (!Enumerable.Any<BaseNetworkable>((IEnumerable<BaseNetworkable>)BaseNetworkable.serverEntities, (Func<BaseNetworkable, bool>)((BaseNetworkable x) => x is T)))
		{
			Debug.LogWarning((object)("Missing " + typeof(T).Name + " - creating"));
			BaseEntity baseEntity = GameManager.server.CreateEntity(prefabName);
			if ((Object)(object)baseEntity == (Object)null)
			{
				Debug.LogWarning((object)"Couldn't create");
			}
			else
			{
				baseEntity.Spawn();
			}
		}
	}

	private void StartSteamServer()
	{
		PlatformService.Instance.Initialize((IPlatformHooks)(object)RustPlatformHooks.Instance);
		((MonoBehaviour)this).InvokeRepeating("UpdateServerInformation", 2f, 30f);
		((MonoBehaviour)this).InvokeRepeating("UpdateItemDefinitions", 10f, 3600f);
		DebugEx.Log((object)"SteamServer Initialized", (StackTraceLogType)0);
	}

	private void UpdateItemDefinitions()
	{
		Debug.Log((object)"Checking for new Steam Item Definitions..");
		PlatformService.Instance.RefreshItemDefinitions();
	}

	internal void OnValidateAuthTicketResponse(ulong SteamId, ulong OwnerId, AuthResponse Status)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Invalid comparison between Unknown and I4
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Invalid comparison between Unknown and I4
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Invalid comparison between Unknown and I4
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Invalid comparison between Unknown and I4
		if (Auth_Steam.ValidateConnecting(SteamId, OwnerId, Status))
		{
			return;
		}
		Connection val = Enumerable.FirstOrDefault<Connection>((IEnumerable<Connection>)Net.sv.connections, (Func<Connection, bool>)((Connection x) => x.userid == SteamId));
		if (val == null)
		{
			Debug.LogWarning((object)$"Steam gave us a {Status} ticket response for unconnected id {SteamId}");
		}
		else if ((int)Status == 2)
		{
			Debug.LogWarning((object)$"Steam gave us a 'ok' ticket response for already connected id {SteamId}");
		}
		else if ((int)Status != 1)
		{
			if (((int)Status == 4 || (int)Status == 3) && !bannedPlayerNotices.Contains(SteamId))
			{
				ConsoleNetwork.BroadcastToAllClients("chat.add", 2, 0, "<color=#fff>SERVER</color> Kicking " + StringEx.EscapeRichText(val.username) + " (banned by anticheat)");
				bannedPlayerNotices.Add(SteamId);
			}
			Debug.Log((object)$"Kicking {val.ipaddress}/{val.userid}/{val.username} (Steam Status \"{((object)(AuthResponse)(ref Status)).ToString()}\")");
			val.authStatus = ((object)(AuthResponse)(ref Status)).ToString();
			Net.sv.Kick(val, "Steam: " + ((object)(AuthResponse)(ref Status)).ToString(), false);
		}
	}

	private void EACUpdate()
	{
		EACServer.DoUpdate();
	}

	private void Update()
	{
		if (!runFrameUpdate)
		{
			return;
		}
		TimeWarning val = TimeWarning.New("ServerMgr.Update", 500);
		try
		{
			if (EACServer.playerTracker != null)
			{
				EACServer.playerTracker.BeginFrame();
			}
			try
			{
				TimeWarning val2 = TimeWarning.New("PlatformService.Update", 100);
				try
				{
					PlatformService.Instance.Update();
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning((object)"Server Exception: Platform Service Update");
				Debug.LogException(ex, (Object)(object)this);
			}
			try
			{
				TimeWarning val2 = TimeWarning.New("Net.sv.Cycle", 100);
				try
				{
					Net.sv.Cycle();
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			catch (Exception ex2)
			{
				Debug.LogWarning((object)"Server Exception: Network Cycle");
				Debug.LogException(ex2, (Object)(object)this);
			}
			try
			{
				TimeWarning val2 = TimeWarning.New("ServerBuildingManager.Cycle", 0);
				try
				{
					BuildingManager.server.Cycle();
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			catch (Exception ex3)
			{
				Debug.LogWarning((object)"Server Exception: Building Manager");
				Debug.LogException(ex3, (Object)(object)this);
			}
			try
			{
				TimeWarning val2 = TimeWarning.New("BasePlayer.ServerCycle", 0);
				try
				{
					bool batchsynctransforms = Physics.batchsynctransforms;
					bool autosynctransforms = Physics.autosynctransforms;
					if (batchsynctransforms && autosynctransforms)
					{
						Physics.set_autoSyncTransforms(false);
					}
					if (!Physics.get_autoSyncTransforms())
					{
						Physics.SyncTransforms();
					}
					BasePlayer.ServerCycle(Time.get_deltaTime());
					try
					{
						TimeWarning val3 = TimeWarning.New("FlameTurret.BudgetedUpdate", 0);
						try
						{
							((ObjectWorkQueue<FlameTurret>)FlameTurret.updateFlameTurretQueueServer).RunQueue(0.25);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex4)
					{
						Debug.LogWarning((object)"Server Exception: FlameTurret.BudgetedUpdate");
						Debug.LogException(ex4, (Object)(object)this);
					}
					try
					{
						TimeWarning val3 = TimeWarning.New("AutoTurret.BudgetedUpdate", 0);
						try
						{
							((ObjectWorkQueue<AutoTurret>)AutoTurret.updateAutoTurretScanQueue).RunQueue(0.5);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex5)
					{
						Debug.LogWarning((object)"Server Exception: AutoTurret.BudgetedUpdate");
						Debug.LogException(ex5, (Object)(object)this);
					}
					try
					{
						TimeWarning val3 = TimeWarning.New("BaseFishingRod.BudgetedUpdate", 0);
						try
						{
							((ObjectWorkQueue<BaseFishingRod>)BaseFishingRod.updateFishingRodQueue).RunQueue(1.0);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex6)
					{
						Debug.LogWarning((object)"Server Exception: BaseFishingRod.BudgetedUpdate");
						Debug.LogException(ex6, (Object)(object)this);
					}
					if (batchsynctransforms && autosynctransforms)
					{
						Physics.set_autoSyncTransforms(true);
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			catch (Exception ex7)
			{
				Debug.LogWarning((object)"Server Exception: Player Update");
				Debug.LogException(ex7, (Object)(object)this);
			}
			try
			{
				TimeWarning val2 = TimeWarning.New("SteamQueryResponse", 0);
				try
				{
					SteamQueryResponse();
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			catch (Exception ex8)
			{
				Debug.LogWarning((object)"Server Exception: Steam Query");
				Debug.LogException(ex8, (Object)(object)this);
			}
			try
			{
				TimeWarning val2 = TimeWarning.New("connectionQueue.Cycle", 0);
				try
				{
					connectionQueue.Cycle(AvailableSlots);
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			catch (Exception ex9)
			{
				Debug.LogWarning((object)"Server Exception: Connection Queue");
				Debug.LogException(ex9, (Object)(object)this);
			}
			try
			{
				TimeWarning val2 = TimeWarning.New("IOEntity.ProcessQueue", 0);
				try
				{
					IOEntity.ProcessQueue();
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			catch (Exception ex10)
			{
				Debug.LogWarning((object)"Server Exception: IOEntity.ProcessQueue");
				Debug.LogException(ex10, (Object)(object)this);
			}
			if (!AI.spliceupdates)
			{
				aiTick = AIThinkManager.QueueType.Human;
			}
			else
			{
				aiTick = ((aiTick == AIThinkManager.QueueType.Human) ? AIThinkManager.QueueType.Animal : AIThinkManager.QueueType.Human);
			}
			if (aiTick == AIThinkManager.QueueType.Human)
			{
				try
				{
					TimeWarning val2 = TimeWarning.New("AIThinkManager.ProcessQueue", 0);
					try
					{
						AIThinkManager.ProcessQueue(AIThinkManager.QueueType.Human);
					}
					finally
					{
						((IDisposable)val2)?.Dispose();
					}
				}
				catch (Exception ex11)
				{
					Debug.LogWarning((object)"Server Exception: AIThinkManager.ProcessQueue");
					Debug.LogException(ex11, (Object)(object)this);
				}
				if (!AI.spliceupdates)
				{
					aiTick = AIThinkManager.QueueType.Animal;
				}
			}
			if (aiTick == AIThinkManager.QueueType.Animal)
			{
				try
				{
					TimeWarning val2 = TimeWarning.New("AIThinkManager.ProcessAnimalQueue", 0);
					try
					{
						AIThinkManager.ProcessQueue(AIThinkManager.QueueType.Animal);
					}
					finally
					{
						((IDisposable)val2)?.Dispose();
					}
				}
				catch (Exception ex12)
				{
					Debug.LogWarning((object)"Server Exception: AIThinkManager.ProcessAnimalQueue");
					Debug.LogException(ex12, (Object)(object)this);
				}
			}
			try
			{
				TimeWarning val2 = TimeWarning.New("AIThinkManager.ProcessPetQueue", 0);
				try
				{
					AIThinkManager.ProcessQueue(AIThinkManager.QueueType.Pets);
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			catch (Exception ex13)
			{
				Debug.LogWarning((object)"Server Exception: AIThinkManager.ProcessPetQueue");
				Debug.LogException(ex13, (Object)(object)this);
			}
			try
			{
				TimeWarning val2 = TimeWarning.New("AIThinkManager.ProcessPetMovementQueue", 0);
				try
				{
					BasePet.ProcessMovementQueue();
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			catch (Exception ex14)
			{
				Debug.LogWarning((object)"Server Exception: AIThinkManager.ProcessPetMovementQueue");
				Debug.LogException(ex14, (Object)(object)this);
			}
			try
			{
				TimeWarning val2 = TimeWarning.New("BaseRidableAnimal.ProcessQueue", 0);
				try
				{
					BaseRidableAnimal.ProcessQueue();
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			catch (Exception ex15)
			{
				Debug.LogWarning((object)"Server Exception: BaseRidableAnimal.ProcessQueue");
				Debug.LogException(ex15, (Object)(object)this);
			}
			try
			{
				TimeWarning val2 = TimeWarning.New("GrowableEntity.BudgetedUpdate", 0);
				try
				{
					((ObjectWorkQueue<GrowableEntity>)GrowableEntity.growableEntityUpdateQueue).RunQueue((double)GrowableEntity.framebudgetms);
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			catch (Exception ex16)
			{
				Debug.LogWarning((object)"Server Exception: GrowableEntity.BudgetedUpdate");
				Debug.LogException(ex16, (Object)(object)this);
			}
			try
			{
				TimeWarning val2 = TimeWarning.New("BasePlayer.BudgetedLifeStoryUpdate", 0);
				try
				{
					((ObjectWorkQueue<BasePlayer>)BasePlayer.lifeStoryQueue).RunQueue((double)BasePlayer.lifeStoryFramebudgetms);
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			catch (Exception ex17)
			{
				Debug.LogWarning((object)"Server Exception: BasePlayer.BudgetedLifeStoryUpdate");
				Debug.LogException(ex17, (Object)(object)this);
			}
			try
			{
				TimeWarning val2 = TimeWarning.New("JunkPileWater.UpdateNearbyPlayers", 0);
				try
				{
					((ObjectWorkQueue<JunkPileWater>)JunkPileWater.junkpileWaterWorkQueue).RunQueue((double)JunkPileWater.framebudgetms);
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			catch (Exception ex18)
			{
				Debug.LogWarning((object)"Server Exception: JunkPileWater.UpdateNearbyPlayers");
				Debug.LogException(ex18, (Object)(object)this);
			}
			if (EACServer.playerTracker != null)
			{
				EACServer.playerTracker.EndFrame();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void FixedUpdate()
	{
		TimeWarning val = TimeWarning.New("ServerMgr.FixedUpdate", 0);
		try
		{
			try
			{
				TimeWarning val2 = TimeWarning.New("BaseMountable.FixedUpdateCycle", 0);
				try
				{
					BaseMountable.FixedUpdateCycle();
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning((object)"Server Exception: Mountable Cycle");
				Debug.LogException(ex, (Object)(object)this);
			}
			try
			{
				TimeWarning val2 = TimeWarning.New("Buoyancy.Cycle", 0);
				try
				{
					Buoyancy.Cycle();
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			catch (Exception ex2)
			{
				Debug.LogWarning((object)"Server Exception: Buoyancy Cycle");
				Debug.LogException(ex2, (Object)(object)this);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void SteamQueryResponse()
	{
		if (!SteamServer.get_IsValid() || !Net.sv.get_AllowPassthroughMessages())
		{
			return;
		}
		TimeWarning val = TimeWarning.New("SteamGameServer.GetNextOutgoingPacket", 0);
		try
		{
			OutgoingPacket val2 = default(OutgoingPacket);
			while (SteamServer.GetOutgoingPacket(ref val2))
			{
				Net.sv.SendUnconnected(((OutgoingPacket)(ref val2)).get_Address(), ((OutgoingPacket)(ref val2)).get_Port(), ((OutgoingPacket)(ref val2)).get_Data(), ((OutgoingPacket)(ref val2)).get_Size());
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void DoTick()
	{
		RCon.Update();
		CompanionServer.Server.Update();
		NexusServer.Update();
		for (int i = 0; i < Net.sv.connections.Count; i++)
		{
			Connection val = Net.sv.connections[i];
			if (!val.get_isAuthenticated() && !(val.GetSecondsConnected() < (float)ConVar.Server.authtimeout))
			{
				Net.sv.Kick(val, "Authentication Timed Out", false);
			}
		}
	}

	private void DoHeartbeat()
	{
		ItemManager.Heartbeat();
	}

	public static string GamemodeName()
	{
		return "rust";
	}

	public static string GamemodeTitle()
	{
		return "Rust: Survival Mode";
	}

	public static string GamemodeDesc()
	{
		return "The default Rust survival gamemode";
	}

	public static string GamemodeImage()
	{
		return "https://files.facepunch.com/garry/3c96c182-ab06-40ff-b66e-f4a510053ca4.png";
	}

	public static string GamemodeUrl()
	{
		return "https://rust.facepunch.com";
	}

	private void UpdateServerInformation()
	{
		if (!SteamServer.get_IsValid())
		{
			return;
		}
		TimeWarning val = TimeWarning.New("UpdateServerInformation", 0);
		try
		{
			SteamServer.set_ServerName(ConVar.Server.hostname);
			SteamServer.set_MaxPlayers(ConVar.Server.maxplayers);
			SteamServer.set_Passworded(false);
			SteamServer.set_MapName(World.Name);
			string text = "stok";
			if (Restarting)
			{
				text = "strst";
			}
			string text2 = $"born{Epoch.FromDateTime(SaveRestore.SaveCreatedTime)}";
			string text3 = $"gm{GamemodeName()}";
			string text4 = (ConVar.Server.pve ? ",pve" : string.Empty);
			string text5 = ConVar.Server.tags?.Trim(new char[1] { ',' }) ?? "";
			string text6 = ((!string.IsNullOrWhiteSpace(text5)) ? ("," + text5) : "");
			SteamServer.set_GameTags($"mp{ConVar.Server.maxplayers},cp{BasePlayer.activePlayerList.get_Count()},pt{Net.sv.get_ProtocolId()},qp{SingletonComponent<ServerMgr>.Instance.connectionQueue.Queued},v{2326}{text4}{text6},h{AssemblyHash},{text},{text2},{text3}");
			if (ConVar.Server.description != null && ConVar.Server.description.Length > 100)
			{
				string[] array = Enumerable.ToArray<string>(StringEx.SplitToChunks(ConVar.Server.description, 100));
				for (int i = 0; i < 16; i++)
				{
					if (i < array.Length)
					{
						SteamServer.SetKey($"description_{i:00}", array[i]);
					}
					else
					{
						SteamServer.SetKey($"description_{i:00}", string.Empty);
					}
				}
			}
			else
			{
				SteamServer.SetKey("description_0", ConVar.Server.description);
				for (int j = 1; j < 16; j++)
				{
					SteamServer.SetKey($"description_{j:00}", string.Empty);
				}
			}
			SteamServer.SetKey("hash", AssemblyHash);
			SteamServer.SetKey("world.seed", World.Seed.ToString());
			SteamServer.SetKey("world.size", World.Size.ToString());
			SteamServer.SetKey("pve", ConVar.Server.pve.ToString());
			SteamServer.SetKey("headerimage", ConVar.Server.headerimage);
			SteamServer.SetKey("logoimage", ConVar.Server.logoimage);
			SteamServer.SetKey("url", ConVar.Server.url);
			SteamServer.SetKey("gmn", GamemodeName());
			SteamServer.SetKey("gmt", GamemodeTitle());
			SteamServer.SetKey("gmd", GamemodeDesc());
			SteamServer.SetKey("gmu", GamemodeUrl());
			SteamServer.SetKey("uptime", ((int)Time.get_realtimeSinceStartup()).ToString());
			SteamServer.SetKey("gc_mb", Performance.report.memoryAllocations.ToString());
			SteamServer.SetKey("gc_cl", Performance.report.memoryCollections.ToString());
			SteamServer.SetKey("fps", Performance.report.frameRate.ToString());
			SteamServer.SetKey("fps_avg", Performance.report.frameRateAverage.ToString("0.00"));
			SteamServer.SetKey("ent_cnt", BaseNetworkable.serverEntities.Count.ToString());
			SteamServer.SetKey("build", BuildInfo.get_Current().get_Scm().get_ChangeId());
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void OnDisconnected(string strReason, Connection connection)
	{
		connectionQueue.RemoveConnection(connection);
		ConnectionAuth.OnDisconnect(connection);
		PlatformService.Instance.EndPlayerSession(connection.userid);
		EACServer.OnLeaveGame(connection);
		BasePlayer basePlayer = connection.player as BasePlayer;
		if ((Object)(object)basePlayer != (Object)null)
		{
			basePlayer.OnDisconnected();
		}
		NexusServer.Logout(connection.userid);
	}

	public static void OnEnterVisibility(Connection connection, Group group)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (Net.sv.IsConnected() && ((BaseNetwork)Net.sv).get_write().Start())
		{
			((BaseNetwork)Net.sv).get_write().PacketID((Type)19);
			((BaseNetwork)Net.sv).get_write().GroupID(group.ID);
			((BaseNetwork)Net.sv).get_write().Send(new SendInfo(connection));
		}
	}

	public static void OnLeaveVisibility(Connection connection, Group group)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		if (Net.sv.IsConnected())
		{
			if (((BaseNetwork)Net.sv).get_write().Start())
			{
				((BaseNetwork)Net.sv).get_write().PacketID((Type)20);
				((BaseNetwork)Net.sv).get_write().GroupID(group.ID);
				((BaseNetwork)Net.sv).get_write().Send(new SendInfo(connection));
			}
			if (((BaseNetwork)Net.sv).get_write().Start())
			{
				((BaseNetwork)Net.sv).get_write().PacketID((Type)8);
				((BaseNetwork)Net.sv).get_write().GroupID(group.ID);
				((BaseNetwork)Net.sv).get_write().Send(new SendInfo(connection));
			}
		}
	}

	internal void SpawnMapEntities()
	{
		new PrefabPreProcess(clientside: false, serverside: true);
		BaseEntity[] array = Object.FindObjectsOfType<BaseEntity>();
		BaseEntity[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].SpawnAsMapEntity();
		}
		DebugEx.Log((object)$"Map Spawned {array.Length} entities", (StackTraceLogType)0);
		array2 = array;
		foreach (BaseEntity baseEntity in array2)
		{
			if ((Object)(object)baseEntity != (Object)null)
			{
				baseEntity.PostMapEntitySpawn();
			}
		}
	}

	public static BasePlayer.SpawnPoint FindSpawnPoint(BasePlayer forPlayer = null)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(serverside: true);
		if (Object.op_Implicit((Object)(object)activeGameMode) && activeGameMode.useCustomSpawns)
		{
			BasePlayer.SpawnPoint playerSpawn = activeGameMode.GetPlayerSpawn(forPlayer);
			if (playerSpawn != null)
			{
				return playerSpawn;
			}
		}
		if ((Object)(object)SingletonComponent<SpawnHandler>.Instance != (Object)null && !flag)
		{
			BasePlayer.SpawnPoint spawnPoint = SpawnHandler.GetSpawnPoint();
			if (spawnPoint != null)
			{
				return spawnPoint;
			}
		}
		BasePlayer.SpawnPoint spawnPoint2 = new BasePlayer.SpawnPoint();
		GameObject[] array = GameObject.FindGameObjectsWithTag("spawnpoint");
		if (array.Length != 0)
		{
			GameObject val = array[Random.Range(0, array.Length)];
			spawnPoint2.pos = val.get_transform().get_position();
			spawnPoint2.rot = val.get_transform().get_rotation();
		}
		else
		{
			Debug.Log((object)"Couldn't find an appropriate spawnpoint for the player - so spawning at camera");
			if ((Object)(object)MainCamera.mainCamera != (Object)null)
			{
				spawnPoint2.pos = MainCamera.position;
				spawnPoint2.rot = MainCamera.rotation;
			}
		}
		RaycastHit val2 = default(RaycastHit);
		if (Physics.Raycast(new Ray(spawnPoint2.pos, Vector3.get_down()), ref val2, 32f, 1537286401))
		{
			spawnPoint2.pos = ((RaycastHit)(ref val2)).get_point();
		}
		return spawnPoint2;
	}

	public void JoinGame(Connection connection)
	{
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		Approval val = Pool.Get<Approval>();
		try
		{
			uint num = (uint)ConVar.Server.encryption;
			if (num > 1 && connection.os == "editor" && DeveloperList.Contains(connection.ownerid))
			{
				num = 1u;
			}
			val.level = Application.get_loadedLevelName();
			val.levelTransfer = World.Transfer;
			val.levelUrl = World.Url;
			val.levelSeed = World.Seed;
			val.levelSize = World.Size;
			val.checksum = World.Checksum;
			val.hostname = ConVar.Server.hostname;
			val.official = ConVar.Server.official;
			val.encryption = num;
			if (((BaseNetwork)Net.sv).get_write().Start())
			{
				((BaseNetwork)Net.sv).get_write().PacketID((Type)3);
				val.WriteToStream((Stream)(object)((BaseNetwork)Net.sv).get_write());
				((BaseNetwork)Net.sv).get_write().Send(new SendInfo(connection));
			}
			connection.encryptionLevel = num;
			connection.encryptOutgoing = true;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		connection.connected = true;
	}

	internal void Shutdown()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer[] array = Enumerable.ToArray<BasePlayer>((IEnumerable<BasePlayer>)BasePlayer.activePlayerList);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Kick("Server Shutting Down");
		}
		ConsoleSystem.Run(Option.get_Server(), "server.save", Array.Empty<object>());
		ConsoleSystem.Run(Option.get_Server(), "server.writecfg", Array.Empty<object>());
	}

	private IEnumerator ServerRestartWarning(string info, int iSeconds)
	{
		if (iSeconds < 0)
		{
			yield break;
		}
		if (!string.IsNullOrEmpty(info))
		{
			ConsoleNetwork.BroadcastToAllClients("chat.add", 2, 0, "<color=#fff>SERVER</color> Restarting: " + info);
		}
		for (int i = iSeconds; i > 0; i--)
		{
			if (i == iSeconds || i % 60 == 0 || (i < 300 && i % 30 == 0) || (i < 60 && i % 10 == 0) || i < 10)
			{
				ConsoleNetwork.BroadcastToAllClients("chat.add", 2, 0, $"<color=#fff>SERVER</color> Restarting in {i} seconds ({info})!");
				Debug.Log((object)$"Restarting in {i} seconds");
			}
			yield return CoroutineEx.waitForSeconds(1f);
		}
		ConsoleNetwork.BroadcastToAllClients("chat.add", 2, 0, "<color=#fff>SERVER</color> Restarting (" + info + ")");
		yield return CoroutineEx.waitForSeconds(2f);
		BasePlayer[] array = Enumerable.ToArray<BasePlayer>((IEnumerable<BasePlayer>)BasePlayer.activePlayerList);
		for (int j = 0; j < array.Length; j++)
		{
			array[j].Kick("Server Restarting");
		}
		yield return CoroutineEx.waitForSeconds(1f);
		ConsoleSystem.Run(Option.get_Server(), "quit", Array.Empty<object>());
	}

	public static void RestartServer(string strNotice, int iSeconds)
	{
		if (!((Object)(object)SingletonComponent<ServerMgr>.Instance == (Object)null))
		{
			if (SingletonComponent<ServerMgr>.Instance.restartCoroutine != null)
			{
				ConsoleNetwork.BroadcastToAllClients("chat.add", 2, 0, "<color=#fff>SERVER</color> Restart interrupted!");
				((MonoBehaviour)SingletonComponent<ServerMgr>.Instance).StopCoroutine(SingletonComponent<ServerMgr>.Instance.restartCoroutine);
				SingletonComponent<ServerMgr>.Instance.restartCoroutine = null;
			}
			SingletonComponent<ServerMgr>.Instance.restartCoroutine = SingletonComponent<ServerMgr>.Instance.ServerRestartWarning(strNotice, iSeconds);
			((MonoBehaviour)SingletonComponent<ServerMgr>.Instance).StartCoroutine(SingletonComponent<ServerMgr>.Instance.restartCoroutine);
			SingletonComponent<ServerMgr>.Instance.UpdateServerInformation();
		}
	}

	public static void SendReplicatedVars(string filter)
	{
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		if (!((BaseNetwork)Net.sv).get_write().Start())
		{
			return;
		}
		List<Connection> list = Pool.GetList<Connection>();
		foreach (Connection connection in Net.sv.connections)
		{
			if (connection.connected)
			{
				list.Add(connection);
			}
		}
		List<Command> list2 = Pool.GetList<Command>();
		foreach (Command item in Server.Replicated)
		{
			if (item.FullName.StartsWith(filter))
			{
				list2.Add(item);
			}
		}
		((BaseNetwork)Net.sv).get_write().PacketID((Type)25);
		((BaseNetwork)Net.sv).get_write().Int32(list2.Count);
		foreach (Command item2 in list2)
		{
			((BaseNetwork)Net.sv).get_write().String(item2.FullName);
			((BaseNetwork)Net.sv).get_write().String(item2.get_String());
		}
		((BaseNetwork)Net.sv).get_write().Send(new SendInfo(list));
		Pool.FreeList<Command>(ref list2);
		Pool.FreeList<Connection>(ref list);
	}

	public static void SendReplicatedVars(Connection connection)
	{
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		if (!((BaseNetwork)Net.sv).get_write().Start())
		{
			return;
		}
		List<Command> replicated = Server.Replicated;
		((BaseNetwork)Net.sv).get_write().PacketID((Type)25);
		((BaseNetwork)Net.sv).get_write().Int32(replicated.Count);
		foreach (Command item in replicated)
		{
			((BaseNetwork)Net.sv).get_write().String(item.FullName);
			((BaseNetwork)Net.sv).get_write().String(item.get_String());
		}
		((BaseNetwork)Net.sv).get_write().Send(new SendInfo(connection));
	}

	private static void OnReplicatedVarChanged(string fullName, string value)
	{
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		if (!((BaseNetwork)Net.sv).get_write().Start())
		{
			return;
		}
		List<Connection> list = Pool.GetList<Connection>();
		foreach (Connection connection in Net.sv.connections)
		{
			if (connection.connected)
			{
				list.Add(connection);
			}
		}
		((BaseNetwork)Net.sv).get_write().PacketID((Type)25);
		((BaseNetwork)Net.sv).get_write().Int32(1);
		((BaseNetwork)Net.sv).get_write().String(fullName);
		((BaseNetwork)Net.sv).get_write().String(value);
		((BaseNetwork)Net.sv).get_write().Send(new SendInfo(list));
		Pool.FreeList<Connection>(ref list);
	}
}
