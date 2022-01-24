using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class PrefabAttribute : MonoBehaviour, IPrefabPreProcess
{
	public class AttributeCollection
	{
		private Dictionary<Type, List<PrefabAttribute>> attributes = new Dictionary<Type, List<PrefabAttribute>>();

		private Dictionary<Type, object> cache = new Dictionary<Type, object>();

		internal List<PrefabAttribute> Find(Type t)
		{
			if (attributes.TryGetValue(t, out var value))
			{
				return value;
			}
			value = new List<PrefabAttribute>();
			attributes.Add(t, value);
			return value;
		}

		public T[] Find<T>()
		{
			if (cache == null)
			{
				cache = new Dictionary<Type, object>();
			}
			if (cache.TryGetValue(typeof(T), out var value))
			{
				return (T[])value;
			}
			value = Enumerable.ToArray<T>(Enumerable.Cast<T>((IEnumerable)Find(typeof(T))));
			cache.Add(typeof(T), value);
			return (T[])value;
		}

		public void Add(PrefabAttribute attribute)
		{
			List<PrefabAttribute> list = Find(attribute.GetIndexedType());
			Assert.IsTrue(!list.Contains(attribute), "AttributeCollection.Add: Adding twice to list");
			list.Add(attribute);
			cache = null;
		}
	}

	public class Library
	{
		public bool clientside;

		public bool serverside;

		private Dictionary<uint, AttributeCollection> prefabs = new Dictionary<uint, AttributeCollection>();

		public Library(bool clientside, bool serverside)
		{
			this.clientside = clientside;
			this.serverside = serverside;
		}

		public AttributeCollection Find(uint prefabID, bool warmup = true)
		{
			if (prefabs.TryGetValue(prefabID, out var value))
			{
				return value;
			}
			value = new AttributeCollection();
			prefabs.Add(prefabID, value);
			if (warmup && (!clientside || serverside))
			{
				if (!clientside && serverside)
				{
					GameManager.server.FindPrefab(prefabID);
				}
				else if (clientside)
				{
					_ = serverside;
				}
			}
			return value;
		}

		public T Find<T>(uint prefabID) where T : PrefabAttribute
		{
			T[] array = Find(prefabID).Find<T>();
			if (array.Length == 0)
			{
				return null;
			}
			return array[0];
		}

		public T[] FindAll<T>(uint prefabID) where T : PrefabAttribute
		{
			return Find(prefabID).Find<T>();
		}

		public void Add(uint prefabID, PrefabAttribute attribute)
		{
			Find(prefabID, warmup: false).Add(attribute);
		}

		public void Invalidate(uint prefabID)
		{
			prefabs.Remove(prefabID);
		}
	}

	[NonSerialized]
	public Vector3 worldPosition;

	[NonSerialized]
	public Quaternion worldRotation;

	[NonSerialized]
	public Vector3 worldForward;

	[NonSerialized]
	public Vector3 localPosition;

	[NonSerialized]
	public Vector3 localScale;

	[NonSerialized]
	public Quaternion localRotation;

	[NonSerialized]
	public string fullName;

	[NonSerialized]
	public string hierachyName;

	[NonSerialized]
	public uint prefabID;

	[NonSerialized]
	public int instanceID;

	[NonSerialized]
	public Library prefabAttribute;

	[NonSerialized]
	public GameManager gameManager;

	[NonSerialized]
	public bool isServer;

	public static Library server = new Library(clientside: false, serverside: true);

	public bool isClient => !isServer;

	public virtual void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		if (!bundling)
		{
			fullName = name;
			hierachyName = ((Component)this).get_transform().GetRecursiveName();
			prefabID = StringPool.Get(name);
			instanceID = ((Object)this).GetInstanceID();
			worldPosition = ((Component)this).get_transform().get_position();
			worldRotation = ((Component)this).get_transform().get_rotation();
			worldForward = ((Component)this).get_transform().get_forward();
			localPosition = ((Component)this).get_transform().get_localPosition();
			localScale = ((Component)this).get_transform().get_localScale();
			localRotation = ((Component)this).get_transform().get_localRotation();
			if (serverside)
			{
				prefabAttribute = server;
				gameManager = GameManager.server;
				isServer = true;
			}
			AttributeSetup(rootObj, name, serverside, clientside, bundling);
			if (serverside)
			{
				server.Add(prefabID, this);
			}
			preProcess.RemoveComponent((Component)(object)this);
			preProcess.NominateForDeletion(((Component)this).get_gameObject());
		}
	}

	protected virtual void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
	}

	protected abstract Type GetIndexedType();

	public static bool operator ==(PrefabAttribute x, PrefabAttribute y)
	{
		return ComparePrefabAttribute(x, y);
	}

	public static bool operator !=(PrefabAttribute x, PrefabAttribute y)
	{
		return !ComparePrefabAttribute(x, y);
	}

	public override bool Equals(object o)
	{
		PrefabAttribute y;
		if ((object)(y = o as PrefabAttribute) != null)
		{
			return ComparePrefabAttribute(this, y);
		}
		return false;
	}

	public override int GetHashCode()
	{
		if (hierachyName == null)
		{
			return ((Object)this).GetHashCode();
		}
		return hierachyName.GetHashCode();
	}

	public static implicit operator bool(PrefabAttribute exists)
	{
		return (object)exists != null;
	}

	internal static bool ComparePrefabAttribute(PrefabAttribute x, PrefabAttribute y)
	{
		bool flag = (object)x == null;
		bool flag2 = (object)y == null;
		if (flag && flag2)
		{
			return true;
		}
		if (flag || flag2)
		{
			return false;
		}
		return x.instanceID == y.instanceID;
	}

	public override string ToString()
	{
		if ((object)this == null)
		{
			return "null";
		}
		return hierachyName;
	}

	protected PrefabAttribute()
		: this()
	{
	}
}
