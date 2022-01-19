using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

public class BaseMountable : BaseCombatEntity
{
	public enum MountStatType
	{
		None,
		Boating,
		Flying,
		Driving
	}

	public enum MountGestureType
	{
		None,
		UpperBody
	}

	[Header("View")]
	[FormerlySerializedAs("eyeOverride")]
	public Transform eyePositionOverride;

	[FormerlySerializedAs("eyeOverride")]
	public Transform eyeCenterOverride;

	public Vector2 pitchClamp = new Vector2(-80f, 50f);

	public Vector2 yawClamp = new Vector2(-80f, 80f);

	public bool canWieldItems = true;

	public bool relativeViewAngles = true;

	[Header("Mounting")]
	public Transform mountAnchor;

	public PlayerModel.MountPoses mountPose;

	public float maxMountDistance = 1.5f;

	public Transform[] dismountPositions;

	public bool checkPlayerLosOnMount;

	public bool disableMeshCullingForPlayers;

	public bool allowHeadLook;

	[FormerlySerializedAs("modifyPlayerCollider")]
	public bool modifiesPlayerCollider;

	public BasePlayer.CapsuleColliderInfo customPlayerCollider;

	public SoundDefinition mountSoundDef;

	public SoundDefinition swapSoundDef;

	public SoundDefinition dismountSoundDef;

	public MountStatType mountTimeStatType;

	public MountGestureType allowedGestures;

	public bool canDrinkWhileMounted = true;

	public bool allowSleeperMounting;

	[Help("Set this to true if the mountable is enclosed so it doesn't move inside cars and such")]
	public bool animateClothInLocalSpace = true;

	[Header("Camera")]
	public BasePlayer.CameraMode MountedCameraMode;

	[FormerlySerializedAs("needsVehicleTick")]
	public bool isMobile;

	public float SideLeanAmount = 0.2f;

	protected BasePlayer _mounted;

	public static ListHashSet<BaseMountable> FixedUpdateMountables = new ListHashSet<BaseMountable>(8);

	public const float playerHeight = 1.8f;

	public const float playerRadius = 0.5f;

	protected override float PositionTickRate => 0.05f;

	public virtual bool IsSummerDlcVehicle => false;

	public virtual bool BlocksDoors => true;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("BaseMountable.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 1735799362 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_WantsDismount "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_WantsDismount", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.get_read();
						RPCMessage msg2 = rPCMessage;
						RPC_WantsDismount(msg2);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
					player.Kick("RPC Error in RPC_WantsDismount");
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 4014300952u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_WantsMount "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_WantsMount", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(4014300952u, "RPC_WantsMount", this, player, 3f))
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
							RPC_WantsMount(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_WantsMount");
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

	public virtual bool CanHoldItems()
	{
		return canWieldItems;
	}

	public virtual BasePlayer.CameraMode GetMountedCameraMode()
	{
		return MountedCameraMode;
	}

	public virtual bool DirectlyMountable()
	{
		return true;
	}

	public virtual Transform GetEyeOverride()
	{
		if ((Object)(object)eyePositionOverride != (Object)null)
		{
			return eyePositionOverride;
		}
		return ((Component)this).get_transform();
	}

	public virtual Quaternion GetMountedBodyAngles()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return GetEyeOverride().get_rotation();
	}

	public virtual bool ModifiesThirdPersonCamera()
	{
		return false;
	}

