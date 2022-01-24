using System.Collections.Generic;

namespace UnityEngine.Rendering.PostProcessing
{
	public class PostProcessRenderContext
	{
		public enum StereoRenderingMode
		{
			MultiPass,
			SinglePass,
			SinglePassInstanced,
			SinglePassMultiview
		}

		public bool dlssEnabled;

		private Camera m_Camera;

		internal PropertySheet uberSheet;

		internal Texture autoExposureTexture;

		internal LogHistogram logHistogram;

		internal Texture logLut;

		internal AutoExposure autoExposure;

		internal int bloomBufferNameID;

		internal bool physicalCamera;

		private RenderTextureDescriptor m_sourceDescriptor;

		public Camera camera
		{
			get
			{
				return m_Camera;
			}
			set
			{
				m_Camera = value;
				if (!m_Camera.get_stereoEnabled())
				{
					width = m_Camera.get_pixelWidth();
					height = m_Camera.get_pixelHeight();
					((RenderTextureDescriptor)(ref m_sourceDescriptor)).set_width(width);
					((RenderTextureDescriptor)(ref m_sourceDescriptor)).set_height(height);
					screenWidth = width;
					screenHeight = height;
					stereoActive = false;
					numberOfEyes = 1;
				}
			}
		}

		public CommandBuffer command { get; set; }

		public RenderTargetIdentifier source { get; set; }

		public RenderTargetIdentifier destination { get; set; }

		public RenderTextureFormat sourceFormat { get; set; }

		public bool flip { get; set; }

		public PostProcessResources resources { get; internal set; }

		public PropertySheetFactory propertySheets { get; internal set; }

		public Dictionary<string, object> userData { get; private set; }

		public PostProcessDebugLayer debugLayer { get; internal set; }

		public int width { get; set; }

		public int height { get; set; }

		public bool stereoActive { get; private set; }

		public int xrActiveEye { get; private set; }

		public int numberOfEyes { get; private set; }

		public StereoRenderingMode stereoRenderingMode { get; private set; }

		public int screenWidth { get; set; }

		public int screenHeight { get; set; }

		public bool isSceneView { get; internal set; }

		public PostProcessLayer.Antialiasing antialiasing { get; internal set; }

		public TemporalAntialiasing temporalAntialiasing { get; internal set; }

		public void Resize(int width, int height, bool dlssEnabled)
		{
			int num3 = (this.width = (screenWidth = width));
			num3 = (this.height = (screenHeight = height));
			this.dlssEnabled = dlssEnabled;
			((RenderTextureDescriptor)(ref m_sourceDescriptor)).set_width(width);
			((RenderTextureDescriptor)(ref m_sourceDescriptor)).set_height(height);
		}

		public void Reset()
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			m_Camera = null;
			width = 0;
			height = 0;
			dlssEnabled = false;
			m_sourceDescriptor = new RenderTextureDescriptor(0, 0);
			physicalCamera = false;
			stereoActive = false;
			xrActiveEye = 0;
			screenWidth = 0;
			screenHeight = 0;
			command = null;
			source = RenderTargetIdentifier.op_Implicit(0);
			destination = RenderTargetIdentifier.op_Implicit(0);
			sourceFormat = (RenderTextureFormat)0;
			flip = false;
			resources = null;
			propertySheets = null;
			debugLayer = null;
			isSceneView = false;
			antialiasing = PostProcessLayer.Antialiasing.None;
			temporalAntialiasing = null;
			uberSheet = null;
			autoExposureTexture = null;
			logLut = null;
			autoExposure = null;
			bloomBufferNameID = -1;
			if (userData == null)
			{
				userData = new Dictionary<string, object>();
			}
			userData.Clear();
		}

		public bool IsTemporalAntialiasingActive()
		{
			if (antialiasing == PostProcessLayer.Antialiasing.TemporalAntialiasing && !isSceneView)
			{
				return temporalAntialiasing.IsSupported();
			}
			return false;
		}

		public bool IsDebugOverlayEnabled(DebugOverlay overlay)
		{
			return debugLayer.debugOverlay == overlay;
		}

