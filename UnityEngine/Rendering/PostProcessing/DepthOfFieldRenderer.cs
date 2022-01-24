using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	[Preserve]
	internal sealed class DepthOfFieldRenderer : PostProcessEffectRenderer<DepthOfField>
	{
		private enum Pass
		{
			CoCCalculation,
			CoCTemporalFilter,
			DownsampleAndPrefilter,
			BokehSmallKernel,
			BokehMediumKernel,
			BokehLargeKernel,
			BokehVeryLargeKernel,
			PostFilter,
			Combine,
			DebugOverlay
		}

		private const int k_NumEyes = 2;

		private const int k_NumCoCHistoryTextures = 2;

		private readonly RenderTexture[][] m_CoCHistoryTextures = new RenderTexture[2][];

		private int[] m_HistoryPingPong = new int[2];

		private const float k_FilmHeight = 0.024f;

		public DepthOfFieldRenderer()
		{
			for (int i = 0; i < 2; i++)
			{
				m_CoCHistoryTextures[i] = (RenderTexture[])(object)new RenderTexture[2];
				m_HistoryPingPong[i] = 0;
			}
		}

		public override DepthTextureMode GetCameraFlags()
		{
			return (DepthTextureMode)1;
		}

		private RenderTextureFormat SelectFormat(RenderTextureFormat primary, RenderTextureFormat secondary)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			if (primary.IsSupported())
			{
				return primary;
			}
			if (secondary.IsSupported())
			{
				return secondary;
			}
			return (RenderTextureFormat)7;
		}

		private float CalculateMaxCoCRadius(int screenHeight)
		{
			float num = (float)base.settings.kernelSize.value * 4f + 6f;
			return Mathf.Min(0.05f, num / (float)screenHeight);
		}

		private RenderTexture CheckHistory(int eye, int id, PostProcessRenderContext context, RenderTextureFormat format)
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			RenderTexture val = m_CoCHistoryTextures[eye][id];
			if (m_ResetHistory || (Object)(object)val == (Object)null || !val.IsCreated() || ((Texture)val).get_width() != context.width || ((Texture)val).get_height() != context.height)
			{
				RenderTexture.ReleaseTemporary(val);
				val = context.GetScreenSpaceTemporaryRT(0, format, (RenderTextureReadWrite)1);
				((Object)val).set_name("CoC History, Eye: " + eye + ", ID: " + id);
				((Texture)val).set_filterMode((FilterMode)1);
				val.Create();
				m_CoCHistoryTextures[eye][id] = val;
			}
			return val;
		}

		public override void Render(PostProcessRenderContext context)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			RenderTextureFormat sourceFormat = context.sourceFormat;
			RenderTextureFormat val = SelectFormat((RenderTextureFormat)16, (RenderTextureFormat)15);
			float num = 0.024f * ((float)context.height / 1080f);
			float num2 = base.settings.focalLength.value / 1000f;
			float num3 = Mathf.Max(base.settings.focusDistance.value, num2);
			float num4 = (float)context.screenWidth / (float)context.screenHeight;
			float num5 = num2 * num2 / (base.settings.aperture.value * (num3 - num2) * num * 2f);
			float num6 = CalculateMaxCoCRadius(context.screenHeight);
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.depthOfField);
			propertySheet.properties.Clear();
			propertySheet.properties.SetFloat(ShaderIDs.Distance, num3);
			propertySheet.properties.SetFloat(ShaderIDs.LensCoeff, num5);
			propertySheet.properties.SetFloat(ShaderIDs.MaxCoC, num6);
			propertySheet.properties.SetFloat(ShaderIDs.RcpMaxCoC, 1f / num6);
			propertySheet.properties.SetFloat(ShaderIDs.RcpAspect, 1f / num4);
			CommandBuffer command = context.command;
			command.BeginSample("DepthOfField");
			context.GetScreenSpaceTemporaryRT(command, ShaderIDs.CoCTex, 0, val, (RenderTextureReadWrite)1, (FilterMode)1);
			command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)0), RenderTargetIdentifier.op_Implicit(ShaderIDs.CoCTex), propertySheet, 0);
			if (context.IsTemporalAntialiasingActive() || context.dlssEnabled)
			{
				float motionBlending = context.temporalAntialiasing.motionBlending;
				float num7 = (m_ResetHistory ? 0f : motionBlending);
				Vector2 jitter = context.temporalAntialiasing.jitter;
				propertySheet.properties.SetVector(ShaderIDs.TaaParams, Vector4.op_Implicit(new Vector3(jitter.x, jitter.y, num7)));
				int num8 = m_HistoryPingPong[context.xrActiveEye];
				RenderTexture val2 = CheckHistory(context.xrActiveEye, ++num8 % 2, context, val);
				RenderTexture val3 = CheckHistory(context.xrActiveEye, ++num8 % 2, context, val);
				m_HistoryPingPong[context.xrActiveEye] = ++num8 % 2;
				command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit((Texture)(object)val2), RenderTargetIdentifier.op_Implicit((Texture)(object)val3), propertySheet, 1);
				command.ReleaseTemporaryRT(ShaderIDs.CoCTex);
				command.SetGlobalTexture(ShaderIDs.CoCTex, RenderTargetIdentifier.op_Implicit((Texture)(object)val3));
			}
			context.GetScreenSpaceTemporaryRT(command, ShaderIDs.DepthOfFieldTex, 0, sourceFormat, (RenderTextureReadWrite)0, (FilterMode)1, context.width / 2, context.height / 2);
			command.BlitFullscreenTriangle(context.source, RenderTargetIdentifier.op_Implicit(ShaderIDs.DepthOfFieldTex), propertySheet, 2);
			context.GetScreenSpaceTemporaryRT(command, ShaderIDs.DepthOfFieldTemp, 0, sourceFormat, (RenderTextureReadWrite)0, (FilterMode)1, context.width / 2, context.height / 2);
			command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit(ShaderIDs.DepthOfFieldTex), RenderTargetIdentifier.op_Implicit(ShaderIDs.DepthOfFieldTemp), propertySheet, (int)(3 + base.settings.kernelSize.value));
			command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit(ShaderIDs.DepthOfFieldTemp), RenderTargetIdentifier.op_Implicit(ShaderIDs.DepthOfFieldTex), propertySheet, 7);
			command.ReleaseTemporaryRT(ShaderIDs.DepthOfFieldTemp);
			if (context.IsDebugOverlayEnabled(DebugOverlay.DepthOfField))
			{
				context.PushDebugOverlay(command, context.source, propertySheet, 9);
			}
			command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 8);
			command.ReleaseTemporaryRT(ShaderIDs.DepthOfFieldTex);
			if (!context.IsTemporalAntialiasingActive() || context.dlssEnabled)
			{
				command.ReleaseTemporaryRT(ShaderIDs.CoCTex);
			}
			command.EndSample("DepthOfField");
			m_ResetHistory = false;
		}

		public override void Release()
		{
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < m_CoCHistoryTextures[i].Length; j++)
				{
					RenderTexture.ReleaseTemporary(m_CoCHistoryTextures[i][j]);
					m_CoCHistoryTextures[i][j] = null;
				}
				m_HistoryPingPong[i] = 0;
			}
			ResetHistory();
		}
	}
}
