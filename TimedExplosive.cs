using System;
using System.Collections.Generic;
using Rust;
using Rust.Ai;
using UnityEngine;

public class TimedExplosive : BaseEntity, ServerProjectile.IProjectileImpact
{
	public float timerAmountMin = 10f;

	public float timerAmountMax = 20f;

	public float minExplosionRadius;

	public float explosionRadius = 10f;

	public bool canStick;

	public bool onlyDamageParent;

	public GameObjectRef explosionEffect;

	[Tooltip("Optional: Will fall back to explosionEffect if not assigned.")]
	public GameObjectRef underwaterExplosionEffect;

	public GameObjectRef stickEffect;

	public GameObjectRef bounceEffect;

	public bool explosionUsesForward;

	public bool waterCausesExplosion;

	public List<DamageTypeEntry> damageTypes = new List<DamageTypeEntry>();

	[NonSerialized]
	private float lastBounceTime;

	private CollisionDetectionMode? initialCollisionDetectionMode;

	public void SetDamageScale(float scale)
	{
		foreach (DamageTypeEntry damageType in damageTypes)
		{
			damageType.amount *= scale;
		}
	}

	public override float GetNetworkTime()
	{
		return Time.get_fixedTime();
	}

	public override void ServerInit()
	{
		lastBounceTime = Time.get_time();
		base.ServerInit();
		SetFuse(GetRandomTimerTime());
		ReceiveCollisionMessages(b: true);
		if (waterCausesExplosion)
		{
			((FacepunchBehaviour)this).InvokeRepeating((Action)WaterCheck, 0f, 0.5f);
		}
	}

	public void WaterCheck()
	{
		if (waterCausesExplosion && WaterFactor() >= 0.5f)
		{
			Explode();
		}
	}

	public virtual void SetFuse(float fuseLength)
	{
		if (base.isServer)
		{
			((FacepunchBehaviour)this).Invoke((Action)Explode, fuseLength);
		}
	}

	public virtual float GetRandomTimerTime()
	{
		return Random.Range(timerAmountMin, timerAmountMax);
	}

	public virtual void ProjectileImpact(RaycastHit info, Vector3 rayOrigin)
	{
		Explode();
	}

