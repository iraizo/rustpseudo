using System;
using UnityEngine;

[Serializable]
public class HairDye
{
	public enum CopyProperty
	{
		DyeColor,
		RootColor,
		TipColor,
		Brightness,
		DyeRoughness,
		DyeScatter,
		Specular,
		Roughness,
		Count
	}

	[Flags]
	public enum CopyPropertyMask
	{
		DyeColor = 0x1,
		RootColor = 0x2,
		TipColor = 0x4,
		Brightness = 0x8,
		DyeRoughness = 0x10,
		DyeScatter = 0x20,
		Specular = 0x40,
		Roughness = 0x80
	}

	[ColorUsage(false, true)]
	public Color capBaseColor;

	public Material sourceMaterial;

	[InspectorFlags]
	public CopyPropertyMask copyProperties;

	private static MaterialPropertyDesc[] transferableProps = new MaterialPropertyDesc[8]
	{
		new MaterialPropertyDesc("_DyeColor", typeof(Color)),
		new MaterialPropertyDesc("_RootColor", typeof(Color)),
		new MaterialPropertyDesc("_TipColor", typeof(Color)),
		new MaterialPropertyDesc("_Brightness", typeof(float)),
		new MaterialPropertyDesc("_DyeRoughness", typeof(float)),
		new MaterialPropertyDesc("_DyeScatter", typeof(float)),
		new MaterialPropertyDesc("_HairSpecular", typeof(float)),
		new MaterialPropertyDesc("_HairRoughness", typeof(float))
	};

	private static int _HairBaseColorUV1 = Shader.PropertyToID("_HairBaseColorUV1");

	private static int _HairBaseColorUV2 = Shader.PropertyToID("_HairBaseColorUV2");

	private static int _HairPackedMapUV1 = Shader.PropertyToID("_HairPackedMapUV1");

	private static int _HairPackedMapUV2 = Shader.PropertyToID("_HairPackedMapUV2");

	public void Apply(HairDyeCollection collection, MaterialPropertyBlock block)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)sourceMaterial != (Object)null))
		{
			return;
		}
		for (int i = 0; i < 8; i++)
		{
			if (((uint)copyProperties & (uint)(1 << i)) == 0)
			{
				continue;
			}
			MaterialPropertyDesc materialPropertyDesc = transferableProps[i];
			if (sourceMaterial.HasProperty(materialPropertyDesc.nameID))
			{
				if (materialPropertyDesc.type == typeof(Color))
				{
					block.SetColor(materialPropertyDesc.nameID, sourceMaterial.GetColor(materialPropertyDesc.nameID));
				}
				else if (materialPropertyDesc.type == typeof(float))
				{
					block.SetFloat(materialPropertyDesc.nameID, sourceMaterial.GetFloat(materialPropertyDesc.nameID));
				}
			}
		}
	}

	public void ApplyCap(HairDyeCollection collection, HairType type, MaterialPropertyBlock block)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (collection.applyCap)
		{
			switch (type)
			{
			case HairType.Head:
			case HairType.Armpit:
			case HairType.Pubic:
				block.SetColor(_HairBaseColorUV1, ((Color)(ref capBaseColor)).get_gamma());
				block.SetTexture(_HairPackedMapUV1, (Texture)(((Object)(object)collection.capMask != (Object)null) ? ((object)collection.capMask) : ((object)Texture2D.get_blackTexture())));
				break;
			case HairType.Facial:
				block.SetColor(_HairBaseColorUV2, ((Color)(ref capBaseColor)).get_gamma());
				block.SetTexture(_HairPackedMapUV2, (Texture)(((Object)(object)collection.capMask != (Object)null) ? ((object)collection.capMask) : ((object)Texture2D.get_blackTexture())));
				break;
			}
		}
	}
}
