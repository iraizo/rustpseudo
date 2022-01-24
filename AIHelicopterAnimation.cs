using UnityEngine;

public class AIHelicopterAnimation : MonoBehaviour
{
	public PatrolHelicopterAI _ai;

	public float swayAmount = 1f;

	public float lastStrafeScalar;

	public float lastForwardBackScalar;

	public float degreeMax = 90f;

	public Vector3 lastPosition = Vector3.get_zero();

	public float oldMoveSpeed;

	public float smoothRateOfChange;

	public float flareAmount;

	public void Awake()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		lastPosition = ((Component)this).get_transform().get_position();
	}

	public Vector3 GetMoveDirection()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return _ai.GetMoveDirection();
	}

	public float GetMoveSpeed()
	{
		return _ai.GetMoveSpeed();
	}

	public void Update()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		lastPosition = ((Component)this).get_transform().get_position();
		Vector3 moveDirection = GetMoveDirection();
		float moveSpeed = GetMoveSpeed();
		float num = 0.25f + Mathf.Clamp01(moveSpeed / _ai.maxSpeed) * 0.75f;
		smoothRateOfChange = Mathf.Lerp(smoothRateOfChange, moveSpeed - oldMoveSpeed, Time.get_deltaTime() * 5f);
		oldMoveSpeed = moveSpeed;
		float num2 = Vector3.Angle(moveDirection, ((Component)this).get_transform().get_forward());
		float num3 = Vector3.Angle(moveDirection, -((Component)this).get_transform().get_forward());
		float num4 = 1f - Mathf.Clamp01(num2 / degreeMax);
		float num5 = 1f - Mathf.Clamp01(num3 / degreeMax);
		float num6 = (num4 - num5) * num;
		float num7 = (lastForwardBackScalar = Mathf.Lerp(lastForwardBackScalar, num6, Time.get_deltaTime() * 2f));
		float num8 = Vector3.Angle(moveDirection, ((Component)this).get_transform().get_right());
		float num9 = Vector3.Angle(moveDirection, -((Component)this).get_transform().get_right());
		float num10 = 1f - Mathf.Clamp01(num8 / degreeMax);
		float num11 = 1f - Mathf.Clamp01(num9 / degreeMax);
		float num12 = (num10 - num11) * num;
		float num13 = (lastStrafeScalar = Mathf.Lerp(lastStrafeScalar, num12, Time.get_deltaTime() * 2f));
		Vector3 zero = Vector3.get_zero();
		zero.x += num7 * swayAmount;
		zero.z -= num13 * swayAmount;
		Quaternion identity = Quaternion.get_identity();
		identity = Quaternion.Euler(zero.x, zero.y, zero.z);
		_ai.helicopterBase.rotorPivot.get_transform().set_localRotation(identity);
	}

	public AIHelicopterAnimation()
		: this()
	{
	}//IL_0017: Unknown result type (might be due to invalid IL or missing references)
	//IL_001c: Unknown result type (might be due to invalid IL or missing references)

}
