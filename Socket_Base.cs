using System;
using UnityEngine;

public class Socket_Base : PrefabAttribute
{
	public bool male = true;

	public bool maleDummy;

	public bool female;

	public bool femaleDummy;

	public bool monogamous;

	[NonSerialized]
	public Vector3 position;

	[NonSerialized]
	public Quaternion rotation;

	public Vector3 selectSize = new Vector3(2f, 0.1f, 2f);

	public Vector3 selectCenter = new Vector3(0f, 0f, 1f);

	[ReadOnly]
	public string socketName;

	[NonSerialized]
	public SocketMod[] socketMods;

	public Vector3 GetSelectPivot(Vector3 position, Quaternion rotation)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return position + rotation * worldPosition;
	}

	public OBB GetSelectBounds(Vector3 position, Quaternion rotation)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		return new OBB(position + rotation * worldPosition, Vector3.get_one(), rotation * worldRotation, new Bounds(selectCenter, selectSize));
	}

	protected override Type GetIndexedType()
	{
		return typeof(Socket_Base);
	}

	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		position = ((Component)this).get_transform().get_position();
		rotation = ((Component)this).get_transform().get_rotation();
		socketMods = ((Component)this).GetComponentsInChildren<SocketMod>(true);
		SocketMod[] array = socketMods;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].baseSocket = this;
		}
	}

	public virtual bool TestTarget(Construction.Target target)
	{
		return target.socket != null;
	}

	public virtual bool IsCompatible(Socket_Base socket)
	{
		if (socket == null)
		{
			return false;
		}
		if (!socket.male && !male)
		{
			return false;
		}
		if (!socket.female && !female)
		{
			return false;
		}
		return ((object)socket).GetType() == ((object)this).GetType();
	}

	public virtual bool CanConnect(Vector3 position, Quaternion rotation, Socket_Base socket, Vector3 socketPosition, Quaternion socketRotation)
	{
		return IsCompatible(socket);
	}

	public virtual Construction.Placement DoPlacement(Construction.Target target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		Quaternion val = Quaternion.LookRotation(target.normal, Vector3.get_up()) * Quaternion.Euler(target.rotation);
		Vector3 val2 = target.position;
		val2 -= val * position;
		return new Construction.Placement
		{
			rotation = val,
			position = val2
		};
	}

	public virtual bool CheckSocketMods(Construction.Placement placement)
	{
		SocketMod[] array = socketMods;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ModifyPlacement(placement);
		}
		array = socketMods;
		foreach (SocketMod socketMod in array)
		{
			if (!socketMod.DoCheck(placement))
			{
				if (socketMod.FailedPhrase.IsValid())
				{
					Construction.lastPlacementError = "Failed Check: (" + socketMod.FailedPhrase.get_translated() + ")";
				}
				return false;
			}
		}
		return true;
	}
}
