using System;
using Facepunch;
using Unity.Collections;

public class NativeList<T> : IPooled where T : unmanaged
{
	private NativeArray<T> _array;

	private int _length;

	public NativeArray<T> Array => _array;

	public int Count => _length;

	public T this[int index]
	{
		get
		{
			return _array.get_Item(index);
		}
		set
		{
			_array.set_Item(index, value);
		}
	}

	public void Add(T item)
	{
		EnsureCapacity(_length + 1);
		_array.set_Item(_length++, item);
	}

	public void Clear()
	{
		for (int i = 0; i < _array.get_Length(); i++)
		{
			_array.set_Item(i, default(T));
		}
		_length = 0;
	}

	public void Resize(int count)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (_array.get_IsCreated())
		{
			_array.Dispose();
		}
		_array = new NativeArray<T>(count, (Allocator)4, (NativeArrayOptions)1);
		_length = count;
	}

	public void EnsureCapacity(int requiredCapacity)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		if (!_array.get_IsCreated() || _array.get_Length() < requiredCapacity)
		{
			int num = Math.Max(_array.get_Length() * 2, requiredCapacity);
			NativeArray<T> array = default(NativeArray<T>);
			array._002Ector(num, (Allocator)4, (NativeArrayOptions)1);
			if (_array.get_IsCreated())
			{
				_array.CopyTo(array.GetSubArray(0, _array.get_Length()));
				_array.Dispose();
			}
			_array = array;
		}
	}

	public void EnterPool()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (_array.get_IsCreated())
		{
			_array.Dispose();
		}
		_array = default(NativeArray<T>);
		_length = 0;
	}

	public void LeavePool()
	{
	}
}
