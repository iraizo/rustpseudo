using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Material Config")]
public class MaterialConfig : ScriptableObject
{
	public class ShaderParameters<T>
	{
		public string Name;

		public T Arid;

		public T Temperate;

		public T Tundra;

		public T Arctic;

		private T[] climates;

		public float FindBlendParameters(Vector3 pos, out T src, out T dst)
		{
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)TerrainMeta.BiomeMap == (Object)null)
			{
				src = Temperate;
				dst = Tundra;
				return 0f;
			}
			if (climates == null || climates.Length == 0)
			{
				climates = new T[4] { Arid, Temperate, Tundra, Arctic };
			}
			int biomeMaxType = TerrainMeta.BiomeMap.GetBiomeMaxType(pos);
			int biomeMaxType2 = TerrainMeta.BiomeMap.GetBiomeMaxType(pos, ~biomeMaxType);
			src = climates[TerrainBiome.TypeToIndex(biomeMaxType)];
			dst = climates[TerrainBiome.TypeToIndex(biomeMaxType2)];
			return TerrainMeta.BiomeMap.GetBiome(pos, biomeMaxType2);
		}

		public T FindBlendParameters(Vector3 pos)
		{
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)TerrainMeta.BiomeMap == (Object)null)
			{
				return Temperate;
			}
			if (climates == null || climates.Length == 0)
			{
				climates = new T[4] { Arid, Temperate, Tundra, Arctic };
			}
			int biomeMaxType = TerrainMeta.BiomeMap.GetBiomeMaxType(pos);
			return climates[TerrainBiome.TypeToIndex(biomeMaxType)];
		}
	}

	[Serializable]
	public class ShaderParametersFloat : ShaderParameters<float>
	{
	}

	[Serializable]
	public class ShaderParametersColor : ShaderParameters<Color>
	{
	}

	[Serializable]
	public class ShaderParametersTexture : ShaderParameters<Texture>
	{
	}

	[Horizontal(4, 0)]
	public ShaderParametersFloat[] Floats;

	[Horizontal(4, 0)]
	public ShaderParametersColor[] Colors;

	[Horizontal(4, 0)]
	public ShaderParametersTexture[] Textures;

	public string[] ScaleUV;

	private MaterialPropertyBlock properties;

	public MaterialPropertyBlock GetMaterialPropertyBlock(Material mat, Vector3 pos, Vector3 scale)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		if (properties == null)
		{
			properties = new MaterialPropertyBlock();
		}
		properties.Clear();
		for (int i = 0; i < Floats.Length; i++)
		{
			ShaderParametersFloat shaderParametersFloat = Floats[i];
			float src;
			float dst;
			float num = shaderParametersFloat.FindBlendParameters(pos, out src, out dst);
			properties.SetFloat(shaderParametersFloat.Name, Mathf.Lerp(src, dst, num));
		}
		for (int j = 0; j < Colors.Length; j++)
		{
			ShaderParametersColor shaderParametersColor = Colors[j];
			Color src2;
			Color dst2;
			float num2 = shaderParametersColor.FindBlendParameters(pos, out src2, out dst2);
			properties.SetColor(shaderParametersColor.Name, Color.Lerp(src2, dst2, num2));
		}
		for (int k = 0; k < Textures.Length; k++)
		{
			ShaderParametersTexture shaderParametersTexture = Textures[k];
			Texture val = shaderParametersTexture.FindBlendParameters(pos);
			if (Object.op_Implicit((Object)(object)val))
			{
				properties.SetTexture(shaderParametersTexture.Name, val);
			}
		}
		for (int l = 0; l < ScaleUV.Length; l++)
		{
			Vector4 vector = mat.GetVector(ScaleUV[l]);
			((Vector4)(ref vector))._002Ector(vector.x * scale.y, vector.y * scale.y, vector.z, vector.w);
			properties.SetVector(ScaleUV[l], vector);
		}
		return properties;
	}

	public MaterialConfig()
		: this()
	{
	}
}
