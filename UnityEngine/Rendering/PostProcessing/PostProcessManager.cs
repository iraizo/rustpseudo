using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace UnityEngine.Rendering.PostProcessing
{
	public sealed class PostProcessManager
	{
		private static PostProcessManager s_Instance;

		private const int k_MaxLayerCount = 32;

		private readonly Dictionary<int, List<PostProcessVolume>> m_SortedVolumes;

		private readonly List<PostProcessVolume> m_Volumes;

		private readonly Dictionary<int, bool> m_SortNeeded;

		private readonly List<PostProcessEffectSettings> m_BaseSettings;

		private readonly List<Collider> m_TempColliders;

		public readonly Dictionary<Type, PostProcessAttribute> settingsTypes;

		public static PostProcessManager instance
		{
			get
			{
				if (s_Instance == null)
				{
					s_Instance = new PostProcessManager();
				}
				return s_Instance;
			}
		}

		private PostProcessManager()
		{
			m_SortedVolumes = new Dictionary<int, List<PostProcessVolume>>();
			m_Volumes = new List<PostProcessVolume>();
			m_SortNeeded = new Dictionary<int, bool>();
			m_BaseSettings = new List<PostProcessEffectSettings>();
			m_TempColliders = new List<Collider>(5);
			settingsTypes = new Dictionary<Type, PostProcessAttribute>();
			ReloadBaseTypes();
		}

		private void CleanBaseTypes()
		{
			settingsTypes.Clear();
			foreach (PostProcessEffectSettings baseSetting in m_BaseSettings)
			{
				RuntimeUtilities.Destroy((Object)(object)baseSetting);
			}
			m_BaseSettings.Clear();
		}

		private void ReloadBaseTypes()
		{
			CleanBaseTypes();
			foreach (Type item in Enumerable.Where<Type>(RuntimeUtilities.GetAllAssemblyTypes(), (Func<Type, bool>)((Type t) => t.IsSubclassOf(typeof(PostProcessEffectSettings)) && t.IsDefined(typeof(PostProcessAttribute), inherit: false) && !t.IsAbstract)))
			{
				settingsTypes.Add(item, item.GetAttribute<PostProcessAttribute>());
				PostProcessEffectSettings postProcessEffectSettings = (PostProcessEffectSettings)(object)ScriptableObject.CreateInstance(item);
				postProcessEffectSettings.SetAllOverridesTo(state: true, excludeEnabled: false);
				m_BaseSettings.Add(postProcessEffectSettings);
			}
		}

		public void GetActiveVolumes(PostProcessLayer layer, List<PostProcessVolume> results, bool skipDisabled = true, bool skipZeroWeight = true)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			int value = ((LayerMask)(ref layer.volumeLayer)).get_value();
			Transform volumeTrigger = layer.volumeTrigger;
			bool flag = (Object)(object)volumeTrigger == (Object)null;
			Vector3 val = (flag ? Vector3.get_zero() : volumeTrigger.get_position());
			foreach (PostProcessVolume item in GrabVolumes(LayerMask.op_Implicit(value)))
			{
				if ((skipDisabled && !((Behaviour)item).get_enabled()) || (Object)(object)item.profileRef == (Object)null || (skipZeroWeight && item.weight <= 0f))
				{
					continue;
				}
				if (item.isGlobal)
				{
					results.Add(item);
				}
				else
				{
					if (flag)
					{
						continue;
					}
					List<Collider> tempColliders = m_TempColliders;
					((Component)item).GetComponents<Collider>(tempColliders);
					if (tempColliders.Count == 0)
					{
						continue;
					}
					float num = float.PositiveInfinity;
					foreach (Collider item2 in tempColliders)
					{
						if (item2.get_enabled())
						{
							Vector3 val2 = (item2.ClosestPoint(val) - val) / 2f;
							float sqrMagnitude = ((Vector3)(ref val2)).get_sqrMagnitude();
							if (sqrMagnitude < num)
							{
								num = sqrMagnitude;
							}
						}
					}
					tempColliders.Clear();
					float num2 = item.blendDistance * item.blendDistance;
					if (num <= num2)
					{
						results.Add(item);
					}
				}
			}
		}

		public PostProcessVolume GetHighestPriorityVolume(PostProcessLayer layer)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)layer == (Object)null)
			{
				throw new ArgumentNullException("layer");
			}
			return GetHighestPriorityVolume(layer.volumeLayer);
		}

		public PostProcessVolume GetHighestPriorityVolume(LayerMask mask)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			float num = float.NegativeInfinity;
			PostProcessVolume result = null;
			if (m_SortedVolumes.TryGetValue(LayerMask.op_Implicit(mask), out var value))
			{
				foreach (PostProcessVolume item in value)
				{
					if (item.priority > num)
					{
						num = item.priority;
						result = item;
					}
				}
				return result;
			}
			return result;
		}

		public PostProcessVolume QuickVolume(int layer, float priority, params PostProcessEffectSettings[] settings)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = new GameObject();
			((Object)val).set_name("Quick Volume");
			val.set_layer(layer);
			((Object)val).set_hideFlags((HideFlags)61);
			PostProcessVolume postProcessVolume = val.AddComponent<PostProcessVolume>();
			postProcessVolume.priority = priority;
			postProcessVolume.isGlobal = true;
			PostProcessProfile profile = postProcessVolume.profile;
			foreach (PostProcessEffectSettings postProcessEffectSettings in settings)
			{
				Assert.IsNotNull<PostProcessEffectSettings>(postProcessEffectSettings, "Trying to create a volume with null effects");
				profile.AddSettings(postProcessEffectSettings);
			}
			return postProcessVolume;
		}

		internal void SetLayerDirty(int layer)
		{
			Assert.IsTrue(layer >= 0 && layer <= 32, "Invalid layer bit");
			foreach (KeyValuePair<int, List<PostProcessVolume>> sortedVolume in m_SortedVolumes)
			{
				int key = sortedVolume.Key;
				if ((key & (1 << layer)) != 0)
				{
					m_SortNeeded[key] = true;
				}
			}
		}

		internal void UpdateVolumeLayer(PostProcessVolume volume, int prevLayer, int newLayer)
		{
			Assert.IsTrue(prevLayer >= 0 && prevLayer <= 32, "Invalid layer bit");
			Unregister(volume, prevLayer);
			Register(volume, newLayer);
		}

		private void Register(PostProcessVolume volume, int layer)
		{
			m_Volumes.Add(volume);
			foreach (KeyValuePair<int, List<PostProcessVolume>> sortedVolume in m_SortedVolumes)
			{
				if ((sortedVolume.Key & (1 << layer)) != 0)
				{
					sortedVolume.Value.Add(volume);
				}
			}
			SetLayerDirty(layer);
		}

		internal void Register(PostProcessVolume volume)
		{
			int layer = ((Component)volume).get_gameObject().get_layer();
			Register(volume, layer);
		}

		private void Unregister(PostProcessVolume volume, int layer)
		{
			m_Volumes.Remove(volume);
			foreach (KeyValuePair<int, List<PostProcessVolume>> sortedVolume in m_SortedVolumes)
			{
				if ((sortedVolume.Key & (1 << layer)) != 0)
				{
					sortedVolume.Value.Remove(volume);
				}
			}
		}

		internal void Unregister(PostProcessVolume volume)
		{
			int layer = ((Component)volume).get_gameObject().get_layer();
			Unregister(volume, layer);
		}

		private void ReplaceData(PostProcessLayer postProcessLayer)
		{
			foreach (PostProcessEffectSettings baseSetting in m_BaseSettings)
			{
				PostProcessEffectSettings settings = postProcessLayer.GetBundle(((object)baseSetting).GetType()).settings;
				int count = baseSetting.parameters.Count;
				for (int i = 0; i < count; i++)
				{
					settings.parameters[i].SetValue(baseSetting.parameters[i]);
				}
			}
		}

		internal void UpdateSettings(PostProcessLayer postProcessLayer, Camera camera)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			ReplaceData(postProcessLayer);
			int value = ((LayerMask)(ref postProcessLayer.volumeLayer)).get_value();
			Transform volumeTrigger = postProcessLayer.volumeTrigger;
			bool flag = (Object)(object)volumeTrigger == (Object)null;
			Vector3 val = (flag ? Vector3.get_zero() : volumeTrigger.get_position());
			foreach (PostProcessVolume item in GrabVolumes(LayerMask.op_Implicit(value)))
			{
				if (!((Behaviour)item).get_enabled() || (Object)(object)item.profileRef == (Object)null || item.weight <= 0f)
				{
					continue;
				}
				List<PostProcessEffectSettings> settings = item.profileRef.settings;
				if (item.isGlobal)
				{
					postProcessLayer.OverrideSettings(settings, Mathf.Clamp01(item.weight));
				}
				else
				{
					if (flag)
					{
						continue;
					}
					List<Collider> tempColliders = m_TempColliders;
					((Component)item).GetComponents<Collider>(tempColliders);
					if (tempColliders.Count == 0)
					{
						continue;
					}
					float num = float.PositiveInfinity;
					foreach (Collider item2 in tempColliders)
					{
						if (item2.get_enabled())
						{
							Vector3 val2 = (item2.ClosestPoint(val) - val) / 2f;
							float sqrMagnitude = ((Vector3)(ref val2)).get_sqrMagnitude();
							if (sqrMagnitude < num)
							{
								num = sqrMagnitude;
							}
						}
					}
					tempColliders.Clear();
					float num2 = item.blendDistance * item.blendDistance;
					if (!(num > num2))
					{
						float num3 = 1f;
						if (num2 > 0f)
						{
							num3 = 1f - num / num2;
						}
						postProcessLayer.OverrideSettings(settings, num3 * Mathf.Clamp01(item.weight));
					}
				}
			}
		}

		private List<PostProcessVolume> GrabVolumes(LayerMask mask)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			if (!m_SortedVolumes.TryGetValue(LayerMask.op_Implicit(mask), out var value))
			{
				value = new List<PostProcessVolume>();
				foreach (PostProcessVolume volume in m_Volumes)
				{
					if ((LayerMask.op_Implicit(mask) & (1 << ((Component)volume).get_gameObject().get_layer())) != 0)
					{
						value.Add(volume);
						m_SortNeeded[LayerMask.op_Implicit(mask)] = true;
					}
				}
				m_SortedVolumes.Add(LayerMask.op_Implicit(mask), value);
			}
			if (m_SortNeeded.TryGetValue(LayerMask.op_Implicit(mask), out var value2) && value2)
			{
				m_SortNeeded[LayerMask.op_Implicit(mask)] = false;
				SortByPriority(value);
			}
			return value;
		}

		private static void SortByPriority(List<PostProcessVolume> volumes)
		{
			Assert.IsNotNull<List<PostProcessVolume>>(volumes, "Trying to sort volumes of non-initialized layer");
			for (int i = 1; i < volumes.Count; i++)
			{
				PostProcessVolume postProcessVolume = volumes[i];
				int num = i - 1;
				while (num >= 0 && volumes[num].priority > postProcessVolume.priority)
				{
					volumes[num + 1] = volumes[num];
					num--;
				}
				volumes[num + 1] = postProcessVolume;
			}
		}

		private static bool IsVolumeRenderedByCamera(PostProcessVolume volume, Camera camera)
		{
			return true;
		}
	}
}
