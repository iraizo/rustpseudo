using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenerateRoadTopology : ProceduralComponent
{
	private const int Smoothen = 8;

	public override void Process(uint seed)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		List<PathList> roads = TerrainMeta.Path.Roads;
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		foreach (PathList item in roads)
		{
			if (!World.Networked)
			{
				PathInterpolator path = item.Path;
				Vector3[] points = path.Points;
				for (int i = 0; i < points.Length; i++)
				{
					Vector3 val = points[i];
					val.y = heightMap.GetHeight(val);
					points[i] = val;
				}
				item.TrimTopology(2048);
				path.Smoothen(8, Vector3.get_up());
				path.RecalculateTangents();
				item.ResetTrims();
			}
			heightMap.Push();
			item.AdjustTerrainHeight();
			heightMap.Pop();
		}
		foreach (PathList item2 in Enumerable.Reverse<PathList>(Enumerable.AsEnumerable<PathList>((IEnumerable<PathList>)roads)))
		{
			item2.AdjustTerrainTexture();
			item2.AdjustTerrainTopology();
		}
		MarkRoadside();
		TerrainMeta.PlacementMap.Reset();
	}

	private void MarkRoadside()
	{
		TerrainHeightMap heightmap = TerrainMeta.HeightMap;
		TerrainTopologyMap topomap = TerrainMeta.TopologyMap;
		int[] map = topomap.dst;
		int res = topomap.res;
		ImageProcessing.Dilate2D(map, res, res, 6144, 6, delegate(int x, int y)
		{
			if (((uint)map[x * res + y] & 0x31u) != 0)
			{
				map[x * res + y] |= 4096;
			}
			float normX = topomap.Coordinate(x);
			float normZ = topomap.Coordinate(y);
			if (heightmap.GetSlope(normX, normZ) > 40f)
			{
				map[x * res + y] |= 2;
			}
		});
	}
}
