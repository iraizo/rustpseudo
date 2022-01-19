using System;
using UnityEngine;

namespace Rust.Modular
{
	[Serializable]
	public class VehicleModuleSlidingComponent
	{
		[Serializable]
		public class SlidingPart
		{
			public Transform transform;

			public Vector3 openPosition;

			public Vector3 closedPosition;
		}

		public string interactionColliderName = "MyCollider";

		public BaseEntity.Flags flag_SliderOpen = BaseEntity.Flags.Reserved3;

		public float moveTime = 1f;

		public SlidingPart[] slidingParts;

		public SoundDefinition openSoundDef;

		public SoundDefinition closeSoundDef;

		private float positionPercent;

		public bool WantsOpenPos(BaseEntity parentEntity)
		{
			return parentEntity.HasFlag(flag_SliderOpen);
		}

		public void Use(BaseVehicleModule parentModule)
		{
			parentModule.SetFlag(flag_SliderOpen, !WantsOpenPos(parentModule));
		}

		public void ServerUpdateTick(BaseVehicleModule parentModule)
		{
			CheckPosition(parentModule, Time.get_fixedDeltaTime());
		}

		private void CheckPosition(BaseEntity parentEntity, float dt)
		{
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			bool flag = WantsOpenPos(parentEntity);
			if ((flag && positionPercent == 1f) || (!flag && positionPercent == 0f))
			{
				return;
			}
			float num = (flag ? (dt / moveTime) : (0f - dt / moveTime));
			positionPercent = Mathf.Clamp01(positionPercent + num);
			SlidingPart[] array = slidingParts;
			foreach (SlidingPart slidingPart in array)
			{
				if (!((Object)(object)slidingPart.transform == (Object)null))
				{
					slidingPart.transform.set_localPosition(Vector3.Lerp(slidingPart.closedPosition, slidingPart.openPosition, positionPercent));
				}
			}
		}
	}
}
