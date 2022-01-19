using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Missions/MoveMission")]
public class MoveMission : BaseMission
{
	public float minDistForMovePoint = 20f;

	public float maxDistForMovePoint = 25f;

	private float minDistFromLocation = 3f;

	public override void MissionStart(MissionInstance instance, BasePlayer assignee)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		Vector3 onUnitSphere = Random.get_onUnitSphere();
		onUnitSphere.y = 0f;
		((Vector3)(ref onUnitSphere)).Normalize();
		Vector3 val = ((Component)assignee).get_transform().get_position() + onUnitSphere * Random.Range(minDistForMovePoint, maxDistForMovePoint);
		float num = val.y;
		float num2 = val.y;
		if ((Object)(object)TerrainMeta.WaterMap != (Object)null)
		{
			num2 = TerrainMeta.WaterMap.GetHeight(val);
		}
		if ((Object)(object)TerrainMeta.HeightMap != (Object)null)
		{
			num = TerrainMeta.HeightMap.GetHeight(val);
		}
		val.y = Mathf.Max(num2, num);
		instance.missionLocation = val;
		base.MissionStart(instance, assignee);
	}

	public override void MissionEnded(MissionInstance instance, BasePlayer assignee)
	{
		base.MissionEnded(instance, assignee);
	}

	public override Sprite GetIcon(MissionInstance instance)
	{
		if (instance.status != MissionStatus.Accomplished)
		{
			return icon;
		}
		return providerIcon;
	}

	public override void Think(MissionInstance instance, BasePlayer assignee, float delta)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		float num = Vector3.Distance(instance.missionLocation, ((Component)assignee).get_transform().get_position());
		if (instance.status == MissionStatus.Active && num <= minDistFromLocation)
		{
			MissionSuccess(instance, assignee);
			BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(instance.providerID);
			if (Object.op_Implicit((Object)(object)baseNetworkable))
			{
				instance.missionLocation = ((Component)baseNetworkable).get_transform().get_position();
			}
		}
		else
		{
			if (instance.status == MissionStatus.Accomplished)
			{
				_ = minDistFromLocation;
			}
			base.Think(instance, assignee, delta);
		}
	}
}
