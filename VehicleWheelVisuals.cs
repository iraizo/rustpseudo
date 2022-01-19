using UnityEngine;

public abstract class VehicleWheelVisuals<T> : MonoBehaviour where T : BaseVehicle, VehicleWheelVisuals<T>.IClientWheelUser
{
	public interface IClientWheelUser
	{
		Vector3 Velocity { get; }

		float DriveWheelVelocity { get; }

		float SteerAngle { get; }

		float MaxSteerAngle { get; }

		float GetThrottleInput();
	}

	protected VehicleWheelVisuals()
		: this()
	{
	}
}
