using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class DudTimedExplosive : TimedExplosive, IIgniteable, ISplashable
{
	public GameObjectRef fizzleEffect;

	public GameObject wickSpark;

	public AudioSource wickSound;

	public float dudChance = 0.3f;

	[ItemSelector(ItemCategory.All)]
	public ItemDefinition itemToGive;

	[NonSerialized]
	private float explodeTime;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("DudTimedExplosive.OnRpcMessage", 0);
		try
		{
			if (rpc == 2436818324u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_Pickup "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_Pickup", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(2436818324u, "RPC_Pickup", this, player, 3f))
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
							RPC_Pickup(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_Pickup");
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

	private bool IsWickBurning()
	{
		return HasFlag(Flags.On);
	}

	public override float GetRandomTimerTime()
	{
		float randomTimerTime = base.GetRandomTimerTime();
		float num = 1f;
		if (Random.Range(0f, 1f) <= 0.15f)
		{
			num = 0.334f;
		}
		else if (Random.Range(0f, 1f) <= 0.15f)
		{
			num = 3f;
		}
		return randomTimerTime * num;
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void RPC_Pickup(RPCMessage msg)
	{
		if (!IsWickBurning())
		{
			BasePlayer player = msg.player;
			if (Random.Range(0f, 1f) >= 0.5f && HasParent())
			{
				SetFuse(Random.Range(2.5f, 3f));
				return;
			}
			player.GiveItem(ItemManager.Create(itemToGive, 1, skinID));
			Kill();
		}
	}

	public override void SetFuse(float fuseLength)
	{
		base.SetFuse(fuseLength);
		explodeTime = Time.get_realtimeSinceStartup() + fuseLength;
		SetFlag(Flags.On, b: true);
		SendNetworkUpdate();
		((FacepunchBehaviour)this).CancelInvoke((Action)base.KillMessage);
	}

	public override void Explode()
	{
		if ((Object)(object)creatorEntity != (Object)null && creatorEntity.IsNpc)
		{
			base.Explode();
		}
		else if (Random.Range(0f, 1f) < dudChance)
		{
			BecomeDud();
		}
		else
		{
			base.Explode();
		}
	}

	public override bool CanStickTo(BaseEntity entity)
	{
		if (base.CanStickTo(entity))
		{
			return IsWickBurning();
		}
		return false;
	}

	public virtual void BecomeDud()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Component)this).get_transform().get_position();
		Quaternion rotation = ((Component)this).get_transform().get_rotation();
		bool flag = false;
		EntityRef entityRef = parentEntity;
		while (entityRef.IsValid(base.isServer) && !flag)
		{
			BaseEntity baseEntity = entityRef.Get(base.isServer);
			if (baseEntity.syncPosition)
			{
				flag = true;
			}
			entityRef = baseEntity.parentEntity;
		}
		if (flag)
		{
			SetParent(null);
		}
		((Component)this).get_transform().SetPositionAndRotation(position, rotation);
		SetFlag(Flags.On, b: false);
		SetCollisionEnabled(wantsCollision: true);
		if (flag)
		{
			SetMotionEnabled(wantsMotion: true);
		}
		Effect.server.Run("assets/bundled/prefabs/fx/impacts/blunt/concrete/concrete1.prefab", this, 0u, Vector3.get_zero(), Vector3.get_zero());
		SendNetworkUpdate();
		((FacepunchBehaviour)this).CancelInvoke((Action)base.KillMessage);
		((FacepunchBehaviour)this).Invoke((Action)base.KillMessage, 1200f);
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.dudExplosive = Pool.Get<DudExplosive>();
		info.msg.dudExplosive.fuseTimeLeft = explodeTime - Time.get_realtimeSinceStartup();
	}

	public void Ignite()
	{
		SetFuse(GetRandomTimerTime());
		ReceiveCollisionMessages(b: true);
		if (waterCausesExplosion)
		{
			((FacepunchBehaviour)this).InvokeRepeating((Action)base.WaterCheck, 0f, 0.5f);
		}
	}

	public bool CanIgnite()
	{
		return !HasFlag(Flags.On);
	}

	public bool WantsSplash(ItemDefinition splashType, int amount)
	{
		if (!base.IsDestroyed)
		{
			return HasFlag(Flags.On);
		}
		return false;
	}

	public int DoSplash(ItemDefinition splashType, int amount)
	{
		BecomeDud();
		if (((FacepunchBehaviour)this).IsInvoking((Action)Explode))
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)Explode);
		}
		return 1;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.dudExplosive != null)
		{
			explodeTime = Time.get_realtimeSinceStartup() + info.msg.dudExplosive.fuseTimeLeft;
		}
	}
}
