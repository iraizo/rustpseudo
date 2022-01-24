using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class VehicleModuleTaxi : VehicleModuleStorage
{
	[Header("Taxi")]
	[SerializeField]
	private SoundDefinition kickButtonSound;

	[SerializeField]
	private SphereCollider kickButtonCollider;

	[SerializeField]
	private float maxKickVelocity = 4f;

	private Vector3 KickButtonPos => ((Component)kickButtonCollider).get_transform().get_position() + ((Component)kickButtonCollider).get_transform().get_rotation() * kickButtonCollider.get_center();

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("VehicleModuleTaxi.OnRpcMessage", 0);
		try
		{
			if (rpc == 2714639811u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_KickPassengers "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_KickPassengers", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(2714639811u, "RPC_KickPassengers", this, player, 3f))
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
							RPC_KickPassengers(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_KickPassengers");
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

	private bool CanKickPassengers(BasePlayer player)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		if (!base.IsOnAVehicle)
		{
			return false;
		}
		if (base.Vehicle.GetSpeed() > maxKickVelocity)
		{
			return false;
		}
		if ((Object)(object)player == (Object)null)
		{
			return false;
		}
		if (!base.Vehicle.PlayerIsMounted(player))
		{
			return false;
		}
		Vector3 val = KickButtonPos - ((Component)player).get_transform().get_position();
		if (Vector3.Dot(val, ((Component)player).get_transform().get_forward()) < 0f)
		{
			return ((Vector3)(ref val)).get_sqrMagnitude() < 4f;
		}
		return false;
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void RPC_KickPassengers(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!((Object)(object)player == (Object)null) && CanKickPassengers(player))
		{
			KickPassengers();
		}
	}

	private void KickPassengers()
	{
		if (!base.IsOnAVehicle)
		{
			return;
		}
		foreach (MountPointInfo mountPoint in base.Vehicle.mountPoints)
		{
			if (ModuleHasMountPoint(mountPoint))
			{
				BaseMountable mountable = mountPoint.mountable;
				BasePlayer mounted = mountable.GetMounted();
				if ((Object)(object)mounted != (Object)null && mountable.HasValidDismountPosition(mounted))
				{
					mountable.AttemptDismount(mounted);
				}
			}
		}
	}
}
