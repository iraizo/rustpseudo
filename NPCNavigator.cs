using ConVar;
using UnityEngine;
using UnityEngine.AI;

public class NPCNavigator : BaseNavigator
{
	public BaseNpc NPC { get; private set; }

	public override void Init(BaseCombatEntity entity, NavMeshAgent agent)
	{
		base.Init(entity, agent);
		NPC = entity as BaseNpc;
	}

	protected override bool CanEnableNavMeshNavigation()
	{
		if (!base.CanEnableNavMeshNavigation())
		{
			return false;
		}
		return true;
	}

	protected override bool CanUpdateMovement()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (!base.CanUpdateMovement())
		{
			return false;
		}
		if ((Object)(object)NPC != (Object)null && (NPC.IsDormant || !NPC.syncPosition) && ((Behaviour)base.Agent).get_enabled())
		{
			SetDestination(NPC.ServerPosition);
			return false;
		}
		return true;
	}

	protected override void UpdatePositionAndRotation(Vector3 moveToPosition, float delta)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		base.UpdatePositionAndRotation(moveToPosition, delta);
		UpdateRotation(moveToPosition, delta);
	}

	private void UpdateRotation(Vector3 moveToPosition, float delta)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		if (overrideFacingDirectionMode != 0)
		{
			return;
		}
		if (traversingNavMeshLink)
		{
			Vector3 val = base.Agent.get_destination() - base.BaseEntity.ServerPosition;
			if (((Vector3)(ref val)).get_sqrMagnitude() > 1f)
			{
				val = currentNavMeshLinkEndPos - base.BaseEntity.ServerPosition;
			}
			((Vector3)(ref val)).get_sqrMagnitude();
			_ = 0.001f;
			return;
		}
		Vector3 val2 = base.Agent.get_destination() - base.BaseEntity.ServerPosition;
		if (((Vector3)(ref val2)).get_sqrMagnitude() > 1f)
		{
			val2 = base.Agent.get_desiredVelocity();
			Vector3 normalized = ((Vector3)(ref val2)).get_normalized();
			if (((Vector3)(ref normalized)).get_sqrMagnitude() > 0.001f)
			{
				base.BaseEntity.ServerRotation = Quaternion.LookRotation(normalized);
			}
		}
	}

	public override void ApplyFacingDirectionOverride()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		base.ApplyFacingDirectionOverride();
		base.BaseEntity.ServerRotation = Quaternion.LookRotation(base.FacingDirectionOverride);
	}

	public override bool IsSwimming()
	{
		if (!AI.npcswimming)
		{
			return false;
		}
		if ((Object)(object)NPC != (Object)null)
		{
			return NPC.swimming;
		}
		return false;
	}
}
