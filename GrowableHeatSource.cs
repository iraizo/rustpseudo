using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;

public class GrowableHeatSource : EntityComponent<BaseEntity>, IServerComponent
{
	public float heatAmount = 5f;

	public float ApplyHeat(Vector3 forPosition)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)base.baseEntity == (Object)null)
		{
			return 0f;
		}
		IOEntity iOEntity;
		if (base.baseEntity.IsOn() || ((iOEntity = base.baseEntity as IOEntity) != null && iOEntity.IsPowered()))
		{
			return Mathx.RemapValClamped(Vector3.Distance(forPosition, ((Component)this).get_transform().get_position()), 0f, Server.artificialTemperatureGrowableRange, 0f, heatAmount);
		}
		return 0f;
	}

	public void ForceUpdateGrowablesInRange()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		List<GrowableEntity> list = Pool.GetList<GrowableEntity>();
		Vis.Entities(((Component)this).get_transform().get_position(), Server.artificialTemperatureGrowableRange, list, 512, (QueryTriggerInteraction)2);
		List<PlanterBox> list2 = Pool.GetList<PlanterBox>();
		foreach (GrowableEntity item in list)
		{
			if (item.isServer)
			{
				PlanterBox planter = item.GetPlanter();
				if ((Object)(object)planter != (Object)null && !list2.Contains(planter))
				{
					list2.Add(planter);
					planter.ForceTemperatureUpdate();
				}
				item.CalculateQualities(firstTime: false, forceArtificialLightUpdates: false, forceArtificialTemperatureUpdates: true);
				item.SendNetworkUpdate();
			}
		}
		Pool.FreeList<PlanterBox>(ref list2);
		Pool.FreeList<GrowableEntity>(ref list);
	}
}
