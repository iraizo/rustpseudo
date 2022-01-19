using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class Landmine : BaseTrap
{
	public GameObjectRef explosionEffect;

	public GameObjectRef triggeredEffect;

	public float minExplosionRadius;

	public float explosionRadius;

	public bool blocked;

	private ulong triggerPlayerID;

	public List<DamageTypeEntry> damageTypes = new List<DamageTypeEntry>();

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("Landmine.OnRpcMessage", 0);
		try
		{
			if (rpc == 1552281787 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_Disarm "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_Disarm", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(1552281787u, "RPC_Disarm", this, player, 3f))
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
							RPC_Disarm(rpc2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_Disarm");
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

	public bool Triggered()
	{
		return HasFlag(Flags.Open);
	}

	public bool Armed()
	{
		return HasFlag(Flags.On);
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
		{
			info.msg.landmine = Pool.Get<Landmine>();
			info.msg.landmine.triggeredID = triggerPlayerID;
		}
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (!info.fromDisk && info.msg.landmine != null)
		{
			triggerPlayerID = info.msg.landmine.triggeredID;
		}
	}

	public override void ServerInit()
	{
		SetFlag(Flags.On, b: false);
		((FacepunchBehaviour)this).Invoke((Action)Arm, 1.5f);
		base.ServerInit();
	}

	public override void ObjectEntered(GameObject obj)
	{
		if (!base.isClient)
		{
			if (!Armed())
			{
				((FacepunchBehaviour)this).CancelInvoke((Action)Arm);
				blocked = true;
			}
			else
			{
				BasePlayer ply = obj.ToBaseEntity() as BasePlayer;
				Trigger(ply);
			}
		}
	}

	public void Trigger(BasePlayer ply = null)
	{
		if (Object.op_Implicit((Object)(object)ply))
		{
			triggerPlayerID = ply.userID;
		}
		SetFlag(Flags.Open, b: true);
		SendNetworkUpdate();
	}

	public override void OnEmpty()
	{
		if (blocked)
		{
			Arm();
			blocked = false;
		}
		else if (Triggered())
		{
			((FacepunchBehaviour)this).Invoke((Action)TryExplode, 0.05f);
		}
	}

	public virtual void Explode()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		base.health = float.PositiveInfinity;
		Effect.server.Run(explosionEffect.resourcePath, PivotPoint(), ((Component)this).get_transform().get_up(), null, broadcast: true);
		DamageUtil.RadiusDamage(this, LookupPrefab(), CenterPoint(), minExplosionRadius, explosionRadius, damageTypes, 2263296, useLineOfSight: true);
		if (!base.IsDestroyed)
		{
			Kill();
		}
	}

	public override void OnKilled(HitInfo info)
	{
		((FacepunchBehaviour)this).Invoke((Action)Explode, Random.Range(0.1f, 0.3f));
	}

	private void OnGroundMissing()
	{
		Explode();
	}

	private void TryExplode()
	{
		if (Armed())
		{
			Explode();
		}
	}

	public override void Arm()
	{
		SetFlag(Flags.On, b: true);
		SendNetworkUpdate();
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	private void RPC_Disarm(RPCMessage rpc)
	{
		if (rpc.player.net.ID != triggerPlayerID && Armed())
		{
			SetFlag(Flags.On, b: false);
			if (Random.Range(0, 100) < 15)
			{
				((FacepunchBehaviour)this).Invoke((Action)TryExplode, 0.05f);
				return;
			}
			rpc.player.GiveItem(ItemManager.CreateByName("trap.landmine", 1, 0uL), GiveItemReason.PickedUp);
			Kill();
		}
	}
}
