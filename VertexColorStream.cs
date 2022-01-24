using UnityEngine;

[ExecuteInEditMode]
public class VertexColorStream : MonoBehaviour
{
	[HideInInspector]
	public Mesh originalMesh;

	[HideInInspector]
	public Mesh paintedMesh;

	[HideInInspector]
	public MeshHolder meshHold;

	[HideInInspector]
	public Vector3[] _vertices;

	[HideInInspector]
	public Vector3[] _normals;

	[HideInInspector]
	public int[] _triangles;

	[HideInInspector]
	public int[][] _Subtriangles;

	[HideInInspector]
	public Matrix4x4[] _bindPoses;

	[HideInInspector]
	public BoneWeight[] _boneWeights;

	[HideInInspector]
	public Bounds _bounds;

	[HideInInspector]
	public int _subMeshCount;

	[HideInInspector]
	public Vector4[] _tangents;

	[HideInInspector]
	public Vector2[] _uv;

	[HideInInspector]
	public Vector2[] _uv2;

	[HideInInspector]
	public Vector2[] _uv3;

	[HideInInspector]
	public Color[] _colors;

	[HideInInspector]
	public Vector2[] _uv4;

	private void OnDidApplyAnimationProperties()
	{
	}

	public void init(Mesh origMesh, bool destroyOld)
	{
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		originalMesh = origMesh;
		paintedMesh = Object.Instantiate<Mesh>(origMesh);
		if (destroyOld)
		{
			Object.DestroyImmediate((Object)(object)origMesh);
		}
		((Object)paintedMesh).set_hideFlags((HideFlags)0);
		((Object)paintedMesh).set_name("vpp_" + ((Object)((Component)this).get_gameObject()).get_name());
		meshHold = new MeshHolder();
		meshHold._vertices = paintedMesh.get_vertices();
		meshHold._normals = paintedMesh.get_normals();
		meshHold._triangles = paintedMesh.get_triangles();
		meshHold._TrianglesOfSubs = new trisPerSubmesh[paintedMesh.get_subMeshCount()];
		for (int i = 0; i < paintedMesh.get_subMeshCount(); i++)
		{
			meshHold._TrianglesOfSubs[i] = new trisPerSubmesh();
			meshHold._TrianglesOfSubs[i].triangles = paintedMesh.GetTriangles(i);
		}
		meshHold._bindPoses = paintedMesh.get_bindposes();
		meshHold._boneWeights = paintedMesh.get_boneWeights();
		meshHold._bounds = paintedMesh.get_bounds();
		meshHold._subMeshCount = paintedMesh.get_subMeshCount();
		meshHold._tangents = paintedMesh.get_tangents();
		meshHold._uv = paintedMesh.get_uv();
		meshHold._uv2 = paintedMesh.get_uv2();
		meshHold._uv3 = paintedMesh.get_uv3();
		meshHold._colors = paintedMesh.get_colors();
		meshHold._uv4 = paintedMesh.get_uv4();
		((Component)this).GetComponent<MeshFilter>().set_sharedMesh(paintedMesh);
		if (Object.op_Implicit((Object)(object)((Component)this).GetComponent<MeshCollider>()))
		{
			((Component)this).GetComponent<MeshCollider>().set_sharedMesh(paintedMesh);
		}
	}

	public void setWholeMesh(Mesh tmpMesh)
	{
		paintedMesh.set_vertices(tmpMesh.get_vertices());
		paintedMesh.set_triangles(tmpMesh.get_triangles());
		paintedMesh.set_normals(tmpMesh.get_normals());
		paintedMesh.set_colors(tmpMesh.get_colors());
		paintedMesh.set_uv(tmpMesh.get_uv());
		paintedMesh.set_uv2(tmpMesh.get_uv2());
		paintedMesh.set_uv3(tmpMesh.get_uv3());
		meshHold._vertices = tmpMesh.get_vertices();
		meshHold._triangles = tmpMesh.get_triangles();
		meshHold._normals = tmpMesh.get_normals();
		meshHold._colors = tmpMesh.get_colors();
		meshHold._uv = tmpMesh.get_uv();
		meshHold._uv2 = tmpMesh.get_uv2();
		meshHold._uv3 = tmpMesh.get_uv3();
	}

