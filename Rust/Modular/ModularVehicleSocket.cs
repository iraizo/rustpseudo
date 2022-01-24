using System;
using UnityEngine;

namespace Rust.Modular
{
	[Serializable]
	public class ModularVehicleSocket
	{
		public enum SocketWheelType
		{
			NoWheel,
			ForwardWheel,
			BackWheel
		}

		public enum SocketLocationType
		{
			Middle,
			Front,
			Back
		}

		[SerializeField]
		private Transform socketTransform;

		[SerializeField]
		private SocketWheelType wheelType;

		[SerializeField]
		private SocketLocationType locationType;

		public Vector3 WorldPosition => socketTransform.get_position();

		public Quaternion WorldRotation => socketTransform.get_rotation();

		public SocketWheelType WheelType => wheelType;

		public SocketLocationType LocationType => locationType;

		public bool ShouldBeActive(ConditionalSocketSettings modelSettings)
		{
			bool flag = true;
			if (modelSettings.restrictOnLocation)
			{
				ConditionalSocketSettings.LocationCondition locationRestriction = modelSettings.locationRestriction;
				switch (LocationType)
				{
				case SocketLocationType.Back:
					flag = locationRestriction == ConditionalSocketSettings.LocationCondition.Back || locationRestriction == ConditionalSocketSettings.LocationCondition.NotFront || locationRestriction == ConditionalSocketSettings.LocationCondition.NotMiddle;
					break;
				case SocketLocationType.Front:
					flag = locationRestriction == ConditionalSocketSettings.LocationCondition.Front || locationRestriction == ConditionalSocketSettings.LocationCondition.NotBack || locationRestriction == ConditionalSocketSettings.LocationCondition.NotMiddle;
					break;
				case SocketLocationType.Middle:
					flag = locationRestriction == ConditionalSocketSettings.LocationCondition.Middle || locationRestriction == ConditionalSocketSettings.LocationCondition.NotFront || locationRestriction == ConditionalSocketSettings.LocationCondition.NotBack;
					break;
				}
			}
			if (flag && modelSettings.restrictOnWheel)
			{
				flag = WheelType == modelSettings.wheelRestriction;
			}
			return flag;
		}
	}
}
