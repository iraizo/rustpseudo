using System;
using Facepunch.Nexus;
using UnityEngine;

public abstract class ZoneController
{
	protected readonly NexusZoneClient ZoneClient;

	public static ZoneController Instance { get; set; }

	protected ZoneController(NexusZoneClient zoneClient)
	{
		ZoneClient = zoneClient ?? throw new ArgumentNullException("zoneClient");
	}

	public abstract string ChooseSpawnZone(ulong steamId, bool isAlreadyAssignedToThisZone);

	public virtual (Vector3, Quaternion) ChooseTransferDestination(string sourceZone, string method, string from, string to, Vector3 position, Quaternion rotation)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		if (method == "ferry")
		{
			if ((Object)(object)SingletonComponent<NexusDock>.Instance != (Object)null)
			{
				Transform arrival = SingletonComponent<NexusDock>.Instance.Arrival;
				return (arrival.get_position(), arrival.get_rotation());
			}
			Debug.LogError((object)("Received a ferry transfer from '" + sourceZone + "' but couldn't find a dock"));
			return ChooseTransferFallbackDestination();
		}
		if (method == "console")
		{
			BasePlayer.SpawnPoint spawnPoint = ServerMgr.FindSpawnPoint();
			return (spawnPoint.pos, spawnPoint.rot);
		}
		if (method != "ocean")
		{
			Debug.LogError((object)("Unhandled transfer method '" + method + "', using default destination"));
		}
		if (!NexusServer.TryGetIsland(sourceZone, out var island))
		{
			Debug.LogError((object)("Couldn't find nexus island for source zone '" + sourceZone + "'"));
			return ChooseTransferFallbackDestination();
		}
		if (!island.TryFindPosition(out var position2))
		{
			Debug.LogError((object)("Couldn't find a destination positon for source zone '" + sourceZone + "'"));
			return ChooseTransferFallbackDestination();
		}
		return (position2, ((Component)island).get_transform().get_rotation());
	}

	protected virtual (Vector3, Quaternion) ChooseTransferFallbackDestination()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)World.Size * 1.5f;
		Vector3 val = Vector3Ex.Scale(Vector3Ex.XZ3D(Random.get_insideUnitCircle()), num, 0f, num);
		Vector3 val2 = Vector3Ex.WithY(val, WaterSystem.GetHeight(val));
		Vector3 val3 = Vector3Ex.WithY(TerrainMeta.Center, val2.y) - val2;
		Quaternion item = Quaternion.LookRotation(((Vector3)(ref val3)).get_normalized(), Vector3.get_up());
		return (val2, item);
	}

	public virtual bool CanRespawnAcrossZones(BasePlayer player)
	{
		return true;
	}
}
