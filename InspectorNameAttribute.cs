using UnityEngine;

public class InspectorNameAttribute : PropertyAttribute
{
	public string name;

	public InspectorNameAttribute(string name)
		: this()
	{
		this.name = name;
	}
}
