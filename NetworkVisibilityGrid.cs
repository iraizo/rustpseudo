using System.Collections.Generic;
using ConVar;
using Network;
using Network.Visibility;
using Rust;
using UnityEngine;
using UnityEngine.Serialization;

public class NetworkVisibilityGrid : MonoBehaviour, Provider
{
	public int startID = 1024;

	public int gridSize = 100;

	public int cellCount = 32;

	[FormerlySerializedAs("visibilityRadius")]
	public int visibilityRadiusFar = 2;

	public int visibilityRadiusNear = 1;

	public float switchTolerance = 20f;

	private void Awake()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		Debug.Assert(Net.sv != null, "Network.Net.sv is NULL when creating Visibility Grid");
		Debug.Assert(Net.sv.visibility == null, "Network.Net.sv.visibility is being set multiple times");
		Net.sv.visibility = new Manager((Provider)(object)this);
	}

	private void OnDisable()
	{
		if (!Application.isQuitting && Net.sv != null && Net.sv.visibility != null)
		{
			Net.sv.visibility.Dispose();
			Net.sv.visibility = null;
		}
	}

	private void OnDrawGizmosSelected()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.set_color(Color.get_blue());
		float num = CellSize();
		float num2 = (float)gridSize / 2f;
		Vector3 position = ((Component)this).get_transform().get_position();
		for (int i = 0; i <= cellCount; i++)
		{
			float num3 = 0f - num2 + (float)i * num - num / 2f;
			Gizmos.DrawLine(new Vector3(num2, position.y, num3), new Vector3(0f - num2, position.y, num3));
			Gizmos.DrawLine(new Vector3(num3, position.y, num2), new Vector3(num3, position.y, 0f - num2));
		}
	}

	private int PositionToGrid(float f)
	{
		f += (float)gridSize / 2f;
		return Mathf.RoundToInt(f / CellSize());
	}

	private float GridToPosition(int i)
	{
		return (float)i * CellSize() - (float)gridSize / 2f;
	}

	public uint CoordToID(int x, int y)
	{
		return (uint)(x * cellCount + y + startID);
	}

	public uint GetID(Vector3 vPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		int num = PositionToGrid(vPos.x);
		int num2 = PositionToGrid(vPos.z);
		if (num < 0)
		{
			return 0u;
		}
		if (num >= cellCount)
		{
			return 0u;
		}
		if (num2 < 0)
		{
			return 0u;
		}
		if (num2 >= cellCount)
		{
			return 0u;
		}
		uint num3 = CoordToID(num, num2);
		if (num3 < startID)
		{
			Debug.LogError((object)("NetworkVisibilityGrid.GetID - group is below range " + num + " " + num2 + " " + num3 + " " + cellCount));
		}
		if (num3 > startID + cellCount * cellCount)
		{
			Debug.LogError((object)("NetworkVisibilityGrid.GetID - group is higher than range " + num + " " + num2 + " " + num3 + " " + cellCount));
		}
		return num3;
	}

	public Vector3 GetPosition(uint uid)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		uid -= (uint)startID;
		int i = (int)((long)uid / (long)cellCount);
		int i2 = (int)((long)uid % (long)cellCount);
		return new Vector3(GridToPosition(i), 0f, GridToPosition(i2));
	}

	public Bounds GetBounds(uint uid)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		float num = CellSize();
		return new Bounds(GetPosition(uid), new Vector3(num, 1048576f, num));
	}

	public float CellSize()
	{
		return (float)gridSize / (float)cellCount;
	}

	public void OnGroupAdded(Group group)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		group.bounds = GetBounds(group.ID);
	}

	public bool IsInside(Group group, Vector3 vPos)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (0 == 0 && group.ID != 0 && !((Bounds)(ref group.bounds)).Contains(vPos))
		{
			return ((Bounds)(ref group.bounds)).SqrDistance(vPos) < switchTolerance;
		}
		return true;
	}

	public Group GetGroup(Vector3 vPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		uint iD = GetID(vPos);
		if (iD == 0)
		{
			return null;
		}
		Group val = Net.sv.visibility.Get(iD);
		if (!IsInside(val, vPos))
		{
			float num = ((Bounds)(ref val.bounds)).SqrDistance(vPos);
			Debug.Log((object)("Group is inside is all fucked " + iD + "/" + num + "/" + vPos));
		}
		return val;
	}

	public void GetVisibleFromFar(Group group, List<Group> groups)
	{
		int visibilityRadiusFarOverride = Net.visibilityRadiusFarOverride;
		int radius = ((visibilityRadiusFarOverride > 0) ? visibilityRadiusFarOverride : visibilityRadiusFar);
		GetVisibleFrom(group, groups, radius);
	}

	public void GetVisibleFromNear(Group group, List<Group> groups)
	{
		int visibilityRadiusNearOverride = Net.visibilityRadiusNearOverride;
		int radius = ((visibilityRadiusNearOverride > 0) ? visibilityRadiusNearOverride : visibilityRadiusNear);
		GetVisibleFrom(group, groups, radius);
	}

	private void GetVisibleFrom(Group group, List<Group> groups, int radius)
	{
		groups.Add(Net.sv.visibility.Get(0u));
		uint iD = group.ID;
		if (iD < startID)
		{
			return;
		}
		iD -= (uint)startID;
		int num = (int)((long)iD / (long)cellCount);
		int num2 = (int)((long)iD % (long)cellCount);
		groups.Add(Net.sv.visibility.Get(CoordToID(num, num2)));
		for (int i = 1; i <= radius; i++)
		{
			groups.Add(Net.sv.visibility.Get(CoordToID(num - i, num2)));
			groups.Add(Net.sv.visibility.Get(CoordToID(num + i, num2)));
			groups.Add(Net.sv.visibility.Get(CoordToID(num, num2 - i)));
			groups.Add(Net.sv.visibility.Get(CoordToID(num, num2 + i)));
			for (int j = 1; j < i; j++)
			{
				groups.Add(Net.sv.visibility.Get(CoordToID(num - i, num2 - j)));
				groups.Add(Net.sv.visibility.Get(CoordToID(num - i, num2 + j)));
				groups.Add(Net.sv.visibility.Get(CoordToID(num + i, num2 - j)));
				groups.Add(Net.sv.visibility.Get(CoordToID(num + i, num2 + j)));
				groups.Add(Net.sv.visibility.Get(CoordToID(num - j, num2 - i)));
				groups.Add(Net.sv.visibility.Get(CoordToID(num + j, num2 - i)));
				groups.Add(Net.sv.visibility.Get(CoordToID(num - j, num2 + i)));
				groups.Add(Net.sv.visibility.Get(CoordToID(num + j, num2 + i)));
			}
			groups.Add(Net.sv.visibility.Get(CoordToID(num - i, num2 - i)));
			groups.Add(Net.sv.visibility.Get(CoordToID(num - i, num2 + i)));
			groups.Add(Net.sv.visibility.Get(CoordToID(num + i, num2 - i)));
			groups.Add(Net.sv.visibility.Get(CoordToID(num + i, num2 + i)));
		}
	}

	public NetworkVisibilityGrid()
		: this()
	{
	}
}
