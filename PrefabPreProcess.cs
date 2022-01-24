using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using Rust.UI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using VLB;

public class PrefabPreProcess : IPrefabProcessor
{
	public static Type[] clientsideOnlyTypes = new Type[38]
	{
		typeof(IClientComponent),
		typeof(SkeletonSkinLod),
		typeof(ImageEffectLayer),
		typeof(NGSS_Directional),
		typeof(VolumetricDustParticles),
		typeof(VolumetricLightBeam),
		typeof(Cloth),
		typeof(MeshFilter),
		typeof(Renderer),
		typeof(AudioLowPassFilter),
		typeof(AudioSource),
		typeof(AudioListener),
		typeof(ParticleSystemRenderer),
		typeof(ParticleSystem),
		typeof(ParticleEmitFromParentObject),
		typeof(ImpostorShadows),
		typeof(Light),
		typeof(LODGroup),
		typeof(Animator),
		typeof(AnimationEvents),
		typeof(PlayerVoiceSpeaker),
		typeof(VoiceProcessor),
		typeof(PlayerVoiceRecorder),
		typeof(ParticleScaler),
		typeof(PostEffectsBase),
		typeof(TOD_ImageEffect),
		typeof(TOD_Scattering),
		typeof(TOD_Rays),
		typeof(Tree),
		typeof(Projector),
		typeof(HttpImage),
		typeof(EventTrigger),
		typeof(StandaloneInputModule),
		typeof(UIBehaviour),
		typeof(Canvas),
		typeof(CanvasRenderer),
		typeof(CanvasGroup),
		typeof(GraphicRaycaster)
	};

	public static Type[] serversideOnlyTypes = new Type[2]
	{
		typeof(IServerComponent),
		typeof(NavMeshObstacle)
	};

	public bool isClientside;

	public bool isServerside;

	public bool isBundling;

	internal Dictionary<string, GameObject> prefabList = new Dictionary<string, GameObject>(StringComparer.OrdinalIgnoreCase);

	private List<Component> destroyList = new List<Component>();

	private List<GameObject> cleanupList = new List<GameObject>();

	public PrefabPreProcess(bool clientside, bool serverside, bool bundling = false)
	{
		isClientside = clientside;
		isServerside = serverside;
		isBundling = bundling;
	}

	public GameObject Find(string strPrefab)
	{
		if (prefabList.TryGetValue(strPrefab, out var value))
		{
			if ((Object)(object)value == (Object)null)
			{
				prefabList.Remove(strPrefab);
				return null;
			}
			return value;
		}
		return null;
	}

	public bool NeedsProcessing(GameObject go)
	{
		if (go.CompareTag("NoPreProcessing"))
		{
			return false;
		}
		if (HasComponents<IPrefabPreProcess>(go.get_transform()))
		{
			return true;
		}
		if (HasComponents<IPrefabPostProcess>(go.get_transform()))
		{
			return true;
		}
		if (HasComponents<IEditorComponent>(go.get_transform()))
		{
			return true;
		}
		if (!isClientside)
		{
			if (Enumerable.Any<Type>((IEnumerable<Type>)clientsideOnlyTypes, (Func<Type, bool>)((Type type) => HasComponents(go.get_transform(), type))))
			{
				return true;
			}
			if (HasComponents<IClientComponentEx>(go.get_transform()))
			{
				return true;
			}
		}
		if (!isServerside)
		{
			if (Enumerable.Any<Type>((IEnumerable<Type>)serversideOnlyTypes, (Func<Type, bool>)((Type type) => HasComponents(go.get_transform(), type))))
			{
				return true;
			}
			if (HasComponents<IServerComponentEx>(go.get_transform()))
			{
				return true;
			}
		}
		return false;
	}

