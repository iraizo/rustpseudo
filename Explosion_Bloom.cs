using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("KriptoFX/Explosion_Bloom")]
[ImageEffectAllowedInSceneView]
public class Explosion_Bloom : MonoBehaviour
{
	[Serializable]
	public struct Settings
	{
		[SerializeField]
		[Tooltip("Filters out pixels under this level of brightness.")]
		public float threshold;

		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("Makes transition between under/over-threshold gradual.")]
		public float softKnee;

		[SerializeField]
		[Range(1f, 7f)]
		[Tooltip("Changes extent of veiling effects in a screen resolution-independent fashion.")]
		public float radius;

		[SerializeField]
		[Tooltip("Blend factor of the result image.")]
		public float intensity;

		[SerializeField]
		[Tooltip("Controls filter quality and buffer resolution.")]
		public bool highQuality;

		[SerializeField]
		[Tooltip("Reduces flashing noise with an additional filter.")]
		public bool antiFlicker;

		public float thresholdGamma
		{
			get
			{
				return Mathf.Max(0f, threshold);
			}
			set
			{
				threshold = value;
			}
		}

		public float thresholdLinear
		{
			get
			{
				return Mathf.GammaToLinearSpace(thresholdGamma);
			}
			set
			{
				threshold = Mathf.LinearToGammaSpace(value);
			}
		}

