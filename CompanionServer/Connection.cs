using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using ConVar;
using Facepunch;
using Fleck;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer
{
	public class Connection : IConnection
	{
		private static readonly MemoryStream MessageStream = new MemoryStream(1048576);

		private readonly Listener _listener;

		private readonly IWebSocketConnection _connection;

		private readonly HashSet<PlayerTarget> _subscribedPlayers;

		private readonly HashSet<EntityTarget> _subscribedEntities;

		public IPAddress Address => _connection.get_ConnectionInfo().get_ClientIpAddress();

		public Connection(Listener listener, IWebSocketConnection connection)
		{
			_listener = listener;
			_connection = connection;
			_subscribedPlayers = new HashSet<PlayerTarget>();
			_subscribedEntities = new HashSet<EntityTarget>();
		}

		public void OnClose()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			Enumerator<PlayerTarget> enumerator = _subscribedPlayers.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					PlayerTarget current = enumerator.get_Current();
					_listener.PlayerSubscribers.Remove(current, this);
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			Enumerator<EntityTarget> enumerator2 = _subscribedEntities.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					EntityTarget current2 = enumerator2.get_Current();
					_listener.EntitySubscribers.Remove(current2, this);
				}
			}
			finally
			{
				((IDisposable)enumerator2).Dispose();
			}
		}

		public void OnMessage(Span<byte> data)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			if (App.update && App.queuelimit > 0)
			{
				MemoryBuffer val = default(MemoryBuffer);
				((MemoryBuffer)(ref val))._002Ector(data.Length);
				data.CopyTo(MemoryBuffer.op_Implicit(val));
				_listener.Enqueue(this, ((MemoryBuffer)(ref val)).Slice(data.Length));
			}
		}

		public void Close()
		{
			IWebSocketConnection connection = _connection;
			if (connection != null)
			{
				connection.Close();
			}
		}

		public void Send(AppResponse response)
		{
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			AppMessage val = Pool.Get<AppMessage>();
			val.response = response;
			MessageStream.Position = 0L;
			val.ToProto((Stream)MessageStream);
			int num = (int)MessageStream.Position;
			MessageStream.Position = 0L;
			MemoryBuffer val2 = default(MemoryBuffer);
			((MemoryBuffer)(ref val2))._002Ector(num);
			MessageStream.Read(((MemoryBuffer)(ref val2)).get_Data(), 0, num);
			if (val.ShouldPool)
			{
				val.Dispose();
			}
			SendRaw(((MemoryBuffer)(ref val2)).Slice(num));
		}

		public void Subscribe(PlayerTarget target)
		{
			if (_subscribedPlayers.Add(target))
			{
				_listener.PlayerSubscribers.Add(target, this);
			}
		}

		public void Unsubscribe(PlayerTarget target)
		{
			if (_subscribedPlayers.Remove(target))
			{
				_listener.PlayerSubscribers.Remove(target, this);
			}
		}

		public void Subscribe(EntityTarget target)
		{
			if (_subscribedEntities.Add(target))
			{
				_listener.EntitySubscribers.Add(target, this);
			}
		}

		public void Unsubscribe(EntityTarget target)
		{
			if (_subscribedEntities.Remove(target))
			{
				_listener.EntitySubscribers.Remove(target, this);
			}
		}

		public void SendRaw(MemoryBuffer data)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				_connection.Send(data);
			}
			catch (Exception arg)
			{
				Debug.LogError((object)$"Failed to send message to app client {_connection.get_ConnectionInfo().get_ClientIpAddress()}: {arg}");
			}
		}
	}
}