	public void ProcessObject(string name, GameObject go, bool resetLocalTransform = true)
	{
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		if (!isClientside)
		{
			Type[] array = clientsideOnlyTypes;
			foreach (Type t in array)
			{
				DestroyComponents(t, go, isClientside, isServerside);
			}
			foreach (IClientComponentEx item in FindComponents<IClientComponentEx>(go.get_transform()))
			{
				item.PreClientComponentCull((IPrefabProcessor)(object)this);
			}
		}
		if (!isServerside)
		{
			Type[] array = serversideOnlyTypes;
			foreach (Type t2 in array)
			{
				DestroyComponents(t2, go, isClientside, isServerside);
			}
			foreach (IServerComponentEx item2 in FindComponents<IServerComponentEx>(go.get_transform()))
			{
				item2.PreServerComponentCull((IPrefabProcessor)(object)this);
			}
		}
		DestroyComponents(typeof(IEditorComponent), go, isClientside, isServerside);
		if (resetLocalTransform)
		{
			go.get_transform().set_localPosition(Vector3.get_zero());
			go.get_transform().set_localRotation(Quaternion.get_identity());
		}
		List<Transform> list = FindComponents<Transform>(go.get_transform());
		list.Reverse();
		MeshColliderCookingOptions val = (MeshColliderCookingOptions)14;
		MeshColliderCookingOptions cookingOptions = (MeshColliderCookingOptions)30;
		MeshColliderCookingOptions val2 = (MeshColliderCookingOptions)(-1);
		foreach (MeshCollider item3 in FindComponents<MeshCollider>(go.get_transform()))
		{
			if (item3.get_cookingOptions() == val || item3.get_cookingOptions() == val2)
			{
				item3.set_cookingOptions(cookingOptions);
			}
		}
		foreach (IPrefabPreProcess item4 in FindComponents<IPrefabPreProcess>(go.get_transform()))
		{
			item4.PreProcess((IPrefabProcessor)(object)this, go, name, isServerside, isClientside, isBundling);
		}
		foreach (Transform item5 in list)
		{
			if (!Object.op_Implicit((Object)(object)item5) || !Object.op_Implicit((Object)(object)((Component)item5).get_gameObject()))
			{
				continue;
			}
			if (isServerside && ((Component)item5).get_gameObject().CompareTag("Server Cull"))
			{
				RemoveComponents(((Component)item5).get_gameObject());
				NominateForDeletion(((Component)item5).get_gameObject());
			}
			if (isClientside)
			{
				bool num = ((Component)item5).get_gameObject().CompareTag("Client Cull");
				bool flag = (Object)(object)item5 != (Object)(object)go.get_transform() && (Object)(object)((Component)item5).get_gameObject().GetComponent<BaseEntity>() != (Object)null;
				if (num || flag)
				{
					RemoveComponents(((Component)item5).get_gameObject());
					NominateForDeletion(((Component)item5).get_gameObject());
				}
			}
		}
		RunCleanupQueue();
		foreach (IPrefabPostProcess item6 in FindComponents<IPrefabPostProcess>(go.get_transform()))
		{
			item6.PostProcess((IPrefabProcessor)(object)this, go, name, isServerside, isClientside, isBundling);
		}
	}

	public void Process(string name, GameObject go)
	{
		if (!go.CompareTag("NoPreProcessing"))
		{
			GameObject hierarchyGroup = GetHierarchyGroup();
			GameObject val = go;
			go = Instantiate.GameObject(val, hierarchyGroup.get_transform());
			((Object)go).set_name(((Object)val).get_name());
			if (NeedsProcessing(go))
			{
				ProcessObject(name, go);
			}
			AddPrefab(name, go);
		}
	}

	public void Invalidate(string name)
	{
		if (prefabList.TryGetValue(name, out var value))
		{
			prefabList.Remove(name);
			if ((Object)(object)value != (Object)null)
			{
				Object.DestroyImmediate((Object)(object)value, true);
			}
		}
	}

	public GameObject GetHierarchyGroup()
	{
		if (isClientside && isServerside)
		{
			return HierarchyUtil.GetRoot("PrefabPreProcess - Generic", groupActive: false, persistant: true);
		}
		if (isServerside)
		{
			return HierarchyUtil.GetRoot("PrefabPreProcess - Server", groupActive: false, persistant: true);
		}
		return HierarchyUtil.GetRoot("PrefabPreProcess - Client", groupActive: false, persistant: true);
	}

