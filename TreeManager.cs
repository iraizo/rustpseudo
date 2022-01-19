using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class TreeManager : BaseEntity
{
	public static ListHashSet<BaseEntity> entities = new ListHashSet<BaseEntity>(8);

	public static TreeManager server;

	private const int maxTreesPerPacket = 100;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("TreeManager.OnRpcMessage", 0);
		try
		{
			if (rpc == 1907121457 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SERVER_RequestTrees "));
				}
				TimeWarning val2 = TimeWarning.New("SERVER_RequestTrees", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(1907121457u, "SERVER_RequestTrees", this, player, 0uL))
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
							SERVER_RequestTrees(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SERVER_RequestTrees");
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

	public static Vector3 ProtoHalf3ToVec3(Half3 half3)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		Vector3 result = default(Vector3);
		result.x = Mathf.HalfToFloat((ushort)half3.x);
		result.y = Mathf.HalfToFloat((ushort)half3.y);
		result.z = Mathf.HalfToFloat((ushort)half3.z);
		return result;
	}

	public static Half3 Vec3ToProtoHalf3(Vector3 vec3)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		Half3 result = default(Half3);
		result.x = Mathf.FloatToHalf(vec3.x);
		result.y = Mathf.FloatToHalf(vec3.y);
		result.z = Mathf.FloatToHalf(vec3.z);
		return result;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		server = this;
	}

	public static void OnTreeDestroyed(BaseEntity billboardEntity)
	{
		entities.Remove(billboardEntity);
		if (!Application.isLoading && !Application.isQuitting)
		{
			server.ClientRPC(null, "CLIENT_TreeDestroyed", billboardEntity.net.ID);
		}
	}

	public static void OnTreeSpawned(BaseEntity billboardEntity)
	{
		entities.Add(billboardEntity);
		if (!Application.isLoading && !Application.isQuitting)
		{
			Tree val = Pool.Get<Tree>();
			try
			{
				ExtractTreeNetworkData(billboardEntity, val);
				server.ClientRPC<Tree>(null, "CLIENT_TreeSpawned", val);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
	}

	private static void ExtractTreeNetworkData(BaseEntity billboardEntity, Tree tree)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		tree.netId = billboardEntity.net.ID;
		tree.prefabId = billboardEntity.prefabID;
		tree.position = Vec3ToProtoHalf3(((Component)billboardEntity).get_transform().get_position());
		tree.scale = ((Component)billboardEntity).get_transform().get_lossyScale().y;
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(0uL)]
	private void SERVER_RequestTrees(RPCMessage msg)
	{
		BufferList<BaseEntity> values = entities.get_Values();
		TreeList val = null;
		for (int i = 0; i < values.get_Count(); i++)
		{
			BaseEntity billboardEntity = values.get_Item(i);
			Tree val2 = Pool.Get<Tree>();
			ExtractTreeNetworkData(billboardEntity, val2);
			if (val == null)
			{
				val = Pool.Get<TreeList>();
				val.trees = Pool.GetList<Tree>();
			}
			val.trees.Add(val2);
			if (val.trees.Count >= 100)
			{
				ClientRPCPlayer<TreeList>(null, msg.player, "CLIENT_ReceiveTrees", val);
				val.Dispose();
				val = null;
			}
		}
		if (val != null)
		{
			ClientRPCPlayer<TreeList>(null, msg.player, "CLIENT_ReceiveTrees", val);
			val.Dispose();
			val = null;
		}
	}
}
