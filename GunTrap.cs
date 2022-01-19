using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using Rust;
using UnityEngine;

public class GunTrap : StorageContainer
{
	public static class GunTrapFlags
	{
		public const Flags Triggered = Flags.Reserved1;
	}

	public GameObjectRef gun_fire_effect;

	public GameObjectRef bulletEffect;

	public GameObjectRef triggeredEffect;

	public Transform muzzlePos;

	public Transform eyeTransform;

	public int numPellets = 15;

	public int aimCone = 30;

	public float sensorRadius = 1.25f;

	public ItemDefinition ammoType;

	public TargetTrigger trigger;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("GunTrap.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public bool UseAmmo()
	{
		foreach (Item item in base.inventory.itemList)
		{
			if ((Object)(object)item.info == (Object)(object)ammoType && item.amount > 0)
			{
				item.UseItem();
				return true;
			}
		}
		return false;
	}

	public void FireWeapon()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (UseAmmo())
		{
			Effect.server.Run(gun_fire_effect.resourcePath, this, StringPool.Get(((Object)((Component)muzzlePos).get_gameObject()).get_name()), Vector3.get_zero(), Vector3.get_zero());
			for (int i = 0; i < numPellets; i++)
			{
				FireBullet();
			}
		}
	}

	public void FireBullet()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		float damageAmount = 10f;
		Vector3 val = ((Component)muzzlePos).get_transform().get_position() - muzzlePos.get_forward() * 0.25f;
		Vector3 val2 = AimConeUtil.GetModifiedAimConeDirection(inputVec: ((Component)muzzlePos).get_transform().get_forward(), aimCone: aimCone);
		Vector3 arg = val + val2 * 300f;
		ClientRPC<Vector3>(null, "CLIENT_FireGun", arg);
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		int layerMask = 1219701505;
		GamePhysics.TraceAll(new Ray(val, val2), 0.1f, list, 300f, layerMask, (QueryTriggerInteraction)0);
		for (int i = 0; i < list.Count; i++)
		{
			RaycastHit hit = list[i];
			BaseEntity entity = hit.GetEntity();
			if ((Object)(object)entity != (Object)null && ((Object)(object)entity == (Object)(object)this || entity.EqualNetID(this)))
			{
				continue;
			}
			if ((Object)(object)(entity as BaseCombatEntity) != (Object)null)
			{
				HitInfo info = new HitInfo(this, entity, DamageType.Bullet, damageAmount, ((RaycastHit)(ref hit)).get_point());
				entity.OnAttacked(info);
				if (entity is BasePlayer || entity is BaseNpc)
				{
					Effect.server.ImpactEffect(new HitInfo
					{
						HitPositionWorld = ((RaycastHit)(ref hit)).get_point(),
						HitNormalWorld = -((RaycastHit)(ref hit)).get_normal(),
						HitMaterial = StringPool.Get("Flesh")
					});
				}
			}
			if (!((Object)(object)entity != (Object)null) || entity.ShouldBlockProjectiles())
			{
				arg = ((RaycastHit)(ref hit)).get_point();
				break;
			}
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		((FacepunchBehaviour)this).InvokeRandomized((Action)TriggerCheck, Random.Range(0f, 1f), 0.5f, 0.1f);
	}

	public void TriggerCheck()
	{
		if (CheckTrigger())
		{
			FireWeapon();
		}
	}

	public bool CheckTrigger()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		HashSet<BaseEntity> entityContents = trigger.entityContents;
		bool flag = false;
		if (entityContents != null)
		{
			foreach (BaseEntity item in entityContents)
			{
				BasePlayer component = ((Component)item).GetComponent<BasePlayer>();
				if (component.IsSleeping() || !component.IsAlive() || component.IsBuildingAuthed())
				{
					continue;
				}
				list.Clear();
				Vector3 position = component.eyes.position;
				Vector3 val = GetEyePosition() - component.eyes.position;
				GamePhysics.TraceAll(new Ray(position, ((Vector3)(ref val)).get_normalized()), 0f, list, 9f, 1218519297, (QueryTriggerInteraction)0);
				for (int i = 0; i < list.Count; i++)
				{
					BaseEntity entity = list[i].GetEntity();
					if ((Object)(object)entity != (Object)null && ((Object)(object)entity == (Object)(object)this || entity.EqualNetID(this)))
					{
						flag = true;
						break;
					}
					if (!((Object)(object)entity != (Object)null) || entity.ShouldBlockProjectiles())
					{
						break;
					}
				}
				if (flag)
				{
					break;
				}
			}
		}
		Pool.FreeList<RaycastHit>(ref list);
		return flag;
	}

	public bool IsTriggered()
	{
		return HasFlag(Flags.Reserved1);
	}

	public Vector3 GetEyePosition()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return eyeTransform.get_position();
	}
}
