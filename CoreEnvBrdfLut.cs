using System;
using UnityEngine;

public class CoreEnvBrdfLut
{
	private static Texture2D runtimeEnvBrdfLut;

	[RuntimeInitializeOnLoadMethod(/*Could not decode attribute arguments.*/)]
	private static void OnRuntimeLoad()
	{
		PrepareTextureForRuntime();
		UpdateReflProbe();
	}

	private static void PrepareTextureForRuntime()
	{
		if ((Object)(object)runtimeEnvBrdfLut == (Object)null)
		{
			runtimeEnvBrdfLut = Generate();
		}
		Shader.SetGlobalTexture("_EnvBrdfLut", (Texture)(object)runtimeEnvBrdfLut);
	}

	private static void UpdateReflProbe()
	{
		int num = (int)Mathf.Log((float)RenderSettings.get_defaultReflectionResolution(), 2f) - 1;
		if (Shader.GetGlobalFloat("_ReflProbeMaxMip") != (float)num)
		{
			Shader.SetGlobalFloat("_ReflProbeMaxMip", (float)num);
		}
	}

	public static Texture2D Generate(bool asset = false)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Expected O, but got Unknown
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		TextureFormat val = (TextureFormat)(asset ? 17 : 16);
		val = (TextureFormat)(SystemInfo.SupportsTextureFormat(val) ? ((int)val) : 5);
		int num = 128;
		int num2 = 32;
		float num3 = 1f / (float)num;
		float num4 = 1f / (float)num2;
		Texture2D val2 = new Texture2D(num, num2, val, false, true);
		((Object)val2).set_name("_EnvBrdfLut");
		((Texture)val2).set_wrapMode((TextureWrapMode)1);
		((Texture)val2).set_filterMode((FilterMode)1);
		Color[] array = (Color[])(object)new Color[num * num2];
		float num5 = 0.0078125f;
		Vector3 val3 = default(Vector3);
		Vector3 val4 = default(Vector3);
		for (int i = 0; i < num2; i++)
		{
			float num6 = ((float)i + 0.5f) * num4;
			float num7 = num6 * num6;
			float num8 = num7 * num7;
			int j = 0;
			int num9 = i * num;
			for (; j < num; j++)
			{
				float num10 = ((float)j + 0.5f) * num3;
				((Vector3)(ref val3))._002Ector(Mathf.Sqrt(1f - num10 * num10), 0f, num10);
				float num11 = 0f;
				float num12 = 0f;
				for (uint num13 = 0u; num13 < 128; num13++)
				{
					float num14 = (float)num13 * num5;
					float num15 = (float)((double)ReverseBits(num13) / 4294967296.0);
					float num16 = (float)Math.PI * 2f * num14;
					float num17 = Mathf.Sqrt((1f - num15) / (1f + (num8 - 1f) * num15));
					float num18 = Mathf.Sqrt(1f - num17 * num17);
					((Vector3)(ref val4))._002Ector(num18 * Mathf.Cos(num16), num18 * Mathf.Sin(num16), num17);
					float num19 = Mathf.Max((2f * Vector3.Dot(val3, val4) * val4 - val3).z, 0f);
					float num20 = Mathf.Max(val4.z, 0f);
					float num21 = Mathf.Max(Vector3.Dot(val3, val4), 0f);
					if (num19 > 0f)
					{
						float num22 = num19 * (num10 * (1f - num7) + num7);
						float num23 = num10 * (num19 * (1f - num7) + num7);
						float num24 = 0.5f / (num22 + num23);
						float num25 = num19 * num24 * (4f * num21 / num20);
						float num26 = 1f - num21;
						num26 *= num26 * num26 * (num26 * num26);
						num11 += num25 * (1f - num26);
						num12 += num25 * num26;
					}
				}
				num11 = Mathf.Clamp(num11 * num5, 0f, 1f);
				num12 = Mathf.Clamp(num12 * num5, 0f, 1f);
				array[num9++] = new Color(num11, num12, 0f, 0f);
			}
		}
		val2.SetPixels(array);
		val2.Apply(false, !asset);
		return val2;
	}

	private static uint ReverseBits(uint Bits)
	{
		Bits = (Bits << 16) | (Bits >> 16);
		Bits = ((Bits & 0xFF00FF) << 8) | ((Bits & 0xFF00FF00u) >> 8);
		Bits = ((Bits & 0xF0F0F0F) << 4) | ((Bits & 0xF0F0F0F0u) >> 4);
		Bits = ((Bits & 0x33333333) << 2) | ((Bits & 0xCCCCCCCCu) >> 2);
		Bits = ((Bits & 0x55555555) << 1) | ((Bits & 0xAAAAAAAAu) >> 1);
		return Bits;
	}
}
