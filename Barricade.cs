using System;
using Rust;
using UnityEngine;
using UnityEngine.AI;

public class Barricade : DecayEntity
{
	public float reflectDamage = 5f;

	public GameObjectRef reflectEffect;

	public bool canNpcSmash = true;

	public NavMeshModifierVolume NavMeshVolumeAnimals;

	public NavMeshModifierVolume NavMeshVolumeHumanoids;

	[NonSerialized]
	public NPCBarricadeTriggerBox NpcTriggerBox;

	private static int nonWalkableArea = -1;

	private static int animalAgentTypeId = -1;

	private static int humanoidAgentTypeId = -1;

	public override void ServerInit()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		if (nonWalkableArea < 0)
		{
			nonWalkableArea = NavMesh.GetAreaFromName("Not Walkable");
		}
		NavMeshBuildSettings settingsByIndex;
		if (animalAgentTypeId < 0)
		{
			settingsByIndex = NavMesh.GetSettingsByIndex(1);
			animalAgentTypeId = ((NavMeshBuildSettings)(ref settingsByIndex)).get_agentTypeID();
		}
		if ((Object)(object)NavMeshVolumeAnimals == (Object)null)
		{
			NavMeshVolumeAnimals = ((Component)this).get_gameObject().AddComponent<NavMeshModifierVolume>();
			NavMeshVolumeAnimals.set_area(nonWalkableArea);
			NavMeshVolumeAnimals.AddAgentType(animalAgentTypeId);
			NavMeshVolumeAnimals.set_center(Vector3.get_zero());
			NavMeshVolumeAnimals.set_size(Vector3.get_one());
		}
		if (!canNpcSmash)
		{
			if (humanoidAgentTypeId < 0)
			{
				settingsByIndex = NavMesh.GetSettingsByIndex(0);
				humanoidAgentTypeId = ((NavMeshBuildSettings)(ref settingsByIndex)).get_agentTypeID();
			}
			if ((Object)(object)NavMeshVolumeHumanoids == (Object)null)
			{
				NavMeshVolumeHumanoids = ((Component)this).get_gameObject().AddComponent<NavMeshModifierVolume>();
				NavMeshVolumeHumanoids.set_area(nonWalkableArea);
				NavMeshVolumeHumanoids.AddAgentType(humanoidAgentTypeId);
				NavMeshVolumeHumanoids.set_center(Vector3.get_zero());
				NavMeshVolumeHumanoids.set_size(Vector3.get_one());
			}
		}
		else if ((Object)(object)NpcTriggerBox == (Object)null)
		{
			NpcTriggerBox = new GameObject("NpcTriggerBox").AddComponent<NPCBarricadeTriggerBox>();
			NpcTriggerBox.Setup(this);
		}
	}

	public override void OnAttacked(HitInfo info)
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		if (base.isServer && info.WeaponPrefab is BaseMelee && !info.IsProjectile())
		{
			BasePlayer basePlayer = info.Initiator as BasePlayer;
			if (Object.op_Implicit((Object)(object)basePlayer) && reflectDamage > 0f)
			{
				basePlayer.Hurt(reflectDamage * Random.Range(0.75f, 1.25f), DamageType.Stab, this);
				if (reflectEffect.isValid)
				{
					Effect.server.Run(reflectEffect.resourcePath, basePlayer, StringPool.closest, ((Component)this).get_transform().get_position(), Vector3.get_up());
				}
			}
		}
		base.OnAttacked(info);
	}
}
