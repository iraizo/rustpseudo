using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ConVar;
using Facepunch;
using Facepunch.Extend;
using Facepunch.Nexus;
using Facepunch.Nexus.Logging;
using Facepunch.Nexus.Models;
using Facepunch.Sqlite;
using ProtoBuf;
using ProtoBuf.Nexus;
using Rust.Nexus.Handlers;
using UnityEngine;

public static class NexusServer
{
	private struct PendingCall
	{
		public bool IsBroadcast;

		public RealTimeUntil TimeUntilTimeout;

		public TaskCompletionSource<bool> Completion;

		public NexusRpcResult Result;
	}

	private static Dictionary<string, NexusIsland> _existingIslands;

	private static readonly Dictionary<Uuid, PendingCall> PendingCalls = new Dictionary<Uuid, PendingCall>();

	private static RealTimeSince _sinceLastRpcTimeoutCheck = RealTimeSince.op_Implicit(0f);

	private static readonly Dictionary<string, ServerStatus> ZoneStatuses = new Dictionary<string, ServerStatus>();

	private static bool _isRefreshingZoneStatus;

	private static RealTimeSince _lastZoneStatusRefresh;

	private static DateTimeOffset? _lastUnsavedTransfer;

	private static readonly Memoized<string, ulong> SteamIdToString = new Memoized<string, ulong>((Func<ulong, string>)((ulong i) => i.ToString("G")));

	private static readonly MemoryStream WriterStream = new MemoryStream();

	private static readonly ByteArrayStream ReaderStream = new ByteArrayStream();

	private static NexusDB _database;

	public static bool NeedsJournalFlush
	{
		get
		{
			if (Started && _database.OldestJournal.HasValue)
			{
				return (DateTimeOffset.UtcNow - _database.OldestJournal.Value).TotalSeconds >= (double)Nexus.transferFlushTime;
			}
			return false;
		}
	}

	private static int RpcMessageTtl => Nexus.messageLockDuration * 4;

	public static bool NeedTransferFlush
	{
		get
		{
			if (Started && _lastUnsavedTransfer.HasValue)
			{
				return (DateTimeOffset.UtcNow - _lastUnsavedTransfer.Value).TotalSeconds >= (double)Nexus.transferFlushTime;
			}
			return false;
		}
	}

	public static NexusZoneClient ZoneClient { get; private set; }

	public static bool Started { get; private set; }

	public static bool FailedToStart { get; private set; }

	public static string ZoneName
	{
		get
		{
			NexusZoneClient zoneClient = ZoneClient;
			if (zoneClient == null)
			{
				return null;
			}
			ZoneDetails zone = zoneClient.get_Zone();
			if (zone == null)
			{
				return null;
			}
			return zone.get_Name();
		}
	}

	public static long? LastReset
	{
		get
		{
			NexusZoneClient zoneClient = ZoneClient;
			if (zoneClient == null)
			{
				return null;
			}
			NexusDetails nexus = zoneClient.get_Nexus();
			if (nexus == null)
			{
				return null;
			}
			return nexus.get_LastReset();
		}
	}

	public static List<NexusZoneDetails> Zones
	{
		get
		{
			NexusZoneClient zoneClient = ZoneClient;
			if (zoneClient == null)
			{
				return null;
			}
			NexusDetails nexus = zoneClient.get_Nexus();
			if (nexus == null)
			{
				return null;
			}
			return nexus.get_Zones();
		}
	}

