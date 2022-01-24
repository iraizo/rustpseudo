using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;

public class MeshData
{
	public List<int> triangles;

	public List<Vector3> vertices;

	public List<Vector3> normals;

	public List<Vector4> tangents;

	public List<Color32> colors32;

	public List<Vector2> uv;

	public List<Vector2> uv2;

	public List<Vector4> positions;

	public void AllocMinimal()
	{
		if (triangles == null)
		{
			triangles = Pool.GetList<int>();
		}
		if (vertices == null)
		{
			vertices = Pool.GetList<Vector3>();
		}
		if (normals == null)
		{
			normals = Pool.GetList<Vector3>();
		}
		if (uv == null)
		{
			uv = Pool.GetList<Vector2>();
		}
	}

	public void Alloc()
	{
		if (triangles == null)
		{
			triangles = Pool.GetList<int>();
		}
		if (vertices == null)
		{
			vertices = Pool.GetList<Vector3>();
		}
		if (normals == null)
		{
			normals = Pool.GetList<Vector3>();
		}
		if (tangents == null)
		{
			tangents = Pool.GetList<Vector4>();
		}
		if (colors32 == null)
		{
			colors32 = Pool.GetList<Color32>();
		}
		if (uv == null)
		{
			uv = Pool.GetList<Vector2>();
		}
		if (uv2 == null)
		{
			uv2 = Pool.GetList<Vector2>();
		}
		if (positions == null)
		{
			positions = Pool.GetList<Vector4>();
		}
	}

	public void Free()
	{
		if (triangles != null)
		{
			Pool.FreeList<int>(ref triangles);
		}
		if (vertices != null)
		{
			Pool.FreeList<Vector3>(ref vertices);
		}
		if (normals != null)
		{
			Pool.FreeList<Vector3>(ref normals);
		}
		if (tangents != null)
		{
			Pool.FreeList<Vector4>(ref tangents);
		}
		if (colors32 != null)
		{
			Pool.FreeList<Color32>(ref colors32);
		}
		if (uv != null)
		{
			Pool.FreeList<Vector2>(ref uv);
		}
		if (uv2 != null)
		{
			Pool.FreeList<Vector2>(ref uv2);
		}
		if (positions != null)
		{
			Pool.FreeList<Vector4>(ref positions);
		}
	}

	public void Clear()
	{
		if (triangles != null)
		{
			triangles.Clear();
		}
		if (vertices != null)
		{
			vertices.Clear();
		}
		if (normals != null)
		{
			normals.Clear();
		}
		if (tangents != null)
		{
			tangents.Clear();
		}
		if (colors32 != null)
		{
			colors32.Clear();
		}
		if (uv != null)
		{
			uv.Clear();
		}
		if (uv2 != null)
		{
			uv2.Clear();
		}
		if (positions != null)
		{
			positions.Clear();
		}
	}

	public void Apply(Mesh mesh)
	{
		mesh.Clear();
		if (vertices != null)
		{
			mesh.SetVertices(vertices);
		}
		if (triangles != null)
		{
			mesh.SetTriangles(triangles, 0);
		}
		if (normals != null)
		{
			if (normals.Count == vertices.Count)
			{
				mesh.SetNormals(normals);
			}
			else if (normals.Count > 0 && Batching.verbose > 0)
			{
				Debug.LogWarning((object)"Skipping mesh normals because some meshes were missing them.");
			}
		}
		if (tangents != null)
		{
			if (tangents.Count == vertices.Count)
			{
				mesh.SetTangents(tangents);
			}
			else if (tangents.Count > 0 && Batching.verbose > 0)
			{
				Debug.LogWarning((object)"Skipping mesh tangents because some meshes were missing them.");
			}
		}
		if (colors32 != null)
		{
			if (colors32.Count == vertices.Count)
			{
				mesh.SetColors(colors32);
			}
			else if (colors32.Count > 0 && Batching.verbose > 0)
			{
				Debug.LogWarning((object)"Skipping mesh colors because some meshes were missing them.");
			}
		}
		if (uv != null)
		{
			if (uv.Count == vertices.Count)
			{
				mesh.SetUVs(0, uv);
			}
			else if (uv.Count > 0 && Batching.verbose > 0)
			{
				Debug.LogWarning((object)"Skipping mesh uvs because some meshes were missing them.");
			}
		}
		if (uv2 != null)
		{
			if (uv2.Count == vertices.Count)
			{
				mesh.SetUVs(1, uv2);
			}
			else if (uv2.Count > 0 && Batching.verbose > 0)
			{
				Debug.LogWarning((object)"Skipping mesh uv2s because some meshes were missing them.");
			}
		}
		if (positions != null)
		{
			mesh.SetUVs(2, positions);
		}
	}

	public void Combine(MeshGroup meshGroup)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val3 = default(Vector3);
		for (int i = 0; i < meshGroup.data.Count; i++)
		{
			MeshInstance meshInstance = meshGroup.data[i];
			Matrix4x4 val = Matrix4x4.TRS(meshInstance.position, meshInstance.rotation, meshInstance.scale);
			int count = vertices.Count;
			for (int j = 0; j < meshInstance.data.triangles.Length; j++)
			{
				triangles.Add(count + meshInstance.data.triangles[j]);
			}
			for (int k = 0; k < meshInstance.data.vertices.Length; k++)
			{
				vertices.Add(((Matrix4x4)(ref val)).MultiplyPoint3x4(meshInstance.data.vertices[k]));
				positions.Add(Vector4.op_Implicit(meshInstance.position));
			}
			for (int l = 0; l < meshInstance.data.normals.Length; l++)
			{
				normals.Add(((Matrix4x4)(ref val)).MultiplyVector(meshInstance.data.normals[l]));
			}
			for (int m = 0; m < meshInstance.data.tangents.Length; m++)
			{
				Vector4 val2 = meshInstance.data.tangents[m];
				((Vector3)(ref val3))._002Ector(val2.x, val2.y, val2.z);
				Vector3 val4 = ((Matrix4x4)(ref val)).MultiplyVector(val3);
				tangents.Add(new Vector4(val4.x, val4.y, val4.z, val2.w));
			}
			for (int n = 0; n < meshInstance.data.colors32.Length; n++)
			{
				colors32.Add(meshInstance.data.colors32[n]);
			}
			for (int num = 0; num < meshInstance.data.uv.Length; num++)
			{
				uv.Add(meshInstance.data.uv[num]);
			}
			for (int num2 = 0; num2 < meshInstance.data.uv2.Length; num2++)
			{
				uv2.Add(meshInstance.data.uv2[num2]);
			}
		}
	}
}
