using System;
using System.Collections.Generic;
using Facepunch;
using Rust;
using UnityEngine;

public class FireBall : BaseEntity, ISplashable
{
	public float lifeTimeMin = 20f;

	public float lifeTimeMax = 40f;

	public ParticleSystem[] movementSystems;

	public ParticleSystem[] restingSystems;

	[NonSerialized]
	public float generation;

	public GameObjectRef spreadSubEntity;

	public float tickRate = 0.5f;

	public float damagePerSecond = 2f;

	public float radius = 0.5f;

	public int waterToExtinguish = 200;

	public bool canMerge;

	public LayerMask AttackLayers = LayerMask.op_Implicit(1219701521);

	public bool ignoreNPC;

	private Vector3 lastPos = Vector3.get_zero();

	private float deathTime;

	private int wetness;

	private float spawnTime;

	public override void ServerInit()
	{
		base.ServerInit();
		((FacepunchBehaviour)this).InvokeRepeating((Action)Think, Random.Range(0f, 1f), tickRate);
		float num = Random.Range(lifeTimeMin, lifeTimeMax);
		float num2 = num * Random.Range(0.9f, 1.1f);
		((FacepunchBehaviour)this).Invoke((Action)Extinguish, num2);
		((FacepunchBehaviour)this).Invoke((Action)TryToSpread, num * Random.Range(0.3f, 0.5f));
		deathTime = Time.get_realtimeSinceStartup() + num2;
		spawnTime = Time.get_realtimeSinceStartup();
	}

	public float GetDeathTime()
	{
		return deathTime;
	}

	public void AddLife(float amountToAdd)
	{
		float num = Mathf.Clamp(GetDeathTime() + amountToAdd, 0f, MaxLifeTime());
		((FacepunchBehaviour)this).Invoke((Action)Extinguish, num);
		deathTime = num;
	}

	public float MaxLifeTime()
	{
		return lifeTimeMax * 2.5f;
	}

	public float TimeLeft()
	{
		float num = deathTime - Time.get_realtimeSinceStartup();
		if (num < 0f)
		{
			num = 0f;
		}
		return num;
	}

	public void TryToSpread()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		float num = 0.9f - generation * 0.1f;
		if (Random.Range(0f, 1f) < num && spreadSubEntity.isValid)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(spreadSubEntity.resourcePath);
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				((Component)baseEntity).get_transform().set_position(((Component)this).get_transform().get_position() + Vector3.get_up() * 0.25f);
				baseEntity.Spawn();
				Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(45f, Vector3.get_up());
				baseEntity.creatorEntity = (((Object)(object)creatorEntity == (Object)null) ? baseEntity : creatorEntity);
				baseEntity.SetVelocity(modifiedAimConeDirection * Random.Range(5f, 8f));
				((Component)baseEntity).SendMessage("SetGeneration", (object)(generation + 1f));
			}
		}
	}

	public void SetGeneration(int gen)
	{
		generation = gen;
	}

	public void Think()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (base.isServer)
		{
			SetResting(Vector3.Distance(lastPos, ((Component)this).get_transform().get_localPosition()) < 0.25f);
			lastPos = ((Component)this).get_transform().get_localPosition();
			if (IsResting())
			{
				DoRadialDamage();
			}
			if (WaterFactor() > 0.5f)
			{
				Extinguish();
			}
			if (wetness > waterToExtinguish)
			{
				Extinguish();
			}
		}
	}

	public void DoRadialDamage()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		List<Collider> list = Pool.GetList<Collider>();
		Vector3 position = ((Component)this).get_transform().get_position() + new Vector3(0f, radius * 0.75f, 0f);
		Vis.Colliders<Collider>(position, radius, list, LayerMask.op_Implicit(AttackLayers), (QueryTriggerInteraction)2);
		HitInfo hitInfo = new HitInfo();
		hitInfo.DoHitEffects = true;
		hitInfo.DidHit = true;
		hitInfo.HitBone = 0u;
		hitInfo.Initiator = (((Object)(object)creatorEntity == (Object)null) ? ((Component)this).get_gameObject().ToBaseEntity() : creatorEntity);
		hitInfo.PointStart = ((Component)this).get_transform().get_position();
		foreach (Collider item in list)
		{
			if (item.get_isTrigger() && (((Component)item).get_gameObject().get_layer() == 29 || ((Component)item).get_gameObject().get_layer() == 18))
			{
				continue;
			}
			BaseCombatEntity baseCombatEntity = ((Component)item).get_gameObject().ToBaseEntity() as BaseCombatEntity;
			if (!((Object)(object)baseCombatEntity == (Object)null) && baseCombatEntity.isServer && baseCombatEntity.IsAlive() && (!ignoreNPC || !baseCombatEntity.IsNpc) && baseCombatEntity.IsVisible(position))
			{
				if (baseCombatEntity is BasePlayer)
				{
					Effect.server.Run("assets/bundled/prefabs/fx/impacts/additive/fire.prefab", baseCombatEntity, 0u, new Vector3(0f, 1f, 0f), Vector3.get_up());
				}
				hitInfo.PointEnd = ((Component)baseCombatEntity).get_transform().get_position();
				hitInfo.HitPositionWorld = ((Component)baseCombatEntity).get_transform().get_position();
				hitInfo.damageTypes.Set(DamageType.Heat, damagePerSecond * tickRate);
				baseCombatEntity.OnAttacked(hitInfo);
			}
		}
		Pool.FreeList<Collider>(ref list);
	}

	public bool CanMerge()
	{
		if (canMerge)
		{
			return TimeLeft() < MaxLifeTime() * 0.8f;
		}
		return false;
	}

	public float TimeAlive()
	{
		return Time.get_realtimeSinceStartup() - spawnTime;
	}

	public void SetResting(bool isResting)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (isResting != IsResting() && isResting && TimeAlive() > 1f && CanMerge())
		{
			List<Collider> list = Pool.GetList<Collider>();
			Vis.Colliders<Collider>(((Component)this).get_transform().get_position(), 0.5f, list, 512, (QueryTriggerInteraction)2);
			foreach (Collider item in list)
			{
				BaseEntity baseEntity = ((Component)item).get_gameObject().ToBaseEntity();
				if (Object.op_Implicit((Object)(object)baseEntity))
				{
					FireBall fireBall = baseEntity.ToServer<FireBall>();
					if (Object.op_Implicit((Object)(object)fireBall) && fireBall.CanMerge() && (Object)(object)fireBall != (Object)(object)this)
					{
						((FacepunchBehaviour)fireBall).Invoke((Action)Extinguish, 1f);
						fireBall.canMerge = false;
						AddLife(fireBall.TimeLeft() * 0.25f);
					}
				}
			}
			Pool.FreeList<Collider>(ref list);
		}
		SetFlag(Flags.OnFire, isResting);
	}

	public void Extinguish()
	{
		((FacepunchBehaviour)this).CancelInvoke((Action)Extinguish);
		if (!base.IsDestroyed)
		{
			Kill();
		}
	}

	public bool WantsSplash(ItemDefinition splashType, int amount)
	{
		return !base.IsDestroyed;
	}

	public int DoSplash(ItemDefinition splashType, int amount)
	{
		wetness += amount;
		return amount;
	}

	public bool IsResting()
	{
		return HasFlag(Flags.OnFire);
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
	}
}
