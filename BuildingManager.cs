using ConVar;
using UnityEngine;
using UnityEngine.AI;

public abstract class BuildingManager
{
	public class Building
	{
		public uint ID;

		public ListHashSet<BuildingPrivlidge> buildingPrivileges = new ListHashSet<BuildingPrivlidge>(8);

		public ListHashSet<BuildingBlock> buildingBlocks = new ListHashSet<BuildingBlock>(8);

		public ListHashSet<DecayEntity> decayEntities = new ListHashSet<DecayEntity>(8);

		public NavMeshObstacle buildingNavMeshObstacle;

		public ListHashSet<NavMeshObstacle> navmeshCarvers;

		public bool isNavMeshCarvingDirty;

		public bool isNavMeshCarveOptimized;

		public bool IsEmpty()
		{
			if (HasBuildingPrivileges())
			{
				return false;
			}
			if (HasBuildingBlocks())
			{
				return false;
			}
			if (HasDecayEntities())
			{
				return false;
			}
			return true;
		}

		public BuildingPrivlidge GetDominatingBuildingPrivilege()
		{
			BuildingPrivlidge buildingPrivlidge = null;
			if (HasBuildingPrivileges())
			{
				for (int i = 0; i < buildingPrivileges.get_Count(); i++)
				{
					BuildingPrivlidge buildingPrivlidge2 = buildingPrivileges.get_Item(i);
					if (!((Object)(object)buildingPrivlidge2 == (Object)null) && buildingPrivlidge2.IsOlderThan(buildingPrivlidge))
					{
						buildingPrivlidge = buildingPrivlidge2;
					}
				}
			}
			return buildingPrivlidge;
		}

		public bool HasBuildingPrivileges()
		{
			if (buildingPrivileges != null)
			{
				return buildingPrivileges.get_Count() > 0;
			}
			return false;
		}

		public bool HasBuildingBlocks()
		{
			if (buildingBlocks != null)
			{
				return buildingBlocks.get_Count() > 0;
			}
			return false;
		}

		public bool HasDecayEntities()
		{
			if (decayEntities != null)
			{
				return decayEntities.get_Count() > 0;
			}
			return false;
		}

		public void AddBuildingPrivilege(BuildingPrivlidge ent)
		{
			if (!((Object)(object)ent == (Object)null) && !buildingPrivileges.Contains(ent))
			{
				buildingPrivileges.Add(ent);
			}
		}

		public void RemoveBuildingPrivilege(BuildingPrivlidge ent)
		{
			if (!((Object)(object)ent == (Object)null))
			{
				buildingPrivileges.Remove(ent);
			}
		}

		public void AddBuildingBlock(BuildingBlock ent)
		{
			if ((Object)(object)ent == (Object)null || buildingBlocks.Contains(ent))
			{
				return;
			}
			buildingBlocks.Add(ent);
			if (!AI.nav_carve_use_building_optimization)
			{
				return;
			}
			NavMeshObstacle component = ((Component)ent).GetComponent<NavMeshObstacle>();
			if ((Object)(object)component != (Object)null)
			{
				isNavMeshCarvingDirty = true;
				if (navmeshCarvers == null)
				{
					navmeshCarvers = new ListHashSet<NavMeshObstacle>(8);
				}
				navmeshCarvers.Add(component);
			}
		}

		public void RemoveBuildingBlock(BuildingBlock ent)
		{
			if ((Object)(object)ent == (Object)null)
			{
				return;
			}
			buildingBlocks.Remove(ent);
			if (!AI.nav_carve_use_building_optimization || navmeshCarvers == null)
			{
				return;
			}
			NavMeshObstacle component = ((Component)ent).GetComponent<NavMeshObstacle>();
			if (!((Object)(object)component != (Object)null))
			{
				return;
			}
			navmeshCarvers.Remove(component);
			if (navmeshCarvers.get_Count() == 0)
			{
				navmeshCarvers = null;
			}
			isNavMeshCarvingDirty = true;
			if (navmeshCarvers == null)
			{
				Building building = ent.GetBuilding();
				if (building != null)
				{
					int ticks = 2;
					server.UpdateNavMeshCarver(building, ref ticks, 0);
				}
			}
		}

		public void AddDecayEntity(DecayEntity ent)
		{
			if (!((Object)(object)ent == (Object)null) && !decayEntities.Contains(ent))
			{
				decayEntities.Add(ent);
			}
		}

		public void RemoveDecayEntity(DecayEntity ent)
		{
			if (!((Object)(object)ent == (Object)null))
			{
				decayEntities.Remove(ent);
			}
		}

		public void Add(DecayEntity ent)
		{
			AddDecayEntity(ent);
			AddBuildingBlock(ent as BuildingBlock);
			AddBuildingPrivilege(ent as BuildingPrivlidge);
		}

		public void Remove(DecayEntity ent)
		{
			RemoveDecayEntity(ent);
			RemoveBuildingBlock(ent as BuildingBlock);
			RemoveBuildingPrivilege(ent as BuildingPrivlidge);
		}

		public void Dirty()
		{
			BuildingPrivlidge dominatingBuildingPrivilege = GetDominatingBuildingPrivilege();
			if ((Object)(object)dominatingBuildingPrivilege != (Object)null)
			{
				dominatingBuildingPrivilege.BuildingDirty();
			}
		}
	}

	public static ServerBuildingManager server = new ServerBuildingManager();

	protected ListHashSet<DecayEntity> decayEntities = new ListHashSet<DecayEntity>(8);

	protected ListDictionary<uint, Building> buildingDictionary = new ListDictionary<uint, Building>();

	public Building GetBuilding(uint buildingID)
	{
		Building result = null;
		buildingDictionary.TryGetValue(buildingID, ref result);
		return result;
	}

	public void Add(DecayEntity ent)
	{
		if (ent.buildingID == 0)
		{
			if (!decayEntities.Contains(ent))
			{
				decayEntities.Add(ent);
			}
			return;
		}
		Building building = GetBuilding(ent.buildingID);
		if (building == null)
		{
			building = CreateBuilding(ent.buildingID);
			buildingDictionary.Add(ent.buildingID, building);
		}
		building.Add(ent);
		building.Dirty();
	}

	public void Remove(DecayEntity ent)
	{
		if (ent.buildingID == 0)
		{
			decayEntities.Remove(ent);
			return;
		}
		Building building = GetBuilding(ent.buildingID);
		if (building != null)
		{
			building.Remove(ent);
			if (building.IsEmpty())
			{
				buildingDictionary.Remove(ent.buildingID);
				DisposeBuilding(ref building);
			}
			else
			{
				building.Dirty();
			}
		}
	}

	public void Clear()
	{
		buildingDictionary.Clear();
	}

	protected abstract Building CreateBuilding(uint id);

	protected abstract void DisposeBuilding(ref Building building);
}
