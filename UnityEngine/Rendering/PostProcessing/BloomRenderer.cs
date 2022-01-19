using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	[Preserve]
	internal sealed class BloomRenderer : PostProcessEffectRenderer<Bloom>
	{
		private enum Pass
		{
			Prefilter13,
			Prefilter4,
			Downsample13,
			Downsample4,
			UpsampleTent,
			UpsampleBox,
			DebugOverlayThreshold,
			DebugOverlayTent,
			DebugOverlayBox
		}

		private struct Level
		{
			internal int down;

			internal int up;
		}

		private Level[] m_Pyramid;

		private const int k_MaxPyramidSize = 16;

		public override void Init()
		{
			m_Pyramid = new Level[16];
			for (int i = 0; i < 16; i++)
			{
				m_Pyramid[i] = new Level
				{
					down = Shader.PropertyToID("_BloomMipDown" + i),
					up = Shader.PropertyToID("_BloomMipUp" + i)
				};
			}
		}

		public override void Render(PostProcessRenderContext context)
		{
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Invalid comparison between Unknown and I4
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0402: Unknown result type (might be due to invalid IL or missing references)
			//IL_040b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0428: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0541: Unknown result type (might be due to invalid IL or missing references)
			//IL_0554: Unknown result type (might be due to invalid IL or missing references)
			//IL_0567: Unknown result type (might be due to invalid IL or missing references)
			//IL_0589: Unknown result type (might be due to invalid IL or missing references)
			CommandBuffer command = context.command;
			command.BeginSample("BloomPyramid");
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.bloom);
			propertySheet.properties.SetTexture(ShaderIDs.AutoExposureTex, context.autoExposureTexture);
			float num = Mathf.Clamp((float)base.settings.anamorphicRatio, -1f, 1f);
			float num2 = ((num < 0f) ? (0f - num) : 0f);
			float num3 = ((num > 0f) ? num : 0f);
			int num4 = Mathf.FloorToInt((float)context.screenWidth / (2f - num2));
			int num5 = Mathf.FloorToInt((float)context.screenHeight / (2f - num3));
			bool flag = context.stereoActive && context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePass && (int)context.camera.get_stereoTargetEye() == 3;
			int num6 = (flag ? (num4 * 2) : num4);
			float num7 = Mathf.Log((float)Mathf.Max(num4, num5), 2f) + Mathf.Min(base.settings.diffusion.value, 10f) - 10f;
			int num8 = Mathf.FloorToInt(num7);
			int num9 = Mathf.Clamp(num8, 1, 16);
			float num10 = 0.5f + num7 - (float)num8;
			propertySheet.properties.SetFloat(ShaderIDs.SampleScale, num10);
			float num11 = Mathf.GammaToLinearSpace(base.settings.threshold.value);
			float num12 = num11 * base.settings.softKnee.value + 1E-05f;
			Vector4 val = default(Vector4);
			((Vector4)(ref val))._002Ector(num11, num11 - num12, num12 * 2f, 0.25f / num12);
			propertySheet.properties.SetVector(ShaderIDs.Threshold, val);
			float num13 = Mathf.GammaToLinearSpace(base.settings.clamp.value);
			propertySheet.properties.SetVector(ShaderIDs.Params, new Vector4(num13, 0f, 0f, 0f));
			int num14 = (base.settings.fastMode ? 1 : 0);
			RenderTargetIdentifier source = context.source;
			for (int i = 0; i < num9; i++)
			{
				int down = m_Pyramid[i].down;
				int up = m_Pyramid[i].up;
				int pass = ((i == 0) ? num14 : (2 + num14));
				context.GetScreenSpaceTemporaryRT(command, down, 0, context.sourceFormat, (RenderTextureReadWrite)0, (FilterMode)1, num6, num5);
				context.GetScreenSpaceTemporaryRT(command, up, 0, context.sourceFormat, (RenderTextureReadWrite)0, (FilterMode)1, num6, num5);
				command.BlitFullscreenTriangle(source, RenderTargetIdentifier.op_Implicit(down), propertySheet, pass);
				source = RenderTargetIdentifier.op_Implicit(down);
				num6 = ((flag && num6 / 2 % 2 > 0) ? (1 + num6 / 2) : (num6 / 2));
				num6 = Mathf.Max(num6, 1);
				num5 = Mathf.Max(num5 / 2, 1);
			}
			int num15 = m_Pyramid[num9 - 1].down;
			for (int num16 = num9 - 2; num16 >= 0; num16--)
			{
				int down2 = m_Pyramid[num16].down;
				int up2 = m_Pyramid[num16].up;
				command.SetGlobalTexture(ShaderIDs.BloomTex, RenderTargetIdentifier.op_Implicit(down2));
				command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit(num15), RenderTargetIdentifier.op_Implicit(up2), propertySheet, 4 + num14);
				num15 = up2;
			}
			Color linear = ((Color)(ref base.settings.color.value)).get_linear();
			float num17 = RuntimeUtilities.Exp2(base.settings.intensity.value / 10f) - 1f;
			Vector4 val2 = default(Vector4);
			((Vector4)(ref val2))._002Ector(num10, num17, base.settings.dirtIntensity.value, (float)num9);
			if (context.IsDebugOverlayEnabled(DebugOverlay.BloomThreshold))
			{
				context.PushDebugOverlay(command, context.source, propertySheet, 6);
			}
			else if (context.IsDebugOverlayEnabled(DebugOverlay.BloomBuffer))
			{
				propertySheet.properties.SetVector(ShaderIDs.ColorIntensity, new Vector4(linear.r, linear.g, linear.b, num17));
				context.PushDebugOverlay(command, RenderTargetIdentifier.op_Implicit(m_Pyramid[0].up), propertySheet, 7 + num14);
			}
			Texture val3 = (Texture)(((Object)(object)base.settings.dirtTexture.value == (Object)null) ? ((object)RuntimeUtilities.blackTexture) : ((object)base.settings.dirtTexture.value));
			float num18 = (float)val3.get_width() / (float)val3.get_height();
			float num19 = (float)context.screenWidth / (float)context.screenHeight;
			Vector4 val4 = default(Vector4);
			((Vector4)(ref val4))._002Ector(1f, 1f, 0f, 0f);
			if (num18 > num19)
			{
				val4.x = num19 / num18;
				val4.z = (1f - val4.x) * 0.5f;
			}
			else if (num19 > num18)
			{
				val4.y = num18 / num19;
				val4.w = (1f - val4.y) * 0.5f;
			}
			PropertySheet uberSheet = context.uberSheet;
			if ((bool)base.settings.fastMode)
			{
				uberSheet.EnableKeyword("BLOOM_LOW");
			}
			else
			{
				uberSheet.EnableKeyword("BLOOM");
			}
			uberSheet.properties.SetVector(ShaderIDs.Bloom_DirtTileOffset, val4);
			uberSheet.properties.SetVector(ShaderIDs.Bloom_Settings, val2);
			uberSheet.properties.SetColor(ShaderIDs.Bloom_Color, linear);
			uberSheet.properties.SetTexture(ShaderIDs.Bloom_DirtTex, val3);
			command.SetGlobalTexture(ShaderIDs.BloomTex, RenderTargetIdentifier.op_Implicit(num15));
			for (int j = 0; j < num9; j++)
			{
				if (m_Pyramid[j].down != num15)
				{
					command.ReleaseTemporaryRT(m_Pyramid[j].down);
				}
				if (m_Pyramid[j].up != num15)
				{
					command.ReleaseTemporaryRT(m_Pyramid[j].up);
				}
			}
			command.EndSample("BloomPyramid");
			context.bloomBufferNameID = num15;
		}
	}
}