		public void PushDebugOverlay(CommandBuffer cmd, RenderTargetIdentifier source, PropertySheet sheet, int pass)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			debugLayer.PushDebugOverlay(cmd, source, sheet, pass);
		}

		private RenderTextureDescriptor GetDescriptor(int depthBufferBits = 0, RenderTextureFormat colorFormat = 7, RenderTextureReadWrite readWrite = 0)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Invalid comparison between Unknown and I4
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Invalid comparison between Unknown and I4
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Invalid comparison between Unknown and I4
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Invalid comparison between Unknown and I4
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			RenderTextureDescriptor result = default(RenderTextureDescriptor);
			((RenderTextureDescriptor)(ref result))._002Ector(((RenderTextureDescriptor)(ref m_sourceDescriptor)).get_width(), ((RenderTextureDescriptor)(ref m_sourceDescriptor)).get_height(), ((RenderTextureDescriptor)(ref m_sourceDescriptor)).get_colorFormat(), depthBufferBits);
			((RenderTextureDescriptor)(ref result)).set_dimension(((RenderTextureDescriptor)(ref m_sourceDescriptor)).get_dimension());
			((RenderTextureDescriptor)(ref result)).set_volumeDepth(((RenderTextureDescriptor)(ref m_sourceDescriptor)).get_volumeDepth());
			((RenderTextureDescriptor)(ref result)).set_vrUsage(((RenderTextureDescriptor)(ref m_sourceDescriptor)).get_vrUsage());
			((RenderTextureDescriptor)(ref result)).set_msaaSamples(((RenderTextureDescriptor)(ref m_sourceDescriptor)).get_msaaSamples());
			((RenderTextureDescriptor)(ref result)).set_memoryless(((RenderTextureDescriptor)(ref m_sourceDescriptor)).get_memoryless());
			((RenderTextureDescriptor)(ref result)).set_useMipMap(((RenderTextureDescriptor)(ref m_sourceDescriptor)).get_useMipMap());
			((RenderTextureDescriptor)(ref result)).set_autoGenerateMips(((RenderTextureDescriptor)(ref m_sourceDescriptor)).get_autoGenerateMips());
			((RenderTextureDescriptor)(ref result)).set_enableRandomWrite(((RenderTextureDescriptor)(ref m_sourceDescriptor)).get_enableRandomWrite());
			((RenderTextureDescriptor)(ref result)).set_shadowSamplingMode(((RenderTextureDescriptor)(ref m_sourceDescriptor)).get_shadowSamplingMode());
			if ((int)colorFormat != 7)
			{
				((RenderTextureDescriptor)(ref result)).set_colorFormat(colorFormat);
			}
			if ((int)readWrite == 2)
			{
				((RenderTextureDescriptor)(ref result)).set_sRGB(true);
			}
			else if ((int)readWrite == 1)
			{
				((RenderTextureDescriptor)(ref result)).set_sRGB(false);
			}
			else if ((int)readWrite == 0)
			{
				((RenderTextureDescriptor)(ref result)).set_sRGB((int)QualitySettings.get_activeColorSpace() > 0);
			}
			return result;
		}

		public void GetScreenSpaceTemporaryRT(CommandBuffer cmd, int nameID, int depthBufferBits = 0, RenderTextureFormat colorFormat = 7, RenderTextureReadWrite readWrite = 0, FilterMode filter = 1, int widthOverride = 0, int heightOverride = 0)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Invalid comparison between Unknown and I4
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			RenderTextureDescriptor descriptor = GetDescriptor(depthBufferBits, colorFormat, readWrite);
			if (widthOverride > 0)
			{
				((RenderTextureDescriptor)(ref descriptor)).set_width(widthOverride);
			}
			if (heightOverride > 0)
			{
				((RenderTextureDescriptor)(ref descriptor)).set_height(heightOverride);
			}
			if (stereoActive && (int)((RenderTextureDescriptor)(ref descriptor)).get_dimension() == 5)
			{
				((RenderTextureDescriptor)(ref descriptor)).set_dimension((TextureDimension)2);
			}
			cmd.GetTemporaryRT(nameID, descriptor, filter);
		}

		public RenderTexture GetScreenSpaceTemporaryRT(int depthBufferBits = 0, RenderTextureFormat colorFormat = 7, RenderTextureReadWrite readWrite = 0, int widthOverride = 0, int heightOverride = 0)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			RenderTextureDescriptor descriptor = GetDescriptor(depthBufferBits, colorFormat, readWrite);
			if (widthOverride > 0)
			{
				((RenderTextureDescriptor)(ref descriptor)).set_width(widthOverride);
			}
			if (heightOverride > 0)
			{
				((RenderTextureDescriptor)(ref descriptor)).set_height(heightOverride);
			}
			return RenderTexture.GetTemporary(descriptor);
		}
	}
}
