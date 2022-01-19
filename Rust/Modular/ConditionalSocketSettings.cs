using System;

namespace Rust.Modular
{
	[Serializable]
	public class ConditionalSocketSettings
	{
		public enum LocationCondition
		{
			Middle,
			Front,
			Back,
			NotMiddle,
			NotFront,
			NotBack
		}

		public bool restrictOnLocation;

		public LocationCondition locationRestriction;

		public bool restrictOnWheel;

		public ModularVehicleSocket.SocketWheelType wheelRestriction;

		public bool HasSocketRestrictions
		{
			get
			{
				if (!restrictOnLocation)
				{
					return restrictOnWheel;
				}
				return true;
			}
		}
	}
}
