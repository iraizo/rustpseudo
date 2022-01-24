using ConVar;
using UnityEngine;

namespace Rust.Ai
{
	public class ScientistSpawner : SpawnGroup
	{
		[Header("Scientist Spawner")]
		public bool Mobile = true;

		public bool NeverMove;

		public bool SpawnHostile;

		public bool OnlyAggroMarkedTargets = true;

		public bool IsPeacekeeper = true;

		public bool IsBandit;

		public bool IsMilitaryTunnelLab;

		public WaypointSet Waypoints;

		public Transform[] LookAtInterestPointsStationary;

		public Vector2 RadioEffectRepeatRange = new Vector2(10f, 15f);

		public Model Model;

		[SerializeField]
		private AiLocationManager _mgr;

		private float _nextForcedRespawn = float.PositiveInfinity;

		private bool _lastSpawnCallHadAliveMembers;

		private bool _lastSpawnCallHadMaxAliveMembers;

		protected override void Spawn(int numToSpawn)
		{
			if (!ConVar.AI.npc_enable)
			{
				return;
			}
			if (base.currentPopulation == maxPopulation)
			{
				_lastSpawnCallHadMaxAliveMembers = true;
				_lastSpawnCallHadAliveMembers = true;
				return;
			}
			if (_lastSpawnCallHadMaxAliveMembers)
			{
				_nextForcedRespawn = Time.get_time() + 2200f;
			}
			if (Time.get_time() < _nextForcedRespawn)
			{
				if (base.currentPopulation == 0 && _lastSpawnCallHadAliveMembers)
				{
					_lastSpawnCallHadMaxAliveMembers = false;
					_lastSpawnCallHadAliveMembers = false;
					return;
				}
				if (base.currentPopulation > 0)
				{
					_lastSpawnCallHadMaxAliveMembers = false;
					_lastSpawnCallHadAliveMembers = base.currentPopulation > 0;
					return;
				}
			}
			_lastSpawnCallHadMaxAliveMembers = false;
			_lastSpawnCallHadAliveMembers = base.currentPopulation > 0;
			base.Spawn(numToSpawn);
		}

		protected override void PostSpawnProcess(BaseEntity entity, BaseSpawnPoint spawnPoint)
		{
		}

		protected override void OnDrawGizmos()
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			base.OnDrawGizmos();
			if (LookAtInterestPointsStationary == null || LookAtInterestPointsStationary.Length == 0)
			{
				return;
			}
			Gizmos.set_color(Color.get_magenta() - new Color(0f, 0f, 0f, 0.5f));
			Transform[] lookAtInterestPointsStationary = LookAtInterestPointsStationary;
			foreach (Transform val in lookAtInterestPointsStationary)
			{
				if ((Object)(object)val != (Object)null)
				{
					Gizmos.DrawSphere(val.get_position(), 0.1f);
					Gizmos.DrawLine(((Component)this).get_transform().get_position(), val.get_position());
				}
			}
		}
	}
}
