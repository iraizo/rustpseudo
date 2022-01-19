using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ConVar;
using Facepunch;
using Network;
using Network.Visibility;
using ProtoBuf;
using Rust;
using Rust.Registry;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class BaseNetworkable : BaseMonoBehaviour, IPrefabPostProcess, IEntity, NetworkHandler
{
	public struct SaveInfo
	{
		public Entity msg;

		public bool forDisk;

		public bool forTransfer;

		public Connection forConnection;

		internal bool SendingTo(Connection ownerConnection)
		{
			if (ownerConnection == null)
			{
				return false;
			}
			if (forConnection == null)
			{
				return false;
			}
			return forConnection == ownerConnection;
		}
	}

	public struct LoadInfo
	{
		public Entity msg;

		public bool fromDisk;

		public bool fromTransfer;
	}

	public class EntityRealmServer : EntityRealm
	{
		protected override Manager visibilityManager
		{
			get
			{
				if (Net.sv == null)
				{
					return null;
				}
				return Net.sv.visibility;
			}
		}
	}

	public abstract class EntityRealm : IEnumerable<BaseNetworkable>, IEnumerable
	{
		private ListDictionary<uint, BaseNetworkable> entityList = new ListDictionary<uint, BaseNetworkable>();

		public int Count => entityList.get_Count();

		protected abstract Manager visibilityManager { get; }

		public bool Contains(uint uid)
		{
			return entityList.Contains(uid);
		}

		public BaseNetworkable Find(uint uid)
		{
			BaseNetworkable result = null;
			if (!entityList.TryGetValue(uid, ref result))
			{
				return null;
			}
			return result;
		}

		public void RegisterID(BaseNetworkable ent)
		{
			if (ent.net != null)
			{
				if (entityList.Contains(ent.net.ID))
				{
					entityList.set_Item(ent.net.ID, ent);
				}
				else
				{
					entityList.Add(ent.net.ID, ent);
				}
			}
		}

		public void UnregisterID(BaseNetworkable ent)
		{
			if (ent.net != null)
			{
				entityList.Remove(ent.net.ID);
			}
		}

		public Group FindGroup(uint uid)
		{
			Manager val = visibilityManager;
			if (val == null)
			{
				return null;
			}
			return val.Get(uid);
		}

		public Group TryFindGroup(uint uid)
		{
			Manager val = visibilityManager;
			if (val == null)
			{
				return null;
			}
			return val.TryGet(uid);
		}

		public void FindInGroup(uint uid, List<BaseNetworkable> list)
		{
			Group val = TryFindGroup(uid);
			if (val == null)
			{
				return;
			}
			int count = val.networkables.get_Values().get_Count();
			Networkable[] buffer = val.networkables.get_Values().get_Buffer();
			for (int i = 0; i < count; i++)
			{
				Networkable val2 = buffer[i];
				BaseNetworkable baseNetworkable = Find(val2.ID);
				if (!((Object)(object)baseNetworkable == (Object)null) && baseNetworkable.net != null && baseNetworkable.net.group != null)
				{
					if (baseNetworkable.net.group.ID != uid)
					{
						Debug.LogWarning((object)("Group ID mismatch: " + ((object)baseNetworkable).ToString()));
					}
					else
					{
						list.Add(baseNetworkable);
					}
				}
			}
		}

		public Enumerator<BaseNetworkable> GetEnumerator()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return entityList.get_Values().GetEnumerator();
		}

		IEnumerator<BaseNetworkable> IEnumerable<BaseNetworkable>.GetEnumerator()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return (IEnumerator<BaseNetworkable>)(object)GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return (IEnumerator)(object)GetEnumerator();
		}

		public void Clear()
		{
			entityList.Clear();
		}
	}

	public enum DestroyMode : byte
	{
		None,
		Gib
	}

	public List<Component> postNetworkUpdateComponents = new List<Component>();

	private bool _limitedNetworking;

	[NonSerialized]
	public EntityRef parentEntity;

	[NonSerialized]
	public readonly List<BaseEntity> children = new List<BaseEntity>();

	private int creationFrame;

	protected bool isSpawned;

	private MemoryStream _NetworkCache;

	public static Queue<MemoryStream> EntityMemoryStreamPool = new Queue<MemoryStream>();

	private MemoryStream _SaveCache;

	[Header("BaseNetworkable")]
	[ReadOnly]
	public uint prefabID;

	[Tooltip("If enabled the entity will send to everyone on the server - regardless of position")]
	public bool globalBroadcast;

	[NonSerialized]
	public Networkable net;

	private string _prefabName;

	private string _prefabNameWithoutExtension;

	public static EntityRealm serverEntities = new EntityRealmServer();

	private const bool isServersideEntity = true;

	private static List<Connection> connectionsInSphereList = new List<Connection>();

	public bool limitNetworking
	{
		get
		{
			return _limitedNetworking;
		}
		set
		{
			if (value != _limitedNetworking)
			{
				_limitedNetworking = value;
				if (_limitedNetworking)
				{
					OnNetworkLimitStart();
				}
				else
				{
					OnNetworkLimitEnd();
				}
				UpdateNetworkGroup();
			}
		}
	}

	public GameManager gameManager
	{
		get
		{
			if (isServer)
			{
				return GameManager.server;
			}
			throw new NotImplementedException("Missing gameManager path");
		}
	}

	public PrefabAttribute.Library prefabAttribute
	{
		get
		{
			if (isServer)
			{
				return PrefabAttribute.server;
			}
			throw new NotImplementedException("Missing prefabAttribute path");
		}
	}

	public static Group GlobalNetworkGroup => Net.sv.visibility.Get(0u);

	public static Group LimboNetworkGroup => Net.sv.visibility.Get(1u);

	public bool IsDestroyed { get; private set; }

	public string PrefabName
	{
		get
		{
			if (_prefabName == null)
			{
				_prefabName = StringPool.Get(prefabID);
			}
			return _prefabName;
		}
	}

	public string ShortPrefabName
	{
		get
		{
			if (_prefabNameWithoutExtension == null)
			{
				_prefabNameWithoutExtension = Path.GetFileNameWithoutExtension(PrefabName);
			}
			return _prefabNameWithoutExtension;
		}
	}

	public bool isServer => true;

	public bool isClient => false;

	public void BroadcastOnPostNetworkUpdate(BaseEntity entity)
	{
		foreach (Component postNetworkUpdateComponent in postNetworkUpdateComponents)
		{
			(postNetworkUpdateComponent as IOnPostNetworkUpdate)?.OnPostNetworkUpdate(entity);
		}
		foreach (BaseEntity child in children)
		{
			child.BroadcastOnPostNetworkUpdate(entity);
		}
	}

	public virtual void PostProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (!serverside)
		{
			postNetworkUpdateComponents = ((Component)this).GetComponentsInChildren<IOnPostNetworkUpdate>(true).Cast<Component>().ToList();
		}
	}

	private void OnNetworkLimitStart()
	{
		LogEntry(LogEntryType.Network, 2, "OnNetworkLimitStart");
		List<Connection> subscribers = GetSubscribers();
		if (subscribers == null)
		{
			return;
		}
		subscribers = subscribers.ToList();
		subscribers.RemoveAll((Connection x) => ShouldNetworkTo(x.player as BasePlayer));
		OnNetworkSubscribersLeave(subscribers);
		if (children == null)
		{
			return;
		}
		foreach (BaseEntity child in children)
		{
			child.OnNetworkLimitStart();
		}
	}

	private void OnNetworkLimitEnd()
	{
		LogEntry(LogEntryType.Network, 2, "OnNetworkLimitEnd");
		List<Connection> subscribers = GetSubscribers();
		if (subscribers == null)
		{
			return;
		}
		OnNetworkSubscribersEnter(subscribers);
		if (children == null)
		{
			return;
		}
		foreach (BaseEntity child in children)
		{
			child.OnNetworkLimitEnd();
		}
	}

	public BaseEntity GetParentEntity()
	{
		return parentEntity.Get(isServer);
	}

	public bool HasParent()
	{
		return parentEntity.IsValid(isServer);
	}

	public void AddChild(BaseEntity child)
	{
		if (!children.Contains(child))
		{
			children.Add(child);
			OnChildAdded(child);
		}
	}

	protected virtual void OnChildAdded(BaseEntity child)
	{
	}

	public void RemoveChild(BaseEntity child)
	{
		children.Remove(child);
		OnChildRemoved(child);
	}

	protected virtual void OnChildRemoved(BaseEntity child)
	{
	}

	public virtual float GetNetworkTime()
	{
		return Time.get_time();
	}

	public virtual void Spawn()
	{
		SpawnShared();
		if (net == null)
		{
			net = Net.sv.CreateNetworkable();
		}
		creationFrame = Time.get_frameCount();
		PreInitShared();
		InitShared();
		ServerInit();
		PostInitShared();
		UpdateNetworkGroup();
		isSpawned = true;
		SendNetworkUpdateImmediate(justCreated: true);
		if (Application.isLoading && !Application.isLoadingSave)
		{
			((Component)this).get_gameObject().SendOnSendNetworkUpdate(this as BaseEntity);
		}
	}

	public bool IsFullySpawned()
	{
		return isSpawned;
	}

	public virtual void ServerInit()
	{
		serverEntities.RegisterID(this);
		if (net != null)
		{
			net.handler = (NetworkHandler)(object)this;
		}
	}

	protected List<Connection> GetSubscribers()
	{
		if (net == null)
		{
			return null;
		}
		if (net.group == null)
		{
			return null;
		}
		return net.group.subscribers;
	}

	public void KillMessage()
	{
		Kill();
	}

	public virtual void AdminKill()
	{
		Kill(DestroyMode.Gib);
	}

	public void Kill(DestroyMode mode = DestroyMode.None)
	{
		if (IsDestroyed)
		{
			Debug.LogWarning((object)("Calling kill - but already IsDestroyed!? " + this));
			return;
		}
		((Component)this).get_gameObject().BroadcastOnParentDestroying();
		DoEntityDestroy();
		TerminateOnClient(mode);
		TerminateOnServer();
		EntityDestroy();
	}

	private void TerminateOnClient(DestroyMode mode)
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		if (net != null && net.group != null && Net.sv.IsConnected())
		{
			LogEntry(LogEntryType.Network, 2, "Term {0}", mode);
			if (((BaseNetwork)Net.sv).get_write().Start())
			{
				((BaseNetwork)Net.sv).get_write().PacketID((Type)6);
				((BaseNetwork)Net.sv).get_write().EntityID(net.ID);
				((BaseNetwork)Net.sv).get_write().UInt8((byte)mode);
				((BaseNetwork)Net.sv).get_write().Send(new SendInfo(net.group.subscribers));
			}
		}
	}

	private void TerminateOnServer()
	{
		if (net != null)
		{
			InvalidateNetworkCache();
			serverEntities.UnregisterID(this);
			Net.sv.DestroyNetworkable(ref net);
			((MonoBehaviour)this).StopAllCoroutines();
			((Component)this).get_gameObject().SetActive(false);
		}
	}

	internal virtual void DoServerDestroy()
	{
		isSpawned = false;
	}

	public virtual bool ShouldNetworkTo(BasePlayer player)
	{
		if (net.group == null)
		{
			return true;
		}
		return player.net.subscriber.IsSubscribed(net.group);
	}

	protected void SendNetworkGroupChange()
	{
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		if (isSpawned && Net.sv.IsConnected())
		{
			if (net.group == null)
			{
				Debug.LogWarning((object)(((object)this).ToString() + " changed its network group to null"));
			}
			else if (((BaseNetwork)Net.sv).get_write().Start())
			{
				((BaseNetwork)Net.sv).get_write().PacketID((Type)7);
				((BaseNetwork)Net.sv).get_write().EntityID(net.ID);
				((BaseNetwork)Net.sv).get_write().GroupID(net.group.ID);
				((BaseNetwork)Net.sv).get_write().Send(new SendInfo(net.group.subscribers));
			}
		}
	}

	protected void SendAsSnapshot(Connection connection, bool justCreated = false)
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).get_write().Start())
		{
			connection.validate.entityUpdates++;
			SaveInfo saveInfo = default(SaveInfo);
			saveInfo.forConnection = connection;
			saveInfo.forDisk = false;
			SaveInfo saveInfo2 = saveInfo;
			((BaseNetwork)Net.sv).get_write().PacketID((Type)5);
			((BaseNetwork)Net.sv).get_write().UInt32(connection.validate.entityUpdates);
			ToStreamForNetwork((Stream)(object)((BaseNetwork)Net.sv).get_write(), saveInfo2);
			((BaseNetwork)Net.sv).get_write().Send(new SendInfo(connection));
		}
	}

	public void SendNetworkUpdate(BasePlayer.NetworkQueue queue = BasePlayer.NetworkQueue.Update)
	{
		if (Application.isLoading || Application.isLoadingSave || IsDestroyed || net == null || !isSpawned)
		{
			return;
		}
		TimeWarning val = TimeWarning.New("SendNetworkUpdate", 0);
		try
		{
			LogEntry(LogEntryType.Network, 2, "SendNetworkUpdate");
			InvalidateNetworkCache();
			List<Connection> subscribers = GetSubscribers();
			if (subscribers != null && subscribers.Count > 0)
			{
				for (int i = 0; i < subscribers.Count; i++)
				{
					BasePlayer basePlayer = subscribers[i].player as BasePlayer;
					if (!((Object)(object)basePlayer == (Object)null) && ShouldNetworkTo(basePlayer))
					{
						basePlayer.QueueUpdate(queue, this);
					}
				}
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		((Component)this).get_gameObject().SendOnSendNetworkUpdate(this as BaseEntity);
	}

	public void SendNetworkUpdateImmediate(bool justCreated = false)
	{
		if (Application.isLoading || Application.isLoadingSave || IsDestroyed || net == null || !isSpawned)
		{
			return;
		}
		TimeWarning val = TimeWarning.New("SendNetworkUpdateImmediate", 0);
		try
		{
			LogEntry(LogEntryType.Network, 2, "SendNetworkUpdateImmediate");
			InvalidateNetworkCache();
			List<Connection> subscribers = GetSubscribers();
			if (subscribers != null && subscribers.Count > 0)
			{
				for (int i = 0; i < subscribers.Count; i++)
				{
					Connection val2 = subscribers[i];
					BasePlayer basePlayer = val2.player as BasePlayer;
					if (!((Object)(object)basePlayer == (Object)null) && ShouldNetworkTo(basePlayer))
					{
						SendAsSnapshot(val2, justCreated);
					}
				}
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		((Component)this).get_gameObject().SendOnSendNetworkUpdate(this as BaseEntity);
	}

	protected void SendNetworkUpdate_Position()
	{
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		if (Application.isLoading || Application.isLoadingSave || IsDestroyed || net == null || !isSpawned)
		{
			return;
		}
		TimeWarning val = TimeWarning.New("SendNetworkUpdate_Position", 0);
		try
		{
			LogEntry(LogEntryType.Network, 2, "SendNetworkUpdate_Position");
			List<Connection> subscribers = GetSubscribers();
			if (subscribers != null && subscribers.Count > 0 && ((BaseNetwork)Net.sv).get_write().Start())
			{
				((BaseNetwork)Net.sv).get_write().PacketID((Type)10);
				((BaseNetwork)Net.sv).get_write().EntityID(net.ID);
				NetWrite write = ((BaseNetwork)Net.sv).get_write();
				Vector3 networkPosition = GetNetworkPosition();
				write.Vector3(ref networkPosition);
				NetWrite write2 = ((BaseNetwork)Net.sv).get_write();
				Quaternion networkRotation = GetNetworkRotation();
				networkPosition = ((Quaternion)(ref networkRotation)).get_eulerAngles();
				write2.Vector3(ref networkPosition);
				((BaseNetwork)Net.sv).get_write().Float(GetNetworkTime());
				uint uid = parentEntity.uid;
				if (uid != 0)
				{
					((BaseNetwork)Net.sv).get_write().EntityID(uid);
				}
				SendInfo val2 = new SendInfo(subscribers);
				val2.method = (SendMethod)1;
				val2.priority = (Priority)0;
				SendInfo val3 = val2;
				((BaseNetwork)Net.sv).get_write().Send(val3);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void ToStream(Stream stream, SaveInfo saveInfo)
	{
		Entity val = (saveInfo.msg = Pool.Get<Entity>());
		try
		{
			Save(saveInfo);
			if (saveInfo.msg.baseEntity == null)
			{
				Debug.LogError((object)string.Concat(this, ": ToStream - no BaseEntity!?"));
			}
			if (saveInfo.msg.baseNetworkable == null)
			{
				Debug.LogError((object)string.Concat(this, ": ToStream - no baseNetworkable!?"));
			}
			saveInfo.msg.ToProto(stream);
			PostSave(saveInfo);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public virtual bool CanUseNetworkCache(Connection connection)
	{
		return ConVar.Server.netcache;
	}

	public void ToStreamForNetwork(Stream stream, SaveInfo saveInfo)
	{
		if (!CanUseNetworkCache(saveInfo.forConnection))
		{
			ToStream(stream, saveInfo);
			return;
		}
		if (_NetworkCache == null)
		{
			_NetworkCache = ((EntityMemoryStreamPool.Count > 0) ? (_NetworkCache = EntityMemoryStreamPool.Dequeue()) : new MemoryStream(8));
			ToStream(_NetworkCache, saveInfo);
			ConVar.Server.netcachesize += (int)_NetworkCache.Length;
		}
		_NetworkCache.WriteTo(stream);
	}

	public void InvalidateNetworkCache()
	{
		TimeWarning val = TimeWarning.New("InvalidateNetworkCache", 0);
		try
		{
			if (_SaveCache != null)
			{
				ConVar.Server.savecachesize -= (int)_SaveCache.Length;
				_SaveCache.SetLength(0L);
				_SaveCache.Position = 0L;
				EntityMemoryStreamPool.Enqueue(_SaveCache);
				_SaveCache = null;
			}
			if (_NetworkCache != null)
			{
				ConVar.Server.netcachesize -= (int)_NetworkCache.Length;
				_NetworkCache.SetLength(0L);
				_NetworkCache.Position = 0L;
				EntityMemoryStreamPool.Enqueue(_NetworkCache);
				_NetworkCache = null;
			}
			LogEntry(LogEntryType.Network, 3, "InvalidateNetworkCache");
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public MemoryStream GetSaveCache()
	{
		if (_SaveCache == null)
		{
			if (EntityMemoryStreamPool.Count > 0)
			{
				_SaveCache = EntityMemoryStreamPool.Dequeue();
			}
			else
			{
				_SaveCache = new MemoryStream(8);
			}
			SaveInfo saveInfo = default(SaveInfo);
			saveInfo.forDisk = true;
			SaveInfo saveInfo2 = saveInfo;
			ToStream(_SaveCache, saveInfo2);
			ConVar.Server.savecachesize += (int)_SaveCache.Length;
		}
		return _SaveCache;
	}

	public virtual void UpdateNetworkGroup()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		Assert.IsTrue(isServer, "UpdateNetworkGroup called on clientside entity!");
		if (net == null)
		{
			return;
		}
		TimeWarning val = TimeWarning.New("UpdateGroups", 0);
		try
		{
			if (net.UpdateGroups(((Component)this).get_transform().get_position()))
			{
				SendNetworkGroupChange();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public virtual Vector3 GetNetworkPosition()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).get_transform().get_localPosition();
	}

	public virtual Quaternion GetNetworkRotation()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).get_transform().get_localRotation();
	}

	public string InvokeString()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		StringBuilder stringBuilder = new StringBuilder();
		List<InvokeAction> list = Pool.GetList<InvokeAction>();
		InvokeHandler.FindInvokes((Behaviour)(object)this, list);
		foreach (InvokeAction item in list)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append(", ");
			}
			stringBuilder.Append(item.action.Method.Name);
		}
		Pool.FreeList<InvokeAction>(ref list);
		return stringBuilder.ToString();
	}

	public BaseEntity LookupPrefab()
	{
		return gameManager.FindPrefab(PrefabName).ToBaseEntity();
	}

	public bool EqualNetID(BaseNetworkable other)
	{
		if ((Object)(object)other != (Object)null && other.net != null && net != null)
		{
			return other.net.ID == net.ID;
		}
		return false;
	}

	public bool EqualNetID(uint otherID)
	{
		if (net != null)
		{
			return otherID == net.ID;
		}
		return false;
	}

	public virtual void ResetState()
	{
		if (children.Count > 0)
		{
			children.Clear();
		}
	}

	public virtual void InitShared()
	{
	}

	public virtual void PreInitShared()
	{
	}

	public virtual void PostInitShared()
	{
	}

	public virtual void DestroyShared()
	{
	}

	public virtual void OnNetworkGroupEnter(Group group)
	{
	}

	public virtual void OnNetworkGroupLeave(Group group)
	{
	}

	public void OnNetworkGroupChange()
	{
		if (children == null)
		{
			return;
		}
		foreach (BaseEntity child in children)
		{
			if (child.ShouldInheritNetworkGroup())
			{
				child.net.SwitchGroup(net.group);
			}
			else if (isServer)
			{
				child.UpdateNetworkGroup();
			}
		}
	}

	public void OnNetworkSubscribersEnter(List<Connection> connections)
	{
		if (!Net.sv.IsConnected())
		{
			return;
		}
		foreach (Connection connection in connections)
		{
			BasePlayer basePlayer = connection.player as BasePlayer;
			if (!((Object)(object)basePlayer == (Object)null))
			{
				basePlayer.QueueUpdate(BasePlayer.NetworkQueue.Update, this as BaseEntity);
			}
		}
	}

	public void OnNetworkSubscribersLeave(List<Connection> connections)
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (Net.sv.IsConnected())
		{
			LogEntry(LogEntryType.Network, 2, "LeaveVisibility");
			if (((BaseNetwork)Net.sv).get_write().Start())
			{
				((BaseNetwork)Net.sv).get_write().PacketID((Type)6);
				((BaseNetwork)Net.sv).get_write().EntityID(net.ID);
				((BaseNetwork)Net.sv).get_write().UInt8((byte)0);
				((BaseNetwork)Net.sv).get_write().Send(new SendInfo(connections));
			}
		}
	}

	private void EntityDestroy()
	{
		if (Object.op_Implicit((Object)(object)((Component)this).get_gameObject()))
		{
			ResetState();
			gameManager.Retire(((Component)this).get_gameObject());
		}
	}

	private void DoEntityDestroy()
	{
		if (IsDestroyed)
		{
			return;
		}
		IsDestroyed = true;
		if (!Application.isQuitting)
		{
			DestroyShared();
			if (isServer)
			{
				DoServerDestroy();
			}
			TimeWarning val = TimeWarning.New("Registry.Entity.Unregister", 0);
			try
			{
				Entity.Unregister(((Component)this).get_gameObject());
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
	}

	private void SpawnShared()
	{
		IsDestroyed = false;
		TimeWarning val = TimeWarning.New("Registry.Entity.Register", 0);
		try
		{
			Entity.Register(((Component)this).get_gameObject(), (IEntity)(object)this);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public virtual void Save(SaveInfo info)
	{
		if (prefabID == 0)
		{
			Debug.LogError((object)("PrefabID is 0! " + ((Component)this).get_transform().GetRecursiveName()), (Object)(object)((Component)this).get_gameObject());
		}
		info.msg.baseNetworkable = Pool.Get<BaseNetworkable>();
		info.msg.baseNetworkable.uid = net.ID;
		info.msg.baseNetworkable.prefabID = prefabID;
		if (net.group != null)
		{
			info.msg.baseNetworkable.group = net.group.ID;
		}
		if (!info.forDisk)
		{
			info.msg.createdThisFrame = creationFrame == Time.get_frameCount();
		}
	}

	public virtual void PostSave(SaveInfo info)
	{
	}

	public void InitLoad(uint entityID)
	{
		net = Net.sv.CreateNetworkable(entityID);
		serverEntities.RegisterID(this);
		PreServerLoad();
	}

	public virtual void PreServerLoad()
	{
	}

	public virtual void Load(LoadInfo info)
	{
		if (info.msg.baseNetworkable != null)
		{
			BaseNetworkable baseNetworkable = info.msg.baseNetworkable;
			if (prefabID != baseNetworkable.prefabID)
			{
				Debug.LogError((object)("Prefab IDs don't match! " + prefabID + "/" + baseNetworkable.prefabID + " -> " + ((Component)this).get_gameObject()), (Object)(object)((Component)this).get_gameObject());
			}
		}
	}

	public virtual void PostServerLoad()
	{
		((Component)this).get_gameObject().SendOnSendNetworkUpdate(this as BaseEntity);
	}

	public T ToServer<T>() where T : BaseNetworkable
	{
		if (isServer)
		{
			return this as T;
		}
		return null;
	}

	public virtual bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		return false;
	}

	public static List<Connection> GetConnectionsWithin(Vector3 position, float distance)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		connectionsInSphereList.Clear();
		float num = distance * distance;
		List<Connection> subscribers = GlobalNetworkGroup.subscribers;
		for (int i = 0; i < subscribers.Count; i++)
		{
			Connection val = subscribers[i];
			if (val.active)
			{
				BasePlayer basePlayer = val.player as BasePlayer;
				if (!((Object)(object)basePlayer == (Object)null) && !(basePlayer.SqrDistance(position) > num))
				{
					connectionsInSphereList.Add(val);
				}
			}
		}
		return connectionsInSphereList;
	}

	public static void GetCloseConnections(Vector3 position, float distance, List<BasePlayer> players)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (Net.sv == null || Net.sv.visibility == null)
		{
			return;
		}
		float num = distance * distance;
		Group group = Net.sv.visibility.GetGroup(position);
		if (group == null)
		{
			return;
		}
		List<Connection> subscribers = group.subscribers;
		for (int i = 0; i < subscribers.Count; i++)
		{
			Connection val = subscribers[i];
			if (val.active)
			{
				BasePlayer basePlayer = val.player as BasePlayer;
				if (!((Object)(object)basePlayer == (Object)null) && !(basePlayer.SqrDistance(position) > num))
				{
					players.Add(basePlayer);
				}
			}
		}
	}

	public static bool HasCloseConnections(Vector3 position, float distance)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (Net.sv == null)
		{
			return false;
		}
		if (Net.sv.visibility == null)
		{
			return false;
		}
		float num = distance * distance;
		Group group = Net.sv.visibility.GetGroup(position);
		if (group == null)
		{
			return false;
		}
		List<Connection> subscribers = group.subscribers;
		for (int i = 0; i < subscribers.Count; i++)
		{
			Connection val = subscribers[i];
			if (val.active)
			{
				BasePlayer basePlayer = val.player as BasePlayer;
				if (!((Object)(object)basePlayer == (Object)null) && !(basePlayer.SqrDistance(position) > num))
				{
					return true;
				}
			}
		}
		return false;
	}
}
