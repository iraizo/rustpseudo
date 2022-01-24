using UnityEngine;

public class MuzzleFlash_Flamelet : MonoBehaviour
{
	public ParticleSystem flameletParticle;

	private void OnEnable()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		ShapeModule shape = flameletParticle.get_shape();
		((ShapeModule)(ref shape)).set_angle((float)Random.Range(6, 13));
		float num = Random.Range(7f, 9f);
		flameletParticle.set_startSpeed(Random.Range(2.5f, num));
		flameletParticle.set_startSize(Random.Range(0.05f, num * 0.015f));
	}

	public MuzzleFlash_Flamelet()
		: this()
	{
	}
}