		public static Settings defaultSettings
		{
			get
			{
				Settings result = default(Settings);
				result.threshold = 2f;
				result.softKnee = 0f;
				result.radius = 7f;
				result.intensity = 0.7f;
				result.highQuality = true;
				result.antiFlicker = true;
				return result;
			}
		}
	}

	[SerializeField]
	public Settings settings = Settings.defaultSettings;

	[SerializeField]
	[HideInInspector]
	private Shader m_Shader;

	private Material m_Material;

	private const int kMaxIterations = 16;

	private RenderTexture[] m_blurBuffer1 = (RenderTexture[])(object)new RenderTexture[16];

	private RenderTexture[] m_blurBuffer2 = (RenderTexture[])(object)new RenderTexture[16];

	private int m_Threshold;

	private int m_Curve;

	private int m_PrefilterOffs;

	private int m_SampleScale;

	private int m_Intensity;

	private int m_BaseTex;

	public Shader shader
	{
		get
		{
			if ((Object)(object)m_Shader == (Object)null)
			{
				m_Shader = Shader.Find("Hidden/KriptoFX/PostEffects/Explosion_Bloom");
			}
			return m_Shader;
		}
	}

	public Material material
	{
		get
		{
			if ((Object)(object)m_Material == (Object)null)
			{
				m_Material = CheckShaderAndCreateMaterial(shader);
			}
			return m_Material;
		}
	}

	public static bool supportsDX11
	{
		get
		{
			if (SystemInfo.get_graphicsShaderLevel() >= 50)
			{
				return SystemInfo.get_supportsComputeShaders();
			}
			return false;
		}
	}

	public static bool IsSupported(Shader s, bool needDepth, bool needHdr, MonoBehaviour effect)
	{
		if ((Object)(object)s == (Object)null || !s.get_isSupported())
		{
			Debug.LogWarningFormat("Missing shader for image effect {0}", new object[1] { effect });
			return false;
		}
		if (!SystemInfo.get_supportsImageEffects())
		{
			Debug.LogWarningFormat("Image effects aren't supported on this device ({0})", new object[1] { effect });
			return false;
		}
		if (needDepth && !SystemInfo.SupportsRenderTextureFormat((RenderTextureFormat)1))
		{
			Debug.LogWarningFormat("Depth textures aren't supported on this device ({0})", new object[1] { effect });
			return false;
		}
		if (needHdr && !SystemInfo.SupportsRenderTextureFormat((RenderTextureFormat)2))
		{
			Debug.LogWarningFormat("Floating point textures aren't supported on this device ({0})", new object[1] { effect });
			return false;
		}
		return true;
	}

	public static Material CheckShaderAndCreateMaterial(Shader s)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		if ((Object)(object)s == (Object)null || !s.get_isSupported())
		{
			return null;
		}
		Material val = new Material(s);
		((Object)val).set_hideFlags((HideFlags)52);
		return val;
	}

	private void Awake()
	{
		m_Threshold = Shader.PropertyToID("_Threshold");
		m_Curve = Shader.PropertyToID("_Curve");
		m_PrefilterOffs = Shader.PropertyToID("_PrefilterOffs");
		m_SampleScale = Shader.PropertyToID("_SampleScale");
		m_Intensity = Shader.PropertyToID("_Intensity");
		m_BaseTex = Shader.PropertyToID("_BaseTex");
	}

	private void OnEnable()
	{
		if (!IsSupported(shader, needDepth: true, needHdr: false, (MonoBehaviour)(object)this))
		{
			((Behaviour)this).set_enabled(false);
		}
	}

	private void OnDisable()
	{
		if ((Object)(object)m_Material != (Object)null)
		{
			Object.DestroyImmediate((Object)(object)m_Material);
		}
		m_Material = null;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		bool isMobilePlatform = Application.get_isMobilePlatform();
		int num = ((Texture)source).get_width();
		int num2 = ((Texture)source).get_height();
		if (!settings.highQuality)
		{
			num /= 2;
			num2 /= 2;
		}
		RenderTextureFormat val = (RenderTextureFormat)(isMobilePlatform ? 7 : 9);
		float num3 = Mathf.Log((float)num2, 2f) + settings.radius - 8f;
		int num4 = (int)num3;
		int num5 = Mathf.Clamp(num4, 1, 16);
		float thresholdLinear = settings.thresholdLinear;
		material.SetFloat(m_Threshold, thresholdLinear);
		float num6 = thresholdLinear * settings.softKnee + 1E-05f;
		Vector3 val2 = default(Vector3);
		((Vector3)(ref val2))._002Ector(thresholdLinear - num6, num6 * 2f, 0.25f / num6);
		material.SetVector(m_Curve, Vector4.op_Implicit(val2));
		bool flag = !settings.highQuality && settings.antiFlicker;
		material.SetFloat(m_PrefilterOffs, flag ? (-0.5f) : 0f);
		material.SetFloat(m_SampleScale, 0.5f + num3 - (float)num4);
		material.SetFloat(m_Intensity, Mathf.Max(0f, settings.intensity));
		RenderTexture temporary = RenderTexture.GetTemporary(num, num2, 0, val);
		Graphics.Blit((Texture)(object)source, temporary, material, settings.antiFlicker ? 1 : 0);
		RenderTexture val3 = temporary;
		for (int i = 0; i < num5; i++)
		{
			m_blurBuffer1[i] = RenderTexture.GetTemporary(((Texture)val3).get_width() / 2, ((Texture)val3).get_height() / 2, 0, val);
			Graphics.Blit((Texture)(object)val3, m_blurBuffer1[i], material, (i == 0) ? (settings.antiFlicker ? 3 : 2) : 4);
			val3 = m_blurBuffer1[i];
		}
		for (int num7 = num5 - 2; num7 >= 0; num7--)
		{
			RenderTexture val4 = m_blurBuffer1[num7];
			material.SetTexture(m_BaseTex, (Texture)(object)val4);
			m_blurBuffer2[num7] = RenderTexture.GetTemporary(((Texture)val4).get_width(), ((Texture)val4).get_height(), 0, val);
			Graphics.Blit((Texture)(object)val3, m_blurBuffer2[num7], material, settings.highQuality ? 6 : 5);
			val3 = m_blurBuffer2[num7];
		}
		int num8 = 7;
		num8 += (settings.highQuality ? 1 : 0);
		material.SetTexture(m_BaseTex, (Texture)(object)source);
		Graphics.Blit((Texture)(object)val3, destination, material, num8);
		for (int j = 0; j < 16; j++)
		{
			if ((Object)(object)m_blurBuffer1[j] != (Object)null)
			{
				RenderTexture.ReleaseTemporary(m_blurBuffer1[j]);
			}
			if ((Object)(object)m_blurBuffer2[j] != (Object)null)
			{
				RenderTexture.ReleaseTemporary(m_blurBuffer2[j]);
			}
			m_blurBuffer1[j] = null;
			m_blurBuffer2[j] = null;
		}
		RenderTexture.ReleaseTemporary(temporary);
	}

	public Explosion_Bloom()
		: this()
	{
	}
}
