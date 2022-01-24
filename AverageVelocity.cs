using UnityEngine;

public class AverageVelocity
{
	private Vector3 pos;

	private float time;

	private float lastEntry;

	private float averageSpeed;

	private Vector3 averageVelocity;

	public float Speed => averageSpeed;

	public Vector3 Average => averageVelocity;

	public void Record(Vector3 newPos)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		float num = Time.get_time() - time;
		if (!(num < 0.1f))
		{
			if (((Vector3)(ref pos)).get_sqrMagnitude() > 0f)
			{
				Vector3 val = newPos - pos;
				averageVelocity = val * (1f / num);
				averageSpeed = ((Vector3)(ref averageVelocity)).get_magnitude();
			}
			time = Time.get_time();
			pos = newPos;
		}
	}
}
