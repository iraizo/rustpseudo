using UnityEngine;

public class SoccerBall : BaseCombatEntity
{
	[Header("Soccer Ball")]
	[SerializeField]
	private Rigidbody rigidBody;

	[SerializeField]
	private float additionalForceMultiplier = 0.2f;

	[SerializeField]
	private float upForceMultiplier = 0.15f;

	[SerializeField]
	private DamageRenderer damageRenderer;

	[SerializeField]
	private float explosionForceMultiplier = 40f;

	[SerializeField]
	private float otherForceMultiplier = 10f;

	protected void OnCollisionEnter(Collision collision)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		if (!base.isClient)
		{
			Vector3 impulse = collision.get_impulse();
			if (((Vector3)(ref impulse)).get_magnitude() > 0f && (Object)(object)collision.get_collider().get_attachedRigidbody() != (Object)null && !((Component)(object)collision.get_collider().get_attachedRigidbody()).HasComponent<SoccerBall>())
			{
				Vector3 val = rigidBody.get_position() - collision.get_collider().get_attachedRigidbody().get_position();
				impulse = collision.get_impulse();
				float magnitude = ((Vector3)(ref impulse)).get_magnitude();
				rigidBody.AddForce(val * magnitude * additionalForceMultiplier + Vector3.get_up() * magnitude * upForceMultiplier, (ForceMode)1);
			}
		}
	}

	public override void Hurt(HitInfo info)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if (!base.isClient)
		{
			float num = 0f;
			float[] types = info.damageTypes.types;
			foreach (float num2 in types)
			{
				num = (((int)num2 != 16 && (int)num2 != 22) ? (num + num2 * otherForceMultiplier) : (num + num2 * explosionForceMultiplier));
			}
			if (num > 3f)
			{
				rigidBody.AddExplosionForce(num, info.HitPositionWorld, 0.25f, 0.5f);
			}
			base.Hurt(info);
		}
	}
}
