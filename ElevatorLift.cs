using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class ElevatorLift : BaseCombatEntity
{
	public GameObject DescendingHurtTrigger;

	public GameObject MovementCollider;

	public Transform UpButtonPoint;

	public Transform DownButtonPoint;

	public TriggerNotify VehicleTrigger;

	public GameObjectRef LiftArrivalScreenBounce;

	public SoundDefinition liftMovementLoopDef;

	public SoundDefinition liftMovementStartDef;

	public SoundDefinition liftMovementStopDef;

	public SoundDefinition liftMovementAccentSoundDef;

	public GameObjectRef liftButtonPressedEffect;

	public float movementAccentMinInterval = 0.75f;

	public float movementAccentMaxInterval = 3f;

	private Sound liftMovementLoopSound;

	private float nextMovementAccent;

	private const Flags PressedUp = Flags.Reserved1;

	private const Flags PressedDown = Flags.Reserved2;

	private Elevator owner => GetParentEntity() as Elevator;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("ElevatorLift.OnRpcMessage", 0);
		try
		{
			if (rpc == 4061236510u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Server_RaiseLowerFloor "));
				}
				TimeWarning val2 = TimeWarning.New("Server_RaiseLowerFloor", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(4061236510u, "Server_RaiseLowerFloor", this, player, 3f))
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
							Server_RaiseLowerFloor(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Server_RaiseLowerFloor");
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

	public override void ServerInit()
	{
		base.ServerInit();
		ToggleHurtTrigger(state: false);
	}

	public void ToggleHurtTrigger(bool state)
	{
		if ((Object)(object)DescendingHurtTrigger != (Object)null)
		{
			DescendingHurtTrigger.SetActive(state);
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void Server_RaiseLowerFloor(RPCMessage msg)
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		if (CanMove())
		{
			Elevator.Direction direction = (Elevator.Direction)msg.read.Int32();
			bool goTopBottom = msg.read.Bit();
			SetFlag((direction == Elevator.Direction.Up) ? Flags.Reserved1 : Flags.Reserved2, b: true);
			owner.Server_RaiseLowerElevator(direction, goTopBottom);
			((FacepunchBehaviour)this).Invoke((Action)ClearDirection, 0.7f);
			if (liftButtonPressedEffect.isValid)
			{
				Effect.server.Run(liftButtonPressedEffect.resourcePath, ((Component)this).get_transform().get_position(), Vector3.get_up());
			}
		}
	}

	private void ClearDirection()
	{
		SetFlag(Flags.Reserved1, b: false);
		SetFlag(Flags.Reserved2, b: false);
	}

	public override void Hurt(HitInfo info)
	{
		BaseCombatEntity baseCombatEntity;
		if (HasParent() && (baseCombatEntity = GetParentEntity() as BaseCombatEntity) != null)
		{
			baseCombatEntity.Hurt(info);
		}
	}

	public override void AdminKill()
	{
		if (HasParent())
		{
			GetParentEntity().AdminKill();
		}
		else
		{
			base.AdminKill();
		}
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		ClearDirection();
	}

	public bool CanMove()
	{
		return !VehicleTrigger.HasContents;
	}

	public void ToggleMovementCollider(bool state)
	{
		if ((Object)(object)MovementCollider != (Object)null)
		{
			MovementCollider.SetActive(state);
		}
	}
}
