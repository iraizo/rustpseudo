using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public static class GrowableGenetics
{
	public enum GeneType
	{
		Empty,
		WaterRequirement,
		GrowthSpeed,
		Yield,
		Hardiness
	}

	public struct GeneWeighting
	{
		public float Weighting;

		public GeneType GeneType;
	}

	public const int GeneSlotCount = 6;

	public const float CrossBreedingRadius = 1.5f;

	private static GeneWeighting[] neighbourWeights = new GeneWeighting[Enum.GetValues(typeof(GeneType)).Length];

	private static GeneWeighting dominant = default(GeneWeighting);

	public static void CrossBreed(GrowableEntity growable)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		List<GrowableEntity> list = Pool.GetList<GrowableEntity>();
		Vis.Entities(((Component)growable).get_transform().get_position(), 1.5f, list, 512, (QueryTriggerInteraction)2);
		bool flag = false;
		for (int i = 0; i < 6; i++)
		{
			GrowableGene growableGene = growable.Genes.Genes[i];
			GeneWeighting dominantGeneWeighting = GetDominantGeneWeighting(growable, list, i);
			if (dominantGeneWeighting.Weighting > growable.Properties.Genes.Weights[(int)growableGene.Type].CrossBreedingWeight)
			{
				flag = true;
				growableGene.Set(dominantGeneWeighting.GeneType);
			}
		}
		if (flag)
		{
			growable.SendNetworkUpdate();
		}
	}

	private static GeneWeighting GetDominantGeneWeighting(GrowableEntity crossBreedingGrowable, List<GrowableEntity> neighbours, int slot)
	{
		PlanterBox planter = crossBreedingGrowable.GetPlanter();
		if ((Object)(object)planter == (Object)null)
		{
			dominant.Weighting = -1f;
			return dominant;
		}
		for (int i = 0; i < neighbourWeights.Length; i++)
		{
			neighbourWeights[i].Weighting = 0f;
			neighbourWeights[i].GeneType = (GeneType)i;
		}
		dominant.Weighting = 0f;
		foreach (GrowableEntity neighbour in neighbours)
		{
			if (!neighbour.isServer)
			{
				continue;
			}
			PlanterBox planter2 = neighbour.GetPlanter();
			if (!((Object)(object)planter2 == (Object)null) && !((Object)(object)planter2 != (Object)(object)planter) && !((Object)(object)neighbour == (Object)(object)crossBreedingGrowable) && neighbour.prefabID == crossBreedingGrowable.prefabID && !neighbour.IsDead())
			{
				GeneType type = neighbour.Genes.Genes[slot].Type;
				float crossBreedingWeight = neighbour.Properties.Genes.Weights[(int)type].CrossBreedingWeight;
				float num = (neighbourWeights[(int)type].Weighting += crossBreedingWeight);
				if (num > dominant.Weighting)
				{
					dominant.Weighting = num;
					dominant.GeneType = type;
				}
			}
		}
		return dominant;
	}
}
