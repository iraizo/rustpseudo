using System;
using Rust;
using UnityEngine;

public class CH47HelicopterAIController : CH47Helicopter
{
	public GameObjectRef scientistPrefab;

	public GameObjectRef dismountablePrefab;

	public GameObjectRef weakDismountablePrefab;

	public float maxTiltAngle = 0.3f;

	public float AiAltitudeForce = 10000f;

	public GameObjectRef lockedCratePrefab;

	public const Flags Flag_Damaged = Flags.Reserved7;

	public const Flags Flag_NearDeath = Flags.OnFire;

	public const Flags Flag_DropDoorOpen = Flags.Reserved8;

	public GameObject triggerHurt;

	public Vector3 landingTarget;

	private int numCrates = 1;

	private bool shouldLand;

	private bool aimDirOverride;

	private Vector3 _aimDirection = Vector3.get_forward();

	private Vector3 _moveTarget = Vector3.get_zero();

	private int lastAltitudeCheckFrame;

	private float altOverride;

	private float currentDesiredAltitude;

	private bool altitudeProtection = true;

	private float hoverHeight = 30f;

	public void DropCrate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (numCrates > 0)
		{
			Vector3 pos = ((Component)this).get_transform().get_position() + Vector3.get_down() * 5f;
			Quaternion rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
			BaseEntity baseEntity = GameManager.server.CreateEntity(lockedCratePrefab.resourcePath, pos, rot);
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				((Component)baseEntity).SendMessage("SetWasDropped");
				baseEntity.Spawn();
			}
			numCrates--;
		}
	}

	public bool OutOfCrates()
	{
		return numCrates <= 0;
	}

	public bool CanDropCrate()
	{
		return numCrates > 0;
	}

	public bool IsDropDoorOpen()
	{
		return HasFlag(Flags.Reserved8);
	}

	public void SetDropDoorOpen(bool open)
	{
		SetFlag(Flags.Reserved8, open);
	}

	public bool ShouldLand()
	{
		return shouldLand;
	}

	public void SetLandingTarget(Vector3 target)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		shouldLand = true;
		landingTarget = target;
		numCrates = 0;
	}

	public void ClearLandingTarget()
	{
		shouldLand = false;
	}

	public void TriggeredEventSpawn()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		float x = TerrainMeta.Size.x;
		float y = 30f;
		Vector3 val = Vector3Ex.Range(-1f, 1f);
		val.y = 0f;
		((Vector3)(ref val)).Normalize();
		val *= x * 1f;
		val.y = y;
		((Component)this).get_transform().set_position(val);
	}

	public override void AttemptMount(BasePlayer player, bool doMountChecks = true)
	{
		if (player.IsNpc || player.IsAdmin)
		{
			base.AttemptMount(player, doMountChecks);
		}
	}

	public override void ServerInit()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		((FacepunchBehaviour)this).Invoke((Action)SpawnScientists, 0.25f);
		SetMoveTarget(((Component)this).get_transform().get_position());
	}

	public void SpawnPassenger(Vector3 spawnPos, string prefabPath)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Quaternion identity = Quaternion.get_identity();
		HumanNPC component = ((Component)GameManager.server.CreateEntity(prefabPath, spawnPos, identity)).GetComponent<HumanNPC>();
		component.Spawn();
		AttemptMount(component);
	}

	public void SpawnPassenger(Vector3 spawnPos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		Quaternion identity = Quaternion.get_identity();
		HumanNPC component = ((Component)GameManager.server.CreateEntity(dismountablePrefab.resourcePath, spawnPos, identity)).GetComponent<HumanNPC>();
		component.Spawn();
		AttemptMount(component);
	}

	public void SpawnScientist(Vector3 spawnPos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		Quaternion identity = Quaternion.get_identity();
		HumanNPC component = ((Component)GameManager.server.CreateEntity(scientistPrefab.resourcePath, spawnPos, identity)).GetComponent<HumanNPC>();
		component.Spawn();
		AttemptMount(component);
		component.Brain.SetEnabled(flag: false);
	}

	public void SpawnScientists()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		if (shouldLand)
		{
			float dropoffScale = CH47LandingZone.GetClosest(landingTarget).dropoffScale;
			int num = Mathf.FloorToInt((float)(mountPoints.Count - 2) * dropoffScale);
			for (int i = 0; i < num; i++)
			{
				Vector3 spawnPos = ((Component)this).get_transform().get_position() + ((Component)this).get_transform().get_forward() * 10f;
				SpawnPassenger(spawnPos, dismountablePrefab.resourcePath);
			}
			for (int j = 0; j < 1; j++)
			{
				Vector3 spawnPos2 = ((Component)this).get_transform().get_position() - ((Component)this).get_transform().get_forward() * 15f;
				SpawnPassenger(spawnPos2);
			}
		}
		else
		{
			for (int k = 0; k < 4; k++)
			{
				Vector3 spawnPos3 = ((Component)this).get_transform().get_position() + ((Component)this).get_transform().get_forward() * 10f;
				SpawnScientist(spawnPos3);
			}
			for (int l = 0; l < 1; l++)
			{
				Vector3 spawnPos4 = ((Component)this).get_transform().get_position() - ((Component)this).get_transform().get_forward() * 15f;
				SpawnScientist(spawnPos4);
			}
		}
	}

	public void EnableFacingOverride(bool enabled)
	{
		aimDirOverride = enabled;
	}

	public void SetMoveTarget(Vector3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		_moveTarget = position;
	}

	public Vector3 GetMoveTarget()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return _moveTarget;
	}

	public void SetAimDirection(Vector3 dir)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		_aimDirection = dir;
	}

	public Vector3 GetAimDirectionOverride()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return _aimDirection;
	}

	public Vector3 GetPosition()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).get_transform().get_position();
	}

	public override void MounteeTookDamage(BasePlayer mountee, HitInfo info)
	{
		InitiateAnger();
	}

	public void CancelAnger()
	{
		if (base.SecondsSinceAttacked > 120f)
		{
			UnHostile();
			((FacepunchBehaviour)this).CancelInvoke((Action)UnHostile);
		}
	}

	public void InitiateAnger()
	{
		((FacepunchBehaviour)this).CancelInvoke((Action)UnHostile);
		((FacepunchBehaviour)this).Invoke((Action)UnHostile, 120f);
		foreach (MountPointInfo mountPoint in mountPoints)
		{
			if (!((Object)(object)mountPoint.mountable != (Object)null))
			{
				continue;
			}
			BasePlayer mounted = mountPoint.mountable.GetMounted();
			if (Object.op_Implicit((Object)(object)mounted))
			{
				ScientistNPC scientistNPC = mounted as ScientistNPC;
				if ((Object)(object)scientistNPC != (Object)null)
				{
					scientistNPC.Brain.SetEnabled(flag: true);
				}
			}
		}
	}

	public void UnHostile()
	{
		foreach (MountPointInfo mountPoint in mountPoints)
		{
			if (!((Object)(object)mountPoint.mountable != (Object)null))
			{
				continue;
			}
			BasePlayer mounted = mountPoint.mountable.GetMounted();
			if (Object.op_Implicit((Object)(object)mounted))
			{
				ScientistNPC scientistNPC = mounted as ScientistNPC;
				if ((Object)(object)scientistNPC != (Object)null)
				{
					scientistNPC.Brain.SetEnabled(flag: false);
				}
			}
		}
	}

	public override void OnKilled(HitInfo info)
	{
		if (!OutOfCrates())
		{
			DropCrate();
		}
		base.OnKilled(info);
	}

	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		InitiateAnger();
		SetFlag(Flags.Reserved7, base.healthFraction <= 0.8f);
		SetFlag(Flags.OnFire, base.healthFraction <= 0.33f);
	}

	public void DelayedKill()
	{
		foreach (MountPointInfo mountPoint in mountPoints)
		{
			if ((Object)(object)mountPoint.mountable != (Object)null)
			{
				BasePlayer mounted = mountPoint.mountable.GetMounted();
				if (Object.op_Implicit((Object)(object)mounted) && (Object)(object)((Component)mounted).get_transform() != (Object)null && !mounted.IsDestroyed && !mounted.IsDead() && mounted.IsNpc)
				{
					mounted.Kill();
				}
			}
		}
		Kill();
	}

	public override void DismountAllPlayers()
	{
		foreach (MountPointInfo mountPoint in mountPoints)
		{
			if ((Object)(object)mountPoint.mountable != (Object)null)
			{
				BasePlayer mounted = mountPoint.mountable.GetMounted();
				if (Object.op_Implicit((Object)(object)mounted))
				{
					mounted.Hurt(10000f, DamageType.Explosion, this, useProtection: false);
				}
			}
		}
	}

	public void SetAltitudeProtection(bool on)
	{
		altitudeProtection = on;
	}

	public void CalculateDesiredAltitude()
	{
		CalculateOverrideAltitude();
		if (altOverride > currentDesiredAltitude)
		{
			currentDesiredAltitude = altOverride;
		}
		else
		{
			currentDesiredAltitude = Mathf.MoveTowards(currentDesiredAltitude, altOverride, Time.get_fixedDeltaTime() * 5f);
		}
	}

	public void SetMinHoverHeight(float newHeight)
	{
		hoverHeight = newHeight;
	}

	public float CalculateOverrideAltitude()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		if (Time.get_frameCount() == lastAltitudeCheckFrame)
		{
			return altOverride;
		}
		lastAltitudeCheckFrame = Time.get_frameCount();
		float y = GetMoveTarget().y;
		float num = Mathf.Max(TerrainMeta.WaterMap.GetHeight(GetMoveTarget()), TerrainMeta.HeightMap.GetHeight(GetMoveTarget()));
		float num2 = Mathf.Max(y, num + hoverHeight);
		if (altitudeProtection)
		{
			Vector3 val = rigidBody.get_velocity();
			Vector3 val2;
			if (!(((Vector3)(ref val)).get_magnitude() < 0.1f))
			{
				val = rigidBody.get_velocity();
				val2 = ((Vector3)(ref val)).get_normalized();
			}
			else
			{
				val2 = ((Component)this).get_transform().get_forward();
			}
			Vector3 val3 = val2;
			val = Vector3.Cross(Vector3.Cross(((Component)this).get_transform().get_up(), val3), Vector3.get_up()) + Vector3.get_down() * 0.3f;
			Vector3 normalized = ((Vector3)(ref val)).get_normalized();
			RaycastHit val4 = default(RaycastHit);
			RaycastHit val5 = default(RaycastHit);
			if (Physics.SphereCast(((Component)this).get_transform().get_position() - normalized * 20f, 20f, normalized, ref val4, 75f, 1218511105) && Physics.SphereCast(((RaycastHit)(ref val4)).get_point() + Vector3.get_up() * 200f, 20f, Vector3.get_down(), ref val5, 200f, 1218511105))
			{
				num2 = ((RaycastHit)(ref val5)).get_point().y + hoverHeight;
			}
		}
		altOverride = num2;
		return altOverride;
	}

	public override void SetDefaultInputState()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		currentInputState.Reset();
		Vector3 moveTarget = GetMoveTarget();
		Vector3 val = Vector3.Cross(((Component)this).get_transform().get_right(), Vector3.get_up());
		Vector3 val2 = Vector3.Cross(Vector3.get_up(), val);
		float num = 0f - Vector3.Dot(Vector3.get_up(), ((Component)this).get_transform().get_right());
		float num2 = Vector3.Dot(Vector3.get_up(), ((Component)this).get_transform().get_forward());
		float num3 = Vector3Ex.Distance2D(((Component)this).get_transform().get_position(), moveTarget);
		float y = ((Component)this).get_transform().get_position().y;
		float num4 = currentDesiredAltitude;
		Vector3 val3 = ((Component)this).get_transform().get_position() + ((Component)this).get_transform().get_forward() * 10f;
		val3.y = num4;
		Vector3 val4 = Vector3Ex.Direction2D(moveTarget, ((Component)this).get_transform().get_position());
		float num5 = 0f - Vector3.Dot(val4, val2);
		float num6 = Vector3.Dot(val4, val);
		float num7 = Mathf.InverseLerp(0f, 25f, num3);
		if (num6 > 0f)
		{
			float num8 = Mathf.InverseLerp(0f - maxTiltAngle, 0f, num2);
			currentInputState.pitch = 1f * num6 * num8 * num7;
		}
		else
		{
			float num9 = 1f - Mathf.InverseLerp(0f, maxTiltAngle, num2);
			currentInputState.pitch = 1f * num6 * num9 * num7;
		}
		if (num5 > 0f)
		{
			float num10 = Mathf.InverseLerp(0f - maxTiltAngle, 0f, num);
			currentInputState.roll = 1f * num5 * num10 * num7;
		}
		else
		{
			float num11 = 1f - Mathf.InverseLerp(0f, maxTiltAngle, num);
			currentInputState.roll = 1f * num5 * num11 * num7;
		}
		float num12 = Mathf.Abs(num4 - y);
		float num13 = 1f - Mathf.InverseLerp(10f, 30f, num12);
		currentInputState.pitch *= num13;
		currentInputState.roll *= num13;
		float num14 = maxTiltAngle;
		float num15 = Mathf.InverseLerp(0f + Mathf.Abs(currentInputState.pitch) * num14, num14 + Mathf.Abs(currentInputState.pitch) * num14, Mathf.Abs(num2));
		currentInputState.pitch += num15 * ((num2 < 0f) ? (-1f) : 1f);
		float num16 = Mathf.InverseLerp(0f + Mathf.Abs(currentInputState.roll) * num14, num14 + Mathf.Abs(currentInputState.roll) * num14, Mathf.Abs(num));
		currentInputState.roll += num16 * ((num < 0f) ? (-1f) : 1f);
		if (aimDirOverride || num3 > 30f)
		{
			Vector3 val5 = (aimDirOverride ? GetAimDirectionOverride() : Vector3Ex.Direction2D(GetMoveTarget(), ((Component)this).get_transform().get_position()));
			Vector3 val6 = (aimDirOverride ? GetAimDirectionOverride() : Vector3Ex.Direction2D(GetMoveTarget(), ((Component)this).get_transform().get_position()));
			float num17 = Vector3.Dot(val2, val5);
			float num18 = Vector3.Angle(val, val6);
			float num19 = Mathf.InverseLerp(0f, 70f, Mathf.Abs(num18));
			currentInputState.yaw = ((num17 > 0f) ? 1f : 0f);
			currentInputState.yaw -= ((num17 < 0f) ? 1f : 0f);
			currentInputState.yaw *= num19;
		}
		float throttle = Mathf.InverseLerp(5f, 30f, num3);
		currentInputState.throttle = throttle;
	}

	public void MaintainAIAltutide()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Component)this).get_transform().get_position() + rigidBody.get_velocity();
		float num = currentDesiredAltitude;
		float y = val.y;
		float num2 = Mathf.Abs(num - y);
		bool flag = num > y;
		float num3 = Mathf.InverseLerp(0f, 10f, num2) * AiAltitudeForce * (flag ? 1f : (-1f));
		rigidBody.AddForce(Vector3.get_up() * num3, (ForceMode)0);
	}

	public override void VehicleFixedUpdate()
	{
		hoverForceScale = 1f;
		base.VehicleFixedUpdate();
		SetFlag(Flags.Reserved5, TOD_Sky.get_Instance().get_IsNight());
		CalculateDesiredAltitude();
		MaintainAIAltutide();
	}

	public override void DestroyShared()
	{
		if (base.isServer)
		{
			foreach (MountPointInfo mountPoint in mountPoints)
			{
				if ((Object)(object)mountPoint.mountable != (Object)null)
				{
					BasePlayer mounted = mountPoint.mountable.GetMounted();
					if (Object.op_Implicit((Object)(object)mounted) && (Object)(object)((Component)mounted).get_transform() != (Object)null && !mounted.IsDestroyed && !mounted.IsDead() && mounted.IsNpc)
					{
						mounted.Kill();
					}
				}
			}
		}
		base.DestroyShared();
	}
}
