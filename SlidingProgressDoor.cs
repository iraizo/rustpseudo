using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

public class SlidingProgressDoor : ProgressDoor
{
	public Vector3 openPosition;

	public Vector3 closedPosition;

	public GameObject doorObject;

	public TriggerVehiclePush vehiclePhysBox;

	private float lastEnergyTime;

	private float lastServerUpdateTime;

	public override void Spawn()
	{
		base.Spawn();
		((FacepunchBehaviour)this).InvokeRepeating((Action)ServerUpdate, 0f, 0.1f);
		if ((Object)(object)vehiclePhysBox != (Object)null)
		{
			((Component)vehiclePhysBox).get_gameObject().SetActive(false);
		}
	}

	public override void NoEnergy()
	{
		base.NoEnergy();
	}

	public override void AddEnergy(float amount)
	{
		lastEnergyTime = Time.get_time();
		base.AddEnergy(amount);
	}

	public void ServerUpdate()
	{
		if (!base.isServer)
		{
			return;
		}
		if (lastServerUpdateTime == 0f)
		{
			lastServerUpdateTime = Time.get_realtimeSinceStartup();
		}
		float num = Time.get_realtimeSinceStartup() - lastServerUpdateTime;
		lastServerUpdateTime = Time.get_realtimeSinceStartup();
		if (Time.get_time() > lastEnergyTime + 0.333f)
		{
			float num2 = energyForOpen * num / secondsToClose;
			float num3 = Mathf.Min(storedEnergy, num2);
			if ((Object)(object)vehiclePhysBox != (Object)null)
			{
				((Component)vehiclePhysBox).get_gameObject().SetActive(num3 > 0f && storedEnergy > 0f);
				if (((Component)vehiclePhysBox).get_gameObject().get_activeSelf() && vehiclePhysBox.ContentsCount > 0)
				{
					num3 = 0f;
				}
			}
			storedEnergy -= num3;
			storedEnergy = Mathf.Clamp(storedEnergy, 0f, energyForOpen);
			if (num3 > 0f)
			{
				IOSlot[] array = outputs;
				foreach (IOSlot iOSlot in array)
				{
					if ((Object)(object)iOSlot.connectedTo.Get() != (Object)null)
					{
						iOSlot.connectedTo.Get().IOInput(this, ioType, 0f - num3, iOSlot.connectedToSlot);
					}
				}
			}
		}
		UpdateProgress();
	}

	public override void UpdateProgress()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		base.UpdateProgress();
		Vector3 localPosition = doorObject.get_transform().get_localPosition();
		float num = storedEnergy / energyForOpen;
		Vector3 val = Vector3.Lerp(closedPosition, openPosition, num);
		doorObject.get_transform().set_localPosition(val);
		if (base.isServer)
		{
			bool b = Vector3.Distance(localPosition, val) > 0.01f;
			SetFlag(Flags.Reserved1, b);
		}
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		_ = info.msg.sphereEntity;
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.sphereEntity = Pool.Get<SphereEntity>();
		info.msg.sphereEntity.radius = storedEnergy;
	}
}
