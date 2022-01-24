using UnityEngine;
using UnityEngine.Rendering;

public class RendererInfo : ComponentInfo<Renderer>
{
	public ShadowCastingMode shadows;

	public Material material;

	public Mesh mesh;

	public MeshFilter meshFilter;

	public override void Reset()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		component.set_shadowCastingMode(shadows);
		if (Object.op_Implicit((Object)(object)material))
		{
			component.set_sharedMaterial(material);
		}
		Renderer obj = component;
		SkinnedMeshRenderer val;
		if ((val = (SkinnedMeshRenderer)(object)((obj is SkinnedMeshRenderer) ? obj : null)) != null)
		{
			val.set_sharedMesh(mesh);
		}
		else if (component is MeshRenderer)
		{
			meshFilter.set_sharedMesh(mesh);
		}
	}

	public override void Setup()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		shadows = component.get_shadowCastingMode();
		material = component.get_sharedMaterial();
		Renderer obj = component;
		SkinnedMeshRenderer val;
		if ((val = (SkinnedMeshRenderer)(object)((obj is SkinnedMeshRenderer) ? obj : null)) != null)
		{
			mesh = val.get_sharedMesh();
		}
		else if (component is MeshRenderer)
		{
			meshFilter = ((Component)this).GetComponent<MeshFilter>();
			mesh = meshFilter.get_sharedMesh();
		}
	}
}
