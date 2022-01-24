using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

public class NPCShopKeeper : NPCPlayer
{
	public EntityRef invisibleVendingMachineRef;

	public InvisibleVendingMachine machine;

	private float greetDir;

	private Vector3 initialFacingDir;

	private BasePlayer lastWavedAtPlayer;

	public InvisibleVendingMachine GetVendingMachine()
	{
		if (!invisibleVendingMachineRef.IsValid(base.isServer))
		{
			return null;
		}
		return ((Component)invisibleVendingMachineRef.Get(base.isServer)).GetComponent<InvisibleVendingMachine>();
	}

	public void OnDrawGizmos()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.set_color(Color.get_green());
		Gizmos.DrawCube(((Component)this).get_transform().get_position() + Vector3.get_up() * 1f, new Vector3(0.5f, 1f, 0.5f));
	}

	public override void UpdateProtectionFromClothing()
	{
	}

	public override void Hurt(HitInfo info)
	{
	}

	public override void ServerInit()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		initialFacingDir = ((Component)this).get_transform().get_rotation() * Vector3.get_forward();
		((FacepunchBehaviour)this).Invoke((Action)DelayedSleepEnd, 3f);
		SetAimDirection(((Component)this).get_transform().get_rotation() * Vector3.get_forward());
		((FacepunchBehaviour)this).InvokeRandomized((Action)Greeting, Random.Range(5f, 10f), 5f, Random.Range(0f, 2f));
		if (invisibleVendingMachineRef.IsValid(serverside: true) && (Object)(object)machine == (Object)null)
		{
			machine = GetVendingMachine();
		}
		else if ((Object)(object)machine != (Object)null && !invisibleVendingMachineRef.IsValid(serverside: true))
		{
			invisibleVendingMachineRef.Set(machine);
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.shopKeeper = Pool.Get<ShopKeeper>();
		info.msg.shopKeeper.vendingRef = invisibleVendingMachineRef.uid;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.shopKeeper != null)
		{
			invisibleVendingMachineRef.uid = info.msg.shopKeeper.vendingRef;
		}
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
	}

	public void DelayedSleepEnd()
	{
		EndSleeping();
	}

	public void GreetPlayer(BasePlayer player)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)player != (Object)null)
		{
			SignalBroadcast(Signal.Gesture, "wave");
			SetAimDirection(Vector3Ex.Direction2D(player.eyes.position, eyes.position));
			lastWavedAtPlayer = player;
		}
		else
		{
			SetAimDirection(initialFacingDir);
		}
	}

	public void Greeting()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		List<BasePlayer> list = Pool.GetList<BasePlayer>();
		Vis.Entities(((Component)this).get_transform().get_position(), 10f, list, 131072, (QueryTriggerInteraction)2);
		((Component)this).get_transform().get_position();
		BasePlayer basePlayer = null;
		foreach (BasePlayer item in list)
		{
			if (!item.isClient && !item.IsNpc && !((Object)(object)item == (Object)(object)this) && item.IsVisible(eyes.position) && !((Object)(object)item == (Object)(object)lastWavedAtPlayer) && !(Vector3.Dot(Vector3Ex.Direction2D(item.eyes.position, eyes.position), initialFacingDir) < 0.2f))
			{
				basePlayer = item;
				break;
			}
		}
		if ((Object)(object)basePlayer == (Object)null && !list.Contains(lastWavedAtPlayer))
		{
			lastWavedAtPlayer = null;
		}
		if ((Object)(object)basePlayer != (Object)null)
		{
			SignalBroadcast(Signal.Gesture, "wave");
			SetAimDirection(Vector3Ex.Direction2D(basePlayer.eyes.position, eyes.position));
			lastWavedAtPlayer = basePlayer;
		}
		else
		{
			SetAimDirection(initialFacingDir);
		}
		Pool.FreeList<BasePlayer>(ref list);
	}
}
