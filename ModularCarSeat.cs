using UnityEngine;

public class ModularCarSeat : BaseVehicleSeat
{
	[SerializeField]
	private bool supportsMouseSteer;

	[SerializeField]
	private Vector3 leftFootIKPos;

	[SerializeField]
	private Vector3 rightFootIKPos;

	[SerializeField]
	private Vector3 leftHandIKPos;

	[SerializeField]
	private Vector3 rightHandIKPos;

	public float providesComfort;

	public VehicleModuleSeating associatedSeatingModule;

	public override bool CanSwapToThis(BasePlayer player)
	{
		if (associatedSeatingModule.DoorsAreLockable)
		{
			ModularCar modularCar = associatedSeatingModule.Vehicle as ModularCar;
			if ((Object)(object)modularCar != (Object)null)
			{
				return modularCar.PlayerCanUseThis(player, ModularCarLock.LockType.Door);
			}
		}
		return true;
	}

	public override float GetComfort()
	{
		return providesComfort;
	}
}
