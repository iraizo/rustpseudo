using System;
using ConVar;
using UnityEngine;

public class SupplyDrop : LootContainer
{
	public GameObjectRef parachutePrefab;

	private const Flags FlagNightLight = Flags.Reserved1;

	private BaseEntity parachute;

	public override void ServerInit()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		if (parachutePrefab.isValid)
		{
			parachute = GameManager.server.CreateEntity(parachutePrefab.resourcePath);
		}
		if (Object.op_Implicit((Object)(object)parachute))
		{
			parachute.SetParent(this, "parachute_attach");
			parachute.Spawn();
		}
		isLootable = false;
		((FacepunchBehaviour)this).Invoke((Action)MakeLootable, 300f);
		((FacepunchBehaviour)this).InvokeRepeating((Action)CheckNightLight, 0f, 30f);
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		RemoveParachute();
	}

	private void RemoveParachute()
	{
		if (Object.op_Implicit((Object)(object)parachute))
		{
			parachute.Kill();
			parachute = null;
		}
	}

	public void MakeLootable()
	{
		isLootable = true;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (((1 << ((Component)collision.get_collider()).get_gameObject().get_layer()) & 0x40A10111) > 0)
		{
			RemoveParachute();
			MakeLootable();
		}
	}

	private void CheckNightLight()
	{
		SetFlag(Flags.Reserved1, Env.time > 20f || Env.time < 7f);
	}
}