	public virtual void Explode()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		Explode(PivotPoint());
	}

	public virtual void Explode(Vector3 explosionFxPos)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		Collider component = ((Component)this).GetComponent<Collider>();
		if (Object.op_Implicit((Object)(object)component))
		{
			component.set_enabled(false);
		}
		bool flag = false;
		if (underwaterExplosionEffect.isValid)
		{
			flag = WaterLevel.GetWaterDepth(((Component)this).get_transform().get_position()) > 1f;
		}
		if (flag)
		{
			Effect.server.Run(underwaterExplosionEffect.resourcePath, explosionFxPos, explosionUsesForward ? ((Component)this).get_transform().get_forward() : Vector3.get_up(), null, broadcast: true);
		}
		else if (explosionEffect.isValid)
		{
			Effect.server.Run(explosionEffect.resourcePath, explosionFxPos, explosionUsesForward ? ((Component)this).get_transform().get_forward() : Vector3.get_up(), null, broadcast: true);
		}
		if (damageTypes.Count > 0)
		{
			Sensation sensation;
			if (onlyDamageParent)
			{
				DamageUtil.RadiusDamage(creatorEntity, LookupPrefab(), CenterPoint(), minExplosionRadius, explosionRadius, damageTypes, 166144, useLineOfSight: true);
				BaseEntity baseEntity = GetParentEntity();
				BaseCombatEntity baseCombatEntity = baseEntity as BaseCombatEntity;
				while ((Object)(object)baseCombatEntity == (Object)null && (Object)(object)baseEntity != (Object)null && baseEntity.HasParent())
				{
					baseEntity = baseEntity.GetParentEntity();
					baseCombatEntity = baseEntity as BaseCombatEntity;
				}
				if (Object.op_Implicit((Object)(object)baseCombatEntity))
				{
					HitInfo hitInfo = new HitInfo();
					hitInfo.Initiator = creatorEntity;
					hitInfo.WeaponPrefab = LookupPrefab();
					hitInfo.damageTypes.Add(damageTypes);
					baseCombatEntity.Hurt(hitInfo);
				}
				if ((Object)(object)creatorEntity != (Object)null && damageTypes != null)
				{
					float num = 0f;
					foreach (DamageTypeEntry damageType in damageTypes)
					{
						num += damageType.amount;
					}
					sensation = default(Sensation);
					sensation.Type = SensationType.Explosion;
					sensation.Position = ((Component)creatorEntity).get_transform().get_position();
					sensation.Radius = explosionRadius * 17f;
					sensation.DamagePotential = num;
					sensation.InitiatorPlayer = creatorEntity as BasePlayer;
					sensation.Initiator = creatorEntity;
					Sense.Stimulate(sensation);
				}
			}
			else
			{
				DamageUtil.RadiusDamage(creatorEntity, LookupPrefab(), CenterPoint(), minExplosionRadius, explosionRadius, damageTypes, 1076005121, useLineOfSight: true);
				if ((Object)(object)creatorEntity != (Object)null && damageTypes != null)
				{
					float num2 = 0f;
					foreach (DamageTypeEntry damageType2 in damageTypes)
					{
						num2 += damageType2.amount;
					}
					sensation = default(Sensation);
					sensation.Type = SensationType.Explosion;
					sensation.Position = ((Component)creatorEntity).get_transform().get_position();
					sensation.Radius = explosionRadius * 17f;
					sensation.DamagePotential = num2;
					sensation.InitiatorPlayer = creatorEntity as BasePlayer;
					sensation.Initiator = creatorEntity;
					Sense.Stimulate(sensation);
				}
			}
		}
		if (!base.IsDestroyed)
		{
			Kill(DestroyMode.Gib);
		}
	}

	public override void OnCollision(Collision collision, BaseEntity hitEntity)
	{
		if (canStick && !IsStuck())
		{
			bool flag = true;
			if (Object.op_Implicit((Object)(object)hitEntity))
			{
				flag = CanStickTo(hitEntity);
				if (!flag)
				{
					Collider component = ((Component)this).GetComponent<Collider>();
					if ((Object)(object)collision.get_collider() != (Object)null && (Object)(object)component != (Object)null)
					{
						Physics.IgnoreCollision(collision.get_collider(), component);
					}
				}
			}
			if (flag)
			{
				DoCollisionStick(collision, hitEntity);
			}
		}
		DoBounceEffect();
	}

	public virtual bool CanStickTo(BaseEntity entity)
	{
		return (Object)(object)((Component)entity).GetComponent<DecorDeployable>() == (Object)null;
	}

	private void DoBounceEffect()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (!bounceEffect.isValid || Time.get_time() - lastBounceTime < 0.2f)
		{
			return;
		}
		Rigidbody component = ((Component)this).GetComponent<Rigidbody>();
		if (Object.op_Implicit((Object)(object)component))
		{
			Vector3 velocity = component.get_velocity();
			if (((Vector3)(ref velocity)).get_magnitude() < 1f)
			{
				return;
			}
		}
		if (bounceEffect.isValid)
		{
			Effect.server.Run(bounceEffect.resourcePath, ((Component)this).get_transform().get_position(), Vector3.get_up(), null, broadcast: true);
		}
		lastBounceTime = Time.get_time();
	}

	private void DoCollisionStick(Collision collision, BaseEntity ent)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		ContactPoint contact = collision.GetContact(0);
		DoStick(((ContactPoint)(ref contact)).get_point(), ((ContactPoint)(ref contact)).get_normal(), ent, collision.get_collider());
	}

	public virtual void SetMotionEnabled(bool wantsMotion)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		Rigidbody component = ((Component)this).GetComponent<Rigidbody>();
		if (Object.op_Implicit((Object)(object)component))
		{
			if (!initialCollisionDetectionMode.HasValue)
			{
				initialCollisionDetectionMode = component.get_collisionDetectionMode();
			}
			component.set_useGravity(wantsMotion);
			if (!wantsMotion)
			{
				component.set_collisionDetectionMode((CollisionDetectionMode)0);
			}
			component.set_isKinematic(!wantsMotion);
			if (wantsMotion)
			{
				component.set_collisionDetectionMode(initialCollisionDetectionMode.Value);
			}
		}
	}

	public bool IsStuck()
	{
		Rigidbody component = ((Component)this).GetComponent<Rigidbody>();
		if (Object.op_Implicit((Object)(object)component) && !component.get_isKinematic())
		{
			return false;
		}
		Collider component2 = ((Component)this).GetComponent<Collider>();
		if (Object.op_Implicit((Object)(object)component2) && component2.get_enabled())
		{
			return false;
		}
		return parentEntity.IsValid(serverside: true);
	}

	public void DoStick(Vector3 position, Vector3 normal, BaseEntity ent, Collider collider)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)ent == (Object)null)
		{
			return;
		}
		if (ent is TimedExplosive)
		{
			if (!ent.HasParent())
			{
				return;
			}
			position = ((Component)ent).get_transform().get_position();
			ent = ent.parentEntity.Get(serverside: true);
		}
		SetMotionEnabled(wantsMotion: false);
		SetCollisionEnabled(wantsCollision: false);
		if (!HasChild(ent))
		{
			((Component)this).get_transform().set_position(position);
			((Component)this).get_transform().set_rotation(Quaternion.LookRotation(normal, ((Component)this).get_transform().get_up()));
			if ((Object)(object)collider != (Object)null)
			{
				SetParent(ent, ent.FindBoneID(((Component)collider).get_transform()), worldPositionStays: true);
			}
			else
			{
				SetParent(ent, StringPool.closest, worldPositionStays: true);
			}
			if (stickEffect.isValid)
			{
				Effect.server.Run(stickEffect.resourcePath, ((Component)this).get_transform().get_position(), Vector3.get_up(), null, broadcast: true);
			}
			ReceiveCollisionMessages(b: false);
		}
	}

	private void UnStick()
	{
		if (Object.op_Implicit((Object)(object)GetParentEntity()))
		{
			SetParent(null, worldPositionStays: true, sendImmediate: true);
			SetMotionEnabled(wantsMotion: true);
			SetCollisionEnabled(wantsCollision: true);
			ReceiveCollisionMessages(b: true);
		}
	}

	internal override void OnParentRemoved()
	{
		UnStick();
	}

	public virtual void SetCollisionEnabled(bool wantsCollision)
	{
		Collider component = ((Component)this).GetComponent<Collider>();
		if (Object.op_Implicit((Object)(object)component) && component.get_enabled() != wantsCollision)
		{
			component.set_enabled(wantsCollision);
		}
	}
}
