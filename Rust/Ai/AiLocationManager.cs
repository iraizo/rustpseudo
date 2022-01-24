using System.Collections.Generic;
using ConVar;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai
{
	public class AiLocationManager : FacepunchBehaviour, IServerComponent
	{
		public static List<AiLocationManager> Managers = new List<AiLocationManager>();

		[SerializeField]
		public AiLocationSpawner MainSpawner;

		[SerializeField]
		public AiLocationSpawner.SquadSpawnerLocation LocationWhenMainSpawnerIsNull = AiLocationSpawner.SquadSpawnerLocation.None;

		public Transform CoverPointGroup;

		public Transform PatrolPointGroup;

		public CoverPointVolume DynamicCoverPointVolume;

		public bool SnapCoverPointsToGround;

		private List<PathInterestNode> patrolPoints;

		public AiLocationSpawner.SquadSpawnerLocation LocationType
		{
			get
			{
				if ((Object)(object)MainSpawner != (Object)null)
				{
					return MainSpawner.Location;
				}
				return LocationWhenMainSpawnerIsNull;
			}
		}

		private void Awake()
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			Managers.Add(this);
			if (!SnapCoverPointsToGround)
			{
				return;
			}
			AICoverPoint[] componentsInChildren = ((Component)CoverPointGroup).GetComponentsInChildren<AICoverPoint>();
			NavMeshHit val = default(NavMeshHit);
			foreach (AICoverPoint aICoverPoint in componentsInChildren)
			{
				if (NavMesh.SamplePosition(((Component)aICoverPoint).get_transform().get_position(), ref val, 4f, -1))
				{
					((Component)aICoverPoint).get_transform().set_position(((NavMeshHit)(ref val)).get_position());
				}
			}
		}

		private void OnDestroy()
		{
			Managers.Remove(this);
		}

		public PathInterestNode GetFirstPatrolPointInRange(Vector3 from, float minRange = 10f, float maxRange = 100f)
		{
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)PatrolPointGroup == (Object)null)
			{
				return null;
			}
			if (patrolPoints == null)
			{
				patrolPoints = new List<PathInterestNode>(((Component)PatrolPointGroup).GetComponentsInChildren<PathInterestNode>());
			}
			if (patrolPoints.Count == 0)
			{
				return null;
			}
			float num = minRange * minRange;
			float num2 = maxRange * maxRange;
			foreach (PathInterestNode patrolPoint in patrolPoints)
			{
				Vector3 val = ((Component)patrolPoint).get_transform().get_position() - from;
				float sqrMagnitude = ((Vector3)(ref val)).get_sqrMagnitude();
				if (sqrMagnitude >= num && sqrMagnitude <= num2)
				{
					return patrolPoint;
				}
			}
			return null;
		}

		public PathInterestNode GetRandomPatrolPointInRange(Vector3 from, float minRange = 10f, float maxRange = 100f, PathInterestNode currentPatrolPoint = null)
		{
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)PatrolPointGroup == (Object)null)
			{
				return null;
			}
			if (patrolPoints == null)
			{
				patrolPoints = new List<PathInterestNode>(((Component)PatrolPointGroup).GetComponentsInChildren<PathInterestNode>());
			}
			if (patrolPoints.Count == 0)
			{
				return null;
			}
			float num = minRange * minRange;
			float num2 = maxRange * maxRange;
			for (int i = 0; i < 20; i++)
			{
				PathInterestNode pathInterestNode = patrolPoints[Random.Range(0, patrolPoints.Count)];
				if (Time.get_time() < pathInterestNode.NextVisitTime)
				{
					if ((Object)(object)pathInterestNode == (Object)(object)currentPatrolPoint)
					{
						return null;
					}
					continue;
				}
				Vector3 val = ((Component)pathInterestNode).get_transform().get_position() - from;
				float sqrMagnitude = ((Vector3)(ref val)).get_sqrMagnitude();
				if (sqrMagnitude >= num && sqrMagnitude <= num2)
				{
					pathInterestNode.NextVisitTime = Time.get_time() + ConVar.AI.npc_patrol_point_cooldown;
					return pathInterestNode;
				}
			}
			return null;
		}

		public AiLocationManager()
			: this()
		{
		}
	}
}
