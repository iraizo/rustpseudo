using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;

public class Marketplace : BaseEntity
{
	[Header("Marketplace")]
	public GameObjectRef terminalPrefab;

	public Transform[] terminalPoints;

	public Transform droneLaunchPoint;

	public GameObjectRef deliveryDronePrefab;

	public EntityRef<MarketTerminal>[] terminalEntities;

	public uint SendDrone(BasePlayer player, MarketTerminal sourceTerminal, VendingMachine vendingMachine)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)sourceTerminal == (Object)null || (Object)(object)vendingMachine == (Object)null)
		{
			return 0u;
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(deliveryDronePrefab?.resourcePath, droneLaunchPoint.get_position(), droneLaunchPoint.get_rotation());
		DeliveryDrone deliveryDrone;
		if ((deliveryDrone = baseEntity as DeliveryDrone) == null)
		{
			baseEntity.Kill();
			return 0u;
		}
		deliveryDrone.OwnerID = player.userID;
		deliveryDrone.Spawn();
		deliveryDrone.Setup(this, sourceTerminal, vendingMachine);
		return deliveryDrone.net.ID;
	}

	public void ReturnDrone(DeliveryDrone deliveryDrone)
	{
		if (deliveryDrone.sourceTerminal.TryGet(serverside: true, out var entity))
		{
			entity.CompleteOrder(deliveryDrone.targetVendingMachine.uid);
		}
		deliveryDrone.Kill();
	}

	public override void Spawn()
	{
		base.Spawn();
		if (!Application.isLoadingSave)
		{
			SpawnSubEntities();
		}
	}

	private void SpawnSubEntities()
	{
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		if (!base.isServer)
		{
			return;
		}
		if (terminalEntities != null && terminalEntities.Length > terminalPoints.Length)
		{
			for (int i = terminalPoints.Length; i < terminalEntities.Length; i++)
			{
				if (terminalEntities[i].TryGet(serverside: true, out var entity))
				{
					entity.Kill();
				}
			}
		}
		Array.Resize(ref terminalEntities, terminalPoints.Length);
		for (int j = 0; j < terminalPoints.Length; j++)
		{
			Transform val = terminalPoints[j];
			if (!terminalEntities[j].TryGet(serverside: true, out var _))
			{
				BaseEntity baseEntity = GameManager.server.CreateEntity(terminalPrefab?.resourcePath, val.get_position(), val.get_rotation());
				baseEntity.SetParent(this, worldPositionStays: true);
				baseEntity.Spawn();
				MarketTerminal marketTerminal;
				if ((marketTerminal = baseEntity as MarketTerminal) == null)
				{
					Debug.LogError((object)("Marketplace.terminalPrefab did not spawn a MarketTerminal (it spawned " + ((object)baseEntity).GetType().FullName + ")"));
					baseEntity.Kill();
				}
				else
				{
					marketTerminal.Setup(this);
					terminalEntities[j].Set(marketTerminal);
				}
			}
		}
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.subEntityList != null)
		{
			List<uint> subEntityIds = info.msg.subEntityList.subEntityIds;
			Array.Resize(ref terminalEntities, subEntityIds.Count);
			for (int i = 0; i < subEntityIds.Count; i++)
			{
				terminalEntities[i] = new EntityRef<MarketTerminal>(subEntityIds[i]);
			}
		}
		SpawnSubEntities();
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.subEntityList = Pool.Get<SubEntityList>();
		info.msg.subEntityList.subEntityIds = Pool.GetList<uint>();
		if (terminalEntities != null)
		{
			for (int i = 0; i < terminalEntities.Length; i++)
			{
				info.msg.subEntityList.subEntityIds.Add(terminalEntities[i].uid);
			}
		}
	}
}
