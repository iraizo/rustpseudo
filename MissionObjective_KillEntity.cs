using System.Collections.Generic;
using Facepunch;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Missions/OBJECTIVES/Kill")]
public class MissionObjective_KillEntity : MissionObjective
{
	public string[] targetPrefabIDs;

	public int numToKill;

	public bool shouldUpdateMissionLocation;

	private float nextLocationUpdateTime;

	public override void ObjectiveStarted(BasePlayer playerFor, int index, BaseMission.MissionInstance instance)
	{
		base.ObjectiveStarted(playerFor, index, instance);
	}

	public override void ProcessMissionEvent(BasePlayer playerFor, BaseMission.MissionInstance instance, int index, BaseMission.MissionEventType type, string identifier, float amount)
	{
		base.ProcessMissionEvent(playerFor, instance, index, type, identifier, amount);
		if (IsCompleted(index, instance) || !CanProgress(index, instance) || type != BaseMission.MissionEventType.KILL_ENTITY)
		{
			return;
		}
		string[] array = targetPrefabIDs;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == identifier)
			{
				instance.objectiveStatuses[index].genericInt1 += (int)amount;
				if (instance.objectiveStatuses[index].genericInt1 >= numToKill)
				{
					CompleteObjective(index, instance, playerFor);
					playerFor.MissionDirty();
				}
				break;
			}
		}
	}

	public override void Think(int index, BaseMission.MissionInstance instance, BasePlayer assignee, float delta)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		if (shouldUpdateMissionLocation && IsStarted(index, instance) && Time.get_realtimeSinceStartup() > nextLocationUpdateTime)
		{
			nextLocationUpdateTime = Time.get_realtimeSinceStartup() + 1f;
			string[] array = targetPrefabIDs;
			foreach (string s in array)
			{
				uint result = 0u;
				uint.TryParse(s, out result);
				List<BaseCombatEntity> list = Pool.GetList<BaseCombatEntity>();
				Vis.Entities(((Component)assignee).get_transform().get_position(), 20f, list, 133120, (QueryTriggerInteraction)2);
				int num = -1;
				float num2 = float.PositiveInfinity;
				for (int j = 0; j < list.Count; j++)
				{
					BaseCombatEntity baseCombatEntity = list[j];
					if (baseCombatEntity.IsAlive() && baseCombatEntity.prefabID == result)
					{
						float num3 = Vector3.Distance(((Component)baseCombatEntity).get_transform().get_position(), ((Component)assignee).get_transform().get_position());
						if (num3 < num2)
						{
							num = j;
							num2 = num3;
						}
					}
				}
				if (num != -1)
				{
					instance.missionLocation = ((Component)list[num]).get_transform().get_position();
					assignee.MissionDirty();
					Pool.FreeList<BaseCombatEntity>(ref list);
					break;
				}
				Pool.FreeList<BaseCombatEntity>(ref list);
			}
		}
		base.Think(index, instance, assignee, delta);
	}
}
