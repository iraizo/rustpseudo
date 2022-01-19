using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using Rust;
using UnityEngine;

public class FlameTurret : StorageContainer
{
	public class UpdateFlameTurretWorkQueue : ObjectWorkQueue<FlameTurret>
	{
		protected override void RunJob(FlameTurret entity)
		{
			if (((ObjectWorkQueue<FlameTurret>)this).ShouldAdd(entity))
			{
				entity.ServerThink();
			}
		}

		protected override bool ShouldAdd(FlameTurret entity)
		{
			if (base.ShouldAdd(entity))
			{
				return entity.IsValid();
			}
			return false;
		}
	}

	public static UpdateFlameTurretWorkQueue updateFlameTurretQueueServer = new UpdateFlameTurretWorkQueue();

	public Transform upper;

	public Vector3 aimDir;

	public float arc = 45f;

	public float triggeredDuration = 5f;

	public float flameRange = 7f;

	public float flameRadius = 4f;

	public float fuelPerSec = 1f;

	public Transform eyeTransform;

	public List<DamageTypeEntry> damagePerSec;

	public GameObjectRef triggeredEffect;

	public GameObjectRef fireballPrefab;

	public GameObjectRef explosionEffect;

	public TargetTrigger trigger;

	private float nextFireballTime;

	private int turnDir = 1;

	private float lastMovementUpdate;

	private float triggeredTime;

	private float lastServerThink;

	private float triggerCheckRate = 2f;

	private float nextTriggerCheckTime;

	private float pendingFuel;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("FlameTurret.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public bool IsTriggered()
	{
		return HasFlag(Flags.Reserved4);
	}

	public Vector3 GetEyePosition()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return eyeTransform.get_position();
	}

	public override bool CanPickup(BasePlayer player)
	{
		if (base.CanPickup(player))
		{
			return !IsTriggered();
		}
		return false;
	}

	public void SetTriggered(bool triggered)
	{
		if (triggered && HasFuel())
		{
			triggeredTime = Time.get_realtimeSinceStartup();
		}
		SetFlag(Flags.Reserved4, triggered && HasFuel());
	}

	public override void ServerInit()
	{
		base.ServerInit();
		((FacepunchBehaviour)this).InvokeRepeating((Action)SendAimDir, 0f, 0.1f);
	}

