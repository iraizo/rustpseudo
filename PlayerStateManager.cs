using System;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;

public class PlayerStateManager
{
	private readonly MruDictionary<ulong, PlayerState> _cache;

	private readonly UserPersistance _persistence;

	public PlayerStateManager(UserPersistance persistence)
	{
		_cache = new MruDictionary<ulong, PlayerState>(1000, (Action<ulong, PlayerState>)FreeOldState);
		_persistence = persistence;
	}

	public PlayerState Get(ulong playerId)
	{
		TimeWarning val = TimeWarning.New("PlayerStateManager.Get", 0);
		try
		{
			PlayerState result = default(PlayerState);
			if (_cache.TryGetValue(playerId, ref result))
			{
				return result;
			}
			byte[] playerState = _persistence.GetPlayerState(playerId);
			PlayerState val2;
			if (playerState != null && playerState.Length != 0)
			{
				try
				{
					val2 = PlayerState.Deserialize(playerState);
					OnPlayerStateLoaded(val2);
					_cache.Add(playerId, val2);
					return val2;
				}
				catch (Exception arg)
				{
					Debug.LogError((object)$"Failed to load player state for {playerId}: {arg}");
				}
			}
			val2 = Pool.Get<PlayerState>();
			_cache.Add(playerId, val2);
			return val2;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void Save(ulong playerId)
	{
		PlayerState state = default(PlayerState);
		if (_cache.TryGetValue(playerId, ref state))
		{
			SaveState(playerId, state);
		}
	}

	private void SaveState(ulong playerId, PlayerState state)
	{
		TimeWarning val = TimeWarning.New("PlayerStateManager.SaveState", 0);
		try
		{
			byte[] state2 = PlayerState.SerializeToBytes(state);
			_persistence.SetPlayerState(playerId, state2);
		}
		catch (Exception arg)
		{
			Debug.LogError((object)$"Failed to save player state for {playerId}: {arg}");
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void FreeOldState(ulong playerId, PlayerState state)
	{
		SaveState(playerId, state);
		state.Dispose();
	}

	public void Reset(ulong playerId)
	{
		_cache.Remove(playerId);
		_persistence.ResetPlayerState(playerId);
	}

	private void OnPlayerStateLoaded(PlayerState state)
	{
		state.unHostileTimestamp = Math.Min(state.unHostileTimestamp, TimeEx.get_currentTimestamp() + 1800.0);
	}
}
