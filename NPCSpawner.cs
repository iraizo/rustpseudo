using System;
using ConVar;
using UnityEngine;

public class NPCSpawner : SpawnGroup
{
	public int AdditionalLOSBlockingLayer;

	public MonumentNavMesh monumentNavMesh;

	public bool shouldFillOnSpawn;

	[Header("InfoZone Config")]
	public AIInformationZone VirtualInfoZone;

	[Header("Navigator Config")]
	public AIMovePointPath Path;

	public BasePath AStarGraph;

	[Header("Human Stat Replacements")]
	public bool UseStatModifiers;

	public float SenseRange = 30f;

	public float TargetLostRange = 50f;

	public float AttackRangeMultiplier = 1f;

	public float ListenRange = 10f;

	public override void SpawnInitial()
	{
		fillOnSpawn = shouldFillOnSpawn;
		if (WaitingForNavMesh())
		{
			((FacepunchBehaviour)this).Invoke((Action)LateSpawn, 10f);
		}
		else
		{
			base.SpawnInitial();
		}
	}

	public bool WaitingForNavMesh()
	{
		if ((Object)(object)monumentNavMesh != (Object)null)
		{
			return monumentNavMesh.IsBuilding;
		}
		if (!DungeonNavmesh.NavReady())
		{
			return true;
		}
		return !AI.move;
	}

	public void LateSpawn()
	{
		if (!WaitingForNavMesh())
		{
			SpawnInitial();
			Debug.Log((object)"Navmesh complete, spawning");
		}
		else
		{
			((FacepunchBehaviour)this).Invoke((Action)LateSpawn, 5f);
		}
	}

	protected override void PostSpawnProcess(BaseEntity entity, BaseSpawnPoint spawnPoint)
	{
		base.PostSpawnProcess(entity, spawnPoint);
		BaseNavigator component = ((Component)entity).GetComponent<BaseNavigator>();
		HumanNPC humanNPC;
		if (AdditionalLOSBlockingLayer != 0 && (Object)(object)entity != (Object)null && (humanNPC = entity as HumanNPC) != null)
		{
			humanNPC.AdditionalLosBlockingLayer = AdditionalLOSBlockingLayer;
		}
		HumanNPC humanNPC2 = entity as HumanNPC;
		if ((Object)(object)humanNPC2 != (Object)null)
		{
			ModifyHumanBrainStats(humanNPC2.Brain);
		}
		if ((Object)(object)VirtualInfoZone != (Object)null)
		{
			if (VirtualInfoZone.Virtual)
			{
				NPCPlayer nPCPlayer = entity as NPCPlayer;
				if ((Object)(object)nPCPlayer != (Object)null)
				{
					nPCPlayer.VirtualInfoZone = VirtualInfoZone;
					if ((Object)(object)humanNPC2 != (Object)null)
					{
						humanNPC2.VirtualInfoZone.RegisterSleepableEntity(humanNPC2.Brain);
					}
				}
			}
			else
			{
				Debug.LogError((object)"NPCSpawner trying to set a virtual info zone without the Virtual property!");
			}
		}
		if ((Object)(object)component != (Object)null)
		{
			component.Path = Path;
			component.AStarGraph = AStarGraph;
		}
	}

	private void ModifyHumanBrainStats(BaseAIBrain<HumanNPC> brain)
	{
		if (UseStatModifiers && !((Object)(object)brain == (Object)null))
		{
			brain.SenseRange = SenseRange;
			brain.TargetLostRange *= TargetLostRange;
			brain.AttackRangeMultiplier = AttackRangeMultiplier;
			brain.ListenRange = ListenRange;
		}
	}
}
