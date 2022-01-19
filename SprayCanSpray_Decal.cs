using UnityEngine;

public class SprayCanSpray_Decal : SprayCanSpray, ICustomMaterialReplacer, IPropRenderNotify
{
	public DeferredDecal DecalComponent;

	public GameObject IconPreviewRoot;
}
