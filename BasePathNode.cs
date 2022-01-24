using System.Collections.Generic;
using UnityEngine;

public class BasePathNode : MonoBehaviour
{
	public BasePath Path;

	public List<BasePathNode> linked;

	public float maxVelocityOnApproach = -1f;

	public bool straightaway;

	public void OnDrawGizmosSelected()
	{
	}

	public BasePathNode()
		: this()
	{
	}
}
