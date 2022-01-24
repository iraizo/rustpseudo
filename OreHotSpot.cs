using UnityEngine;

public class OreHotSpot : BaseCombatEntity, ILOD
{
	public float visualDistance = 20f;

	public GameObjectRef visualEffect;

	public GameObjectRef finishEffect;

	public GameObjectRef damageEffect;

	public OreResourceEntity owner;

	public void OreOwner(OreResourceEntity newOwner)
	{
		owner = newOwner;
	}

	public override void ServerInit()
	{
		base.ServerInit();
	}

	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		if (!base.isClient && Object.op_Implicit((Object)(object)owner))
		{
			owner.OnAttacked(info);
		}
	}

	public override void OnKilled(HitInfo info)
	{
		FireFinishEffect();
		base.OnKilled(info);
	}

	public void FireFinishEffect()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (finishEffect.isValid)
		{
			Effect.server.Run(finishEffect.resourcePath, ((Component)this).get_transform().get_position(), ((Component)this).get_transform().get_forward());
		}
	}
}
