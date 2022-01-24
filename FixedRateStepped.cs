using UnityEngine;

public class FixedRateStepped
{
	public float rate = 0.1f;

	public int maxSteps = 3;

	internal float nextCall;

	public bool ShouldStep()
	{
		if (nextCall > Time.get_time())
		{
			return false;
		}
		if (nextCall == 0f)
		{
			nextCall = Time.get_time();
		}
		if (nextCall + rate * (float)maxSteps < Time.get_time())
		{
			nextCall = Time.get_time() - rate * (float)maxSteps;
		}
		nextCall += rate;
		return true;
	}
}
