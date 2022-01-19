using System.Collections.Generic;
using UnityEngine;

public class GenerateRiverLayout : ProceduralComponent
{
	public const float Width = 36f;

	public const float InnerPadding = 1f;

	public const float OuterPadding = 1f;

	public const float InnerFade = 10f;

	public const float OuterFade = 20f;

	public const float RandomScale = 0.75f;

	public const float MeshOffset = -0.5f;

	public const float TerrainOffset = -1.5f;

	private const int Smoothen = 4;

	public override void Process(uint seed)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		if (World.Networked)
		{
			TerrainMeta.Path.Rivers.Clear();
			TerrainMeta.Path.Rivers.AddRange(World.GetPaths("River"));
			return;
		}
		List<PathList> list = new List<PathList>();
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		List<Vector3> list2 = new List<Vector3>();
		Vector3 val = default(Vector3);
		for (float num = TerrainMeta.Position.z; num < TerrainMeta.Position.z + TerrainMeta.Size.z; num += 50f)
		{
			for (float num2 = TerrainMeta.Position.x; num2 < TerrainMeta.Position.x + TerrainMeta.Size.x; num2 += 50f)
			{
				((Vector3)(ref val))._002Ector(num2, 0f, num);
				float num3 = (val.y = heightMap.GetHeight(val));
				if (val.y <= 5f)
				{
					continue;
				}
				Vector3 normal = heightMap.GetNormal(val);
				if (normal.y <= 0.01f)
				{
					continue;
				}
				Vector2 val2 = new Vector2(normal.x, normal.z);
				Vector2 normalized = ((Vector2)(ref val2)).get_normalized();
				list2.Add(val);
				float radius = 18f;
				int num4 = 18;
				for (int i = 0; i < 10000; i++)
				{
					val.x += normalized.x;
					val.z += normalized.y;
					if (heightMap.GetSlope(val) > 30f)
					{
						break;
					}
					float height = heightMap.GetHeight(val);
					if (height > num3 + 10f)
					{
						break;
					}
					float num5 = Mathf.Min(height, num3);
					val.y = Mathf.Lerp(val.y, num5, 0.15f);
					int topology = topologyMap.GetTopology(val, radius);
					int topology2 = topologyMap.GetTopology(val);
					int num6 = 2694148;
					int num7 = 128;
					if ((topology & num6) != 0)
					{
						list2.Add(val);
						break;
					}
					if ((topology2 & num7) != 0 && --num4 <= 0)
					{
						list2.Add(val);
						if (list2.Count >= 25)
						{
							int num8 = TerrainMeta.Path.Rivers.Count + list.Count;
							PathList pathList = new PathList("River " + num8, list2.ToArray());
							pathList.Width = 36f;
							pathList.InnerPadding = 1f;
							pathList.OuterPadding = 1f;
							pathList.InnerFade = 10f;
							pathList.OuterFade = 20f;
							pathList.RandomScale = 0.75f;
							pathList.MeshOffset = -0.5f;
							pathList.TerrainOffset = -1.5f;
							pathList.Topology = 16384;
							pathList.Splat = 64;
							pathList.Start = true;
							pathList.End = true;
							list.Add(pathList);
						}
						break;
					}
					if (i % 12 == 0)
					{
						list2.Add(val);
					}
					normal = heightMap.GetNormal(val);
					val2 = new Vector2(normalized.x + 0.15f * normal.x, normalized.y + 0.15f * normal.z);
					normalized = ((Vector2)(ref val2)).get_normalized();
					num3 = num5;
				}
				list2.Clear();
			}
		}
		list.Sort((PathList a, PathList b) => b.Path.Points.Length.CompareTo(a.Path.Points.Length));
		int num9 = Mathf.RoundToInt(10f * TerrainMeta.Size.x * TerrainMeta.Size.z * 1E-06f);
		int num10 = Mathf.NextPowerOfTwo((int)((float)World.Size / 36f));
		bool[,] array = new bool[num10, num10];
		for (int j = 0; j < list.Count; j++)
		{
			if (j >= num9)
			{
				ListEx.RemoveUnordered<PathList>(list, j--);
				continue;
			}
			PathList pathList2 = list[j];
			bool flag = false;
			for (int k = 0; k < j; k++)
			{
				if (Vector3.Distance(list[k].Path.GetStartPoint(), pathList2.Path.GetStartPoint()) < 100f)
				{
					ListEx.RemoveUnordered<PathList>(list, j--);
					flag = true;
					break;
				}
			}
			if (flag)
			{
				continue;
			}
			int num11 = -1;
			int num12 = -1;
			for (int l = 0; l < pathList2.Path.Points.Length; l++)
			{
				Vector3 val3 = pathList2.Path.Points[l];
				int num13 = Mathf.Clamp((int)(TerrainMeta.NormalizeX(val3.x) * (float)num10), 0, num10 - 1);
				int num14 = Mathf.Clamp((int)(TerrainMeta.NormalizeZ(val3.z) * (float)num10), 0, num10 - 1);
				if (num11 == num13 && num12 == num14)
				{
					continue;
				}
				if (array[num14, num13])
				{
					ListEx.RemoveUnordered<PathList>(list, j--);
					flag = true;
					break;
				}
				if (num11 != num13 && num12 != num14)
				{
					if (num11 != -1)
					{
						array[num14, num11] = true;
					}
					if (num12 != -1)
					{
						array[num12, num13] = true;
					}
					num11 = num13;
					num12 = num14;
					array[num14, num13] = true;
				}
				else
				{
					num11 = num13;
					num12 = num14;
					array[num14, num13] = true;
				}
			}
		}
		for (int m = 0; m < list.Count; m++)
		{
			list[m].Name = "River " + (TerrainMeta.Path.Rivers.Count + m);
		}
		foreach (PathList item in list)
		{
			item.Path.Smoothen(4);
			item.Path.RecalculateTangents();
		}
		TerrainMeta.Path.Rivers.AddRange(list);
	}
}
