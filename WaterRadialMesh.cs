using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WaterRadialMesh
{
	private const float AlignmentGranularity = 1f;

	private const float MaxHorizontalDisplacement = 1f;

	private Mesh[] meshes;

	private bool initialized;

	public Mesh[] Meshes => meshes;

	public bool IsInitialized => initialized;

	public void Initialize(int vertexCount)
	{
		meshes = GenerateMeshes(vertexCount);
		initialized = true;
	}

	public void Destroy()
	{
		if (initialized)
		{
			Mesh[] array = meshes;
			for (int i = 0; i < array.Length; i++)
			{
				Object.DestroyImmediate((Object)(object)array[i]);
			}
			meshes = null;
			initialized = false;
		}
	}

	private Mesh CreateMesh(string name, Vector3[] vertices, int[] indices)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		Mesh val = new Mesh();
		((Object)val).set_hideFlags((HideFlags)52);
		((Object)val).set_name(name);
		val.set_vertices(vertices);
		val.SetIndices(indices, (MeshTopology)2, 0);
		val.RecalculateBounds();
		val.UploadMeshData(true);
		return val;
	}

	private Mesh[] GenerateMeshes(int vertexCount, bool volume = false)
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		int num = Mathf.RoundToInt((float)Mathf.RoundToInt(Mathf.Sqrt((float)vertexCount)) * 0.4f);
		int num2 = Mathf.RoundToInt((float)vertexCount / (float)num);
		int num3 = (volume ? (num2 / 2) : num2);
		List<Mesh> list = new List<Mesh>();
		List<Vector3> list2 = new List<Vector3>();
		List<int> list3 = new List<int>();
		Vector2[] array = (Vector2[])(object)new Vector2[num];
		int num4 = 0;
		int num5 = 0;
		for (int i = 0; i < num; i++)
		{
			float num6 = ((float)i / (float)(num - 1) * 2f - 1f) * (float)Math.PI * 0.25f;
			int num7 = i;
			Vector2 val = new Vector2(Mathf.Sin(num6), Mathf.Cos(num6));
			array[num7] = ((Vector2)(ref val)).get_normalized();
		}
		for (int j = 0; j < num3; j++)
		{
			float num8 = (float)j / (float)(num2 - 1);
			num8 = 1f - Mathf.Cos(num8 * (float)Math.PI * 0.5f);
			for (int k = 0; k < num; k++)
			{
				Vector2 val2 = array[k] * num8;
				if (j < num3 - 2 || !volume)
				{
					list2.Add(new Vector3(val2.x, 0f, val2.y));
				}
				else if (j == num3 - 2)
				{
					list2.Add(new Vector3(val2.x * 10f, -0.9f, val2.y) * 0.5f);
				}
				else
				{
					list2.Add(new Vector3(val2.x * 10f, -0.9f, val2.y * -10f) * 0.5f);
				}
				if (k != 0 && j != 0 && num4 > num)
				{
					list3.Add(num4);
					list3.Add(num4 - num);
					list3.Add(num4 - num - 1);
					list3.Add(num4 - 1);
				}
				num4++;
				if (num4 >= 65000)
				{
					list.Add(CreateMesh("WaterMesh_" + num + "x" + num2 + "_" + num5, list2.ToArray(), list3.ToArray()));
					k--;
					j--;
					num8 = 1f - Mathf.Cos((float)j / (float)(num2 - 1) * (float)Math.PI * 0.5f);
					num4 = 0;
					list2.Clear();
					list3.Clear();
					num5++;
				}
			}
		}
		if (num4 != 0)
		{
			list.Add(CreateMesh("WaterMesh_" + num + "x" + num2 + "_" + num5, list2.ToArray(), list3.ToArray()));
		}
		return list.ToArray();
	}

	private Vector3 RaycastPlane(Camera camera, float planeHeight, Vector3 pos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		Ray val = camera.ViewportPointToRay(pos);
		if (((Component)camera).get_transform().get_position().y > planeHeight)
		{
			if (((Ray)(ref val)).get_direction().y > -0.01f)
			{
				((Ray)(ref val)).set_direction(new Vector3(((Ray)(ref val)).get_direction().x, 0f - ((Ray)(ref val)).get_direction().y - 0.02f, ((Ray)(ref val)).get_direction().z));
			}
		}
		else if (((Ray)(ref val)).get_direction().y < 0.01f)
		{
			((Ray)(ref val)).set_direction(new Vector3(((Ray)(ref val)).get_direction().x, 0f - ((Ray)(ref val)).get_direction().y + 0.02f, ((Ray)(ref val)).get_direction().z));
		}
		float num = (0f - (((Ray)(ref val)).get_origin().y - planeHeight)) / ((Ray)(ref val)).get_direction().y;
		return Quaternion.AngleAxis(0f - ((Component)camera).get_transform().get_eulerAngles().y, Vector3.get_up()) * (((Ray)(ref val)).get_direction() * num);
	}

	public Matrix4x4 ComputeLocalToWorldMatrix(Camera camera, float oceanWaterLevel)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)camera == (Object)null)
		{
			return Matrix4x4.get_identity();
		}
		Matrix4x4 worldToCameraMatrix = camera.get_worldToCameraMatrix();
		Vector3 val = ((Matrix4x4)(ref worldToCameraMatrix)).MultiplyVector(Vector3.get_up());
		worldToCameraMatrix = camera.get_worldToCameraMatrix();
		Vector3 val2 = ((Matrix4x4)(ref worldToCameraMatrix)).MultiplyVector(Vector3.Cross(((Component)camera).get_transform().get_forward(), Vector3.get_up()));
		Vector3 val3 = new Vector3(val.x, val.y, 0f);
		val = ((Vector3)(ref val3)).get_normalized() * 0.5f + new Vector3(0.5f, 0f, 0.5f);
		val3 = new Vector3(val2.x, val2.y, 0f);
		val2 = ((Vector3)(ref val3)).get_normalized() * 0.5f;
		Vector3 val4 = RaycastPlane(camera, oceanWaterLevel, val - val2);
		Vector3 val5 = RaycastPlane(camera, oceanWaterLevel, val + val2);
		float num = Mathf.Min(camera.get_farClipPlane(), 5000f);
		Vector3 position = ((Component)camera).get_transform().get_position();
		Vector3 val6 = default(Vector3);
		val6.x = num * Mathf.Tan(camera.get_fieldOfView() * 0.5f * ((float)Math.PI / 180f)) * camera.get_aspect() + 2f;
		val6.y = num;
		val6.z = num;
		float num2 = Mathf.Abs(val5.x - val4.x);
		float num3 = Mathf.Min(val4.z, val5.z) - (num2 + 2f) * val6.z / val6.x;
		num3 = Mathf.Min(num3, -15f);
		Vector3 forward = ((Component)camera).get_transform().get_forward();
		forward.y = 0f;
		((Vector3)(ref forward)).Normalize();
		val6.z -= num3;
		position = new Vector3(position.x, oceanWaterLevel, position.z) + forward * num3;
		Quaternion val7 = Quaternion.AngleAxis(Mathf.Atan2(forward.x, forward.z) * 57.29578f, Vector3.get_up());
		return Matrix4x4.TRS(position, val7, val6);
	}
}
