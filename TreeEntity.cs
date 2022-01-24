using System;
using ConVar;
using Network;
using UnityEngine;

public class TreeEntity : ResourceEntity, IPrefabPreProcess
{
	[Header("Falling")]
	public bool fallOnKilled = true;

	public float fallDuration = 1.5f;

	public GameObjectRef fallStartSound;

	public GameObjectRef fallImpactSound;

	public GameObjectRef fallImpactParticles;

	public SoundDefinition fallLeavesLoopDef;

	[NonSerialized]
	public bool[] usedHeights = new bool[20];

	public bool impactSoundPlayed;

	private float treeDistanceUponFalling;

	public GameObjectRef prefab;

	public bool hasBonusGame = true;

	public GameObjectRef bonusHitEffect;

	public GameObjectRef bonusHitSound;

	public Collider serverCollider;

	public Collider clientCollider;

	public SoundDefinition smallCrackSoundDef;

	public SoundDefinition medCrackSoundDef;

	private float lastAttackDamage;

	[NonSerialized]
	protected BaseEntity xMarker;

	private int currentBonusLevel;

	private float lastDirection = -1f;

	private float lastHitTime;

	private int lastHitMarkerIndex = -1;

	private float nextBirdTime;

	private uint birdCycleIndex;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("TreeEntity.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override void ResetState()
	{
		base.ResetState();
	}

	public override float BoundsPadding()
	{
		return 1f;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		lastDirection = ((Random.Range(0, 2) != 0) ? 1 : (-1));
		TreeManager.OnTreeSpawned(this);
	}

	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		CleanupMarker();
		TreeManager.OnTreeDestroyed(this);
	}

