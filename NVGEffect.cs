using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/NVG Effect")]
public class NVGEffect : PostEffectsBase, IImageEffect
{
	[Serializable]
	public struct ColorCorrectionParams
	{
		public float saturation;

		public AnimationCurve redChannel;

		public AnimationCurve greenChannel;

		public AnimationCurve blueChannel;
	}

	[Serializable]
	public struct NoiseAndGrainParams
	{
		public float intensityMultiplier;

		public float generalIntensity;

		public float blackIntensity;

		public float whiteIntensity;

		public float midGrey;

		public bool monochrome;

		public Vector3 intensities;

		public Vector3 tiling;

		public float monochromeTiling;

		public FilterMode filterMode;
	}

	public ColorCorrectionParams ColorCorrection1;

	public ColorCorrectionParams ColorCorrection2;

	public NoiseAndGrainParams NoiseAndGrain;

	private Texture2D rgbChannelTex1;

	private Texture2D rgbChannelTex2;

	private bool updateTexturesOnStartup;

	public Texture2D NoiseTexture;

	private static float NOISE_TILE_AMOUNT = 64f;

	public Shader Shader;

	private Material material;

	private void Awake()
	{
		updateTexturesOnStartup = true;
	}

	private void OnDestroy()
	{
		if ((Object)(object)rgbChannelTex1 != (Object)null)
		{
			Object.DestroyImmediate((Object)(object)rgbChannelTex1);
			rgbChannelTex1 = null;
		}
		if ((Object)(object)rgbChannelTex2 != (Object)null)
		{
			Object.DestroyImmediate((Object)(object)rgbChannelTex2);
			rgbChannelTex2 = null;
		}
		if ((Object)(object)material != (Object)null)
		{
			Object.DestroyImmediate((Object)(object)material);
			material = null;
		}
	}

	private void UpdateColorCorrectionTexture(ColorCorrectionParams param, ref Texture2D tex)
	{
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		if (param.redChannel != null && param.greenChannel != null && param.blueChannel != null)
		{
			for (float num = 0f; num <= 1f; num += 0.003921569f)
			{
				float num2 = Mathf.Clamp(param.redChannel.Evaluate(num), 0f, 1f);
				float num3 = Mathf.Clamp(param.greenChannel.Evaluate(num), 0f, 1f);
				float num4 = Mathf.Clamp(param.blueChannel.Evaluate(num), 0f, 1f);
				tex.SetPixel((int)Mathf.Floor(num * 255f), 0, new Color(num2, num2, num2));
				tex.SetPixel((int)Mathf.Floor(num * 255f), 1, new Color(num3, num3, num3));
				tex.SetPixel((int)Mathf.Floor(num * 255f), 2, new Color(num4, num4, num4));
			}
			tex.Apply();
		}
	}

	public void UpdateTextures()
	{
		((PostEffectsBase)this).CheckResources();
		UpdateColorCorrectionTexture(ColorCorrection1, ref rgbChannelTex1);
		UpdateColorCorrectionTexture(ColorCorrection2, ref rgbChannelTex2);
	}

