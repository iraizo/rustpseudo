using UnityEngine;
using UnityEngine.Serialization;

namespace VLB
{
	[HelpURL("http://saladgamer.com/vlb-doc/config/")]
	public class Config : ScriptableObject
	{
		public int geometryLayerID = 1;

		public string geometryTag = "Untagged";

		public int geometryRenderQueue = 3000;

		public bool forceSinglePass;

		[SerializeField]
		[HighlightNull]
		private Shader beamShader1Pass;

		[FormerlySerializedAs("BeamShader")]
		[FormerlySerializedAs("beamShader")]
		[SerializeField]
		[HighlightNull]
		private Shader beamShader2Pass;

		public int sharedMeshSides = 24;

		public int sharedMeshSegments = 5;

		[Range(0.01f, 2f)]
		public float globalNoiseScale = 0.5f;

		public Vector3 globalNoiseVelocity = Consts.NoiseVelocityDefault;

		[HighlightNull]
		public TextAsset noise3DData;

		public int noise3DSize = 64;

		[HighlightNull]
		public ParticleSystem dustParticlesPrefab;

		private static Config m_Instance;

		public Shader beamShader
		{
			get
			{
				if (!forceSinglePass)
				{
					return beamShader2Pass;
				}
				return beamShader1Pass;
			}
		}

		public Vector4 globalNoiseParam => new Vector4(globalNoiseVelocity.x, globalNoiseVelocity.y, globalNoiseVelocity.z, globalNoiseScale);

		public static Config Instance
		{
			get
			{
				if ((Object)(object)m_Instance == (Object)null)
				{
					Config[] array = Resources.LoadAll<Config>("Config");
					Debug.Assert(array.Length != 0, $"Can't find any resource of type '{typeof(Config)}'. Make sure you have a ScriptableObject of this type in a 'Resources' folder.");
					m_Instance = array[0];
				}
				return m_Instance;
			}
		}

		public void Reset()
		{
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			geometryLayerID = 1;
			geometryTag = "Untagged";
			geometryRenderQueue = 3000;
			beamShader1Pass = Shader.Find("Hidden/VolumetricLightBeam1Pass");
			beamShader2Pass = Shader.Find("Hidden/VolumetricLightBeam2Pass");
			sharedMeshSides = 24;
			sharedMeshSegments = 5;
			globalNoiseScale = 0.5f;
			globalNoiseVelocity = Consts.NoiseVelocityDefault;
			ref TextAsset val = ref noise3DData;
			Object obj = Resources.Load("Noise3D_64x64x64");
			val = (TextAsset)(object)((obj is TextAsset) ? obj : null);
			noise3DSize = 64;
			ref ParticleSystem val2 = ref dustParticlesPrefab;
			Object obj2 = Resources.Load("DustParticles", typeof(ParticleSystem));
			val2 = (ParticleSystem)(object)((obj2 is ParticleSystem) ? obj2 : null);
		}

		public ParticleSystem NewVolumetricDustParticles()
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			if (!Object.op_Implicit((Object)(object)dustParticlesPrefab))
			{
				if (Application.get_isPlaying())
				{
					Debug.LogError((object)"Failed to instantiate VolumetricDustParticles prefab.");
				}
				return null;
			}
			ParticleSystem obj = Object.Instantiate<ParticleSystem>(dustParticlesPrefab);
			obj.set_useAutoRandomSeed(false);
			((Object)obj).set_name("Dust Particles");
			((Object)((Component)obj).get_gameObject()).set_hideFlags(Consts.ProceduralObjectsHideFlags);
			((Component)obj).get_gameObject().SetActive(true);
			return obj;
		}

		public Config()
			: this()
		{
		}//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)

	}
}
