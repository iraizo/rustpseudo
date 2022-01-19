using UnityEngine;

public class FlameExplosive : TimedExplosive
{
	public GameObjectRef createOnExplode;

	public float numToCreate = 10f;

	public float minVelocity = 2f;

	public float maxVelocity = 5f;

	public float spreadAngle = 90f;

	public override void Explode()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		FlameExplode(-((Component)this).get_transform().get_forward());
	}

	public void FlameExplode(Vector3 surfaceNormal)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		if (!base.isServer)
		{
			return;
		}
		for (int i = 0; (float)i < numToCreate; i++)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(createOnExplode.resourcePath, ((Component)this).get_transform().get_position());
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(spreadAngle, surfaceNormal);
				((Component)baseEntity).get_transform().SetPositionAndRotation(((Component)this).get_transform().get_position(), Quaternion.LookRotation(modifiedAimConeDirection));
				baseEntity.creatorEntity = (((Object)(object)creatorEntity == (Object)null) ? baseEntity : creatorEntity);
				baseEntity.Spawn();
				baseEntity.SetVelocity(modifiedAimConeDirection * Random.Range(minVelocity, maxVelocity));
			}
		}
		base.Explode();
	}

	public override void ProjectileImpact(RaycastHit info, Vector3 rayOrigin)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		FlameExplode(((RaycastHit)(ref info)).get_normal());
	}
}
