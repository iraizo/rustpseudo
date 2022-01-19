using UnityEngine;

public class ParticleDisableOnParentDestroy : MonoBehaviour, IOnParentDestroying
{
	public float destroyAfterSeconds;

	public void OnParentDestroying()
	{
		((Component)this).get_transform().set_parent((Transform)null);
		((Component)this).GetComponent<ParticleSystem>().set_enableEmission(false);
		if (destroyAfterSeconds > 0f)
		{
			GameManager.Destroy(((Component)this).get_gameObject(), destroyAfterSeconds);
		}
	}

	public ParticleDisableOnParentDestroy()
		: this()
	{
	}
}