	public Vector3[] setVertices(Vector3[] _deformedVertices)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		paintedMesh.set_vertices(_deformedVertices);
		meshHold._vertices = _deformedVertices;
		paintedMesh.RecalculateNormals();
		paintedMesh.RecalculateBounds();
		meshHold._normals = paintedMesh.get_normals();
		meshHold._bounds = paintedMesh.get_bounds();
		((Component)this).GetComponent<MeshCollider>().set_sharedMesh((Mesh)null);
		if (Object.op_Implicit((Object)(object)((Component)this).GetComponent<MeshCollider>()))
		{
			((Component)this).GetComponent<MeshCollider>().set_sharedMesh(paintedMesh);
		}
		return meshHold._normals;
	}

	public Vector3[] getVertices()
	{
		return paintedMesh.get_vertices();
	}

	public Vector3[] getNormals()
	{
		return paintedMesh.get_normals();
	}

	public int[] getTriangles()
	{
		return paintedMesh.get_triangles();
	}

	public void setTangents(Vector4[] _meshTangents)
	{
		paintedMesh.set_tangents(_meshTangents);
		meshHold._tangents = _meshTangents;
	}

	public Vector4[] getTangents()
	{
		return paintedMesh.get_tangents();
	}

	public void setColors(Color[] _vertexColors)
	{
		paintedMesh.set_colors(_vertexColors);
		meshHold._colors = _vertexColors;
	}

	public Color[] getColors()
	{
		return paintedMesh.get_colors();
	}

	public Vector2[] getUVs()
	{
		return paintedMesh.get_uv();
	}

	public void setUV4s(Vector2[] _uv4s)
	{
		paintedMesh.set_uv4(_uv4s);
		meshHold._uv4 = _uv4s;
	}

	public Vector2[] getUV4s()
	{
		return paintedMesh.get_uv4();
	}

	public void unlink()
	{
		init(paintedMesh, destroyOld: false);
	}

	public void rebuild()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)((Component)this).GetComponent<MeshFilter>()))
		{
			return;
		}
		paintedMesh = new Mesh();
		((Object)paintedMesh).set_hideFlags((HideFlags)61);
		((Object)paintedMesh).set_name("vpp_" + ((Object)((Component)this).get_gameObject()).get_name());
		if (meshHold == null || meshHold._vertices.Length == 0 || meshHold._TrianglesOfSubs.Length == 0)
		{
			paintedMesh.set_subMeshCount(_subMeshCount);
			paintedMesh.set_vertices(_vertices);
			paintedMesh.set_normals(_normals);
			paintedMesh.set_triangles(_triangles);
			meshHold._TrianglesOfSubs = new trisPerSubmesh[paintedMesh.get_subMeshCount()];
			for (int i = 0; i < paintedMesh.get_subMeshCount(); i++)
			{
				meshHold._TrianglesOfSubs[i] = new trisPerSubmesh();
				meshHold._TrianglesOfSubs[i].triangles = paintedMesh.GetTriangles(i);
			}
			paintedMesh.set_bindposes(_bindPoses);
			paintedMesh.set_boneWeights(_boneWeights);
			paintedMesh.set_bounds(_bounds);
			paintedMesh.set_tangents(_tangents);
			paintedMesh.set_uv(_uv);
			paintedMesh.set_uv2(_uv2);
			paintedMesh.set_uv3(_uv3);
			paintedMesh.set_colors(_colors);
			paintedMesh.set_uv4(_uv4);
			init(paintedMesh, destroyOld: true);
		}
		else
		{
			paintedMesh.set_subMeshCount(meshHold._subMeshCount);
			paintedMesh.set_vertices(meshHold._vertices);
			paintedMesh.set_normals(meshHold._normals);
			for (int j = 0; j < meshHold._subMeshCount; j++)
			{
				paintedMesh.SetTriangles(meshHold._TrianglesOfSubs[j].triangles, j);
			}
			paintedMesh.set_bindposes(meshHold._bindPoses);
			paintedMesh.set_boneWeights(meshHold._boneWeights);
			paintedMesh.set_bounds(meshHold._bounds);
			paintedMesh.set_tangents(meshHold._tangents);
			paintedMesh.set_uv(meshHold._uv);
			paintedMesh.set_uv2(meshHold._uv2);
			paintedMesh.set_uv3(meshHold._uv3);
			paintedMesh.set_colors(meshHold._colors);
			paintedMesh.set_uv4(meshHold._uv4);
			init(paintedMesh, destroyOld: true);
		}
	}

	private void Start()
	{
		if (!Object.op_Implicit((Object)(object)paintedMesh) || meshHold == null)
		{
			rebuild();
		}
	}

	public VertexColorStream()
		: this()
	{
	}
}
