using System.Diagnostics;
using UnityEngine;

public class Profile
{
	public Stopwatch watch = new Stopwatch();

	public string category;

	public string name;

	public float warnTime;

	public Profile(string cat, string nam, float WarnTime = 1f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		category = cat;
		name = nam;
		warnTime = WarnTime;
	}

	public void Start()
	{
		watch.Reset();
		watch.Start();
	}

	public void Stop()
	{
		watch.Stop();
		if ((float)watch.get_Elapsed().Seconds > warnTime)
		{
			Debug.Log((object)(category + "." + name + ": Took " + watch.get_Elapsed().Seconds + " seconds"));
		}
	}
}
