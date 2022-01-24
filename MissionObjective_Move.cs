using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Missions/OBJECTIVES/Move")]
public class MissionObjective_Move : MissionObjective
{
	public string positionName = "default";

	public float distForCompletion = 3f;

	public bool use2D;

	public override void ObjectiveStarted(BasePlayer playerFor, int index, BaseMission.MissionInstance instance)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		base.ObjectiveStarted(playerFor, index, instance);
		instance.missionLocation = instance.GetMissionPoint(positionName, playerFor);
		playerFor.MissionDirty();
	}

	public override void Think(int index, BaseMission.MissionInstance instance, BasePlayer assignee, float delta)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		base.Think(index, instance, assignee, delta);
		if (ShouldThink(index, instance))
		{
			Vector3 missionPoint = instance.GetMissionPoint(positionName, assignee);
			if ((use2D ? Vector3Ex.Distance2D(missionPoint, ((Component)assignee).get_transform().get_position()) : Vector3.Distance(missionPoint, ((Component)assignee).get_transform().get_position())) <= distForCompletion)
			{
				CompleteObjective(index, instance, assignee);
				assignee.MissionDirty();
			}
		}
	}
}
