using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

public class HitchTrough : StorageContainer
{
	[Serializable]
	public class HitchSpot
	{
		public HitchTrough owner;

		public Transform spot;

		public EntityRef horse;

		public RidableHorse GetHorse(bool isServer = true)
		{
			return horse.Get(isServer) as RidableHorse;
		}

		public bool IsOccupied(bool isServer = true)
		{
			return horse.IsValid(isServer);
		}

		public void SetOccupiedBy(RidableHorse newHorse)
		{
			horse.Set(newHorse);
		}
	}

	public HitchSpot[] hitchSpots;

	public float caloriesToDecaySeconds = 36f;

	public Item GetFoodItem()
	{
		foreach (Item item in base.inventory.itemList)
		{
			if (item.info.category == ItemCategory.Food && Object.op_Implicit((Object)(object)((Component)item.info).GetComponent<ItemModConsumable>()))
			{
				return item;
			}
		}
		return null;
	}

	public bool ValidHitchPosition(Vector3 pos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (GetClosest(pos, includeOccupied: false, 1f) != null)
		{
			return true;
		}
		return false;
	}

	public bool HasSpace()
	{
		HitchSpot[] array = hitchSpots;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].IsOccupied())
			{
				return true;
			}
		}
		return false;
	}

	public HitchSpot GetClosest(Vector3 testPos, bool includeOccupied = false, float maxRadius = -1f)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		float num = 10000f;
		HitchSpot result = null;
		for (int i = 0; i < hitchSpots.Length; i++)
		{
			float num2 = Vector3.Distance(testPos, hitchSpots[i].spot.get_position());
			if (num2 < num && (maxRadius == -1f || num2 <= maxRadius) && (includeOccupied || !hitchSpots[i].IsOccupied()))
			{
				num = num2;
				result = hitchSpots[i];
			}
		}
		return result;
	}

	public void Unhitch(RidableHorse horse)
	{
		HitchSpot[] array = hitchSpots;
		foreach (HitchSpot hitchSpot in array)
		{
			if ((Object)(object)hitchSpot.GetHorse(base.isServer) == (Object)(object)horse)
			{
				hitchSpot.SetOccupiedBy(null);
				horse.SetHitch(null);
			}
		}
	}

	public int NumHitched()
	{
		int num = 0;
		HitchSpot[] array = hitchSpots;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].IsOccupied())
			{
				num++;
			}
		}
		return num;
	}

	public bool AttemptToHitch(RidableHorse horse, HitchSpot hitch = null)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)horse == (Object)null)
		{
			return false;
		}
		if (hitch == null)
		{
			hitch = GetClosest(((Component)horse).get_transform().get_position());
		}
		if (hitch != null)
		{
			hitch.SetOccupiedBy(horse);
			horse.SetHitch(this);
			((Component)horse).get_transform().SetPositionAndRotation(hitch.spot.get_position(), hitch.spot.get_rotation());
			horse.DismountAllPlayers();
			return true;
		}
		return false;
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity = Pool.Get<IOEntity>();
		info.msg.ioEntity.genericEntRef1 = hitchSpots[0].horse.uid;
		info.msg.ioEntity.genericEntRef2 = hitchSpots[1].horse.uid;
	}

	public override void PostServerLoad()
	{
		HitchSpot[] array = hitchSpots;
		foreach (HitchSpot hitchSpot in array)
		{
			AttemptToHitch(hitchSpot.GetHorse(), hitchSpot);
		}
	}

	public void UnhitchAll()
	{
		HitchSpot[] array = hitchSpots;
		for (int i = 0; i < array.Length; i++)
		{
			RidableHorse horse = array[i].GetHorse();
			if (Object.op_Implicit((Object)(object)horse))
			{
				Unhitch(horse);
			}
		}
	}

	public override void DestroyShared()
	{
		if (base.isServer)
		{
			UnhitchAll();
		}
		base.DestroyShared();
	}

	public override void OnKilled(HitInfo info)
	{
		base.OnKilled(info);
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			hitchSpots[0].horse.uid = info.msg.ioEntity.genericEntRef1;
			hitchSpots[1].horse.uid = info.msg.ioEntity.genericEntRef2;
		}
	}
}
