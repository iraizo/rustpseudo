using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Vehicles/WorldSpline Shared Data", fileName = "WorldSpline Prefab Shared Data")]
public class WorldSplineSharedData : ScriptableObject
{
	[SerializeField]
	private List<WorldSplineData> dataList;

	public static WorldSplineSharedData instance;

	private static string[] worldSplineFolders = new string[2] { "Assets/Content/Structures", "Assets/bundled/Prefabs/autospawn" };

	[RuntimeInitializeOnLoadMethod]
	private static void Init()
	{
		instance = Resources.Load<WorldSplineSharedData>("WorldSpline Prefab Shared Data");
	}

	public static WorldSplineData GetDataFor(WorldSpline worldSpline)
	{
		if ((Object)(object)instance == (Object)null)
		{
			Debug.LogError((object)"No instance of WorldSplineSharedData found.");
			return null;
		}
		if (worldSpline.dataIndex < 0 || worldSpline.dataIndex >= instance.dataList.Count)
		{
			Debug.LogError((object)$"Data index out of range ({worldSpline.dataIndex}/{instance.dataList.Count}) for world spline: {((Object)worldSpline).get_name()}", (Object)(object)((Component)worldSpline).get_gameObject());
			return null;
		}
		return instance.dataList[worldSpline.dataIndex];
	}

	public WorldSplineSharedData()
		: this()
	{
	}
}
