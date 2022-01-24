using System;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai
{
	[DefaultExecutionOrder(-103)]
	public class AiManager : SingletonComponent<AiManager>, IServerComponent
	{
		[Header("Cover System")]
		[SerializeField]
		public bool UseCover = true;

		public float CoverPointVolumeCellSize = 20f;

		public float CoverPointVolumeCellHeight = 8f;

		public float CoverPointRayLength = 1f;

		public CoverPointVolume cpvPrefab;

		[SerializeField]
		public LayerMask DynamicCoverPointVolumeLayerMask;

		private WorldSpaceGrid<CoverPointVolume> coverPointVolumeGrid;

		[ServerVar(Help = "If true we'll wait for the navmesh to generate before completely starting the server. This might cause your server to hitch and lag as it generates in the background.")]
		public static bool nav_wait = true;

		[ServerVar(Help = "If set to true the navmesh won't generate.. which means Ai that uses the navmesh won't be able to move")]
		public static bool nav_disable = false;

		[ServerVar(Help = "If set to true, npcs will attempt to place themselves on the navmesh if not on a navmesh when set destination is called.")]
		public static bool setdestination_navmesh_failsafe = false;

		[ServerVar(Help = "If ai_dormant is true, any npc outside the range of players will render itself dormant and take up less resources, but wildlife won't simulate as well.")]
		public static bool ai_dormant = true;

		[ServerVar(Help = "If an agent is beyond this distance to a player, it's flagged for becoming dormant.")]
		public static float ai_to_player_distance_wakeup_range = 160f;

		[ServerVar(Help = "nav_obstacles_carve_state defines which obstacles can carve the terrain. 0 - No carving, 1 - Only player construction carves, 2 - All obstacles carve.")]
		public static int nav_obstacles_carve_state = 2;

		[ServerVar(Help = "ai_dormant_max_wakeup_per_tick defines the maximum number of dormant agents we will wake up in a single tick. (default: 30)")]
		public static int ai_dormant_max_wakeup_per_tick = 30;

		[ServerVar(Help = "ai_htn_player_tick_budget defines the maximum amount of milliseconds ticking htn player agents are allowed to consume. (default: 4 ms)")]
		public static float ai_htn_player_tick_budget = 4f;

		[ServerVar(Help = "ai_htn_player_junkpile_tick_budget defines the maximum amount of milliseconds ticking htn player junkpile agents are allowed to consume. (default: 4 ms)")]
		public static float ai_htn_player_junkpile_tick_budget = 4f;

		[ServerVar(Help = "ai_htn_animal_tick_budget defines the maximum amount of milliseconds ticking htn animal agents are allowed to consume. (default: 4 ms)")]
		public static float ai_htn_animal_tick_budget = 4f;

		[ServerVar(Help = "If ai_htn_use_agency_tick is true, the ai manager's agency system will tick htn agents at the ms budgets defined in ai_htn_player_tick_budget and ai_htn_animal_tick_budget. If it's false, each agent registers with the invoke system individually, with no frame-budget restrictions. (default: true)")]
		public static bool ai_htn_use_agency_tick = true;

		private readonly BasePlayer[] playerVicinityQuery = new BasePlayer[1];

		private readonly Func<BasePlayer, bool> filter = InterestedInPlayersOnly;

		[ServerVar(Help = "The maximum amount of nodes processed each frame in the asynchronous pathfinding process. Increasing this value will cause the paths to be processed faster, but can cause some hiccups in frame rate. Default value is 100, a good range for tuning is between 50 and 500.")]
		public static int pathfindingIterationsPerFrame
		{
			get
			{
				return NavMesh.get_pathfindingIterationsPerFrame();
			}
			set
			{
				NavMesh.set_pathfindingIterationsPerFrame(value);
			}
		}

		public bool repeat => true;

		internal void OnEnableAgency()
		{
		}

		internal void OnDisableAgency()
		{
		}

		internal void UpdateAgency()
		{
		}

		internal void OnEnableCover()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			if (coverPointVolumeGrid == null)
			{
				Vector3 size = TerrainMeta.Size;
				coverPointVolumeGrid = new WorldSpaceGrid<CoverPointVolume>(size.x, CoverPointVolumeCellSize);
			}
		}

		internal void OnDisableCover()
		{
			if (coverPointVolumeGrid != null && coverPointVolumeGrid.Cells != null)
			{
				for (int i = 0; i < coverPointVolumeGrid.Cells.Length; i++)
				{
					Object.Destroy((Object)(object)coverPointVolumeGrid.Cells[i]);
				}
			}
		}

		public static CoverPointVolume CreateNewCoverVolume(Vector3 point, Transform coverPointGroup)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)SingletonComponent<AiManager>.Instance != (Object)null && ((Behaviour)SingletonComponent<AiManager>.Instance).get_enabled() && SingletonComponent<AiManager>.Instance.UseCover)
			{
				CoverPointVolume coverPointVolume = SingletonComponent<AiManager>.Instance.GetCoverVolumeContaining(point);
				if ((Object)(object)coverPointVolume == (Object)null)
				{
					Vector2i val = SingletonComponent<AiManager>.Instance.coverPointVolumeGrid.WorldToGridCoords(point);
					coverPointVolume = ((!((Object)(object)SingletonComponent<AiManager>.Instance.cpvPrefab != (Object)null)) ? new GameObject("CoverPointVolume").AddComponent<CoverPointVolume>() : Object.Instantiate<CoverPointVolume>(SingletonComponent<AiManager>.Instance.cpvPrefab));
					((Component)coverPointVolume).get_transform().set_localPosition(default(Vector3));
					((Component)coverPointVolume).get_transform().set_position(SingletonComponent<AiManager>.Instance.coverPointVolumeGrid.GridToWorldCoords(val) + Vector3.get_up() * point.y);
					((Component)coverPointVolume).get_transform().set_localScale(new Vector3(SingletonComponent<AiManager>.Instance.CoverPointVolumeCellSize, SingletonComponent<AiManager>.Instance.CoverPointVolumeCellHeight, SingletonComponent<AiManager>.Instance.CoverPointVolumeCellSize));
					coverPointVolume.CoverLayerMask = SingletonComponent<AiManager>.Instance.DynamicCoverPointVolumeLayerMask;
					coverPointVolume.CoverPointRayLength = SingletonComponent<AiManager>.Instance.CoverPointRayLength;
					SingletonComponent<AiManager>.Instance.coverPointVolumeGrid.set_Item(val, coverPointVolume);
					coverPointVolume.GenerateCoverPoints(coverPointGroup);
				}
				return coverPointVolume;
			}
			return null;
		}

		public CoverPointVolume GetCoverVolumeContaining(Vector3 point)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			if (coverPointVolumeGrid == null)
			{
				return null;
			}
			Vector2i val = coverPointVolumeGrid.WorldToGridCoords(point);
			return coverPointVolumeGrid.get_Item(val);
		}

		public void Initialize()
		{
			OnEnableAgency();
			if (UseCover)
			{
				OnEnableCover();
			}
		}

		private void OnDisable()
		{
			if (!Application.isQuitting)
			{
				OnDisableAgency();
				if (UseCover)
				{
					OnDisableCover();
				}
			}
		}

		public float? ExecuteUpdate(float deltaTime, float nextInterval)
		{
			if (nav_disable)
			{
				return nextInterval;
			}
			UpdateAgency();
			return Random.get_value() + 1f;
		}

		private static bool InterestedInPlayersOnly(BaseEntity entity)
		{
			BasePlayer basePlayer = entity as BasePlayer;
			if ((Object)(object)basePlayer == (Object)null)
			{
				return false;
			}
			if (basePlayer.IsSleeping() || !basePlayer.IsConnected)
			{
				return false;
			}
			return true;
		}
	}
}
