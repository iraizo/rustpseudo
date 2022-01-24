using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public class DoorManipulator : IOEntity
{
	public enum DoorEffect
	{
		Close,
		Open,
		Toggle
	}

	public EntityRef entityRef;

	public Door targetDoor;

	public DoorEffect powerAction;

	private bool toggle = true;

	public virtual bool PairWithLockedDoors()
	{
		return true;
	}

	public virtual void SetTargetDoor(Door newTargetDoor)
	{
		Door door = targetDoor;
		targetDoor = newTargetDoor;
		SetFlag(Flags.On, (Object)(object)targetDoor != (Object)null);
		entityRef.Set(newTargetDoor);
		if ((Object)(object)door != (Object)(object)targetDoor && (Object)(object)targetDoor != (Object)null)
		{
			DoAction();
		}
	}

	public virtual void SetupInitialDoorConnection()
	{
		if ((Object)(object)targetDoor == (Object)null && !entityRef.IsValid(serverside: true))
		{
			SetTargetDoor(FindDoor(PairWithLockedDoors()));
		}
		if ((Object)(object)targetDoor != (Object)null && !entityRef.IsValid(serverside: true))
		{
			entityRef.Set(targetDoor);
		}
		if (entityRef.IsValid(serverside: true) && (Object)(object)targetDoor == (Object)null)
		{
			SetTargetDoor(((Component)entityRef.Get(serverside: true)).GetComponent<Door>());
		}
	}

	public override void Init()
	{
		base.Init();
		SetupInitialDoorConnection();
	}

	public Door FindDoor(bool allowLocked = true)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		List<Door> list = Pool.GetList<Door>();
		Vis.Entities(((Component)this).get_transform().get_position(), 1f, list, 2097152, (QueryTriggerInteraction)1);
		Door result = null;
		float num = float.PositiveInfinity;
		foreach (Door item in list)
		{
			if (!item.isServer)
			{
				continue;
			}
			if (!allowLocked)
			{
				BaseLock baseLock = item.GetSlot(Slot.Lock) as BaseLock;
				if ((Object)(object)baseLock != (Object)null && baseLock.IsLocked())
				{
					continue;
				}
			}
			float num2 = Vector3.Distance(((Component)item).get_transform().get_position(), ((Component)this).get_transform().get_position());
			if (num2 < num)
			{
				result = item;
				num = num2;
			}
		}
		Pool.FreeList<Door>(ref list);
		return result;
	}

	public virtual void DoActionDoorMissing()
	{
		SetTargetDoor(FindDoor(PairWithLockedDoors()));
	}

	public void DoAction()
	{
		bool flag = IsPowered();
		if ((Object)(object)targetDoor == (Object)null)
		{
			DoActionDoorMissing();
		}
		if (!((Object)(object)targetDoor != (Object)null))
		{
			return;
		}
		if (targetDoor.IsBusy())
		{
			((FacepunchBehaviour)this).Invoke((Action)DoAction, 1f);
		}
		else if (powerAction == DoorEffect.Open)
		{
			if (flag)
			{
				if (!targetDoor.IsOpen())
				{
					targetDoor.SetOpen(open: true);
				}
			}
			else if (targetDoor.IsOpen())
			{
				targetDoor.SetOpen(open: false);
			}
		}
		else if (powerAction == DoorEffect.Close)
		{
			if (flag)
			{
				if (targetDoor.IsOpen())
				{
					targetDoor.SetOpen(open: false);
				}
			}
			else if (!targetDoor.IsOpen())
			{
				targetDoor.SetOpen(open: true);
			}
		}
		else if (powerAction == DoorEffect.Toggle)
		{
			if (flag && toggle)
			{
				targetDoor.SetOpen(!targetDoor.IsOpen());
				toggle = false;
			}
			else if (!toggle)
			{
				toggle = true;
			}
		}
	}

	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		DoAction();
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity.genericEntRef1 = entityRef.uid;
		info.msg.ioEntity.genericInt1 = (int)powerAction;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			entityRef.uid = info.msg.ioEntity.genericEntRef1;
			powerAction = (DoorEffect)info.msg.ioEntity.genericInt1;
		}
	}
}