	public void SendAimDir()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		float delta = Time.get_realtimeSinceStartup() - lastMovementUpdate;
		lastMovementUpdate = Time.get_realtimeSinceStartup();
		MovementUpdate(delta);
		ClientRPC<Vector3>(null, "CLIENT_ReceiveAimDir", aimDir);
		((ObjectWorkQueue<FlameTurret>)updateFlameTurretQueueServer).Add(this);
	}

	public float GetSpinSpeed()
	{
		return IsTriggered() ? 180 : 45;
	}

	public override void OnAttacked(HitInfo info)
	{
		if (!base.isClient)
		{
			if (info.damageTypes.IsMeleeType())
			{
				SetTriggered(triggered: true);
			}
			base.OnAttacked(info);
		}
	}

	public void MovementUpdate(float delta)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		aimDir += new Vector3(0f, delta * GetSpinSpeed(), 0f) * (float)turnDir;
		if (aimDir.y >= arc || aimDir.y <= 0f - arc)
		{
			turnDir *= -1;
			aimDir.y = Mathf.Clamp(aimDir.y, 0f - arc, arc);
		}
	}

	public void ServerThink()
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		bool num = IsTriggered();
		float delta = Time.get_realtimeSinceStartup() - lastServerThink;
		lastServerThink = Time.get_realtimeSinceStartup();
		if (IsTriggered() && (Time.get_realtimeSinceStartup() - triggeredTime > triggeredDuration || !HasFuel()))
		{
			SetTriggered(triggered: false);
		}
		if (!IsTriggered() && HasFuel() && CheckTrigger())
		{
			SetTriggered(triggered: true);
			Effect.server.Run(triggeredEffect.resourcePath, ((Component)this).get_transform().get_position(), Vector3.get_up());
		}
		if (num != IsTriggered())
		{
			SendNetworkUpdateImmediate();
		}
		if (IsTriggered())
		{
			DoFlame(delta);
		}
	}

	public bool CheckTrigger()
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		if (Time.get_realtimeSinceStartup() < nextTriggerCheckTime)
		{
			return false;
		}
		nextTriggerCheckTime = Time.get_realtimeSinceStartup() + 1f / triggerCheckRate;
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		HashSet<BaseEntity> entityContents = trigger.entityContents;
		bool flag = false;
		if (entityContents != null)
		{
			foreach (BaseEntity item in entityContents)
			{
				BasePlayer component = ((Component)item).GetComponent<BasePlayer>();
				if (component.IsSleeping() || !component.IsAlive() || !(((Component)component).get_transform().get_position().y <= GetEyePosition().y + 0.5f) || component.IsBuildingAuthed())
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

	public override void OnKilled(HitInfo info)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)GetFuelAmount() / 500f;
		DamageUtil.RadiusDamage(this, LookupPrefab(), GetEyePosition(), 2f, 6f, damagePerSec, 133120, useLineOfSight: true);
		Effect.server.Run(explosionEffect.resourcePath, ((Component)this).get_transform().get_position(), Vector3.get_up());
		int num2 = Mathf.CeilToInt(Mathf.Clamp(num * 8f, 1f, 8f));
		for (int i = 0; i < num2; i++)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(fireballPrefab.resourcePath, ((Component)this).get_transform().get_position(), ((Component)this).get_transform().get_rotation());
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				Vector3 onUnitSphere = Random.get_onUnitSphere();
				((Component)baseEntity).get_transform().set_position(((Component)this).get_transform().get_position() + new Vector3(0f, 1.5f, 0f) + onUnitSphere * Random.Range(-1f, 1f));
				baseEntity.Spawn();
				baseEntity.SetVelocity(onUnitSphere * (float)Random.Range(3, 10));
			}
		}
		base.OnKilled(info);
	}

	public int GetFuelAmount()
	{
		Item slot = base.inventory.GetSlot(0);
		if (slot == null || slot.amount < 1)
		{
			return 0;
		}
		return slot.amount;
	}

	public bool HasFuel()
	{
		return GetFuelAmount() > 0;
	}

	public bool UseFuel(float seconds)
	{
		Item slot = base.inventory.GetSlot(0);
		if (slot == null || slot.amount < 1)
		{
			return false;
		}
		pendingFuel += seconds * fuelPerSec;
		if (pendingFuel >= 1f)
		{
			int num = Mathf.FloorToInt(pendingFuel);
			slot.UseItem(num);
			pendingFuel -= num;
		}
		return true;
	}

	public void DoFlame(float delta)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		if (!UseFuel(delta))
		{
			return;
		}
		Ray val = default(Ray);
		((Ray)(ref val))._002Ector(GetEyePosition(), ((Component)this).get_transform().TransformDirection(Quaternion.Euler(aimDir) * Vector3.get_forward()));
		Vector3 origin = ((Ray)(ref val)).get_origin();
		RaycastHit val2 = default(RaycastHit);
		bool flag = Physics.SphereCast(val, 0.4f, ref val2, flameRange, 1218652417);
		if (!flag)
		{
			((RaycastHit)(ref val2)).set_point(origin + ((Ray)(ref val)).get_direction() * flameRange);
		}
		float amount = damagePerSec[0].amount;
		damagePerSec[0].amount = amount * delta;
		DamageUtil.RadiusDamage(this, LookupPrefab(), ((RaycastHit)(ref val2)).get_point() - ((Ray)(ref val)).get_direction() * 0.1f, flameRadius * 0.5f, flameRadius, damagePerSec, 2230272, useLineOfSight: true);
		DamageUtil.RadiusDamage(this, LookupPrefab(), ((Component)this).get_transform().get_position() + new Vector3(0f, 1.25f, 0f), 0.25f, 0.25f, damagePerSec, 133120, useLineOfSight: false);
		damagePerSec[0].amount = amount;
		if (Time.get_realtimeSinceStartup() >= nextFireballTime)
		{
			nextFireballTime = Time.get_realtimeSinceStartup() + Random.Range(1f, 2f);
			Vector3 val3 = ((Random.Range(0, 10) <= 7 && flag) ? ((RaycastHit)(ref val2)).get_point() : (((Ray)(ref val)).get_origin() + ((Ray)(ref val)).get_direction() * (flag ? ((RaycastHit)(ref val2)).get_distance() : flameRange) * Random.Range(0.4f, 1f)));
			BaseEntity baseEntity = GameManager.server.CreateEntity(fireballPrefab.resourcePath, val3 - ((Ray)(ref val)).get_direction() * 0.25f);
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				baseEntity.creatorEntity = this;
				baseEntity.Spawn();
			}
		}
	}
}
