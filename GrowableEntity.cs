using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class GrowableEntity : BaseCombatEntity, IInstanceDataReceiver
{
	public class GrowableEntityUpdateQueue : ObjectWorkQueue<GrowableEntity>
	{
		protected override void RunJob(GrowableEntity entity)
		{
			if (((ObjectWorkQueue<GrowableEntity>)this).ShouldAdd(entity))
			{
				entity.CalculateQualities_Water();
				entity.SendNetworkUpdate();
			}
		}

		protected override bool ShouldAdd(GrowableEntity entity)
		{
			if (base.ShouldAdd(entity))
			{
				return entity.IsValid();
			}
			return false;
		}
	}

	private const float artificalLightQuality = 1f;

	private const float planterGroundModifierBase = 0.6f;

	private const float fertilizerGroundModifierBonus = 0.4f;

	private const float growthGeneSpeedMultiplier = 0.25f;

	private const float waterGeneRequirementMultiplier = 0.1f;

	private const float hardinessGeneModifierBonus = 0.2f;

	private const float hardinessGeneTemperatureModifierBonus = 0.05f;

	private const float baseYieldIncreaseMultiplier = 1f;

	private const float yieldGeneBonusMultiplier = 0.25f;

	private const float maxNonPlanterGroundQuality = 0.6f;

	private const float deathRatePerQuality = 0.1f;

	private TimeCachedValue<float> sunExposure;

	private TimeCachedValue<float> artificialLightExposure;

	private TimeCachedValue<float> artificialTemperatureExposure;

	[ServerVar]
	[Help("How many miliseconds to budget for processing growable quality updates per frame")]
	public static float framebudgetms = 0.25f;

	public static GrowableEntityUpdateQueue growableEntityUpdateQueue = new GrowableEntityUpdateQueue();

	private bool underWater;

	private int seasons;

	private int harvests;

	private float terrainTypeValue;

	private float yieldPool;

	private PlanterBox planter;

	public PlantProperties Properties;

	public ItemDefinition SourceItemDef;

	private float stageAge;

	public GrowableGenes Genes = new GrowableGenes();

	private const float startingHealth = 10f;

	public float CurrentTemperature
	{
		get
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)GetPlanter() != (Object)null)
			{
				return GetPlanter().GetPlantTemperature();
			}
			return Climate.GetTemperature(((Component)this).get_transform().get_position()) + (artificialTemperatureExposure?.Get(force: false) ?? 0f);
		}
	}

	public PlantProperties.State State { get; private set; }

	public float Age { get; private set; }

	public float LightQuality { get; private set; }

	public float GroundQuality { get; private set; } = 1f;


	public float WaterQuality { get; private set; }

	public float WaterConsumption { get; private set; }

	public bool Fertilized { get; private set; }

	public float TemperatureQuality { get; private set; }

	public float OverallQuality { get; private set; }

	public float Yield { get; private set; }

	public float StageProgressFraction => stageAge / currentStage.lifeLengthSeconds;

	private PlantProperties.Stage currentStage => Properties.stages[(int)State];

	public static float ThinkDeltaTime => ConVar.Server.planttick;

	private float growDeltaTime => ConVar.Server.planttick * ConVar.Server.planttickscale;

	public int CurrentPickAmount => Mathf.RoundToInt(CurrentPickAmountFloat);

	public float CurrentPickAmountFloat => (currentStage.resources + Yield) * (float)Properties.pickupMultiplier;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("GrowableEntity.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 598660365 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_PickFruit "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_PickFruit", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(598660365u, "RPC_PickFruit", this, player, 3f))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(598660365u, "RPC_PickFruit", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							RPCMessage msg2 = rPCMessage;
							RPC_PickFruit(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_PickFruit");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1959480148 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_RemoveDying "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_RemoveDying", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(1959480148u, "RPC_RemoveDying", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							RPCMessage msg3 = rPCMessage;
							RPC_RemoveDying(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_RemoveDying");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2222960834u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_TakeClone "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_TakeClone", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(2222960834u, "RPC_TakeClone", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							RPCMessage msg4 = rPCMessage;
							RPC_TakeClone(msg4);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in RPC_TakeClone");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public void QueueForQualityUpdate()
	{
		((ObjectWorkQueue<GrowableEntity>)growableEntityUpdateQueue).Add(this);
	}

	public void CalculateQualities(bool firstTime, bool forceArtificialLightUpdates = false, bool forceArtificialTemperatureUpdates = false)
	{
		if (!IsDead())
		{
			if (sunExposure == null)
			{
				sunExposure = new TimeCachedValue<float>
				{
					refreshCooldown = 30f,
					refreshRandomRange = 5f,
					updateValue = SunRaycast
				};
			}
			if (artificialLightExposure == null)
			{
				artificialLightExposure = new TimeCachedValue<float>
				{
					refreshCooldown = 60f,
					refreshRandomRange = 5f,
					updateValue = CalculateArtificialLightExposure
				};
			}
			if (artificialTemperatureExposure == null)
			{
				artificialTemperatureExposure = new TimeCachedValue<float>
				{
					refreshCooldown = 60f,
					refreshRandomRange = 5f,
					updateValue = CalculateArtificialTemperature
				};
			}
			if (forceArtificialTemperatureUpdates)
			{
				artificialTemperatureExposure.ForceNextRun();
			}
			CalculateLightQuality(forceArtificialLightUpdates || firstTime);
			CalculateWaterQuality();
			CalculateWaterConsumption();
			CalculateGroundQuality(firstTime);
			CalculateTemperatureQuality();
			CalculateOverallQuality();
		}
	}

	private void CalculateQualities_Water()
	{
		CalculateWaterQuality();
		CalculateWaterConsumption();
		CalculateOverallQuality();
	}

	public void CalculateLightQuality(bool forceArtificalUpdate)
	{
		float num = Mathf.Clamp01(Properties.timeOfDayHappiness.Evaluate(TOD_Sky.get_Instance().Cycle.Hour));
		if (!ConVar.Server.plantlightdetection)
		{
			LightQuality = num;
			return;
		}
		LightQuality = CalculateSunExposure(forceArtificalUpdate) * num;
		if (LightQuality <= 0f)
		{
			LightQuality = GetArtificialLightExposure(forceArtificalUpdate);
		}
		LightQuality = RemapValue(LightQuality, 0f, Properties.OptimalLightQuality, 0f, 1f);
	}

	private float CalculateSunExposure(bool force)
	{
		if (TOD_Sky.get_Instance().get_IsNight())
		{
			return 0f;
		}
		if ((Object)(object)GetPlanter() != (Object)null)
		{
			return GetPlanter().GetSunExposure();
		}
		return sunExposure?.Get(force) ?? 0f;
	}

	private float SunRaycast()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		return SunRaycast(((Component)this).get_transform().get_position() + new Vector3(0f, 1f, 0f));
	}

	private float GetArtificialLightExposure(bool force)
	{
		if ((Object)(object)GetPlanter() != (Object)null)
		{
			return GetPlanter().GetArtificialLightExposure();
		}
		return artificialLightExposure?.Get(force) ?? 0f;
	}

	private float CalculateArtificialLightExposure()
	{
		return CalculateArtificialLightExposure(((Component)this).get_transform());
	}

	public static float CalculateArtificialLightExposure(Transform forTransform)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		float result = 0f;
		List<CeilingLight> list = Pool.GetList<CeilingLight>();
		Vis.Entities(forTransform.get_position() + new Vector3(0f, ConVar.Server.ceilingLightHeightOffset, 0f), ConVar.Server.ceilingLightGrowableRange, list, 256, (QueryTriggerInteraction)2);
		foreach (CeilingLight item in list)
		{
			if (item.IsOn())
			{
				result = 1f;
				break;
			}
		}
		Pool.FreeList<CeilingLight>(ref list);
		return result;
	}

	public static float SunRaycast(Vector3 checkPosition)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = TOD_Sky.get_Instance().get_Components().Sun.get_transform().get_position() - checkPosition;
		Vector3 normalized = ((Vector3)(ref val)).get_normalized();
		RaycastHit val2 = default(RaycastHit);
		if (!Physics.Raycast(checkPosition, normalized, ref val2, 100f, 10551297))
		{
			return 1f;
		}
		return 0f;
	}

	public void CalculateWaterQuality()
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Invalid comparison between Unknown and I4
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Invalid comparison between Unknown and I4
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Invalid comparison between Unknown and I4
		if ((Object)(object)GetPlanter() != (Object)null)
		{
			float soilSaturationFraction = planter.soilSaturationFraction;
			if (soilSaturationFraction > ConVar.Server.optimalPlanterQualitySaturation)
			{
				WaterQuality = RemapValue(soilSaturationFraction, ConVar.Server.optimalPlanterQualitySaturation, 1f, 1f, 0.6f);
			}
			else
			{
				WaterQuality = RemapValue(soilSaturationFraction, 0f, ConVar.Server.optimalPlanterQualitySaturation, 0f, 1f);
			}
		}
		else
		{
			Enum val = (Enum)TerrainMeta.BiomeMap.GetBiomeMaxType(((Component)this).get_transform().get_position());
			if (val - 1 > 1 && (int)val != 4)
			{
				if ((int)val == 8)
				{
					WaterQuality = 0.1f;
				}
				else
				{
					WaterQuality = 0f;
				}
			}
			else
			{
				WaterQuality = 0.3f;
			}
		}
		WaterQuality = Mathf.Clamp01(WaterQuality);
		WaterQuality = RemapValue(WaterQuality, 0f, Properties.OptimalWaterQuality, 0f, 1f);
	}

	public void CalculateGroundQuality(bool firstCheck)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (underWater && !firstCheck)
		{
			GroundQuality = 0f;
			return;
		}
		if (firstCheck)
		{
			Vector3 position = ((Component)this).get_transform().get_position();
			if (WaterLevel.Test(position, waves: true, this))
			{
				underWater = true;
				GroundQuality = 0f;
				return;
			}
			underWater = false;
			terrainTypeValue = GetGroundTypeValue(position);
		}
		if ((Object)(object)GetPlanter() != (Object)null)
		{
			GroundQuality = 0.6f;
			GroundQuality += (Fertilized ? 0.4f : 0f);
		}
		else
		{
			GroundQuality = terrainTypeValue;
			float num = (float)Genes.GetGeneTypeCount(GrowableGenetics.GeneType.Hardiness) * 0.2f;
			float num2 = GroundQuality + num;
			GroundQuality = Mathf.Min(0.6f, num2);
		}
		GroundQuality = RemapValue(GroundQuality, 0f, Properties.OptimalGroundQuality, 0f, 1f);
	}

	private float GetGroundTypeValue(Vector3 pos)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Invalid comparison between Unknown and I4
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected I4, but got Unknown
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Invalid comparison between Unknown and I4
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Invalid comparison between Unknown and I4
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Invalid comparison between Unknown and I4
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Invalid comparison between Unknown and I4
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Invalid comparison between Unknown and I4
		Enum val = (Enum)TerrainMeta.SplatMap.GetSplatMaxType(pos);
		if ((int)val <= 16)
		{
			switch (val - 1)
			{
			default:
				if ((int)val != 8)
				{
					if ((int)val != 16)
					{
						break;
					}
					return 0.3f;
				}
				return 0f;
			case 1:
				return 0f;
			case 0:
				return 0.3f;
			case 3:
				return 0f;
			case 2:
				break;
			}
		}
		else
		{
			if ((int)val == 32)
			{
				return 0.2f;
			}
			if ((int)val == 64)
			{
				return 0f;
			}
			if ((int)val == 128)
			{
				return 0f;
			}
		}
		return 0.5f;
	}

	private void CalculateTemperatureQuality()
	{
		TemperatureQuality = Mathf.Clamp01(Properties.temperatureHappiness.Evaluate(CurrentTemperature));
		float num = (float)Genes.GetGeneTypeCount(GrowableGenetics.GeneType.Hardiness) * 0.05f;
		TemperatureQuality = Mathf.Clamp01(TemperatureQuality + num);
		TemperatureQuality = RemapValue(TemperatureQuality, 0f, Properties.OptimalTemperatureQuality, 0f, 1f);
	}

	public float CalculateOverallQuality()
	{
		float num = 1f;
		if (ConVar.Server.useMinimumPlantCondition)
		{
			num = Mathf.Min(num, LightQuality);
			num = Mathf.Min(num, WaterQuality);
			num = Mathf.Min(num, GroundQuality);
			num = Mathf.Min(num, TemperatureQuality);
		}
		else
		{
			num = LightQuality * WaterQuality * GroundQuality * TemperatureQuality;
		}
		OverallQuality = num;
		return OverallQuality;
	}

	public void CalculateWaterConsumption()
	{
		float num = Properties.temperatureWaterRequirementMultiplier.Evaluate(CurrentTemperature);
		float num2 = 1f + (float)Genes.GetGeneTypeCount(GrowableGenetics.GeneType.WaterRequirement) * 0.1f;
		WaterConsumption = Properties.WaterIntake * num * num2;
	}

	private float CalculateArtificialTemperature()
	{
		return CalculateArtificialTemperature(((Component)this).get_transform());
	}

	public static float CalculateArtificialTemperature(Transform forTransform)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = forTransform.get_position();
		List<GrowableHeatSource> list = Pool.GetList<GrowableHeatSource>();
		Vis.Components<GrowableHeatSource>(position, ConVar.Server.artificialTemperatureGrowableRange, list, 256, (QueryTriggerInteraction)2);
		float num = 0f;
		foreach (GrowableHeatSource item in list)
		{
			num = Mathf.Max(item.ApplyHeat(position), num);
		}
		Pool.FreeList<GrowableHeatSource>(ref list);
		return num;
	}

	public int CalculateMarketValue()
	{
		int baseMarketValue = Properties.BaseMarketValue;
		int num = Genes.GetPositiveGeneCount() * 10;
		int num2 = Genes.GetNegativeGeneCount() * -10;
		baseMarketValue += num;
		baseMarketValue += num2;
		return Mathf.Max(0, baseMarketValue);
	}

	private static float RemapValue(float inValue, float minA, float maxA, float minB, float maxB)
	{
		if (inValue >= maxA)
		{
			return maxB;
		}
		float num = Mathf.InverseLerp(minA, maxA, inValue);
		return Mathf.Lerp(minB, maxB, num);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		((FacepunchBehaviour)this).InvokeRandomized((Action)RunUpdate, ThinkDeltaTime, ThinkDeltaTime, ThinkDeltaTime * 0.1f);
		base.health = 10f;
		ResetSeason();
		Genes.GenerateRandom(this);
		if (!Application.isLoadingSave)
		{
			CalculateQualities(firstTime: true);
		}
	}

	public PlanterBox GetPlanter()
	{
		if ((Object)(object)planter == (Object)null)
		{
			BaseEntity baseEntity = GetParentEntity();
			if ((Object)(object)baseEntity != (Object)null)
			{
				planter = baseEntity as PlanterBox;
			}
		}
		return planter;
	}

	public override void OnParentChanging(BaseEntity oldParent, BaseEntity newParent)
	{
		base.OnParentChanging(oldParent, newParent);
		planter = newParent as PlanterBox;
		if (!Application.isLoadingSave && (Object)(object)planter != (Object)null)
		{
			planter.FertilizeGrowables();
		}
		CalculateQualities(firstTime: true);
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		CalculateQualities(firstTime: true);
	}

	public void ResetSeason()
	{
		Yield = 0f;
		yieldPool = 0f;
	}

	private void RunUpdate()
	{
		if (!IsDead())
		{
			CalculateQualities(firstTime: false);
			float overallQuality = CalculateOverallQuality();
			float actualStageAgeIncrease = UpdateAge(overallQuality);
			UpdateHealthAndYield(overallQuality, actualStageAgeIncrease);
			if (base.health <= 0f)
			{
				Die();
				return;
			}
			UpdateState();
			ConsumeWater();
			SendNetworkUpdate();
		}
	}

	private float UpdateAge(float overallQuality)
	{
		Age += growDeltaTime;
		float num = (currentStage.IgnoreConditions ? 1f : (Mathf.Max(overallQuality, 0f) * GetGrowthBonus(overallQuality)));
		float num2 = growDeltaTime * num;
		stageAge += num2;
		return num2;
	}

	private void UpdateHealthAndYield(float overallQuality, float actualStageAgeIncrease)
	{
		if ((Object)(object)GetPlanter() == (Object)null && Random.Range(0f, 1f) <= ConVar.Server.nonPlanterDeathChancePerTick)
		{
			base.health = 0f;
			return;
		}
		if (overallQuality <= 0f)
		{
			ApplyDeathRate();
		}
		base.health += overallQuality * currentStage.health * growDeltaTime;
		if (yieldPool > 0f)
		{
			float num = currentStage.yield / (currentStage.lifeLengthSeconds / growDeltaTime);
			float num2 = Mathf.Min(yieldPool, num * (actualStageAgeIncrease / growDeltaTime));
			yieldPool -= num;
			float num3 = 1f + (float)Genes.GetGeneTypeCount(GrowableGenetics.GeneType.Yield) * 0.25f;
			Yield += num2 * 1f * num3;
		}
	}

	private void ApplyDeathRate()
	{
		float num = 0f;
		if (WaterQuality <= 0f)
		{
			num += 0.1f;
		}
		if (LightQuality <= 0f)
		{
			num += 0.1f;
		}
		if (GroundQuality <= 0f)
		{
			num += 0.1f;
		}
		if (TemperatureQuality <= 0f)
		{
			num += 0.1f;
		}
		base.health -= num;
	}

	private float GetGrowthBonus(float overallQuality)
	{
		float result = 1f + (float)Genes.GetGeneTypeCount(GrowableGenetics.GeneType.GrowthSpeed) * 0.25f;
		if (overallQuality <= 0f)
		{
			result = 1f;
		}
		return result;
	}

	private PlantProperties.State UpdateState()
	{
		if (stageAge <= currentStage.lifeLengthSeconds)
		{
			return State;
		}
		if (State == PlantProperties.State.Dying)
		{
			Die();
			return PlantProperties.State.Dying;
		}
		if (currentStage.nextState <= State)
		{
			seasons++;
		}
		if (seasons >= Properties.MaxSeasons)
		{
			ChangeState(PlantProperties.State.Dying, resetAge: true);
		}
		else
		{
			ChangeState(currentStage.nextState, resetAge: true);
		}
		return State;
	}

	private void ConsumeWater()
	{
		if (State != PlantProperties.State.Dying && !((Object)(object)GetPlanter() == (Object)null))
		{
			int num = Mathf.CeilToInt(Mathf.Min((float)planter.soilSaturation, WaterConsumption));
			if ((float)num > 0f)
			{
				planter.ConsumeWater(num, this);
			}
		}
	}

	public void Fertilize()
	{
		if (!Fertilized)
		{
			Fertilized = true;
			CalculateQualities(firstTime: false);
			SendNetworkUpdate();
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void RPC_TakeClone(RPCMessage msg)
	{
		TakeClones(msg.player);
	}

	private void TakeClones(BasePlayer player)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)player == (Object)null || !CanClone())
		{
			return;
		}
		int num = Properties.BaseCloneCount + Genes.GetGeneTypeCount(GrowableGenetics.GeneType.Yield) / 2;
		if (num > 0)
		{
			Item item = ItemManager.Create(Properties.CloneItem, num, 0uL);
			GrowableGeneEncoding.EncodeGenesToItem(this, item);
			player.GiveItem(item, GiveItemReason.PickedUp);
			if (Properties.pickEffect.isValid)
			{
				Effect.server.Run(Properties.pickEffect.resourcePath, ((Component)this).get_transform().get_position(), Vector3.get_up());
			}
			Die();
		}
	}

	public void PickFruit(BasePlayer player)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		if (!CanPick())
		{
			return;
		}
		harvests++;
		GiveFruit(player, CurrentPickAmount);
		RandomItemDispenser randomItemDispenser = PrefabAttribute.server.Find<RandomItemDispenser>(prefabID);
		if (randomItemDispenser != null)
		{
			randomItemDispenser.DistributeItems(player, ((Component)this).get_transform().get_position());
		}
		ResetSeason();
		if (Properties.pickEffect.isValid)
		{
			Effect.server.Run(Properties.pickEffect.resourcePath, ((Component)this).get_transform().get_position(), Vector3.get_up());
		}
		if (harvests >= Properties.maxHarvests)
		{
			if (Properties.disappearAfterHarvest)
			{
				Die();
			}
			else
			{
				ChangeState(PlantProperties.State.Dying, resetAge: true);
			}
		}
		else
		{
			ChangeState(PlantProperties.State.Mature, resetAge: true);
		}
	}

	private void GiveFruit(BasePlayer player, int amount)
	{
		if (amount <= 0)
		{
			return;
		}
		bool enabled = Properties.pickupItem.condition.enabled;
		if (enabled)
		{
			for (int i = 0; i < amount; i++)
			{
				GiveFruit(player, 1, enabled);
			}
		}
		else
		{
			GiveFruit(player, amount, enabled);
		}
	}

	private void GiveFruit(BasePlayer player, int amount, bool applyCondition)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		Item item = ItemManager.Create(Properties.pickupItem, amount, 0uL);
		if (applyCondition)
		{
			item.conditionNormalized = Properties.fruitVisualScaleCurve.Evaluate(StageProgressFraction);
		}
		if ((Object)(object)player != (Object)null)
		{
			player.GiveItem(item, GiveItemReason.PickedUp);
		}
		else
		{
			item.Drop(((Component)this).get_transform().get_position() + Vector3.get_up() * 0.5f, Vector3.get_up() * 1f);
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	[RPC_Server.IsVisible(3f)]
	public void RPC_PickFruit(RPCMessage msg)
	{
		PickFruit(msg.player);
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void RPC_RemoveDying(RPCMessage msg)
	{
		RemoveDying(msg.player);
	}

	public void RemoveDying(BasePlayer receiver)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		if (State == PlantProperties.State.Dying && !((Object)(object)Properties.removeDyingItem == (Object)null))
		{
			if (Properties.removeDyingEffect.isValid)
			{
				Effect.server.Run(Properties.removeDyingEffect.resourcePath, ((Component)this).get_transform().get_position(), Vector3.get_up());
			}
			Item item = ItemManager.Create(Properties.removeDyingItem, 1, 0uL);
			if ((Object)(object)receiver != (Object)null)
			{
				receiver.GiveItem(item, GiveItemReason.PickedUp);
			}
			else
			{
				item.Drop(((Component)this).get_transform().get_position() + Vector3.get_up() * 0.5f, Vector3.get_up() * 1f);
			}
			Die();
		}
	}

	[ServerVar(ServerAdmin = true)]
	public static void GrowAll(Arg arg)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = arg.Player();
		if (!basePlayer.IsAdmin)
		{
			return;
		}
		List<GrowableEntity> list = Pool.GetList<GrowableEntity>();
		Vis.Entities(basePlayer.ServerPosition, 6f, list, -1, (QueryTriggerInteraction)2);
		foreach (GrowableEntity item in list)
		{
			if (item.isServer)
			{
				item.ChangeState(item.currentStage.nextState, resetAge: false);
			}
		}
		Pool.FreeList<GrowableEntity>(ref list);
	}

	public void ReceiveInstanceData(InstanceData data)
	{
		GrowableGeneEncoding.DecodeIntToGenes(data.dataInt, Genes);
		GrowableGeneEncoding.DecodeIntToPreviousGenes(data.dataInt, Genes);
	}

	public override void ResetState()
	{
		base.ResetState();
		State = PlantProperties.State.Seed;
	}

	public bool CanPick()
	{
		return currentStage.resources > 0f;
	}

	public bool CanTakeSeeds()
	{
		if (currentStage.resources > 0f)
		{
			return (Object)(object)Properties.SeedItem != (Object)null;
		}
		return false;
	}

	public bool CanClone()
	{
		if (currentStage.resources > 0f)
		{
			return (Object)(object)Properties.CloneItem != (Object)null;
		}
		return false;
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.growableEntity = Pool.Get<GrowableEntity>();
		info.msg.growableEntity.state = (int)State;
		info.msg.growableEntity.totalAge = Age;
		info.msg.growableEntity.stageAge = stageAge;
		info.msg.growableEntity.yieldFraction = Yield;
		info.msg.growableEntity.yieldPool = yieldPool;
		info.msg.growableEntity.fertilized = Fertilized;
		if (Genes != null)
		{
			Genes.Save(info);
		}
		if (!info.forDisk)
		{
			info.msg.growableEntity.lightModifier = LightQuality;
			info.msg.growableEntity.groundModifier = GroundQuality;
			info.msg.growableEntity.waterModifier = WaterQuality;
			info.msg.growableEntity.happiness = OverallQuality;
			info.msg.growableEntity.temperatureModifier = TemperatureQuality;
			info.msg.growableEntity.waterConsumption = WaterConsumption;
		}
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.growableEntity != null)
		{
			Age = info.msg.growableEntity.totalAge;
			stageAge = info.msg.growableEntity.stageAge;
			Yield = info.msg.growableEntity.yieldFraction;
			Fertilized = info.msg.growableEntity.fertilized;
			yieldPool = info.msg.growableEntity.yieldPool;
			Genes.Load(info);
			ChangeState((PlantProperties.State)info.msg.growableEntity.state, resetAge: false, loading: true);
		}
		else
		{
			Genes.GenerateRandom(this);
		}
	}

	private void ChangeState(PlantProperties.State state, bool resetAge, bool loading = false)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		if (base.isServer && State == state)
		{
			return;
		}
		State = state;
		if (!base.isServer)
		{
			return;
		}
		if (!loading)
		{
			if (currentStage.resources > 0f)
			{
				yieldPool = currentStage.yield;
			}
			if (state == PlantProperties.State.Crossbreed)
			{
				if (Properties.CrossBreedEffect.isValid)
				{
					Effect.server.Run(Properties.CrossBreedEffect.resourcePath, ((Component)this).get_transform().get_position(), Vector3.get_up());
				}
				GrowableGenetics.CrossBreed(this);
			}
			SendNetworkUpdate();
		}
		if (resetAge)
		{
			stageAge = 0f;
		}
	}
}
