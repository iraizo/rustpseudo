using System;
using System.Collections.Generic;
using ConVar;
using Network;
using UnityEngine;

public class XMasRefill : BaseEntity
{
	public GameObjectRef[] giftPrefabs;

	public List<BasePlayer> goodKids;

	public List<Stocking> stockings;

	public AudioSource bells;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("XMasRefill.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public float GiftRadius()
	{
		return XMas.spawnRange;
	}

	public int GiftsPerPlayer()
	{
		return XMas.giftsPerPlayer;
	}

	public int GiftSpawnAttempts()
	{
		return XMas.giftsPerPlayer * XMas.spawnAttempts;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (!XMas.enabled)
		{
			((FacepunchBehaviour)this).Invoke((Action)RemoveMe, 0.1f);
			return;
		}
		goodKids = ((BasePlayer.activePlayerList != null) ? new List<BasePlayer>((IEnumerable<BasePlayer>)BasePlayer.activePlayerList) : new List<BasePlayer>());
		stockings = ((Stocking.stockings != null) ? new List<Stocking>((IEnumerable<Stocking>)Stocking.stockings.get_Values()) : new List<Stocking>());
		((FacepunchBehaviour)this).Invoke((Action)RemoveMe, 60f);
		((FacepunchBehaviour)this).InvokeRepeating((Action)DistributeLoot, 3f, 0.02f);
		((FacepunchBehaviour)this).Invoke((Action)SendBells, 0.5f);
	}

	public void SendBells()
	{
		ClientRPC(null, "PlayBells");
	}

	public void RemoveMe()
	{
		if (goodKids.Count == 0 && stockings.Count == 0)
		{
			Kill();
		}
		else
		{
			((FacepunchBehaviour)this).Invoke((Action)RemoveMe, 60f);
		}
	}

	public void DistributeLoot()
	{
		if (goodKids.Count > 0)
		{
			BasePlayer basePlayer = null;
			foreach (BasePlayer goodKid in goodKids)
			{
				if (!goodKid.IsSleeping() && !goodKid.IsWounded() && goodKid.IsAlive())
				{
					basePlayer = goodKid;
					break;
				}
			}
			if (Object.op_Implicit((Object)(object)basePlayer))
			{
				DistributeGiftsForPlayer(basePlayer);
				goodKids.Remove(basePlayer);
			}
		}
		if (stockings.Count > 0)
		{
			Stocking stocking = stockings[0];
			if ((Object)(object)stocking != (Object)null)
			{
				stocking.SpawnLoot();
			}
			stockings.RemoveAt(0);
		}
	}

	protected bool DropToGround(ref Vector3 pos)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		int num = 1235288065;
		int num2 = 8454144;
		if (Object.op_Implicit((Object)(object)TerrainMeta.TopologyMap) && ((uint)TerrainMeta.TopologyMap.GetTopology(pos) & 0x14080u) != 0)
		{
			return false;
		}
		if (Object.op_Implicit((Object)(object)TerrainMeta.HeightMap) && Object.op_Implicit((Object)(object)TerrainMeta.Collision) && !TerrainMeta.Collision.GetIgnore(pos))
		{
			float height = TerrainMeta.HeightMap.GetHeight(pos);
			pos.y = Mathf.Max(pos.y, height);
		}
		if (!TransformUtil.GetGroundInfo(pos, out var hitOut, 80f, LayerMask.op_Implicit(num)))
		{
			return false;
		}
		if (((1 << ((Component)((RaycastHit)(ref hitOut)).get_transform()).get_gameObject().get_layer()) & num2) == 0)
		{
			return false;
		}
		pos = ((RaycastHit)(ref hitOut)).get_point();
		return true;
	}

	public bool DistributeGiftsForPlayer(BasePlayer player)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		int num = GiftsPerPlayer();
		int num2 = GiftSpawnAttempts();
		for (int i = 0; i < num2; i++)
		{
			if (num <= 0)
			{
				break;
			}
			Vector2 val = Random.get_insideUnitCircle() * GiftRadius();
			Vector3 pos = ((Component)player).get_transform().get_position() + new Vector3(val.x, 10f, val.y);
			Quaternion rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
			if (DropToGround(ref pos))
			{
				string resourcePath = giftPrefabs[Random.Range(0, giftPrefabs.Length)].resourcePath;
				BaseEntity baseEntity = GameManager.server.CreateEntity(resourcePath, pos, rot);
				if (Object.op_Implicit((Object)(object)baseEntity))
				{
					baseEntity.Spawn();
					num--;
				}
			}
		}
		return true;
	}
}
