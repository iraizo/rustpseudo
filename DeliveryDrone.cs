using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

public class DeliveryDrone : Drone
{
	private enum State
	{
		Invalid,
		Takeoff,
		FlyToVendingMachine,
		DescendToVendingMachine,
		PickUpItems,
		AscendBeforeReturn,
		ReturnToTerminal,
		Landing
	}

	[Header("Delivery Drone")]
	public float stateTimeout = 300f;

	public float targetPositionTolerance = 1f;

	public float preferredCruiseHeight = 20f;

	public float preferredHeightAboveObstacle = 5f;

	public float marginAbovePreferredHeight = 3f;

	public float obstacleHeightLockDuration = 3f;

	public int pickUpDelayInTicks = 3;

	public DeliveryDroneConfig config;

	public GameObjectRef mapMarkerPrefab;

	public EntityRef<Marketplace> sourceMarketplace;

	public EntityRef<MarketTerminal> sourceTerminal;

	public EntityRef<VendingMachine> targetVendingMachine;

	private State _state;

	private RealTimeSince _sinceLastStateChange;

	private Vector3? _stateGoalPosition;

	private float? _goToY;

	private TimeSince _sinceLastObstacleBlock;

	private float? _minimumYLock;

	private int _pickUpTicks;

	private BaseEntity _mapMarkerInstance;

