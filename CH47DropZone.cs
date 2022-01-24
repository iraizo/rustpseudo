using System.Collections.Generic;
using UnityEngine;

public class CH47DropZone : MonoBehaviour
{
	public float lastDropTime;

	private static List<CH47DropZone> dropZones = new List<CH47DropZone>();

	public void Awake()
	{
		if (!dropZones.Contains(this))
		{
			dropZones.Add(this);
		}
	}

	public static CH47DropZone GetClosest(Vector3 pos)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		float num = float.PositiveInfinity;
		CH47DropZone result = null;
		foreach (CH47DropZone dropZone in dropZones)
		{
			float num2 = Vector3Ex.Distance2D(pos, ((Component)dropZone).get_transform().get_position());
			if (num2 < num)
			{
				num = num2;
				result = dropZone;
			}
		}
		return result;
	}

	public void OnDestroy()
	{
		if (dropZones.Contains(this))
		{
			dropZones.Remove(this);
		}
	}

	public float TimeSinceLastDrop()
	{
		return Time.get_time() - lastDropTime;
	}

	public void Used()
	{
		lastDropTime = Time.get_time();
	}

	public void OnDrawGizmos()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.set_color(Color.get_yellow());
		Gizmos.DrawSphere(((Component)this).get_transform().get_position(), 5f);
	}

	public CH47DropZone()
		: this()
	{
	}
}
