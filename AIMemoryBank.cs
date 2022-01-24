using UnityEngine;

public class AIMemoryBank<T>
{
	private MemoryBankType type;

	private T[] slots;

	private float[] slotSetTimestamps;

	private int slotCount;

	public AIMemoryBank(MemoryBankType type, int slots)
	{
		Init(type, slots);
	}

	public void Init(MemoryBankType type, int slots)
	{
		this.type = type;
		slotCount = slots;
		this.slots = new T[slotCount];
		slotSetTimestamps = new float[slotCount];
	}

	public void Set(T item, int index)
	{
		if (index >= 0 && index < slotCount)
		{
			slots[index] = item;
			slotSetTimestamps[index] = Time.get_realtimeSinceStartup();
		}
	}

	public T Get(int index)
	{
		if (index < 0 || index >= slotCount)
		{
			return default(T);
		}
		return slots[index];
	}

	public float GetTimeSinceSet(int index)
	{
		if (index < 0 || index >= slotCount)
		{
			return 0f;
		}
		return Time.get_realtimeSinceStartup() - slotSetTimestamps[index];
	}

	public void Remove(int index)
	{
		if (index >= 0 && index < slotCount)
		{
			slots[index] = default(T);
		}
	}
}
