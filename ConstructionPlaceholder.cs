using System;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class ConstructionPlaceholder : PrefabAttribute, IPrefabPreProcess
{
	public Mesh mesh;

	public Material material;

	public bool renderer;

	public bool collider;

	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		if (!clientside)
		{
			return;
		}
		if (renderer)
		{
			MeshFilter component = rootObj.GetComponent<MeshFilter>();
			MeshRenderer component2 = rootObj.GetComponent<MeshRenderer>();
			if (!Object.op_Implicit((Object)(object)component))
			{
				rootObj.AddComponent<MeshFilter>().set_sharedMesh(mesh);
			}
			if (!Object.op_Implicit((Object)(object)component2))
			{
				component2 = rootObj.AddComponent<MeshRenderer>();
				((Renderer)component2).set_sharedMaterial(material);
				((Renderer)component2).set_shadowCastingMode((ShadowCastingMode)0);
			}
		}
		if (collider && !Object.op_Implicit((Object)(object)rootObj.GetComponent<MeshCollider>()))
		{
			rootObj.AddComponent<MeshCollider>().set_sharedMesh(mesh);
		}
	}

	protected override Type GetIndexedType()
	{
		return typeof(ConstructionPlaceholder);
	}
}
