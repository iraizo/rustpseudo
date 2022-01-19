using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public class VehicleSpawner : BaseEntity
{
	[Serializable]
	public class SpawnPair
	{
		public string message;

		public GameObjectRef prefabToSpawn;
	}

	public float spawnNudgeRadius = 6f;

	public float cleanupRadius = 10f;

	public float occupyRadius = 5f;

	public SpawnPair[] objectsToSpawn;

	public Transform spawnOffset;

	public float safeRadius = 10f;

	public virtual int GetOccupyLayer()
	{
		return 32768;
	}

	public BaseVehicle GetVehicleOccupying()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		BaseVehicle result = null;
		List<BaseVehicle> list = Pool.GetList<BaseVehicle>();
		Vis.Entities(((Component)spawnOffset).get_transform().get_position(), occupyRadius, list, GetOccupyLayer(), (QueryTriggerInteraction)1);
		if (list.Count > 0)
		{
			result = list[0];
		}
		Pool.FreeList<BaseVehicle>(ref list);
		return result;
	}

	public bool IsPadOccupied()
	{
		BaseVehicle vehicleOccupying = GetVehicleOccupying();
		if ((Object)(object)vehicleOccupying != (Object)null)
		{
			return !vehicleOccupying.IsDespawnEligable();
		}
		return false;
	}

	public override void OnEntityMessage(BaseEntity from, string msg)
	{
		BasePlayer newOwner = null;
		NPCTalking component = ((Component)from).GetComponent<NPCTalking>();
		if (Object.op_Implicit((Object)(object)component))
		{
			newOwner = component.GetActionPlayer();
		}
		SpawnPair[] array = objectsToSpawn;
		foreach (SpawnPair spawnPair in array)
		{
			if (msg == spawnPair.message)
			{
				SpawnVehicle(spawnPair.prefabToSpawn.resourcePath, newOwner);
				break;
			}
		}
	}

	public BaseVehicle SpawnVehicle(string prefabToSpawn, BasePlayer newOwner)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		CleanupArea(cleanupRadius);
		NudgePlayersInRadius(spawnNudgeRadius);
		BaseEntity baseEntity = GameManager.server.CreateEntity(prefabToSpawn, ((Component)spawnOffset).get_transform().get_position(), ((Component)spawnOffset).get_transform().get_rotation());
		baseEntity.Spawn();
		BaseVehicle component = ((Component)baseEntity).GetComponent<BaseVehicle>();
		EntityFuelSystem fuelSystem = component.GetFuelSystem();
		if ((Object)(object)newOwner != (Object)null)
		{
			component.SetupOwner(newOwner, ((Component)spawnOffset).get_transform().get_position(), safeRadius);
		}
		fuelSystem?.AddStartingFuel(component.StartingFuelUnits());
		return component;
	}

	public void CleanupArea(float radius)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		List<BaseVehicle> list = Pool.GetList<BaseVehicle>();
		Vis.Entities(((Component)spawnOffset).get_transform().get_position(), radius, list, 32768, (QueryTriggerInteraction)2);
		foreach (BaseVehicle item in list)
		{
			if (!item.isClient && !item.IsDestroyed)
			{
				item.Kill();
			}
		}
		List<ServerGib> list2 = Pool.GetList<ServerGib>();
		Vis.Entities(((Component)spawnOffset).get_transform().get_position(), radius, list2, 67108865, (QueryTriggerInteraction)2);
		foreach (ServerGib item2 in list2)
		{
			if (!item2.isClient)
			{
				item2.Kill();
			}
		}
		Pool.FreeList<BaseVehicle>(ref list);
		Pool.FreeList<ServerGib>(ref list2);
	}

	public void NudgePlayersInRadius(float radius)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		List<BasePlayer> list = Pool.GetList<BasePlayer>();
		Vis.Entities(((Component)spawnOffset).get_transform().get_position(), radius, list, 131072, (QueryTriggerInteraction)2);
		foreach (BasePlayer item in list)
		{
			if (!item.IsNpc && !item.isMounted && item.IsConnected)
			{
				Vector3 position = ((Component)spawnOffset).get_transform().get_position();
				position += Vector3Ex.Direction2D(((Component)item).get_transform().get_position(), ((Component)spawnOffset).get_transform().get_position()) * radius;
				position += Vector3.get_up() * 0.1f;
				item.MovePosition(position);
				item.ClientRPCPlayer<Vector3>(null, item, "ForcePositionTo", position);
			}
		}
		Pool.FreeList<BasePlayer>(ref list);
	}
}
