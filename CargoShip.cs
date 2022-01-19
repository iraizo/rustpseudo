using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;

public class CargoShip : BaseEntity
{
	public int targetNodeIndex = -1;

	public GameObject wakeParent;

	public GameObjectRef scientistTurretPrefab;

	public Transform[] scientistSpawnPoints;

	public List<Transform> crateSpawns;

	public GameObjectRef lockedCratePrefab;

	public GameObjectRef militaryCratePrefab;

	public GameObjectRef eliteCratePrefab;

	public GameObjectRef junkCratePrefab;

	public Transform waterLine;

	public Transform rudder;

	public Transform propeller;

	public GameObjectRef escapeBoatPrefab;

	public Transform escapeBoatPoint;

	public GameObjectRef microphonePrefab;

	public Transform microphonePoint;

	public GameObjectRef speakerPrefab;

	public Transform[] speakerPoints;

	public GameObject radiation;

	public GameObjectRef mapMarkerEntityPrefab;

	public GameObject hornOrigin;

	public SoundDefinition hornDef;

	public CargoShipSounds cargoShipSounds;

	public GameObject[] layouts;

	public GameObjectRef playerTest;

	private uint layoutChoice;

	[ServerVar]
	public static bool event_enabled = true;

	[ServerVar]
	public static float event_duration_minutes = 50f;

	[ServerVar]
	public static float egress_duration_minutes = 10f;

	[ServerVar]
	public static int loot_rounds = 3;

	[ServerVar]
	public static float loot_round_spacing_minutes = 10f;

	private BaseEntity mapMarkerInstance;

	private Vector3 currentVelocity = Vector3.get_zero();

	private float currentThrottle;

	private float currentTurnSpeed;

	private float turnScale;

	private int lootRoundsPassed;

	private int hornCount;

	private float currentRadiation;

	private bool egressing;

