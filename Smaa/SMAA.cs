using UnityEngine;

namespace Smaa
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	[AddComponentMenu("Image Effects/Subpixel Morphological Antialiasing")]
	public class SMAA : MonoBehaviour
	{
		public DebugPass DebugPass;

		public QualityPreset Quality = QualityPreset.High;

		public EdgeDetectionMethod DetectionMethod = EdgeDetectionMethod.Luma;

		public bool UsePredication;

		public Preset CustomPreset;

		public PredicationPreset CustomPredicationPreset;

		public Shader Shader;

		public Texture2D AreaTex;

		public Texture2D SearchTex;

		protected Camera m_Camera;

		protected Preset m_LowPreset;

		protected Preset m_MediumPreset;

		protected Preset m_HighPreset;

		protected Preset m_UltraPreset;

		protected Material m_Material;

		public Material Material
		{
			get
			{
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Expected O, but got Unknown
				if ((Object)(object)m_Material == (Object)null)
				{
					m_Material = new Material(Shader);
					((Object)m_Material).set_hideFlags((HideFlags)61);
				}
				return m_Material;
			}
		}

		public SMAA()
			: this()
		{
		}
	}
}