	public virtual Vector2 GetPitchClamp()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return pitchClamp;
	}

	public virtual Vector2 GetYawClamp()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return yawClamp;
	}

	public virtual bool IsMounted()
	{
		return IsBusy();
	}

	public virtual Vector3 EyePositionForPlayer(BasePlayer player, Quaternion lookRot)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)player.GetMounted() != (Object)(object)this)
		{
			return Vector3.get_zero();
		}
		return ((Component)eyePositionOverride).get_transform().get_position();
	}

	public virtual Vector3 EyeCenterForPlayer(BasePlayer player, Quaternion lookRot)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)player.GetMounted() != (Object)(object)this)
		{
			return Vector3.get_zero();
		}
		return ((Component)eyeCenterOverride).get_transform().get_position();
	}

	public virtual float WaterFactorForPlayer(BasePlayer player)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		OBB val = player.WorldSpaceBounds();
		return WaterLevel.Factor(((OBB)(ref val)).ToBounds(), this);
	}

	public override float MaxVelocity()
	{
		BaseEntity baseEntity = GetParentEntity();
		if (Object.op_Implicit((Object)(object)baseEntity))
		{
			return baseEntity.MaxVelocity();
		}
		return base.MaxVelocity();
	}

	public virtual bool PlayerIsMounted(BasePlayer player)
	{
		if (player.IsValid())
		{
			return (Object)(object)player.GetMounted() == (Object)(object)this;
		}
		return false;
	}

	public virtual BaseVehicle VehicleParent()
	{
		return GetParentEntity() as BaseVehicle;
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		SetFlag(Flags.Busy, (Object)(object)_mounted != (Object)null);
	}

	public BasePlayer GetMounted()
	{
		return _mounted;
	}

	public virtual void MounteeTookDamage(BasePlayer mountee, HitInfo info)
	{
	}

	public virtual void LightToggle(BasePlayer player)
	{
	}

	public virtual bool CanSwapToThis(BasePlayer player)
	{
		return true;
	}

	public override bool CanPickup(BasePlayer player)
	{
		if (base.CanPickup(player))
		{
			return !IsMounted();
		}
		return false;
	}

	public override void OnKilled(HitInfo info)
	{
		DismountAllPlayers();
		base.OnKilled(info);
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_WantsMount(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (DirectlyMountable())
		{
			AttemptMount(player);
		}
	}

	public virtual void AttemptMount(BasePlayer player, bool doMountChecks = true)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)_mounted != (Object)null || IsDead() || !player.CanMountMountablesNow())
		{
			return;
		}
		if (doMountChecks)
		{
			if (checkPlayerLosOnMount && Physics.Linecast(player.eyes.position, mountAnchor.get_position() + ((Component)this).get_transform().get_up() * 0.5f, 1218652417))
			{
				Debug.Log((object)"No line of sight to mount pos");
				return;
			}
			if (!HasValidDismountPosition(player))
			{
				Debug.Log((object)"no valid dismount");
				return;
			}
		}
		MountPlayer(player);
	}

	public virtual bool AttemptDismount(BasePlayer player)
	{
		if ((Object)(object)player != (Object)(object)_mounted)
		{
			return false;
		}
		DismountPlayer(player);
		return true;
	}

	[RPC_Server]
	public void RPC_WantsDismount(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (HasValidDismountPosition(player))
		{
			AttemptDismount(player);
		}
	}

	public void MountPlayer(BasePlayer player)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)_mounted != (Object)null) && !((Object)(object)mountAnchor == (Object)null))
		{
			player.EnsureDismounted();
			_mounted = player;
			Transform transform = ((Component)mountAnchor).get_transform();
			player.MountObject(this);
			player.MovePosition(transform.get_position());
			((Component)player).get_transform().set_rotation(transform.get_rotation());
			player.ServerRotation = transform.get_rotation();
			Quaternion rotation = transform.get_rotation();
			player.OverrideViewAngles(((Quaternion)(ref rotation)).get_eulerAngles());
			_mounted.eyes.NetworkUpdate(transform.get_rotation());
			player.ClientRPCPlayer<Vector3>(null, player, "ForcePositionTo", ((Component)player).get_transform().get_position());
			SetFlag(Flags.Busy, b: true);
			OnPlayerMounted();
		}
	}

	public virtual void OnPlayerMounted()
	{
	}

	public virtual void OnPlayerDismounted(BasePlayer player)
	{
	}

	public virtual void DismountAllPlayers()
	{
		if (Object.op_Implicit((Object)(object)_mounted))
		{
			DismountPlayer(_mounted);
		}
	}

	public void DismountPlayer(BasePlayer player, bool lite = false)
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)_mounted == (Object)null || (Object)(object)_mounted != (Object)(object)player)
		{
			return;
		}
		BaseVehicle baseVehicle = VehicleParent();
		Vector3 res;
		if (lite)
		{
			if ((Object)(object)baseVehicle != (Object)null)
			{
				baseVehicle.PrePlayerDismount(player, this);
			}
			_mounted.DismountObject();
			_mounted = null;
			SetFlag(Flags.Busy, b: false);
			if ((Object)(object)baseVehicle != (Object)null)
			{
				baseVehicle.PlayerDismounted(player, this);
			}
		}
		else if (!GetDismountPosition(player, out res) || Distance(res) > 10f)
		{
			if ((Object)(object)baseVehicle != (Object)null)
			{
				baseVehicle.PrePlayerDismount(player, this);
			}
			res = ((Component)player).get_transform().get_position();
			_mounted.DismountObject();
			_mounted.MovePosition(res);
			_mounted.ClientRPCPlayer<Vector3>(null, _mounted, "ForcePositionTo", res);
			BasePlayer mounted = _mounted;
			_mounted = null;
			Debug.LogWarning((object)("Killing player due to invalid dismount point :" + player.displayName + " / " + player.userID + " on obj : " + ((Object)((Component)this).get_gameObject()).get_name()));
			mounted.Hurt(1000f, DamageType.Suicide, mounted, useProtection: false);
			SetFlag(Flags.Busy, b: false);
			if ((Object)(object)baseVehicle != (Object)null)
			{
				baseVehicle.PlayerDismounted(player, this);
			}
		}
		else
		{
			if ((Object)(object)baseVehicle != (Object)null)
			{
				baseVehicle.PrePlayerDismount(player, this);
			}
			_mounted.DismountObject();
			((Component)_mounted).get_transform().set_rotation(Quaternion.LookRotation(Vector3.get_forward(), Vector3.get_up()));
			_mounted.MovePosition(res);
			_mounted.SendNetworkUpdateImmediate();
			_mounted.SendModelState(force: true);
			_mounted = null;
			SetFlag(Flags.Busy, b: false);
			if ((Object)(object)baseVehicle != (Object)null)
			{
				baseVehicle.PlayerDismounted(player, this);
			}
			player.ForceUpdateTriggers();
			if (Object.op_Implicit((Object)(object)player.GetParentEntity()))
			{
				BaseEntity baseEntity = player.GetParentEntity();
				player.ClientRPCPlayer<Vector3, uint>(null, player, "ForcePositionToParentOffset", ((Component)baseEntity).get_transform().InverseTransformPoint(res), baseEntity.net.ID);
			}
			else
			{
				player.ClientRPCPlayer<Vector3>(null, player, "ForcePositionTo", res);
			}
			OnPlayerDismounted(player);
		}
	}

	public bool ValidDismountPosition(Vector3 disPos, Vector3 visualCheckOrigin)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		bool debugDismounts = Debugging.DebugDismounts;
		if (debugDismounts)
		{
			Debug.Log((object)$"ValidDismountPosition debug: Checking dismount point {disPos} from {visualCheckOrigin}.");
		}
		Vector3 val = disPos + new Vector3(0f, 0.5f, 0f);
		Vector3 val2 = disPos + new Vector3(0f, 1.3f, 0f);
		if (!Physics.CheckCapsule(val, val2, 0.5f, 1537286401))
		{
			Vector3 val3 = disPos + ((Component)this).get_transform().get_up() * 0.5f;
			if (IsVisible(val3))
			{
				if (debugDismounts)
				{
					Debug.Log((object)$"ValidDismountPosition debug: Dismount point {disPos} is visible.");
				}
				RaycastHit hit2 = default(RaycastHit);
				if (!Physics.Linecast(visualCheckOrigin, val3, ref hit2, 1486946561) || HitOurself(hit2))
				{
					if (debugDismounts)
					{
						Debug.Log((object)$"ValidDismountPosition debug: Dismount point {disPos} linecast is OK.");
					}
					Ray val4 = new Ray(visualCheckOrigin, Vector3Ex.Direction(val3, visualCheckOrigin));
					float num = Vector3.Distance(visualCheckOrigin, val3);
					if (!Physics.SphereCast(val4, 0.5f, ref hit2, num, 1486946561) || HitOurself(hit2))
					{
						if (debugDismounts)
						{
							if (debugDismounts)
							{
								Debug.Log((object)$"<color=green>ValidDismountPosition debug: Dismount point {disPos} is valid</color>.");
							}
							Debug.DrawLine(visualCheckOrigin, disPos, Color.get_green(), 10f);
						}
						return true;
					}
				}
			}
		}
		if (debugDismounts)
		{
			Debug.DrawLine(visualCheckOrigin, disPos, Color.get_red(), 10f);
			if (debugDismounts)
			{
				Debug.Log((object)$"<color=red>ValidDismountPosition debug: Dismount point {disPos} is invalid</color>.");
			}
		}
		return false;
		bool HitOurself(RaycastHit hit)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			BaseEntity entity = hit.GetEntity();
			if (!((Object)(object)entity == (Object)(object)this))
			{
				return EqualNetID(entity);
			}
			return true;
		}
	}

	public virtual bool HasValidDismountPosition(BasePlayer player)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		BaseVehicle baseVehicle = VehicleParent();
		if ((Object)(object)baseVehicle != (Object)null)
		{
			return baseVehicle.HasValidDismountPosition(player);
		}
		Vector3 visualCheckOrigin = player.TriggerPoint();
		Transform[] array = dismountPositions;
		foreach (Transform val in array)
		{
			if (ValidDismountPosition(((Component)val).get_transform().get_position(), visualCheckOrigin))
			{
				return true;
			}
		}
		return false;
	}

	public virtual bool GetDismountPosition(BasePlayer player, out Vector3 res)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		BaseVehicle baseVehicle = VehicleParent();
		if ((Object)(object)baseVehicle != (Object)null)
		{
			return baseVehicle.GetDismountPosition(player, out res);
		}
		int num = 0;
		Vector3 visualCheckOrigin = player.TriggerPoint();
		Transform[] array = dismountPositions;
		foreach (Transform val in array)
		{
			if (ValidDismountPosition(((Component)val).get_transform().get_position(), visualCheckOrigin))
			{
				res = ((Component)val).get_transform().get_position();
				return true;
			}
			num++;
		}
		Debug.LogWarning((object)("Failed to find dismount position for player :" + player.displayName + " / " + player.userID + " on obj : " + ((Object)((Component)this).get_gameObject()).get_name()));
		res = ((Component)player).get_transform().get_position();
		return false;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (isMobile)
		{
			FixedUpdateMountables.Add(this);
		}
	}

	internal override void DoServerDestroy()
	{
		FixedUpdateMountables.Remove(this);
		base.DoServerDestroy();
	}

	public static void FixedUpdateCycle()
	{
		for (int num = FixedUpdateMountables.get_Count() - 1; num >= 0; num--)
		{
			BaseMountable baseMountable = FixedUpdateMountables.get_Item(num);
			if ((Object)(object)baseMountable == (Object)null)
			{
				FixedUpdateMountables.RemoveAt(num);
			}
			else if (baseMountable.isSpawned)
			{
				baseMountable.VehicleFixedUpdate();
			}
		}
	}

	public virtual void VehicleFixedUpdate()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)_mounted))
		{
			((Component)_mounted).get_transform().set_rotation(((Component)mountAnchor).get_transform().get_rotation());
			_mounted.ServerRotation = ((Component)mountAnchor).get_transform().get_rotation();
			_mounted.MovePosition(((Component)mountAnchor).get_transform().get_position());
		}
	}

	public virtual void PlayerServerInput(InputState inputState, BasePlayer player)
	{
	}

	public virtual float GetComfort()
	{
		return 0f;
	}

	public virtual void ScaleDamageForPlayer(BasePlayer player, HitInfo info)
	{
	}

	public static bool TryFireProjectile(StorageContainer ammoStorage, AmmoTypes ammoType, Vector3 firingPos, Vector3 firingDir, BasePlayer driver, float launchOffset, float minSpeed, out ServerProjectile projectile)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		projectile = null;
		if ((Object)(object)ammoStorage == (Object)null)
		{
			return false;
		}
		bool result = false;
		List<Item> list = Pool.GetList<Item>();
		ammoStorage.inventory.FindAmmo(list, ammoType);
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (list[num].amount <= 0)
			{
				list.RemoveAt(num);
			}
		}
		if (list.Count > 0)
		{
			RaycastHit val = default(RaycastHit);
			if (Physics.Raycast(firingPos, firingDir, ref val, launchOffset, 1236478737))
			{
				launchOffset = ((RaycastHit)(ref val)).get_distance() - 0.1f;
			}
			Item item = list[list.Count - 1];
			ItemModProjectile component = ((Component)item.info).GetComponent<ItemModProjectile>();
			BaseEntity baseEntity = GameManager.server.CreateEntity(component.projectileObject.resourcePath, firingPos + firingDir * launchOffset);
			projectile = ((Component)baseEntity).GetComponent<ServerProjectile>();
			Vector3 val2 = projectile.initialVelocity + firingDir * projectile.speed;
			if (minSpeed > 0f)
			{
				float num2 = Vector3.Dot(val2, firingDir) - minSpeed;
				if (num2 < 0f)
				{
					val2 += firingDir * (0f - num2);
				}
			}
			projectile.InitializeVelocity(val2);
			if (driver.IsValid())
			{
				baseEntity.creatorEntity = driver;
				baseEntity.OwnerID = driver.userID;
			}
			baseEntity.Spawn();
			item.UseItem();
			result = true;
		}
		Pool.FreeList<Item>(ref list);
		return result;
	}

	public override void DisableTransferProtection()
	{
		base.DisableTransferProtection();
		BasePlayer mounted = GetMounted();
		if ((Object)(object)mounted != (Object)null && mounted.IsTransferProtected())
		{
			mounted.DisableTransferProtection();
		}
	}

	public virtual bool IsInstrument()
	{
		return false;
	}

	public bool NearMountPoint(BasePlayer player)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)player == (Object)null)
		{
			return false;
		}
		if ((Object)(object)mountAnchor == (Object)null)
		{
			return false;
		}
		if (Vector3.Distance(((Component)player).get_transform().get_position(), mountAnchor.get_position()) <= maxMountDistance)
		{
			RaycastHit hit = default(RaycastHit);
			if (!Physics.SphereCast(player.eyes.HeadRay(), 0.25f, ref hit, 2f, 1218652417))
			{
				return false;
			}
			BaseEntity entity = hit.GetEntity();
			if ((Object)(object)entity != (Object)null)
			{
				if ((Object)(object)entity == (Object)(object)this || EqualNetID(entity))
				{
					return true;
				}
				BaseEntity baseEntity = entity.GetParentEntity();
				if (hit.IsOnLayer((Layer)13) && ((Object)(object)baseEntity == (Object)(object)this || EqualNetID(baseEntity)))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static Vector3 ConvertVector(Vector3 vec)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 3; i++)
		{
			if (((Vector3)(ref vec)).get_Item(i) > 180f)
			{
				ref Vector3 reference = ref vec;
				int num = i;
				((Vector3)(ref reference)).set_Item(num, ((Vector3)(ref reference)).get_Item(num) - 360f);
			}
			else if (((Vector3)(ref vec)).get_Item(i) < -180f)
			{
				ref Vector3 reference = ref vec;
				int num = i;
				((Vector3)(ref reference)).set_Item(num, ((Vector3)(ref reference)).get_Item(num) + 360f);
			}
		}
		return vec;
	}
}
