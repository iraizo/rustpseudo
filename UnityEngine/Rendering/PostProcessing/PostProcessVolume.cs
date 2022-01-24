using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.PostProcessing
{
	[ExecuteAlways]
	[AddComponentMenu("Rendering/Post-process Volume", 1001)]
	public sealed class PostProcessVolume : MonoBehaviour
	{
		public PostProcessProfile sharedProfile;

		[Tooltip("Check this box to mark this volume as global. This volume's Profile will be applied to the whole Scene.")]
		public bool isGlobal;

		[Min(0f)]
		[Tooltip("The distance (from the attached Collider) to start blending from. A value of 0 means there will be no blending and the Volume overrides will be applied immediatly upon entry to the attached Collider.")]
		public float blendDistance;

		[Range(0f, 1f)]
		[Tooltip("The total weight of this Volume in the Scene. A value of 0 signifies that it will have no effect, 1 signifies full effect.")]
		public float weight = 1f;

		[Tooltip("The volume priority in the stack. A higher value means higher priority. Negative values are supported.")]
		public float priority;

		private int m_PreviousLayer;

		private float m_PreviousPriority;

		private List<Collider> m_TempColliders;

		private PostProcessProfile m_InternalProfile;

		public PostProcessProfile profile
		{
			get
			{
				if ((Object)(object)m_InternalProfile == (Object)null)
				{
					m_InternalProfile = ScriptableObject.CreateInstance<PostProcessProfile>();
					if ((Object)(object)sharedProfile != (Object)null)
					{
						foreach (PostProcessEffectSettings setting in sharedProfile.settings)
						{
							PostProcessEffectSettings item = Object.Instantiate<PostProcessEffectSettings>(setting);
							m_InternalProfile.settings.Add(item);
						}
					}
				}
				return m_InternalProfile;
			}
			set
			{
				m_InternalProfile = value;
			}
		}

		internal PostProcessProfile profileRef
		{
			get
			{
				if (!((Object)(object)m_InternalProfile == (Object)null))
				{
					return m_InternalProfile;
				}
				return sharedProfile;
			}
		}

		public bool HasInstantiatedProfile()
		{
			return (Object)(object)m_InternalProfile != (Object)null;
		}

		private void OnEnable()
		{
			PostProcessManager.instance.Register(this);
			m_PreviousLayer = ((Component)this).get_gameObject().get_layer();
			m_TempColliders = new List<Collider>();
		}

		private void OnDisable()
		{
			PostProcessManager.instance.Unregister(this);
		}

		private void Update()
		{
			int layer = ((Component)this).get_gameObject().get_layer();
			if (layer != m_PreviousLayer)
			{
				PostProcessManager.instance.UpdateVolumeLayer(this, m_PreviousLayer, layer);
				m_PreviousLayer = layer;
			}
			if (priority != m_PreviousPriority)
			{
				PostProcessManager.instance.SetLayerDirty(layer);
				m_PreviousPriority = priority;
			}
		}

		private void OnDrawGizmos()
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Expected O, but got Unknown
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Expected O, but got Unknown
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Expected O, but got Unknown
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			List<Collider> tempColliders = m_TempColliders;
			((Component)this).GetComponents<Collider>(tempColliders);
			if (isGlobal || tempColliders == null)
			{
				return;
			}
			Vector3 lossyScale = ((Component)this).get_transform().get_lossyScale();
			Vector3 val = default(Vector3);
			((Vector3)(ref val))._002Ector(1f / lossyScale.x, 1f / lossyScale.y, 1f / lossyScale.z);
			Gizmos.set_matrix(Matrix4x4.TRS(((Component)this).get_transform().get_position(), ((Component)this).get_transform().get_rotation(), lossyScale));
			foreach (Collider item in tempColliders)
			{
				if (!item.get_enabled())
				{
					continue;
				}
				Type type = ((object)item).GetType();
				if (type == typeof(BoxCollider))
				{
					BoxCollider val2 = (BoxCollider)item;
					Gizmos.DrawCube(val2.get_center(), val2.get_size());
					Gizmos.DrawWireCube(val2.get_center(), val2.get_size() + val * blendDistance * 4f);
				}
				else if (type == typeof(SphereCollider))
				{
					SphereCollider val3 = (SphereCollider)item;
					Gizmos.DrawSphere(val3.get_center(), val3.get_radius());
					Gizmos.DrawWireSphere(val3.get_center(), val3.get_radius() + val.x * blendDistance * 2f);
				}
				else if (type == typeof(MeshCollider))
				{
					MeshCollider val4 = (MeshCollider)item;
					if (!val4.get_convex())
					{
						val4.set_convex(true);
					}
					Gizmos.DrawMesh(val4.get_sharedMesh());
					Gizmos.DrawWireMesh(val4.get_sharedMesh(), Vector3.get_zero(), Quaternion.get_identity(), Vector3.get_one() + val * blendDistance * 4f);
				}
			}
			tempColliders.Clear();
		}

		public PostProcessVolume()
			: this()
		{
		}
	}
}
