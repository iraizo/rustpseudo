using System;
using System.Collections.Generic;
using UnityEngine;

public class Sled : BaseVehicle, INotifyTrigger
{
	private const Flags BrakeOn = Flags.Reserved1;

	private const Flags OnSnow = Flags.Reserved2;

	private const Flags IsGrounded = Flags.Reserved3;

	private const Flags OnSand = Flags.Reserved4;

	public PhysicMaterial BrakeMaterial;

	public PhysicMaterial SnowMaterial;

	public PhysicMaterial NonSnowMaterial;

	public Transform CentreOfMassTransform;

	public Collider[] PhysicsMaterialTargets;

	public float InitialForceCutoff = 3f;

	public float InitialForceIncreaseRate = 0.05f;

	public float TurnForce = 1f;

	public float DirectionMatchForce = 1f;

	public float VerticalAdjustmentForce = 1f;

	public float VerticalAdjustmentAngleThreshold = 15f;

	public float NudgeCooldown = 3f;

	public float NudgeForce = 2f;

	public float MaxNudgeVelocity = 2f;

	public const float DecayFrequency = 60f;

	public float DecayAmount = 10f;

	public ParticleSystemContainer TrailEffects;

	public SoundDefinition enterSnowSoundDef;

	public SoundDefinition snowSlideLoopSoundDef;

	public SoundDefinition dirtSlideLoopSoundDef;

	public AnimationCurve movementLoopGainCurve;

	public AnimationCurve movementLoopPitchCurve;

	private VehicleTerrainHandler terrainHandler;

	private PhysicMaterial cachedMaterial;

	private float initialForceScale;

	private TimeSince leftIce;

	private TimeSince lastNudge;

	public override bool BlocksDoors => false;

