using System;
using ConVar;
using Rust;
using UnityEngine;

public static class BaseMetabolism
{
	public const float targetHeartRate = 0.05f;
}
public abstract class BaseMetabolism<T> : EntityComponent<T> where T : BaseCombatEntity
{
	protected T owner;

	public MetabolismAttribute calories = new MetabolismAttribute();

	public MetabolismAttribute hydration = new MetabolismAttribute();

	public MetabolismAttribute heartrate = new MetabolismAttribute();

	protected float timeSinceLastMetabolism;

	public virtual void Reset()
	{
		calories.Reset();
		hydration.Reset();
		heartrate.Reset();
	}

	protected virtual void OnDisable()
	{
		if (!Application.isQuitting)
		{
			owner = null;
		}
	}

	public virtual void ServerInit(T owner)
	{
		Reset();
		this.owner = owner;
	}

	public virtual void ServerUpdate(BaseCombatEntity ownerEntity, float delta)
	{
		timeSinceLastMetabolism += delta;
		if (!(timeSinceLastMetabolism <= ConVar.Server.metabolismtick))
		{
			if (Object.op_Implicit((Object)(object)owner) && !owner.IsDead())
			{
				RunMetabolism(ownerEntity, timeSinceLastMetabolism);
				DoMetabolismDamage(ownerEntity, timeSinceLastMetabolism);
			}
			timeSinceLastMetabolism = 0f;
		}
	}

	protected virtual void DoMetabolismDamage(BaseCombatEntity ownerEntity, float delta)
	{
		if (calories.value <= 20f)
		{
			TimeWarning val = TimeWarning.New("Calories Hurt", 0);
			try
			{
				ownerEntity.Hurt(Mathf.InverseLerp(20f, 0f, calories.value) * delta * 0.083333336f, DamageType.Hunger);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		if (hydration.value <= 20f)
		{
			TimeWarning val = TimeWarning.New("Hyration Hurt", 0);
			try
			{
				ownerEntity.Hurt(Mathf.InverseLerp(20f, 0f, hydration.value) * delta * (142f / (339f * (float)Math.PI)), DamageType.Thirst);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
	}

	protected virtual void RunMetabolism(BaseCombatEntity ownerEntity, float delta)
	{
		if (calories.value > 200f)
		{
			ownerEntity.Heal(Mathf.InverseLerp(200f, 1000f, calories.value) * delta * 0.016666668f);
		}
		if (hydration.value > 200f)
		{
			ownerEntity.Heal(Mathf.InverseLerp(200f, 1000f, hydration.value) * delta * 0.016666668f);
		}
		hydration.MoveTowards(0f, delta * 0.008333334f);
		calories.MoveTowards(0f, delta * 0.016666668f);
		heartrate.MoveTowards(0.05f, delta * 0.016666668f);
	}

	public void ApplyChange(MetabolismAttribute.Type type, float amount, float time)
	{
		FindAttribute(type)?.Add(amount);
	}

	public bool ShouldDie()
	{
		if (Object.op_Implicit((Object)(object)owner))
		{
			return owner.Health() <= 0f;
		}
		return false;
	}

	public virtual MetabolismAttribute FindAttribute(MetabolismAttribute.Type type)
	{
		return type switch
		{
			MetabolismAttribute.Type.Calories => calories, 
			MetabolismAttribute.Type.Hydration => hydration, 
			MetabolismAttribute.Type.Heartrate => heartrate, 
			_ => null, 
		};
	}
}
