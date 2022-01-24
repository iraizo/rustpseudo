using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public class ImpostorBatch
{
	public NativeList<Vector4> Positions;

	private NativeList<uint> args;

	private Queue<int> recycle = new Queue<int>(32);

	public Mesh Mesh { get; private set; }

	public Material Material { get; private set; }

	public ComputeBuffer PositionBuffer { get; private set; }

	public ComputeBuffer ArgsBuffer { get; private set; }

	public int Count => Positions.Count;

	public bool Visible => Positions.Count - recycle.get_Count() > 0;

	private ComputeBuffer SafeRelease(ComputeBuffer buffer)
	{
		if (buffer != null)
		{
			buffer.Release();
		}
		return null;
	}

	public void Initialize(Mesh mesh, Material material)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		Mesh = mesh;
		Material = material;
		Positions = Pool.Get<NativeList<Vector4>>();
		args = Pool.Get<NativeList<uint>>();
		args.Resize(5);
		ArgsBuffer = SafeRelease(ArgsBuffer);
		ArgsBuffer = new ComputeBuffer(1, args.Count * 4, (ComputeBufferType)256);
		args[0] = Mesh.GetIndexCount(0);
		args[2] = Mesh.GetIndexStart(0);
		args[3] = Mesh.GetBaseVertex(0);
	}

	public void Release()
	{
		recycle.Clear();
		Pool.Free<NativeList<Vector4>>(ref Positions);
		Pool.Free<NativeList<uint>>(ref args);
		PositionBuffer = SafeRelease(PositionBuffer);
		ArgsBuffer = SafeRelease(ArgsBuffer);
	}

	public void AddInstance(ImpostorInstanceData data)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		data.Batch = this;
		if (recycle.get_Count() > 0)
		{
			data.BatchIndex = recycle.Dequeue();
			Positions[data.BatchIndex] = data.PositionAndScale();
		}
		else
		{
			data.BatchIndex = Positions.Count;
			Positions.Add(data.PositionAndScale());
		}
	}

	public void RemoveInstance(ImpostorInstanceData data)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		Positions[data.BatchIndex] = new Vector4(0f, 0f, 0f, -1f);
		recycle.Enqueue(data.BatchIndex);
		data.BatchIndex = 0;
		data.Batch = null;
	}

	public void UpdateBuffers()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		if (PositionBuffer == null || PositionBuffer.get_count() != Positions.Count)
		{
			PositionBuffer = SafeRelease(PositionBuffer);
			PositionBuffer = new ComputeBuffer(Positions.Count, 16);
			flag = true;
		}
		if (PositionBuffer != null)
		{
			PositionBuffer.SetData<Vector4>(Positions.Array, 0, 0, Positions.Count);
		}
		if (ArgsBuffer != null && flag)
		{
			args[1] = (uint)Positions.Count;
			ArgsBuffer.SetData<uint>(args.Array, 0, 0, args.Count);
		}
	}
}