	public static void UpdateIslands()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		if (_existingIslands == null)
		{
			_existingIslands = new Dictionary<string, NexusIsland>();
		}
		HashSet<NexusIsland> val = Pool.Get<HashSet<NexusIsland>>();
		val.Clear();
		if (_existingIslands.Count == 0)
		{
			Enumerator<BaseNetworkable> enumerator = BaseNetworkable.serverEntities.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					NexusIsland nexusIsland;
					if ((nexusIsland = enumerator.get_Current() as NexusIsland) != null)
					{
						if (string.IsNullOrEmpty(nexusIsland.ZoneName) || _existingIslands.ContainsKey(nexusIsland.ZoneName))
						{
							val.Add(nexusIsland);
						}
						else
						{
							_existingIslands.Add(nexusIsland.ZoneName, nexusIsland);
						}
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}
		Dictionary<string, NexusZoneDetails> dictionary = Pool.Get<Dictionary<string, NexusZoneDetails>>();
		dictionary.Clear();
		foreach (NexusZoneDetails zone in ZoneClient.get_Nexus().get_Zones())
		{
			if (TryGetZoneStatus(zone.get_Name(), out var status) && status.IsOnline)
			{
				dictionary.Add(zone.get_Name(), zone);
			}
		}
		foreach (KeyValuePair<string, NexusZoneDetails> item in dictionary)
		{
			if (item.Key == ZoneName)
			{
				continue;
			}
			if (!IsCloseTo(item.Value))
			{
				if (_existingIslands.TryGetValue(item.Key, out var value))
				{
					val.Add(value);
				}
				continue;
			}
			var (val2, val3) = CalculateIslandTransform(item.Value);
			if (_existingIslands.TryGetValue(item.Key, out var value2) && (Object)(object)value2 != (Object)null)
			{
				((Component)value2).get_transform().SetPositionAndRotation(val2, val3);
				continue;
			}
			BaseEntity baseEntity = GameManager.server.CreateEntity("assets/content/nexus/island/nexusisland.prefab", val2, val3);
			NexusIsland nexusIsland2;
			if ((nexusIsland2 = baseEntity as NexusIsland) != null)
			{
				nexusIsland2.ZoneName = item.Key;
				nexusIsland2.Spawn();
				_existingIslands[item.Key] = nexusIsland2;
			}
			else
			{
				baseEntity.Kill();
				Debug.LogError((object)"Failed to spawn nexus island entity!");
			}
		}
		foreach (KeyValuePair<string, NexusIsland> existingIsland in _existingIslands)
		{
			if (!dictionary.ContainsKey(existingIsland.Key))
			{
				val.Add(existingIsland.Value);
			}
		}
		Enumerator<NexusIsland> enumerator5 = val.GetEnumerator();
		try
		{
			while (enumerator5.MoveNext())
			{
				NexusIsland current4 = enumerator5.get_Current();
				if (current4 != null)
				{
					if (current4.ZoneName != null)
					{
						_existingIslands.Remove(current4.ZoneName);
					}
					current4.Kill();
				}
			}
		}
		finally
		{
			((IDisposable)enumerator5).Dispose();
		}
		val.Clear();
		Pool.Free<HashSet<NexusIsland>>(ref val);
		dictionary.Clear();
		Pool.Free<Dictionary<string, NexusZoneDetails>>(ref dictionary);
	}

	public static bool TryGetIsland(string zoneName, out NexusIsland island)
	{
		if (_existingIslands == null)
		{
			island = null;
			return false;
		}
		if (_existingIslands.TryGetValue(zoneName, out island))
		{
			return (Object)(object)island != (Object)null;
		}
		return false;
	}

