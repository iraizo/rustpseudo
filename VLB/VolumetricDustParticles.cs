using System;
using UnityEngine;

namespace VLB
{
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(VolumetricLightBeam))]
	[HelpURL("http://saladgamer.com/vlb-doc/comp-dustparticles/")]
	public class VolumetricDustParticles : MonoBehaviour
	{
		public enum Direction
		{
			Beam,
			Random
		}

		[Range(0f, 1f)]
		public float alpha = 0.5f;

		[Range(0.0001f, 0.1f)]
		public float size = 0.01f;

		public Direction direction = Direction.Random;

		public float speed = 0.03f;

		public float density = 5f;

		[Range(0f, 1f)]
		public float spawnMaxDistance = 0.7f;

		public bool cullingEnabled = true;

		public float cullingMaxDistance = 10f;

		public static bool isFeatureSupported = true;

		private ParticleSystem m_Particles;

		private ParticleSystemRenderer m_Renderer;

		private static bool ms_NoMainCameraLogged = false;

		private static Camera ms_MainCamera = null;

		private VolumetricLightBeam m_Master;

		public bool isCulled { get; private set; }

		public bool particlesAreInstantiated => Object.op_Implicit((Object)(object)m_Particles);

		public int particlesCurrentCount
		{
			get
			{
				if (!Object.op_Implicit((Object)(object)m_Particles))
				{
					return 0;
				}
				return m_Particles.get_particleCount();
			}
		}

		public int particlesMaxCount
		{
			get
			{
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				if (!Object.op_Implicit((Object)(object)m_Particles))
				{
					return 0;
				}
				MainModule main = m_Particles.get_main();
				return ((MainModule)(ref main)).get_maxParticles();
			}
		}

		public Camera mainCamera
		{
			get
			{
				if (!Object.op_Implicit((Object)(object)ms_MainCamera))
				{
					ms_MainCamera = Camera.get_main();
					if (!Object.op_Implicit((Object)(object)ms_MainCamera) && !ms_NoMainCameraLogged)
					{
						Debug.LogErrorFormat((Object)(object)((Component)this).get_gameObject(), "In order to use 'VolumetricDustParticles' culling, you must have a MainCamera defined in your scene.", Array.Empty<object>());
						ms_NoMainCameraLogged = true;
					}
				}
				return ms_MainCamera;
			}
		}

		private void Start()
		{
			isCulled = false;
			m_Master = ((Component)this).GetComponent<VolumetricLightBeam>();
			Debug.Assert(Object.op_Implicit((Object)(object)m_Master));
			InstantiateParticleSystem();
			SetActiveAndPlay();
		}

		private void InstantiateParticleSystem()
		{
			ParticleSystem[] componentsInChildren = ((Component)this).GetComponentsInChildren<ParticleSystem>(true);
			for (int num = componentsInChildren.Length - 1; num >= 0; num--)
			{
				Object.DestroyImmediate((Object)(object)((Component)componentsInChildren[num]).get_gameObject());
			}
			m_Particles = Config.Instance.NewVolumetricDustParticles();
			if (Object.op_Implicit((Object)(object)m_Particles))
			{
				((Component)m_Particles).get_transform().SetParent(((Component)this).get_transform(), false);
				m_Renderer = ((Component)m_Particles).GetComponent<ParticleSystemRenderer>();
			}
		}

		private void OnEnable()
		{
			SetActiveAndPlay();
		}

		private void SetActiveAndPlay()
		{
			if (Object.op_Implicit((Object)(object)m_Particles))
			{
				((Component)m_Particles).get_gameObject().SetActive(true);
				SetParticleProperties();
				m_Particles.Play(true);
			}
		}

		private void OnDisable()
		{
			if (Object.op_Implicit((Object)(object)m_Particles))
			{
				((Component)m_Particles).get_gameObject().SetActive(false);
			}
		}

		private void OnDestroy()
		{
			if (Object.op_Implicit((Object)(object)m_Particles))
			{
				Object.DestroyImmediate((Object)(object)((Component)m_Particles).get_gameObject());
			}
			m_Particles = null;
		}

		private void Update()
		{
			if (Application.get_isPlaying())
			{
				UpdateCulling();
			}
			SetParticleProperties();
		}

		private void SetParticleProperties()
		{
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Expected O, but got Unknown
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			if (!Object.op_Implicit((Object)(object)m_Particles) || !((Component)m_Particles).get_gameObject().get_activeSelf())
			{
				return;
			}
			float num = Mathf.Clamp01(1f - m_Master.fresnelPow / 10f);
			float num2 = m_Master.fadeEnd * spawnMaxDistance;
			float num3 = num2 * density;
			int maxParticles = (int)(num3 * 4f);
			MainModule main = m_Particles.get_main();
			MinMaxCurve startLifetime = ((MainModule)(ref main)).get_startLifetime();
			((MinMaxCurve)(ref startLifetime)).set_mode((ParticleSystemCurveMode)3);
			((MinMaxCurve)(ref startLifetime)).set_constantMin(4f);
			((MinMaxCurve)(ref startLifetime)).set_constantMax(6f);
			((MainModule)(ref main)).set_startLifetime(startLifetime);
			MinMaxCurve startSize = ((MainModule)(ref main)).get_startSize();
			((MinMaxCurve)(ref startSize)).set_mode((ParticleSystemCurveMode)3);
			((MinMaxCurve)(ref startSize)).set_constantMin(size * 0.9f);
			((MinMaxCurve)(ref startSize)).set_constantMax(size * 1.1f);
			((MainModule)(ref main)).set_startSize(startSize);
			MinMaxGradient startColor = ((MainModule)(ref main)).get_startColor();
			if (m_Master.colorMode == ColorMode.Flat)
			{
				((MinMaxGradient)(ref startColor)).set_mode((ParticleSystemGradientMode)0);
				Color color = m_Master.color;
				color.a *= alpha;
				((MinMaxGradient)(ref startColor)).set_color(color);
			}
			else
			{
				((MinMaxGradient)(ref startColor)).set_mode((ParticleSystemGradientMode)1);
				Gradient colorGradient = m_Master.colorGradient;
				GradientColorKey[] colorKeys = colorGradient.get_colorKeys();
				GradientAlphaKey[] alphaKeys = colorGradient.get_alphaKeys();
				for (int i = 0; i < alphaKeys.Length; i++)
				{
					alphaKeys[i].alpha *= alpha;
				}
				Gradient val = new Gradient();
				val.SetKeys(colorKeys, alphaKeys);
				((MinMaxGradient)(ref startColor)).set_gradient(val);
			}
			((MainModule)(ref main)).set_startColor(startColor);
			MinMaxCurve startSpeed = ((MainModule)(ref main)).get_startSpeed();
			((MinMaxCurve)(ref startSpeed)).set_constant(speed);
			((MainModule)(ref main)).set_startSpeed(startSpeed);
			((MainModule)(ref main)).set_maxParticles(maxParticles);
			ShapeModule shape = m_Particles.get_shape();
			((ShapeModule)(ref shape)).set_shapeType((ParticleSystemShapeType)8);
			((ShapeModule)(ref shape)).set_radius(m_Master.coneRadiusStart * Mathf.Lerp(0.3f, 1f, num));
			((ShapeModule)(ref shape)).set_angle(m_Master.coneAngle * 0.5f * Mathf.Lerp(0.7f, 1f, num));
			((ShapeModule)(ref shape)).set_length(num2);
			((ShapeModule)(ref shape)).set_arc(360f);
			((ShapeModule)(ref shape)).set_randomDirectionAmount((direction == Direction.Random) ? 1f : 0f);
			EmissionModule emission = m_Particles.get_emission();
			MinMaxCurve rateOverTime = ((EmissionModule)(ref emission)).get_rateOverTime();
			((MinMaxCurve)(ref rateOverTime)).set_constant(num3);
			((EmissionModule)(ref emission)).set_rateOverTime(rateOverTime);
			if (Object.op_Implicit((Object)(object)m_Renderer))
			{
				((Renderer)m_Renderer).set_sortingLayerID(m_Master.sortingLayerID);
				((Renderer)m_Renderer).set_sortingOrder(m_Master.sortingOrder);
			}
		}

		private void UpdateCulling()
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			if (!Object.op_Implicit((Object)(object)m_Particles))
			{
				return;
			}
			bool flag = true;
			if (cullingEnabled && m_Master.hasGeometry)
			{
				if (Object.op_Implicit((Object)(object)mainCamera))
				{
					float num = cullingMaxDistance * cullingMaxDistance;
					Bounds bounds = m_Master.bounds;
					flag = ((Bounds)(ref bounds)).SqrDistance(((Component)mainCamera).get_transform().get_position()) <= num;
				}
				else
				{
					cullingEnabled = false;
				}
			}
			if (((Component)m_Particles).get_gameObject().get_activeSelf() != flag)
			{
				((Component)m_Particles).get_gameObject().SetActive(flag);
				isCulled = !flag;
			}
			if (flag && !m_Particles.get_isPlaying())
			{
				m_Particles.Play();
			}
		}

		public VolumetricDustParticles()
			: this()
		{
		}
	}
}
