using System;
using UnityEngine;

public class SolarPanel : IOEntity
{
	public Transform sunSampler;

	private const int tickrateSeconds = 60;

	public int maximalPowerOutput = 10;

	public float dot_minimum = 0.1f;

	public float dot_maximum = 0.6f;

	public override bool IsRootEntity()
	{
		return true;
	}

	public override int MaximalPowerOutput()
	{
		return maximalPowerOutput;
	}

	public override int ConsumptionAmount()
	{
		return 0;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		((FacepunchBehaviour)this).InvokeRandomized((Action)SunUpdate, 1f, 5f, 2f);
	}

	public void SunUpdate()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		int num = currentEnergy;
		if (TOD_Sky.get_Instance().get_IsNight())
		{
			num = 0;
		}
		else
		{
			Vector3 val = TOD_Sky.get_Instance().get_Components().Sun.get_transform().get_position() - ((Component)sunSampler).get_transform().get_position();
			Vector3 normalized = ((Vector3)(ref val)).get_normalized();
			float num2 = Vector3.Dot(((Component)sunSampler).get_transform().get_forward(), normalized);
			float num3 = Mathf.InverseLerp(dot_minimum, dot_maximum, num2);
			if (num3 > 0f && !IsVisible(((Component)sunSampler).get_transform().get_position() + normalized * 100f, 101f))
			{
				num3 = 0f;
			}
			num = Mathf.FloorToInt((float)maximalPowerOutput * num3 * base.healthFraction);
		}
		bool num4 = currentEnergy != num;
		currentEnergy = num;
		if (num4)
		{
			MarkDirty();
		}
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (outputSlot != 0)
		{
			return 0;
		}
		return currentEnergy;
	}
}