	private static (Vector3, Quaternion) CalculateIslandTransform(NexusZoneDetails otherZone)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		ZoneDetails zone = ZoneClient.get_Zone();
		Vector2 val = new Vector2((float)zone.get_PositionX(), (float)zone.get_PositionY());
		Vector2 val2 = default(Vector2);
		((Vector2)(ref val2))._002Ector((float)otherZone.get_PositionX(), (float)otherZone.get_PositionY());
		float num = Vector2Ex.AngleFromTo(val, val2);
		Vector3 val3 = TerrainMeta.Center + Quaternion.Euler(0f, num, 0f) * Vector3.get_forward() * (float)World.Size * Nexus.islandSpawnDistance;
		Quaternion item = Quaternion.Euler(0f, Vector2Ex.AngleFromTo(Vector3Ex.XZ2D(val3), Vector3Ex.XZ2D(TerrainMeta.Center)), 0f);
		return (Vector3Ex.WithY(val3, WaterSystem.OceanLevel), item);
	}

	private static bool IsCloseTo(NexusZoneDetails otherZone)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return Vector2.Distance(ZoneClient.get_Zone().Position(), otherZone.Position()) <= Nexus.maxBoatTravelDistance;
	}

	private static void ReadIncomingMessages()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		NexusMessage val = default(NexusMessage);
		while (ZoneClient.TryReceiveMessage(ref val))
		{
			if (!((NexusMessage)(ref val)).get_IsBinary())
			{
				Debug.LogWarning((object)"Received a nexus message that's not binary, ignoring");
				ZoneClient.AcknowledgeMessage(val);
				continue;
			}
			byte[] asBinary;
			Packet val2;
			try
			{
				asBinary = ((NexusMessage)(ref val)).get_AsBinary();
				val2 = ReadPacket(asBinary);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				ZoneClient.AcknowledgeMessage(val);
				continue;
			}
			bool num = !RequiresJournaling(val2) || _database.SeenJournaled(Uuid.op_Implicit(((NexusMessage)(ref val)).get_Id()), asBinary);
			ZoneClient.AcknowledgeMessage(val);
			if (!num)
			{
				Debug.LogWarning((object)"Already saw this nexus message, ignoring");
				val2.Dispose();
			}
			else
			{
				HandleMessage(((NexusMessage)(ref val)).get_Id(), val2);
			}
		}
	}

	public static void RestoreUnsavedState()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (Started)
		{
			ReplayJournaledMessages();
			DeleteTransferredEntities();
			ConsoleSystem.Run(Option.get_Server(), "server.save", Array.Empty<object>());
		}
	}

	private static void ReplayJournaledMessages()
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		List<(Guid, long, byte[])> list = _database.ReadJournal();
		if (list.Count == 0)
		{
			Debug.Log((object)"No messages found in the nexus message journal");
			return;
		}
		Debug.Log((object)$"Replaying {list.Count} nexus messages from the journal");
		foreach (var (guid, seconds, data) in list)
		{
			try
			{
				Debug.Log((object)$"Replaying message ID {guid}, received {DateTimeOffset.FromUnixTimeSeconds(seconds):R}");
				Packet packet = ReadPacket(data);
				HandleMessage(Uuid.op_Implicit(guid), packet);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}
		Debug.Log((object)$"Finished replaying {list.Count} nexus messages from the journal");
	}

	private static void DeleteTransferredEntities()
	{
		List<uint> list = _database.ReadTransferred();
		if (list.Count == 0)
		{
			Debug.Log((object)"No entities found in the transferred list");
			return;
		}
		foreach (uint item in list)
		{
			try
			{
				BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(item);
				if (!((Object)(object)baseNetworkable == (Object)null))
				{
					Debug.Log((object)$"Found {baseNetworkable}, killing it because it was transferred away");
					baseNetworkable.Kill();
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}
		Debug.Log((object)$"Finished making sure {list.Count} entities do not exist");
	}

	private static bool RequiresJournaling(Packet packet)
	{
		if (packet.request == null || !packet.request.isFireAndForget)
		{
			return false;
		}
		return packet.request.transfer != null;
	}

	private static void HandleMessage(Uuid id, Packet packet)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (packet.protocol != 220)
			{
				Debug.LogWarning((object)"Received a nexus message with wrong protocol, ignoring");
				return;
			}
			NexusZoneDetails val = List.FindWith<NexusZoneDetails, int>(ZoneClient.get_Nexus().get_Zones(), (Func<NexusZoneDetails, int>)((NexusZoneDetails z) => z.get_Id()), packet.sourceZone);
			if (val == null)
			{
				Debug.LogWarning((object)$"Received a nexus message from unknown zone ID {packet.sourceZone}, ignoring");
			}
			else if (packet.request != null)
			{
				HandleRpcInvocation(val, id, packet);
			}
			else if (packet.response != null)
			{
				HandleRpcResponse(val, id, packet);
			}
			else
			{
				Debug.LogWarning((object)"Received a nexus message without the request or request sections set, ignoring");
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
		finally
		{
			if (packet != null)
			{
				packet.Dispose();
			}
		}
	}

	private static Packet ReadPacket(byte[] data)
	{
		ReaderStream.SetData(data, 0, data.Length);
		return Packet.Deserialize((Stream)(object)ReaderStream);
	}

	private static Task SendRequestImpl(Uuid id, Request request, string toZoneName, int? ttl = null)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		Packet val = Pool.Get<Packet>();
		val.protocol = 220u;
		val.sourceZone = ZoneClient.get_Zone().get_ZoneId();
		val.request = request;
		return SendPacket(id, val, toZoneName, ttl);
	}

	private static async void SendResponseImpl(Response response, string toZoneName, int? ttl = null)
	{
		try
		{
			Packet val = Pool.Get<Packet>();
			val.protocol = 220u;
			val.sourceZone = ZoneClient.get_Zone().get_ZoneId();
			val.response = response;
			await SendPacket(Uuid.Generate(), val, toZoneName, ttl);
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	private static Task SendPacket(Uuid id, Packet packet, string toZoneName, int? ttl = null)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		WriterStream.SetLength(0L);
		WriterStream.Position = 0L;
		packet.WriteToStream((Stream)WriterStream);
		Memory<byte> memory = new Memory<byte>(WriterStream.GetBuffer(), 0, (int)WriterStream.Length);
		packet.Dispose();
		return ZoneClient.SendMessage(toZoneName, id, memory, ttl);
	}

	public static async Task<Response> ZoneRpc(string zone, Request request, float timeoutAfter = 30f)
	{
		if (string.IsNullOrEmpty(zone))
		{
			throw new ArgumentNullException("zone");
		}
		using NexusRpcResult nexusRpcResult = await CallRpcImpl(zone, request, timeoutAfter, throwOnTimeout: true);
		Response val = nexusRpcResult.Responses[zone];
		if (!string.IsNullOrWhiteSpace(val.status?.errorMessage))
		{
			throw new Exception(val.status.errorMessage);
		}
		return val.Copy();
	}

	public static Task<NexusRpcResult> BroadcastRpc(Request request, float timeoutAfter = 30f)
	{
		return CallRpcImpl(null, request, timeoutAfter, throwOnTimeout: false);
	}

	private static async Task<NexusRpcResult> CallRpcImpl(string zone, Request request, float timeoutAfter, bool throwOnTimeout)
	{
		Uuid id = Uuid.Generate();
		TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
		NexusRpcResult result = Pool.Get<NexusRpcResult>();
		try
		{
			await SendRequestImpl(id, request, zone, RpcMessageTtl);
			PendingCalls.Add(id, new PendingCall
			{
				IsBroadcast = string.IsNullOrWhiteSpace(zone),
				TimeUntilTimeout = RealTimeUntil.op_Implicit(timeoutAfter * Nexus.rpcTimeoutMultiplier),
				Completion = tcs,
				Result = result
			});
			bool flag = await tcs.Task;
			if (throwOnTimeout)
			{
				if (!flag)
				{
					throw new TimeoutException("Nexus RPC invocation timed out");
				}
				return result;
			}
			return result;
		}
		catch
		{
			Pool.Free<NexusRpcResult>(ref result);
			throw;
		}
	}

	internal static void SendRpcResponse(string zone, Response response)
	{
		SendResponseImpl(response, zone, RpcMessageTtl);
	}

	private static void HandleRpcInvocation(NexusZoneDetails from, Uuid id, Packet packet)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (Handle<TransferRequest, TransferHandler>((Request r) => r.transfer, out var requestHandler2) || Handle<PingRequest, PingHandler>((Request r) => r.ping, out requestHandler2) || Handle<SpawnOptionsRequest, SpawnOptionsHandler>((Request r) => r.spawnOptions, out requestHandler2) || Handle<SleepingBagRespawnRequest, RespawnAtBagHandler>((Request r) => r.respawnAtBag, out requestHandler2) || Handle<SleepingBagDestroyRequest, DestroyBagHandler>((Request r) => r.destroyBag, out requestHandler2))
		{
			requestHandler2.Execute();
			Pool.FreeDynamic<INexusRequestHandler>(ref requestHandler2);
		}
		else
		{
			Debug.LogError((object)"Received a nexus RPC invocation with a missing or unsupported request, ignoring");
		}
		bool Handle<TProto, THandler>(Func<Request, TProto> protoSelector, out INexusRequestHandler requestHandler) where TProto : class where THandler : BaseNexusRequestHandler<TProto>, new()
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			TProto val = protoSelector(packet.request);
			if (val == null)
			{
				requestHandler = null;
				return false;
			}
			THandler val2 = Pool.Get<THandler>();
			val2.Initialize(from, id, packet.request.isFireAndForget, val);
			requestHandler = val2;
			return true;
		}
	}

	private static void HandleRpcResponse(NexusZoneDetails from, Uuid id, Packet packet)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		if (!PendingCalls.TryGetValue(packet.response.id, out var value))
		{
			Debug.LogWarning((object)"Received an unexpected nexus RPC response (likely timed out), ignoring");
			return;
		}
		if (!value.Result.Responses.ContainsKey(from.get_Name()))
		{
			value.Result.Responses.Add(from.get_Name(), packet.response.Copy());
		}
		int num;
		if (!value.IsBroadcast)
		{
			num = 1;
		}
		else
		{
			NexusZoneClient zoneClient = ZoneClient;
			int? obj;
			if (zoneClient == null)
			{
				obj = null;
			}
			else
			{
				NexusDetails nexus = zoneClient.get_Nexus();
				obj = ((nexus == null) ? null : nexus.get_Zones()?.Count);
			}
			num = (obj ?? 0) - 1;
		}
		int num2 = num;
		if (value.Result.Responses.Count >= num2)
		{
			PendingCalls.Remove(id);
			value.Completion.TrySetResult(result: true);
		}
	}

	private static void CheckForRpcTimeouts()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		if (RealTimeSince.op_Implicit(_sinceLastRpcTimeoutCheck) < 1f)
		{
			return;
		}
		_sinceLastRpcTimeoutCheck = RealTimeSince.op_Implicit(0f);
		List<(Uuid, PendingCall)> list = Pool.GetList<(Uuid, PendingCall)>();
		foreach (KeyValuePair<Uuid, PendingCall> pendingCall in PendingCalls)
		{
			Uuid key = pendingCall.Key;
			PendingCall value = pendingCall.Value;
			if (RealTimeUntil.op_Implicit(value.TimeUntilTimeout) <= 0f)
			{
				list.Add((key, value));
			}
		}
		foreach (var item3 in list)
		{
			Uuid item = item3.Item1;
			PendingCall item2 = item3.Item2;
			PendingCalls.Remove(item);
			item2.Completion.TrySetResult(result: false);
		}
		Pool.FreeList<(Uuid, PendingCall)>(ref list);
	}

	private static void RefreshZoneStatus()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (!_isRefreshingZoneStatus && !(RealTimeSince.op_Implicit(_lastZoneStatusRefresh) < Nexus.pingInterval))
		{
			RefreshZoneStatusImpl();
		}
		static async void RefreshZoneStatusImpl()
		{
			try
			{
				_isRefreshingZoneStatus = true;
				_lastZoneStatusRefresh = RealTimeSince.op_Implicit(0f);
				Request obj = Pool.Get<Request>();
				obj.ping = Pool.Get<PingRequest>();
				using (NexusRpcResult nexusRpcResult = await BroadcastRpc(obj))
				{
					List<string> list = Pool.GetList<string>();
					foreach (string key in ZoneStatuses.Keys)
					{
						if (List.FindWith<NexusZoneDetails, string>(Zones, (Func<NexusZoneDetails, string>)((NexusZoneDetails z) => z.get_Name()), key) == null)
						{
							list.Add(key);
						}
					}
					foreach (string item in list)
					{
						ZoneStatuses.Remove(item);
					}
					Pool.FreeList<string>(ref list);
					ServerStatus value;
					foreach (KeyValuePair<string, Response> response in nexusRpcResult.Responses)
					{
						value = (ZoneStatuses[response.Key] = new ServerStatus
						{
							IsOnline = true,
							LastSeen = RealTimeSince.op_Implicit(0f),
							Players = response.Value.ping.players,
							MaxPlayers = response.Value.ping.maxPlayers,
							QueuedPlayers = response.Value.ping.queuedPlayers
						});
					}
					foreach (NexusZoneDetails zone in Zones)
					{
						if (!nexusRpcResult.Responses.ContainsKey(zone.get_Name()))
						{
							if (ZoneStatuses.TryGetValue(zone.get_Name(), out var value2))
							{
								Dictionary<string, ServerStatus> zoneStatuses = ZoneStatuses;
								string name = zone.get_Name();
								value = new ServerStatus
								{
									IsOnline = false,
									LastSeen = value2.LastSeen,
									Players = value2.Players,
									MaxPlayers = value2.MaxPlayers,
									QueuedPlayers = value2.QueuedPlayers
								};
								zoneStatuses[name] = value;
							}
							else
							{
								Dictionary<string, ServerStatus> zoneStatuses2 = ZoneStatuses;
								string name2 = zone.get_Name();
								value = new ServerStatus
								{
									IsOnline = false
								};
								zoneStatuses2[name2] = value;
							}
						}
					}
				}
				_lastZoneStatusRefresh = RealTimeSince.op_Implicit(0f);
			}
			finally
			{
				_isRefreshingZoneStatus = false;
			}
			OnZoneStatusesRefreshed();
		}
	}

	public static bool TryGetZoneStatus(string zone, out ServerStatus status)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (!Started)
		{
			status = default(ServerStatus);
			return false;
		}
		if (zone == ZoneName)
		{
			status = new ServerStatus
			{
				IsOnline = true,
				LastSeen = RealTimeSince.op_Implicit(0f),
				Players = BasePlayer.activePlayerList.get_Count(),
				MaxPlayers = Server.maxplayers,
				QueuedPlayers = SingletonComponent<ServerMgr>.Instance.connectionQueue.Queued
			};
			return true;
		}
		return ZoneStatuses.TryGetValue(zone, out status);
	}

	private static void OnZoneStatusesRefreshed()
	{
		UpdateIslands();
	}

	public static async Task TransferEntity(BaseEntity entity, string toZoneName, string method)
	{
		try
		{
			await TransferEntityImpl(FindRootEntity(entity), toZoneName, method, ZoneName, toZoneName);
		}
		catch (Exception ex)
		{
			Debug.LogWarning((object)ex);
		}
	}

	public static async Task TransferEntityImpl(BaseEntity rootEntity, string toZoneName, string method, string from, string to)
	{
		if ((Object)(object)rootEntity == (Object)null)
		{
			throw new ArgumentNullException("rootEntity");
		}
		if (rootEntity.HasParent())
		{
			throw new ArgumentException("Cannot transfer an entity which has a parent - transfer the root instead", "rootEntity");
		}
		if (string.IsNullOrWhiteSpace(toZoneName))
		{
			throw new ArgumentNullException("toZoneName");
		}
		if (toZoneName == ZoneName)
		{
			throw new ArgumentException("Attempted to transfer a player to the current server's zone", "toZoneName");
		}
		NexusZoneDetails toZone = List.FindWith<NexusZoneDetails, string>(ZoneClient.get_Nexus().get_Zones(), (Func<NexusZoneDetails, string>)((NexusZoneDetails z) => z.get_Name()), toZoneName);
		if (toZone == null)
		{
			throw new ArgumentException("Target zone (" + toZoneName + ") was not found in the nexus", "toZoneName");
		}
		BuildTransferRequest(rootEntity, method, from, to, out var request, out var entities, out var players, out var playerIds);
		if (playerIds.Count > 0)
		{
			await ZoneClient.RegisterTransfers(toZoneName, (IEnumerable<string>)playerIds);
		}
		await SendRequestImpl(Uuid.Generate(), request, toZoneName);
		foreach (BasePlayer item in players)
		{
			if (item.IsConnected)
			{
				ConsoleNetwork.SendClientCommand(item.net.get_connection(), "nexus.redirect", toZone.get_IpAddress(), toZone.get_Port());
				item.Kick("Redirecting to another zone...");
			}
		}
		List<uint> list = Pool.GetList<uint>();
		for (int num = entities.Count - 1; num >= 0; num--)
		{
			try
			{
				BaseNetworkable baseNetworkable = entities[num];
				if (baseNetworkable.net != null && baseNetworkable.net.ID != 0)
				{
					list.Add(baseNetworkable.net.ID);
				}
				baseNetworkable.Kill();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}
		_database.MarkTransferred(list);
		Pool.FreeList<uint>(ref list);
		Pool.FreeList<BaseNetworkable>(ref entities);
		Pool.FreeList<BasePlayer>(ref players);
		Pool.FreeList<string>(ref playerIds);
		_lastUnsavedTransfer = DateTimeOffset.UtcNow;
	}

	private static void BuildTransferRequest(BaseEntity rootEntity, string method, string from, string to, out Request request, out List<BaseNetworkable> entities, out List<BasePlayer> players, out List<string> playerIds)
	{
		List<BaseNetworkable> entitiesList = (entities = Pool.GetList<BaseNetworkable>());
		List<BasePlayer> playerList = (players = Pool.GetList<BasePlayer>());
		List<string> playerIdsList = (playerIds = Pool.GetList<string>());
		request = Pool.Get<Request>();
		request.isFireAndForget = true;
		request.transfer = Pool.Get<TransferRequest>();
		request.transfer.method = method;
		request.transfer.from = from;
		request.transfer.to = to;
		List<Entity> serializedEntities = (request.transfer.entities = Pool.GetList<Entity>());
		List<PlayerSecondaryData> secondaryData = (request.transfer.secondaryData = Pool.GetList<PlayerSecondaryData>());
		Queue<BaseNetworkable> pendingEntities = Pool.Get<Queue<BaseNetworkable>>();
		pendingEntities.Clear();
		HashSet<uint> seenEntityIds = Pool.Get<HashSet<uint>>();
		seenEntityIds.Clear();
		pendingEntities.Enqueue((BaseNetworkable)rootEntity);
		seenEntityIds.Add(rootEntity.net.ID);
		while (pendingEntities.get_Count() > 0)
		{
			BaseNetworkable baseNetworkable = pendingEntities.Dequeue();
			Entity val = null;
			if (CanTransferEntity(baseNetworkable))
			{
				val = AddEntity(baseNetworkable);
			}
			foreach (BaseEntity child in baseNetworkable.children)
			{
				if ((Object)(object)child != (Object)null && seenEntityIds.Add(child.net.ID))
				{
					pendingEntities.Enqueue((BaseNetworkable)child);
				}
			}
			BaseMountable baseMountable;
			if ((baseMountable = baseNetworkable as BaseMountable) != null)
			{
				BasePlayer mounted = baseMountable.GetMounted();
				if ((Object)(object)mounted != (Object)null && seenEntityIds.Add(mounted.net.ID))
				{
					pendingEntities.Enqueue((BaseNetworkable)mounted);
				}
			}
			if (val != null)
			{
				val.InspectUids((UidInspector<uint>)ScanForAdditionalEntities);
			}
		}
		seenEntityIds.Clear();
		Pool.Free<HashSet<uint>>(ref seenEntityIds);
		pendingEntities.Clear();
		Pool.Free<Queue<BaseNetworkable>>(ref pendingEntities);
		Entity AddEntity(BaseNetworkable entity)
		{
			BaseNetworkable.SaveInfo saveInfo = default(BaseNetworkable.SaveInfo);
			saveInfo.forDisk = true;
			saveInfo.forTransfer = true;
			saveInfo.msg = Pool.Get<Entity>();
			BaseNetworkable.SaveInfo info = saveInfo;
			entity.Save(info);
			serializedEntities.Add(info.msg);
			entitiesList.Add(entity);
			BasePlayer basePlayer;
			if ((basePlayer = entity as BasePlayer) != null && ((object)basePlayer).GetType() == typeof(BasePlayer) && basePlayer.userID > uint.MaxValue)
			{
				playerList.Add(basePlayer);
				playerIdsList.Add(basePlayer.UserIDString);
				secondaryData.Add(basePlayer.SaveSecondaryData());
			}
			return info.msg;
		}
		void ScanForAdditionalEntities(UidType type, ref uint uid)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			if ((int)type == 0 && uid != 0 && seenEntityIds.Add(uid))
			{
				BaseNetworkable baseNetworkable2 = BaseNetworkable.serverEntities.Find(uid);
				if ((Object)(object)baseNetworkable2 != (Object)null)
				{
					pendingEntities.Enqueue(baseNetworkable2);
				}
			}
		}
	}

	private static bool CanTransferEntity(BaseNetworkable networkable)
	{
		if ((Object)(object)networkable == (Object)null)
		{
			return false;
		}
		BaseEntity baseEntity;
		if ((baseEntity = networkable as BaseEntity) != null && !baseEntity.enableSaving)
		{
			return false;
		}
		return true;
	}

	public static BaseEntity FindRootEntity(BaseEntity startEntity)
	{
		BaseEntity baseEntity = startEntity;
		BaseEntity parent2;
		while (TryGetParent(baseEntity, out parent2))
		{
			baseEntity = parent2;
		}
		return baseEntity;
		static bool TryGetParent(BaseEntity entity, out BaseEntity parent)
		{
			BaseEntity parentEntity = entity.GetParentEntity();
			if ((Object)(object)parentEntity != (Object)null)
			{
				parent = parentEntity;
				return true;
			}
			BasePlayer basePlayer;
			if ((basePlayer = entity as BasePlayer) != null)
			{
				BaseMountable mounted = basePlayer.GetMounted();
				if ((Object)(object)mounted != (Object)null)
				{
					parent = mounted;
					return true;
				}
			}
			parent = null;
			return false;
		}
	}

	public static IEnumerator Initialize()
	{
		if (Started)
		{
			Debug.LogError((object)"NexusServer was already started");
			yield break;
		}
		ZoneClient = null;
		_database = null;
		ZoneController.Instance = null;
		Started = false;
		FailedToStart = true;
		if (string.IsNullOrWhiteSpace(Nexus.endpoint) || !Nexus.endpoint.StartsWith("http") || string.IsNullOrWhiteSpace(Nexus.secretKey))
		{
			Debug.Log((object)"Nexus endpoint and/or secret key is not set, not starting nexus connection");
			FailedToStart = false;
			yield break;
		}
		_database = new NexusDB();
		((Database)_database).Open($"{Server.rootFolder}/nexus.{220}.db", true);
		_database.Initialize();
		ZoneClient = new NexusZoneClient((INexusLogger)(object)NexusLogger.Instance, Nexus.endpoint, Nexus.secretKey, Nexus.messageLockDuration);
		NexusZoneClient zoneClient = ZoneClient;
		object obj = _003C_003Ec._003C_003E9__65_0;
		if (obj == null)
		{
			NexusErrorHandler val = delegate(NexusClient _, Exception ex)
			{
				Debug.LogException(ex);
			};
			obj = (object)val;
			_003C_003Ec._003C_003E9__65_0 = val;
		}
		((NexusClient)zoneClient).add_OnError((NexusErrorHandler)obj);
		Task startTask = ((NexusClient)ZoneClient).Start();
		yield return (object)new WaitUntil((Func<bool>)(() => startTask.IsCompleted));
		if (startTask.Exception != null)
		{
			Debug.LogException((Exception)startTask.Exception);
			yield break;
		}
		if (string.IsNullOrWhiteSpace(ZoneName))
		{
			Debug.LogError((object)"Zone name is not available after nexus initialization");
			yield break;
		}
		Debug.Log((object)$"Connected as zone '{ZoneName}' in Nexus {ZoneClient.get_Zone().get_NexusName()} (id={ZoneClient.get_Zone().get_NexusId()})");
		ZoneController.Instance = BuildZoneController(Nexus.zoneController);
		if (ZoneController.Instance == null)
		{
			Debug.LogError((object)(string.IsNullOrWhiteSpace(Nexus.zoneController) ? "Zone controller was not specified (nexus.zoneController convar)" : ("Zone controller is not supported: " + Nexus.zoneController)));
			yield break;
		}
		Started = true;
		FailedToStart = false;
	}

	public static void Shutdown()
	{
		Started = false;
		FailedToStart = false;
		_existingIslands?.Clear();
		NexusZoneClient zoneClient = ZoneClient;
		if (zoneClient != null)
		{
			((NexusClient)zoneClient).Dispose();
		}
		ZoneClient = null;
		NexusDB database = _database;
		if (database != null)
		{
			((Database)database).Close();
		}
		_database = null;
	}

	public static void Update()
	{
		if (Started)
		{
			ReadIncomingMessages();
			CheckForRpcTimeouts();
			RefreshZoneStatus();
		}
	}

	public static NexusZoneDetails FindZone(string zoneName)
	{
		NexusZoneClient zoneClient = ZoneClient;
		if (zoneClient == null)
		{
			return null;
		}
		NexusDetails nexus = zoneClient.get_Nexus();
		if (nexus == null)
		{
			return null;
		}
		List<NexusZoneDetails> zones = nexus.get_Zones();
		if (zones == null)
		{
			return null;
		}
		return List.FindWith<NexusZoneDetails, string>(zones, (Func<NexusZoneDetails, string>)((NexusZoneDetails z) => z.get_Name()), zoneName);
	}

	public static Task<NexusLoginResult> Login(ulong steamId)
	{
		return ZoneClient.PlayerLogin(SteamIdToString.Get(steamId));
	}

	public static void Logout(ulong steamId)
	{
		NexusZoneClient zoneClient = ZoneClient;
		if (zoneClient != null)
		{
			zoneClient.PlayerLogout(SteamIdToString.Get(steamId));
		}
	}

	public static bool TryGetPlayer(ulong steamId, out NexusPlayer player)
	{
		if (!Started)
		{
			player = null;
			return false;
		}
		return ZoneClient.TryGetPlayer(SteamIdToString.Get(steamId), ref player);
	}

	public static Task AssignInitialZone(ulong steamId, string zoneName)
	{
		return ZoneClient.Assign(steamId.ToString("G"), zoneName);
	}

	private static ZoneController BuildZoneController(string name)
	{
		string text = name.ToLowerInvariant();
		if (text == "basic")
		{
			return new BasicZoneController(ZoneClient);
		}
		return null;
	}

	public static void PostGameSaved()
	{
		_database?.ClearJournal();
		_database?.ClearTransferred();
		_lastUnsavedTransfer = null;
	}
}
