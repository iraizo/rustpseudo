using System;
using Facepunch;
using Facepunch.Nexus;
using Facepunch.Nexus.Models;
using ProtoBuf.Nexus;
using UnityEngine;

namespace Rust.Nexus.Handlers
{
	public abstract class BaseNexusRequestHandler<T> : INexusRequestHandler, IPooled where T : class
	{
		private bool _fireAndForget;

		private bool _sentResult;

		protected NexusZoneDetails FromZone { get; private set; }

		protected Uuid RequestId { get; private set; }

		protected T Request { get; private set; }

		public void Initialize(NexusZoneDetails fromZone, Uuid id, bool fireAndForget, T request)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			FromZone = fromZone;
			RequestId = id;
			Request = request;
			_fireAndForget = fireAndForget;
		}

		public void EnterPool()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			FromZone = null;
			RequestId = Uuid.Empty;
			Request = null;
			_fireAndForget = false;
			_sentResult = false;
		}

		public void LeavePool()
		{
		}

		public void Execute()
		{
			try
			{
				Handle();
				if (!_sentResult)
				{
					SendSuccess();
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				if (_sentResult)
				{
					Debug.LogError((object)"Nexus RPC handler threw an exception but already sent a response!");
				}
				else
				{
					SendError(ex.Message);
				}
			}
		}

		protected abstract void Handle();

		protected void SendSuccess()
		{
			SendResult(success: true);
		}

		protected void SendSuccess(Response response)
		{
			SendResult(success: true, response);
		}

		protected void SendResult(bool success)
		{
			SendResult(success, NewResponse());
		}

		protected void SendResult(bool success, Response response)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			if (_fireAndForget)
			{
				response.Dispose();
				return;
			}
			if (_sentResult)
			{
				response.Dispose();
				throw new InvalidOperationException("Already sent a response for this nexus RPC invocation!");
			}
			response.id = RequestId;
			response.status = Pool.Get<Status>();
			response.status.success = success;
			NexusServer.SendRpcResponse(FromZone.get_Name(), response);
			_sentResult = true;
		}

		protected void SendError(string message)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			if (!_fireAndForget)
			{
				if (_sentResult)
				{
					throw new InvalidOperationException("Already sent a response for this nexus RPC invocation!");
				}
				Response val = Pool.Get<Response>();
				val.id = RequestId;
				val.status = Pool.Get<Status>();
				val.status.success = false;
				val.status.errorMessage = message;
				NexusServer.SendRpcResponse(FromZone.get_Name(), val);
				_sentResult = true;
			}
		}

		protected static Response NewResponse()
		{
			return Pool.Get<Response>();
		}
	}
}
