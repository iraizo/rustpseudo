using UnityEngine;

public class TerrainCollisionProxy : MonoBehaviour, IServerComponent
{
	public WheelCollider[] colliders;

	public TerrainCollisionProxy()
		: this()
	{
	}
}
