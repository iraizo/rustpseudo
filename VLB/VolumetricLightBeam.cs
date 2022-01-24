using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace VLB
{
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[SelectionBase]
	[HelpURL("http://saladgamer.com/vlb-doc/comp-lightbeam/")]
	public class VolumetricLightBeam : MonoBehaviour
	{
		public bool colorFromLight = true;

		public ColorMode colorMode;

		[ColorUsage(true, true)]
		[FormerlySerializedAs("colorValue")]
		public Color color = Consts.FlatColor;

		public Gradient colorGradient;

		[Range(0f, 1f)]
		public float alphaInside = 1f;

		[FormerlySerializedAs("alpha")]
		[Range(0f, 1f)]
		public float alphaOutside = 1f;

		public BlendingMode blendingMode;

		[FormerlySerializedAs("angleFromLight")]
		public bool spotAngleFromLight = true;

		[Range(0.1f, 179.9f)]
		public float spotAngle = 35f;

		[FormerlySerializedAs("radiusStart")]
		public float coneRadiusStart = 0.1f;

		public MeshType geomMeshType;

		[FormerlySerializedAs("geomSides")]
		public int geomCustomSides = 18;

		public int geomCustomSegments = 5;

		public bool geomCap;

		public bool fadeEndFromLight = true;

		public AttenuationEquation attenuationEquation = AttenuationEquation.Quadratic;

		[Range(0f, 1f)]
		public float attenuationCustomBlending = 0.5f;

		public float fadeStart;

		public float fadeEnd = 3f;

		public float depthBlendDistance = 2f;

		public float cameraClippingDistance = 0.5f;

		[Range(0f, 1f)]
		public float glareFrontal = 0.5f;

		[Range(0f, 1f)]
		public float glareBehind = 0.5f;

		[Obsolete("Use 'glareFrontal' instead")]
		public float boostDistanceInside = 0.5f;

		[Obsolete("This property has been merged with 'fresnelPow'")]
		public float fresnelPowInside = 6f;

		[FormerlySerializedAs("fresnelPowOutside")]
		public float fresnelPow = 8f;

		public bool noiseEnabled;

		[Range(0f, 1f)]
		public float noiseIntensity = 0.5f;

		public bool noiseScaleUseGlobal = true;

		[Range(0.01f, 2f)]
		public float noiseScaleLocal = 0.5f;

		public bool noiseVelocityUseGlobal = true;

		public Vector3 noiseVelocityLocal = Consts.NoiseVelocityDefault;

		private Plane m_PlaneWS;

		[SerializeField]
		private int pluginVersion = -1;

		[FormerlySerializedAs("trackChangesDuringPlaytime")]
		[SerializeField]
		private bool _TrackChangesDuringPlaytime;

		[SerializeField]
		private int _SortingLayerID;

		[SerializeField]
		private int _SortingOrder;

		private BeamGeometry m_BeamGeom;

		private Coroutine m_CoPlaytimeUpdate;

		private Light _CachedLight;

		public float coneAngle => Mathf.Atan2(coneRadiusEnd - coneRadiusStart, fadeEnd) * 57.29578f * 2f;

		public float coneRadiusEnd => fadeEnd * Mathf.Tan(spotAngle * ((float)Math.PI / 180f) * 0.5f);

		public float coneVolume
		{
			get
			{
				float num = coneRadiusStart;
				float num2 = coneRadiusEnd;
				return (float)Math.PI / 3f * (num * num + num * num2 + num2 * num2) * fadeEnd;
			}
		}

		public float coneApexOffsetZ
		{
			get
			{
				float num = coneRadiusStart / coneRadiusEnd;
				if (num != 1f)
				{
					return fadeEnd * num / (1f - num);
				}
				return float.MaxValue;
			}
		}

		public int geomSides
		{
			get
			{
				if (geomMeshType != MeshType.Custom)
				{
					return Config.Instance.sharedMeshSides;
				}
				return geomCustomSides;
			}
			set
			{
				geomCustomSides = value;
				Debug.LogWarning((object)"The setter VLB.VolumetricLightBeam.geomSides is OBSOLETE and has been renamed to geomCustomSides.");
			}
		}

		public int geomSegments
		{
			get
			{
				if (geomMeshType != MeshType.Custom)
				{
					return Config.Instance.sharedMeshSegments;
				}
				return geomCustomSegments;
			}
			set
			{
				geomCustomSegments = value;
				Debug.LogWarning((object)"The setter VLB.VolumetricLightBeam.geomSegments is OBSOLETE and has been renamed to geomCustomSegments.");
			}
		}

		public float attenuationLerpLinearQuad
		{
			get
			{
				if (attenuationEquation == AttenuationEquation.Linear)
				{
					return 0f;
				}
				if (attenuationEquation == AttenuationEquation.Quadratic)
				{
					return 1f;
				}
				return attenuationCustomBlending;
			}
		}

		public int sortingLayerID
		{
			get
			{
				return _SortingLayerID;
			}
			set
			{
				_SortingLayerID = value;
				if (Object.op_Implicit((Object)(object)m_BeamGeom))
				{
					m_BeamGeom.sortingLayerID = value;
				}
			}
		}

		public string sortingLayerName
		{
			get
			{
				return SortingLayer.IDToName(sortingLayerID);
			}
			set
			{
				sortingLayerID = SortingLayer.NameToID(value);
			}
		}

		public int sortingOrder
		{
			get
			{
				return _SortingOrder;
			}
			set
			{
				_SortingOrder = value;
				if (Object.op_Implicit((Object)(object)m_BeamGeom))
				{
					m_BeamGeom.sortingOrder = value;
				}
			}
		}

		public bool trackChangesDuringPlaytime
		{
			get
			{
				return _TrackChangesDuringPlaytime;
			}
			set
			{
				_TrackChangesDuringPlaytime = value;
				StartPlaytimeUpdateIfNeeded();
			}
		}

		public bool isCurrentlyTrackingChanges => m_CoPlaytimeUpdate != null;

		public bool hasGeometry => (Object)(object)m_BeamGeom != (Object)null;

		public Bounds bounds
		{
			get
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				if (!((Object)(object)m_BeamGeom != (Object)null))
				{
					return new Bounds(Vector3.get_zero(), Vector3.get_zero());
				}
				return ((Renderer)m_BeamGeom.meshRenderer).get_bounds();
			}
		}

		public int blendingModeAsInt => Mathf.Clamp((int)blendingMode, 0, Enum.GetValues(typeof(BlendingMode)).Length);

		public MeshRenderer Renderer
		{
			get
			{
				if (!((Object)(object)m_BeamGeom != (Object)null))
				{
					return null;
				}
				return m_BeamGeom.meshRenderer;
			}
		}

		public string meshStats
		{
			get
			{
				Mesh val = (Object.op_Implicit((Object)(object)m_BeamGeom) ? m_BeamGeom.coneMesh : null);
				if (Object.op_Implicit((Object)(object)val))
				{
					return $"Cone angle: {coneAngle:0.0} degrees\nMesh: {val.get_vertexCount()} vertices, {val.get_triangles().Length / 3} triangles";
				}
				return "no mesh available";
			}
		}

		public int meshVerticesCount
		{
			get
			{
				if (!Object.op_Implicit((Object)(object)m_BeamGeom) || !Object.op_Implicit((Object)(object)m_BeamGeom.coneMesh))
				{
					return 0;
				}
				return m_BeamGeom.coneMesh.get_vertexCount();
			}
		}

		public int meshTrianglesCount
		{
			get
			{
				if (!Object.op_Implicit((Object)(object)m_BeamGeom) || !Object.op_Implicit((Object)(object)m_BeamGeom.coneMesh))
				{
					return 0;
				}
				return m_BeamGeom.coneMesh.get_triangles().Length / 3;
			}
		}

		private Light lightSpotAttached
		{
			get
			{
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				if ((Object)(object)_CachedLight == (Object)null)
				{
					_CachedLight = ((Component)this).GetComponent<Light>();
				}
				if (Object.op_Implicit((Object)(object)_CachedLight) && (int)_CachedLight.get_type() == 0)
				{
					return _CachedLight;
				}
				return null;
			}
		}

		public void SetClippingPlane(Plane planeWS)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			if (Object.op_Implicit((Object)(object)m_BeamGeom))
			{
				m_BeamGeom.SetClippingPlane(planeWS);
			}
			m_PlaneWS = planeWS;
		}

		public void SetClippingPlaneOff()
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			if (Object.op_Implicit((Object)(object)m_BeamGeom))
			{
				m_BeamGeom.SetClippingPlaneOff();
			}
			m_PlaneWS = default(Plane);
		}

		public bool IsColliderHiddenByDynamicOccluder(Collider collider)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			Debug.Assert(Object.op_Implicit((Object)(object)collider), "You should pass a valid Collider to VLB.VolumetricLightBeam.IsColliderHiddenByDynamicOccluder");
			if (!m_PlaneWS.IsValid())
			{
				return false;
			}
			return !GeometryUtility.TestPlanesAABB((Plane[])(object)new Plane[1] { m_PlaneWS }, collider.get_bounds());
		}

		public float GetInsideBeamFactor(Vector3 posWS)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			return GetInsideBeamFactorFromObjectSpacePos(((Component)this).get_transform().InverseTransformPoint(posWS));
		}

		public float GetInsideBeamFactorFromObjectSpacePos(Vector3 posOS)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			if (posOS.z < 0f)
			{
				return -1f;
			}
			Vector2 val = posOS.xy();
			val = new Vector2(((Vector2)(ref val)).get_magnitude(), posOS.z + coneApexOffsetZ);
			Vector2 normalized = ((Vector2)(ref val)).get_normalized();
			return Mathf.Clamp((Mathf.Abs(Mathf.Sin(coneAngle * ((float)Math.PI / 180f) / 2f)) - Mathf.Abs(normalized.x)) / 0.1f, -1f, 1f);
		}

		[Obsolete("Use 'GenerateGeometry()' instead")]
		public void Generate()
		{
			GenerateGeometry();
		}

		public virtual void GenerateGeometry()
		{
			HandleBackwardCompatibility(pluginVersion, 1510);
			pluginVersion = 1510;
			ValidateProperties();
			if ((Object)(object)m_BeamGeom == (Object)null)
			{
				Shader beamShader = Config.Instance.beamShader;
				if (!Object.op_Implicit((Object)(object)beamShader))
				{
					Debug.LogError((object)"Invalid BeamShader set in VLB Config");
					return;
				}
				m_BeamGeom = Utils.NewWithComponent<BeamGeometry>("Beam Geometry");
				m_BeamGeom.Initialize(this, beamShader);
			}
			m_BeamGeom.RegenerateMesh();
			m_BeamGeom.visible = ((Behaviour)this).get_enabled();
		}

		public virtual void UpdateAfterManualPropertyChange()
		{
			ValidateProperties();
			if (Object.op_Implicit((Object)(object)m_BeamGeom))
			{
				m_BeamGeom.UpdateMaterialAndBounds();
			}
		}

		private void Start()
		{
			GenerateGeometry();
		}

		private void OnEnable()
		{
			if (Object.op_Implicit((Object)(object)m_BeamGeom))
			{
				m_BeamGeom.visible = true;
			}
			StartPlaytimeUpdateIfNeeded();
		}

		private void OnDisable()
		{
			if (Object.op_Implicit((Object)(object)m_BeamGeom))
			{
				m_BeamGeom.visible = false;
			}
			m_CoPlaytimeUpdate = null;
		}

		private void StartPlaytimeUpdateIfNeeded()
		{
		}

		private IEnumerator CoPlaytimeUpdate()
		{
			while (trackChangesDuringPlaytime && ((Behaviour)this).get_enabled())
			{
				UpdateAfterManualPropertyChange();
				yield return null;
			}
			m_CoPlaytimeUpdate = null;
		}

		private void OnDestroy()
		{
			DestroyBeam();
		}

		private void DestroyBeam()
		{
			if (Object.op_Implicit((Object)(object)m_BeamGeom))
			{
				Object.DestroyImmediate((Object)(object)((Component)m_BeamGeom).get_gameObject());
			}
			m_BeamGeom = null;
		}

		private void AssignPropertiesFromSpotLight(Light lightSpot)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			if (Object.op_Implicit((Object)(object)lightSpot) && (int)lightSpot.get_type() == 0)
			{
				if (fadeEndFromLight)
				{
					fadeEnd = lightSpot.get_range();
				}
				if (spotAngleFromLight)
				{
					spotAngle = lightSpot.get_spotAngle();
				}
				if (colorFromLight)
				{
					colorMode = ColorMode.Flat;
					color = lightSpot.get_color();
				}
			}
		}

		private void ClampProperties()
		{
			alphaInside = Mathf.Clamp01(alphaInside);
			alphaOutside = Mathf.Clamp01(alphaOutside);
			attenuationCustomBlending = Mathf.Clamp01(attenuationCustomBlending);
			fadeEnd = Mathf.Max(0.01f, fadeEnd);
			fadeStart = Mathf.Clamp(fadeStart, 0f, fadeEnd - 0.01f);
			spotAngle = Mathf.Clamp(spotAngle, 0.1f, 179.9f);
			coneRadiusStart = Mathf.Max(coneRadiusStart, 0f);
			depthBlendDistance = Mathf.Max(depthBlendDistance, 0f);
			cameraClippingDistance = Mathf.Max(cameraClippingDistance, 0f);
			geomCustomSides = Mathf.Clamp(geomCustomSides, 3, 256);
			geomCustomSegments = Mathf.Clamp(geomCustomSegments, 0, 64);
			fresnelPow = Mathf.Max(0f, fresnelPow);
			glareBehind = Mathf.Clamp01(glareBehind);
			glareFrontal = Mathf.Clamp01(glareFrontal);
			noiseIntensity = Mathf.Clamp(noiseIntensity, 0f, 1f);
		}

		private void ValidateProperties()
		{
			AssignPropertiesFromSpotLight(lightSpotAttached);
			ClampProperties();
		}

		private void HandleBackwardCompatibility(int serializedVersion, int newVersion)
		{
			if (serializedVersion != -1 && serializedVersion != newVersion)
			{
				if (serializedVersion < 1301)
				{
					attenuationEquation = AttenuationEquation.Linear;
				}
				if (serializedVersion < 1501)
				{
					geomMeshType = MeshType.Custom;
					geomCustomSegments = 5;
				}
				Utils.MarkCurrentSceneDirty();
			}
		}

		public VolumetricLightBeam()
			: this()
		{
		}//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)

	}
}
