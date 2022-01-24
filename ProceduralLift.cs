using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class ProceduralLift : BaseEntity
{
	public float movementSpeed = 1f;

	public float resetDelay = 5f;

	public ProceduralLiftCabin cabin;

	public ProceduralLiftStop[] stops;

	public GameObjectRef triggerPrefab;

	public string triggerBone;

	private int floorIndex = -1;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("ProceduralLift.OnRpcMessage", 0);
		try
		{
			if (rpc == 2657791441u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_UseLift "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_UseLift", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(2657791441u, "RPC_UseLift", this, player, 3f))
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
							RPCMessage rpc2 = rPCMessage;
							RPC_UseLift(rpc2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_UseLift");
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

	public override void Spawn()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		base.Spawn();
		if (!Application.isLoadingSave)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(triggerPrefab.resourcePath, Vector3.get_zero(), Quaternion.get_identity());
			baseEntity.Spawn();
			baseEntity.SetParent(this, triggerBone);
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	private void RPC_UseLift(RPCMessage rpc)
	{
		if (rpc.player.CanInteract() && !IsBusy())
		{
			MoveToFloor((floorIndex + 1) % stops.Length);
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		SnapToFloor(0);
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.lift = Pool.Get<Lift>();
		info.msg.lift.floor = floorIndex;
	}

	public override void Load(LoadInfo info)
	{
		if (info.msg.lift != null)
		{
			if (floorIndex == -1)
			{
				SnapToFloor(info.msg.lift.floor);
			}
			else
			{
				MoveToFloor(info.msg.lift.floor);
			}
		}
		base.Load(info);
	}

	private void ResetLift()
	{
		MoveToFloor(0);
	}

	private void MoveToFloor(int floor)
	{
		floorIndex = Mathf.Clamp(floor, 0, stops.Length - 1);
		if (base.isServer)
		{
			SetFlag(Flags.Busy, b: true);
			SendNetworkUpdateImmediate();
			((FacepunchBehaviour)this).CancelInvoke((Action)ResetLift);
		}
	}

	private void SnapToFloor(int floor)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		floorIndex = Mathf.Clamp(floor, 0, stops.Length - 1);
		ProceduralLiftStop proceduralLiftStop = stops[floorIndex];
		((Component)cabin).get_transform().set_position(((Component)proceduralLiftStop).get_transform().get_position());
		if (base.isServer)
		{
			SetFlag(Flags.Busy, b: false);
			SendNetworkUpdateImmediate();
			((FacepunchBehaviour)this).CancelInvoke((Action)ResetLift);
		}
	}

	private void OnFinishedMoving()
	{
		if (base.isServer)
		{
			SetFlag(Flags.Busy, b: false);
			SendNetworkUpdateImmediate();
			if (floorIndex != 0)
			{
				((FacepunchBehaviour)this).Invoke((Action)ResetLift, resetDelay);
			}
		}
	}

	protected void Update()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		if (floorIndex < 0 || floorIndex > stops.Length - 1)
		{
			return;
		}
		ProceduralLiftStop proceduralLiftStop = stops[floorIndex];
		if (!(((Component)cabin).get_transform().get_position() == ((Component)proceduralLiftStop).get_transform().get_position()))
		{
			((Component)cabin).get_transform().set_position(Vector3.MoveTowards(((Component)cabin).get_transform().get_position(), ((Component)proceduralLiftStop).get_transform().get_position(), movementSpeed * Time.get_deltaTime()));
			if (((Component)cabin).get_transform().get_position() == ((Component)proceduralLiftStop).get_transform().get_position())
			{
				OnFinishedMoving();
			}
		}
	}
}
