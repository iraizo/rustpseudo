using UnityEngine;

[ExecuteInEditMode]
public class DeferredDecal : MonoBehaviour
{
	public Mesh mesh;

	public Material material;

	public DeferredDecalQueue queue;

	public DeferredDecal()
		: this()
	{
	}
}
