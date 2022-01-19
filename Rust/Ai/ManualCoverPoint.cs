using UnityEngine;

namespace Rust.Ai
{
	public class ManualCoverPoint : FacepunchBehaviour
	{
		public bool IsDynamic;

		public float Score = 2f;

		public CoverPointVolume Volume;

		public Vector3 Normal;

		public CoverPoint.CoverType NormalCoverType;

		public Vector3 Position => ((Component)this).get_transform().get_position();

		public float DirectionMagnitude
		{
			get
			{
				if ((Object)(object)Volume != (Object)null)
				{
					return Volume.CoverPointRayLength;
				}
				return 1f;
			}
		}

		private void Awake()
		{
			if ((Object)(object)((Component)this).get_transform().get_parent() != (Object)null)
			{
				Volume = ((Component)((Component)this).get_transform().get_parent()).GetComponent<CoverPointVolume>();
			}
		}

		public CoverPoint ToCoverPoint(CoverPointVolume volume)
		{
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			Volume = volume;
			if (IsDynamic)
			{
				CoverPoint obj = new CoverPoint(Volume, Score)
				{
					IsDynamic = true,
					SourceTransform = ((Component)this).get_transform(),
					NormalCoverType = NormalCoverType
				};
				Transform transform = ((Component)this).get_transform();
				obj.Position = ((transform != null) ? transform.get_position() : Vector3.get_zero());
				return obj;
			}
			Vector3 val = ((Component)this).get_transform().get_rotation() * Normal;
			Vector3 normalized = ((Vector3)(ref val)).get_normalized();
			return new CoverPoint(Volume, Score)
			{
				IsDynamic = false,
				Position = ((Component)this).get_transform().get_position(),
				Normal = normalized,
				NormalCoverType = NormalCoverType
			};
		}

		public ManualCoverPoint()
			: this()
		{
		}
	}
}
