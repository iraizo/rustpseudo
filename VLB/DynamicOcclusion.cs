using System;
using UnityEngine;

namespace VLB
{
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(VolumetricLightBeam))]
	[HelpURL("http://saladgamer.com/vlb-doc/comp-dynocclusion/")]
	public class DynamicOcclusion : MonoBehaviour
	{
		private enum Direction
		{
			Up,
			Right,
			Down,
			Left
		}

		public LayerMask layerMask = LayerMask.op_Implicit(-1);

		public float minOccluderArea;

		public int waitFrameCount = 3;

		public float minSurfaceRatio = 0.5f;

		public float maxSurfaceDot = 0.25f;

		public PlaneAlignment planeAlignment;

		public float planeOffset = 0.1f;

		private VolumetricLightBeam m_Master;

		private int m_FrameCountToWait;

		private float m_RangeMultiplier = 1f;

		private uint m_PrevNonSubHitDirectionId;

		private void OnValidate()
		{
			minOccluderArea = Mathf.Max(minOccluderArea, 0f);
			waitFrameCount = Mathf.Clamp(waitFrameCount, 1, 60);
		}

		private void OnEnable()
		{
			m_Master = ((Component)this).GetComponent<VolumetricLightBeam>();
			Debug.Assert(Object.op_Implicit((Object)(object)m_Master));
		}

		private void OnDisable()
		{
			SetHitNull();
		}

		private void Start()
		{
			if (Application.get_isPlaying())
			{
				TriggerZone component = ((Component)this).GetComponent<TriggerZone>();
				if (Object.op_Implicit((Object)(object)component))
				{
					m_RangeMultiplier = Mathf.Max(1f, component.rangeMultiplier);
				}
			}
		}

		private void LateUpdate()
		{
			if (m_FrameCountToWait <= 0)
			{
				ProcessRaycasts();
				m_FrameCountToWait = waitFrameCount;
			}
			m_FrameCountToWait--;
		}

		private Vector3 GetRandomVectorAround(Vector3 direction, float angleDiff)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			float num = angleDiff * 0.5f;
			return Quaternion.Euler(Random.Range(0f - num, num), Random.Range(0f - num, num), Random.Range(0f - num, num)) * direction;
		}

		private RaycastHit GetBestHit(Vector3 rayPos, Vector3 rayDir)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			RaycastHit[] array = Physics.RaycastAll(rayPos, rayDir, m_Master.fadeEnd * m_RangeMultiplier, ((LayerMask)(ref layerMask)).get_value());
			int num = -1;
			float num2 = float.MaxValue;
			for (int i = 0; i < array.Length; i++)
			{
				if (!((RaycastHit)(ref array[i])).get_collider().get_isTrigger() && ((RaycastHit)(ref array[i])).get_collider().get_bounds().GetMaxArea2D() >= minOccluderArea && ((RaycastHit)(ref array[i])).get_distance() < num2)
				{
					num2 = ((RaycastHit)(ref array[i])).get_distance();
					num = i;
				}
			}
			if (num != -1)
			{
				return array[num];
			}
			return default(RaycastHit);
		}

		private Vector3 GetDirection(uint dirInt)
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			dirInt %= (uint)Enum.GetValues(typeof(Direction)).Length;
			return (Vector3)(dirInt switch
			{
				0u => ((Component)this).get_transform().get_up(), 
				1u => ((Component)this).get_transform().get_right(), 
				2u => -((Component)this).get_transform().get_up(), 
				3u => -((Component)this).get_transform().get_right(), 
				_ => Vector3.get_zero(), 
			});
		}

		private bool IsHitValid(RaycastHit hit)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			if (Object.op_Implicit((Object)(object)((RaycastHit)(ref hit)).get_collider()))
			{
				return Vector3.Dot(((RaycastHit)(ref hit)).get_normal(), -((Component)this).get_transform().get_forward()) >= maxSurfaceDot;
			}
			return false;
		}

		private void ProcessRaycasts()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			RaycastHit hit = GetBestHit(((Component)this).get_transform().get_position(), ((Component)this).get_transform().get_forward());
			if (IsHitValid(hit))
			{
				if (minSurfaceRatio > 0.5f)
				{
					for (uint num = 0u; num < (uint)Enum.GetValues(typeof(Direction)).Length; num++)
					{
						Vector3 direction = GetDirection(num + m_PrevNonSubHitDirectionId);
						Vector3 val = ((Component)this).get_transform().get_position() + direction * m_Master.coneRadiusStart * (minSurfaceRatio * 2f - 1f);
						Vector3 val2 = ((Component)this).get_transform().get_position() + ((Component)this).get_transform().get_forward() * m_Master.fadeEnd + direction * m_Master.coneRadiusEnd * (minSurfaceRatio * 2f - 1f);
						RaycastHit bestHit = GetBestHit(val, val2 - val);
						if (IsHitValid(bestHit))
						{
							if (((RaycastHit)(ref bestHit)).get_distance() > ((RaycastHit)(ref hit)).get_distance())
							{
								hit = bestHit;
							}
							continue;
						}
						m_PrevNonSubHitDirectionId = num;
						SetHitNull();
						return;
					}
				}
				SetHit(hit);
			}
			else
			{
				SetHitNull();
			}
		}

		private void SetHit(RaycastHit hit)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			PlaneAlignment planeAlignment = this.planeAlignment;
			if (planeAlignment != 0 && planeAlignment == PlaneAlignment.Beam)
			{
				SetClippingPlane(new Plane(-((Component)this).get_transform().get_forward(), ((RaycastHit)(ref hit)).get_point()));
			}
			else
			{
				SetClippingPlane(new Plane(((RaycastHit)(ref hit)).get_normal(), ((RaycastHit)(ref hit)).get_point()));
			}
		}

		private void SetHitNull()
		{
			SetClippingPlaneOff();
		}

		private void SetClippingPlane(Plane planeWS)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			planeWS = planeWS.TranslateCustom(((Plane)(ref planeWS)).get_normal() * planeOffset);
			m_Master.SetClippingPlane(planeWS);
		}

		private void SetClippingPlaneOff()
		{
			m_Master.SetClippingPlaneOff();
		}

		public DynamicOcclusion()
			: this()
		{
		}//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)

	}
}
