using System;
using ConVar;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class HackableLockedCrate : LootContainer
{
	public const Flags Flag_Hacking = Flags.Reserved1;

	public const Flags Flag_FullyHacked = Flags.Reserved2;

	public Text timerText;

	[ServerVar(Help = "How many seconds for the crate to unlock")]
	public static float requiredHackSeconds = 900f;

	[ServerVar(Help = "How many seconds until the crate is destroyed without any hack attempts")]
	public static float decaySeconds = 7200f;

	public SoundPlayer hackProgressBeep;

	private float hackSeconds;

	public GameObjectRef shockEffect;

	public GameObjectRef mapMarkerEntityPrefab;

	public GameObjectRef landEffect;

	public bool shouldDecay = true;

	private BaseEntity mapMarkerInstance;

	private bool hasLanded;

	private bool wasDropped;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("HackableLockedCrate.OnRpcMessage", 0);
		try
		{
			if (rpc == 888500940 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_Hack "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_Hack", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(888500940u, "RPC_Hack", this, player, 3f))
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
							RPC_Hack(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_Hack");
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

	public bool IsBeingHacked()
	{
		return HasFlag(Flags.Reserved1);
	}

	public bool IsFullyHacked()
	{
		return HasFlag(Flags.Reserved2);
	}

	public override void DestroyShared()
	{
		if (base.isServer && Object.op_Implicit((Object)(object)mapMarkerInstance))
		{
			mapMarkerInstance.Kill();
		}
		base.DestroyShared();
	}

	public void CreateMapMarker(float durationMinutes)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)mapMarkerInstance))
		{
			mapMarkerInstance.Kill();
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(mapMarkerEntityPrefab.resourcePath, ((Component)this).get_transform().get_position(), Quaternion.get_identity());
		baseEntity.Spawn();
		baseEntity.SetParent(this);
		((Component)baseEntity).get_transform().set_localPosition(Vector3.get_zero());
		baseEntity.SendNetworkUpdate();
		mapMarkerInstance = baseEntity;
	}

	public void RefreshDecay()
	{
		((FacepunchBehaviour)this).CancelInvoke((Action)DelayedDestroy);
		if (shouldDecay)
		{
			((FacepunchBehaviour)this).Invoke((Action)DelayedDestroy, decaySeconds);
		}
	}

	public void DelayedDestroy()
	{
		Kill();
	}

	public override void OnAttacked(HitInfo info)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (base.isServer)
		{
			if (StringPool.Get(info.HitBone) == "laptopcollision")
			{
				Effect.server.Run(shockEffect.resourcePath, info.HitPositionWorld, Vector3.get_up());
				hackSeconds -= 8f * (info.damageTypes.Total() / 50f);
				if (hackSeconds < 0f)
				{
					hackSeconds = 0f;
				}
			}
			RefreshDecay();
		}
		base.OnAttacked(info);
	}

	public void SetWasDropped()
	{
		wasDropped = true;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (base.isClient)
		{
			return;
		}
		if (!Application.isLoadingSave)
		{
			SetFlag(Flags.Reserved1, b: false);
			SetFlag(Flags.Reserved2, b: false);
			if (wasDropped)
			{
				((FacepunchBehaviour)this).InvokeRepeating((Action)LandCheck, 0f, 0.015f);
			}
		}
		RefreshDecay();
		isLootable = IsFullyHacked();
		CreateMapMarker(120f);
	}

	public void LandCheck()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		RaycastHit val = default(RaycastHit);
		if (!hasLanded && Physics.Raycast(new Ray(((Component)this).get_transform().get_position() + Vector3.get_up() * 0.5f, Vector3.get_down()), ref val, 1f, 1218511105))
		{
			Effect.server.Run(landEffect.resourcePath, ((RaycastHit)(ref val)).get_point(), Vector3.get_up());
			hasLanded = true;
			((FacepunchBehaviour)this).CancelInvoke((Action)LandCheck);
		}
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		SetFlag(Flags.Reserved1, b: false);
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_Hack(RPCMessage msg)
	{
		if (!IsBeingHacked())
		{
			StartHacking();
		}
	}

	public void StartHacking()
	{
		BroadcastEntityMessage("HackingStarted", 20f, 256);
		SetFlag(Flags.Reserved1, b: true);
		((FacepunchBehaviour)this).InvokeRepeating((Action)HackProgress, 1f, 1f);
		ClientRPC(null, "UpdateHackProgress", 0, (int)requiredHackSeconds);
		RefreshDecay();
	}

	public void HackProgress()
	{
		hackSeconds += 1f;
		if (hackSeconds > requiredHackSeconds)
		{
			RefreshDecay();
			SetFlag(Flags.Reserved2, b: true);
			isLootable = true;
			((FacepunchBehaviour)this).CancelInvoke((Action)HackProgress);
		}
		ClientRPC(null, "UpdateHackProgress", (int)hackSeconds, (int)requiredHackSeconds);
	}
}
