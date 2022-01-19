using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai
{
	public class CoverPointVolume : MonoBehaviour, IServerComponent
	{
		internal enum CoverType
		{
			None,
			Partial,
			Full
		}

		public float DefaultCoverPointScore = 1f;

		public float CoverPointRayLength = 1f;

		public LayerMask CoverLayerMask;

		public Transform BlockerGroup;

		public Transform ManualCoverPointGroup;

		[ServerVar(Help = "cover_point_sample_step_size defines the size of the steps we do horizontally for the cover point volume's cover point generation (smaller steps gives more accurate cover points, but at a higher processing cost). (default: 6.0)")]
		public static float cover_point_sample_step_size = 6f;

		[ServerVar(Help = "cover_point_sample_step_height defines the height of the steps we do vertically for the cover point volume's cover point generation (smaller steps gives more accurate cover points, but at a higher processing cost). (default: 2.0)")]
		public static float cover_point_sample_step_height = 2f;

		public readonly List<CoverPoint> CoverPoints = new List<CoverPoint>();

		private readonly List<CoverPointBlockerVolume> _coverPointBlockers = new List<CoverPointBlockerVolume>();

		private float _dynNavMeshBuildCompletionTime = -1f;

		private int _genAttempts;

		private Bounds bounds = new Bounds(Vector3.get_zero(), Vector3.get_zero());

		public bool repeat => true;

		public float? ExecuteUpdate(float deltaTime, float nextInterval)
		{
			if (CoverPoints.Count == 0)
			{
				if (_dynNavMeshBuildCompletionTime < 0f)
				{
					if ((Object)(object)SingletonComponent<DynamicNavMesh>.Instance == (Object)null || !((Behaviour)SingletonComponent<DynamicNavMesh>.Instance).get_enabled() || !SingletonComponent<DynamicNavMesh>.Instance.IsBuilding)
					{
						_dynNavMeshBuildCompletionTime = Time.get_realtimeSinceStartup();
					}
				}
				else if (_genAttempts < 4 && Time.get_realtimeSinceStartup() - _dynNavMeshBuildCompletionTime > 0.25f)
				{
					GenerateCoverPoints(null);
					if (CoverPoints.Count != 0)
					{
						return null;
					}
					_dynNavMeshBuildCompletionTime = Time.get_realtimeSinceStartup();
					_genAttempts++;
					if (_genAttempts >= 4)
					{
						Object.Destroy((Object)(object)((Component)this).get_gameObject());
						return null;
					}
				}
			}
			return 1f + Random.get_value() * 2f;
		}

		[ContextMenu("Clear Cover Points")]
		private void ClearCoverPoints()
		{
			CoverPoints.Clear();
			_coverPointBlockers.Clear();
		}

		public Bounds GetBounds()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			Vector3 center = ((Bounds)(ref bounds)).get_center();
			if (Mathf.Approximately(((Vector3)(ref center)).get_sqrMagnitude(), 0f))
			{
				bounds = new Bounds(((Component)this).get_transform().get_position(), ((Component)this).get_transform().get_localScale());
			}
			return bounds;
		}

		[ContextMenu("Pre-Generate Cover Points")]
		public void PreGenerateCoverPoints()
		{
			GenerateCoverPoints(null);
		}

		[ContextMenu("Convert to Manual Cover Points")]
		public void ConvertToManualCoverPoints()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			foreach (CoverPoint coverPoint in CoverPoints)
			{
				ManualCoverPoint manualCoverPoint = new GameObject("MCP").AddComponent<ManualCoverPoint>();
				((Component)manualCoverPoint).get_transform().set_localPosition(Vector3.get_zero());
				((Component)manualCoverPoint).get_transform().set_position(coverPoint.Position);
				manualCoverPoint.Normal = coverPoint.Normal;
				manualCoverPoint.NormalCoverType = coverPoint.NormalCoverType;
				manualCoverPoint.Volume = this;
			}
		}

		public void GenerateCoverPoints(Transform coverPointGroup)
		{
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			Time.get_realtimeSinceStartup();
			ClearCoverPoints();
			if ((Object)(object)ManualCoverPointGroup == (Object)null)
			{
				ManualCoverPointGroup = coverPointGroup;
			}
			if ((Object)(object)ManualCoverPointGroup == (Object)null)
			{
				ManualCoverPointGroup = ((Component)this).get_transform();
			}
			if (ManualCoverPointGroup.get_childCount() > 0)
			{
				ManualCoverPoint[] componentsInChildren = ((Component)ManualCoverPointGroup).GetComponentsInChildren<ManualCoverPoint>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					CoverPoint item = componentsInChildren[i].ToCoverPoint(this);
					CoverPoints.Add(item);
				}
			}
			if (_coverPointBlockers.Count == 0 && (Object)(object)BlockerGroup != (Object)null)
			{
				CoverPointBlockerVolume[] componentsInChildren2 = ((Component)BlockerGroup).GetComponentsInChildren<CoverPointBlockerVolume>();
				if (componentsInChildren2 != null && componentsInChildren2.Length != 0)
				{
					_coverPointBlockers.AddRange(componentsInChildren2);
				}
			}
			NavMeshHit val = default(NavMeshHit);
			if (CoverPoints.Count != 0 || !NavMesh.SamplePosition(((Component)this).get_transform().get_position(), ref val, ((Component)this).get_transform().get_localScale().y * cover_point_sample_step_height, -1))
			{
				return;
			}
			Vector3 position = ((Component)this).get_transform().get_position();
			Vector3 val2 = ((Component)this).get_transform().get_lossyScale() * 0.5f;
			NavMeshHit info = default(NavMeshHit);
			for (float num = position.x - val2.x + 1f; num < position.x + val2.x - 1f; num += cover_point_sample_step_size)
			{
				for (float num2 = position.z - val2.z + 1f; num2 < position.z + val2.z - 1f; num2 += cover_point_sample_step_size)
				{
					for (float num3 = position.y - val2.y; num3 < position.y + val2.y; num3 += cover_point_sample_step_height)
					{
						if (!NavMesh.FindClosestEdge(new Vector3(num, num3, num2), ref info, ((NavMeshHit)(ref val)).get_mask()))
						{
							continue;
						}
						((NavMeshHit)(ref info)).set_position(new Vector3(((NavMeshHit)(ref info)).get_position().x, ((NavMeshHit)(ref info)).get_position().y + 0.5f, ((NavMeshHit)(ref info)).get_position().z));
						bool flag = true;
						foreach (CoverPoint coverPoint2 in CoverPoints)
						{
							Vector3 val3 = coverPoint2.Position - ((NavMeshHit)(ref info)).get_position();
							if (((Vector3)(ref val3)).get_sqrMagnitude() < cover_point_sample_step_size * cover_point_sample_step_size)
							{
								flag = false;
								break;
							}
						}
						if (flag)
						{
							CoverPoint coverPoint = CalculateCoverPoint(info);
							if (coverPoint != null)
							{
								CoverPoints.Add(coverPoint);
							}
						}
					}
				}
			}
		}

		private CoverPoint CalculateCoverPoint(NavMeshHit info)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			RaycastHit rayHit;
			CoverType coverType = ProvidesCoverInDir(new Ray(((NavMeshHit)(ref info)).get_position(), -((NavMeshHit)(ref info)).get_normal()), CoverPointRayLength, out rayHit);
			if (coverType == CoverType.None)
			{
				return null;
			}
			CoverPoint coverPoint = new CoverPoint(this, DefaultCoverPointScore)
			{
				Position = ((NavMeshHit)(ref info)).get_position(),
				Normal = -((NavMeshHit)(ref info)).get_normal()
			};
			switch (coverType)
			{
			case CoverType.Full:
				coverPoint.NormalCoverType = CoverPoint.CoverType.Full;
				break;
			case CoverType.Partial:
				coverPoint.NormalCoverType = CoverPoint.CoverType.Partial;
				break;
			}
			return coverPoint;
		}

		internal CoverType ProvidesCoverInDir(Ray ray, float maxDistance, out RaycastHit rayHit)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			rayHit = default(RaycastHit);
			if (Vector3Ex.IsNaNOrInfinity(((Ray)(ref ray)).get_origin()))
			{
				return CoverType.None;
			}
			if (Vector3Ex.IsNaNOrInfinity(((Ray)(ref ray)).get_direction()))
			{
				return CoverType.None;
			}
			if (((Ray)(ref ray)).get_direction() == Vector3.get_zero())
			{
				return CoverType.None;
			}
			((Ray)(ref ray)).set_origin(((Ray)(ref ray)).get_origin() + PlayerEyes.EyeOffset);
			if (Physics.Raycast(((Ray)(ref ray)).get_origin(), ((Ray)(ref ray)).get_direction(), ref rayHit, maxDistance, LayerMask.op_Implicit(CoverLayerMask)))
			{
				return CoverType.Full;
			}
			((Ray)(ref ray)).set_origin(((Ray)(ref ray)).get_origin() + PlayerEyes.DuckOffset);
			if (Physics.Raycast(((Ray)(ref ray)).get_origin(), ((Ray)(ref ray)).get_direction(), ref rayHit, maxDistance, LayerMask.op_Implicit(CoverLayerMask)))
			{
				return CoverType.Partial;
			}
			return CoverType.None;
		}

		public bool Contains(Vector3 point)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			Bounds val = default(Bounds);
			((Bounds)(ref val))._002Ector(((Component)this).get_transform().get_position(), ((Component)this).get_transform().get_localScale());
			return ((Bounds)(ref val)).Contains(point);
		}

		public CoverPointVolume()
			: this()
		{
		}//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)

	}
}
