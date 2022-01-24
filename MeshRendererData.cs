using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;

public class MeshRendererData
{
	public List<int> triangles;

	public List<Vector3> vertices;

	public List<Vector3> normals;

	public List<Vector4> tangents;

	public List<Color32> colors32;

	public List<Vector2> uv;

	public List<Vector2> uv2;

	public List<Vector4> positions;

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
				Debug.LogWarning((object)"Skipping renderer normals because some meshes were missing them.");
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
				Debug.LogWarning((object)"Skipping renderer tangents because some meshes were missing them.");
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
				Debug.LogWarning((object)"Skipping renderer colors because some meshes were missing them.");
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
				Debug.LogWarning((object)"Skipping renderer uvs because some meshes were missing them.");
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
				Debug.LogWarning((object)"Skipping renderer uv2s because some meshes were missing them.");
			}
		}
		if (positions != null)
		{
			mesh.SetUVs(2, positions);
		}
	}

	public void Combine(MeshRendererGroup meshGroup)
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
			MeshRendererInstance meshRendererInstance = meshGroup.data[i];
			Matrix4x4 val = Matrix4x4.TRS(meshRendererInstance.position, meshRendererInstance.rotation, meshRendererInstance.scale);
			int count = vertices.Count;
			for (int j = 0; j < meshRendererInstance.data.triangles.Length; j++)
			{
				triangles.Add(count + meshRendererInstance.data.triangles[j]);
			}
			for (int k = 0; k < meshRendererInstance.data.vertices.Length; k++)
			{
				vertices.Add(((Matrix4x4)(ref val)).MultiplyPoint3x4(meshRendererInstance.data.vertices[k]));
				positions.Add(Vector4.op_Implicit(meshRendererInstance.position));
			}
			for (int l = 0; l < meshRendererInstance.data.normals.Length; l++)
			{
				normals.Add(((Matrix4x4)(ref val)).MultiplyVector(meshRendererInstance.data.normals[l]));
			}
			for (int m = 0; m < meshRendererInstance.data.tangents.Length; m++)
			{
				Vector4 val2 = meshRendererInstance.data.tangents[m];
				((Vector3)(ref val3))._002Ector(val2.x, val2.y, val2.z);
				Vector3 val4 = ((Matrix4x4)(ref val)).MultiplyVector(val3);
				tangents.Add(new Vector4(val4.x, val4.y, val4.z, val2.w));
			}
			for (int n = 0; n < meshRendererInstance.data.colors32.Length; n++)
			{
				colors32.Add(meshRendererInstance.data.colors32[n]);
			}
			for (int num = 0; num < meshRendererInstance.data.uv.Length; num++)
			{
				uv.Add(meshRendererInstance.data.uv[num]);
			}
			for (int num2 = 0; num2 < meshRendererInstance.data.uv2.Length; num2++)
			{
				uv2.Add(meshRendererInstance.data.uv2[num2]);
			}
		}
	}

	public void Combine(MeshRendererGroup meshGroup, MeshRendererLookup rendererLookup)
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
			MeshRendererInstance instance = meshGroup.data[i];
			Matrix4x4 val = Matrix4x4.TRS(instance.position, instance.rotation, instance.scale);
			int count = vertices.Count;
			for (int j = 0; j < instance.data.triangles.Length; j++)
			{
				triangles.Add(count + instance.data.triangles[j]);
			}
			for (int k = 0; k < instance.data.vertices.Length; k++)
			{
				vertices.Add(((Matrix4x4)(ref val)).MultiplyPoint3x4(instance.data.vertices[k]));
				positions.Add(Vector4.op_Implicit(instance.position));
			}
			for (int l = 0; l < instance.data.normals.Length; l++)
			{
				normals.Add(((Matrix4x4)(ref val)).MultiplyVector(instance.data.normals[l]));
			}
			for (int m = 0; m < instance.data.tangents.Length; m++)
			{
				Vector4 val2 = instance.data.tangents[m];
				((Vector3)(ref val3))._002Ector(val2.x, val2.y, val2.z);
				Vector3 val4 = ((Matrix4x4)(ref val)).MultiplyVector(val3);
				tangents.Add(new Vector4(val4.x, val4.y, val4.z, val2.w));
			}
			for (int n = 0; n < instance.data.colors32.Length; n++)
			{
				colors32.Add(instance.data.colors32[n]);
			}
			for (int num = 0; num < instance.data.uv.Length; num++)
			{
				uv.Add(instance.data.uv[num]);
			}
			for (int num2 = 0; num2 < instance.data.uv2.Length; num2++)
			{
				uv2.Add(instance.data.uv2[num2]);
			}
			rendererLookup.Add(instance);
		}
	}
}
