using System.Collections;
using UnityEngine;

namespace Rust.Ai
{
	public class CoverPoint
	{
		public enum CoverType
		{
			Full,
			Partial,
			None
		}

		public CoverType NormalCoverType;

		public bool IsDynamic;

		public Transform SourceTransform;

		private Vector3 _staticPosition;

		private Vector3 _staticNormal;

		public CoverPointVolume Volume { get; private set; }

		public Vector3 Position
		{
			get
			{
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				if (IsDynamic && (Object)(object)SourceTransform != (Object)null)
				{
					return SourceTransform.get_position();
				}
				return _staticPosition;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				_staticPosition = value;
			}
		}

		public Vector3 Normal
		{
			get
			{
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				if (IsDynamic && (Object)(object)SourceTransform != (Object)null)
				{
					return SourceTransform.get_forward();
				}
				return _staticNormal;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				_staticNormal = value;
			}
		}

		public BaseEntity ReservedFor { get; set; }

		public bool IsReserved => (Object)(object)ReservedFor != (Object)null;

		public bool IsCompromised { get; set; }

		public float Score { get; set; }

		public bool IsValidFor(BaseEntity entity)
		{
			if (!IsCompromised)
			{
				if (!((Object)(object)ReservedFor == (Object)null))
				{
					return (Object)(object)ReservedFor == (Object)(object)entity;
				}
				return true;
			}
			return false;
		}

		public CoverPoint(CoverPointVolume volume, float score)
		{
			Volume = volume;
			Score = score;
		}

		public void CoverIsCompromised(float cooldown)
		{
			if (!IsCompromised && (Object)(object)Volume != (Object)null)
			{
				((MonoBehaviour)Volume).StartCoroutine(StartCooldown(cooldown));
			}
		}

		private IEnumerator StartCooldown(float cooldown)
		{
			IsCompromised = true;
			yield return CoroutineEx.waitForSeconds(cooldown);
			IsCompromised = false;
		}

		public bool ProvidesCoverFromPoint(Vector3 point, float arcThreshold)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = Position - point;
			Vector3 normalized = ((Vector3)(ref val)).get_normalized();
			return Vector3.Dot(Normal, normalized) < arcThreshold;
		}
	}
}
