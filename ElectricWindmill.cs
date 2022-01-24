using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

public class ElectricWindmill : IOEntity
{
	public Animator animator;

	public int maxPowerGeneration = 100;

	public Transform vaneRot;

	public SoundDefinition wooshSound;

	public Transform wooshOrigin;

	public float targetSpeed;

	private float serverWindSpeed;

	public override int MaximalPowerOutput()
	{
		return maxPowerGeneration;
	}

	public override bool IsRootEntity()
	{
		return true;
	}

	public float GetWindSpeedScale()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		float num = Time.get_time() / 600f;
		float num2 = ((Component)this).get_transform().get_position().x / 512f;
		float num3 = ((Component)this).get_transform().get_position().z / 512f;
		float num4 = Mathf.PerlinNoise(num2 + num, num3 + num * 0.1f);
		float height = TerrainMeta.HeightMap.GetHeight(((Component)this).get_transform().get_position());
		float num5 = ((Component)this).get_transform().get_position().y - height;
		if (num5 < 0f)
		{
			num5 = 0f;
		}
		return Mathf.Clamp01(Mathf.InverseLerp(0f, 50f, num5) * 0.5f + num4);
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		((FacepunchBehaviour)this).InvokeRandomized((Action)WindUpdate, 1f, 20f, 2f);
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
		{
			if (info.msg.ioEntity == null)
			{
				info.msg.ioEntity = Pool.Get<IOEntity>();
			}
			info.msg.ioEntity.genericFloat1 = Time.get_time();
			info.msg.ioEntity.genericFloat2 = serverWindSpeed;
		}
	}

	public bool AmIVisible()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		int num = 15;
		Vector3 val = ((Component)this).get_transform().get_position() + Vector3.get_up() * 6f;
		if (!IsVisible(val + ((Component)this).get_transform().get_up() * (float)num, (float)(num + 1)))
		{
			return false;
		}
		Vector3 windAimDir = GetWindAimDir(Time.get_time());
		if (!IsVisible(val + windAimDir * (float)num, (float)(num + 1)))
		{
			return false;
		}
		return true;
	}

	public void WindUpdate()
	{
		serverWindSpeed = GetWindSpeedScale();
		if (!AmIVisible())
		{
			serverWindSpeed = 0f;
		}
		int num = Mathf.FloorToInt((float)maxPowerGeneration * serverWindSpeed);
		bool num2 = currentEnergy != num;
		currentEnergy = num;
		if (num2)
		{
			MarkDirty();
		}
		SendNetworkUpdate();
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (outputSlot != 0)
		{
			return 0;
		}
		return currentEnergy;
	}

	public Vector3 GetWindAimDir(float time)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		float num = time / 3600f * 360f;
		int num2 = 10;
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(Mathf.Sin(num * ((float)Math.PI / 180f)) * (float)num2, 0f, Mathf.Cos(num * ((float)Math.PI / 180f)) * (float)num2);
		return ((Vector3)(ref val)).get_normalized();
	}
}
