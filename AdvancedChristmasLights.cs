using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class AdvancedChristmasLights : IOEntity
{
	public struct pointEntry
	{
		public Vector3 point;

		public Vector3 normal;
	}

	public enum AnimationType
	{
		ON = 1,
		FLASHING = 2,
		CHASING = 3,
		FADE = 4,
		SLOWGLOW = 6
	}

	public GameObjectRef bulbPrefab;

	public LineRenderer lineRenderer;

	public List<pointEntry> points = new List<pointEntry>();

	public List<BaseBulb> bulbs = new List<BaseBulb>();

	public float bulbSpacing = 0.25f;

	public float wireThickness = 0.02f;

	public Transform wireEmission;

	public AnimationType animationStyle = AnimationType.ON;

	public RendererLOD _lod;

	[Tooltip("This many units used will result in +1 power usage")]
	public float lengthToPowerRatio = 5f;

	private bool finalized;

	private int lengthUsed;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("AdvancedChristmasLights.OnRpcMessage", 0);
		try
		{
			if (rpc == 1435781224 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SetAnimationStyle "));
				}
				TimeWarning val2 = TimeWarning.New("SetAnimationStyle", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1435781224u, "SetAnimationStyle", this, player, 3f))
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
							RPCMessage rPCMessage2 = rPCMessage;
							SetAnimationStyle(rPCMessage2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SetAnimationStyle");
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

	public void ClearPoints()
	{
		points.Clear();
	}

	public void FinishEditing()
	{
		finalized = true;
	}

	public bool IsFinalized()
	{
		return finalized;
	}

	public void AddPoint(Vector3 newPoint, Vector3 newNormal)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (base.isServer && points.Count == 0)
		{
			newPoint = wireEmission.get_position();
		}
		pointEntry item = default(pointEntry);
		item.point = newPoint;
		item.normal = newNormal;
		points.Add(item);
		if (base.isServer)
		{
			SendNetworkUpdate();
		}
	}

	public override int ConsumptionAmount()
	{
		return 5;
	}

	protected override int GetPickupCount()
	{
		return Mathf.Max(lengthUsed, 1);
	}

	public void AddLengthUsed(int addLength)
	{
		lengthUsed += addLength;
	}

	public override void ServerInit()
	{
		base.ServerInit();
	}

	public override void Save(SaveInfo info)
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		info.msg.lightString = Pool.Get<LightString>();
		info.msg.lightString.points = Pool.GetList<StringPoint>();
		info.msg.lightString.lengthUsed = lengthUsed;
		info.msg.lightString.animationStyle = (int)animationStyle;
		foreach (pointEntry point in points)
		{
			StringPoint val = Pool.Get<StringPoint>();
			val.point = point.point;
			val.normal = point.normal;
			info.msg.lightString.points.Add(val);
		}
	}

	public override void Load(LoadInfo info)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.lightString == null)
		{
			return;
		}
		ClearPoints();
		foreach (StringPoint point in info.msg.lightString.points)
		{
			AddPoint(point.point, point.normal);
		}
		lengthUsed = info.msg.lightString.lengthUsed;
		animationStyle = (AnimationType)info.msg.lightString.animationStyle;
		if (info.fromDisk)
		{
			FinishEditing();
		}
	}

	public bool IsStyle(AnimationType testType)
	{
		return testType == animationStyle;
	}

	public bool CanPlayerManipulate(BasePlayer player)
	{
		return true;
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void SetAnimationStyle(RPCMessage msg)
	{
		int num = msg.read.Int32();
		num = Mathf.Clamp(num, 1, 7);
		if (Global.developer > 0)
		{
			Debug.Log((object)("Set animation style to :" + num + " old was : " + (int)animationStyle));
		}
		AnimationType animationType = (AnimationType)num;
		if (animationType != animationStyle)
		{
			animationStyle = animationType;
			SendNetworkUpdate();
		}
	}
}
