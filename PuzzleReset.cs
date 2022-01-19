using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Rust;
using UnityEngine;

public class PuzzleReset : FacepunchBehaviour
{
	public SpawnGroup[] respawnGroups;

	public IOEntity[] resetEnts;

	public GameObject[] resetObjects;

	public bool playersBlockReset;

	public bool CheckSleepingAIZForPlayers;

	public float playerDetectionRadius;

	public float playerHeightDetectionMinMax = -1f;

	public Transform playerDetectionOrigin;

	public float timeBetweenResets = 30f;

	public bool scaleWithServerPopulation;

	[HideInInspector]
	public Vector3[] resetPositions;

	private AIInformationZone zone;

	private float resetTimeElapsed;

	private float resetTickTime = 10f;

	public float GetResetSpacing()
	{
		return timeBetweenResets * (scaleWithServerPopulation ? (1f - SpawnHandler.PlayerLerp(Spawn.min_rate, Spawn.max_rate)) : 1f);
	}

	public void Start()
	{
		if (timeBetweenResets != float.PositiveInfinity)
		{
			ResetTimer();
		}
	}

	public void ResetTimer()
	{
		resetTimeElapsed = 0f;
		((FacepunchBehaviour)this).CancelInvoke((Action)ResetTick);
		((FacepunchBehaviour)this).InvokeRandomized((Action)ResetTick, Random.Range(0f, 1f), resetTickTime, 0.5f);
	}

	public bool PassesResetCheck()
	{
		if (playersBlockReset)
		{
			if (CheckSleepingAIZForPlayers)
			{
				return AIZSleeping();
			}
			return !PlayersWithinDistance();
		}
		return true;
	}

	private bool AIZSleeping()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)zone != (Object)null)
		{
			if (!zone.PointInside(((Component)this).get_transform().get_position()))
			{
				zone = AIInformationZone.GetForPoint(((Component)this).get_transform().get_position());
			}
		}
		else
		{
			zone = AIInformationZone.GetForPoint(((Component)this).get_transform().get_position());
		}
		if ((Object)(object)zone == (Object)null)
		{
			return false;
		}
		return zone.Sleeping;
	}

	private bool PlayersWithinDistance()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BasePlayer current = enumerator.get_Current();
				if (!current.IsSleeping() && current.IsAlive() && Vector3.Distance(((Component)current).get_transform().get_position(), playerDetectionOrigin.get_position()) < playerDetectionRadius)
				{
					return true;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		return false;
	}

	public void ResetTick()
	{
		if (PassesResetCheck())
		{
			resetTimeElapsed += resetTickTime;
		}
		if (resetTimeElapsed > GetResetSpacing())
		{
			resetTimeElapsed = 0f;
			DoReset();
		}
	}

	public void CleanupSleepers()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)playerDetectionOrigin == (Object)null || BasePlayer.sleepingPlayerList == null)
		{
			return;
		}
		for (int num = BasePlayer.sleepingPlayerList.get_Count() - 1; num >= 0; num--)
		{
			BasePlayer basePlayer = BasePlayer.sleepingPlayerList.get_Item(num);
			if (!((Object)(object)basePlayer == (Object)null) && basePlayer.IsSleeping() && Vector3.Distance(((Component)basePlayer).get_transform().get_position(), playerDetectionOrigin.get_position()) <= playerDetectionRadius)
			{
				basePlayer.Hurt(1000f, DamageType.Suicide, basePlayer, useProtection: false);
			}
		}
	}

	public void DoReset()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		CleanupSleepers();
		IOEntity component = ((Component)this).GetComponent<IOEntity>();
		if ((Object)(object)component != (Object)null)
		{
			ResetIOEntRecursive(component, Time.get_frameCount());
			component.MarkDirty();
		}
		else if (resetPositions != null)
		{
			Vector3[] array = resetPositions;
			foreach (Vector3 val in array)
			{
				Vector3 position = ((Component)this).get_transform().TransformPoint(val);
				List<IOEntity> list = Pool.GetList<IOEntity>();
				Vis.Entities(position, 0.5f, list, 1235288065, (QueryTriggerInteraction)1);
				foreach (IOEntity item in list)
				{
					if (item.IsRootEntity() && item.isServer)
					{
						ResetIOEntRecursive(item, Time.get_frameCount());
						item.MarkDirty();
					}
				}
				Pool.FreeList<IOEntity>(ref list);
			}
		}
		List<SpawnGroup> list2 = Pool.GetList<SpawnGroup>();
		Vis.Components<SpawnGroup>(((Component)this).get_transform().get_position(), 1f, list2, 262144, (QueryTriggerInteraction)2);
		foreach (SpawnGroup item2 in list2)
		{
			if (!((Object)(object)item2 == (Object)null))
			{
				item2.Clear();
				item2.DelayedSpawn();
			}
		}
		Pool.FreeList<SpawnGroup>(ref list2);
		GameObject[] array2 = resetObjects;
		foreach (GameObject val2 in array2)
		{
			if ((Object)(object)val2 != (Object)null)
			{
				val2.SendMessage("OnPuzzleReset", (SendMessageOptions)1);
			}
		}
	}

	public static void ResetIOEntRecursive(IOEntity target, int resetIndex)
	{
		if (target.lastResetIndex == resetIndex)
		{
			return;
		}
		target.lastResetIndex = resetIndex;
		target.ResetIOState();
		IOEntity.IOSlot[] outputs = target.outputs;
		foreach (IOEntity.IOSlot iOSlot in outputs)
		{
			if ((Object)(object)iOSlot.connectedTo.Get() != (Object)null && (Object)(object)iOSlot.connectedTo.Get() != (Object)(object)target)
			{
				ResetIOEntRecursive(iOSlot.connectedTo.Get(), resetIndex);
			}
		}
	}

	public PuzzleReset()
		: this()
	{
	}
}
