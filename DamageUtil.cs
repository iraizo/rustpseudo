using System;
using System.Collections.Generic;
using Facepunch;
using Rust;
using UnityEngine;

public static class DamageUtil
{
	public static void RadiusDamage(BaseEntity attackingPlayer, BaseEntity weaponPrefab, Vector3 pos, float minradius, float radius, List<DamageTypeEntry> damage, int layers, bool useLineOfSight)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("DamageUtil.RadiusDamage", 0);
		try
		{
			List<HitInfo> list = Pool.GetList<HitInfo>();
			List<BaseEntity> list2 = Pool.GetList<BaseEntity>();
			List<BaseEntity> list3 = Pool.GetList<BaseEntity>();
			Vis.Entities(pos, radius, list3, layers, (QueryTriggerInteraction)2);
			for (int i = 0; i < list3.Count; i++)
			{
				BaseEntity baseEntity = list3[i];
				if (!baseEntity.isServer || list2.Contains(baseEntity))
				{
					continue;
				}
				Vector3 val2 = baseEntity.ClosestPoint(pos);
				float num = Mathf.Clamp01((Vector3.Distance(val2, pos) - minradius) / (radius - minradius));
				if (!(num > 1f))
				{
					float amount = 1f - num;
					if (!useLineOfSight || baseEntity.IsVisible(pos))
					{
						HitInfo hitInfo = new HitInfo();
						hitInfo.Initiator = attackingPlayer;
						hitInfo.WeaponPrefab = weaponPrefab;
						hitInfo.damageTypes.Add(damage);
						hitInfo.damageTypes.ScaleAll(amount);
						hitInfo.HitPositionWorld = val2;
						Vector3 val3 = pos - val2;
						hitInfo.HitNormalWorld = ((Vector3)(ref val3)).get_normalized();
						hitInfo.PointStart = pos;
						hitInfo.PointEnd = hitInfo.HitPositionWorld;
						list.Add(hitInfo);
						list2.Add(baseEntity);
					}
				}
			}
			for (int j = 0; j < list2.Count; j++)
			{
				BaseEntity baseEntity2 = list2[j];
				HitInfo info = list[j];
				baseEntity2.OnAttacked(info);
			}
			Pool.FreeList<HitInfo>(ref list);
			Pool.FreeList<BaseEntity>(ref list2);
			Pool.FreeList<BaseEntity>(ref list3);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}
}
