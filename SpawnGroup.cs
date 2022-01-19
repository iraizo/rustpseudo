using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using UnityEngine;

public class SpawnGroup : BaseMonoBehaviour, IServerComponent, ISpawnPointUser, ISpawnGroup
{
	[Serializable]
	public class SpawnEntry
	{
		public GameObjectRef prefab;

		public int weight = 1;

		public bool mobile;
	}

	[InspectorFlags]
	public MonumentTier Tier = (MonumentTier)(-1);

	public List<SpawnEntry> prefabs;

	public int maxPopulation = 5;

	public int numToSpawnPerTickMin = 1;

	public int numToSpawnPerTickMax = 2;

	public float respawnDelayMin = 10f;

	public float respawnDelayMax = 20f;

	public bool wantsInitialSpawn = true;

	public bool temporary;

	public bool forceInitialSpawn;

	public bool preventDuplicates;

	protected bool fillOnSpawn;

	protected BaseSpawnPoint[] spawnPoints;

	private List<SpawnPointInstance> spawnInstances = new List<SpawnPointInstance>();

	private LocalClock spawnClock = new LocalClock();

	public int currentPopulation => spawnInstances.Count;

	public virtual bool WantsInitialSpawn()
	{
		return wantsInitialSpawn;
	}

	public virtual bool WantsTimedSpawn()
	{
		return respawnDelayMax != float.PositiveInfinity;
	}

	public float GetSpawnDelta()
	{
		return (respawnDelayMax + respawnDelayMin) * 0.5f / SpawnHandler.PlayerScale(ConVar.Spawn.player_scale);
	}

	public float GetSpawnVariance()
	{
		return (respawnDelayMax - respawnDelayMin) * 0.5f / SpawnHandler.PlayerScale(ConVar.Spawn.player_scale);
	}

