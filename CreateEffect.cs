using UnityEngine;

public class CreateEffect : MonoBehaviour
{
	public GameObjectRef EffectToCreate;

	public void OnEnable()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		Effect.client.Run(EffectToCreate.resourcePath, ((Component)this).get_transform().get_position(), ((Component)this).get_transform().get_up(), ((Component)this).get_transform().get_forward());
	}

	public CreateEffect()
		: this()
	{
	}
}
