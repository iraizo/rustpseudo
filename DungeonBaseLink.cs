using System.Collections.Generic;
using UnityEngine;

public class DungeonBaseLink : MonoBehaviour
{
	public DungeonBaseLinkType Type;

	public int Cost = 1;

	public int MaxFloor = -1;

	public int MaxCountLocal = -1;

	public int MaxCountGlobal = -1;

	[Tooltip("If set to a positive number, all segments with the same MaxCountIdentifier are counted towards MaxCountLocal and MaxCountGlobal")]
	public int MaxCountIdentifier = -1;

	internal DungeonBaseInfo Dungeon;

	public MeshRenderer[] MapRenderers;

	private List<DungeonBaseSocket> sockets;

	private List<DungeonVolume> volumes;

	internal List<DungeonBaseSocket> Sockets
	{
		get
		{
			if (sockets == null)
			{
				sockets = new List<DungeonBaseSocket>();
				((Component)this).GetComponentsInChildren<DungeonBaseSocket>(true, sockets);
			}
			return sockets;
		}
	}

	internal List<DungeonVolume> Volumes
	{
		get
		{
			if (volumes == null)
			{
				volumes = new List<DungeonVolume>();
				((Component)this).GetComponentsInChildren<DungeonVolume>(true, volumes);
			}
			return volumes;
		}
	}

	protected void Start()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)TerrainMeta.Path == (Object)null))
		{
			Dungeon = TerrainMeta.Path.FindClosest(TerrainMeta.Path.DungeonBaseEntrances, ((Component)this).get_transform().get_position());
			if (!((Object)(object)Dungeon == (Object)null))
			{
				Dungeon.Add(this);
			}
		}
	}

	public DungeonBaseLink()
		: this()
	{
	}
}
