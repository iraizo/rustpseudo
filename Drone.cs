using Network;
using UnityEngine;

public class Drone : RemoteControlEntity
{
	private struct DroneInputState
	{
		public Vector3 movement;

		public float throttle;

		public float pitch;

		public float yaw;

		public void Reset()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			movement = Vector3.get_zero();
			pitch = 0f;
			yaw = 0f;
		}
	}

	[Header("Drone")]
	public Rigidbody body;

	public bool killInWater = true;

	public bool enableGrounding = true;

	public bool keepAboveTerrain = true;

	public float groundTraceDist = 0.1f;

	public float altitudeAcceleration = 10f;

	public float movementAcceleration = 10f;

	public float yawSpeed = 2f;

	public float uprightSpeed = 2f;

	public float uprightPrediction = 0.15f;

	public float uprightDot = 0.5f;

	public float leanWeight = 0.1f;

	public float leanMaxVelocity = 5f;

	public float hurtVelocityThreshold = 3f;

	public float hurtDamagePower = 3f;

	public float collisionDisableTime = 0.25f;

	[Header("Sound")]
	public SoundDefinition movementLoopSoundDef;

	public SoundDefinition movementStartSoundDef;

	public SoundDefinition movementStopSoundDef;

	public AnimationCurve movementLoopPitchCurve;

	protected Vector3? targetPosition;

	private DroneInputState currentInput;

	private float lastInputTime;

	private double lastCollision = -1000.0;

	private bool isGrounded;

	public override bool RequiresMouse => true;

	protected override bool PositionTickFixedTime => true;

	public override void UserInput(InputState inputState, BasePlayer player)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		currentInput.Reset();
		int num = (inputState.IsDown(BUTTON.FORWARD) ? 1 : 0) + (inputState.IsDown(BUTTON.BACKWARD) ? (-1) : 0);
		int num2 = (inputState.IsDown(BUTTON.RIGHT) ? 1 : 0) + (inputState.IsDown(BUTTON.LEFT) ? (-1) : 0);
		ref DroneInputState reference = ref currentInput;
		Vector3 val = new Vector3((float)num2, 0f, (float)num);
		reference.movement = ((Vector3)(ref val)).get_normalized();
		currentInput.throttle = (inputState.IsDown(BUTTON.SPRINT) ? 1 : 0) + (inputState.IsDown(BUTTON.DUCK) ? (-1) : 0);
		currentInput.yaw = inputState.current.mouseDelta.x;
		currentInput.pitch = inputState.current.mouseDelta.y;
		lastInputTime = Time.get_time();
	}

	public virtual void Update()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		if (base.IsBeingControlled || !targetPosition.HasValue)
		{
			return;
		}
		Vector3 position = ((Component)this).get_transform().get_position();
		float height = TerrainMeta.HeightMap.GetHeight(position);
		Vector3 val = targetPosition.Value - body.get_velocity() * 0.5f;
		if (keepAboveTerrain)
		{
			val.y = Mathf.Max(val.y, height + 1f);
		}
		Vector2 val2 = Vector3Ex.XZ2D(val);
		Vector2 val3 = Vector3Ex.XZ2D(position);
		Vector3 val4 = default(Vector3);
		float num = default(float);
		Vector3Ex.ToDirectionAndMagnitude(Vector3Ex.XZ3D(val2 - val3), ref val4, ref num);
		currentInput.Reset();
		lastInputTime = Time.get_time();
		if (position.y - height > 1f)
		{
			float num2 = Mathf.Clamp01(num);
			currentInput.movement = ((Component)this).get_transform().InverseTransformVector(val4) * num2;
			if (num > 0.5f)
			{
				Quaternion val5 = ((Component)this).get_transform().get_rotation();
				float y = ((Quaternion)(ref val5)).get_eulerAngles().y;
				val5 = Quaternion.FromToRotation(Vector3.get_forward(), val4);
				float y2 = ((Quaternion)(ref val5)).get_eulerAngles().y;
				currentInput.yaw = Mathf.Clamp(Mathf.LerpAngle(y, y2, Time.get_deltaTime()) - y, -2f, 2f);
			}
		}
		currentInput.throttle = Mathf.Clamp(val.y - position.y, -1f, 1f);
	}

	public void FixedUpdate()
	{
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		if (!base.isServer || IsDead() || (!base.IsBeingControlled && !targetPosition.HasValue))
		{
			return;
		}
		float num = WaterFactor();
		if (killInWater && num > 0f)
		{
			if (num > 0.99f)
			{
				Kill();
			}
			return;
		}
		double currentTimestamp = TimeEx.get_currentTimestamp();
		bool num2 = lastCollision > 0.0 && currentTimestamp - lastCollision < (double)collisionDisableTime;
		RaycastHit val = default(RaycastHit);
		isGrounded = enableGrounding && body.SweepTest(-((Component)this).get_transform().get_up(), ref val, groundTraceDist);
		Vector3 val2 = ((Component)this).get_transform().TransformDirection(currentInput.movement);
		Vector3 val3 = default(Vector3);
		float num3 = default(float);
		Vector3Ex.ToDirectionAndMagnitude(Vector3Ex.WithY(body.get_velocity(), 0f), ref val3, ref num3);
		float num4 = Mathf.Clamp01(num3 / leanMaxVelocity);
		Vector3 val4 = (Mathf.Approximately(((Vector3)(ref val2)).get_sqrMagnitude(), 0f) ? ((0f - num4) * val3) : val2);
		Vector3 val5 = Vector3.get_up() + val4 * leanWeight * num4;
		Vector3 normalized = ((Vector3)(ref val5)).get_normalized();
		Vector3 up = ((Component)this).get_transform().get_up();
		float num5 = Mathf.Max(Vector3.Dot(normalized, up), 0f);
		if (!num2 || isGrounded)
		{
			Vector3 val6 = ((isGrounded && currentInput.throttle <= 0f) ? Vector3.get_zero() : (-1f * ((Component)this).get_transform().get_up() * Physics.get_gravity().y));
			Vector3 val7 = (isGrounded ? Vector3.get_zero() : (val2 * movementAcceleration));
			Vector3 val8 = ((Component)this).get_transform().get_up() * currentInput.throttle * altitudeAcceleration;
			Vector3 val9 = val6 + val7 + val8;
			body.AddForce(val9 * num5, (ForceMode)5);
		}
		if (!num2 && !isGrounded)
		{
			Vector3 val10 = ((Component)this).get_transform().TransformVector(0f, currentInput.yaw * yawSpeed, 0f);
			Vector3 val11 = Vector3.Cross(Quaternion.Euler(body.get_angularVelocity() * uprightPrediction) * up, normalized) * uprightSpeed;
			float num6 = ((num5 < uprightDot) ? 0f : num5);
			Vector3 val12 = val10 * num5 + val11 * num6;
			body.AddTorque(val12 * num5, (ForceMode)5);
		}
	}

	public void OnCollisionEnter(Collision collision)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (base.isServer)
		{
			lastCollision = TimeEx.get_currentTimestamp();
			Vector3 relativeVelocity = collision.get_relativeVelocity();
			float magnitude = ((Vector3)(ref relativeVelocity)).get_magnitude();
			if (magnitude > hurtVelocityThreshold)
			{
				Hurt(Mathf.Pow(magnitude, hurtDamagePower));
			}
		}
	}

	public void OnCollisionStay()
	{
		if (base.isServer)
		{
			lastCollision = TimeEx.get_currentTimestamp();
		}
	}

	public override float GetNetworkTime()
	{
		return Time.get_fixedTime();
	}
}
