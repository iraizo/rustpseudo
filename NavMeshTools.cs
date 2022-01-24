using System;
using System.Collections;
using System.Collections.Generic;
using ConVar;
using Rust.Ai;
using UnityEngine;
using UnityEngine.AI;

public static class NavMeshTools
{
	public static IEnumerator CollectSourcesAsync(Bounds bounds, int mask, NavMeshCollectGeometry geometry, int area, bool useBakedTerrainMesh, int cellSize, List<NavMeshBuildSource> sources, Action<List<NavMeshBuildSource>> append, Action callback)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		while (!AI.move && !AiManager.nav_wait)
		{
			yield return CoroutineEx.waitForSeconds(1f);
		}
		float time = Time.get_realtimeSinceStartup();
		Debug.Log((object)"Starting Navmesh Source Collecting");
		mask = ((!useBakedTerrainMesh) ? (mask | 0x800000) : (mask & -8388609));
		List<NavMeshBuildMarkup> list = new List<NavMeshBuildMarkup>();
		NavMeshBuilder.CollectSources(bounds, mask, geometry, area, list, sources);
		if (useBakedTerrainMesh && (Object)(object)TerrainMeta.HeightMap != (Object)null)
		{
			for (float x = 0f - ((Bounds)(ref bounds)).get_extents().x; x < ((Bounds)(ref bounds)).get_extents().x - (float)(cellSize / 2); x += (float)cellSize)
			{
				for (float z = 0f - ((Bounds)(ref bounds)).get_extents().z; z < ((Bounds)(ref bounds)).get_extents().z - (float)(cellSize / 2); z += (float)cellSize)
				{
					AsyncTerrainNavMeshBake terrainSource = new AsyncTerrainNavMeshBake(new Vector3(x, 0f, z), cellSize, cellSize, normal: false, alpha: true);
					yield return terrainSource;
					sources.Add(terrainSource.CreateNavMeshBuildSource(area));
				}
			}
		}
		append?.Invoke(sources);
		Debug.Log((object)$"Navmesh Source Collecting took {Time.get_realtimeSinceStartup() - time:0.00} seconds");
		callback?.Invoke();
	}

	public static IEnumerator CollectSourcesAsync(Transform root, int mask, NavMeshCollectGeometry geometry, int area, List<NavMeshBuildSource> sources, Action<List<NavMeshBuildSource>> append, Action callback)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		while (!AI.move && !AiManager.nav_wait)
		{
			yield return CoroutineEx.waitForSeconds(1f);
		}
		float realtimeSinceStartup = Time.get_realtimeSinceStartup();
		Debug.Log((object)"Starting Navmesh Source Collecting");
		List<NavMeshBuildMarkup> list = new List<NavMeshBuildMarkup>();
		NavMeshBuilder.CollectSources(root, mask, geometry, area, list, sources);
		append?.Invoke(sources);
		Debug.Log((object)$"Navmesh Source Collecting took {Time.get_realtimeSinceStartup() - realtimeSinceStartup:0.00} seconds");
		callback?.Invoke();
	}
}