	public bool DidHitMarker(HitInfo info)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)xMarker == (Object)null)
		{
			return false;
		}
		if (PrefabAttribute.server.Find<TreeMarkerData>(prefabID) != null)
		{
			Bounds val = default(Bounds);
			((Bounds)(ref val))._002Ector(((Component)xMarker).get_transform().get_position(), Vector3.get_one() * 0.2f);
			if (((Bounds)(ref val)).Contains(info.HitPositionWorld))
			{
				return true;
			}
		}
		else
		{
			Vector3 val2 = Vector3Ex.Direction2D(((Component)this).get_transform().get_position(), ((Component)xMarker).get_transform().get_position());
			Vector3 attackNormal = info.attackNormal;
			float num = Vector3.Dot(val2, attackNormal);
			float num2 = Vector3.Distance(((Component)xMarker).get_transform().get_position(), info.HitPositionWorld);
			if (num >= 0.3f && num2 <= 0.2f)
			{
				return true;
			}
		}
		return false;
	}

	public void StartBonusGame()
	{
		if (((FacepunchBehaviour)this).IsInvoking((Action)StopBonusGame))
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)StopBonusGame);
		}
		((FacepunchBehaviour)this).Invoke((Action)StopBonusGame, 60f);
	}

	public void StopBonusGame()
	{
		CleanupMarker();
		lastHitTime = 0f;
		currentBonusLevel = 0;
	}

	public bool BonusActive()
	{
		return (Object)(object)xMarker != (Object)null;
	}

	private void DoBirds()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		if (!base.isClient && !(Time.get_realtimeSinceStartup() < nextBirdTime) && !(((Bounds)(ref bounds)).get_extents().y < 6f))
		{
			uint num = net.ID + birdCycleIndex;
			if (SeedRandom.Range(ref num, 0, 2) == 0)
			{
				Effect.server.Run("assets/prefabs/npc/birds/birdemission.prefab", ((Component)this).get_transform().get_position() + Vector3.get_up() * Random.Range(((Bounds)(ref bounds)).get_extents().y * 0.65f, ((Bounds)(ref bounds)).get_extents().y * 0.9f), Vector3.get_up());
			}
			birdCycleIndex++;
			nextBirdTime = Time.get_realtimeSinceStartup() + 90f;
		}
	}

	public override void OnAttacked(HitInfo info)
	{
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		bool canGather = info.CanGather;
		float num = Time.get_time() - lastHitTime;
		lastHitTime = Time.get_time();
		DoBirds();
		if (!hasBonusGame || !canGather || (Object)(object)info.Initiator == (Object)null || (BonusActive() && !DidHitMarker(info)))
		{
			base.OnAttacked(info);
			return;
		}
		if ((Object)(object)xMarker != (Object)null && !info.DidGather && info.gatherScale > 0f)
		{
			xMarker.ClientRPC(null, "MarkerHit", currentBonusLevel);
			currentBonusLevel++;
			info.gatherScale = 1f + Mathf.Clamp((float)currentBonusLevel * 0.125f, 0f, 1f);
		}
		Vector3 val = (((Object)(object)xMarker != (Object)null) ? ((Component)xMarker).get_transform().get_position() : info.HitPositionWorld);
		CleanupMarker();
		TreeMarkerData treeMarkerData = PrefabAttribute.server.Find<TreeMarkerData>(prefabID);
		if (treeMarkerData != null)
		{
			Vector3 nearbyPoint = treeMarkerData.GetNearbyPoint(((Component)this).get_transform().InverseTransformPoint(val), ref lastHitMarkerIndex, out var normal);
			nearbyPoint = ((Component)this).get_transform().TransformPoint(nearbyPoint);
			Quaternion rot = QuaternionEx.LookRotationNormal(((Component)this).get_transform().TransformDirection(normal));
			xMarker = GameManager.server.CreateEntity("assets/content/nature/treesprefabs/trees/effects/tree_marking_nospherecast.prefab", nearbyPoint, rot);
		}
		else
		{
			Vector3 val2 = Vector3Ex.Direction2D(((Component)this).get_transform().get_position(), val);
			Vector3 val3 = Vector3.Cross(val2, Vector3.get_up());
			float num2 = lastDirection;
			float num3 = Random.Range(0.5f, 0.5f);
			Vector3 val4 = Vector3.Lerp(-val2, val3 * num2, num3);
			Vector3 val5 = ((Component)this).get_transform().InverseTransformDirection(((Vector3)(ref val4)).get_normalized()) * 2.5f;
			val5 = ((Component)this).get_transform().InverseTransformPoint(GetCollider().ClosestPoint(((Component)this).get_transform().TransformPoint(val5)));
			Vector3 val6 = ((Component)this).get_transform().TransformPoint(val5);
			Vector3 val7 = ((Component)this).get_transform().InverseTransformPoint(info.HitPositionWorld);
			val5.y = val7.y;
			Vector3 val8 = ((Component)this).get_transform().InverseTransformPoint(info.Initiator.CenterPoint());
			float num4 = Mathf.Max(0.75f, val8.y);
			float num5 = val8.y + 0.5f;
			val5.y = Mathf.Clamp(val5.y + Random.Range(0.1f, 0.2f) * ((Random.Range(0, 2) == 0) ? (-1f) : 1f), num4, num5);
			Vector3 val9 = Vector3Ex.Direction2D(((Component)this).get_transform().get_position(), val6);
			Vector3 val10 = val9;
			val9 = ((Component)this).get_transform().InverseTransformDirection(val9);
			Quaternion val11 = QuaternionEx.LookRotationNormal(-val9, Vector3.get_zero());
			val5 = ((Component)this).get_transform().TransformPoint(val5);
			val11 = QuaternionEx.LookRotationNormal(-val10, Vector3.get_zero());
			val5 = GetCollider().ClosestPoint(val5);
			Line val12 = default(Line);
			((Line)(ref val12))._002Ector(((Component)GetCollider()).get_transform().TransformPoint(new Vector3(0f, 10f, 0f)), ((Component)GetCollider()).get_transform().TransformPoint(new Vector3(0f, -10f, 0f)));
			val11 = QuaternionEx.LookRotationNormal(-Vector3Ex.Direction(((Line)(ref val12)).ClosestPoint(val5), val5));
			xMarker = GameManager.server.CreateEntity("assets/content/nature/treesprefabs/trees/effects/tree_marking.prefab", val5, val11);
		}
		xMarker.Spawn();
		if (num > 5f)
		{
			StartBonusGame();
		}
		base.OnAttacked(info);
		if (health > 0f)
		{
			lastAttackDamage = info.damageTypes.Total();
			int num6 = Mathf.CeilToInt(health / lastAttackDamage);
			if (num6 < 2)
			{
				ClientRPC(null, "CrackSound", 1);
			}
			else if (num6 < 5)
			{
				ClientRPC(null, "CrackSound", 0);
			}
		}
	}

	public void CleanupMarker()
	{
		if (Object.op_Implicit((Object)(object)xMarker))
		{
			xMarker.Kill();
		}
		xMarker = null;
	}

	public Collider GetCollider()
	{
		if (base.isServer)
		{
			if (!((Object)(object)serverCollider == (Object)null))
			{
				return serverCollider;
			}
			return (Collider)(object)((Component)this).GetComponentInChildren<CapsuleCollider>();
		}
		if (!((Object)(object)clientCollider == (Object)null))
		{
			return clientCollider;
		}
		return ((Component)this).GetComponent<Collider>();
	}

	public override void OnKilled(HitInfo info)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (isKilled)
		{
			return;
		}
		isKilled = true;
		CleanupMarker();
		if (fallOnKilled)
		{
			Collider collider = GetCollider();
			if (Object.op_Implicit((Object)(object)collider))
			{
				collider.set_enabled(false);
			}
			ClientRPC<Vector3>(null, "TreeFall", info.attackNormal);
			((FacepunchBehaviour)this).Invoke((Action)DelayedKill, fallDuration + 1f);
		}
		else
		{
			DelayedKill();
		}
	}

	public void DelayedKill()
	{
		Kill();
	}

	public override void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(preProcess, rootObj, name, serverside, clientside, bundling);
		if (serverside)
		{
			globalBroadcast = Tree.global_broadcast;
		}
	}
}
