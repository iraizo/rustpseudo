using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class ReflectionProbeEx : MonoBehaviour
{
	private struct CubemapSkyboxVertex
	{
		public float x;

		public float y;

		public float z;

		public Color color;

		public float tu;

		public float tv;

		public float tw;
	}

	private struct CubemapFaceMatrices
	{
		public Matrix4x4 worldToView;

		public Matrix4x4 viewToWorld;

		public CubemapFaceMatrices(Vector3 x, Vector3 y, Vector3 z)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			worldToView = Matrix4x4.get_identity();
			((Matrix4x4)(ref worldToView)).set_Item(0, 0, ((Vector3)(ref x)).get_Item(0));
			((Matrix4x4)(ref worldToView)).set_Item(0, 1, ((Vector3)(ref x)).get_Item(1));
			((Matrix4x4)(ref worldToView)).set_Item(0, 2, ((Vector3)(ref x)).get_Item(2));
			((Matrix4x4)(ref worldToView)).set_Item(1, 0, ((Vector3)(ref y)).get_Item(0));
			((Matrix4x4)(ref worldToView)).set_Item(1, 1, ((Vector3)(ref y)).get_Item(1));
			((Matrix4x4)(ref worldToView)).set_Item(1, 2, ((Vector3)(ref y)).get_Item(2));
			((Matrix4x4)(ref worldToView)).set_Item(2, 0, ((Vector3)(ref z)).get_Item(0));
			((Matrix4x4)(ref worldToView)).set_Item(2, 1, ((Vector3)(ref z)).get_Item(1));
			((Matrix4x4)(ref worldToView)).set_Item(2, 2, ((Vector3)(ref z)).get_Item(2));
			viewToWorld = ((Matrix4x4)(ref worldToView)).get_inverse();
		}
	}

	[Serializable]
	public enum ConvolutionQuality
	{
		Lowest,
		Low,
		Medium,
		High,
		VeryHigh
	}

	[Serializable]
	public struct RenderListEntry
	{
		public Renderer renderer;

		public bool alwaysEnabled;

		public RenderListEntry(Renderer renderer, bool alwaysEnabled)
		{
			this.renderer = renderer;
			this.alwaysEnabled = alwaysEnabled;
		}
	}

	private Mesh blitMesh;

	private Mesh skyboxMesh;

	private static float[] octaVerts = new float[72]
	{
		0f, 1f, 0f, 0f, 0f, -1f, 1f, 0f, 0f, 0f,
		1f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 1f,
		0f, 0f, 0f, 1f, -1f, 0f, 0f, 0f, 1f, 0f,
		-1f, 0f, 0f, 0f, 0f, -1f, 0f, -1f, 0f, 1f,
		0f, 0f, 0f, 0f, -1f, 0f, -1f, 0f, 0f, 0f,
		1f, 1f, 0f, 0f, 0f, -1f, 0f, -1f, 0f, 0f,
		0f, 0f, 1f, 0f, -1f, 0f, 0f, 0f, -1f, -1f,
		0f, 0f
	};

	private static readonly CubemapFaceMatrices[] cubemapFaceMatrices = new CubemapFaceMatrices[6]
	{
		new CubemapFaceMatrices(new Vector3(0f, 0f, -1f), new Vector3(0f, -1f, 0f), new Vector3(-1f, 0f, 0f)),
		new CubemapFaceMatrices(new Vector3(0f, 0f, 1f), new Vector3(0f, -1f, 0f), new Vector3(1f, 0f, 0f)),
		new CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, 1f), new Vector3(0f, -1f, 0f)),
		new CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, -1f), new Vector3(0f, 1f, 0f)),
		new CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, -1f, 0f), new Vector3(0f, 0f, -1f)),
		new CubemapFaceMatrices(new Vector3(-1f, 0f, 0f), new Vector3(0f, -1f, 0f), new Vector3(0f, 0f, 1f))
	};

	private static readonly CubemapFaceMatrices[] cubemapFaceMatricesD3D11 = new CubemapFaceMatrices[6]
	{
		new CubemapFaceMatrices(new Vector3(0f, 0f, -1f), new Vector3(0f, 1f, 0f), new Vector3(-1f, 0f, 0f)),
		new CubemapFaceMatrices(new Vector3(0f, 0f, 1f), new Vector3(0f, 1f, 0f), new Vector3(1f, 0f, 0f)),
		new CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, -1f), new Vector3(0f, -1f, 0f)),
		new CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, 1f), new Vector3(0f, 1f, 0f)),
		new CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 1f, 0f), new Vector3(0f, 0f, -1f)),
		new CubemapFaceMatrices(new Vector3(-1f, 0f, 0f), new Vector3(0f, 1f, 0f), new Vector3(0f, 0f, 1f))
	};

	private static readonly CubemapFaceMatrices[] shadowCubemapFaceMatrices = new CubemapFaceMatrices[6]
	{
		new CubemapFaceMatrices(new Vector3(0f, 0f, 1f), new Vector3(0f, -1f, 0f), new Vector3(-1f, 0f, 0f)),
		new CubemapFaceMatrices(new Vector3(0f, 0f, -1f), new Vector3(0f, -1f, 0f), new Vector3(1f, 0f, 0f)),
		new CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, 1f), new Vector3(0f, 1f, 0f)),
		new CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, -1f), new Vector3(0f, -1f, 0f)),
		new CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, -1f, 0f), new Vector3(0f, 0f, 1f)),
		new CubemapFaceMatrices(new Vector3(-1f, 0f, 0f), new Vector3(0f, -1f, 0f), new Vector3(0f, 0f, -1f))
	};

	private CubemapFaceMatrices[] platformCubemapFaceMatrices;

	private static readonly int[] tab32 = new int[32]
	{
		0, 9, 1, 10, 13, 21, 2, 29, 11, 14,
		16, 18, 22, 25, 3, 30, 8, 12, 20, 28,
		15, 17, 24, 7, 19, 27, 23, 6, 26, 5,
		4, 31
	};

	public ReflectionProbeRefreshMode refreshMode = (ReflectionProbeRefreshMode)1;

	public bool timeSlicing;

	public int resolution = 128;

	[InspectorName("HDR")]
	public bool hdr = true;

	public float shadowDistance;

	public ReflectionProbeClearFlags clearFlags = (ReflectionProbeClearFlags)1;

	public Color background = new Color(0.192f, 0.301f, 0.474f);

	public float nearClip = 0.3f;

	public float farClip = 1000f;

	public Transform attachToTarget;

	public Light directionalLight;

	public float textureMipBias = 2f;

	public bool highPrecision;

	public bool enableShadows;

	public ConvolutionQuality convolutionQuality;

	public List<RenderListEntry> staticRenderList = new List<RenderListEntry>();

	public Cubemap reflectionCubemap;

	public float reflectionIntensity = 1f;

	private void CreateMeshes()
	{
		if ((Object)(object)blitMesh == (Object)null)
		{
			blitMesh = CreateBlitMesh();
		}
		if ((Object)(object)skyboxMesh == (Object)null)
		{
			skyboxMesh = CreateSkyboxMesh();
		}
	}

	private void DestroyMeshes()
	{
		if ((Object)(object)blitMesh != (Object)null)
		{
			Object.DestroyImmediate((Object)(object)blitMesh);
			blitMesh = null;
		}
		if ((Object)(object)skyboxMesh != (Object)null)
		{
			Object.DestroyImmediate((Object)(object)skyboxMesh);
			skyboxMesh = null;
		}
	}

	private static Mesh CreateBlitMesh()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		Mesh val = new Mesh();
		val.set_vertices((Vector3[])(object)new Vector3[4]
		{
			new Vector3(-1f, -1f, 0f),
			new Vector3(-1f, 1f, 0f),
			new Vector3(1f, 1f, 0f),
			new Vector3(1f, -1f, 0f)
		});
		val.set_uv((Vector2[])(object)new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f),
			new Vector2(1f, 0f)
		});
		val.set_triangles(new int[6] { 0, 1, 2, 0, 2, 3 });
		return val;
	}

	private static CubemapSkyboxVertex SubDivVert(CubemapSkyboxVertex v1, CubemapSkyboxVertex v2)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = new Vector3(v1.x, v1.y, v1.z);
		Vector3 val2 = default(Vector3);
		((Vector3)(ref val2))._002Ector(v2.x, v2.y, v2.z);
		Vector3 val3 = Vector3.Normalize(Vector3.Lerp(val, val2, 0.5f));
		CubemapSkyboxVertex result = default(CubemapSkyboxVertex);
		result.x = (result.tu = val3.x);
		result.y = (result.tv = val3.y);
		result.z = (result.tw = val3.z);
		result.color = Color.get_white();
		return result;
	}

	private static void Subdivide(List<CubemapSkyboxVertex> destArray, CubemapSkyboxVertex v1, CubemapSkyboxVertex v2, CubemapSkyboxVertex v3)
	{
		CubemapSkyboxVertex item = SubDivVert(v1, v2);
		CubemapSkyboxVertex item2 = SubDivVert(v2, v3);
		CubemapSkyboxVertex item3 = SubDivVert(v1, v3);
		destArray.Add(v1);
		destArray.Add(item);
		destArray.Add(item3);
		destArray.Add(item);
		destArray.Add(v2);
		destArray.Add(item2);
		destArray.Add(item2);
		destArray.Add(item3);
		destArray.Add(item);
		destArray.Add(v3);
		destArray.Add(item3);
		destArray.Add(item2);
	}

	private static void SubdivideYOnly(List<CubemapSkyboxVertex> destArray, CubemapSkyboxVertex v1, CubemapSkyboxVertex v2, CubemapSkyboxVertex v3)
	{
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.Abs(v2.y - v1.y);
		float num2 = Mathf.Abs(v2.y - v3.y);
		float num3 = Mathf.Abs(v3.y - v1.y);
		CubemapSkyboxVertex cubemapSkyboxVertex;
		CubemapSkyboxVertex cubemapSkyboxVertex2;
		CubemapSkyboxVertex cubemapSkyboxVertex3;
		if (num < num2 && num < num3)
		{
			cubemapSkyboxVertex = v3;
			cubemapSkyboxVertex2 = v1;
			cubemapSkyboxVertex3 = v2;
		}
		else if (num2 < num && num2 < num3)
		{
			cubemapSkyboxVertex = v1;
			cubemapSkyboxVertex2 = v2;
			cubemapSkyboxVertex3 = v3;
		}
		else
		{
			cubemapSkyboxVertex = v2;
			cubemapSkyboxVertex2 = v3;
			cubemapSkyboxVertex3 = v1;
		}
		CubemapSkyboxVertex item = SubDivVert(cubemapSkyboxVertex, cubemapSkyboxVertex2);
		CubemapSkyboxVertex item2 = SubDivVert(cubemapSkyboxVertex, cubemapSkyboxVertex3);
		destArray.Add(cubemapSkyboxVertex);
		destArray.Add(item);
		destArray.Add(item2);
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(item2.x - cubemapSkyboxVertex2.x, item2.y - cubemapSkyboxVertex2.y, item2.z - cubemapSkyboxVertex2.z);
		Vector3 val2 = default(Vector3);
		((Vector3)(ref val2))._002Ector(item.x - cubemapSkyboxVertex3.x, item.y - cubemapSkyboxVertex3.y, item.z - cubemapSkyboxVertex3.z);
		if (val.x * val.x + val.y * val.y + val.z * val.z > val2.x * val2.x + val2.y * val2.y + val2.z * val2.z)
		{
			destArray.Add(item);
			destArray.Add(cubemapSkyboxVertex2);
			destArray.Add(cubemapSkyboxVertex3);
			destArray.Add(item2);
			destArray.Add(item);
			destArray.Add(cubemapSkyboxVertex3);
		}
		else
		{
			destArray.Add(item2);
			destArray.Add(item);
			destArray.Add(cubemapSkyboxVertex2);
			destArray.Add(item2);
			destArray.Add(cubemapSkyboxVertex2);
			destArray.Add(cubemapSkyboxVertex3);
		}
	}

	private static Mesh CreateSkyboxMesh()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Expected O, but got Unknown
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		List<CubemapSkyboxVertex> list = new List<CubemapSkyboxVertex>();
		for (int i = 0; i < 24; i++)
		{
			CubemapSkyboxVertex item = default(CubemapSkyboxVertex);
			Vector3 val = Vector3.Normalize(new Vector3(octaVerts[i * 3], octaVerts[i * 3 + 1], octaVerts[i * 3 + 2]));
			item.x = (item.tu = val.x);
			item.y = (item.tv = val.y);
			item.z = (item.tw = val.z);
			item.color = Color.get_white();
			list.Add(item);
		}
		for (int j = 0; j < 3; j++)
		{
			List<CubemapSkyboxVertex> list2 = new List<CubemapSkyboxVertex>(list.Count);
			list2.AddRange(list);
			int count = list2.Count;
			list.Clear();
			list.Capacity = count * 4;
			for (int k = 0; k < count; k += 3)
			{
				Subdivide(list, list2[k], list2[k + 1], list2[k + 2]);
			}
		}
		for (int l = 0; l < 2; l++)
		{
			List<CubemapSkyboxVertex> list3 = new List<CubemapSkyboxVertex>(list.Count);
			list3.AddRange(list);
			int count2 = list3.Count;
			float num = Mathf.Pow(0.5f, (float)l + 1f);
			list.Clear();
			list.Capacity = count2 * 4;
			for (int m = 0; m < count2; m += 3)
			{
				if (Mathf.Max(Mathf.Max(Mathf.Abs(list3[m].y), Mathf.Abs(list3[m + 1].y)), Mathf.Abs(list3[m + 2].y)) > num)
				{
					list.Add(list3[m]);
					list.Add(list3[m + 1]);
					list.Add(list3[m + 2]);
				}
				else
				{
					SubdivideYOnly(list, list3[m], list3[m + 1], list3[m + 2]);
				}
			}
		}
		Mesh val2 = new Mesh();
		Vector3[] array = (Vector3[])(object)new Vector3[list.Count];
		Vector2[] array2 = (Vector2[])(object)new Vector2[list.Count];
		int[] array3 = new int[list.Count];
		for (int n = 0; n < list.Count; n++)
		{
			array[n] = new Vector3(list[n].x, list[n].y, list[n].z);
			array2[n] = Vector2.op_Implicit(new Vector3(list[n].tu, list[n].tv));
			array3[n] = n;
		}
		val2.set_vertices(array);
		val2.set_uv(array2);
		val2.set_triangles(array3);
		return val2;
	}

	private bool InitializeCubemapFaceMatrices()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected I4, but got Unknown
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		GraphicsDeviceType graphicsDeviceType = SystemInfo.get_graphicsDeviceType();
		if ((int)graphicsDeviceType != 2)
		{
			switch (graphicsDeviceType - 16)
			{
			case 1:
				platformCubemapFaceMatrices = cubemapFaceMatrices;
				break;
			case 2:
				platformCubemapFaceMatrices = cubemapFaceMatricesD3D11;
				break;
			case 5:
				platformCubemapFaceMatrices = cubemapFaceMatricesD3D11;
				break;
			case 0:
				platformCubemapFaceMatrices = cubemapFaceMatricesD3D11;
				break;
			default:
				platformCubemapFaceMatrices = null;
				break;
			}
		}
		else
		{
			platformCubemapFaceMatrices = cubemapFaceMatricesD3D11;
		}
		if (platformCubemapFaceMatrices == null)
		{
			Debug.LogError((object)("[ReflectionProbeEx] Initialization failed. No cubemap ortho basis defined for " + SystemInfo.get_graphicsDeviceType()));
			return false;
		}
		return true;
	}

	private int FastLog2(int value)
	{
		value |= value >> 1;
		value |= value >> 2;
		value |= value >> 4;
		value |= value >> 8;
		value |= value >> 16;
		return tab32[(uint)((long)value * 130329821L) >> 27];
	}

	private uint ReverseBits(uint bits)
	{
		bits = (bits << 16) | (bits >> 16);
		bits = ((bits & 0xFF00FF) << 8) | ((bits & 0xFF00FF00u) >> 8);
		bits = ((bits & 0xF0F0F0F) << 4) | ((bits & 0xF0F0F0F0u) >> 4);
		bits = ((bits & 0x33333333) << 2) | ((bits & 0xCCCCCCCCu) >> 2);
		bits = ((bits & 0x55555555) << 1) | ((bits & 0xAAAAAAAAu) >> 1);
		return bits;
	}

	private void SafeCreateMaterial(ref Material mat, Shader shader)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		if ((Object)(object)mat == (Object)null)
		{
			mat = new Material(shader);
		}
	}

	private void SafeCreateMaterial(ref Material mat, string shaderName)
	{
		if ((Object)(object)mat == (Object)null)
		{
			SafeCreateMaterial(ref mat, Shader.Find(shaderName));
		}
	}

	private void SafeCreateCubeRT(ref RenderTexture rt, string name, int size, int depth, bool mips, TextureDimension dim, FilterMode filter, RenderTextureFormat format, RenderTextureReadWrite readWrite = 1)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Invalid comparison between Unknown and I4
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)rt == (Object)null || !rt.IsCreated())
		{
			this.SafeDestroy<RenderTexture>(ref rt);
			RenderTexture val = new RenderTexture(size, size, depth, format, readWrite);
			((Object)val).set_hideFlags((HideFlags)52);
			rt = val;
			((Object)rt).set_name(name);
			((Texture)rt).set_dimension(dim);
			if ((int)dim == 5)
			{
				rt.set_volumeDepth(6);
			}
			rt.set_useMipMap(mips);
			rt.set_autoGenerateMips(false);
			((Texture)rt).set_filterMode(filter);
			((Texture)rt).set_anisoLevel(0);
			rt.Create();
		}
	}

	private void SafeCreateCB(ref CommandBuffer cb, string name)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		if (cb == null)
		{
			cb = new CommandBuffer();
			cb.set_name(name);
		}
	}

	private void SafeDestroy<T>(ref T obj) where T : Object
	{
		if ((Object)(object)obj != (Object)null)
		{
			Object.DestroyImmediate((Object)(object)obj);
			obj = default(T);
		}
	}

	private void SafeDispose<T>(ref T obj) where T : IDisposable
	{
		if (obj != null)
		{
			obj.Dispose();
			obj = default(T);
		}
	}

	public ReflectionProbeEx()
		: this()
	{
	}//IL_0002: Unknown result type (might be due to invalid IL or missing references)
	//IL_001b: Unknown result type (might be due to invalid IL or missing references)
	//IL_0030: Unknown result type (might be due to invalid IL or missing references)
	//IL_0035: Unknown result type (might be due to invalid IL or missing references)

}
