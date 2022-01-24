using System;
using System.Collections.Generic;
using System.Linq;
using Network;
using ProtoBuf;
using UnityEngine;

namespace Facepunch.CardGames
{
	public abstract class CardGameController : IDisposable
	{
		public enum CardGameState
		{
			NotPlaying,
			InGameBetweenRounds,
			InGameRound
		}

		public const int IDLE_KICK_SECONDS = 600;

		protected CardPlayerData[] playerData;

		protected CardList localPlayerCards;

		public CardGameState State { get; private set; }

		public bool HasGameInProgress => State >= CardGameState.InGameBetweenRounds;

		public bool HasRoundInProgress => State == CardGameState.InGameRound;

		public abstract int MinPlayers { get; }

		public abstract int MinBuyIn { get; }

		public abstract int MaxBuyIn { get; }

		public virtual float MaxTurnTime => 30f;

		public virtual int TimeBetweenRounds => 8;

		protected CardTable Owner { get; private set; }

		protected int ScrapItemID => Owner.ScrapItemID;

		protected bool IsServer => Owner.isServer;

		protected bool IsClient => Owner.isClient;

		public WinnerBreakdown winnerInfo { get; private set; }

		public CardGameController(CardTable owner)
		{
			Owner = owner;
			playerData = new CardPlayerData[MaxPlayersAtTable()];
			winnerInfo = Pool.Get<WinnerBreakdown>();
			winnerInfo.winners = Pool.GetList<Winner>();
			winnerInfo.winningScore = 0;
			localPlayerCards = Pool.Get<CardList>();
			localPlayerCards.cards = Pool.GetList<int>();
			if (IsServer)
			{
				for (int i = 0; i < playerData.Length; i++)
				{
					playerData[i] = new CardPlayerData(ScrapItemID, owner.GetPlayerStorage, i, IsServer);
				}
			}
		}

		public void Dispose()
		{
			for (int i = 0; i < playerData.Length; i++)
			{
				playerData[i].Dispose();
			}
			localPlayerCards.Dispose();
			winnerInfo.Dispose();
		}

		public int NumPlayersAllowedToPlay(CardPlayerData ignore = null)
		{
			int num = 0;
			CardPlayerData[] array = playerData;
			foreach (CardPlayerData cardPlayerData in array)
			{
				if (cardPlayerData != ignore && IsAllowedToPlay(cardPlayerData))
				{
					num++;
				}
			}
			return num;
		}

		public int RelToAbsIndex(int relIndex, bool includeFolded)
		{
			if (!HasRoundInProgress)
			{
				Debug.LogError((object)(GetType().Name + ": Called RelToAbsIndex outside of a round. No-one is playing. Returning -1."));
				return -1;
			}
			int num = 0;
			for (int i = 0; i < playerData.Length; i++)
			{
				if (includeFolded ? playerData[i].HasUserInGame : playerData[i].HasUserInCurrentRound)
				{
					if (num == relIndex)
					{
						return i;
					}
					num++;
				}
			}
			Debug.LogError((object)$"{GetType().Name}: No absolute index found for relative index {relIndex}. Only {NumPlayersInCurrentRound()} total players are in the round. Returning -1.");
			return -1;
		}

		public int GameToRoundIndex(int gameRelIndex)
		{
			if (!HasRoundInProgress)
			{
				Debug.LogError((object)(GetType().Name + ": Called GameToRoundIndex outside of a round. No-one is playing. Returning 0."));
				return 0;
			}
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < playerData.Length; i++)
			{
				if (playerData[i].HasUserInCurrentRound)
				{
					if (num == gameRelIndex)
					{
						return num2;
					}
					num++;
					num2++;
				}
				else if (playerData[i].HasUserInGame)
				{
					if (num == gameRelIndex)
					{
						return num2;
					}
					num++;
				}
			}
			Debug.LogError((object)$"{GetType().Name}: No round index found for game index {gameRelIndex}. Only {NumPlayersInCurrentRound()} total players are in the round. Returning 0.");
			return 0;
		}

