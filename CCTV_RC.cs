using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class CCTV_RC : PoweredRemoteControlEntity
{
	public Transform pivotOrigin;

	public Transform yaw;

	public Transform pitch;

	public Vector2 pitchClamp = new Vector2(-50f, 50f);

	public Vector2 yawClamp = new Vector2(-50f, 50f);

	public float turnSpeed = 25f;

	public float serverLerpSpeed = 15f;

	public float clientLerpSpeed = 10f;

	private float pitchAmount;

	private float yawAmount;

	public bool hasPTZ = true;

	public const Flags Flag_HasViewer = Flags.Reserved5;

	private int numViewers;

	private bool externalViewer;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("CCTV_RC.OnRpcMessage", 0);
		try
		{
			if (rpc == 3353964129u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Server_SetDir "));
				}
				TimeWarning val2 = TimeWarning.New("Server_SetDir", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						RPCMessage rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.get_read();
						RPCMessage msg2 = rPCMessage;
						Server_SetDir(msg2);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
					player.Kick("RPC Error in Server_SetDir");
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

	public override int ConsumptionAmount()
	{
		return 5;
	}

	public override void ServerInit()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		if (!base.isClient && IsStatic())
		{
			pitchAmount = pitch.get_localEulerAngles().x;
			yawAmount = yaw.get_localEulerAngles().y;
			UpdateRCAccess(isOnline: true);
		}
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		UpdateRotation(10000f);
	}

	public override void UserInput(InputState inputState, BasePlayer player)
	{
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		if (hasPTZ)
		{
			float num = 1f;
			float num2 = Mathf.Clamp(0f - inputState.current.mouseDelta.y, -1f, 1f);
			float num3 = Mathf.Clamp(inputState.current.mouseDelta.x, -1f, 1f);
			pitchAmount = Mathf.Clamp(pitchAmount + num2 * num * turnSpeed, pitchClamp.x, pitchClamp.y);
			yawAmount = Mathf.Clamp(yawAmount + num3 * num * turnSpeed, yawClamp.x, yawClamp.y);
			Quaternion localRotation = Quaternion.Euler(pitchAmount, 0f, 0f);
			Quaternion localRotation2 = Quaternion.Euler(0f, yawAmount, 0f);
			((Component)pitch).get_transform().set_localRotation(localRotation);
			((Component)yaw).get_transform().set_localRotation(localRotation2);
			if (num2 != 0f || num3 != 0f)
			{
				SendNetworkUpdate();
			}
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.rcEntity.aim.x = pitchAmount;
		info.msg.rcEntity.aim.y = yawAmount;
		info.msg.rcEntity.aim.z = 0f;
	}

	[RPC_Server]
	public void Server_SetDir(RPCMessage msg)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		if (!IsStatic())
		{
			BasePlayer player = msg.player;
			if (player.CanBuild() && player.IsBuildingAuthed())
			{
				Vector3 val = Vector3Ex.Direction(player.eyes.position, ((Component)yaw).get_transform().get_position());
				val = ((Component)this).get_transform().InverseTransformDirection(val);
				Quaternion val2 = Quaternion.LookRotation(val);
				Vector3 val3 = BaseMountable.ConvertVector(((Quaternion)(ref val2)).get_eulerAngles());
				pitchAmount = val3.x;
				yawAmount = val3.y;
				pitchAmount = Mathf.Clamp(pitchAmount, pitchClamp.x, pitchClamp.y);
				yawAmount = Mathf.Clamp(yawAmount, yawClamp.x, yawClamp.y);
				Quaternion localRotation = Quaternion.Euler(pitchAmount, 0f, 0f);
				Quaternion localRotation2 = Quaternion.Euler(0f, yawAmount, 0f);
				((Component)pitch).get_transform().set_localRotation(localRotation);
				((Component)yaw).get_transform().set_localRotation(localRotation2);
				SendNetworkUpdate();
			}
		}
	}

	public override void InitializeControl(BasePlayer controller)
	{
		base.InitializeControl(controller);
		numViewers++;
		UpdateViewers();
	}

	public override void StopControl()
	{
		base.StopControl();
		numViewers--;
		UpdateViewers();
	}

	public void PingFromExternalViewer()
	{
		((FacepunchBehaviour)this).Invoke((Action)ResetExternalViewer, 10f);
		externalViewer = true;
		UpdateViewers();
	}

	private void ResetExternalViewer()
	{
		externalViewer = false;
		UpdateViewers();
	}

	public void UpdateViewers()
	{
		SetFlag(Flags.Reserved5, externalViewer || numViewers > 0);
	}

	public void UpdateRotation(float delta)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		Quaternion val = Quaternion.Euler(pitchAmount, 0f, 0f);
		Quaternion val2 = Quaternion.Euler(0f, yawAmount, 0f);
		float num = delta * (base.isServer ? serverLerpSpeed : clientLerpSpeed);
		((Component)pitch).get_transform().set_localRotation(Quaternion.Lerp(((Component)pitch).get_transform().get_localRotation(), val, num));
		((Component)yaw).get_transform().set_localRotation(Quaternion.Lerp(((Component)yaw).get_transform().get_localRotation(), val2, num));
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.rcEntity != null)
		{
			pitchAmount = info.msg.rcEntity.aim.x;
			yawAmount = info.msg.rcEntity.aim.y;
		}
	}
}
