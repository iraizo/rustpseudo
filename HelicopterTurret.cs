using System.Collections.Generic;
using ConVar;
using UnityEngine;

public class HelicopterTurret : MonoBehaviour
{
	public PatrolHelicopterAI _heliAI;

	public float fireRate = 0.125f;

	public float burstLength = 3f;

	public float timeBetweenBursts = 3f;

	public float maxTargetRange = 300f;

	public float loseTargetAfter = 5f;

	public Transform gun_yaw;

	public Transform gun_pitch;

	public Transform muzzleTransform;

	public bool left;

	public BaseCombatEntity _target;

	private float lastBurstTime = float.NegativeInfinity;

	private float lastFireTime = float.NegativeInfinity;

	private float lastSeenTargetTime = float.NegativeInfinity;

	private bool targetVisible;

	public void SetTarget(BaseCombatEntity newTarget)
	{
		_target = newTarget;
		UpdateTargetVisibility();
	}

	public bool NeedsNewTarget()
	{
		if (HasTarget())
		{
			if (!targetVisible)
			{
				return TimeSinceTargetLastSeen() > loseTargetAfter;
			}
			return false;
		}
		return true;
	}

	public bool UpdateTargetFromList(List<PatrolHelicopterAI.targetinfo> newTargetList)
	{
		int num = Random.Range(0, newTargetList.Count);
		int num2 = newTargetList.Count;
		while (num2 >= 0)
		{
			num2--;
			PatrolHelicopterAI.targetinfo targetinfo = newTargetList[num];
			if (targetinfo != null && (Object)(object)targetinfo.ent != (Object)null && targetinfo.IsVisible() && InFiringArc(targetinfo.ply))
			{
				SetTarget(targetinfo.ply);
				return true;
			}
			num++;
			if (num >= newTargetList.Count)
			{
				num = 0;
			}
		}
		return false;
	}

	public bool TargetVisible()
	{
		UpdateTargetVisibility();
		return targetVisible;
	}

	public float TimeSinceTargetLastSeen()
	{
		return Time.get_realtimeSinceStartup() - lastSeenTargetTime;
	}

	public bool HasTarget()
	{
		return (Object)(object)_target != (Object)null;
	}

	public void ClearTarget()
	{
		_target = null;
		targetVisible = false;
	}

	public void TurretThink()
	{
		if (HasTarget() && TimeSinceTargetLastSeen() > loseTargetAfter * 2f)
		{
			ClearTarget();
		}
		if (HasTarget())
		{
			if (Time.get_time() - lastBurstTime > burstLength + timeBetweenBursts && TargetVisible())
			{
				lastBurstTime = Time.get_time();
			}
			if (Time.get_time() < lastBurstTime + burstLength && Time.get_time() - lastFireTime >= fireRate && InFiringArc(_target))
			{
				lastFireTime = Time.get_time();
				FireGun();
			}
		}
	}

	public void FireGun()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		_heliAI.FireGun(((Component)_target).get_transform().get_position() + new Vector3(0f, 0.25f, 0f), PatrolHelicopter.bulletAccuracy, left);
	}

	public Vector3 GetPositionForEntity(BaseCombatEntity potentialtarget)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)potentialtarget).get_transform().get_position();
	}

	public float AngleToTarget(BaseCombatEntity potentialtarget)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		Vector3 positionForEntity = GetPositionForEntity(potentialtarget);
		Vector3 position = muzzleTransform.get_position();
		Vector3 val = positionForEntity - position;
		Vector3 normalized = ((Vector3)(ref val)).get_normalized();
		return Vector3.Angle(left ? (-((Component)_heliAI).get_transform().get_right()) : ((Component)_heliAI).get_transform().get_right(), normalized);
	}

	public bool InFiringArc(BaseCombatEntity potentialtarget)
	{
		return AngleToTarget(potentialtarget) < 80f;
	}

	public void UpdateTargetVisibility()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		if (HasTarget())
		{
			Vector3 position = ((Component)_target).get_transform().get_position();
			BasePlayer basePlayer = _target as BasePlayer;
			if (Object.op_Implicit((Object)(object)basePlayer))
			{
				position = basePlayer.eyes.position;
			}
			bool flag = false;
			float num = Vector3.Distance(position, muzzleTransform.get_position());
			Vector3 val = position - muzzleTransform.get_position();
			Vector3 normalized = ((Vector3)(ref val)).get_normalized();
			if (num < maxTargetRange && InFiringArc(_target) && GamePhysics.Trace(new Ray(muzzleTransform.get_position() + normalized * 6f, normalized), 0f, out var hitInfo, num * 1.1f, 1218652417, (QueryTriggerInteraction)0) && (Object)(object)((Component)((RaycastHit)(ref hitInfo)).get_collider()).get_gameObject().ToBaseEntity() == (Object)(object)_target)
			{
				flag = true;
			}
			if (flag)
			{
				lastSeenTargetTime = Time.get_realtimeSinceStartup();
			}
			targetVisible = flag;
		}
	}

	public HelicopterTurret()
		: this()
	{
	}
}
