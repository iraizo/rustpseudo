using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using Rust;
using UnityEngine;

public class CeilingLight : IOEntity
{
	public float pushScale = 2f;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("CeilingLight.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override int ConsumptionAmount()
	{
		if (IsOn())
		{
			return 2;
		}
		return base.ConsumptionAmount();
	}

	public override void Hurt(HitInfo info)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (base.isServer)
		{
			if (info.damageTypes.Has(DamageType.Explosion))
			{
				ClientRPC<int, Vector3, Vector3>(null, "ClientPhysPush", 0, info.attackNormal * 3f * (info.damageTypes.Total() / 50f), info.HitPositionWorld);
			}
			base.Hurt(info);
		}
	}

	public void RefreshGrowables()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		List<GrowableEntity> list = Pool.GetList<GrowableEntity>();
		Vis.Entities(((Component)this).get_transform().get_position() + new Vector3(0f, 0f - ConVar.Server.ceilingLightHeightOffset, 0f), ConVar.Server.ceilingLightGrowableRange, list, 512, (QueryTriggerInteraction)2);
		List<PlanterBox> list2 = Pool.GetList<PlanterBox>();
		foreach (GrowableEntity item in list)
		{
			if (item.isServer)
			{
				PlanterBox planter = item.GetPlanter();
				if ((Object)(object)planter != (Object)null && !list2.Contains(planter))
				{
					list2.Add(planter);
					planter.ForceLightUpdate();
				}
				item.CalculateQualities(firstTime: false, forceArtificialLightUpdates: true);
				item.SendNetworkUpdate();
			}
		}
		Pool.FreeList<PlanterBox>(ref list2);
		Pool.FreeList<GrowableEntity>(ref list);
	}

	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		bool num = IsOn();
		SetFlag(Flags.On, IsPowered());
		if (num != IsOn())
		{
			if (IsOn())
			{
				LightsOn();
			}
			else
			{
				LightsOff();
			}
		}
	}

	public void LightsOn()
	{
		RefreshGrowables();
	}

	public void LightsOff()
	{
		RefreshGrowables();
	}

	public override void OnKilled(HitInfo info)
	{
		base.OnKilled(info);
		RefreshGrowables();
	}

	public override void OnAttacked(HitInfo info)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		float num = 3f * (info.damageTypes.Total() / 50f);
		ClientRPC<uint, Vector3, Vector3>(null, "ClientPhysPush", ((Object)(object)info.Initiator != (Object)null && info.Initiator is BasePlayer && !info.IsPredicting) ? info.Initiator.net.ID : 0u, info.attackNormal * num, info.HitPositionWorld);
		base.OnAttacked(info);
	}
}