	public void Setup(Marketplace marketplace, MarketTerminal terminal, VendingMachine vendingMachine)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		sourceMarketplace.Set(marketplace);
		sourceTerminal.Set(terminal);
		targetVendingMachine.Set(vendingMachine);
		_state = State.Takeoff;
		_sinceLastStateChange = RealTimeSince.op_Implicit(0f);
		_pickUpTicks = 0;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		((FacepunchBehaviour)this).InvokeRandomized((Action)Think, 0f, 0.5f, 0.25f);
		CreateMapMarker();
	}

	public void CreateMapMarker()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)_mapMarkerInstance != (Object)null)
		{
			_mapMarkerInstance.Kill();
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(mapMarkerPrefab?.resourcePath, Vector3.get_zero(), Quaternion.get_identity());
		baseEntity.OwnerID = base.OwnerID;
		baseEntity.Spawn();
		baseEntity.SetParent(this);
		_mapMarkerInstance = baseEntity;
	}

	private void Think()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		if (RealTimeSince.op_Implicit(_sinceLastStateChange) > stateTimeout)
		{
			Debug.LogError((object)"Delivery drone hasn't change state in too long, killing", (Object)(object)this);
			ForceRemove();
			return;
		}
		if (!sourceMarketplace.TryGet(serverside: true, out var marketplace) || !sourceTerminal.TryGet(serverside: true, out var _))
		{
			Debug.LogError((object)"Delivery drone's marketplace or terminal was destroyed, killing", (Object)(object)this);
			ForceRemove();
			return;
		}
		if (!targetVendingMachine.TryGet(serverside: true, out var entity2) && _state <= State.AscendBeforeReturn)
		{
			SetState(State.ReturnToTerminal);
		}
		Vector3 currentPosition = ((Component)this).get_transform().get_position();
		float num = GetMinimumHeight(Vector3.get_zero());
		if (_goToY.HasValue)
		{
			if (!IsAtGoToY())
			{
				targetPosition = Vector3Ex.WithY(currentPosition, _goToY.Value);
				return;
			}
			_goToY = null;
			_sinceLastObstacleBlock = TimeSince.op_Implicit(0f);
			_minimumYLock = currentPosition.y;
		}
		Vector3 waitPosition;
		switch (_state)
		{
		case State.Takeoff:
			SetGoalPosition(marketplace.droneLaunchPoint.get_position() + Vector3.get_up() * 15f);
			if (IsAtGoalPosition())
			{
				SetState(State.FlyToVendingMachine);
			}
			break;
		case State.FlyToVendingMachine:
		{
			bool isBlocked2;
			float num2 = CalculatePreferredY(out isBlocked2);
			if (isBlocked2 && currentPosition.y < num2)
			{
				SetGoToY(num2 + marginAbovePreferredHeight);
				return;
			}
			config.FindDescentPoints(entity2, num2 + marginAbovePreferredHeight, out waitPosition, out var descendPosition);
			SetGoalPosition(descendPosition);
			if (IsAtGoalPosition())
			{
				SetState(State.DescendToVendingMachine);
			}
			break;
		}
		case State.DescendToVendingMachine:
		{
			config.FindDescentPoints(entity2, currentPosition.y, out var waitPosition2, out waitPosition);
			SetGoalPosition(waitPosition2);
			if (IsAtGoalPosition())
			{
				SetState(State.PickUpItems);
			}
			break;
		}
		case State.PickUpItems:
			_pickUpTicks++;
			if (_pickUpTicks >= pickUpDelayInTicks)
			{
				SetState(State.AscendBeforeReturn);
			}
			break;
		case State.AscendBeforeReturn:
		{
			config.FindDescentPoints(entity2, num + preferredCruiseHeight, out waitPosition, out var descendPosition2);
			SetGoalPosition(descendPosition2);
			if (IsAtGoalPosition())
			{
				SetState(State.ReturnToTerminal);
			}
			break;
		}
		case State.ReturnToTerminal:
		{
			bool isBlocked3;
			float num3 = CalculatePreferredY(out isBlocked3);
			if (isBlocked3 && currentPosition.y < num3)
			{
				SetGoToY(num3 + marginAbovePreferredHeight);
				return;
			}
			Vector3 val = LandingPosition();
			if (Vector3Ex.Distance2D(currentPosition, val) < 30f)
			{
				val.y = Mathf.Max(val.y, num3 + marginAbovePreferredHeight);
			}
			else
			{
				val.y = num3 + marginAbovePreferredHeight;
			}
			SetGoalPosition(val);
			if (IsAtGoalPosition())
			{
				SetState(State.Landing);
			}
			break;
		}
		case State.Landing:
			SetGoalPosition(LandingPosition());
			if (IsAtGoalPosition())
			{
				marketplace.ReturnDrone(this);
				SetState(State.Invalid);
			}
			break;
		default:
			ForceRemove();
			break;
		}
		if (_minimumYLock.HasValue)
		{
			if (TimeSince.op_Implicit(_sinceLastObstacleBlock) > obstacleHeightLockDuration)
			{
				_minimumYLock = null;
			}
			else if (targetPosition.HasValue && targetPosition.Value.y < _minimumYLock.Value)
			{
				targetPosition = Vector3Ex.WithY(targetPosition.Value, _minimumYLock.Value);
			}
		}
		float CalculatePreferredY(out bool isBlocked)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val2 = default(Vector3);
			float num4 = default(float);
			Vector3Ex.ToDirectionAndMagnitude(Vector3Ex.WithY(body.get_velocity(), 0f), ref val2, ref num4);
			if (num4 < 0.5f)
			{
				float num5 = GetMinimumHeight(Vector3.get_zero()) + preferredCruiseHeight;
				Vector3 val3 = Vector3Ex.WithY(currentPosition, num5 + 1000f);
				Vector3Ex.WithY(currentPosition, num5);
				RaycastHit val4 = default(RaycastHit);
				isBlocked = Physics.Raycast(val3, Vector3.get_down(), ref val4, 1000f, LayerMask.op_Implicit(config.layerMask));
				if (!isBlocked)
				{
					return num5;
				}
				return num5 + (1000f - ((RaycastHit)(ref val4)).get_distance()) + preferredHeightAboveObstacle;
			}
			float num6 = num4 * 2f;
			float minimumHeight = GetMinimumHeight(Vector3.get_zero());
			float minimumHeight2 = GetMinimumHeight(new Vector3(0f, 0f, num6 / 2f));
			float minimumHeight3 = GetMinimumHeight(new Vector3(0f, 0f, num6));
			float num7 = Mathf.Max(Mathf.Max(minimumHeight, minimumHeight2), minimumHeight3) + preferredCruiseHeight;
			Quaternion val5 = Quaternion.FromToRotation(Vector3.get_forward(), val2);
			Vector3 val6 = Vector3Ex.WithZ(config.halfExtents, num6 / 2f);
			Vector3 val7 = Vector3Ex.WithY(Vector3Ex.WithY(currentPosition, num7) + val5 * new Vector3(0f, 0f, val6.z / 2f), num7 + 1000f);
			RaycastHit val8 = default(RaycastHit);
			isBlocked = Physics.BoxCast(val7, val6, Vector3.get_down(), ref val8, val5, 1000f, LayerMask.op_Implicit(config.layerMask));
			if (isBlocked)
			{
				Ray ray = default(Ray);
				((Ray)(ref ray))._002Ector(val7, Vector3.get_down());
				Vector3 val9 = ray.ClosestPoint(((RaycastHit)(ref val8)).get_point());
				float num8 = Vector3.Distance(((Ray)(ref ray)).get_origin(), val9);
				return num7 + (1000f - num8) + preferredHeightAboveObstacle;
			}
			return num7;
		}
		float GetMinimumHeight(Vector3 offset)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val10 = ((Component)this).get_transform().TransformPoint(offset);
			float height = TerrainMeta.HeightMap.GetHeight(val10);
			float height2 = WaterSystem.GetHeight(val10);
			return Mathf.Max(height, height2);
		}
		bool IsAtGoToY()
		{
			if (_goToY.HasValue)
			{
				return Mathf.Abs(_goToY.Value - currentPosition.y) < targetPositionTolerance;
			}
			return false;
		}
		bool IsAtGoalPosition()
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			if (_stateGoalPosition.HasValue)
			{
				return Vector3.Distance(_stateGoalPosition.Value, currentPosition) < targetPositionTolerance;
			}
			return false;
		}
		Vector3 LandingPosition()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return marketplace.droneLaunchPoint.get_position();
		}
		void SetGoToY(float y)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			_goToY = y;
			targetPosition = Vector3Ex.WithY(currentPosition, y);
		}
		void SetGoalPosition(Vector3 position)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			_goToY = null;
			_stateGoalPosition = position;
			targetPosition = position;
		}
		void SetState(State newState)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			_state = newState;
			_sinceLastStateChange = RealTimeSince.op_Implicit(0f);
			_pickUpTicks = 0;
			_stateGoalPosition = null;
			_goToY = null;
			SetFlag(Flags.Reserved1, _state >= State.AscendBeforeReturn);
		}
	}

	private void ForceRemove()
	{
		if (sourceMarketplace.TryGet(serverside: true, out var entity))
		{
			entity.ReturnDrone(this);
		}
		else
		{
			Kill();
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			info.msg.deliveryDrone = Pool.Get<DeliveryDrone>();
			info.msg.deliveryDrone.marketplaceId = sourceMarketplace.uid;
			info.msg.deliveryDrone.terminalId = sourceTerminal.uid;
			info.msg.deliveryDrone.vendingMachineId = targetVendingMachine.uid;
			info.msg.deliveryDrone.state = (int)_state;
		}
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.deliveryDrone != null)
		{
			sourceMarketplace = new EntityRef<Marketplace>(info.msg.deliveryDrone.marketplaceId);
			sourceTerminal = new EntityRef<MarketTerminal>(info.msg.deliveryDrone.terminalId);
			targetVendingMachine = new EntityRef<VendingMachine>(info.msg.deliveryDrone.vendingMachineId);
			_state = (State)info.msg.deliveryDrone.state;
		}
	}

	public override bool CanControl()
	{
		return false;
	}
}
