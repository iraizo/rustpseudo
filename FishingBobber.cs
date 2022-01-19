using System;
using UnityEngine;

public class FishingBobber : BaseCombatEntity
{
	public Transform centerOfMass;

	public Rigidbody myRigidBody;

	public Transform lineAttachPoint;

	public Transform bobberRoot;

	public const Flags CaughtFish = Flags.Reserved1;

	public float HorizontalMoveSpeed = 1f;

	public float PullAwayMoveSpeed = 1f;

	public float SidewaysInputForce = 1f;

	public float ReelInMoveSpeed = 1f;

	private float bobberForcePingPong;

	private Vector3 initialDirection;

	private Vector3 initialTargetPosition;

	private Vector3 spawnPosition;

	private TimeSince initialCastTime;

	private float initialDistance;

	public float TireAmount { get; private set; }

	public override void ServerInit()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		myRigidBody.set_centerOfMass(centerOfMass.get_localPosition());
		base.ServerInit();
	}

	public void InitialiseBobber(BasePlayer forPlayer, WaterBody forBody, Vector3 targetPos)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		initialDirection = Vector3Ex.WithY(forPlayer.eyes.HeadForward(), 0f);
		spawnPosition = ((Component)this).get_transform().get_position();
		initialTargetPosition = targetPos;
		initialCastTime = TimeSince.op_Implicit(0f);
		initialDistance = Vector3.Distance(targetPos, Vector3Ex.WithY(((Component)forPlayer).get_transform().get_position(), targetPos.y));
		((FacepunchBehaviour)this).InvokeRepeating((Action)ProcessInitialCast, 0f, 0f);
	}

	private void ProcessInitialCast()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		float num = 0.8f;
		if (TimeSince.op_Implicit(initialCastTime) > num)
		{
			((Component)this).get_transform().set_position(initialTargetPosition);
			((FacepunchBehaviour)this).CancelInvoke((Action)ProcessInitialCast);
			return;
		}
		float num2 = TimeSince.op_Implicit(initialCastTime) / num;
		Vector3 val = Vector3.Lerp(spawnPosition, initialTargetPosition, 0.5f);
		val.y += 1.5f;
		Vector3 position = Vector3.Lerp(Vector3.Lerp(spawnPosition, val, num2), Vector3.Lerp(val, initialTargetPosition, num2), num2);
		((Component)this).get_transform().set_position(position);
	}

	public void ServerMovementUpdate(bool inputLeft, bool inputRight, bool inputBack, ref BaseFishingRod.FishState state, Vector3 playerPos, ItemModFishable fishableModifier)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = playerPos - ((Component)this).get_transform().get_position();
		Vector3 normalized = ((Vector3)(ref val)).get_normalized();
		Vector3 val2 = Vector3.get_zero();
		bobberForcePingPong = Mathf.Clamp(Mathf.PingPong(Time.get_time(), 2f), 0.2f, 2f);
		if (state.Contains(BaseFishingRod.FishState.PullingLeft))
		{
			val2 = ((Component)this).get_transform().get_right() * (Time.get_deltaTime() * HorizontalMoveSpeed * bobberForcePingPong * fishableModifier.MoveMultiplier * (inputRight ? 0.5f : 1f));
		}
		if (state.Contains(BaseFishingRod.FishState.PullingRight))
		{
			val2 = -((Component)this).get_transform().get_right() * (Time.get_deltaTime() * HorizontalMoveSpeed * bobberForcePingPong * fishableModifier.MoveMultiplier * (inputLeft ? 0.5f : 1f));
		}
		if (state.Contains(BaseFishingRod.FishState.PullingBack))
		{
			val2 += -((Component)this).get_transform().get_forward() * (Time.get_deltaTime() * PullAwayMoveSpeed * bobberForcePingPong * fishableModifier.MoveMultiplier * (inputBack ? 0.5f : 1f));
		}
		if (inputLeft || inputRight)
		{
			float num = 0.8f;
			if ((inputLeft && state == BaseFishingRod.FishState.PullingRight) || (inputRight && state == BaseFishingRod.FishState.PullingLeft))
			{
				num = 1.25f;
			}
			TireAmount += Time.get_deltaTime() * num;
		}
		else
		{
			TireAmount -= Time.get_deltaTime() * 0.1f;
		}
		if (inputLeft && !state.Contains(BaseFishingRod.FishState.PullingLeft))
		{
			val2 += ((Component)this).get_transform().get_right() * (Time.get_deltaTime() * SidewaysInputForce);
		}
		else if (inputRight && !state.Contains(BaseFishingRod.FishState.PullingRight))
		{
			val2 += -((Component)this).get_transform().get_right() * (Time.get_deltaTime() * SidewaysInputForce);
		}
		if (inputBack)
		{
			float num2 = Mathx.RemapValClamped(TireAmount, 0f, 5f, 1f, 3f);
			val2 += normalized * (ReelInMoveSpeed * fishableModifier.ReelInSpeedMultiplier * num2 * Time.get_deltaTime());
		}
		((Component)this).get_transform().LookAt(Vector3Ex.WithY(playerPos, ((Component)this).get_transform().get_position().y));
		Vector3 val3 = ((Component)this).get_transform().get_position() + val2;
		if (!IsDirectionValid(val3, ((Vector3)(ref val2)).get_magnitude(), playerPos))
		{
			state = state.FlipHorizontal();
		}
		else
		{
			((Component)this).get_transform().set_position(val3);
		}
	}

	private bool IsDirectionValid(Vector3 pos, float checkLength, Vector3 playerPos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = pos - playerPos;
		if (Vector3.Angle(Vector3Ex.WithY(((Vector3)(ref val)).get_normalized(), 0f), initialDirection) > 60f)
		{
			return false;
		}
		Vector3 position = ((Component)this).get_transform().get_position();
		val = pos - position;
		if (GamePhysics.Trace(new Ray(position, ((Vector3)(ref val)).get_normalized()), 0.1f, out var _, checkLength, 1218511105, (QueryTriggerInteraction)0))
		{
			return false;
		}
		return true;
	}
}