	public override float GetNetworkTime()
	{
		return Time.get_fixedTime();
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.simpleUID != null)
		{
			layoutChoice = info.msg.simpleUID.uid;
		}
	}

	public void RefreshActiveLayout()
	{
		for (int i = 0; i < layouts.Length; i++)
		{
			layouts[i].SetActive(layoutChoice == i);
		}
	}

	public void TriggeredEventSpawn()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = TerrainMeta.RandomPointOffshore();
		val.y = TerrainMeta.WaterMap.GetHeight(val);
		((Component)this).get_transform().set_position(val);
		if (!event_enabled || event_duration_minutes == 0f)
		{
			((FacepunchBehaviour)this).Invoke((Action)DelayedDestroy, 1f);
		}
	}

	public void CreateMapMarker()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)mapMarkerInstance))
		{
			mapMarkerInstance.Kill();
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(mapMarkerEntityPrefab.resourcePath, Vector3.get_zero(), Quaternion.get_identity());
		baseEntity.Spawn();
		baseEntity.SetParent(this);
		mapMarkerInstance = baseEntity;
	}

	public void DisableCollisionTest()
	{
	}

	public void SpawnCrate(string resourcePath)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		int index = Random.Range(0, crateSpawns.Count);
		Vector3 position = crateSpawns[index].get_position();
		Quaternion rotation = crateSpawns[index].get_rotation();
		crateSpawns.Remove(crateSpawns[index]);
		BaseEntity baseEntity = GameManager.server.CreateEntity(resourcePath, position, rotation);
		if (Object.op_Implicit((Object)(object)baseEntity))
		{
			baseEntity.enableSaving = false;
			((Component)baseEntity).SendMessage("SetWasDropped", (SendMessageOptions)1);
			baseEntity.Spawn();
			baseEntity.SetParent(this, worldPositionStays: true);
			Rigidbody component = ((Component)baseEntity).GetComponent<Rigidbody>();
			if ((Object)(object)component != (Object)null)
			{
				component.set_isKinematic(true);
			}
		}
	}

	public void RespawnLoot()
	{
		((FacepunchBehaviour)this).InvokeRepeating((Action)PlayHorn, 0f, 8f);
		SpawnCrate(lockedCratePrefab.resourcePath);
		SpawnCrate(eliteCratePrefab.resourcePath);
		for (int i = 0; i < 4; i++)
		{
			SpawnCrate(militaryCratePrefab.resourcePath);
		}
		for (int j = 0; j < 4; j++)
		{
			SpawnCrate(junkCratePrefab.resourcePath);
		}
		lootRoundsPassed++;
		if (lootRoundsPassed >= loot_rounds)
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)RespawnLoot);
		}
	}

	public void SpawnSubEntities()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		BaseEntity baseEntity = GameManager.server.CreateEntity(escapeBoatPrefab.resourcePath, escapeBoatPoint.get_position(), escapeBoatPoint.get_rotation());
		if (Object.op_Implicit((Object)(object)baseEntity))
		{
			baseEntity.enableSaving = false;
			baseEntity.SetParent(this, worldPositionStays: true);
			baseEntity.Spawn();
			((Component)baseEntity).GetComponent<Rigidbody>().set_isKinematic(true);
			RHIB component = ((Component)baseEntity).GetComponent<RHIB>();
			if (Object.op_Implicit((Object)(object)component))
			{
				component.AddFuel(50);
			}
		}
		MicrophoneStand microphoneStand = GameManager.server.CreateEntity(microphonePrefab.resourcePath, microphonePoint.get_position(), microphonePoint.get_rotation()) as MicrophoneStand;
		if (Object.op_Implicit((Object)(object)microphoneStand))
		{
			microphoneStand.enableSaving = false;
			microphoneStand.SetParent(this, worldPositionStays: true);
			microphoneStand.Spawn();
			microphoneStand.SpawnChildEntity();
			IOEntity iOEntity = microphoneStand.ioEntity.Get(serverside: true);
			Transform[] array = speakerPoints;
			foreach (Transform val in array)
			{
				IOEntity iOEntity2 = GameManager.server.CreateEntity(speakerPrefab.resourcePath, val.get_position(), val.get_rotation()) as IOEntity;
				iOEntity2.enableSaving = false;
				iOEntity2.SetParent(this, worldPositionStays: true);
				iOEntity2.Spawn();
				iOEntity.outputs[0].connectedTo.Set(iOEntity2);
				iOEntity2.inputs[0].connectedTo.Set(iOEntity);
				iOEntity = iOEntity2;
			}
			microphoneStand.ioEntity.Get(serverside: true).MarkDirtyForceUpdateOutputs();
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.simpleUID = Pool.Get<SimpleUID>();
		info.msg.simpleUID.uid = layoutChoice;
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		RefreshActiveLayout();
	}

	public void PlayHorn()
	{
		ClientRPC(null, "DoHornSound");
		hornCount++;
		if (hornCount >= 3)
		{
			hornCount = 0;
			((FacepunchBehaviour)this).CancelInvoke((Action)PlayHorn);
		}
	}

	public override void Spawn()
	{
		if (!Application.isLoadingSave)
		{
			layoutChoice = (uint)Random.Range(0, layouts.Length);
			SendNetworkUpdate();
			RefreshActiveLayout();
		}
		base.Spawn();
	}

	public override void ServerInit()
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		((FacepunchBehaviour)this).Invoke((Action)FindInitialNode, 2f);
		((FacepunchBehaviour)this).InvokeRepeating((Action)BuildingCheck, 1f, 5f);
		((FacepunchBehaviour)this).InvokeRepeating((Action)RespawnLoot, 10f, 60f * loot_round_spacing_minutes);
		((FacepunchBehaviour)this).Invoke((Action)DisableCollisionTest, 10f);
		float height = TerrainMeta.WaterMap.GetHeight(((Component)this).get_transform().get_position());
		Vector3 val = ((Component)this).get_transform().InverseTransformPoint(((Component)waterLine).get_transform().get_position());
		((Component)this).get_transform().set_position(new Vector3(((Component)this).get_transform().get_position().x, height - val.y, ((Component)this).get_transform().get_position().z));
		SpawnSubEntities();
		((FacepunchBehaviour)this).Invoke((Action)StartEgress, 60f * event_duration_minutes);
		CreateMapMarker();
	}

	public void UpdateRadiation()
	{
		currentRadiation += 1f;
		TriggerRadiation[] componentsInChildren = radiation.GetComponentsInChildren<TriggerRadiation>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].RadiationAmountOverride = currentRadiation;
		}
	}

	public void StartEgress()
	{
		if (!egressing)
		{
			egressing = true;
			((FacepunchBehaviour)this).CancelInvoke((Action)PlayHorn);
			radiation.SetActive(true);
			SetFlag(Flags.Reserved8, b: true);
			((FacepunchBehaviour)this).InvokeRepeating((Action)UpdateRadiation, 10f, 1f);
			((FacepunchBehaviour)this).Invoke((Action)DelayedDestroy, 60f * egress_duration_minutes);
		}
	}

	public void DelayedDestroy()
	{
		Kill();
	}

	public void FindInitialNode()
	{
		targetNodeIndex = GetClosestNodeToUs();
	}

	public void BuildingCheck()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		List<DecayEntity> list = Pool.GetList<DecayEntity>();
		Vis.Entities(WorldSpaceBounds(), list, 2097152, (QueryTriggerInteraction)2);
		foreach (DecayEntity item in list)
		{
			if (item.isServer && item.IsAlive())
			{
				item.Kill(DestroyMode.Gib);
			}
		}
		Pool.FreeList<DecayEntity>(ref list);
	}

	public void FixedUpdate()
	{
		if (!base.isClient)
		{
			UpdateMovement();
		}
	}

	public void UpdateMovement()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		if (TerrainMeta.Path.OceanPatrolFar == null || TerrainMeta.Path.OceanPatrolFar.Count == 0 || targetNodeIndex == -1)
		{
			return;
		}
		Vector3 val = TerrainMeta.Path.OceanPatrolFar[targetNodeIndex];
		Vector3 val2;
		if (egressing)
		{
			Vector3 position = ((Component)this).get_transform().get_position();
			val2 = ((Component)this).get_transform().get_position() - Vector3.get_zero();
			val = position + ((Vector3)(ref val2)).get_normalized() * 10000f;
		}
		float num = 0f;
		val2 = val - ((Component)this).get_transform().get_position();
		Vector3 normalized = ((Vector3)(ref val2)).get_normalized();
		float num2 = Vector3.Dot(((Component)this).get_transform().get_forward(), normalized);
		num = Mathf.InverseLerp(0f, 1f, num2);
		float num3 = Vector3.Dot(((Component)this).get_transform().get_right(), normalized);
		float num4 = 2.5f;
		float num5 = Mathf.InverseLerp(0.05f, 0.5f, Mathf.Abs(num3));
		turnScale = Mathf.Lerp(turnScale, num5, Time.get_deltaTime() * 0.2f);
		float num6 = ((!(num3 < 0f)) ? 1 : (-1));
		currentTurnSpeed = num4 * turnScale * num6;
		((Component)this).get_transform().Rotate(Vector3.get_up(), Time.get_deltaTime() * currentTurnSpeed, (Space)0);
		currentThrottle = Mathf.Lerp(currentThrottle, num, Time.get_deltaTime() * 0.2f);
		currentVelocity = ((Component)this).get_transform().get_forward() * (8f * currentThrottle);
		Transform transform = ((Component)this).get_transform();
		transform.set_position(transform.get_position() + currentVelocity * Time.get_deltaTime());
		if (Vector3.Distance(((Component)this).get_transform().get_position(), val) < 80f)
		{
			targetNodeIndex++;
			if (targetNodeIndex >= TerrainMeta.Path.OceanPatrolFar.Count)
			{
				targetNodeIndex = 0;
			}
		}
	}

	public int GetClosestNodeToUs()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		int result = 0;
		float num = float.PositiveInfinity;
		for (int i = 0; i < TerrainMeta.Path.OceanPatrolFar.Count; i++)
		{
			Vector3 val = TerrainMeta.Path.OceanPatrolFar[i];
			float num2 = Vector3.Distance(((Component)this).get_transform().get_position(), val);
			if (num2 < num)
			{
				result = i;
				num = num2;
			}
		}
		return result;
	}

	public override Vector3 GetLocalVelocityServer()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return currentVelocity;
	}

	public override Quaternion GetAngularVelocityServer()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return Quaternion.Euler(0f, currentTurnSpeed, 0f);
	}

	public override float InheritedVelocityScale()
	{
		return 1f;
	}

	public override bool BlocksWaterFor(BasePlayer player)
	{
		return true;
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("CargoShip.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}
}