	public override void ServerInit()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		terrainHandler = new VehicleTerrainHandler(this);
		terrainHandler.RayLength = 0.6f;
		rigidBody.set_centerOfMass(CentreOfMassTransform.get_localPosition());
		((FacepunchBehaviour)this).InvokeRandomized((Action)DecayOverTime, Random.Range(30f, 60f), 60f, 6f);
	}

	public override void OnDeployed(BaseEntity parent, BasePlayer deployedBy, Item fromItem)
	{
		base.OnDeployed(parent, deployedBy, fromItem);
		SetFlag(Flags.Reserved1, b: true);
		UpdateGroundedFlag();
		UpdatePhysicsMaterial();
	}

	public override void VehicleFixedUpdate()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		base.VehicleFixedUpdate();
		if (!AnyMounted())
		{
			return;
		}
		terrainHandler.FixedUpdate();
		if (!terrainHandler.IsGrounded)
		{
			Quaternion val = Quaternion.FromToRotation(((Component)this).get_transform().get_up(), Vector3.get_up()) * rigidBody.get_rotation();
			if (Quaternion.Angle(rigidBody.get_rotation(), val) > VerticalAdjustmentAngleThreshold)
			{
				rigidBody.MoveRotation(Quaternion.Slerp(rigidBody.get_rotation(), val, Time.get_fixedDeltaTime() * VerticalAdjustmentForce));
			}
		}
	}

	private void UpdatePhysicsMaterial()
	{
		cachedMaterial = GetPhysicMaterial();
		Collider[] physicsMaterialTargets = PhysicsMaterialTargets;
		for (int i = 0; i < physicsMaterialTargets.Length; i++)
		{
			physicsMaterialTargets[i].set_sharedMaterial(cachedMaterial);
		}
		if (!AnyMounted() && rigidBody.IsSleeping())
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)UpdatePhysicsMaterial);
		}
		SetFlag(Flags.Reserved2, terrainHandler.IsOnSnowOrIce);
		SetFlag(Flags.Reserved4, terrainHandler.OnSurface == VehicleTerrainHandler.Surface.Sand);
	}

	private void UpdateGroundedFlag()
	{
		if (!AnyMounted() && rigidBody.IsSleeping())
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)UpdateGroundedFlag);
		}
		SetFlag(Flags.Reserved3, terrainHandler.IsGrounded);
	}

	private PhysicMaterial GetPhysicMaterial()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (HasFlag(Flags.Reserved1) || !AnyMounted())
		{
			return BrakeMaterial;
		}
		bool flag = terrainHandler.IsOnSnowOrIce || terrainHandler.OnSurface == VehicleTerrainHandler.Surface.Sand;
		if (flag)
		{
			leftIce = TimeSince.op_Implicit(0f);
		}
		else if (TimeSince.op_Implicit(leftIce) < 2f)
		{
			flag = true;
		}
		if (!flag)
		{
			return NonSnowMaterial;
		}
		return SnowMaterial;
	}

	public override void PlayerMounted(BasePlayer player, BaseMountable seat)
	{
		base.PlayerMounted(player, seat);
		if (HasFlag(Flags.Reserved1))
		{
			initialForceScale = 0f;
			((FacepunchBehaviour)this).InvokeRepeating((Action)ApplyInitialForce, 0f, 0.1f);
			SetFlag(Flags.Reserved1, b: false);
		}
		if (!((FacepunchBehaviour)this).IsInvoking((Action)UpdatePhysicsMaterial))
		{
			((FacepunchBehaviour)this).InvokeRepeating((Action)UpdatePhysicsMaterial, 0f, 0.5f);
		}
		if (!((FacepunchBehaviour)this).IsInvoking((Action)UpdateGroundedFlag))
		{
			((FacepunchBehaviour)this).InvokeRepeating((Action)UpdateGroundedFlag, 0f, 0.1f);
		}
		if (rigidBody.IsSleeping())
		{
			rigidBody.WakeUp();
		}
	}

	private void ApplyInitialForce()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		Vector3 forward = ((Component)this).get_transform().get_forward();
		Vector3 val = ((Vector3.Dot(forward, -Vector3.get_up()) > Vector3.Dot(-forward, -Vector3.get_up())) ? forward : (-forward));
		rigidBody.AddForce(val * initialForceScale * (terrainHandler.IsOnSnowOrIce ? 1f : 0.25f), (ForceMode)5);
		initialForceScale += InitialForceIncreaseRate;
		if (initialForceScale >= InitialForceCutoff)
		{
			Vector3 velocity = rigidBody.get_velocity();
			if (((Vector3)(ref velocity)).get_magnitude() > 1f || !terrainHandler.IsOnSnowOrIce)
			{
				((FacepunchBehaviour)this).CancelInvoke((Action)ApplyInitialForce);
			}
		}
	}

	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		base.PlayerServerInput(inputState, player);
		if (Vector3.Dot(((Component)this).get_transform().get_up(), Vector3.get_up()) < 0.1f || WaterFactor() > 0.25f)
		{
			DismountAllPlayers();
			return;
		}
		float num = (inputState.IsDown(BUTTON.LEFT) ? (-1f) : 0f);
		num += (inputState.IsDown(BUTTON.RIGHT) ? 1f : 0f);
		Vector3 velocity;
		if (inputState.IsDown(BUTTON.FORWARD) && TimeSince.op_Implicit(lastNudge) > NudgeCooldown)
		{
			velocity = rigidBody.get_velocity();
			if (((Vector3)(ref velocity)).get_magnitude() < MaxNudgeVelocity)
			{
				rigidBody.WakeUp();
				rigidBody.AddForce(((Component)this).get_transform().get_forward() * NudgeForce, (ForceMode)1);
				rigidBody.AddForce(((Component)this).get_transform().get_up() * NudgeForce * 0.5f, (ForceMode)1);
				lastNudge = TimeSince.op_Implicit(0f);
			}
		}
		num *= TurnForce;
		Vector3 velocity2 = rigidBody.get_velocity();
		if (num != 0f)
		{
			((Component)this).get_transform().Rotate(Vector3.get_up() * num * Time.get_deltaTime() * ((Vector3)(ref velocity2)).get_magnitude(), (Space)1);
		}
		if (terrainHandler.IsGrounded)
		{
			velocity = rigidBody.get_velocity();
			if (Vector3.Dot(((Vector3)(ref velocity)).get_normalized(), ((Component)this).get_transform().get_forward()) >= 0.5f)
			{
				rigidBody.set_velocity(Vector3.Lerp(rigidBody.get_velocity(), ((Component)this).get_transform().get_forward() * ((Vector3)(ref velocity2)).get_magnitude(), Time.get_deltaTime() * DirectionMatchForce));
			}
		}
	}

	private void DecayOverTime()
	{
		if (!AnyMounted())
		{
			Hurt(DecayAmount);
		}
	}

	public override bool CanPickup(BasePlayer player)
	{
		if (base.CanPickup(player))
		{
			return !player.isMounted;
		}
		return false;
	}

	public void OnObjects(TriggerNotify trigger)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		Enumerator<BaseEntity> enumerator = trigger.entityContents.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BaseEntity current = enumerator.get_Current();
				if (!(current is Sled))
				{
					BaseVehicleModule baseVehicleModule;
					if ((baseVehicleModule = current as BaseVehicleModule) != null && (Object)(object)baseVehicleModule.Vehicle != (Object)null && (baseVehicleModule.Vehicle.IsOn() || !baseVehicleModule.Vehicle.IsStationary()))
					{
						Kill(DestroyMode.Gib);
						break;
					}
					BaseVehicle baseVehicle;
					if ((baseVehicle = current as BaseVehicle) != null && baseVehicle.HasDriver() && (baseVehicle.IsMoving() || baseVehicle.HasFlag(Flags.On)))
					{
						Kill(DestroyMode.Gib);
						break;
					}
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
	}

	public void OnEmpty()
	{
	}
}
