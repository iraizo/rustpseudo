using ConVar;
using UnityEngine;
using UnityEngine.UI;

public class UIScale : MonoBehaviour
{
	public CanvasScaler scaler;

	private void Update()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(1280f / Graphics.uiscale, 720f / Graphics.uiscale);
		if (scaler.get_referenceResolution() != val)
		{
			scaler.set_referenceResolution(val);
		}
	}

	public UIScale()
		: this()
	{
	}
}
