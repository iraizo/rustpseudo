using System.Collections.Generic;
using ConVar;
using UnityEngine;
using UnityEngine.Rendering;

public class MeshRendererBatch : MeshBatch
{
	private Vector3 position;

	private Mesh meshBatch;

	private MeshFilter meshFilter;

	private MeshRenderer meshRenderer;

	private MeshRendererData meshData;

	private MeshRendererGroup meshGroup;

	private MeshRendererLookup meshLookup;

	public override int VertexCapacity => Batching.renderer_capacity;

	public override int VertexCutoff => Batching.renderer_vertices;

	protected void Awake()
	{
		meshFilter = ((Component)this).GetComponent<MeshFilter>();
		meshRenderer = ((Component)this).GetComponent<MeshRenderer>();
		meshData = new MeshRendererData();
		meshGroup = new MeshRendererGroup();
		meshLookup = new MeshRendererLookup();
	}

	public void Setup(Vector3 position, Material material, ShadowCastingMode shadows, int layer)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Invalid comparison between Unknown and I4
		Vector3 val;
		((Component)this).get_transform().set_position(val = position);
		this.position = val;
		((Component)this).get_gameObject().set_layer(layer);
		((Renderer)meshRenderer).set_sharedMaterial(material);
		((Renderer)meshRenderer).set_shadowCastingMode(shadows);
		if ((int)shadows == 3)
		{
			((Renderer)meshRenderer).set_receiveShadows(false);
			((Renderer)meshRenderer).set_motionVectors(false);
			((Renderer)meshRenderer).set_lightProbeUsage((LightProbeUsage)0);
			((Renderer)meshRenderer).set_reflectionProbeUsage((ReflectionProbeUsage)0);
		}
		else
		{
			((Renderer)meshRenderer).set_receiveShadows(true);
			((Renderer)meshRenderer).set_motionVectors(true);
			((Renderer)meshRenderer).set_lightProbeUsage((LightProbeUsage)1);
			((Renderer)meshRenderer).set_reflectionProbeUsage((ReflectionProbeUsage)1);
		}
	}

	public void Add(MeshRendererInstance instance)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		ref Vector3 reference = ref instance.position;
		reference -= position;
		meshGroup.data.Add(instance);
		AddVertices(instance.mesh.get_vertexCount());
	}

	protected override void AllocMemory()
	{
		meshGroup.Alloc();
		meshData.Alloc();
	}

	protected override void FreeMemory()
	{
		meshGroup.Free();
		meshData.Free();
	}

	protected override void RefreshMesh()
	{
		meshLookup.dst.Clear();
		meshData.Clear();
		meshData.Combine(meshGroup, meshLookup);
	}

	protected override void ApplyMesh()
	{
		if (!Object.op_Implicit((Object)(object)meshBatch))
		{
			meshBatch = AssetPool.Get<Mesh>();
		}
		meshLookup.Apply();
		meshData.Apply(meshBatch);
		meshBatch.UploadMeshData(false);
	}

	protected override void ToggleMesh(bool state)
	{
		List<MeshRendererLookup.LookupEntry> data = meshLookup.src.data;
		for (int i = 0; i < data.Count; i++)
		{
			Renderer renderer = data[i].renderer;
			if (Object.op_Implicit((Object)(object)renderer))
			{
				renderer.set_enabled(!state);
			}
		}
		if (state)
		{
			if (Object.op_Implicit((Object)(object)meshFilter))
			{
				meshFilter.set_sharedMesh(meshBatch);
			}
			if (Object.op_Implicit((Object)(object)meshRenderer))
			{
				((Renderer)meshRenderer).set_enabled(true);
			}
		}
		else
		{
			if (Object.op_Implicit((Object)(object)meshFilter))
			{
				meshFilter.set_sharedMesh((Mesh)null);
			}
			if (Object.op_Implicit((Object)(object)meshRenderer))
			{
				((Renderer)meshRenderer).set_enabled(false);
			}
		}
	}

	protected override void OnPooled()
	{
		if (Object.op_Implicit((Object)(object)meshFilter))
		{
			meshFilter.set_sharedMesh((Mesh)null);
		}
		if (Object.op_Implicit((Object)(object)meshBatch))
		{
			AssetPool.Free(ref meshBatch);
		}
		meshData.Free();
		meshGroup.Free();
		meshLookup.src.Clear();
		meshLookup.dst.Clear();
	}
}
