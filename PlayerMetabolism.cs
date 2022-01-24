using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;

public class PlayerMetabolism : BaseMetabolism<BasePlayer>
{
	public const float HotThreshold = 40f;

	public const float ColdThreshold = 5f;

	public const float OxygenHurtThreshold = 0.5f;

	public const float OxygenDepleteTime = 10f;

	public const float OxygenRefillTime = 1f;

	public MetabolismAttribute temperature = new MetabolismAttribute();

	public MetabolismAttribute poison = new MetabolismAttribute();

	public MetabolismAttribute radiation_level = new MetabolismAttribute();

	public MetabolismAttribute radiation_poison = new MetabolismAttribute();

	public MetabolismAttribute wetness = new MetabolismAttribute();

	public MetabolismAttribute dirtyness = new MetabolismAttribute();

	public MetabolismAttribute oxygen = new MetabolismAttribute();

	public MetabolismAttribute bleeding = new MetabolismAttribute();

	public MetabolismAttribute comfort = new MetabolismAttribute();

	public MetabolismAttribute pending_health = new MetabolismAttribute();

	public bool isDirty;

	private float lastConsumeTime;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("PlayerMetabolism.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override void Reset()
	{
		base.Reset();
		poison.Reset();
		radiation_level.Reset();
		radiation_poison.Reset();
		temperature.Reset();
		oxygen.Reset();
		bleeding.Reset();
		wetness.Reset();
		dirtyness.Reset();
		comfort.Reset();
		pending_health.Reset();
		lastConsumeTime = float.NegativeInfinity;
		isDirty = true;
	}

	public override void ServerUpdate(BaseCombatEntity ownerEntity, float delta)
	{
		base.ServerUpdate(ownerEntity, delta);
		SendChangesToClient();
	}

	internal bool HasChanged()
	{
		bool flag = isDirty;
		flag = calories.HasChanged() || flag;
		flag = hydration.HasChanged() || flag;
		flag = heartrate.HasChanged() || flag;
		flag = poison.HasChanged() || flag;
		flag = radiation_level.HasChanged() || flag;
		flag = radiation_poison.HasChanged() || flag;
		flag = temperature.HasChanged() || flag;
		flag = wetness.HasChanged() || flag;
		flag = dirtyness.HasChanged() || flag;
		flag = comfort.HasChanged() || flag;
		return pending_health.HasChanged() || flag;
	}

	protected override void DoMetabolismDamage(BaseCombatEntity ownerEntity, float delta)
	{
		base.DoMetabolismDamage(ownerEntity, delta);
		if (temperature.value < -20f)
		{
			owner.Hurt(Mathf.InverseLerp(1f, -50f, temperature.value) * delta * 1f, DamageType.Cold);
		}
		else if (temperature.value < -10f)
		{
			owner.Hurt(Mathf.InverseLerp(1f, -50f, temperature.value) * delta * 0.3f, DamageType.Cold);
		}
		else if (temperature.value < 1f)
		{
			owner.Hurt(Mathf.InverseLerp(1f, -50f, temperature.value) * delta * 0.1f, DamageType.Cold);
		}
		if (temperature.value > 60f)
		{
			owner.Hurt(Mathf.InverseLerp(60f, 200f, temperature.value) * delta * 5f, DamageType.Heat);
		}
		if (oxygen.value < 0.5f)
		{
			owner.Hurt(Mathf.InverseLerp(0.5f, 0f, oxygen.value) * delta * 20f, DamageType.Drowned, null, useProtection: false);
		}
		if (bleeding.value > 0f)
		{
			float num = delta * 0.33333334f;
			owner.Hurt(num, DamageType.Bleeding);
			bleeding.Subtract(num);
		}
		if (poison.value > 0f)
		{
			owner.Hurt(poison.value * delta * 0.1f, DamageType.Poison);
		}
		if (ConVar.Server.radiation && radiation_poison.value > 0f)
		{
			float num2 = (1f + Mathf.Clamp01(radiation_poison.value / 25f) * 5f) * (delta / 5f);
			owner.Hurt(num2, DamageType.Radiation);
			radiation_poison.Subtract(num2);
		}
	}

	public bool SignificantBleeding()
	{
		return bleeding.value > 0f;
	}

	protected override void RunMetabolism(BaseCombatEntity ownerEntity, float delta)
	{
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		float currentTemperature = owner.currentTemperature;
		float fTarget = owner.currentComfort;
		float currentCraftLevel = owner.currentCraftLevel;
		owner.SetPlayerFlag(BasePlayer.PlayerFlags.Workbench1, currentCraftLevel == 1f);
		owner.SetPlayerFlag(BasePlayer.PlayerFlags.Workbench2, currentCraftLevel == 2f);
		owner.SetPlayerFlag(BasePlayer.PlayerFlags.Workbench3, currentCraftLevel == 3f);
		owner.SetPlayerFlag(BasePlayer.PlayerFlags.SafeZone, owner.InSafeZone());
		float num = currentTemperature;
		num -= DeltaWet() * 34f;
		float num2 = Mathf.Clamp(owner.baseProtection.amounts[18] * 1.5f, -1f, 1f);
		float num3 = Mathf.InverseLerp(20f, -50f, currentTemperature);
		float num4 = Mathf.InverseLerp(20f, 30f, currentTemperature);
		num += num3 * 70f * num2;
		num += num4 * 10f * Mathf.Abs(num2);
		num += heartrate.value * 5f;
		temperature.MoveTowards(num, delta * 5f);
		if (temperature.value >= 40f)
		{
			fTarget = 0f;
		}
		comfort.MoveTowards(fTarget, delta / 5f);
		float num5 = 0.6f + 0.4f * comfort.value;
		if (calories.value > 100f && owner.healthFraction < num5 && radiation_poison.Fraction() < 0.25f && owner.SecondsSinceAttacked > 10f && !SignificantBleeding() && temperature.value >= 10f && hydration.value > 40f)
		{
			float num6 = Mathf.InverseLerp(calories.min, calories.max, calories.value);
			float num7 = 5f;
			float num8 = num7 * owner.MaxHealth() * 0.8f / 600f;
			num8 += num8 * num6 * 0.5f;
			float num9 = num8 / num7;
			num9 += num9 * comfort.value * 6f;
			ownerEntity.Heal(num9 * delta);
			calories.Subtract(num8 * delta);
			hydration.Subtract(num8 * delta * 0.2f);
		}
		float num10 = owner.estimatedSpeed2D / owner.GetMaxSpeed() * 0.75f;
		float fTarget2 = Mathf.Clamp(0.05f + num10, 0f, 1f);
		heartrate.MoveTowards(fTarget2, delta * 0.1f);
		if (!owner.IsGod())
		{
			float num11 = heartrate.Fraction() * 0.375f;
			calories.MoveTowards(0f, delta * num11);
			float num12 = 0.008333334f;
			num12 += Mathf.InverseLerp(40f, 60f, temperature.value) * 0.083333336f;
			num12 += heartrate.value * (71f / (339f * (float)Math.PI));
			hydration.MoveTowards(0f, delta * num12);
		}
		bool b = hydration.Fraction() <= 0f || radiation_poison.value >= 100f;
		owner.SetPlayerFlag(BasePlayer.PlayerFlags.NoSprint, b);
		if (temperature.value > 40f)
		{
			hydration.Add(Mathf.InverseLerp(40f, 200f, temperature.value) * delta * -1f);
		}
		if (temperature.value < 10f)
		{
			float num13 = Mathf.InverseLerp(20f, -100f, temperature.value);
			heartrate.MoveTowards(Mathf.Lerp(0.2f, 1f, num13), delta * 2f * num13);
		}
		float num14 = owner.AirFactor();
		float num15 = ((num14 > oxygen.value) ? 1f : 0.1f);
		oxygen.MoveTowards(num14, delta * num15);
		float num16 = 0f;
		float num17 = 0f;
		if (owner.IsOutside(owner.eyes.position))
		{
			num16 = Climate.GetRain(owner.eyes.position) * Weather.wetness_rain;
			num17 = Climate.GetSnow(owner.eyes.position) * Weather.wetness_snow;
		}
		bool flag = owner.baseProtection.amounts[4] > 0f;
		float currentEnvironmentalWetness = owner.currentEnvironmentalWetness;
		currentEnvironmentalWetness = Mathf.Clamp(currentEnvironmentalWetness, 0f, 0.8f);
		float num18 = owner.WaterFactor();
		if (!flag && num18 > 0f)
		{
			wetness.value = Mathf.Max(wetness.value, Mathf.Clamp(num18, wetness.min, wetness.max));
		}
		float num19 = Mathx.Max(wetness.value, num16, num17, currentEnvironmentalWetness);
		num19 = Mathf.Min(num19, flag ? 0f : num19);
		wetness.MoveTowards(num19, delta * 0.05f);
		if (num18 < wetness.value && currentEnvironmentalWetness <= 0f)
		{
			wetness.MoveTowards(0f, delta * 0.2f * Mathf.InverseLerp(0f, 100f, currentTemperature));
		}
		poison.MoveTowards(0f, delta * (5f / 9f));
		if (wetness.Fraction() > 0.4f && owner.estimatedSpeed > 0.25f && radiation_level.Fraction() == 0f)
		{
			radiation_poison.Subtract(radiation_poison.value * 0.2f * wetness.Fraction() * delta * 0.2f);
		}
		if (ConVar.Server.radiation && !owner.IsGod())
		{
			radiation_level.value = owner.radiationLevel;
			if (radiation_level.value > 0f)
			{
				radiation_poison.Add(radiation_level.value * delta);
			}
		}
		if (pending_health.value > 0f)
		{
			float num20 = Mathf.Min(1f * delta, pending_health.value);
			ownerEntity.Heal(num20);
			if (ownerEntity.healthFraction == 1f)
			{
				pending_health.value = 0f;
			}
			else
			{
				pending_health.Subtract(num20);
			}
		}
	}

	private float DeltaHot()
	{
		return Mathf.InverseLerp(20f, 100f, temperature.value);
	}

	private float DeltaCold()
	{
		return Mathf.InverseLerp(20f, -50f, temperature.value);
	}

	private float DeltaWet()
	{
		return wetness.value;
	}

	public void UseHeart(float frate)
	{
		if (heartrate.value > frate)
		{
			heartrate.Add(frate);
		}
		else
		{
			heartrate.value = frate;
		}
	}

	public void SendChangesToClient()
	{
		if (HasChanged())
		{
			isDirty = false;
			PlayerMetabolism val = Save();
			try
			{
				base.baseEntity.ClientRPCPlayerAndSpectators<PlayerMetabolism>(null, base.baseEntity, "UpdateMetabolism", val);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
	}

	public bool CanConsume()
	{
		if (Object.op_Implicit((Object)(object)owner) && owner.IsHeadUnderwater())
		{
			return false;
		}
		return Time.get_time() - lastConsumeTime > 1f;
	}

	public void MarkConsumption()
	{
		lastConsumeTime = Time.get_time();
	}

	public PlayerMetabolism Save()
	{
		PlayerMetabolism val = Pool.Get<PlayerMetabolism>();
		val.calories = calories.value;
		val.hydration = hydration.value;
		val.heartrate = heartrate.value;
		val.temperature = temperature.value;
		val.radiation_level = radiation_level.value;
		val.radiation_poisoning = radiation_poison.value;
		val.wetness = wetness.value;
		val.dirtyness = dirtyness.value;
		val.oxygen = oxygen.value;
		val.bleeding = bleeding.value;
		val.comfort = comfort.value;
		val.pending_health = pending_health.value;
		if (Object.op_Implicit((Object)(object)owner))
		{
			val.health = owner.Health();
		}
		return val;
	}

	public void Load(PlayerMetabolism s)
	{
		calories.SetValue(s.calories);
		hydration.SetValue(s.hydration);
		comfort.SetValue(s.comfort);
		heartrate.value = s.heartrate;
		temperature.value = s.temperature;
		radiation_level.value = s.radiation_level;
		radiation_poison.value = s.radiation_poisoning;
		wetness.value = s.wetness;
		dirtyness.value = s.dirtyness;
		oxygen.value = s.oxygen;
		bleeding.value = s.bleeding;
		pending_health.value = s.pending_health;
		if (Object.op_Implicit((Object)(object)owner))
		{
			owner.health = s.health;
		}
	}

	public override MetabolismAttribute FindAttribute(MetabolismAttribute.Type type)
	{
		return type switch
		{
			MetabolismAttribute.Type.Poison => poison, 
			MetabolismAttribute.Type.Bleeding => bleeding, 
			MetabolismAttribute.Type.Radiation => radiation_poison, 
			MetabolismAttribute.Type.HealthOverTime => pending_health, 
			_ => base.FindAttribute(type), 
		};
	}
}
