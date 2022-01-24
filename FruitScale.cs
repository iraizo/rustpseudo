using UnityEngine;

public class FruitScale : MonoBehaviour, IClientComponent
{
	public void SetProgress(float progress)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).get_transform().set_localScale(Vector3.get_one() * progress);
	}

	public FruitScale()
		: this()
	{
	}
}
