using System.Collections.Generic;
using UnityEngine;

public class CH47LandingZone : MonoBehaviour
{
	public float lastDropTime;

	private static List<CH47LandingZone> landingZones = new List<CH47LandingZone>();

	public float dropoffScale = 1f;

	public void Awake()
	{
		if (!landingZones.Contains(this))
		{
			landingZones.Add(this);
		}
	}

	public static CH47LandingZone GetClosest(Vector3 pos)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		float num = float.PositiveInfinity;
		CH47LandingZone result = null;
		foreach (CH47LandingZone landingZone in landingZones)
		{
			float num2 = Vector3Ex.Distance2D(pos, ((Component)landingZone).get_transform().get_position());
			if (num2 < num)
			{
				num = num2;
				result = landingZone;
			}
		}
		return result;
	}

	public void OnDestroy()
	{
		if (landingZones.Contains(this))
		{
			landingZones.Remove(this);
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
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		Color magenta = Color.get_magenta();
		magenta.a = 0.25f;
		Gizmos.set_color(magenta);
		GizmosUtil.DrawCircleY(((Component)this).get_transform().get_position(), 6f);
		magenta.a = 1f;
		Gizmos.set_color(magenta);
		GizmosUtil.DrawWireCircleY(((Component)this).get_transform().get_position(), 6f);
	}

	public CH47LandingZone()
		: this()
	{
	}
}
