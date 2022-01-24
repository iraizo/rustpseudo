using UnityEngine;

public class AITraversalWaitPoint : MonoBehaviour
{
	public float nextFreeTime;

	public bool Occupied()
	{
		return Time.get_time() > nextFreeTime;
	}

	public void Occupy(float dur = 1f)
	{
		nextFreeTime = Time.get_time() + dur;
	}

	public AITraversalWaitPoint()
		: this()
	{
	}
}
