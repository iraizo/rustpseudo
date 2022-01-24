using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;
using UnityEngine.AI;

public class AsyncTerrainNavMeshBake : CustomYieldInstruction
{
	private List<int> indices;

	private List<Vector3> vertices;

	private List<Vector3> normals;

	private List<int> triangles;

	private Vector3 pivot;

	private int width;

	private int height;

	private bool normal;

	private bool alpha;

	private Action worker;

	public override bool keepWaiting => worker != null;

	public bool isDone => worker == null;

	public Mesh mesh
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			Mesh val = new Mesh();
			if (vertices != null)
			{
				val.SetVertices(vertices);
				Pool.FreeList<Vector3>(ref vertices);
			}
			if (normals != null)
			{
				val.SetNormals(normals);
				Pool.FreeList<Vector3>(ref normals);
			}
			if (triangles != null)
			{
				val.SetTriangles(triangles, 0);
				Pool.FreeList<int>(ref triangles);
			}
			if (indices != null)
			{
				Pool.FreeList<int>(ref indices);
			}
			return val;
		}
	}

	public NavMeshBuildSource CreateNavMeshBuildSource()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		NavMeshBuildSource result = default(NavMeshBuildSource);
		((NavMeshBuildSource)(ref result)).set_transform(Matrix4x4.TRS(pivot, Quaternion.get_identity(), Vector3.get_one()));
		((NavMeshBuildSource)(ref result)).set_shape((NavMeshBuildSourceShape)0);
		((NavMeshBuildSource)(ref result)).set_sourceObject((Object)(object)mesh);
		return result;
	}

	public NavMeshBuildSource CreateNavMeshBuildSource(int area)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		NavMeshBuildSource result = CreateNavMeshBuildSource();
		((NavMeshBuildSource)(ref result)).set_area(area);
		return result;
	}

	public AsyncTerrainNavMeshBake(Vector3 pivot, int width, int height, bool normal, bool alpha)
		: this()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		this.pivot = pivot;
		this.width = width;
		this.height = height;
		this.normal = normal;
		this.alpha = alpha;
		indices = Pool.GetList<int>();
		vertices = Pool.GetList<Vector3>();
		normals = (normal ? Pool.GetList<Vector3>() : null);
		triangles = Pool.GetList<int>();
		Invoke();
	}

	private void DoWork()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector((float)(width / 2), 0f, (float)(height / 2));
		Vector3 val2 = default(Vector3);
		((Vector3)(ref val2))._002Ector(pivot.x - val.x, 0f, pivot.z - val.z);
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainAlphaMap alphaMap = TerrainMeta.AlphaMap;
		int num = 0;
		for (int i = 0; i <= height; i++)
		{
			int num2 = 0;
			while (num2 <= width)
			{
				Vector3 worldPos = new Vector3((float)num2, 0f, (float)i) + val2;
				Vector3 item = new Vector3((float)num2, 0f, (float)i) - val;
				float num3 = heightMap.GetHeight(worldPos);
				if (num3 < -1f)
				{
					indices.Add(-1);
				}
				else if (alpha && alphaMap.GetAlpha(worldPos) < 0.1f)
				{
					indices.Add(-1);
				}
				else
				{
					if (normal)
					{
						Vector3 item2 = heightMap.GetNormal(worldPos);
						normals.Add(item2);
					}
					worldPos.y = (item.y = num3 - pivot.y);
					indices.Add(vertices.Count);
					vertices.Add(item);
				}
				num2++;
				num++;
			}
		}
		int num4 = 0;
		int num5 = 0;
		while (num5 < height)
		{
			int num6 = 0;
			while (num6 < width)
			{
				int num7 = indices[num4];
				int num8 = indices[num4 + width + 1];
				int num9 = indices[num4 + 1];
				int num10 = indices[num4 + 1];
				int num11 = indices[num4 + width + 1];
				int num12 = indices[num4 + width + 2];
				if (num7 != -1 && num8 != -1 && num9 != -1)
				{
					triangles.Add(num7);
					triangles.Add(num8);
					triangles.Add(num9);
				}
				if (num10 != -1 && num11 != -1 && num12 != -1)
				{
					triangles.Add(num10);
					triangles.Add(num11);
					triangles.Add(num12);
				}
				num6++;
				num4++;
			}
			num5++;
			num4++;
		}
	}

	private void Invoke()
	{
		worker = DoWork;
		worker.BeginInvoke(Callback, null);
	}

	private void Callback(IAsyncResult result)
	{
		worker.EndInvoke(result);
		worker = null;
	}
}
