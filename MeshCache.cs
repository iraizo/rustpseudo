using System;
using System.Collections.Generic;
using UnityEngine;

public static class MeshCache
{
	[Serializable]
	public class Data
	{
		public Mesh mesh;

		public Vector3[] vertices;

		public Vector3[] normals;

		public Vector4[] tangents;

		public Color32[] colors32;

		public int[] triangles;

		public Vector2[] uv;

		public Vector2[] uv2;

		public Vector2[] uv3;

		public Vector2[] uv4;
	}

	public static Dictionary<Mesh, Data> dictionary = new Dictionary<Mesh, Data>();

	public static Data Get(Mesh mesh)
	{
		if (!dictionary.TryGetValue(mesh, out var value))
		{
			value = new Data();
			value.mesh = mesh;
			value.vertices = mesh.get_vertices();
			value.normals = mesh.get_normals();
			value.tangents = mesh.get_tangents();
			value.colors32 = mesh.get_colors32();
			value.triangles = mesh.get_triangles();
			value.uv = mesh.get_uv();
			value.uv2 = mesh.get_uv2();
			value.uv3 = mesh.get_uv3();
			value.uv4 = mesh.get_uv4();
			dictionary.Add(mesh, value);
		}
		return value;
	}
}
