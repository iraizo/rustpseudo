using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using ConVar;
using Rust;
using Rust.Ai;
using UnityEngine;
using UnityEngine.AI;

public class DungeonNavmesh : FacepunchBehaviour, IServerComponent
{
	public int NavMeshAgentTypeIndex;

	[Tooltip("The default area associated with the NavMeshAgent index.")]
	public string DefaultAreaName = "HumanNPC";

	public float NavmeshResolutionModifier = 1.25f;

	[Tooltip("Bounds which are auto calculated from CellSize * CellCount")]
	public Bounds Bounds;

	public NavMeshData NavMeshData;

	public NavMeshDataInstance NavMeshDataInstance;

	public LayerMask LayerMask;

	public NavMeshCollectGeometry NavMeshCollectGeometry;

	public static List<DungeonNavmesh> Instances = new List<DungeonNavmesh>();

	[ServerVar]
	public static bool use_baked_terrain_mesh = true;

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

	public static bool NavReady()
	{
		if (Instances == null || Instances.Count == 0)
		{
			return true;
		}
		foreach (DungeonNavmesh instance in Instances)
		{
			if (instance.IsBuilding)
			{
				return false;
			}
		}
		return true;
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
		Instances.Add(this);
	}

	private void OnDisable()
	{
		if (!Application.isQuitting)
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)FinishBuildingNavmesh);
			((NavMeshDataInstance)(ref NavMeshDataInstance)).Remove();
			Instances.Remove(this);
		}
	}

	[ContextMenu("Update Monument Nav Mesh")]
	public void UpdateNavMeshAsync()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		if (!HasBuildOperationStarted && !AiManager.nav_disable && AI.npc_enable)
		{
			float realtimeSinceStartup = Time.get_realtimeSinceStartup();
			Debug.Log((object)("Starting Dungeon Navmesh Build with " + sources.Count + " sources"));
			NavMeshBuildSettings settingsByIndex = NavMesh.GetSettingsByIndex(NavMeshAgentTypeIndex);
			((NavMeshBuildSettings)(ref settingsByIndex)).set_overrideVoxelSize(true);
			((NavMeshBuildSettings)(ref settingsByIndex)).set_voxelSize(((NavMeshBuildSettings)(ref settingsByIndex)).get_voxelSize() * NavmeshResolutionModifier);
			BuildingOperation = NavMeshBuilder.UpdateNavMeshDataAsync(NavMeshData, settingsByIndex, sources, Bounds);
			BuildTimer.Reset();
			BuildTimer.Start();
			HasBuildOperationStarted = true;
			float num = Time.get_realtimeSinceStartup() - realtimeSinceStartup;
			if (num > 0.1f)
			{
				Debug.LogWarning((object)("Calling UpdateNavMesh took " + num));
			}
			NotifyInformationZonesOfCompletion();
		}
	}

	public void NotifyInformationZonesOfCompletion()
	{
		foreach (AIInformationZone zone in AIInformationZone.zones)
		{
			zone.NavmeshBuildingComplete();
		}
	}

	public void SourcesCollected()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		int count = sources.Count;
		Debug.Log((object)("Source count Pre cull : " + sources.Count));
		Vector3 val = default(Vector3);
		for (int num = sources.Count - 1; num >= 0; num--)
		{
			NavMeshBuildSource item = sources[num];
			Matrix4x4 transform = ((NavMeshBuildSource)(ref item)).get_transform();
			((Vector3)(ref val))._002Ector(((Matrix4x4)(ref transform)).get_Item(0, 3), ((Matrix4x4)(ref transform)).get_Item(1, 3), ((Matrix4x4)(ref transform)).get_Item(2, 3));
			bool flag = false;
			foreach (AIInformationZone zone in AIInformationZone.zones)
			{
				if (Vector3Ex.Distance2D(zone.ClosestPointTo(val), val) <= 50f)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				sources.Remove(item);
			}
		}
		Debug.Log((object)("Source count post cull : " + sources.Count + " total removed : " + (count - sources.Count)));
	}

	public IEnumerator UpdateNavMeshAndWait()
	{
		if (HasBuildOperationStarted || AiManager.nav_disable || !AI.npc_enable)
		{
			yield break;
		}
		HasBuildOperationStarted = false;
		((Bounds)(ref Bounds)).set_center(((Component)this).get_transform().get_position());
		((Bounds)(ref Bounds)).set_size(new Vector3(1000000f, 100000f, 100000f));
		IEnumerator enumerator = NavMeshTools.CollectSourcesAsync(((Component)this).get_transform(), ((LayerMask)(ref LayerMask)).get_value(), NavMeshCollectGeometry, defaultArea, sources, AppendModifierVolumes, UpdateNavMeshAsync);
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
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		Vector3 size = default(Vector3);
		foreach (NavMeshModifierVolume activeModifier in NavMeshModifierVolume.get_activeModifiers())
		{
			if ((LayerMask.op_Implicit(LayerMask) & (1 << ((Component)activeModifier).get_gameObject().get_layer())) != 0 && activeModifier.AffectsAgentType(agentTypeId))
			{
				Vector3 val = ((Component)activeModifier).get_transform().TransformPoint(activeModifier.get_center());
				if (((Bounds)(ref Bounds)).Contains(val))
				{
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
			Debug.Log((object)$"Monument Navmesh Build took {BuildTimer.Elapsed.TotalSeconds:0.00} seconds");
			BuildingOperation = null;
		}
	}

	public void OnDrawGizmosSelected()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.set_color(Color.get_magenta() * new Color(1f, 1f, 1f, 0.5f));
		Gizmos.DrawCube(((Component)this).get_transform().get_position(), ((Bounds)(ref Bounds)).get_size());
	}

	public DungeonNavmesh()
		: this()
	{
	}
}
