using UnityEngine;

public class DrawSkeleton : MonoBehaviour
{
	private void OnDrawGizmos()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.set_color(Color.get_white());
		DrawTransform(((Component)this).get_transform());
	}

	private static void DrawTransform(Transform t)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < t.get_childCount(); i++)
		{
			Gizmos.DrawLine(t.get_position(), t.GetChild(i).get_position());
			DrawTransform(t.GetChild(i));
		}
	}

	public DrawSkeleton()
		: this()
	{
	}
}