	protected void Awake()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)TerrainMeta.TopologyMap == (Object)null)
		{
			return;
		}
		int topology = TerrainMeta.TopologyMap.GetTopology(((Component)this).get_transform().get_position());
		int num = 469762048;
		int num2 = MonumentInfo.TierToMask(Tier);
		if (num2 == num || (num2 & topology) != 0)
		{
			spawnPoints = ((Component)this).GetComponentsInChildren<BaseSpawnPoint>();
			if (WantsTimedSpawn())
			{
				spawnClock.Add(GetSpawnDelta(), GetSpawnVariance(), Spawn);
			}
			if (!temporary && Object.op_Implicit((Object)(object)SingletonComponent<SpawnHandler>.Instance))
			{
				SingletonComponent<SpawnHandler>.Instance.SpawnGroups.Add(this);
			}
			if (forceInitialSpawn)
			{
				((FacepunchBehaviour)this).Invoke((Action)SpawnInitial, 1f);
			}
		}
	}

	public void Fill()
	{
		Spawn(maxPopulation);
	}

	public void Clear()
	{
		foreach (SpawnPointInstance spawnInstance in spawnInstances)
		{
			BaseEntity baseEntity = ((Component)spawnInstance).get_gameObject().ToBaseEntity();
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				baseEntity.Kill();
			}
		}
		spawnInstances.Clear();
	}

	public bool HasSpawned(uint prefabID)
	{
		foreach (SpawnPointInstance spawnInstance in spawnInstances)
		{
			BaseEntity baseEntity = ((Component)spawnInstance).get_gameObject().ToBaseEntity();
			if (Object.op_Implicit((Object)(object)baseEntity) && baseEntity.prefabID == prefabID)
			{
				return true;
			}
		}
		return false;
	}

	public virtual void SpawnInitial()
	{
		if (wantsInitialSpawn)
		{
			if (fillOnSpawn)
			{
				Spawn(maxPopulation);
			}
			else
			{
				Spawn();
			}
		}
	}

	public void SpawnRepeating()
	{
		for (int i = 0; i < spawnClock.events.Count; i++)
		{
			LocalClock.TimedEvent value = spawnClock.events[i];
			if (Time.get_time() > value.time)
			{
				value.delta = GetSpawnDelta();
				value.variance = GetSpawnVariance();
				spawnClock.events[i] = value;
			}
		}
		spawnClock.Tick();
	}

	public void ObjectSpawned(SpawnPointInstance instance)
	{
		spawnInstances.Add(instance);
	}

	public void ObjectRetired(SpawnPointInstance instance)
	{
		spawnInstances.Remove(instance);
	}

	public void DelayedSpawn()
	{
		((FacepunchBehaviour)this).Invoke((Action)Spawn, 1f);
	}

	public void Spawn()
	{
		Spawn(Random.Range(numToSpawnPerTickMin, numToSpawnPerTickMax + 1));
	}

	protected virtual void Spawn(int numToSpawn)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		numToSpawn = Mathf.Min(numToSpawn, maxPopulation - currentPopulation);
		for (int i = 0; i < numToSpawn; i++)
		{
			GameObjectRef prefab = GetPrefab();
			if (prefab == null || string.IsNullOrEmpty(prefab.guid))
			{
				continue;
			}
			Vector3 pos;
			Quaternion rot;
			BaseSpawnPoint spawnPoint = GetSpawnPoint(prefab, out pos, out rot);
			if (!Object.op_Implicit((Object)(object)spawnPoint))
			{
				continue;
			}
			BaseEntity baseEntity = GameManager.server.CreateEntity(prefab.resourcePath, pos, rot, startActive: false);
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				if (baseEntity.enableSaving && !(spawnPoint is SpaceCheckingSpawnPoint))
				{
					baseEntity.enableSaving = false;
				}
				((Component)baseEntity).get_gameObject().AwakeFromInstantiate();
				baseEntity.Spawn();
				PostSpawnProcess(baseEntity, spawnPoint);
				SpawnPointInstance spawnPointInstance = ((Component)baseEntity).get_gameObject().AddComponent<SpawnPointInstance>();
				spawnPointInstance.parentSpawnPointUser = this;
				spawnPointInstance.parentSpawnPoint = spawnPoint;
				spawnPointInstance.Notify();
			}
		}
	}

	protected virtual void PostSpawnProcess(BaseEntity entity, BaseSpawnPoint spawnPoint)
	{
	}

	protected GameObjectRef GetPrefab()
	{
		float num = prefabs.Sum((SpawnEntry x) => (!preventDuplicates || !HasSpawned(x.prefab.resourceID)) ? x.weight : 0);
		if (num == 0f)
		{
			return null;
		}
		float num2 = Random.Range(0f, num);
		foreach (SpawnEntry prefab in prefabs)
		{
			int num3 = ((!preventDuplicates || !HasSpawned(prefab.prefab.resourceID)) ? prefab.weight : 0);
			if ((num2 -= (float)num3) <= 0f)
			{
				return prefab.prefab;
			}
		}
		return prefabs[prefabs.Count - 1].prefab;
	}

	protected virtual BaseSpawnPoint GetSpawnPoint(GameObjectRef prefabRef, out Vector3 pos, out Quaternion rot)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		BaseSpawnPoint baseSpawnPoint = null;
		pos = Vector3.get_zero();
		rot = Quaternion.get_identity();
		int num = Random.Range(0, spawnPoints.Length);
		for (int i = 0; i < spawnPoints.Length; i++)
		{
			BaseSpawnPoint baseSpawnPoint2 = spawnPoints[(num + i) % spawnPoints.Length];
			if (!((Object)(object)baseSpawnPoint2 == (Object)null) && baseSpawnPoint2.IsAvailableTo(prefabRef))
			{
				baseSpawnPoint = baseSpawnPoint2;
				break;
			}
		}
		if (Object.op_Implicit((Object)(object)baseSpawnPoint))
		{
			baseSpawnPoint.GetLocation(out pos, out rot);
		}
		return baseSpawnPoint;
	}

	protected virtual void OnDrawGizmos()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.set_color(new Color(1f, 1f, 0f, 1f));
		Gizmos.DrawSphere(((Component)this).get_transform().get_position(), 0.25f);
	}
}
