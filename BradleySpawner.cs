using System;
using ConVar;
using Rust;
using UnityEngine;

public class BradleySpawner : MonoBehaviour, IServerComponent
{
	public BasePath path;

	public GameObjectRef bradleyPrefab;

	[NonSerialized]
	public BradleyAPC spawned;

	public bool initialSpawn;

	public float minRespawnTimeMinutes = 5f;

	public float maxRespawnTimeMinutes = 5f;

	public static BradleySpawner singleton;

	private bool pendingRespawn;

	public void Start()
	{
		singleton = this;
		((MonoBehaviour)this).Invoke("DelayedStart", 3f);
	}

	public void DelayedStart()
	{
		if (initialSpawn)
		{
			DoRespawn();
		}
		((MonoBehaviour)this).InvokeRepeating("CheckIfRespawnNeeded", 0f, 5f);
	}

	public void CheckIfRespawnNeeded()
	{
		if (!pendingRespawn && ((Object)(object)spawned == (Object)null || !spawned.IsAlive()))
		{
			ScheduleRespawn();
		}
	}

	public void ScheduleRespawn()
	{
		((MonoBehaviour)this).CancelInvoke("DoRespawn");
		((MonoBehaviour)this).Invoke("DoRespawn", Random.Range(Bradley.respawnDelayMinutes - Bradley.respawnDelayVariance, Bradley.respawnDelayMinutes + Bradley.respawnDelayVariance) * 60f);
		pendingRespawn = true;
	}

	public void DoRespawn()
	{
		if (!Application.isLoading && !Application.isLoadingSave)
		{
			SpawnBradley();
		}
		pendingRespawn = false;
	}

	public void SpawnBradley()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)spawned != (Object)null)
		{
			Debug.LogWarning((object)"Bradley attempting to spawn but one already exists!");
		}
		else if (Bradley.enabled)
		{
			Vector3 position = ((Component)path.interestZones[Random.Range(0, path.interestZones.Count)]).get_transform().get_position();
			BaseEntity baseEntity = GameManager.server.CreateEntity(bradleyPrefab.resourcePath, position);
			BradleyAPC component = ((Component)baseEntity).GetComponent<BradleyAPC>();
			if (Object.op_Implicit((Object)(object)component))
			{
				baseEntity.Spawn();
				component.InstallPatrolPath(path);
			}
			else
			{
				baseEntity.Kill();
			}
			Debug.Log((object)("BradleyAPC Spawned at :" + position));
			spawned = component;
		}
	}

	public BradleySpawner()
		: this()
	{
	}
}
