using UnityEngine;

public class HolosightReticlePositioning : MonoBehaviour
{
	public IronsightAimPoint aimPoint;

	public RectTransform rectTransform
	{
		get
		{
			Transform transform = ((Component)this).get_transform();
			return (RectTransform)(object)((transform is RectTransform) ? transform : null);
		}
	}

	private void Update()
	{
		if (MainCamera.isValid)
		{
			UpdatePosition(MainCamera.mainCamera);
		}
	}

	private void UpdatePosition(Camera cam)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Component)aimPoint.targetPoint).get_transform().get_position();
		Vector2 val = RectTransformUtility.WorldToScreenPoint(cam, position);
		Transform parent = ((Transform)rectTransform).get_parent();
		RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)(object)((parent is RectTransform) ? parent : null), val, cam, ref val);
		ref float x = ref val.x;
		float num = x;
		Transform parent2 = ((Transform)rectTransform).get_parent();
		Rect rect = ((RectTransform)((parent2 is RectTransform) ? parent2 : null)).get_rect();
		x = num / (((Rect)(ref rect)).get_width() * 0.5f);
		ref float y = ref val.y;
		float num2 = y;
		Transform parent3 = ((Transform)rectTransform).get_parent();
		rect = ((RectTransform)((parent3 is RectTransform) ? parent3 : null)).get_rect();
		y = num2 / (((Rect)(ref rect)).get_height() * 0.5f);
		rectTransform.set_anchoredPosition(val);
	}

	public HolosightReticlePositioning()
		: this()
	{
	}
}
