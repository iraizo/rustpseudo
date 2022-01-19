using UnityEngine;

public class ParticleSystemPlayer : MonoBehaviour, IOnParentDestroying
{
	protected void OnEnable()
	{
		((Component)this).GetComponent<ParticleSystem>().set_enableEmission(true);
	}

	public void OnParentDestroying()
	{
		((Component)this).GetComponent<ParticleSystem>().set_enableEmission(false);
	}

	public ParticleSystemPlayer()
		: this()
	{
	}
}
