using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public class SurveyCharge : TimedExplosive
{
	public GameObjectRef craterPrefab;

	public GameObjectRef craterPrefab_Oil;

	public override void Explode()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		base.Explode();
		if (WaterLevel.Test(((Component)this).get_transform().get_position(), waves: true, this))
		{
			return;
		}
		ResourceDepositManager.ResourceDeposit orCreate = ResourceDepositManager.GetOrCreate(((Component)this).get_transform().get_position());
		if (orCreate == null || Time.get_realtimeSinceStartup() - orCreate.lastSurveyTime < 10f)
		{
			return;
		}
		orCreate.lastSurveyTime = Time.get_realtimeSinceStartup();
		if (!TransformUtil.GetGroundInfo(((Component)this).get_transform().get_position(), out var hitOut, 0.3f, LayerMask.op_Implicit(8388608)))
		{
			return;
		}
		Vector3 point = ((RaycastHit)(ref hitOut)).get_point();
		((RaycastHit)(ref hitOut)).get_normal();
		List<SurveyCrater> list = Pool.GetList<SurveyCrater>();
		Vis.Entities(((Component)this).get_transform().get_position(), 10f, list, 1, (QueryTriggerInteraction)2);
		bool num = list.Count > 0;
		Pool.FreeList<SurveyCrater>(ref list);
		if (num)
		{
			return;
		}
		bool flag = false;
		bool flag2 = false;
		foreach (ResourceDepositManager.ResourceDeposit.ResourceDepositEntry resource in orCreate._resources)
		{
			if (resource.spawnType == ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM && !resource.isLiquid && resource.amount >= 1000)
			{
				int num2 = Mathf.Clamp(Mathf.CeilToInt(2.5f / resource.workNeeded * 10f), 0, 5);
				int iAmount = 1;
				flag = true;
				if (resource.isLiquid)
				{
					flag2 = true;
				}
				for (int i = 0; i < num2; i++)
				{
					Item item = ItemManager.Create(resource.type, iAmount, 0uL);
					Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(20f, Vector3.get_up());
					BaseEntity baseEntity = item.Drop(((Component)this).get_transform().get_position() + Vector3.get_up() * 1f, GetInheritedDropVelocity() + modifiedAimConeDirection * Random.Range(5f, 10f), Random.get_rotation());
					Quaternion rotation = Random.get_rotation();
					baseEntity.SetAngularVelocity(((Quaternion)(ref rotation)).get_eulerAngles() * 5f);
				}
			}
		}
		if (flag)
		{
			string strPrefab = (flag2 ? craterPrefab_Oil.resourcePath : craterPrefab.resourcePath);
			BaseEntity baseEntity2 = GameManager.server.CreateEntity(strPrefab, point, Quaternion.get_identity());
			if (Object.op_Implicit((Object)(object)baseEntity2))
			{
				baseEntity2.Spawn();
			}
		}
	}
}
