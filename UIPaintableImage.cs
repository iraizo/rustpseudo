using UnityEngine;
using UnityEngine.UI;

public class UIPaintableImage : MonoBehaviour
{
	public enum DrawMode
	{
		AlphaBlended,
		Additive,
		Lighten,
		Erase
	}

	public RawImage image;

	public int texSize = 64;

	public Color clearColor = Color.get_clear();

	public FilterMode filterMode = (FilterMode)1;

	public bool mipmaps;

	public RectTransform rectTransform
	{
		get
		{
			Transform transform = ((Component)this).get_transform();
			return (RectTransform)(object)((transform is RectTransform) ? transform : null);
		}
	}

	public UIPaintableImage()
		: this()
	{
	}//IL_0009: Unknown result type (might be due to invalid IL or missing references)
	//IL_000e: Unknown result type (might be due to invalid IL or missing references)
	//IL_0015: Unknown result type (might be due to invalid IL or missing references)

}
