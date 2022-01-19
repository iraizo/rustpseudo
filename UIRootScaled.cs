using ConVar;
using UnityEngine;
using UnityEngine.UI;

public class UIRootScaled : UIRoot
{
	private static UIRootScaled Instance;

	public bool OverrideReference;

	public Vector2 TargetReference = new Vector2(1280f, 720f);

	public CanvasScaler scaler;

	public static Canvas DragOverlayCanvas => Instance.overlayCanvas;

	protected override void Awake()
	{
		Instance = this;
		base.Awake();
	}

	protected override void Refresh()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(1280f / Graphics.uiscale, 720f / Graphics.uiscale);
		if (OverrideReference)
		{
			((Vector2)(ref val))._002Ector(TargetReference.x / Graphics.uiscale, TargetReference.y / Graphics.uiscale);
		}
		if (scaler.get_referenceResolution() != val)
		{
			scaler.set_referenceResolution(val);
		}
	}
}
