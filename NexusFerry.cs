using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using Facepunch.Extend;
using Facepunch.Nexus.Models;
using ProtoBuf;
using UnityEngine;

public class NexusFerry : BaseEntity
{
	public enum State
	{
		Invalid,
		Arrival,
		Docking,
		Stopping,
		Waiting,
		CastingOff,
		Departure,
		Transferring
	}

	public float MoveSpeed = 10f;

	public float TurnSpeed = 1f;

	public float WaitTime = 90f;

	private List<string> _schedule;

	private int _scheduleIndex;

	private State _state;

	private NexusDock _targetDock;

	private bool _isTransferring;

	private TimeSince _sinceStartedWaiting;

	private TimeSince _sinceLastTransferAttempt;

	protected override bool PositionTickFixedTime => true;

	public override void ServerInit()
	{
		base.ServerInit();
		if (!NexusServer.Started)
		{
			Debug.LogError((object)"NexusFerry will not work without being connected to a nexus! Destroying it");
			Kill();
		}
	}

	public void FixedUpdate()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (!base.isServer)
		{
			return;
		}
		if (_state == State.Waiting)
		{
			if (!(TimeSince.op_Implicit(_sinceStartedWaiting) >= WaitTime))
			{
				return;
			}
			SwitchToNextState();
		}
		if (MoveTowardsTarget())
		{
			SwitchToNextState();
		}
	}

	public override float GetNetworkTime()
	{
		return Time.get_fixedTime();
	}

	private bool MoveTowardsTarget()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		Transform targetTransform = GetTargetTransform(_state);
		Vector3 position = targetTransform.get_position();
		Quaternion rotation = targetTransform.get_rotation();
		Vector3 position2 = ((Component)this).get_transform().get_position();
		position.y = position2.y;
		Vector3 val = default(Vector3);
		float num = default(float);
		Vector3Ex.ToDirectionAndMagnitude(position - position2, ref val, ref num);
		float num2 = MoveSpeed * Time.get_deltaTime();
		float num3 = Mathf.Min(num2, num);
		Vector3 val2 = position2 + val * num3;
		Quaternion rotation2 = ((Component)this).get_transform().get_rotation();
		State previousState = GetPreviousState(_state);
		Quaternion val3;
		if (previousState != 0)
		{
			Transform targetTransform2 = GetTargetTransform(previousState);
			Vector3 position3 = targetTransform2.get_position();
			Quaternion rotation3 = targetTransform2.get_rotation();
			position3.y = position2.y;
			float num4 = Vector3Ex.Distance2D(position3, position);
			val3 = Quaternion.Slerp(rotation, rotation3, num / num4);
		}
		else
		{
			val3 = Quaternion.Slerp(rotation2, rotation, TurnSpeed * Time.get_deltaTime());
		}
		((Component)this).get_transform().SetPositionAndRotation(val2, val3);
		return num3 < num2;
	}

	private void SwitchToNextState()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (_state == State.Departure)
		{
			if (!_isTransferring)
			{
				TransferToNextZone();
			}
			return;
		}
		_state = GetNextState(_state);
		if (_state == State.Waiting)
		{
			_sinceStartedWaiting = TimeSince.op_Implicit(0f);
		}
		if (_state == State.CastingOff)
		{
			EjectLoadingPlayers();
		}
	}

	private static State GetNextState(State currentState)
	{
		State state = currentState + 1;
		if (state >= State.Departure)
		{
			state = State.Departure;
		}
		return state;
	}

	private static State GetPreviousState(State currentState)
	{
		if (currentState != 0)
		{
			return currentState - 1;
		}
		return State.Invalid;
	}

	private Transform GetTargetTransform(State state)
	{
		EnsureInitialized();
		switch (state)
		{
		case State.Arrival:
			return _targetDock.Arrival;
		case State.Docking:
			return _targetDock.Docking;
		case State.Stopping:
		case State.Waiting:
			return _targetDock.Docked;
		case State.CastingOff:
			return _targetDock.CastingOff;
		case State.Departure:
			return _targetDock.Departure;
		default:
			return ((Component)this).get_transform();
		}
	}

	private async void TransferToNextZone()
	{
		if (_isTransferring || TimeSince.op_Implicit(_sinceLastTransferAttempt) < 5f)
		{
			return;
		}
		_isTransferring = true;
		_sinceLastTransferAttempt = TimeSince.op_Implicit(0f);
		int oldScheduleIndex = _scheduleIndex;
		State oldState = _state;
		try
		{
			EnsureInitialized();
			_scheduleIndex++;
			if (_scheduleIndex >= _schedule.Count)
			{
				_scheduleIndex = 0;
			}
			string text = _schedule[_scheduleIndex];
			_state = State.Transferring;
			Debug.Log((object)("Sending ferry to " + text));
			await NexusServer.TransferEntity(this, text, "ferry");
		}
		finally
		{
			_isTransferring = false;
			_scheduleIndex = oldScheduleIndex;
			_state = oldState;
		}
	}

	private void EnsureInitialized()
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)_targetDock == (Object)null)
		{
			_targetDock = SingletonComponent<NexusDock>.Instance;
			if ((Object)(object)_targetDock == (Object)null)
			{
				Debug.LogError((object)"NexusFerry has no dock to go to!");
				Kill();
				return;
			}
		}
		if (_state == State.Invalid)
		{
			_state = State.Stopping;
			Transform targetTransform = GetTargetTransform(_state);
			((Component)this).get_transform().SetPositionAndRotation(targetTransform.get_position(), targetTransform.get_rotation());
		}
		if (_schedule != null)
		{
			_ = _schedule.Count;
			_ = 0;
		}
		if (_schedule == null || _schedule.Count == 0)
		{
			Debug.Log((object)"Ferry has no schedule set - generating one now");
			_schedule = Enumerable.ToList<string>(Enumerable.Select<NexusZoneDetails, string>((IEnumerable<NexusZoneDetails>)NexusServer.Zones, (Func<NexusZoneDetails, string>)((NexusZoneDetails z) => z.get_Name())));
			_scheduleIndex = _schedule.IndexOf(NexusServer.ZoneName);
		}
	}

	private void EjectLoadingPlayers()
	{
		if ((Object)(object)_targetDock == (Object)null)
		{
			return;
		}
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		foreach (BaseEntity child in children)
		{
			bool flag = false;
			BasePlayer player;
			BaseVehicle baseVehicle;
			if ((player = child as BasePlayer) != null)
			{
				flag = !IsPlayerReady(player);
			}
			else if ((baseVehicle = child as BaseVehicle) != null)
			{
				List<BasePlayer> list2 = Pool.GetList<BasePlayer>();
				baseVehicle.GetDrivers(list2);
				flag = list2.Count > 0;
				foreach (BasePlayer item in list2)
				{
					if (IsPlayerReady(item))
					{
						flag = false;
						break;
					}
				}
				Pool.FreeList<BasePlayer>(ref list2);
			}
			if (flag)
			{
				list.Add(child);
			}
		}
		foreach (BaseEntity item2 in list)
		{
			EjectEntity(item2);
		}
		Pool.FreeList<BaseEntity>(ref list);
	}

	private void EjectEntity(BaseEntity entity)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)entity == (Object)null))
		{
			if (_targetDock.TryFindEjectionPosition(out var position))
			{
				Debug.Log((object)$"Kicking {entity} off the ferry", (Object)(object)entity);
				entity.SetParent(null);
				entity.ServerPosition = position;
				entity.SendNetworkUpdateImmediate();
			}
			else
			{
				Debug.LogWarning((object)$"Couldn't find an ejection point for {entity}", (Object)(object)entity);
			}
		}
	}

	private static bool IsPlayerReady(BasePlayer player)
	{
		if ((Object)(object)player != (Object)null && player.IsConnected)
		{
			return !player.IsLoadingAfterTransfer();
		}
		return false;
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		((FacepunchBehaviour)this).Invoke((Action)DisableTransferProtection, 0.1f);
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.nexusFerry = Pool.Get<NexusFerry>();
		info.msg.nexusFerry.schedule = List.ShallowClonePooled<string>(_schedule);
		info.msg.nexusFerry.scheduleIndex = _scheduleIndex;
		info.msg.nexusFerry.state = (int)_state;
	}

	public override void Load(LoadInfo info)
	{
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.nexusFerry == null)
		{
			return;
		}
		if (_schedule != null)
		{
			Pool.FreeList<string>(ref _schedule);
		}
		_schedule = info.msg.nexusFerry.schedule;
		info.msg.nexusFerry.schedule = null;
		_scheduleIndex = info.msg.nexusFerry.scheduleIndex;
		_state = (State)info.msg.nexusFerry.state;
		if (base.isServer)
		{
			EnsureInitialized();
			if (_state == State.Transferring)
			{
				_state = State.Arrival;
				Transform targetTransform = GetTargetTransform(_state);
				((Component)this).get_transform().SetPositionAndRotation(targetTransform.get_position(), targetTransform.get_rotation());
			}
		}
	}
}
