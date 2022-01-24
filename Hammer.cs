using System;
using UnityEngine;

public class Hammer : BaseMelee
{
	public override bool CanHit(HitTest info)
	{
		if ((Object)(object)info.HitEntity == (Object)null)
		{
			return false;
		}
		if (info.HitEntity is BasePlayer)
		{
			return false;
		}
		return info.HitEntity is BaseCombatEntity;
	}

	public override void DoAttackShared(HitInfo info)
	{
		BasePlayer ownerPlayer = GetOwnerPlayer();
		BaseCombatEntity baseCombatEntity = info.HitEntity as BaseCombatEntity;
		if ((Object)(object)baseCombatEntity != (Object)null && (Object)(object)ownerPlayer != (Object)null && base.isServer)
		{
			TimeWarning val = TimeWarning.New("DoRepair", 50);
			try
			{
				baseCombatEntity.DoRepair(ownerPlayer);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		info.DoDecals = false;
		if (base.isServer)
		{
			Effect.server.ImpactEffect(info);
		}
		else
		{
			Effect.client.ImpactEffect(info);
		}
	}
}
