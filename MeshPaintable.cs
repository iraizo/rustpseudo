using UnityEngine;

public class MeshPaintable : MonoBehaviour, IClientComponent
{
	public string replacementTextureName = "_MainTex";

	public int textureWidth = 256;

	public int textureHeight = 256;

	public Color clearColor = Color.get_clear();

	public Texture2D targetTexture;

	public bool hasChanges;

	public MeshPaintable()
		: this()
	{
	}//IL_0022: Unknown result type (might be due to invalid IL or missing references)
	//IL_0027: Unknown result type (might be due to invalid IL or missing references)

}
