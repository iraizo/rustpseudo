using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Rust;
using Rust.Ai;
using UnityEngine;
using UnityEngine.AI;

public class DynamicNavMesh : SingletonComponent<DynamicNavMesh>, IServerComponent
{
	public int NavMeshAgentTypeIndex;

	[Tooltip("The default area associated with the NavMeshAgent index.")]
	public string DefaultAreaName = "Walkable";

	public int AsyncTerrainNavMeshBakeCellSize = 80;

	public int AsyncTerrainNavMeshBakeCellHeight = 100;

	public Bounds Bounds;

	public NavMeshData NavMeshData;

	public NavMeshDataInstance NavMeshDataInstance;

	public LayerMask LayerMask;

	public NavMeshCollectGeometry NavMeshCollectGeometry;

	[ServerVar]
	public static bool use_baked_terrain_mesh;

	private List<NavMeshBuildSource> sources;

	private AsyncOperation BuildingOperation;

	private bool HasBuildOperationStarted;

	private Stopwatch BuildTimer = new Stopwatch();

	private int defaultArea;

	private int agentTypeId;

	public bool IsBuilding
	{
		get
		{
			if (!HasBuildOperationStarted || BuildingOperation != null)
			{
				return true;
			}
			return false;
		}
	}

	private void OnEnable()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		NavMeshBuildSettings settingsByIndex = NavMesh.GetSettingsByIndex(NavMeshAgentTypeIndex);
		agentTypeId = ((NavMeshBuildSettings)(ref settingsByIndex)).get_agentTypeID();
		NavMeshData = new NavMeshData(agentTypeId);
		sources = new List<NavMeshBuildSource>();
		defaultArea = NavMesh.GetAreaFromName(DefaultAreaName);
		((FacepunchBehaviour)this).InvokeRepeating((Action)FinishBuildingNavmesh, 0f, 1f);
	}

	private void OnDisable()
	{
		if (!Application.isQuitting)
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)FinishBuildingNavmesh);
			((NavMeshDataInstance)(ref NavMeshDataInstance)).Remove();
		}
	}

	[ContextMenu("Update Nav Mesh")]
	public void UpdateNavMeshAsync()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (!HasBuildOperationStarted && !AiManager.nav_disable)
		{
			float realtimeSinceStartup = Time.get_realtimeSinceStartup();
			Debug.Log((object)("Starting Navmesh Build with " + sources.Count + " sources"));
			NavMeshBuildSettings settingsByIndex = NavMesh.GetSettingsByIndex(NavMeshAgentTypeIndex);
			((NavMeshBuildSettings)(ref settingsByIndex)).set_overrideVoxelSize(true);
			((NavMeshBuildSettings)(ref settingsByIndex)).set_voxelSize(((NavMeshBuildSettings)(ref settingsByIndex)).get_voxelSize() * 2f);
			BuildingOperation = NavMeshBuilder.UpdateNavMeshDataAsync(NavMeshData, settingsByIndex, sources, Bounds);
			BuildTimer.Reset();
			BuildTimer.Start();
			HasBuildOperationStarted = true;
			float num = Time.get_realtimeSinceStartup() - realtimeSinceStartup;
			if (num > 0.1f)
			{
				Debug.LogWarning((object)("Calling UpdateNavMesh took " + num));
			}
		}
	}

	public IEnumerator UpdateNavMeshAndWait()
	{
		if (HasBuildOperationStarted || AiManager.nav_disable)
		{
			yield break;
		}
		HasBuildOperationStarted = false;
		((Bounds)(ref Bounds)).set_size(TerrainMeta.Size);
		NavMesh.set_pathfindingIterationsPerFrame(AiManager.pathfindingIterationsPerFrame);
		IEnumerator enumerator = NavMeshTools.CollectSourcesAsync(Bounds, LayerMask.op_Implicit(LayerMask), NavMeshCollectGeometry, defaultArea, use_baked_terrain_mesh, AsyncTerrainNavMeshBakeCellSize, sources, AppendModifierVolumes, UpdateNavMeshAsync);
		if (AiManager.nav_wait)
		{
			yield return enumerator;
		}
		else
		{
			((MonoBehaviour)this).StartCoroutine(enumerator);
		}
		if (!AiManager.nav_wait)
		{
			Debug.Log((object)"nav_wait is false, so we're not waiting for the navmesh to finish generating. This might cause your server to sputter while it's generating.");
			yield break;
		}
		int lastPct = 0;
		while (!HasBuildOperationStarted)
		{
			Thread.Sleep(250);
			yield return null;
		}
		while (BuildingOperation != null)
		{
			int num = (int)(BuildingOperation.get_progress() * 100f);
			if (lastPct != num)
			{
				Debug.LogFormat("{0}%", new object[1] { num });
				lastPct = num;
			}
			Thread.Sleep(250);
			FinishBuildingNavmesh();
			yield return null;
		}
	}

	private void AppendModifierVolumes(List<NavMeshBuildSource> sources)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		Vector3 size = default(Vector3);
		foreach (NavMeshModifierVolume activeModifier in NavMeshModifierVolume.get_activeModifiers())
		{
			if ((LayerMask.op_Implicit(LayerMask) & (1 << ((Component)activeModifier).get_gameObject().get_layer())) != 0 && activeModifier.AffectsAgentType(agentTypeId))
			{
				Vector3 val = ((Component)activeModifier).get_transform().TransformPoint(activeModifier.get_center());
				Vector3 lossyScale = ((Component)activeModifier).get_transform().get_lossyScale();
				((Vector3)(ref size))._002Ector(activeModifier.get_size().x * Mathf.Abs(lossyScale.x), activeModifier.get_size().y * Mathf.Abs(lossyScale.y), activeModifier.get_size().z * Mathf.Abs(lossyScale.z));
				NavMeshBuildSource item = default(NavMeshBuildSource);
				((NavMeshBuildSource)(ref item)).set_shape((NavMeshBuildSourceShape)5);
				((NavMeshBuildSource)(ref item)).set_transform(Matrix4x4.TRS(val, ((Component)activeModifier).get_transform().get_rotation(), Vector3.get_one()));
				((NavMeshBuildSource)(ref item)).set_size(size);
				((NavMeshBuildSource)(ref item)).set_area(activeModifier.get_area());
				sources.Add(item);
			}
		}
	}

	public void FinishBuildingNavmesh()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (BuildingOperation != null && BuildingOperation.get_isDone())
		{
			if (!((NavMeshDataInstance)(ref NavMeshDataInstance)).get_valid())
			{
				NavMeshDataInstance = NavMesh.AddNavMeshData(NavMeshData);
			}
			Debug.Log((object)$"Navmesh Build took {BuildTimer.Elapsed.TotalSeconds:0.00} seconds");
			BuildingOperation = null;
		}
	}
}