	public override bool CheckResources()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
		((PostEffectsBase)this).CheckSupport(false);
		material = ((PostEffectsBase)this).CheckShaderAndCreateMaterial(Shader, material);
		if ((Object)(object)rgbChannelTex1 == (Object)null || (Object)(object)rgbChannelTex2 == (Object)null)
		{
			Texture2D val = new Texture2D(256, 4, (TextureFormat)5, false, true);
			((Object)val).set_hideFlags((HideFlags)52);
			((Texture)val).set_wrapMode((TextureWrapMode)1);
			rgbChannelTex1 = val;
			Texture2D val2 = new Texture2D(256, 4, (TextureFormat)5, false, true);
			((Object)val2).set_hideFlags((HideFlags)52);
			((Texture)val2).set_wrapMode((TextureWrapMode)1);
			rgbChannelTex2 = val2;
		}
		if (!base.isSupported)
		{
			((PostEffectsBase)this).ReportAutoDisable();
		}
		return base.isSupported;
	}

	public bool IsActive()
	{
		if (((Behaviour)this).get_enabled() && ((PostEffectsBase)this).CheckResources())
		{
			return (Object)(object)NoiseTexture != (Object)null;
		}
		return false;
	}

	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		if (!((PostEffectsBase)this).CheckResources())
		{
			Graphics.Blit((Texture)(object)source, destination);
			if ((Object)(object)NoiseTexture == (Object)null)
			{
				Debug.LogWarning((object)"[NVGEffect] Noise & Grain effect failing as noise texture is not assigned. please assign.", (Object)(object)((Component)this).get_transform());
			}
			return;
		}
		if (updateTexturesOnStartup)
		{
			UpdateTextures();
			updateTexturesOnStartup = false;
		}
		material.SetTexture("_MainTex", (Texture)(object)source);
		material.SetTexture("_RgbTex1", (Texture)(object)rgbChannelTex1);
		material.SetFloat("_Saturation1", ColorCorrection1.saturation);
		material.SetTexture("_RgbTex2", (Texture)(object)rgbChannelTex2);
		material.SetFloat("_Saturation2", ColorCorrection2.saturation);
		material.SetTexture("_NoiseTex", (Texture)(object)NoiseTexture);
		material.SetVector("_NoisePerChannel", Vector4.op_Implicit(NoiseAndGrain.monochrome ? Vector3.get_one() : NoiseAndGrain.intensities));
		material.SetVector("_NoiseTilingPerChannel", Vector4.op_Implicit(NoiseAndGrain.monochrome ? (Vector3.get_one() * NoiseAndGrain.monochromeTiling) : NoiseAndGrain.tiling));
		material.SetVector("_MidGrey", Vector4.op_Implicit(new Vector3(NoiseAndGrain.midGrey, 1f / (1f - NoiseAndGrain.midGrey), -1f / NoiseAndGrain.midGrey)));
		material.SetVector("_NoiseAmount", Vector4.op_Implicit(new Vector3(NoiseAndGrain.generalIntensity, NoiseAndGrain.blackIntensity, NoiseAndGrain.whiteIntensity) * NoiseAndGrain.intensityMultiplier));
		if (Object.op_Implicit((Object)(object)NoiseTexture))
		{
			((Texture)NoiseTexture).set_wrapMode((TextureWrapMode)0);
			((Texture)NoiseTexture).set_filterMode(NoiseAndGrain.filterMode);
		}
		RenderTexture.set_active(destination);
		float num = (float)((Texture)NoiseTexture).get_width() * 1f;
		float num2 = 1f * (float)((Texture)source).get_width() / NOISE_TILE_AMOUNT;
		GL.PushMatrix();
		GL.LoadOrtho();
		float num3 = 1f * (float)((Texture)source).get_width() / (1f * (float)((Texture)source).get_height());
		float num4 = 1f / num2;
		float num5 = num4 * num3;
		float num6 = num / ((float)((Texture)NoiseTexture).get_width() * 1f);
		material.SetPass(0);
		GL.Begin(7);
		for (float num7 = 0f; num7 < 1f; num7 += num4)
		{
			for (float num8 = 0f; num8 < 1f; num8 += num5)
			{
				float num9 = Random.Range(0f, 1f);
				float num10 = Random.Range(0f, 1f);
				num9 = Mathf.Floor(num9 * num) / num;
				num10 = Mathf.Floor(num10 * num) / num;
				float num11 = 1f / num;
				GL.MultiTexCoord2(0, num9, num10);
				GL.MultiTexCoord2(1, 0f, 0f);
				GL.Vertex3(num7, num8, 0.1f);
				GL.MultiTexCoord2(0, num9 + num6 * num11, num10);
				GL.MultiTexCoord2(1, 1f, 0f);
				GL.Vertex3(num7 + num4, num8, 0.1f);
				GL.MultiTexCoord2(0, num9 + num6 * num11, num10 + num6 * num11);
				GL.MultiTexCoord2(1, 1f, 1f);
				GL.Vertex3(num7 + num4, num8 + num5, 0.1f);
				GL.MultiTexCoord2(0, num9, num10 + num6 * num11);
				GL.MultiTexCoord2(1, 0f, 1f);
				GL.Vertex3(num7, num8 + num5, 0.1f);
			}
		}
		GL.End();
		GL.PopMatrix();
	}

	public NVGEffect()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected O, but got Unknown
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Expected O, but got Unknown
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Expected O, but got Unknown
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Expected O, but got Unknown
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Expected O, but got Unknown
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		ColorCorrectionParams colorCorrectionParams = new ColorCorrectionParams
		{
			saturation = 1f,
			redChannel = new AnimationCurve((Keyframe[])(object)new Keyframe[2]
			{
				new Keyframe(0f, 0f),
				new Keyframe(1f, 1f)
			}),
			greenChannel = new AnimationCurve((Keyframe[])(object)new Keyframe[2]
			{
				new Keyframe(0f, 0f),
				new Keyframe(1f, 1f)
			}),
			blueChannel = new AnimationCurve((Keyframe[])(object)new Keyframe[2]
			{
				new Keyframe(0f, 0f),
				new Keyframe(1f, 1f)
			})
		};
		ColorCorrection1 = colorCorrectionParams;
		colorCorrectionParams = new ColorCorrectionParams
		{
			saturation = 1f,
			redChannel = new AnimationCurve((Keyframe[])(object)new Keyframe[2]
			{
				new Keyframe(0f, 0f),
				new Keyframe(1f, 1f)
			}),
			greenChannel = new AnimationCurve((Keyframe[])(object)new Keyframe[2]
			{
				new Keyframe(0f, 0f),
				new Keyframe(1f, 1f)
			}),
			blueChannel = new AnimationCurve((Keyframe[])(object)new Keyframe[2]
			{
				new Keyframe(0f, 0f),
				new Keyframe(1f, 1f)
			})
		};
		ColorCorrection2 = colorCorrectionParams;
		NoiseAndGrain = new NoiseAndGrainParams
		{
			intensityMultiplier = 1.5f,
			generalIntensity = 1f,
			blackIntensity = 1f,
			whiteIntensity = 1f,
			midGrey = 0.182f,
			monochrome = true,
			intensities = new Vector3(1f, 1f, 1f),
			tiling = new Vector3(60f, 70f, 80f),
			monochromeTiling = 55f,
			filterMode = (FilterMode)0
		};
		updateTexturesOnStartup = true;
		((PostEffectsBase)this)._002Ector();
	}
}