		public int NumPlayersInGame()
		{
			int num = 0;
			CardPlayerData[] array = playerData;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].HasUserInGame)
				{
					num++;
				}
			}
			return num;
		}

		public int NumPlayersInCurrentRound()
		{
			int num = 0;
			CardPlayerData[] array = playerData;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].HasUserInCurrentRound)
				{
					num++;
				}
			}
			return num;
		}

		public int MaxPlayersAtTable()
		{
			return Owner.mountPoints.Count;
		}

		public bool PlayerIsInGame(BasePlayer player)
		{
			return Enumerable.Any<CardPlayerData>((IEnumerable<CardPlayerData>)playerData, (Func<CardPlayerData, bool>)((CardPlayerData data) => data.HasUserInGame && data.UserID == player.userID));
		}

		public bool IsAtTable(BasePlayer player)
		{
			return IsAtTable(player.userID);
		}

		public virtual List<PlayingCard> GetTableCards()
		{
			return null;
		}

		public void StartTurnTimer(float turnTime)
		{
			if (IsServer)
			{
				((FacepunchBehaviour)SingletonComponent<InvokeHandler>.Instance).CancelInvoke((Action)TimeoutTurn);
				((FacepunchBehaviour)SingletonComponent<InvokeHandler>.Instance).Invoke((Action)TimeoutTurn, MaxTurnTime);
				Owner.ClientRPC(null, "ClientStartTurnTimer", turnTime);
			}
		}

		private bool IsAtTable(ulong userID)
		{
			return Enumerable.Any<CardPlayerData>((IEnumerable<CardPlayerData>)playerData, (Func<CardPlayerData, bool>)((CardPlayerData data) => data.UserID == userID));
		}

		public int GetScrapInPot()
		{
			if (IsServer)
			{
				StorageContainer pot = Owner.GetPot();
				if ((Object)(object)pot != (Object)null)
				{
					return pot.inventory.GetAmount(ScrapItemID, onlyUsableAmounts: true);
				}
				return 0;
			}
			return 0;
		}

		public bool TryGetCardPlayerData(int index, out CardPlayerData cardPlayer)
		{
			if (index >= 0 && index < playerData.Length)
			{
				cardPlayer = playerData[index];
				return true;
			}
			cardPlayer = null;
			return false;
		}

		public bool TryGetCardPlayerData(ulong forPlayer, out CardPlayerData cardPlayer)
		{
			CardPlayerData[] array = playerData;
			foreach (CardPlayerData cardPlayerData in array)
			{
				if (cardPlayerData.UserID == forPlayer)
				{
					cardPlayer = cardPlayerData;
					return true;
				}
			}
			cardPlayer = null;
			return false;
		}

		public bool TryGetCardPlayerData(BasePlayer forPlayer, out CardPlayerData cardPlayer)
		{
			for (int i = 0; i < playerData.Length; i++)
			{
				if (playerData[i].UserID == forPlayer.userID)
				{
					cardPlayer = playerData[i];
					return true;
				}
			}
			cardPlayer = null;
			return false;
		}

		public abstract bool IsAllowedToPlay(CardPlayerData cpd);

		protected void ClearWinnerInfo()
		{
			winnerInfo.winningScore = 0;
			if (winnerInfo.winners == null)
			{
				return;
			}
			foreach (Winner winner in winnerInfo.winners)
			{
				if (winner != null)
				{
					winner.Dispose();
				}
			}
			winnerInfo.winners.Clear();
		}

		public void JoinTable(BasePlayer player)
		{
			JoinTable(player.userID);
		}

		protected void SyncAllLocalPlayerCards()
		{
			CardPlayerData[] array = playerData;
			foreach (CardPlayerData cardPlayerData in array)
			{
				BasePlayer basePlayer = BasePlayer.FindByID(cardPlayerData.UserID);
				if (!((Object)(object)basePlayer != (Object)null))
				{
					continue;
				}
				localPlayerCards.cards.Clear();
				foreach (PlayingCard card in cardPlayerData.Cards)
				{
					localPlayerCards.cards.Add(card.GetIndex());
				}
				Owner.ClientRPCPlayer<CardList>(null, basePlayer, "ReceiveCardsForPlayer", localPlayerCards);
			}
		}

		private void JoinTable(ulong userID)
		{
			if (IsAtTable(userID) || NumPlayersAllowedToPlay() >= MaxPlayersAtTable())
			{
				return;
			}
			int mountPointIndex = Owner.GetMountPointIndex(userID);
			if (mountPointIndex < 0)
			{
				return;
			}
			playerData[mountPointIndex].AddUser(userID);
			if (!HasGameInProgress)
			{
				if (!TryStartNewRound())
				{
					Owner.SendNetworkUpdate();
				}
			}
			else
			{
				Owner.SendNetworkUpdate();
			}
		}

		public void LeaveTable(ulong userID)
		{
			if (TryGetCardPlayerData(userID, out var cardPlayer))
			{
				SubOnPlayerLeaving(cardPlayer);
				cardPlayer.ClearAllData();
				if (HasRoundInProgress && NumPlayersInCurrentRound() < MinPlayers)
				{
					EndRound();
				}
				if (cardPlayer.HasUserInGame)
				{
					Owner.ClientRPC(null, "ClientOnPlayerLeft", cardPlayer.UserID);
				}
				Owner.SendNetworkUpdate();
			}
		}

		protected int AddToPot(CardPlayerData playerData, int maxAmount)
		{
			int num = 0;
			StorageContainer storage = playerData.GetStorage();
			StorageContainer pot = Owner.GetPot();
			if ((Object)(object)storage != (Object)null && (Object)(object)pot != (Object)null)
			{
				List<Item> list = Pool.GetList<Item>();
				num = storage.inventory.Take(list, ScrapItemID, maxAmount);
				if (num > 0)
				{
					foreach (Item item in list)
					{
						item.MoveToContainer(pot.inventory, -1, allowStack: true, ignoreStackLimit: true);
					}
				}
				Pool.FreeList<Item>(ref list);
			}
			else
			{
				Debug.LogError((object)(GetType().Name + ": TryAddToPot: Null storage."));
			}
			playerData.betThisRound += num;
			playerData.betThisTurn += num;
			return num;
		}

		protected int AddAllToPot(CardPlayerData playerData)
		{
			return AddToPot(playerData, int.MaxValue);
		}

		protected int PayOut(CardPlayerData playerData, int maxAmount)
		{
			int num = 0;
			StorageContainer storage = playerData.GetStorage();
			StorageContainer pot = Owner.GetPot();
			if ((Object)(object)storage != (Object)null && (Object)(object)pot != (Object)null)
			{
				List<Item> list = Pool.GetList<Item>();
				num = pot.inventory.Take(list, ScrapItemID, maxAmount);
				if (num > 0)
				{
					foreach (Item item in list)
					{
						item.MoveToContainer(storage.inventory, -1, allowStack: true, ignoreStackLimit: true);
					}
				}
				Pool.FreeList<Item>(ref list);
			}
			else
			{
				Debug.LogError((object)(GetType().Name + ": PayOut: Null storage."));
			}
			return num;
		}

		protected int PayOutAll(CardPlayerData playerData)
		{
			return PayOut(playerData, int.MaxValue);
		}

		protected int RemoveScrapFromStorage(CardPlayerData data)
		{
			StorageContainer storage = data.GetStorage();
			BasePlayer basePlayer = BasePlayer.FindByID(data.UserID);
			int num = 0;
			if ((Object)(object)basePlayer != (Object)null)
			{
				List<Item> list = Pool.GetList<Item>();
				num = storage.inventory.Take(list, ScrapItemID, int.MaxValue);
				if (num > 0)
				{
					foreach (Item item in list)
					{
						item.MoveToContainer(basePlayer.inventory.containerMain, -1, allowStack: true, ignoreStackLimit: true);
					}
				}
				Pool.FreeList<Item>(ref list);
			}
			return num;
		}

		public virtual void Save(CardTable syncData)
		{
			syncData.players = Pool.GetList<CardPlayer>();
			CardPlayerData[] array = playerData;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Save(syncData.players);
			}
			syncData.pot = GetScrapInPot();
		}

		private void InvokeStartNewRound()
		{
			TryStartNewRound();
		}

		private bool TryStartNewRound()
		{
			if (HasRoundInProgress)
			{
				return false;
			}
			CardPlayerData[] array = playerData;
			foreach (CardPlayerData cardPlayerData in array)
			{
				BasePlayer basePlayer;
				if (State == CardGameState.NotPlaying)
				{
					cardPlayerData.lastActionTime = Time.get_unscaledTime();
				}
				else if (cardPlayerData.HasBeenIdleFor(600) && BasePlayer.TryFindByID(cardPlayerData.UserID, out basePlayer))
				{
					basePlayer.GetMounted().DismountPlayer(basePlayer);
				}
			}
			if (NumPlayersAllowedToPlay() < MinPlayers)
			{
				EndGameplay();
				return false;
			}
			array = playerData;
			foreach (CardPlayerData cardPlayerData2 in array)
			{
				if (IsAllowedToPlay(cardPlayerData2))
				{
					cardPlayerData2.JoinRound();
				}
				else
				{
					cardPlayerData2.LeaveGame();
				}
			}
			State = CardGameState.InGameRound;
			SubStartRound();
			Owner.SendNetworkUpdate();
			return true;
		}

		protected abstract void TimeoutTurn();

		protected abstract void SubStartRound();

		protected abstract void SubReceivedInputFromPlayer(CardPlayerData playerData, int input, int value, bool countAsAction);

		protected abstract int GetAvailableInputsForPlayer(CardPlayerData playerData);

		protected abstract void SubOnPlayerLeaving(CardPlayerData playerData);

		protected abstract void SubEndRound();

		protected abstract void SubEndGameplay();

		protected void EndRound()
		{
			State = CardGameState.InGameBetweenRounds;
			SubEndRound();
			CardPlayerData[] array = playerData;
			foreach (CardPlayerData cardPlayerData in array)
			{
				if (cardPlayerData.HasUserInCurrentRound)
				{
					BasePlayer basePlayer = BasePlayer.FindByID(cardPlayerData.UserID);
					if ((Object)(object)basePlayer != (Object)null && basePlayer.metabolism.CanConsume())
					{
						basePlayer.metabolism.MarkConsumption();
						basePlayer.metabolism.ApplyChange(MetabolismAttribute.Type.Calories, 2f, 0f);
						basePlayer.metabolism.ApplyChange(MetabolismAttribute.Type.Hydration, 2f, 0f);
					}
				}
				cardPlayerData.LeaveCurrentRound(clearBets: true, leftRoundEarly: false);
			}
			Owner.SendNetworkUpdate();
			((FacepunchBehaviour)Owner).Invoke((Action)InvokeStartNewRound, (float)TimeBetweenRounds);
		}

		private void EndGameplay()
		{
			if (HasGameInProgress)
			{
				SubEndGameplay();
				State = CardGameState.NotPlaying;
				CardPlayerData[] array = playerData;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].LeaveGame();
				}
				SyncAllLocalPlayerCards();
				Owner.SendNetworkUpdate();
			}
		}

		public void ReceivedInputFromPlayer(BasePlayer player, int input, bool countAsAction, int value = 0)
		{
			if (!((Object)(object)player == (Object)null))
			{
				player.ResetInputIdleTime();
				if (TryGetCardPlayerData(player, out var cardPlayer))
				{
					ReceivedInputFromPlayer(cardPlayer, input, countAsAction, value);
				}
			}
		}

		protected void ReceivedInputFromPlayer(CardPlayerData pData, int input, bool countAsAction, int value = 0, bool playerInitiated = true)
		{
			if (HasGameInProgress && pData != null)
			{
				if (playerInitiated)
				{
					pData.lastActionTime = Time.get_unscaledTime();
				}
				SubReceivedInputFromPlayer(pData, input, value, countAsAction);
				UpdateAllAvailableInputs();
				Owner.SendNetworkUpdate();
			}
		}

		protected void UpdateAllAvailableInputs()
		{
			for (int i = 0; i < playerData.Length; i++)
			{
				playerData[i].availableInputs = GetAvailableInputsForPlayer(playerData[i]);
			}
		}

		public void PlayerStorageChanged()
		{
			if (!HasGameInProgress)
			{
				TryStartNewRound();
			}
		}

		protected void ServerPlaySound(CardGameSounds.SoundType type)
		{
			Owner.ClientRPC(null, "ClientPlaySound", (int)type);
		}

		public void GetConnectionsInGame(List<Connection> connections)
		{
			CardPlayerData[] array = playerData;
			foreach (CardPlayerData cardPlayerData in array)
			{
				if (cardPlayerData.HasUserInGame && BasePlayer.TryFindByID(cardPlayerData.UserID, out var basePlayer))
				{
					connections.Add(basePlayer.net.get_connection());
				}
			}
		}

		public virtual void OnTableDestroyed()
		{
			if (((FacepunchBehaviour)SingletonComponent<InvokeHandler>.Instance).IsInvoking((Action)TimeoutTurn))
			{
				((FacepunchBehaviour)SingletonComponent<InvokeHandler>.Instance).CancelInvoke((Action)TimeoutTurn);
			}
		}

		public void EditorMakeRandomMove()
		{
		}
	}
}
