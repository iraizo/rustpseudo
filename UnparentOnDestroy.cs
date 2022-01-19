using UnityEngine;

public class UnparentOnDestroy : MonoBehaviour, IOnParentDestroying
{
	public float destroyAfterSeconds = 1f;

	public void OnParentDestroying()
	{
		((Component)this).get_transform().set_parent((Transform)null);
		GameManager.Destroy(((Component)this).get_gameObject(), (destroyAfterSeconds <= 0f) ? 1f : destroyAfterSeconds);
	}

	protected void OnValidate()
	{
		if (destroyAfterSeconds <= 0f)
		{
			destroyAfterSeconds = 1f;
		}
	}

	public UnparentOnDestroy()
		: this()
	{
	}
}
