using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class BearTrap : BaseTrap
{
	protected Animator animator;

	private GameObject hurtTarget;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("BearTrap.OnRpcMessage", 0);
		try
		{
			if (rpc == 547827602 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_Arm "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_Arm", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(547827602u, "RPC_Arm", this, player, 3f))
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
							RPC_Arm(rpc2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_Arm");
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

	public bool Armed()
	{
		return HasFlag(Flags.On);
	}

	public override void InitShared()
	{
		animator = ((Component)this).GetComponent<Animator>();
		base.InitShared();
	}

	public override bool CanPickup(BasePlayer player)
	{
		if (base.CanPickup(player) && !Armed())
		{
			return player.CanBuild();
		}
		return false;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		Arm();
	}

	public override void Arm()
	{
		base.Arm();
		RadialResetCorpses(120f);
	}

	public void Fire()
	{
		SetFlag(Flags.On, b: false);
		SendNetworkUpdate();
	}

	public override void ObjectEntered(GameObject obj)
	{
		if (Armed())
		{
			hurtTarget = obj;
			((FacepunchBehaviour)this).Invoke((Action)DelayedFire, 0.05f);
		}
	}

	public void DelayedFire()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)hurtTarget))
		{
			BaseEntity baseEntity = hurtTarget.ToBaseEntity();
			if ((Object)(object)baseEntity != (Object)null)
			{
				HitInfo hitInfo = new HitInfo(this, baseEntity, DamageType.Bite, 50f, ((Component)this).get_transform().get_position());
				hitInfo.damageTypes.Add(DamageType.Stab, 30f);
				baseEntity.OnAttacked(hitInfo);
			}
			hurtTarget = null;
		}
		RadialResetCorpses(1800f);
		Fire();
		Hurt(25f);
	}

	public void RadialResetCorpses(float duration)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		List<BaseCorpse> list = Pool.GetList<BaseCorpse>();
		Vis.Entities(((Component)this).get_transform().get_position(), 5f, list, 512, (QueryTriggerInteraction)2);
		foreach (BaseCorpse item in list)
		{
			item.ResetRemovalTime(duration);
		}
		Pool.FreeList<BaseCorpse>(ref list);
	}

	public override void OnAttacked(HitInfo info)
	{
		float num = info.damageTypes.Total();
		if ((info.damageTypes.IsMeleeType() && num > 20f) || num > 30f)
		{
			Fire();
		}
		base.OnAttacked(info);
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	private void RPC_Arm(RPCMessage rpc)
	{
		if (!Armed())
		{
			Arm();
		}
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (!base.isServer && animator.get_isInitialized())
		{
			animator.SetBool("armed", Armed());
		}
	}
}
