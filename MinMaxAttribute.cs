using UnityEngine;

public class MinMaxAttribute : PropertyAttribute
{
	public float min;

	public float max;

	public MinMaxAttribute(float min, float max)
		: this()
	{
		this.min = min;
		this.max = max;
	}
}
