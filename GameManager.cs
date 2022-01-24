using System;
using ConVar;
using Facepunch;
using Rust;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager
{
	public static GameManager server = new GameManager(clientside: false, serverside: true);

	internal PrefabPreProcess preProcessed;

	internal PrefabPoolCollection pool;

	private bool Clientside;

	private bool Serverside;

	public void Reset()
	{
		pool.Clear();
	}

	public GameManager(bool clientside, bool serverside)
	{
		Clientside = clientside;
		Serverside = serverside;
		preProcessed = new PrefabPreProcess(clientside, serverside);
		pool = new PrefabPoolCollection();
	}

	public GameObject FindPrefab(uint prefabID)
	{
		string text = StringPool.Get(prefabID);
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		return FindPrefab(text);
	}

	public GameObject FindPrefab(BaseEntity ent)
	{
		if ((Object)(object)ent == (Object)null)
		{
			return null;
		}
		return FindPrefab(ent.PrefabName);
	}

	public GameObject FindPrefab(string strPrefab)
	{
		GameObject val = preProcessed.Find(strPrefab);
		if ((Object)(object)val != (Object)null)
		{
			return val;
		}
		val = FileSystem.LoadPrefab(strPrefab);
		if ((Object)(object)val == (Object)null)
		{
			return null;
		}
		preProcessed.Process(strPrefab, val);
		GameObject val2 = preProcessed.Find(strPrefab);
		if (!((Object)(object)val2 != (Object)null))
		{
			return val;
		}
		return val2;
	}

	public GameObject CreatePrefab(string strPrefab, Vector3 pos, Quaternion rot, Vector3 scale, bool active = true)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = Instantiate(strPrefab, pos, rot);
		if (Object.op_Implicit((Object)(object)val))
		{
			val.get_transform().set_localScale(scale);
			if (active)
			{
				val.AwakeFromInstantiate();
			}
		}
		return val;
	}

	public GameObject CreatePrefab(string strPrefab, Vector3 pos, Quaternion rot, bool active = true)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = Instantiate(strPrefab, pos, rot);
		if (Object.op_Implicit((Object)(object)val) && active)
		{
			val.AwakeFromInstantiate();
		}
		return val;
	}

	public GameObject CreatePrefab(string strPrefab, bool active = true)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = Instantiate(strPrefab, Vector3.get_zero(), Quaternion.get_identity());
		if (Object.op_Implicit((Object)(object)val) && active)
		{
			val.AwakeFromInstantiate();
		}
		return val;
	}

	public GameObject CreatePrefab(string strPrefab, Transform parent, bool active = true)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = Instantiate(strPrefab, parent.get_position(), parent.get_rotation());
		if (Object.op_Implicit((Object)(object)val))
		{
			val.get_transform().SetParent(parent, false);
			val.Identity();
			if (active)
			{
				val.AwakeFromInstantiate();
			}
		}
		return val;
	}

	public BaseEntity CreateEntity(string strPrefab, Vector3 pos = default(Vector3), Quaternion rot = default(Quaternion), bool startActive = true)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(strPrefab))
		{
			return null;
		}
		GameObject val = CreatePrefab(strPrefab, pos, rot, startActive);
		if ((Object)(object)val == (Object)null)
		{
			return null;
		}
		BaseEntity component = val.GetComponent<BaseEntity>();
		if (!Object.op_Implicit((Object)(object)component))
		{
			Debug.LogError((object)("CreateEntity called on a prefab that isn't an entity! " + strPrefab));
			Object.Destroy((Object)(object)val);
			return null;
		}
		return component;
	}

	private GameObject Instantiate(string strPrefab, Vector3 pos, Quaternion rot)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		if (!StringEx.IsLower(strPrefab))
		{
			Debug.LogWarning((object)("Converting prefab name to lowercase: " + strPrefab));
			strPrefab = strPrefab.ToLower();
		}
		GameObject val = FindPrefab(strPrefab);
		if (!Object.op_Implicit((Object)(object)val))
		{
			Debug.LogError((object)("Couldn't find prefab \"" + strPrefab + "\""));
			return null;
		}
		GameObject val2 = pool.Pop(StringPool.Get(strPrefab), pos, rot);
		if ((Object)(object)val2 == (Object)null)
		{
			val2 = Instantiate.GameObject(val, pos, rot);
			((Object)val2).set_name(strPrefab);
		}
		else
		{
			val2.get_transform().set_localScale(val.get_transform().get_localScale());
		}
		if (!Clientside && Serverside && (Object)(object)val2.get_transform().get_parent() == (Object)null)
		{
			SceneManager.MoveGameObjectToScene(val2, Rust.Server.EntityScene);
		}
		return val2;
	}

	public static void Destroy(Component component, float delay = 0f)
	{
		if ((component as BaseEntity).IsValid())
		{
			Debug.LogError((object)("Trying to destroy an entity without killing it first: " + ((Object)component).get_name()));
		}
		Object.Destroy((Object)(object)component, delay);
	}

	public static void Destroy(GameObject instance, float delay = 0f)
	{
		if (Object.op_Implicit((Object)(object)instance))
		{
			if (instance.GetComponent<BaseEntity>().IsValid())
			{
				Debug.LogError((object)("Trying to destroy an entity without killing it first: " + ((Object)instance).get_name()));
			}
			Object.Destroy((Object)(object)instance, delay);
		}
	}

	public static void DestroyImmediate(Component component, bool allowDestroyingAssets = false)
	{
		if ((component as BaseEntity).IsValid())
		{
			Debug.LogError((object)("Trying to destroy an entity without killing it first: " + ((Object)component).get_name()));
		}
		Object.DestroyImmediate((Object)(object)component, allowDestroyingAssets);
	}

	public static void DestroyImmediate(GameObject instance, bool allowDestroyingAssets = false)
	{
		if (instance.GetComponent<BaseEntity>().IsValid())
		{
			Debug.LogError((object)("Trying to destroy an entity without killing it first: " + ((Object)instance).get_name()));
		}
		Object.DestroyImmediate((Object)(object)instance, allowDestroyingAssets);
	}

	public void Retire(GameObject instance)
	{
		if (!Object.op_Implicit((Object)(object)instance))
		{
			return;
		}
		TimeWarning val = TimeWarning.New("GameManager.Retire", 0);
		try
		{
			if (instance.GetComponent<BaseEntity>().IsValid())
			{
				Debug.LogError((object)("Trying to retire an entity without killing it first: " + ((Object)instance).get_name()));
			}
			if (!Application.isQuitting && Pool.enabled && instance.SupportsPooling())
			{
				pool.Push(instance);
			}
			else
			{
				Object.Destroy((Object)(object)instance);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}
}
