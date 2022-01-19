using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class SearchLight : IOEntity
{
	public static class SearchLightFlags
	{
		public const Flags PlayerUsing = Flags.Reserved5;
	}

	public GameObject pitchObject;

	public GameObject yawObject;

	public GameObject eyePoint;

	public SoundPlayer turnLoop;

	public bool needsBuildingPrivilegeToUse = true;

	private Vector3 aimDir = Vector3.get_zero();

	private BasePlayer mountedPlayer;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("SearchLight.OnRpcMessage", 0);
		try
		{
			if (rpc == 3611615802u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_UseLight "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_UseLight", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(3611615802u, "RPC_UseLight", this, player, 3f))
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
							RPC_UseLight(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_UseLight");
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

	public override void ResetState()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		aimDir = Vector3.get_zero();
	}

	public override int ConsumptionAmount()
	{
		return 10;
	}

	public bool IsMounted()
	{
		return (Object)(object)mountedPlayer != (Object)null;
	}

	public override void Save(SaveInfo info)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		info.msg.autoturret = Pool.Get<AutoTurret>();
		info.msg.autoturret.aimDir = aimDir;
	}

	public override void Load(LoadInfo info)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.autoturret != null)
		{
			aimDir = info.msg.autoturret.aimDir;
		}
	}

	public void PlayerEnter(BasePlayer player)
	{
		if (!IsMounted() || !((Object)(object)player != (Object)(object)mountedPlayer))
		{
			PlayerExit();
			if ((Object)(object)player != (Object)null)
			{
				mountedPlayer = player;
				SetFlag(Flags.Reserved5, b: true);
			}
		}
	}

	public void PlayerExit()
	{
		if (Object.op_Implicit((Object)(object)mountedPlayer))
		{
			mountedPlayer = null;
		}
		SetFlag(Flags.Reserved5, b: false);
	}

	public void MountedUpdate()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)mountedPlayer == (Object)null || mountedPlayer.IsSleeping() || !mountedPlayer.IsAlive() || mountedPlayer.IsWounded() || Vector3.Distance(((Component)mountedPlayer).get_transform().get_position(), ((Component)this).get_transform().get_position()) > 2f)
		{
			PlayerExit();
			return;
		}
		Vector3 targetAimpoint = eyePoint.get_transform().get_position() + mountedPlayer.eyes.BodyForward() * 100f;
		SetTargetAimpoint(targetAimpoint);
		SendNetworkUpdate();
	}

	public void SetTargetAimpoint(Vector3 worldPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = worldPos - eyePoint.get_transform().get_position();
		aimDir = ((Vector3)(ref val)).get_normalized();
	}

	public override int GetCurrentEnergy()
	{
		if (currentEnergy >= ConsumptionAmount())
		{
			return base.GetCurrentEnergy();
		}
		return Mathf.Clamp(currentEnergy - base.ConsumptionAmount(), 0, currentEnergy);
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void RPC_UseLight(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		bool flag = msg.read.Bit();
		if ((!flag || !IsMounted()) && (!needsBuildingPrivilegeToUse || msg.player.CanBuild()))
		{
			if (flag)
			{
				PlayerEnter(player);
			}
			else
			{
				PlayerExit();
			}
		}
	}

	public override void OnKilled(HitInfo info)
	{
		SetFlag(Flags.On, b: false);
		base.OnKilled(info);
	}

	public void Update()
	{
		if (base.isServer && IsMounted())
		{
			MountedUpdate();
		}
	}
}
