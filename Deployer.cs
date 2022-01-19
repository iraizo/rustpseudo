using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class Deployer : HeldEntity
{
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("Deployer.OnRpcMessage", 0);
		try
		{
			if (rpc == 3001117906u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - DoDeploy "));
				}
				TimeWarning val2 = TimeWarning.New("DoDeploy", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(3001117906u, "DoDeploy", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							RPCMessage msg2 = rPCMessage;
							DoDeploy(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in DoDeploy");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public ItemModDeployable GetModDeployable()
	{
		ItemDefinition ownerItemDefinition = GetOwnerItemDefinition();
		if ((Object)(object)ownerItemDefinition == (Object)null)
		{
			return null;
		}
		return ((Component)ownerItemDefinition).GetComponent<ItemModDeployable>();
	}

	public Deployable GetDeployable()
	{
		ItemModDeployable modDeployable = GetModDeployable();
		if ((Object)(object)modDeployable == (Object)null)
		{
			return null;
		}
		return modDeployable.GetDeployable(this);
	}

	public Quaternion GetDeployedRotation(Vector3 normal, Vector3 placeDir)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		return Quaternion.LookRotation(normal, placeDir) * Quaternion.Euler(90f, 0f, 0f);
	}

	public bool IsPlacementAngleAcceptable(Vector3 pos, Quaternion rot)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = rot * Vector3.get_up();
		if (Mathf.Acos(Vector3.Dot(val, Vector3.get_up())) <= 0.61086524f)
		{
			return true;
		}
		return false;
	}

	public bool CheckPlacement(Deployable deployable, Ray ray, float fDistance)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("Deploy.CheckPlacement", 0);
		try
		{
			RaycastHit val2 = default(RaycastHit);
			if (!Physics.Raycast(ray, ref val2, fDistance, 1235288065))
			{
				return false;
			}
			DeployVolume[] volumes = PrefabAttribute.server.FindAll<DeployVolume>(deployable.prefabID);
			Vector3 point = ((RaycastHit)(ref val2)).get_point();
			Quaternion deployedRotation = GetDeployedRotation(((RaycastHit)(ref val2)).get_normal(), ((Ray)(ref ray)).get_direction());
			if (DeployVolume.Check(point, deployedRotation, volumes))
			{
				return false;
			}
			if (!IsPlacementAngleAcceptable(((RaycastHit)(ref val2)).get_point(), deployedRotation))
			{
				return false;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return true;
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	private void DoDeploy(RPCMessage msg)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (!msg.player.CanInteract())
		{
			return;
		}
		Deployable deployable = GetDeployable();
		if (!(deployable == null))
		{
			Ray ray = msg.read.Ray();
			uint entityID = msg.read.UInt32();
			if (deployable.toSlot)
			{
				DoDeploy_Slot(deployable, ray, entityID);
			}
			else
			{
				DoDeploy_Regular(deployable, ray);
			}
		}
	}

	public void DoDeploy_Slot(Deployable deployable, Ray ray, uint entityID)
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		if (!HasItemAmount())
		{
			return;
		}
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if (!Object.op_Implicit((Object)(object)ownerPlayer))
		{
			return;
		}
		if (!ownerPlayer.CanBuild())
		{
			ownerPlayer.ChatMessage("Building is blocked at player position!");
			return;
		}
		BaseEntity baseEntity = BaseNetworkable.serverEntities.Find(entityID) as BaseEntity;
		if ((Object)(object)baseEntity == (Object)null || !baseEntity.HasSlot(deployable.slot) || (Object)(object)baseEntity.GetSlot(deployable.slot) != (Object)null)
		{
			return;
		}
		if (ownerPlayer.Distance(baseEntity) > 3f)
		{
			ownerPlayer.ChatMessage("Too far away!");
			return;
		}
		if (!ownerPlayer.CanBuild(baseEntity.WorldSpaceBounds()))
		{
			ownerPlayer.ChatMessage("Building is blocked at placement position!");
			return;
		}
		Item ownerItem = GetOwnerItem();
		ItemModDeployable modDeployable = GetModDeployable();
		BaseEntity baseEntity2 = GameManager.server.CreateEntity(modDeployable.entityPrefab.resourcePath);
		if ((Object)(object)baseEntity2 != (Object)null)
		{
			baseEntity2.skinID = ownerItem.skin;
			baseEntity2.SetParent(baseEntity, baseEntity.GetSlotAnchorName(deployable.slot));
			baseEntity2.OwnerID = ownerPlayer.userID;
			baseEntity2.OnDeployed(baseEntity, ownerPlayer, ownerItem);
			baseEntity2.Spawn();
			baseEntity.SetSlot(deployable.slot, baseEntity2);
			if (deployable.placeEffect.isValid)
			{
				Effect.server.Run(deployable.placeEffect.resourcePath, ((Component)baseEntity).get_transform().get_position(), Vector3.get_up());
			}
		}
		modDeployable.OnDeployed(baseEntity2, ownerPlayer);
		UseItemAmount(1);
	}

	public void DoDeploy_Regular(Deployable deployable, Ray ray)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		if (!HasItemAmount())
		{
			return;
		}
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if (!Object.op_Implicit((Object)(object)ownerPlayer))
		{
			return;
		}
		if (!ownerPlayer.CanBuild())
		{
			ownerPlayer.ChatMessage("Building is blocked at player position!");
		}
		else if (ConVar.AntiHack.objectplacement && ownerPlayer.TriggeredAntiHack())
		{
			ownerPlayer.ChatMessage("AntiHack!");
		}
		else
		{
			RaycastHit val = default(RaycastHit);
			if (!CheckPlacement(deployable, ray, 8f) || !Physics.Raycast(ray, ref val, 8f, 1235288065))
			{
				return;
			}
			Vector3 point = ((RaycastHit)(ref val)).get_point();
			Quaternion deployedRotation = GetDeployedRotation(((RaycastHit)(ref val)).get_normal(), ((Ray)(ref ray)).get_direction());
			Item ownerItem = GetOwnerItem();
			ItemModDeployable modDeployable = GetModDeployable();
			if (ownerPlayer.Distance(point) > 3f)
			{
				ownerPlayer.ChatMessage("Too far away!");
				return;
			}
			if (!ownerPlayer.CanBuild(point, deployedRotation, deployable.bounds))
			{
				ownerPlayer.ChatMessage("Building is blocked at placement position!");
				return;
			}
			BaseEntity baseEntity = GameManager.server.CreateEntity(modDeployable.entityPrefab.resourcePath, point, deployedRotation);
			if (!Object.op_Implicit((Object)(object)baseEntity))
			{
				Debug.LogWarning((object)("Couldn't create prefab:" + modDeployable.entityPrefab.resourcePath));
				return;
			}
			baseEntity.skinID = ownerItem.skin;
			((Component)baseEntity).SendMessage("SetDeployedBy", (object)ownerPlayer, (SendMessageOptions)1);
			baseEntity.OwnerID = ownerPlayer.userID;
			baseEntity.Spawn();
			modDeployable.OnDeployed(baseEntity, ownerPlayer);
			UseItemAmount(1);
		}
	}
}
