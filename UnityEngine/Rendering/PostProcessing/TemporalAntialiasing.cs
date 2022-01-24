using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	[Serializable]
	[Preserve]
	public sealed class TemporalAntialiasing
	{
		private enum Pass
		{
			SolverDilate,
			SolverNoDilate
		}

		[Tooltip("The diameter (in texels) inside which jitter samples are spread. Smaller values result in crisper but more aliased output, while larger values result in more stable, but blurrier, output.")]
		[Range(0.1f, 1f)]
		public float jitterSpread = 0.75f;

		[Tooltip("Controls the amount of sharpening applied to the color buffer. High values may introduce dark-border artifacts.")]
		[Range(0f, 3f)]
		public float sharpness = 0.25f;

		[Tooltip("The blend coefficient for a stationary fragment. Controls the percentage of history sample blended into the final color.")]
		[Range(0f, 0.99f)]
		public float stationaryBlending = 0.95f;

		[Tooltip("The blend coefficient for a fragment with significant motion. Controls the percentage of history sample blended into the final color.")]
		[Range(0f, 0.99f)]
		public float motionBlending = 0.85f;

		public Func<Camera, Vector2, Matrix4x4> jitteredMatrixFunc;

		private readonly RenderTargetIdentifier[] m_Mrt = (RenderTargetIdentifier[])(object)new RenderTargetIdentifier[2];

		private bool m_ResetHistory = true;

		private const int k_NumEyes = 2;

		private const int k_NumHistoryTextures = 2;

		private readonly RenderTexture[][] m_HistoryTextures = new RenderTexture[2][];

		private readonly int[] m_HistoryPingPong = new int[2];

		public Vector2 jitter { get; private set; }

		public Vector2 jitterRaw { get; private set; }

		public int sampleIndex { get; private set; }

		public int sampleCount { get; set; }

		public bool IsSupported()
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Invalid comparison between Unknown and I4
			if (SystemInfo.get_supportedRenderTargetCount() >= 2 && SystemInfo.get_supportsMotionVectors())
			{
				return (int)SystemInfo.get_graphicsDeviceType() != 8;
			}
			return false;
		}

		internal DepthTextureMode GetCameraFlags()
		{
			return (DepthTextureMode)5;
		}

		internal void ResetHistory()
		{
			m_ResetHistory = true;
		}

		private Vector2 GenerateRandomOffset()
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			Vector2 result = new Vector2(HaltonSeq.Get((sampleIndex & 0x3FF) + 1, 2) - 0.5f, HaltonSeq.Get((sampleIndex & 0x3FF) + 1, 3) - 0.5f);
			if (++sampleIndex >= sampleCount)
			{
				sampleIndex = 0;
			}
			return result;
		}

		public Matrix4x4 GetJitteredProjectionMatrix(Camera camera)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			jitter = GenerateRandomOffset();
			jitter *= jitterSpread;
			Matrix4x4 result = ((jitteredMatrixFunc == null) ? (camera.get_orthographic() ? RuntimeUtilities.GetJitteredOrthographicProjectionMatrix(camera, jitter) : RuntimeUtilities.GetJitteredPerspectiveProjectionMatrix(camera, jitter)) : jitteredMatrixFunc(camera, jitter));
			jitterRaw = jitter;
			jitter = new Vector2(jitter.x / (float)camera.get_pixelWidth(), jitter.y / (float)camera.get_pixelHeight());
			return result;
		}

		public void ConfigureJitteredProjectionMatrix(PostProcessRenderContext context)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			Camera camera = context.camera;
			camera.set_nonJitteredProjectionMatrix(camera.get_projectionMatrix());
			camera.set_projectionMatrix(GetJitteredProjectionMatrix(camera));
			camera.set_useJitteredProjectionMatrixForTransparentRendering(true);
		}

		public void ConfigureStereoJitteredProjectionMatrices(PostProcessRenderContext context)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Invalid comparison between Unknown and I4
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			Camera camera = context.camera;
			jitter = GenerateRandomOffset();
			jitter *= jitterSpread;
			for (StereoscopicEye val = (StereoscopicEye)0; (int)val <= 1; val = (StereoscopicEye)(val + 1))
			{
				context.camera.CopyStereoDeviceProjectionMatrixToNonJittered(val);
				Matrix4x4 stereoNonJitteredProjectionMatrix = context.camera.GetStereoNonJitteredProjectionMatrix(val);
				Matrix4x4 val2 = RuntimeUtilities.GenerateJitteredProjectionMatrixFromOriginal(context, stereoNonJitteredProjectionMatrix, jitter);
				context.camera.SetStereoProjectionMatrix(val, val2);
			}
			jitter = new Vector2(jitter.x / (float)context.screenWidth, jitter.y / (float)context.screenHeight);
			camera.set_useJitteredProjectionMatrixForTransparentRendering(true);
		}

		private void GenerateHistoryName(RenderTexture rt, int id, PostProcessRenderContext context)
		{
			((Object)rt).set_name("Temporal Anti-aliasing History id #" + id);
			if (context.stereoActive)
			{
				((Object)rt).set_name(((Object)rt).get_name() + " for eye " + context.xrActiveEye);
			}
		}

		private RenderTexture CheckHistory(int id, PostProcessRenderContext context)
		{
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			int xrActiveEye = context.xrActiveEye;
			if (m_HistoryTextures[xrActiveEye] == null)
			{
				m_HistoryTextures[xrActiveEye] = (RenderTexture[])(object)new RenderTexture[2];
			}
			RenderTexture val = m_HistoryTextures[xrActiveEye][id];
			if (m_ResetHistory || (Object)(object)val == (Object)null || !val.IsCreated())
			{
				RenderTexture.ReleaseTemporary(val);
				val = context.GetScreenSpaceTemporaryRT(0, context.sourceFormat, (RenderTextureReadWrite)0);
				GenerateHistoryName(val, id, context);
				((Texture)val).set_filterMode((FilterMode)1);
				m_HistoryTextures[xrActiveEye][id] = val;
				context.command.BlitFullscreenTriangle(context.source, RenderTargetIdentifier.op_Implicit((Texture)(object)val));
			}
			else if (((Texture)val).get_width() != context.width || ((Texture)val).get_height() != context.height)
			{
				RenderTexture screenSpaceTemporaryRT = context.GetScreenSpaceTemporaryRT(0, context.sourceFormat, (RenderTextureReadWrite)0);
				GenerateHistoryName(screenSpaceTemporaryRT, id, context);
				((Texture)screenSpaceTemporaryRT).set_filterMode((FilterMode)1);
				m_HistoryTextures[xrActiveEye][id] = screenSpaceTemporaryRT;
				context.command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit((Texture)(object)val), RenderTargetIdentifier.op_Implicit((Texture)(object)screenSpaceTemporaryRT));
				RenderTexture.ReleaseTemporary(val);
			}
			return m_HistoryTextures[xrActiveEye][id];
		}

		internal void Render(PostProcessRenderContext context)
		{
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.temporalAntialiasing);
			CommandBuffer command = context.command;
			command.BeginSample("TemporalAntialiasing");
			int num = m_HistoryPingPong[context.xrActiveEye];
			RenderTexture val = CheckHistory(++num % 2, context);
			RenderTexture val2 = CheckHistory(++num % 2, context);
			m_HistoryPingPong[context.xrActiveEye] = ++num % 2;
			propertySheet.properties.SetVector(ShaderIDs.Jitter, Vector4.op_Implicit(jitter));
			propertySheet.properties.SetFloat(ShaderIDs.Sharpness, sharpness);
			propertySheet.properties.SetVector(ShaderIDs.FinalBlendParameters, new Vector4(stationaryBlending, motionBlending, 6000f, 0f));
			propertySheet.properties.SetTexture(ShaderIDs.HistoryTex, (Texture)(object)val);
			int pass = (context.camera.get_orthographic() ? 1 : 0);
			m_Mrt[0] = context.destination;
			m_Mrt[1] = RenderTargetIdentifier.op_Implicit((Texture)(object)val2);
			command.BlitFullscreenTriangle(context.source, m_Mrt, context.source, propertySheet, pass);
			command.EndSample("TemporalAntialiasing");
			m_ResetHistory = false;
		}

		internal void Release()
		{
			if (m_HistoryTextures != null)
			{
				for (int i = 0; i < m_HistoryTextures.Length; i++)
				{
					if (m_HistoryTextures[i] != null)
					{
						for (int j = 0; j < m_HistoryTextures[i].Length; j++)
						{
							RenderTexture.ReleaseTemporary(m_HistoryTextures[i][j]);
							m_HistoryTextures[i][j] = null;
						}
						m_HistoryTextures[i] = null;
					}
				}
			}
			sampleIndex = 0;
			m_HistoryPingPong[0] = 0;
			m_HistoryPingPong[1] = 0;
			ResetHistory();
		}
	}
}
