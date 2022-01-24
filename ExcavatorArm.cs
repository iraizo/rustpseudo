using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class ExcavatorArm : BaseEntity
{
	public float yaw1;

	public float yaw2;

	public Transform wheel;

	public float wheelSpeed = 2f;

	public float turnSpeed = 0.1f;

	public Transform miningOffset;

	public GameObjectRef bounceEffect;

	public LightGroupAtTime lights;

	public Material conveyorMaterial;

	public float beltSpeedMax = 0.1f;

	public const Flags Flag_HasPower = Flags.Reserved8;

	public List<ExcavatorOutputPile> outputPiles;

	public SoundDefinition miningStartButtonSoundDef;

	[Header("Production")]
	public ItemAmount[] resourcesToMine;

	public float resourceProductionTickRate = 3f;

	public float timeForFullResources = 120f;

	private ItemAmount[] pendingResources;

	private float movedAmount;

	private float currentTurnThrottle;

	private float lastMoveYaw;

	private float excavatorStartTime;

	private int resourceMiningIndex;

	protected override float PositionTickRate => 0.05f;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("ExcavatorArm.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 2059417170 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_SetResourceTarget "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_SetResourceTarget", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(2059417170u, "RPC_SetResourceTarget", this, player, 3f))
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
							rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							RPCMessage msg2 = rPCMessage;
							RPC_SetResourceTarget(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_SetResourceTarget");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2882020740u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_StopMining "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_StopMining", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(2882020740u, "RPC_StopMining", this, player, 3f))
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
							rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							RPCMessage msg3 = rPCMessage;
							RPC_StopMining(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_StopMining");
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

	public bool IsPowered()
	{
		return HasFlag(Flags.Reserved8);
	}

	public bool IsMining()
	{
		return IsOn();
	}

	public override float GetNetworkTime()
	{
		return Time.get_fixedTime();
	}

	public void FixedUpdate()
	{
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		if (!base.isClient)
		{
			bool flag = IsMining() && IsPowered();
			float num = (flag ? 1f : 0f);
			currentTurnThrottle = Mathf.Lerp(currentTurnThrottle, num, Time.get_fixedDeltaTime() * (flag ? 0.333f : 1f));
			if (Mathf.Abs(num - currentTurnThrottle) < 0.025f)
			{
				currentTurnThrottle = num;
			}
			movedAmount += Time.get_fixedDeltaTime() * turnSpeed * currentTurnThrottle;
			float num2 = (Mathf.Sin(movedAmount) + 1f) / 2f;
			float num3 = Mathf.Lerp(yaw1, yaw2, num2);
			if (num3 != lastMoveYaw)
			{
				lastMoveYaw = num3;
				((Component)this).get_transform().set_rotation(Quaternion.Euler(0f, num3, 0f));
				((Component)this).get_transform().set_hasChanged(true);
			}
		}
	}

	public void BeginMining()
	{
		if (IsPowered())
		{
			SetFlag(Flags.On, b: true);
			((FacepunchBehaviour)this).InvokeRepeating((Action)ProduceResources, resourceProductionTickRate, resourceProductionTickRate);
			ExcavatorServerEffects.SetMining(isMining: true);
			Analytics.ExcavatorStarted();
			excavatorStartTime = GetNetworkTime();
		}
	}

	public void StopMining()
	{
		ExcavatorServerEffects.SetMining(isMining: false);
		((FacepunchBehaviour)this).CancelInvoke((Action)ProduceResources);
		if (HasFlag(Flags.On))
		{
			Analytics.ExcavatorStopped(GetNetworkTime() - excavatorStartTime);
		}
		SetFlag(Flags.On, b: false);
	}

	public void ProduceResources()
	{
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		float num = resourceProductionTickRate / timeForFullResources;
		float num2 = resourcesToMine[resourceMiningIndex].amount * num;
		pendingResources[resourceMiningIndex].amount += num2;
		ItemAmount[] array = pendingResources;
		foreach (ItemAmount itemAmount in array)
		{
			if (!(itemAmount.amount >= (float)outputPiles.Count))
			{
				continue;
			}
			int num3 = Mathf.FloorToInt(itemAmount.amount / (float)outputPiles.Count);
			itemAmount.amount -= num3 * 2;
			foreach (ExcavatorOutputPile outputPile in outputPiles)
			{
				Item item = ItemManager.Create(resourcesToMine[resourceMiningIndex].itemDef, num3, 0uL);
				if (!item.MoveToContainer(outputPile.inventory))
				{
					item.Drop(outputPile.GetDropPosition(), outputPile.GetDropVelocity());
				}
			}
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
			StopMining();
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void RPC_SetResourceTarget(RPCMessage msg)
	{
		switch (msg.read.String(256))
		{
		case "HQM":
			resourceMiningIndex = 0;
			break;
		case "Sulfur":
			resourceMiningIndex = 1;
			break;
		case "Stone":
			resourceMiningIndex = 2;
			break;
		case "Metal":
			resourceMiningIndex = 3;
			break;
		}
		if (!IsOn())
		{
			BeginMining();
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void RPC_StopMining(RPCMessage msg)
	{
	}

	public override void Spawn()
	{
		base.Spawn();
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		Init();
		if (IsOn() && IsPowered())
		{
			BeginMining();
		}
		else
		{
			StopMining();
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity = Pool.Get<IOEntity>();
		info.msg.ioEntity.genericFloat1 = movedAmount;
		info.msg.ioEntity.genericInt1 = resourceMiningIndex;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			movedAmount = info.msg.ioEntity.genericFloat1;
			resourceMiningIndex = info.msg.ioEntity.genericInt1;
		}
	}

	public override void PostMapEntitySpawn()
	{
		base.PostMapEntitySpawn();
		Init();
	}

	public void Init()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		pendingResources = new ItemAmount[resourcesToMine.Length];
		for (int i = 0; i < resourcesToMine.Length; i++)
		{
			pendingResources[i] = new ItemAmount(resourcesToMine[i].itemDef);
		}
		List<ExcavatorOutputPile> list = Pool.GetList<ExcavatorOutputPile>();
		Vis.Entities(((Component)this).get_transform().get_position(), 200f, list, 512, (QueryTriggerInteraction)2);
		outputPiles = new List<ExcavatorOutputPile>();
		foreach (ExcavatorOutputPile item in list)
		{
			if (!item.isClient)
			{
				outputPiles.Add(item);
			}
		}
		Pool.FreeList<ExcavatorOutputPile>(ref list);
	}
}
