using System;
using UnityEngine;

public class HorseSpawner : VehicleSpawner
{
	public float respawnDelay = 10f;

	public float respawnDelayVariance = 5f;

	public bool spawnForSale = true;

	public override void ServerInit()
	{
		base.ServerInit();
		((FacepunchBehaviour)this).InvokeRandomized((Action)RespawnHorse, Random.Range(0f, 4f), respawnDelay, respawnDelayVariance);
	}

	public override int GetOccupyLayer()
	{
		return 2048;
	}

	public void RespawnHorse()
	{
		if (!((Object)(object)GetVehicleOccupying() != (Object)null))
		{
			BaseVehicle baseVehicle = SpawnVehicle(objectsToSpawn[0].prefabToSpawn.resourcePath, null);
			if (spawnForSale)
			{
				baseVehicle.SetFlag(Flags.Reserved2, b: true);
			}
		}
	}
}
