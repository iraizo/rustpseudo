using UnityEngine;

public class BaseVehicleMountPoint : BaseMountable
{
	public override bool DirectlyMountable()
	{
		return false;
	}

	public override BaseVehicle VehicleParent()
	{
		BaseVehicle baseVehicle = GetParentEntity() as BaseVehicle;
		while ((Object)(object)baseVehicle != (Object)null && !baseVehicle.IsVehicleRoot())
		{
			BaseVehicle baseVehicle2 = baseVehicle.GetParentEntity() as BaseVehicle;
			if ((Object)(object)baseVehicle2 == (Object)null)
			{
				return baseVehicle;
			}
			baseVehicle = baseVehicle2;
		}
		return baseVehicle;
	}

	public override bool BlocksWaterFor(BasePlayer player)
	{
		BaseVehicle baseVehicle = VehicleParent();
		if ((Object)(object)baseVehicle == (Object)null)
		{
			return false;
		}
		return baseVehicle.BlocksWaterFor(player);
	}

	public override float WaterFactorForPlayer(BasePlayer player)
	{
		BaseVehicle baseVehicle = VehicleParent();
		if ((Object)(object)baseVehicle == (Object)null)
		{
			return 0f;
		}
		return baseVehicle.WaterFactorForPlayer(player);
	}

	public override float AirFactor()
	{
		BaseVehicle baseVehicle = VehicleParent();
		if ((Object)(object)baseVehicle == (Object)null)
		{
			return 0f;
		}
		return baseVehicle.AirFactor();
	}
}
