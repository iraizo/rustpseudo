using UnityEngine;

public class ScrapTransportHelicopterWheelEffects : MonoBehaviour, IServerComponent
{
	public WheelCollider wheelCollider;

	public GameObjectRef impactEffect;

	public float minTimeBetweenEffects = 0.25f;

	public float minDistBetweenEffects = 0.1f;

	private bool wasGrounded;

	private float lastEffectPlayed;

	private Vector3 lastCollisionPos;

	public void Update()
	{
		bool isGrounded = wheelCollider.get_isGrounded();
		if (isGrounded && !wasGrounded)
		{
			DoImpactEffect();
		}
		wasGrounded = isGrounded;
	}

	private void DoImpactEffect()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		if (impactEffect.isValid && !(Time.get_time() < lastEffectPlayed + minTimeBetweenEffects) && (!(Vector3.Distance(((Component)this).get_transform().get_position(), lastCollisionPos) < minDistBetweenEffects) || lastEffectPlayed == 0f))
		{
			Effect.server.Run(impactEffect.resourcePath, ((Component)this).get_transform().get_position(), ((Component)this).get_transform().get_up());
			lastEffectPlayed = Time.get_time();
			lastCollisionPos = ((Component)this).get_transform().get_position();
		}
	}

	public ScrapTransportHelicopterWheelEffects()
		: this()
	{
	}
}
