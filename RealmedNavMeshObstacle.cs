using Rust.Ai;
using UnityEngine;
using UnityEngine.AI;

public class RealmedNavMeshObstacle : BasePrefab
{
	public NavMeshObstacle Obstacle;

	public override void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (bundling)
		{
			return;
		}
		base.PreProcess(process, rootObj, name, serverside, clientside, bundling: false);
		if (base.isServer && Object.op_Implicit((Object)(object)Obstacle))
		{
			if (AiManager.nav_disable)
			{
				process.RemoveComponent((Component)(object)Obstacle);
				Obstacle = null;
			}
			else if (AiManager.nav_obstacles_carve_state >= 2)
			{
				Obstacle.set_carving(true);
			}
			else if (AiManager.nav_obstacles_carve_state == 1)
			{
				Obstacle.set_carving(((Component)Obstacle).get_gameObject().get_layer() == 21);
			}
			else
			{
				Obstacle.set_carving(false);
			}
		}
		process.RemoveComponent((Component)(object)this);
	}
}
