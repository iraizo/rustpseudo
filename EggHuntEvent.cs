using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using ProtoBuf;
using UnityEngine;

public class EggHuntEvent : BaseHuntEvent
{
	public class EggHunter
	{
		public ulong userid;

		public string displayName;

		public int numEggs;
	}

	public float warmupTime = 10f;

	public float cooldownTime = 10f;

	public float warnTime = 20f;

	public float timeAlive;

	public static EggHuntEvent serverEvent = null;

	public static EggHuntEvent clientEvent = null;

	[NonSerialized]
	public static float durationSeconds = 180f;

	private Dictionary<ulong, EggHunter> _eggHunters = new Dictionary<ulong, EggHunter>();

	public List<CollectableEasterEgg> _spawnedEggs = new List<CollectableEasterEgg>();

	public ItemAmount[] placementAwards;

	public bool IsEventActive()
	{
		if (timeAlive > warmupTime)
		{
			return timeAlive - warmupTime < durationSeconds;
		}
		return false;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (Object.op_Implicit((Object)(object)serverEvent) && base.isServer)
		{
			serverEvent.Kill();
			serverEvent = null;
		}
		serverEvent = this;
		((FacepunchBehaviour)this).Invoke((Action)StartEvent, warmupTime);
	}

	public void StartEvent()
	{
		SpawnEggs();
	}

