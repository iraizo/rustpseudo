using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	[ExecuteAlways]
	[DisallowMultipleComponent]
	[ImageEffectAllowedInSceneView]
	[AddComponentMenu("Rendering/Post-process Layer", 1000)]
	[RequireComponent(typeof(Camera))]
	public class PostProcessLayer : MonoBehaviour
	{
		private enum ScalingMode
		{
			NATIVE,
			BILINEAR,
			DLSS
		}

		public enum Antialiasing
		{
			None,
			FastApproximateAntialiasing,
			SubpixelMorphologicalAntialiasing,
			TemporalAntialiasing
		}

		[Serializable]
		public sealed class SerializedBundleRef
		{
			public string assemblyQualifiedName;

			public PostProcessBundle bundle;
		}

		public Transform volumeTrigger;

		public LayerMask volumeLayer;

		public bool stopNaNPropagation = true;

		public bool finalBlitToCameraTarget;

		public Antialiasing antialiasingMode;

		public TemporalAntialiasing temporalAntialiasing;

		public SubpixelMorphologicalAntialiasing subpixelMorphologicalAntialiasing;

		public FastApproximateAntialiasing fastApproximateAntialiasing;

		public Fog fog;

		private Dithering dithering;

		public PostProcessDebugLayer debugLayer;

		public RenderTextureFormat intermediateFormat = (RenderTextureFormat)9;

		private RenderTextureFormat prevIntermediateFormat = (RenderTextureFormat)9;

		private bool supportsIntermediateFormat = true;

		[SerializeField]
		private PostProcessResources m_Resources;

		[Preserve]
		[SerializeField]
		private bool m_ShowToolkit;

		[Preserve]
		[SerializeField]
		private bool m_ShowCustomSorter;

		public bool breakBeforeColorGrading;

		[SerializeField]
		private List<SerializedBundleRef> m_BeforeTransparentBundles;

		[SerializeField]
		private List<SerializedBundleRef> m_BeforeStackBundles;

		[SerializeField]
		private List<SerializedBundleRef> m_AfterStackBundles;

		private Dictionary<Type, PostProcessBundle> m_Bundles;

		private PropertySheetFactory m_PropertySheetFactory;

		private CommandBuffer m_LegacyCmdBufferBeforeReflections;

		private CommandBuffer m_LegacyCmdBufferBeforeLighting;

		private CommandBuffer m_LegacyCmdBufferOpaque;

		private CommandBuffer m_LegacyCmdBuffer;

		private Camera m_Camera;

		private PostProcessRenderContext m_CurrentContext;

		private LogHistogram m_LogHistogram;

		private bool m_SettingsUpdateNeeded = true;

		private bool m_IsRenderingInSceneView;

		private TargetPool m_TargetPool;

		private bool m_NaNKilled;

		private readonly List<PostProcessEffectRenderer> m_ActiveEffects = new List<PostProcessEffectRenderer>();

		private readonly List<RenderTargetIdentifier> m_Targets = new List<RenderTargetIdentifier>();

		public Dictionary<PostProcessEvent, List<SerializedBundleRef>> sortedBundles { get; private set; }

		public bool haveBundlesBeenInited { get; private set; }

		private void OnEnable()
		{
			Init(null);
			if (!haveBundlesBeenInited)
			{
				InitBundles();
			}
			m_LogHistogram = new LogHistogram();
			m_PropertySheetFactory = new PropertySheetFactory();
			m_TargetPool = new TargetPool();
			debugLayer.OnEnable();
			if (!RuntimeUtilities.scriptableRenderPipelineActive)
			{
				InitLegacy();
			}
		}

		private void InitLegacy()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Expected O, but got Unknown
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Expected O, but got Unknown
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Expected O, but got Unknown
			CommandBuffer val = new CommandBuffer();
			val.set_name("Deferred Ambient Occlusion");
			m_LegacyCmdBufferBeforeReflections = val;
			CommandBuffer val2 = new CommandBuffer();
			val2.set_name("Deferred Ambient Occlusion");
			m_LegacyCmdBufferBeforeLighting = val2;
			CommandBuffer val3 = new CommandBuffer();
			val3.set_name("Opaque Only Post-processing");
			m_LegacyCmdBufferOpaque = val3;
			CommandBuffer val4 = new CommandBuffer();
			val4.set_name("Post-processing");
			m_LegacyCmdBuffer = val4;
			m_Camera = ((Component)this).GetComponent<Camera>();
			m_Camera.AddCommandBuffer((CameraEvent)21, m_LegacyCmdBufferBeforeReflections);
			m_Camera.AddCommandBuffer((CameraEvent)6, m_LegacyCmdBufferBeforeLighting);
			m_Camera.AddCommandBuffer((CameraEvent)12, m_LegacyCmdBufferOpaque);
			m_Camera.AddCommandBuffer((CameraEvent)18, m_LegacyCmdBuffer);
			m_CurrentContext = new PostProcessRenderContext();
		}

		[ImageEffectUsesCommandBuffer]
		private void OnRenderImage(RenderTexture src, RenderTexture dst)
		{
			if (finalBlitToCameraTarget)
			{
				RenderTexture.set_active(dst);
			}
			else
			{
				Graphics.Blit((Texture)(object)src, dst);
			}
		}

		public void Init(PostProcessResources resources)
		{
			if ((Object)(object)resources != (Object)null)
			{
				m_Resources = resources;
			}
			RuntimeUtilities.CreateIfNull(ref temporalAntialiasing);
			RuntimeUtilities.CreateIfNull(ref subpixelMorphologicalAntialiasing);
			RuntimeUtilities.CreateIfNull(ref fastApproximateAntialiasing);
			RuntimeUtilities.CreateIfNull(ref dithering);
			RuntimeUtilities.CreateIfNull(ref fog);
			RuntimeUtilities.CreateIfNull(ref debugLayer);
		}

		public void InitBundles()
		{
			if (haveBundlesBeenInited)
			{
				return;
			}
			RuntimeUtilities.CreateIfNull(ref m_BeforeTransparentBundles);
			RuntimeUtilities.CreateIfNull(ref m_BeforeStackBundles);
			RuntimeUtilities.CreateIfNull(ref m_AfterStackBundles);
			m_Bundles = new Dictionary<Type, PostProcessBundle>();
			foreach (Type key in PostProcessManager.instance.settingsTypes.Keys)
			{
				PostProcessBundle value = new PostProcessBundle((PostProcessEffectSettings)(object)ScriptableObject.CreateInstance(key));
				m_Bundles.Add(key, value);
			}
			UpdateBundleSortList(m_BeforeTransparentBundles, PostProcessEvent.BeforeTransparent);
			UpdateBundleSortList(m_BeforeStackBundles, PostProcessEvent.BeforeStack);
			UpdateBundleSortList(m_AfterStackBundles, PostProcessEvent.AfterStack);
			sortedBundles = new Dictionary<PostProcessEvent, List<SerializedBundleRef>>(default(PostProcessEventComparer))
			{
				{
					PostProcessEvent.BeforeTransparent,
					m_BeforeTransparentBundles
				},
				{
					PostProcessEvent.BeforeStack,
					m_BeforeStackBundles
				},
				{
					PostProcessEvent.AfterStack,
					m_AfterStackBundles
				}
			};
			haveBundlesBeenInited = true;
		}

		private void UpdateBundleSortList(List<SerializedBundleRef> sortedList, PostProcessEvent evt)
		{
			List<PostProcessBundle> effects = (from kvp in m_Bundles
				where kvp.Value.attribute.eventType == evt && !kvp.Value.attribute.builtinEffect
				select kvp.Value).ToList();
			sortedList.RemoveAll(delegate(SerializedBundleRef x)
			{
				string searchStr = x.assemblyQualifiedName;
				return !effects.Exists((PostProcessBundle b) => ((object)b.settings).GetType().AssemblyQualifiedName == searchStr);
			});
			foreach (PostProcessBundle item2 in effects)
			{
				string typeName2 = ((object)item2.settings).GetType().AssemblyQualifiedName;
				if (!sortedList.Exists((SerializedBundleRef b) => b.assemblyQualifiedName == typeName2))
				{
					SerializedBundleRef item = new SerializedBundleRef
					{
						assemblyQualifiedName = typeName2
					};
					sortedList.Add(item);
				}
			}
			foreach (SerializedBundleRef sorted in sortedList)
			{
				string typeName = sorted.assemblyQualifiedName;
				PostProcessBundle postProcessBundle = (sorted.bundle = effects.Find((PostProcessBundle b) => ((object)b.settings).GetType().AssemblyQualifiedName == typeName));
			}
		}

		private void OnDisable()
		{
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)m_Camera != (Object)null)
			{
				if (m_LegacyCmdBufferBeforeReflections != null)
				{
					m_Camera.RemoveCommandBuffer((CameraEvent)21, m_LegacyCmdBufferBeforeReflections);
				}
				if (m_LegacyCmdBufferBeforeLighting != null)
				{
					m_Camera.RemoveCommandBuffer((CameraEvent)6, m_LegacyCmdBufferBeforeLighting);
				}
				if (m_LegacyCmdBufferOpaque != null)
				{
					m_Camera.RemoveCommandBuffer((CameraEvent)12, m_LegacyCmdBufferOpaque);
				}
				if (m_LegacyCmdBuffer != null)
				{
					m_Camera.RemoveCommandBuffer((CameraEvent)18, m_LegacyCmdBuffer);
				}
			}
			temporalAntialiasing.Release();
			m_LogHistogram.Release();
			foreach (PostProcessBundle value in m_Bundles.Values)
			{
				value.Release();
			}
			m_Bundles.Clear();
			m_PropertySheetFactory.Release();
			if (debugLayer != null)
			{
				debugLayer.OnDisable();
			}
			TextureLerper.instance.Clear();
			m_Camera.ResetProjectionMatrix();
			m_Camera.set_nonJitteredProjectionMatrix(m_Camera.get_projectionMatrix());
			Shader.SetGlobalVector("_FrustumJitter", Vector4.op_Implicit(Vector2.get_zero()));
			haveBundlesBeenInited = false;
		}

		private void Reset()
		{
			volumeTrigger = ((Component)this).get_transform();
		}

		private void OnPreCull()
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			if (!RuntimeUtilities.scriptableRenderPipelineActive)
			{
				if ((Object)(object)m_Camera == (Object)null || m_CurrentContext == null)
				{
					InitLegacy();
				}
				if (!m_Camera.get_usePhysicalProperties())
				{
					m_Camera.ResetProjectionMatrix();
				}
				m_Camera.set_nonJitteredProjectionMatrix(m_Camera.get_projectionMatrix());
				if (m_Camera.get_stereoEnabled())
				{
					m_Camera.ResetStereoProjectionMatrices();
				}
				else
				{
					Shader.SetGlobalFloat(ShaderIDs.RenderViewportScaleFactor, 1f);
				}
				BuildCommandBuffers();
				Shader.SetGlobalVector("_FrustumJitter", Vector4.op_Implicit(temporalAntialiasing.jitter));
			}
		}

		private void OnPreRender()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Invalid comparison between Unknown and I4
			if (!RuntimeUtilities.scriptableRenderPipelineActive && (int)m_Camera.get_stereoActiveEye() == 1)
			{
				BuildCommandBuffers();
			}
		}

		private RenderTextureFormat GetIntermediateFormat()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			if (intermediateFormat != prevIntermediateFormat)
			{
				supportsIntermediateFormat = SystemInfo.SupportsRenderTextureFormat(intermediateFormat);
				prevIntermediateFormat = intermediateFormat;
			}
			if (!supportsIntermediateFormat)
			{
				return (RenderTextureFormat)9;
			}
			return intermediateFormat;
		}

		private static bool RequiresInitialBlit(Camera camera, PostProcessRenderContext context)
		{
			if (camera.get_allowMSAA())
			{
				return true;
			}
			if (RuntimeUtilities.scriptableRenderPipelineActive)
			{
				return true;
			}
			return false;
		}

		private void UpdateSrcDstForOpaqueOnly(ref int src, ref int dst, PostProcessRenderContext context, RenderTargetIdentifier cameraTarget, int opaqueOnlyEffectsRemaining)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			if (src > -1)
			{
				context.command.ReleaseTemporaryRT(src);
			}
			context.source = context.destination;
			src = dst;
			if (opaqueOnlyEffectsRemaining == 1)
			{
				context.destination = cameraTarget;
				return;
			}
			dst = m_TargetPool.Get();
			context.destination = RenderTargetIdentifier.op_Implicit(dst);
			context.GetScreenSpaceTemporaryRT(context.command, dst, 0, context.sourceFormat, (RenderTextureReadWrite)0, (FilterMode)1);
		}

		private void BuildCommandBuffers()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			PostProcessRenderContext currentContext = m_CurrentContext;
			RenderTextureFormat val = GetIntermediateFormat();
			RenderTextureFormat val2 = (RenderTextureFormat)(m_Camera.get_allowHDR() ? ((int)val) : 7);
			if (!RuntimeUtilities.isFloatingPointFormat(val2))
			{
				m_NaNKilled = true;
			}
			currentContext.Reset();
			currentContext.camera = m_Camera;
			currentContext.sourceFormat = val2;
			m_LegacyCmdBufferBeforeReflections.Clear();
			m_LegacyCmdBufferBeforeLighting.Clear();
			m_LegacyCmdBufferOpaque.Clear();
			m_LegacyCmdBuffer.Clear();
			SetupContext(currentContext);
			currentContext.command = m_LegacyCmdBufferOpaque;
			TextureLerper.instance.BeginFrame(currentContext);
			UpdateVolumeSystem(currentContext.camera, currentContext.command);
			PostProcessBundle bundle = GetBundle<AmbientOcclusion>();
			AmbientOcclusion ambientOcclusion = bundle.CastSettings<AmbientOcclusion>();
			AmbientOcclusionRenderer ambientOcclusionRenderer = bundle.CastRenderer<AmbientOcclusionRenderer>();
			bool flag = ambientOcclusion.IsEnabledAndSupported(currentContext);
			bool flag2 = ambientOcclusionRenderer.IsAmbientOnly(currentContext);
			bool flag3 = flag && flag2;
			bool flag4 = flag && !flag2;
			PostProcessBundle bundle2 = GetBundle<ScreenSpaceReflections>();
			PostProcessEffectSettings settings = bundle2.settings;
			PostProcessEffectRenderer renderer = bundle2.renderer;
			bool flag5 = settings.IsEnabledAndSupported(currentContext);
			if (flag3)
			{
				IAmbientOcclusionMethod ambientOcclusionMethod = ambientOcclusionRenderer.Get();
				currentContext.command = m_LegacyCmdBufferBeforeReflections;
				ambientOcclusionMethod.RenderAmbientOnly(currentContext);
				currentContext.command = m_LegacyCmdBufferBeforeLighting;
				ambientOcclusionMethod.CompositeAmbientOnly(currentContext);
			}
			else if (flag4)
			{
				currentContext.command = m_LegacyCmdBufferOpaque;
				ambientOcclusionRenderer.Get().RenderAfterOpaque(currentContext);
			}
			bool flag6 = fog.IsEnabledAndSupported(currentContext);
			bool flag7 = HasOpaqueOnlyEffects(currentContext);
			int num = 0;
			num += (flag5 ? 1 : 0);
			num += (flag6 ? 1 : 0);
			num += (flag7 ? 1 : 0);
			RenderTargetIdentifier val3 = default(RenderTargetIdentifier);
			((RenderTargetIdentifier)(ref val3))._002Ector((BuiltinRenderTextureType)2);
			if (num > 0)
			{
				CommandBuffer val4 = (currentContext.command = m_LegacyCmdBufferOpaque);
				currentContext.source = val3;
				currentContext.destination = val3;
				int src = -1;
				int dst = -1;
				UpdateSrcDstForOpaqueOnly(ref src, ref dst, currentContext, val3, num + 1);
				if (RequiresInitialBlit(m_Camera, currentContext) || num == 1)
				{
					val4.BuiltinBlit(currentContext.source, currentContext.destination, RuntimeUtilities.copyStdMaterial, stopNaNPropagation ? 1 : 0);
					UpdateSrcDstForOpaqueOnly(ref src, ref dst, currentContext, val3, num);
				}
				if (flag5)
				{
					renderer.Render(currentContext);
					num--;
					UpdateSrcDstForOpaqueOnly(ref src, ref dst, currentContext, val3, num);
				}
				if (flag6)
				{
					fog.Render(currentContext);
					num--;
					UpdateSrcDstForOpaqueOnly(ref src, ref dst, currentContext, val3, num);
				}
				if (flag7)
				{
					RenderOpaqueOnly(currentContext);
				}
				val4.ReleaseTemporaryRT(src);
			}
			BuildPostEffectsOld(val2, currentContext, val3);
		}

		private void BuildPostEffectsOld(RenderTextureFormat sourceFormat, PostProcessRenderContext context, RenderTargetIdentifier cameraTarget)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			int num = -1;
			bool flag = !m_NaNKilled && stopNaNPropagation && RuntimeUtilities.isFloatingPointFormat(sourceFormat);
			if (RequiresInitialBlit(m_Camera, context) || flag)
			{
				num = m_TargetPool.Get();
				context.GetScreenSpaceTemporaryRT(m_LegacyCmdBuffer, num, 0, sourceFormat, (RenderTextureReadWrite)2, (FilterMode)1);
				m_LegacyCmdBuffer.BuiltinBlit(cameraTarget, RenderTargetIdentifier.op_Implicit(num), RuntimeUtilities.copyStdMaterial, stopNaNPropagation ? 1 : 0);
				if (!m_NaNKilled)
				{
					m_NaNKilled = stopNaNPropagation;
				}
				context.source = RenderTargetIdentifier.op_Implicit(num);
			}
			else
			{
				context.source = cameraTarget;
			}
			context.destination = cameraTarget;
			if (finalBlitToCameraTarget && !RuntimeUtilities.scriptableRenderPipelineActive)
			{
				if (Object.op_Implicit((Object)(object)m_Camera.get_targetTexture()))
				{
					context.destination = RenderTargetIdentifier.op_Implicit(m_Camera.get_targetTexture().get_colorBuffer());
				}
				else
				{
					context.flip = true;
					context.destination = RenderTargetIdentifier.op_Implicit(Display.get_main().get_colorBuffer());
				}
			}
			context.command = m_LegacyCmdBuffer;
			Render(context);
			if (num > -1)
			{
				m_LegacyCmdBuffer.ReleaseTemporaryRT(num);
			}
		}

		private void OnPostRender()
		{
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Invalid comparison between Unknown and I4
			if (!RuntimeUtilities.scriptableRenderPipelineActive && m_CurrentContext.IsTemporalAntialiasingActive())
			{
				if (m_CurrentContext.physicalCamera)
				{
					m_Camera.set_usePhysicalProperties(true);
				}
				else
				{
					m_Camera.ResetProjectionMatrix();
				}
				if (m_CurrentContext.stereoActive && (RuntimeUtilities.isSinglePassStereoEnabled || (int)m_Camera.get_stereoActiveEye() == 1))
				{
					m_Camera.ResetStereoProjectionMatrices();
				}
			}
		}

		public PostProcessBundle GetBundle<T>() where T : PostProcessEffectSettings
		{
			return GetBundle(typeof(T));
		}

		public PostProcessBundle GetBundle(Type settingsType)
		{
			Assert.IsTrue(m_Bundles.ContainsKey(settingsType), "Invalid type");
			return m_Bundles[settingsType];
		}

		public T GetSettings<T>() where T : PostProcessEffectSettings
		{
			return GetBundle<T>().CastSettings<T>();
		}

		public void BakeMSVOMap(CommandBuffer cmd, Camera camera, RenderTargetIdentifier destination, RenderTargetIdentifier? depthMap, bool invert, bool isMSAA = false)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			MultiScaleVO multiScaleVO = GetBundle<AmbientOcclusion>().CastRenderer<AmbientOcclusionRenderer>().GetMultiScaleVO();
			multiScaleVO.SetResources(m_Resources);
			multiScaleVO.GenerateAOMap(cmd, camera, destination, depthMap, invert, isMSAA);
		}

		internal void OverrideSettings(List<PostProcessEffectSettings> baseSettings, float interpFactor)
		{
			foreach (PostProcessEffectSettings baseSetting in baseSettings)
			{
				if (!baseSetting.active)
				{
					continue;
				}
				PostProcessEffectSettings settings = GetBundle(((object)baseSetting).GetType()).settings;
				int count = baseSetting.parameters.Count;
				for (int i = 0; i < count; i++)
				{
					ParameterOverride parameterOverride = baseSetting.parameters[i];
					if (parameterOverride.overrideState)
					{
						ParameterOverride parameterOverride2 = settings.parameters[i];
						parameterOverride2.Interp(parameterOverride2, parameterOverride, interpFactor);
					}
				}
			}
		}

		private void SetLegacyCameraFlags(PostProcessRenderContext context)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			DepthTextureMode val = context.camera.get_depthTextureMode();
			foreach (KeyValuePair<Type, PostProcessBundle> bundle in m_Bundles)
			{
				if (bundle.Value.settings.IsEnabledAndSupported(context))
				{
					val = (DepthTextureMode)(val | bundle.Value.renderer.GetCameraFlags());
				}
			}
			if (context.IsTemporalAntialiasingActive())
			{
				val = (DepthTextureMode)(val | temporalAntialiasing.GetCameraFlags());
			}
			if (fog.IsEnabledAndSupported(context))
			{
				val = (DepthTextureMode)(val | fog.GetCameraFlags());
			}
			if (debugLayer.debugOverlay != 0)
			{
				val = (DepthTextureMode)(val | debugLayer.GetCameraFlags());
			}
			context.camera.set_depthTextureMode(val);
		}

		public void ResetHistory()
		{
			foreach (KeyValuePair<Type, PostProcessBundle> bundle in m_Bundles)
			{
				bundle.Value.ResetHistory();
			}
			temporalAntialiasing.ResetHistory();
		}

		public bool HasOpaqueOnlyEffects(PostProcessRenderContext context)
		{
			return HasActiveEffects(PostProcessEvent.BeforeTransparent, context);
		}

		public bool HasActiveEffects(PostProcessEvent evt, PostProcessRenderContext context)
		{
			foreach (SerializedBundleRef item in sortedBundles[evt])
			{
				bool flag = item.bundle.settings.IsEnabledAndSupported(context);
				if (context.isSceneView)
				{
					if (item.bundle.attribute.allowInSceneView && flag)
					{
						return true;
					}
				}
				else if (flag)
				{
					return true;
				}
			}
			return false;
		}

		private void SetupContext(PostProcessRenderContext context)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Invalid comparison between Unknown and I4
			RuntimeUtilities.s_Resources = m_Resources;
			m_IsRenderingInSceneView = (int)context.camera.get_cameraType() == 2;
			context.isSceneView = m_IsRenderingInSceneView;
			context.resources = m_Resources;
			context.propertySheets = m_PropertySheetFactory;
			context.debugLayer = debugLayer;
			context.antialiasing = antialiasingMode;
			context.temporalAntialiasing = temporalAntialiasing;
			context.logHistogram = m_LogHistogram;
			context.physicalCamera = context.camera.get_usePhysicalProperties();
			SetLegacyCameraFlags(context);
			debugLayer.SetFrameSize(context.width, context.height);
			m_CurrentContext = context;
		}

		public void UpdateVolumeSystem(Camera cam, CommandBuffer cmd)
		{
			if (m_SettingsUpdateNeeded)
			{
				cmd.BeginSample("VolumeBlending");
				PostProcessManager.instance.UpdateSettings(this, cam);
				cmd.EndSample("VolumeBlending");
				m_TargetPool.Reset();
				if (RuntimeUtilities.scriptableRenderPipelineActive)
				{
					Shader.SetGlobalFloat(ShaderIDs.RenderViewportScaleFactor, 1f);
				}
			}
			m_SettingsUpdateNeeded = false;
		}

		public void RenderOpaqueOnly(PostProcessRenderContext context)
		{
			if (RuntimeUtilities.scriptableRenderPipelineActive)
			{
				SetupContext(context);
			}
			TextureLerper.instance.BeginFrame(context);
			UpdateVolumeSystem(context.camera, context.command);
			RenderList(sortedBundles[PostProcessEvent.BeforeTransparent], context, "OpaqueOnly");
		}

		public void Render(PostProcessRenderContext context)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Invalid comparison between Unknown and I4
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			if (RuntimeUtilities.scriptableRenderPipelineActive)
			{
				SetupContext(context);
			}
			TextureLerper.instance.BeginFrame(context);
			CommandBuffer command = context.command;
			UpdateVolumeSystem(context.camera, context.command);
			int num = -1;
			RenderTargetIdentifier source = context.source;
			if (context.stereoActive && context.numberOfEyes > 1 && context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePass)
			{
				command.SetSinglePassStereo((SinglePassStereoMode)0);
				command.DisableShaderKeyword("UNITY_SINGLE_PASS_STEREO");
			}
			for (int i = 0; i < context.numberOfEyes; i++)
			{
				bool flag = false;
				if (stopNaNPropagation && !m_NaNKilled)
				{
					num = m_TargetPool.Get();
					context.GetScreenSpaceTemporaryRT(command, num, 0, context.sourceFormat, (RenderTextureReadWrite)0, (FilterMode)1);
					if (context.stereoActive && context.numberOfEyes > 1)
					{
						if (context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePassInstanced)
						{
							command.BlitFullscreenTriangleFromTexArray(context.source, RenderTargetIdentifier.op_Implicit(num), RuntimeUtilities.copyFromTexArraySheet, 1, clear: false, i);
							flag = true;
						}
						else if (context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePass)
						{
							command.BlitFullscreenTriangleFromDoubleWide(context.source, RenderTargetIdentifier.op_Implicit(num), RuntimeUtilities.copyStdFromDoubleWideMaterial, 1, i);
							flag = true;
						}
					}
					else
					{
						command.BlitFullscreenTriangle(context.source, RenderTargetIdentifier.op_Implicit(num), RuntimeUtilities.copySheet, 1);
					}
					context.source = RenderTargetIdentifier.op_Implicit(num);
					m_NaNKilled = true;
				}
				if (!flag && context.numberOfEyes > 1)
				{
					num = m_TargetPool.Get();
					context.GetScreenSpaceTemporaryRT(command, num, 0, context.sourceFormat, (RenderTextureReadWrite)0, (FilterMode)1);
					if (context.stereoActive)
					{
						if (context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePassInstanced)
						{
							command.BlitFullscreenTriangleFromTexArray(context.source, RenderTargetIdentifier.op_Implicit(num), RuntimeUtilities.copyFromTexArraySheet, 1, clear: false, i);
							flag = true;
						}
						else if (context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePass)
						{
							command.BlitFullscreenTriangleFromDoubleWide(context.source, RenderTargetIdentifier.op_Implicit(num), RuntimeUtilities.copyStdFromDoubleWideMaterial, stopNaNPropagation ? 1 : 0, i);
							flag = true;
						}
					}
					context.source = RenderTargetIdentifier.op_Implicit(num);
				}
				if (context.IsTemporalAntialiasingActive())
				{
					temporalAntialiasing.sampleCount = 8;
					if (!RuntimeUtilities.scriptableRenderPipelineActive)
					{
						if (context.stereoActive)
						{
							if ((int)context.camera.get_stereoActiveEye() != 1)
							{
								temporalAntialiasing.ConfigureStereoJitteredProjectionMatrices(context);
							}
						}
						else
						{
							temporalAntialiasing.ConfigureJitteredProjectionMatrix(context);
						}
					}
					int num2 = m_TargetPool.Get();
					RenderTargetIdentifier destination = context.destination;
					context.GetScreenSpaceTemporaryRT(command, num2, 0, context.sourceFormat, (RenderTextureReadWrite)0, (FilterMode)1);
					context.destination = RenderTargetIdentifier.op_Implicit(num2);
					temporalAntialiasing.Render(context);
					context.source = RenderTargetIdentifier.op_Implicit(num2);
					context.destination = destination;
					if (num > -1)
					{
						command.ReleaseTemporaryRT(num);
					}
					num = num2;
				}
				bool num3 = HasActiveEffects(PostProcessEvent.BeforeStack, context);
				bool flag2 = HasActiveEffects(PostProcessEvent.AfterStack, context) && !breakBeforeColorGrading;
				bool flag3 = (flag2 || antialiasingMode == Antialiasing.FastApproximateAntialiasing || (antialiasingMode == Antialiasing.SubpixelMorphologicalAntialiasing && subpixelMorphologicalAntialiasing.IsSupported())) && !breakBeforeColorGrading;
				if (num3)
				{
					num = RenderInjectionPoint(PostProcessEvent.BeforeStack, context, "BeforeStack", num);
				}
				num = RenderBuiltins(context, !flag3, num, i);
				if (flag2)
				{
					num = RenderInjectionPoint(PostProcessEvent.AfterStack, context, "AfterStack", num);
				}
				if (flag3)
				{
					RenderFinalPass(context, num, i);
				}
				if (context.stereoActive)
				{
					context.source = source;
				}
			}
			if (context.stereoActive && context.numberOfEyes > 1 && context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePass)
			{
				command.SetSinglePassStereo((SinglePassStereoMode)1);
				command.EnableShaderKeyword("UNITY_SINGLE_PASS_STEREO");
			}
			debugLayer.RenderSpecialOverlays(context);
			debugLayer.RenderMonitors(context);
			TextureLerper.instance.EndFrame();
			debugLayer.EndFrame();
			m_SettingsUpdateNeeded = true;
			m_NaNKilled = false;
		}

		private int RenderInjectionPoint(PostProcessEvent evt, PostProcessRenderContext context, string marker, int releaseTargetAfterUse = -1)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			int num = m_TargetPool.Get();
			RenderTargetIdentifier destination = context.destination;
			CommandBuffer command = context.command;
			context.GetScreenSpaceTemporaryRT(command, num, 0, context.sourceFormat, (RenderTextureReadWrite)0, (FilterMode)1);
			context.destination = RenderTargetIdentifier.op_Implicit(num);
			RenderList(sortedBundles[evt], context, marker);
			context.source = RenderTargetIdentifier.op_Implicit(num);
			context.destination = destination;
			if (releaseTargetAfterUse > -1)
			{
				command.ReleaseTemporaryRT(releaseTargetAfterUse);
			}
			return num;
		}

		private void RenderList(List<SerializedBundleRef> list, PostProcessRenderContext context, string marker)
		{
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			CommandBuffer command = context.command;
			command.BeginSample(marker);
			m_ActiveEffects.Clear();
			for (int i = 0; i < list.Count; i++)
			{
				PostProcessBundle bundle = list[i].bundle;
				if (bundle.settings.IsEnabledAndSupported(context) && (!context.isSceneView || (context.isSceneView && bundle.attribute.allowInSceneView)))
				{
					m_ActiveEffects.Add(bundle.renderer);
				}
			}
			int count = m_ActiveEffects.Count;
			if (count == 1)
			{
				m_ActiveEffects[0].Render(context);
			}
			else
			{
				m_Targets.Clear();
				m_Targets.Add(context.source);
				int num = m_TargetPool.Get();
				int num2 = m_TargetPool.Get();
				for (int j = 0; j < count - 1; j++)
				{
					m_Targets.Add(RenderTargetIdentifier.op_Implicit((j % 2 == 0) ? num : num2));
				}
				m_Targets.Add(context.destination);
				context.GetScreenSpaceTemporaryRT(command, num, 0, context.sourceFormat, (RenderTextureReadWrite)0, (FilterMode)1);
				if (count > 2)
				{
					context.GetScreenSpaceTemporaryRT(command, num2, 0, context.sourceFormat, (RenderTextureReadWrite)0, (FilterMode)1);
				}
				for (int k = 0; k < count; k++)
				{
					context.source = m_Targets[k];
					context.destination = m_Targets[k + 1];
					m_ActiveEffects[k].Render(context);
				}
				command.ReleaseTemporaryRT(num);
				if (count > 2)
				{
					command.ReleaseTemporaryRT(num2);
				}
			}
			command.EndSample(marker);
		}

		private void ApplyFlip(PostProcessRenderContext context, MaterialPropertyBlock properties)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			if (context.flip && !context.isSceneView)
			{
				properties.SetVector(ShaderIDs.UVTransform, new Vector4(1f, 1f, 0f, 0f));
			}
			else
			{
				ApplyDefaultFlip(properties);
			}
		}

		private void ApplyDefaultFlip(MaterialPropertyBlock properties)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			properties.SetVector(ShaderIDs.UVTransform, SystemInfo.get_graphicsUVStartsAtTop() ? new Vector4(1f, -1f, 0f, 1f) : new Vector4(1f, 1f, 0f, 0f));
		}

		private int RenderBuiltins(PostProcessRenderContext context, bool isFinalPass, int releaseTargetAfterUse = -1, int eye = -1)
		{
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.uber);
			propertySheet.ClearKeywords();
			propertySheet.properties.Clear();
			context.uberSheet = propertySheet;
			context.bloomBufferNameID = -1;
			context.autoExposureTexture = (Texture)(object)RuntimeUtilities.whiteTexture;
			if (isFinalPass && context.stereoActive && context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePassInstanced)
			{
				propertySheet.EnableKeyword("STEREO_INSTANCING_ENABLED");
			}
			CommandBuffer command = context.command;
			command.BeginSample("BuiltinStack");
			int num = -1;
			RenderTargetIdentifier destination = context.destination;
			if (!isFinalPass)
			{
				num = m_TargetPool.Get();
				context.GetScreenSpaceTemporaryRT(command, num, 0, context.sourceFormat, (RenderTextureReadWrite)0, (FilterMode)1);
				context.destination = RenderTargetIdentifier.op_Implicit(num);
				if (antialiasingMode == Antialiasing.FastApproximateAntialiasing && !fastApproximateAntialiasing.keepAlpha)
				{
					propertySheet.properties.SetFloat(ShaderIDs.LumaInAlpha, 1f);
				}
			}
			int num2 = RenderEffect<DepthOfFieldEffect>(context, useTempTarget: true);
			int num3 = RenderEffect<MotionBlur>(context, useTempTarget: true);
			if (ShouldGenerateLogHistogram(context))
			{
				m_LogHistogram.Generate(context);
			}
			RenderEffect<AutoExposure>(context);
			propertySheet.properties.SetTexture(ShaderIDs.AutoExposureTex, context.autoExposureTexture);
			RenderEffect<LensDistortion>(context);
			RenderEffect<ChromaticAberration>(context);
			RenderEffect<Bloom>(context);
			RenderEffect<Vignette>(context);
			RenderEffect<Grain>(context);
			if (!breakBeforeColorGrading)
			{
				RenderEffect<ColorGrading>(context);
			}
			if (isFinalPass)
			{
				propertySheet.EnableKeyword("FINALPASS");
				dithering.Render(context);
				ApplyFlip(context, propertySheet.properties);
			}
			else
			{
				ApplyDefaultFlip(propertySheet.properties);
			}
			if (context.stereoActive && context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePassInstanced)
			{
				propertySheet.properties.SetFloat(ShaderIDs.DepthSlice, (float)eye);
				command.BlitFullscreenTriangleToTexArray(context.source, context.destination, propertySheet, 0, clear: false, eye);
			}
			else if (isFinalPass && context.stereoActive && context.numberOfEyes > 1 && context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePass)
			{
				command.BlitFullscreenTriangleToDoubleWide(context.source, context.destination, propertySheet, 0, eye);
			}
			else
			{
				command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0);
			}
			context.source = context.destination;
			context.destination = destination;
			if (releaseTargetAfterUse > -1)
			{
				command.ReleaseTemporaryRT(releaseTargetAfterUse);
			}
			if (num3 > -1)
			{
				command.ReleaseTemporaryRT(num3);
			}
			if (num2 > -1)
			{
				command.ReleaseTemporaryRT(num2);
			}
			if (context.bloomBufferNameID > -1)
			{
				command.ReleaseTemporaryRT(context.bloomBufferNameID);
			}
			command.EndSample("BuiltinStack");
			return num;
		}

		private void RenderFinalPass(PostProcessRenderContext context, int releaseTargetAfterUse = -1, int eye = -1)
		{
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			CommandBuffer command = context.command;
			command.BeginSample("FinalPass");
			if (breakBeforeColorGrading)
			{
				PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.discardAlpha);
				if (context.stereoActive && context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePassInstanced)
				{
					propertySheet.EnableKeyword("STEREO_INSTANCING_ENABLED");
				}
				if (context.stereoActive && context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePassInstanced)
				{
					propertySheet.properties.SetFloat(ShaderIDs.DepthSlice, (float)eye);
					command.BlitFullscreenTriangleToTexArray(context.source, context.destination, propertySheet, 0, clear: false, eye);
				}
				else if (context.stereoActive && context.numberOfEyes > 1 && context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePass)
				{
					command.BlitFullscreenTriangleToDoubleWide(context.source, context.destination, propertySheet, 0, eye);
				}
				else
				{
					command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0);
				}
			}
			else
			{
				PropertySheet propertySheet2 = context.propertySheets.Get(context.resources.shaders.finalPass);
				propertySheet2.ClearKeywords();
				propertySheet2.properties.Clear();
				context.uberSheet = propertySheet2;
				int num = -1;
				if (context.stereoActive && context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePassInstanced)
				{
					propertySheet2.EnableKeyword("STEREO_INSTANCING_ENABLED");
				}
				if (antialiasingMode == Antialiasing.FastApproximateAntialiasing)
				{
					propertySheet2.EnableKeyword(fastApproximateAntialiasing.fastMode ? "FXAA_LOW" : "FXAA");
					if (fastApproximateAntialiasing.keepAlpha)
					{
						propertySheet2.EnableKeyword("FXAA_KEEP_ALPHA");
					}
				}
				else if (antialiasingMode == Antialiasing.SubpixelMorphologicalAntialiasing && subpixelMorphologicalAntialiasing.IsSupported())
				{
					num = m_TargetPool.Get();
					RenderTargetIdentifier destination = context.destination;
					context.GetScreenSpaceTemporaryRT(context.command, num, 0, context.sourceFormat, (RenderTextureReadWrite)0, (FilterMode)1);
					context.destination = RenderTargetIdentifier.op_Implicit(num);
					subpixelMorphologicalAntialiasing.Render(context);
					context.source = RenderTargetIdentifier.op_Implicit(num);
					context.destination = destination;
				}
				dithering.Render(context);
				ApplyFlip(context, propertySheet2.properties);
				if (context.stereoActive && context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePassInstanced)
				{
					propertySheet2.properties.SetFloat(ShaderIDs.DepthSlice, (float)eye);
					command.BlitFullscreenTriangleToTexArray(context.source, context.destination, propertySheet2, 0, clear: false, eye);
				}
				else if (context.stereoActive && context.numberOfEyes > 1 && context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePass)
				{
					command.BlitFullscreenTriangleToDoubleWide(context.source, context.destination, propertySheet2, 0, eye);
				}
				else
				{
					command.BlitFullscreenTriangle(context.source, context.destination, propertySheet2, 0);
				}
				if (num > -1)
				{
					command.ReleaseTemporaryRT(num);
				}
			}
			if (releaseTargetAfterUse > -1)
			{
				command.ReleaseTemporaryRT(releaseTargetAfterUse);
			}
			command.EndSample("FinalPass");
		}

		private int RenderEffect<T>(PostProcessRenderContext context, bool useTempTarget = false) where T : PostProcessEffectSettings
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			PostProcessBundle bundle = GetBundle<T>();
			if (!bundle.settings.IsEnabledAndSupported(context))
			{
				return -1;
			}
			if (m_IsRenderingInSceneView && !bundle.attribute.allowInSceneView)
			{
				return -1;
			}
			if (!useTempTarget)
			{
				bundle.renderer.Render(context);
				return -1;
			}
			RenderTargetIdentifier destination = context.destination;
			int num = m_TargetPool.Get();
			context.GetScreenSpaceTemporaryRT(context.command, num, 0, context.sourceFormat, (RenderTextureReadWrite)0, (FilterMode)1);
			context.destination = RenderTargetIdentifier.op_Implicit(num);
			bundle.renderer.Render(context);
			context.source = RenderTargetIdentifier.op_Implicit(num);
			context.destination = destination;
			return num;
		}

		private bool ShouldGenerateLogHistogram(PostProcessRenderContext context)
		{
			bool num = GetBundle<AutoExposure>().settings.IsEnabledAndSupported(context);
			bool flag = debugLayer.lightMeter.IsRequestedAndSupported(context);
			return num || flag;
		}

		public PostProcessLayer()
			: this()
		{
		}//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)

	}
}