	public void AddPrefab(string name, GameObject go)
	{
		go.SetActive(false);
		prefabList.Add(name, go);
	}

	private void DestroyComponents(Type t, GameObject go, bool client, bool server)
	{
		List<Component> list = new List<Component>();
		FindComponents(go.get_transform(), list, t);
		list.Reverse();
		foreach (Component item in list)
		{
			RealmedRemove component = item.GetComponent<RealmedRemove>();
			if (!((Object)(object)component != (Object)null) || component.ShouldDelete(item, client, server))
			{
				if (!item.get_gameObject().CompareTag("persist"))
				{
					NominateForDeletion(item.get_gameObject());
				}
				Object.DestroyImmediate((Object)(object)item, true);
			}
		}
	}

	private bool ShouldExclude(Transform transform)
	{
		if ((Object)(object)((Component)transform).GetComponent<BaseEntity>() != (Object)null)
		{
			return true;
		}
		return false;
	}

	private bool HasComponents<T>(Transform transform)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		if (((Component)transform).GetComponent<T>() != null)
		{
			return true;
		}
		foreach (Transform item in transform)
		{
			Transform transform2 = item;
			if (!ShouldExclude(transform2) && HasComponents<T>(transform2))
			{
				return true;
			}
		}
		return false;
	}

	private bool HasComponents(Transform transform, Type t)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		if ((Object)(object)((Component)transform).GetComponent(t) != (Object)null)
		{
			return true;
		}
		foreach (Transform item in transform)
		{
			Transform transform2 = item;
			if (!ShouldExclude(transform2) && HasComponents(transform2, t))
			{
				return true;
			}
		}
		return false;
	}

	public List<T> FindComponents<T>(Transform transform)
	{
		List<T> list = new List<T>();
		FindComponents(transform, list);
		return list;
	}

	public void FindComponents<T>(Transform transform, List<T> list)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		list.AddRange(((Component)transform).GetComponents<T>());
		foreach (Transform item in transform)
		{
			Transform transform2 = item;
			if (!ShouldExclude(transform2))
			{
				FindComponents(transform2, list);
			}
		}
	}

	public List<Component> FindComponents(Transform transform, Type t)
	{
		List<Component> list = new List<Component>();
		FindComponents(transform, list, t);
		return list;
	}

	public void FindComponents(Transform transform, List<Component> list, Type t)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		list.AddRange(((Component)transform).GetComponents(t));
		foreach (Transform item in transform)
		{
			Transform transform2 = item;
			if (!ShouldExclude(transform2))
			{
				FindComponents(transform2, list, t);
			}
		}
	}

	public void RemoveComponent(Component c)
	{
		if (!((Object)(object)c == (Object)null))
		{
			destroyList.Add(c);
		}
	}

	public void RemoveComponents(GameObject gameObj)
	{
		Component[] components = gameObj.GetComponents<Component>();
		foreach (Component val in components)
		{
			if (!(val is Transform))
			{
				destroyList.Add(val);
			}
		}
	}

	public void NominateForDeletion(GameObject gameObj)
	{
		cleanupList.Add(gameObj);
	}

	private void RunCleanupQueue()
	{
		foreach (Component destroy in destroyList)
		{
			Object.DestroyImmediate((Object)(object)destroy, true);
		}
		destroyList.Clear();
		foreach (GameObject cleanup in cleanupList)
		{
			DoCleanup(cleanup);
		}
		cleanupList.Clear();
	}

	private void DoCleanup(GameObject go)
	{
		if (!((Object)(object)go == (Object)null) && go.GetComponentsInChildren<Component>(true).Length <= 1)
		{
			Transform parent = go.get_transform().get_parent();
			if (!((Object)(object)parent == (Object)null) && !((Object)parent).get_name().StartsWith("PrefabPreProcess - "))
			{
				Object.DestroyImmediate((Object)(object)go, true);
			}
		}
	}
}
