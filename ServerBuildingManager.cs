using System;
using ConVar;
using UnityEngine;
using UnityEngine.AI;

public class ServerBuildingManager : BuildingManager
{
	private int decayTickBuildingIndex;

	private int decayTickEntityIndex;

	private int decayTickWorldIndex;

	private int navmeshCarveTickBuildingIndex;

	private uint maxBuildingID;

	public void CheckSplit(DecayEntity ent)
	{
		if (ent.buildingID != 0)
		{
			Building building = ent.GetBuilding();
			if (building != null && ShouldSplit(building))
			{
				Split(building);
			}
		}
	}

	private bool ShouldSplit(Building building)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (building.HasBuildingBlocks())
		{
			building.buildingBlocks.get_Item(0).EntityLinkBroadcast();
			Enumerator<BuildingBlock> enumerator = building.buildingBlocks.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.get_Current().ReceivedEntityLinkBroadcast())
					{
						return true;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}
		return false;
	}

	private void Split(Building building)
	{
		while (building.HasBuildingBlocks())
		{
			BuildingBlock buildingBlock = building.buildingBlocks.get_Item(0);
			uint newID = BuildingManager.server.NewBuildingID();
			buildingBlock.EntityLinkBroadcast(delegate(BuildingBlock b)
			{
				b.AttachToBuilding(newID);
			});
		}
		while (building.HasBuildingPrivileges())
		{
			BuildingPrivlidge buildingPrivlidge = building.buildingPrivileges.get_Item(0);
			BuildingBlock nearbyBuildingBlock = buildingPrivlidge.GetNearbyBuildingBlock();
			buildingPrivlidge.AttachToBuilding(Object.op_Implicit((Object)(object)nearbyBuildingBlock) ? nearbyBuildingBlock.buildingID : 0u);
		}
		while (building.HasDecayEntities())
		{
			DecayEntity decayEntity = building.decayEntities.get_Item(0);
			BuildingBlock nearbyBuildingBlock2 = decayEntity.GetNearbyBuildingBlock();
			decayEntity.AttachToBuilding(Object.op_Implicit((Object)(object)nearbyBuildingBlock2) ? nearbyBuildingBlock2.buildingID : 0u);
		}
		if (AI.nav_carve_use_building_optimization)
		{
			building.isNavMeshCarvingDirty = true;
			int ticks = 2;
			UpdateNavMeshCarver(building, ref ticks, 0);
		}
	}

	public void CheckMerge(DecayEntity ent)
	{
		if (ent.buildingID == 0)
		{
			return;
		}
		Building building = ent.GetBuilding();
		if (building == null)
		{
			return;
		}
		ent.EntityLinkMessage(delegate(BuildingBlock b)
		{
			if (b.buildingID != building.ID)
			{
				Building building2 = b.GetBuilding();
				if (building2 != null)
				{
					Merge(building, building2);
				}
			}
		});
		if (AI.nav_carve_use_building_optimization)
		{
			building.isNavMeshCarvingDirty = true;
			int ticks = 2;
			UpdateNavMeshCarver(building, ref ticks, 0);
		}
	}

	private void Merge(Building building1, Building building2)
	{
		while (building2.HasDecayEntities())
		{
			building2.decayEntities.get_Item(0).AttachToBuilding(building1.ID);
		}
		if (AI.nav_carve_use_building_optimization)
		{
			building1.isNavMeshCarvingDirty = true;
			building2.isNavMeshCarvingDirty = true;
			int ticks = 3;
			UpdateNavMeshCarver(building1, ref ticks, 0);
			UpdateNavMeshCarver(building1, ref ticks, 0);
		}
	}

	public void Cycle()
	{
		TimeWarning val = TimeWarning.New("StabilityCheckQueue", 0);
		try
		{
			((ObjectWorkQueue<StabilityEntity>)StabilityEntity.stabilityCheckQueue).RunQueue((double)Stability.stabilityqueue);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		val = TimeWarning.New("UpdateSurroundingsQueue", 0);
		try
		{
			((ObjectWorkQueue<Bounds>)StabilityEntity.updateSurroundingsQueue).RunQueue((double)Stability.surroundingsqueue);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		val = TimeWarning.New("UpdateSkinQueue", 0);
		try
		{
			((ObjectWorkQueue<BuildingBlock>)BuildingBlock.updateSkinQueueServer).RunQueue(1.0);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		val = TimeWarning.New("BuildingDecayTick", 0);
		try
		{
			int num = 5;
			BufferList<Building> values = buildingDictionary.get_Values();
			for (int i = decayTickBuildingIndex; i < values.get_Count(); i++)
			{
				if (num <= 0)
				{
					break;
				}
				BufferList<DecayEntity> values2 = values.get_Item(i).decayEntities.get_Values();
				for (int j = decayTickEntityIndex; j < values2.get_Count(); j++)
				{
					if (num <= 0)
					{
						break;
					}
					values2.get_Item(j).DecayTick();
					num--;
					if (num <= 0)
					{
						decayTickBuildingIndex = i;
						decayTickEntityIndex = j;
					}
				}
				if (num > 0)
				{
					decayTickEntityIndex = 0;
				}
			}
			if (num > 0)
			{
				decayTickBuildingIndex = 0;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		val = TimeWarning.New("WorldDecayTick", 0);
		try
		{
			int num2 = 5;
			BufferList<DecayEntity> values3 = decayEntities.get_Values();
			for (int k = decayTickWorldIndex; k < values3.get_Count(); k++)
			{
				if (num2 <= 0)
				{
					break;
				}
				values3.get_Item(k).DecayTick();
				num2--;
				if (num2 <= 0)
				{
					decayTickWorldIndex = k;
				}
			}
			if (num2 > 0)
			{
				decayTickWorldIndex = 0;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		if (!AI.nav_carve_use_building_optimization)
		{
			return;
		}
		val = TimeWarning.New("NavMeshCarving", 0);
		try
		{
			int ticks = 5;
			BufferList<Building> values4 = buildingDictionary.get_Values();
			for (int l = navmeshCarveTickBuildingIndex; l < values4.get_Count(); l++)
			{
				if (ticks <= 0)
				{
					break;
				}
				Building building = values4.get_Item(l);
				UpdateNavMeshCarver(building, ref ticks, l);
			}
			if (ticks > 0)
			{
				navmeshCarveTickBuildingIndex = 0;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void UpdateNavMeshCarver(Building building, ref int ticks, int i)
	{
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		if (!AI.nav_carve_use_building_optimization || (!building.isNavMeshCarveOptimized && building.navmeshCarvers.get_Count() < AI.nav_carve_min_building_blocks_to_apply_optimization) || !building.isNavMeshCarvingDirty)
		{
			return;
		}
		building.isNavMeshCarvingDirty = false;
		if (building.navmeshCarvers == null)
		{
			if ((Object)(object)building.buildingNavMeshObstacle != (Object)null)
			{
				Object.Destroy((Object)(object)((Component)building.buildingNavMeshObstacle).get_gameObject());
				building.buildingNavMeshObstacle = null;
				building.isNavMeshCarveOptimized = false;
			}
			return;
		}
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector((float)World.Size, (float)World.Size, (float)World.Size);
		Vector3 val2 = default(Vector3);
		((Vector3)(ref val2))._002Ector((float)(0L - (long)World.Size), (float)(0L - (long)World.Size), (float)(0L - (long)World.Size));
		int count = building.navmeshCarvers.get_Count();
		if (count > 0)
		{
			for (int j = 0; j < count; j++)
			{
				NavMeshObstacle val3 = building.navmeshCarvers.get_Item(j);
				if (((Behaviour)val3).get_enabled())
				{
					((Behaviour)val3).set_enabled(false);
				}
				for (int k = 0; k < 3; k++)
				{
					Vector3 position = ((Component)val3).get_transform().get_position();
					if (((Vector3)(ref position)).get_Item(k) < ((Vector3)(ref val)).get_Item(k))
					{
						int num = k;
						position = ((Component)val3).get_transform().get_position();
						((Vector3)(ref val)).set_Item(num, ((Vector3)(ref position)).get_Item(k));
					}
					position = ((Component)val3).get_transform().get_position();
					if (((Vector3)(ref position)).get_Item(k) > ((Vector3)(ref val2)).get_Item(k))
					{
						int num2 = k;
						position = ((Component)val3).get_transform().get_position();
						((Vector3)(ref val2)).set_Item(num2, ((Vector3)(ref position)).get_Item(k));
					}
				}
			}
			Vector3 val4 = (val2 + val) * 0.5f;
			Vector3 val5 = Vector3.get_zero();
			float num3 = Mathf.Abs(val4.x - val.x);
			float num4 = Mathf.Abs(val4.y - val.y);
			float num5 = Mathf.Abs(val4.z - val.z);
			float num6 = Mathf.Abs(val2.x - val4.x);
			float num7 = Mathf.Abs(val2.y - val4.y);
			float num8 = Mathf.Abs(val2.z - val4.z);
			val5.x = Mathf.Max((num3 > num6) ? num3 : num6, AI.nav_carve_min_base_size);
			val5.y = Mathf.Max((num4 > num7) ? num4 : num7, AI.nav_carve_min_base_size);
			val5.z = Mathf.Max((num5 > num8) ? num5 : num8, AI.nav_carve_min_base_size);
			val5 = ((count >= 10) ? (val5 * (AI.nav_carve_size_multiplier - 1f)) : (val5 * AI.nav_carve_size_multiplier));
			if (building.navmeshCarvers.get_Count() > 0)
			{
				if ((Object)(object)building.buildingNavMeshObstacle == (Object)null)
				{
					building.buildingNavMeshObstacle = new GameObject($"Building ({building.ID}) NavMesh Carver").AddComponent<NavMeshObstacle>();
					((Behaviour)building.buildingNavMeshObstacle).set_enabled(false);
					building.buildingNavMeshObstacle.set_carving(true);
					building.buildingNavMeshObstacle.set_shape((NavMeshObstacleShape)1);
					building.buildingNavMeshObstacle.set_height(AI.nav_carve_height);
					building.isNavMeshCarveOptimized = true;
				}
				if ((Object)(object)building.buildingNavMeshObstacle != (Object)null)
				{
					((Component)building.buildingNavMeshObstacle).get_transform().set_position(val4);
					building.buildingNavMeshObstacle.set_size(val5);
					if (!((Behaviour)building.buildingNavMeshObstacle).get_enabled())
					{
						((Behaviour)building.buildingNavMeshObstacle).set_enabled(true);
					}
				}
			}
		}
		else if ((Object)(object)building.buildingNavMeshObstacle != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)building.buildingNavMeshObstacle).get_gameObject());
			building.buildingNavMeshObstacle = null;
			building.isNavMeshCarveOptimized = false;
		}
		ticks--;
		if (ticks <= 0)
		{
			navmeshCarveTickBuildingIndex = i;
		}
	}

	public uint NewBuildingID()
	{
		return ++maxBuildingID;
	}

	public void LoadBuildingID(uint id)
	{
		maxBuildingID = Mathx.Max(maxBuildingID, id);
	}

	protected override Building CreateBuilding(uint id)
	{
		return new Building
		{
			ID = id
		};
	}

	protected override void DisposeBuilding(ref Building building)
	{
		building = null;
	}
}
