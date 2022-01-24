using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using CompanionServer.Handlers;
using ConVar;
using Facepunch;
using Fleck;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer
{
	public class Listener : IDisposable, IBroadcastSender<Connection, AppBroadcast>
	{
		private struct Message
		{
			public readonly Connection Connection;

			public readonly MemoryBuffer Buffer;

			public Message(Connection connection, MemoryBuffer buffer)
			{
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				Connection = connection;
				Buffer = buffer;
			}
		}

		private static readonly ByteArrayStream Stream = new ByteArrayStream();

		private readonly TokenBucketList<IPAddress> _ipTokenBuckets;

		private readonly BanList<IPAddress> _ipBans;

		private readonly TokenBucketList<ulong> _playerTokenBuckets;

		private readonly TokenBucketList<ulong> _pairingTokenBuckets;

		private readonly Queue<Message> _messageQueue;

		private readonly WebSocketServer _server;

		private readonly Stopwatch _stopwatch;

		private RealTimeSince _lastCleanup;

		public readonly IPAddress Address;

		public readonly int Port;

		public readonly ConnectionLimiter Limiter;

		public readonly SubscriberList<PlayerTarget, Connection, AppBroadcast> PlayerSubscribers;

		public readonly SubscriberList<EntityTarget, Connection, AppBroadcast> EntitySubscribers;

		public Listener(IPAddress ipAddress, int port)
		{
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Expected O, but got Unknown
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Expected O, but got Unknown
			Address = ipAddress;
			Port = port;
			Limiter = new ConnectionLimiter();
			_ipTokenBuckets = new TokenBucketList<IPAddress>(50.0, 15.0);
			_ipBans = new BanList<IPAddress>();
			_playerTokenBuckets = new TokenBucketList<ulong>(25.0, 3.0);
			_pairingTokenBuckets = new TokenBucketList<ulong>(5.0, 0.1);
			_messageQueue = new Queue<Message>();
			_server = new WebSocketServer($"ws://{Address}:{Port}/", true);
			_server.Start((Action<IWebSocketConnection>)delegate(IWebSocketConnection socket)
			{
				//IL_0077: Unknown result type (might be due to invalid IL or missing references)
				//IL_0081: Expected O, but got Unknown
				IPAddress address = socket.get_ConnectionInfo().get_ClientIpAddress();
				if (!Limiter.TryAdd(address) || _ipBans.IsBanned(address))
				{
					socket.Close();
				}
				else
				{
					Connection conn = new Connection(this, socket);
					socket.set_OnClose((Action)delegate
					{
						Limiter.Remove(address);
						conn.OnClose();
					});
					socket.set_OnBinary(new BinaryDataHandler(conn.OnMessage));
					socket.set_OnError((Action<Exception>)Debug.LogError);
				}
			});
			_stopwatch = new Stopwatch();
			PlayerSubscribers = new SubscriberList<PlayerTarget, Connection, AppBroadcast>(this);
			EntitySubscribers = new SubscriberList<EntityTarget, Connection, AppBroadcast>(this);
		}

		public void Dispose()
		{
			WebSocketServer server = _server;
			if (server != null)
			{
				server.Dispose();
			}
		}

		internal void Enqueue(Connection connection, MemoryBuffer data)
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			lock (_messageQueue)
			{
				if (!App.update || _messageQueue.get_Count() >= App.queuelimit)
				{
					((MemoryBuffer)(ref data)).Dispose();
					return;
				}
				Message message = new Message(connection, data);
				_messageQueue.Enqueue(message);
			}
		}

		public void Update()
		{
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			if (!App.update)
			{
				return;
			}
			TimeWarning val = TimeWarning.New("CompanionServer.MessageQueue", 0);
			try
			{
				lock (_messageQueue)
				{
					_stopwatch.Restart();
					while (_messageQueue.get_Count() > 0 && _stopwatch.get_Elapsed().TotalMilliseconds < 5.0)
					{
						Message message = _messageQueue.Dequeue();
						Dispatch(message);
					}
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
			if (RealTimeSince.op_Implicit(_lastCleanup) >= 3f)
			{
				_lastCleanup = RealTimeSince.op_Implicit(0f);
				_ipTokenBuckets.Cleanup();
				_ipBans.Cleanup();
				_playerTokenBuckets.Cleanup();
				_pairingTokenBuckets.Cleanup();
			}
		}

		private void Dispatch(Message message)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			MemoryBuffer buffer = message.Buffer;
			AppRequest request;
			try
			{
				ByteArrayStream stream = Stream;
				MemoryBuffer buffer2 = message.Buffer;
				byte[] data = ((MemoryBuffer)(ref buffer2)).get_Data();
				buffer2 = message.Buffer;
				stream.SetData(data, 0, ((MemoryBuffer)(ref buffer2)).get_Length());
				request = AppRequest.Deserialize((Stream)(object)Stream);
			}
			catch
			{
				DebugEx.LogWarning((object)$"Malformed companion packet from {message.Connection.Address}", (StackTraceLogType)0);
				message.Connection.Close();
				throw;
			}
			finally
			{
				((MemoryBuffer)(ref buffer)).Dispose();
			}
			if (Handle<AppEmpty, Info>((AppRequest r) => r.getInfo, out var requestHandler2) || Handle<AppEmpty, CompanionServer.Handlers.Time>((AppRequest r) => r.getTime, out requestHandler2) || Handle<AppEmpty, Map>((AppRequest r) => r.getMap, out requestHandler2) || Handle<AppEmpty, TeamInfo>((AppRequest r) => r.getTeamInfo, out requestHandler2) || Handle<AppEmpty, TeamChat>((AppRequest r) => r.getTeamChat, out requestHandler2) || Handle<AppSendMessage, SendTeamChat>((AppRequest r) => r.sendTeamMessage, out requestHandler2) || Handle<AppEmpty, EntityInfo>((AppRequest r) => r.getEntityInfo, out requestHandler2) || Handle<AppSetEntityValue, SetEntityValue>((AppRequest r) => r.setEntityValue, out requestHandler2) || Handle<AppEmpty, CheckSubscription>((AppRequest r) => r.checkSubscription, out requestHandler2) || Handle<AppFlag, SetSubscription>((AppRequest r) => r.setSubscription, out requestHandler2) || Handle<AppEmpty, MapMarkers>((AppRequest r) => r.getMapMarkers, out requestHandler2) || Handle<AppPromoteToLeader, PromoteToLeader>((AppRequest r) => r.promoteToLeader, out requestHandler2))
			{
				try
				{
					ValidationResult validationResult = requestHandler2.Validate();
					switch (validationResult)
					{
					case ValidationResult.Rejected:
						message.Connection.Close();
						break;
					default:
						requestHandler2.SendError(validationResult.ToErrorCode());
						break;
					case ValidationResult.Success:
						requestHandler2.Execute();
						break;
					}
				}
				catch (Exception arg)
				{
					Debug.LogError((object)$"AppRequest threw an exception: {arg}");
					requestHandler2.SendError("server_error");
				}
				Pool.FreeDynamic<IHandler>(ref requestHandler2);
			}
			else
			{
				AppResponse val = Pool.Get<AppResponse>();
				val.seq = request.seq;
				val.error = Pool.Get<AppError>();
				val.error.error = "unhandled";
				message.Connection.Send(val);
				request.Dispose();
			}
			bool Handle<TProto, THandler>(Func<AppRequest, TProto> protoSelector, out IHandler requestHandler) where TProto : class where THandler : BaseHandler<TProto>, new()
			{
				TProto val2 = protoSelector(request);
				if (val2 == null)
				{
					requestHandler = null;
					return false;
				}
				THandler val3 = Pool.Get<THandler>();
				val3.Initialize(_playerTokenBuckets, message.Connection, request, val2);
				requestHandler = val3;
				return true;
			}
		}

		public void BroadcastTo(List<Connection> targets, AppBroadcast broadcast)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			MemoryBuffer broadcastBuffer = GetBroadcastBuffer(broadcast);
			foreach (Connection target in targets)
			{
				target.SendRaw(((MemoryBuffer)(ref broadcastBuffer)).DontDispose());
			}
			((MemoryBuffer)(ref broadcastBuffer)).Dispose();
		}

		private static MemoryBuffer GetBroadcastBuffer(AppBroadcast broadcast)
		{
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			MemoryBuffer val = default(MemoryBuffer);
			((MemoryBuffer)(ref val))._002Ector(65536);
			Stream.SetData(((MemoryBuffer)(ref val)).get_Data(), 0, ((MemoryBuffer)(ref val)).get_Length());
			AppMessage val2 = Pool.Get<AppMessage>();
			val2.broadcast = broadcast;
			val2.ToProto((Stream)(object)Stream);
			if (val2.ShouldPool)
			{
				val2.Dispose();
			}
			return ((MemoryBuffer)(ref val)).Slice((int)((Stream)(object)Stream).Position);
		}

		public bool CanSendPairingNotification(ulong playerId)
		{
			return _pairingTokenBuckets.Get(playerId).TryTake(1.0);
		}
	}
}
