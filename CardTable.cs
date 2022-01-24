using System;
using ConVar;
using Facepunch;
using Facepunch.CardGames;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class CardTable : BaseVehicle
{
	[Serializable]
	public class ChipStack : IComparable<ChipStack>
	{
		public int chipValue;

		public GameObject[] chips;

		public int CompareTo(ChipStack other)
		{
			if (other == null)
			{
				return 1;
			}
			return chipValue.CompareTo(other.chipValue);
		}
	}

	public enum CardGameOption
	{
		TexasHoldEm
	}

	[Serializable]
	public class PlayerStorageInfo
	{
		public Transform storagePos;

		public EntityRef storageInstance;

		public CardTablePlayerStorage GetStorage()
		{
			BaseEntity baseEntity = storageInstance.Get(serverside: true);
			if ((Object)(object)baseEntity != (Object)null && baseEntity.IsValid())
			{
				return baseEntity as CardTablePlayerStorage;
			}
			return null;
		}
	}

	public EntityRef PotInstance;

	[Header("Card Table")]
	[SerializeField]
	private GameObjectRef uiPrefab;

	[SerializeField]
	private GameObjectRef playerStoragePrefab;

	[SerializeField]
	private GameObjectRef potPrefab;

	[SerializeField]
	private ViewModel viewModel;

	[SerializeField]
	private CardTableUI.PlayingCardImage[] tableCards;

	[SerializeField]
	private Renderer[] tableCardBackings;

	[SerializeField]
	private Canvas cardUICanvas;

	[SerializeField]
	private Image[] tableCardImages;

	[SerializeField]
	private Sprite blankCard;

	[SerializeField]
	private Transform chipStacksParent;

	[SerializeField]
	private ChipStack[] chipStacks;

	[SerializeField]
	private ChipStack[] fillerStacks;

	public ItemDefinition scrapItemDef;

	public PlayerStorageInfo[] playerStoragePoints;

	public CardGameOption gameOption;

	private CardGameController _gameCont;

	private const float MAX_STORAGE_INTERACTION_DIST = 1f;

	protected override bool CanSwapSeats => false;

	public int ScrapItemID => scrapItemDef.itemid;

	public CardGameController GameController
	{
		get
		{
			if (_gameCont == null)
			{
				_gameCont = GetGameController();
			}
			return _gameCont;
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("CardTable.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 2395020190u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_Editor_MakeRandomMove "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_Editor_MakeRandomMove", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(2395020190u, "RPC_Editor_MakeRandomMove", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							RPCMessage msg2 = rPCMessage;
							RPC_Editor_MakeRandomMove(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_Editor_MakeRandomMove");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1608700874 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_Editor_SpawnTestPlayer "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_Editor_SpawnTestPlayer", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1608700874u, "RPC_Editor_SpawnTestPlayer", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							RPCMessage msg3 = rPCMessage;
							RPC_Editor_SpawnTestPlayer(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_Editor_SpawnTestPlayer");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1499640189 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_LeaveTable "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_LeaveTable", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1499640189u, "RPC_LeaveTable", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							RPCMessage msg4 = rPCMessage;
							RPC_LeaveTable(msg4);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in RPC_LeaveTable");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 331989034 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_OpenLoot "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_OpenLoot", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(331989034u, "RPC_OpenLoot", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							RPCMessage msg5 = rPCMessage;
							RPC_OpenLoot(msg5);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in RPC_OpenLoot");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2847205856u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_Play "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_Play", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(2847205856u, "RPC_Play", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							RPCMessage msg6 = rPCMessage;
							RPC_Play(msg6);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex5)
					{
						Debug.LogException(ex5);
						player.Kick("RPC Error in RPC_Play");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2495306863u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_PlayerInput "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_PlayerInput", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(2495306863u, "RPC_PlayerInput", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							RPCMessage msg7 = rPCMessage;
							RPC_PlayerInput(msg7);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex6)
					{
						Debug.LogException(ex6);
						player.Kick("RPC Error in RPC_PlayerInput");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public StorageContainer GetPot()
	{
		BaseEntity baseEntity = PotInstance.Get(serverside: true);
		if ((Object)(object)baseEntity != (Object)null && baseEntity.IsValid())
		{
			return baseEntity as StorageContainer;
		}
		return null;
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.cardTable = Pool.Get<CardTable>();
		info.msg.cardTable.potRef = PotInstance.uid;
		if (!info.forDisk)
		{
			GameController.Save(info.msg.cardTable);
		}
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		int num = 0;
		foreach (BaseEntity child in children)
		{
			CardTablePlayerStorage ent;
			if ((ent = child as CardTablePlayerStorage) != null)
			{
				playerStoragePoints[num].storageInstance.Set(ent);
				num++;
			}
		}
		StorageContainer pot = GetPot();
		if ((Object)(object)pot != (Object)null)
		{
			pot.inventory.Clear();
		}
	}

	public override void SpawnSubEntities()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		base.SpawnSubEntities();
		BaseEntity baseEntity = GameManager.server.CreateEntity(potPrefab.resourcePath, Vector3.get_zero(), Quaternion.get_identity());
		StorageContainer storageContainer = baseEntity as StorageContainer;
		if ((Object)(object)storageContainer != (Object)null)
		{
			storageContainer.SetParent(this);
			storageContainer.Spawn();
			PotInstance.Set(baseEntity);
		}
		else
		{
			Debug.LogError((object)(((object)this).GetType().Name + ": Spawned prefab is not a StorageContainer as expected."));
		}
		PlayerStorageInfo[] array = playerStoragePoints;
		foreach (PlayerStorageInfo playerStorageInfo in array)
		{
			baseEntity = GameManager.server.CreateEntity(playerStoragePrefab.resourcePath, playerStorageInfo.storagePos.get_localPosition(), playerStorageInfo.storagePos.get_localRotation());
			CardTablePlayerStorage cardTablePlayerStorage = baseEntity as CardTablePlayerStorage;
			if ((Object)(object)cardTablePlayerStorage != (Object)null)
			{
				cardTablePlayerStorage.SetCardTable(this);
				cardTablePlayerStorage.SetParent(this);
				cardTablePlayerStorage.Spawn();
				playerStorageInfo.storageInstance.Set(baseEntity);
			}
			else
			{
				Debug.LogError((object)(((object)this).GetType().Name + ": Spawned prefab is not a CardTablePlayerStorage as expected."));
			}
		}
	}

	internal override void DoServerDestroy()
	{
		GameController?.OnTableDestroyed();
		PlayerStorageInfo[] array = playerStoragePoints;
		for (int i = 0; i < array.Length; i++)
		{
			CardTablePlayerStorage storage = array[i].GetStorage();
			if ((Object)(object)storage != (Object)null)
			{
				storage.DropItems();
			}
		}
		StorageContainer pot = GetPot();
		if ((Object)(object)pot != (Object)null)
		{
			pot.DropItems();
		}
		base.DoServerDestroy();
	}

	public override void PlayerDismounted(BasePlayer player, BaseMountable seat)
	{
		base.PlayerDismounted(player, seat);
		GameController.LeaveTable(player.userID);
	}

	public override void PrePlayerDismount(BasePlayer player, BaseMountable seat)
	{
		base.PrePlayerDismount(player, seat);
		CardTablePlayerStorage playerStorage = GetPlayerStorage(player.userID);
		if ((Object)(object)playerStorage != (Object)null)
		{
			playerStorage.inventory.GetSlot(0)?.MoveToContainer(player.inventory.containerMain, -1, allowStack: true, ignoreStackLimit: true);
		}
	}

	public int GetMountPointIndex(ulong playerID)
	{
		int num = -1;
		for (int i = 0; i < mountPoints.Count; i++)
		{
			BaseMountable mountable = mountPoints[i].mountable;
			if ((Object)(object)mountable != (Object)null)
			{
				BasePlayer mounted = mountable.GetMounted();
				if ((Object)(object)mounted != (Object)null && mounted.userID == playerID)
				{
					num = i;
				}
			}
		}
		if (num < 0)
		{
			Debug.LogError((object)(((object)this).GetType().Name + ": Couldn't find mount point for this player."));
		}
		return num;
	}

	public CardTablePlayerStorage GetPlayerStorage(ulong playerID)
	{
		int mountPointIndex = GetMountPointIndex(playerID);
		if (mountPointIndex < 0)
		{
			return null;
		}
		return playerStoragePoints[mountPointIndex].GetStorage();
	}

	public CardTablePlayerStorage GetPlayerStorage(int storageIndex)
	{
		return playerStoragePoints[storageIndex].GetStorage();
	}

	public void PlayerStorageChanged()
	{
		GameController.PlayerStorageChanged();
	}

	public BasePlayer IDToPlayer(ulong id)
	{
		foreach (MountPointInfo mountPoint in mountPoints)
		{
			if ((Object)(object)mountPoint.mountable != (Object)null && (Object)(object)mountPoint.mountable.GetMounted() != (Object)null && mountPoint.mountable.GetMounted().userID == id)
			{
				return mountPoint.mountable.GetMounted();
			}
		}
		return null;
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_Editor_SpawnTestPlayer(RPCMessage msg)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (!Application.get_isEditor())
		{
			return;
		}
		int num = GameController.MaxPlayersAtTable();
		if (GameController.NumPlayersAllowedToPlay() >= num || NumMounted() >= num)
		{
			return;
		}
		Debug.Log((object)"Adding test NPC for card table");
		BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/player/player.prefab", ((Component)this).get_transform().get_position(), Quaternion.get_identity());
		baseEntity.Spawn();
		BasePlayer basePlayer = (BasePlayer)baseEntity;
		AttemptMount(basePlayer, doMountChecks: false);
		GameController.JoinTable(basePlayer);
		if (GameController.TryGetCardPlayerData(basePlayer, out var cardPlayer))
		{
			int scrapAmount = cardPlayer.GetScrapAmount();
			if (scrapAmount < 400)
			{
				StorageContainer storage = cardPlayer.GetStorage();
				if ((Object)(object)storage != (Object)null)
				{
					storage.inventory.AddItem(scrapItemDef, 400 - scrapAmount, 0uL);
				}
				else
				{
					Debug.LogError((object)"Couldn't get storage for NPC.");
				}
			}
		}
		else
		{
			Debug.Log((object)"Couldn't find player data for NPC. No scrap given.");
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_Editor_MakeRandomMove(RPCMessage msg)
	{
		if (Application.get_isEditor())
		{
			GameController.EditorMakeRandomMove();
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	private void RPC_Play(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if ((Object)(object)player != (Object)null && PlayerIsMounted(player))
		{
			GameController.JoinTable(player);
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	private void RPC_OpenLoot(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if ((Object)(object)player != (Object)null && PlayerIsMounted(player))
		{
			GetPlayerStorage(player.userID).PlayerOpenLoot(player);
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	private void RPC_LeaveTable(RPCMessage msg)
	{
		GameController.LeaveTable(msg.player.userID);
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	private void RPC_PlayerInput(RPCMessage msg)
	{
		GameController.ReceivedInputFromPlayer(msg.player, msg.read.Int32(), countAsAction: true, msg.read.Int32());
	}

	public override void DestroyShared()
	{
		base.DestroyShared();
		GameController.Dispose();
	}

	private CardGameController GetGameController()
	{
		CardGameOption cardGameOption = gameOption;
		return new TexasHoldEmController(this);
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (base.isServer)
		{
			PotInstance.uid = info.msg.cardTable.potRef;
		}
	}
}
