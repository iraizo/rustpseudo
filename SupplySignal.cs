using System;
using UnityEngine;

public class SupplySignal : TimedExplosive
{
	public GameObjectRef smokeEffectPrefab;

	public GameObjectRef EntityToCreate;

	[NonSerialized]
	public GameObject smokeEffect;

	public override void Explode()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		BaseEntity baseEntity = GameManager.server.CreateEntity(EntityToCreate.resourcePath);
		if (Object.op_Implicit((Object)(object)baseEntity))
		{
			Vector3 val = default(Vector3);
			((Vector3)(ref val))._002Ector(Random.Range(-20f, 20f), 0f, Random.Range(-20f, 20f));
			((Component)baseEntity).SendMessage("InitDropPosition", (object)(((Component)this).get_transform().get_position() + val), (SendMessageOptions)1);
			baseEntity.Spawn();
		}
		((FacepunchBehaviour)this).Invoke((Action)FinishUp, 210f);
		SetFlag(Flags.On, b: true);
		SendNetworkUpdateImmediate();
	}

	public void FinishUp()
	{
		Kill();
	}
}
