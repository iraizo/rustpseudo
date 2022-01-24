using UnityEngine;

public class ParticleRandomLifetime : MonoBehaviour
{
	public ParticleSystem mySystem;

	public float minScale = 0.5f;

	public float maxScale = 1f;

	public void Awake()
	{
		if (Object.op_Implicit((Object)(object)mySystem))
		{
			float startLifetime = Random.Range(minScale, maxScale);
			mySystem.set_startLifetime(startLifetime);
		}
	}

	public ParticleRandomLifetime()
		: this()
	{
	}
}
