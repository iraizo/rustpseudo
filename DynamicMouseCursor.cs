using System.Collections.Generic;
using Rust.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class DynamicMouseCursor : MonoBehaviour
{
	public Texture2D RegularCursor;

	public Vector2 RegularCursorPos;

	public Texture2D HoverCursor;

	public Vector2 HoverCursorPos;

	private Texture2D current;

	private PointerEventData pointer;

	private List<RaycastResult> results = new List<RaycastResult>();

	private void LateUpdate()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		if (!Cursor.get_visible())
		{
			return;
		}
		GameObject val = CurrentlyHoveredItem();
		if ((Object)(object)val != (Object)null)
		{
			RustControl componentInParent = val.GetComponentInParent<RustControl>();
			if ((Object)(object)componentInParent != (Object)null && componentInParent.get_IsDisabled())
			{
				UpdateCursor(RegularCursor, RegularCursorPos);
				return;
			}
			if (val.GetComponentInParent<ISubmitHandler>() != null)
			{
				UpdateCursor(HoverCursor, HoverCursorPos);
				return;
			}
			if (val.GetComponentInParent<IPointerDownHandler>() != null)
			{
				UpdateCursor(HoverCursor, HoverCursorPos);
				return;
			}
		}
		UpdateCursor(RegularCursor, RegularCursorPos);
	}

	private void UpdateCursor(Texture2D cursor, Vector2 offs)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)current == (Object)(object)cursor))
		{
			current = cursor;
			Cursor.SetCursor(cursor, offs, (CursorMode)0);
		}
	}

	private GameObject CurrentlyHoveredItem()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		if (pointer == null)
		{
			pointer = new PointerEventData(EventSystem.get_current());
		}
		pointer.set_position(Vector2.op_Implicit(Input.get_mousePosition()));
		EventSystem.get_current().RaycastAll(pointer, results);
		using (List<RaycastResult>.Enumerator enumerator = results.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				RaycastResult val = enumerator.Current;
				return ((RaycastResult)(ref val)).get_gameObject();
			}
		}
		return null;
	}

	public DynamicMouseCursor()
		: this()
	{
	}
}
