using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class ExcavatorSignalComputer : BaseCombatEntity
{
	public float chargePower;

	public const Flags Flag_Ready = Flags.Reserved7;

	public const Flags Flag_HasPower = Flags.Reserved8;

	public GameObjectRef supplyPlanePrefab;

	public Transform[] dropPoints;

	public Text statusText;

	public Text timerText;

	public static readonly Phrase readyphrase = new Phrase("excavator.signal.ready", "READY");

	public static readonly Phrase chargephrase = new Phrase("excavator.signal.charging", "COMSYS CHARGING");

	[ServerVar]
	public static float chargeNeededForSupplies = 600f;

	private float lastChargeTime;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("ExcavatorSignalComputer.OnRpcMessage", 0);
		try
		{
			if (rpc == 1824723998 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RequestSupplies "));
				}
				TimeWarning val2 = TimeWarning.New("RequestSupplies", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(1824723998u, "RequestSupplies", this, player, 5uL))
						{
							return true;
						}
						if (!RPC_Server.IsVisible.Test(1824723998u, "RequestSupplies", this, player, 3f))
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
							RequestSupplies(rpc2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RequestSupplies");
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

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity = Pool.Get<IOEntity>();
		info.msg.ioEntity.genericFloat1 = chargePower;
		info.msg.ioEntity.genericFloat2 = chargeNeededForSupplies;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		lastChargeTime = Time.get_time();
		((FacepunchBehaviour)this).InvokeRepeating((Action)ChargeThink, 0f, 1f);
	}

	public override void PostServerLoad()
	{
		SetFlag(Flags.Reserved8, b: false);
		SetFlag(Flags.Reserved7, b: false);
	}

	public void ChargeThink()
	{
		float num = chargePower;
		float num2 = Time.get_time() - lastChargeTime;
		lastChargeTime = Time.get_time();
		if (IsPowered())
		{
			chargePower += num2;
		}
		chargePower = Mathf.Clamp(chargePower, 0f, chargeNeededForSupplies);
		SetFlag(Flags.Reserved7, chargePower >= chargeNeededForSupplies);
		if (num != chargePower)
		{
			SendNetworkUpdate();
		}
	}

	public override void OnEntityMessage(BaseEntity from, string msg)
	{
		base.OnEntityMessage(from, msg);
		if (msg == "DieselEngineOn")
		{
			SetFlag(Flags.Reserved8, b: true);
		}
		else if (msg == "DieselEngineOff")
		{
			SetFlag(Flags.Reserved8, b: false);
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	[RPC_Server.CallsPerSecond(5uL)]
	public void RequestSupplies(RPCMessage rpc)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		if (HasFlag(Flags.Reserved7) && IsPowered() && chargePower >= chargeNeededForSupplies)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(supplyPlanePrefab.resourcePath);
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				Vector3 position = dropPoints[Random.Range(0, dropPoints.Length)].get_position();
				Vector3 val = default(Vector3);
				((Vector3)(ref val))._002Ector(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));
				((Component)baseEntity).SendMessage("InitDropPosition", (object)(position + val), (SendMessageOptions)1);
				baseEntity.Spawn();
			}
			chargePower -= chargeNeededForSupplies;
			SetFlag(Flags.Reserved7, b: false);
			SendNetworkUpdate();
		}
	}

	public bool IsPowered()
	{
		return HasFlag(Flags.Reserved8);
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			chargePower = info.msg.ioEntity.genericFloat1;
		}
	}
}
