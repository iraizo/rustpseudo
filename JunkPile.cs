using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using UnityEngine;

public class JunkPile : BaseEntity
{
	public GameObjectRef sinkEffect;

	public SpawnGroup[] spawngroups;

	private const float lifetimeMinutes = 30f;

	protected bool isSinking;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("JunkPile.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		((FacepunchBehaviour)this).Invoke((Action)TimeOut, 1800f);
		((FacepunchBehaviour)this).InvokeRepeating((Action)CheckEmpty, 10f, 30f);
		((FacepunchBehaviour)this).Invoke((Action)SpawnInitial, 1f);
		isSinking = false;
	}

	private void SpawnInitial()
	{
		SpawnGroup[] array = spawngroups;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SpawnInitial();
		}
	}

	public bool SpawnGroupsEmpty()
	{
		SpawnGroup[] array = spawngroups;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].currentPopulation > 0)
			{
				return false;
			}
		}
		return true;
	}

	public void CheckEmpty()
	{
		if (SpawnGroupsEmpty() && !PlayersNearby())
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)CheckEmpty);
			SinkAndDestroy();
		}
	}

	public bool PlayersNearby()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		List<BasePlayer> list = Pool.GetList<BasePlayer>();
		Vis.Entities(((Component)this).get_transform().get_position(), TimeoutPlayerCheckRadius(), list, 131072, (QueryTriggerInteraction)2);
		bool result = false;
		foreach (BasePlayer item in list)
		{
			if (!item.IsSleeping() && item.IsAlive())
			{
				result = true;
				break;
			}
		}
		Pool.FreeList<BasePlayer>(ref list);
		return result;
	}

	public virtual float TimeoutPlayerCheckRadius()
	{
		return 15f;
	}

	public void TimeOut()
	{
		if (PlayersNearby())
		{
			((FacepunchBehaviour)this).Invoke((Action)TimeOut, 30f);
		}
		SpawnGroupsEmpty();
		SinkAndDestroy();
	}

	public void SinkAndDestroy()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		((FacepunchBehaviour)this).CancelInvoke((Action)SinkAndDestroy);
		SpawnGroup[] array = spawngroups;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Clear();
		}
		SetFlag(Flags.Reserved8, b: true, recursive: true);
		ClientRPC(null, "CLIENT_StartSink");
		Transform transform = ((Component)this).get_transform();
		transform.set_position(transform.get_position() - new Vector3(0f, 5f, 0f));
		isSinking = true;
		((FacepunchBehaviour)this).Invoke((Action)KillMe, 22f);
	}

	public void KillMe()
	{
		Kill();
	}
}