	public void SpawnEggsAtPoint(int numEggs, Vector3 pos, Vector3 aimDir, float minDist = 1f, float maxDist = 2f)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < numEggs; i++)
		{
			Vector3 val = pos;
			aimDir = ((!(aimDir == Vector3.get_zero())) ? AimConeUtil.GetModifiedAimConeDirection(90f, aimDir) : Random.get_onUnitSphere());
			val = pos + Vector3Ex.Direction2D(pos + aimDir * 10f, pos) * Random.Range(minDist, maxDist);
			val.y = TerrainMeta.HeightMap.GetHeight(val);
			CollectableEasterEgg collectableEasterEgg = GameManager.server.CreateEntity(HuntablePrefab[Random.Range(0, HuntablePrefab.Length)].resourcePath, val) as CollectableEasterEgg;
			collectableEasterEgg.Spawn();
			_spawnedEggs.Add(collectableEasterEgg);
		}
	}

	[ContextMenu("SpawnDebug")]
	public void SpawnEggs()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BasePlayer current = enumerator.get_Current();
				SpawnEggsAtPoint(Random.Range(4, 6) + Mathf.RoundToInt(current.eggVision), ((Component)current).get_transform().get_position(), current.eyes.BodyForward(), 15f, 25f);
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
	}

	public void RandPickup()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				enumerator.get_Current();
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
	}

	public void EggCollected(BasePlayer player)
	{
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		EggHunter eggHunter = null;
		if (_eggHunters.ContainsKey(player.userID))
		{
			eggHunter = _eggHunters[player.userID];
		}
		else
		{
			eggHunter = new EggHunter();
			eggHunter.displayName = player.displayName;
			eggHunter.userid = player.userID;
			_eggHunters.Add(player.userID, eggHunter);
		}
		if (eggHunter == null)
		{
			Debug.LogWarning((object)"Easter error");
			return;
		}
		eggHunter.numEggs++;
		QueueUpdate();
		int num = ((!((float)Mathf.RoundToInt(player.eggVision) * 0.5f < 1f)) ? 1 : Random.Range(0, 2));
		SpawnEggsAtPoint(Random.Range(1 + num, 2 + num), ((Component)player).get_transform().get_position(), player.eyes.BodyForward(), 15f, 25f);
	}

	public void QueueUpdate()
	{
		if (!((FacepunchBehaviour)this).IsInvoking((Action)DoNetworkUpdate))
		{
			((FacepunchBehaviour)this).Invoke((Action)DoNetworkUpdate, 2f);
		}
	}

	public void DoNetworkUpdate()
	{
		SendNetworkUpdate();
	}

	public static void Sort(List<EggHunter> hunterList)
	{
		hunterList.Sort((EggHunter a, EggHunter b) => b.numEggs.CompareTo(a.numEggs));
	}

	public List<EggHunter> GetTopHunters()
	{
		List<EggHunter> list = Pool.GetList<EggHunter>();
		foreach (KeyValuePair<ulong, EggHunter> eggHunter in _eggHunters)
		{
			list.Add(eggHunter.Value);
		}
		Sort(list);
		return list;
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.eggHunt = Pool.Get<EggHunt>();
		List<EggHunter> topHunters = GetTopHunters();
		info.msg.eggHunt.hunters = Pool.GetList<EggHunter>();
		for (int i = 0; i < Mathf.Min(10, topHunters.Count); i++)
		{
			EggHunter val = Pool.Get<EggHunter>();
			val.displayName = topHunters[i].displayName;
			val.numEggs = topHunters[i].numEggs;
			val.playerID = topHunters[i].userid;
			info.msg.eggHunt.hunters.Add(val);
		}
	}

	public void CleanupEggs()
	{
		foreach (CollectableEasterEgg spawnedEgg in _spawnedEggs)
		{
			if ((Object)(object)spawnedEgg != (Object)null)
			{
				spawnedEgg.Kill();
			}
		}
	}

	public void Cooldown()
	{
		((FacepunchBehaviour)this).CancelInvoke((Action)Cooldown);
		Kill();
	}

	public virtual void PrintWinnersAndAward()
	{
		List<EggHunter> topHunters = GetTopHunters();
		if (topHunters.Count > 0)
		{
			EggHunter eggHunter = topHunters[0];
			Chat.Broadcast(eggHunter.displayName + " is the top bunny with " + eggHunter.numEggs + " eggs collected.", "", "#eee", 0uL);
			for (int i = 0; i < topHunters.Count; i++)
			{
				EggHunter eggHunter2 = topHunters[i];
				BasePlayer basePlayer = BasePlayer.FindByID(eggHunter2.userid);
				if (Object.op_Implicit((Object)(object)basePlayer))
				{
					basePlayer.ChatMessage("You placed " + (i + 1) + " of " + topHunters.Count + " with " + topHunters[i].numEggs + " eggs collected.");
				}
				else
				{
					Debug.LogWarning((object)("EggHuntEvent Printwinners could not find player with id :" + eggHunter2.userid));
				}
			}
			for (int j = 0; j < placementAwards.Length && j < topHunters.Count; j++)
			{
				BasePlayer basePlayer2 = BasePlayer.FindByID(topHunters[j].userid);
				if (Object.op_Implicit((Object)(object)basePlayer2))
				{
					basePlayer2.inventory.GiveItem(ItemManager.Create(placementAwards[j].itemDef, (int)placementAwards[j].amount, 0uL), basePlayer2.inventory.containerMain);
					basePlayer2.ChatMessage("You received " + (int)placementAwards[j].amount + "x " + placementAwards[j].itemDef.displayName.english + " as an award!");
				}
			}
		}
		else
		{
			Chat.Broadcast("Wow, no one played so no one won.", "", "#eee", 0uL);
		}
	}

	public override void DestroyShared()
	{
		base.DestroyShared();
		if (base.isServer)
		{
			serverEvent = null;
		}
		else
		{
			clientEvent = null;
		}
	}

	public void Update()
	{
		timeAlive += Time.get_deltaTime();
		if (base.isServer && !base.IsDestroyed)
		{
			if (timeAlive - warmupTime > durationSeconds - warnTime)
			{
				SetFlag(Flags.Reserved1, b: true);
			}
			if (timeAlive - warmupTime > durationSeconds && !((FacepunchBehaviour)this).IsInvoking((Action)Cooldown))
			{
				SetFlag(Flags.Reserved2, b: true);
				CleanupEggs();
				PrintWinnersAndAward();
				((FacepunchBehaviour)this).Invoke((Action)Cooldown, 10f);
			}
		}
	}

	public float GetTimeRemaining()
	{
		float num = durationSeconds - timeAlive;
		if (num < 0f)
		{
			num = 0f;
		}
		return num;
	}
}
