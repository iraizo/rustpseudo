using Facepunch;
using UnityEngine;
using UnityEngine.UI;

public class MonumentMarker : MonoBehaviour
{
	public Text text;

	public Image imageBackground;

	public Image image;

	public Color dayColor;

	public Color nightColor;

	public void Setup(LandmarkInfo info)
	{
		text.set_text(info.displayPhrase.IsValid() ? info.displayPhrase.get_translated() : ((Object)((Component)info).get_transform().get_root()).get_name());
		if ((Object)(object)info.mapIcon != (Object)null)
		{
			image.set_sprite(info.mapIcon);
			ComponentExtensions.SetActive<Text>(text, false);
			ComponentExtensions.SetActive<Image>(imageBackground, true);
		}
		else
		{
			ComponentExtensions.SetActive<Text>(text, true);
			ComponentExtensions.SetActive<Image>(imageBackground, false);
		}
		SetNightMode(nightMode: false);
	}

	public void SetNightMode(bool nightMode)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		Color color = (nightMode ? nightColor : dayColor);
		Color color2 = (nightMode ? dayColor : nightColor);
		if ((Object)(object)text != (Object)null)
		{
			((Graphic)text).set_color(color);
		}
		if ((Object)(object)image != (Object)null)
		{
			((Graphic)image).set_color(color);
		}
		if ((Object)(object)imageBackground != (Object)null)
		{
			((Graphic)imageBackground).set_color(color2);
		}
	}

	public MonumentMarker()
		: this()
	{
	}
}
