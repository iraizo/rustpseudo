using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ConVar;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class Planner : HeldEntity
{
	public BaseEntity[] buildableList;

	public bool isTypeDeployable => (Object)(object)GetModDeployable() != (Object)null;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("Planner.OnRpcMessage", 0);
		try
		{
			if (rpc == 1872774636 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - DoPlace "));
				}
				TimeWarning val2 = TimeWarning.New("DoPlace", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(1872774636u, "DoPlace", this, player))
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
							DoPlace(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in DoPlace");
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

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	private void DoPlace(RPCMessage msg)
	{
		if (msg.player.CanInteract())
		{
			CreateBuilding val = CreateBuilding.Deserialize((Stream)(object)msg.read);
			try
			{
				DoBuild(val);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
	}

	public Socket_Base FindSocket(string name, uint prefabIDToFind)
	{
		return Enumerable.FirstOrDefault<Socket_Base>((IEnumerable<Socket_Base>)PrefabAttribute.server.FindAll<Socket_Base>(prefabIDToFind), (Func<Socket_Base, bool>)((Socket_Base s) => s.socketName == name));
	}

	public void DoBuild(CreateBuilding msg)
	{
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if (!Object.op_Implicit((Object)(object)ownerPlayer))
		{
			return;
		}
		if (ConVar.AntiHack.objectplacement && ownerPlayer.TriggeredAntiHack())
		{
			ownerPlayer.ChatMessage("AntiHack!");
			return;
		}
		Construction construction = PrefabAttribute.server.Find<Construction>(msg.blockID);
		if (construction == null)
		{
			ownerPlayer.ChatMessage("Couldn't find Construction " + msg.blockID);
			return;
		}
		if (!CanAffordToPlace(construction))
		{
			ownerPlayer.ChatMessage("Can't afford to place!");
			return;
		}
		if (!ownerPlayer.CanBuild() && !construction.canBypassBuildingPermission)
		{
			ownerPlayer.ChatMessage("Building is blocked!");
			return;
		}
		Deployable deployable = GetDeployable();
		if (construction.deployable != deployable)
		{
			ownerPlayer.ChatMessage("Deployable mismatch!");
			AntiHack.NoteAdminHack(ownerPlayer);
			return;
		}
		Construction.Target target = default(Construction.Target);
		BaseEntity baseEntity = null;
		if (msg.entity != 0)
		{
			baseEntity = BaseNetworkable.serverEntities.Find(msg.entity) as BaseEntity;
			if (!Object.op_Implicit((Object)(object)baseEntity))
			{
				ownerPlayer.ChatMessage("Couldn't find entity " + msg.entity);
				return;
			}
			msg.position = ((Component)baseEntity).get_transform().TransformPoint(msg.position);
			msg.normal = ((Component)baseEntity).get_transform().TransformDirection(msg.normal);
			msg.rotation = ((Component)baseEntity).get_transform().get_rotation() * msg.rotation;
			if (msg.socket == 0)
			{
				if ((bool)deployable && deployable.setSocketParent && baseEntity.Distance(msg.position) > 1f)
				{
					ownerPlayer.ChatMessage("Parent too far away: " + baseEntity.Distance(msg.position));
					return;
				}
				if (baseEntity is Door)
				{
					ownerPlayer.ChatMessage("Can't deploy on door");
					return;
				}
			}
			target.entity = baseEntity;
			if (msg.socket != 0)
			{
				string text = StringPool.Get(msg.socket);
				if (text != "" && (Object)(object)target.entity != (Object)null)
				{
					target.socket = FindSocket(text, target.entity.prefabID);
				}
				else
				{
					ownerPlayer.ChatMessage("Invalid Socket!");
				}
			}
		}
		target.ray = msg.ray;
		target.onTerrain = msg.onterrain;
		target.position = msg.position;
		target.normal = msg.normal;
		target.rotation = msg.rotation;
		target.player = ownerPlayer;
		target.valid = true;
		DoBuild(target, construction);
	}

	public void DoBuild(Construction.Target target, Construction component)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if (!Object.op_Implicit((Object)(object)ownerPlayer) || target.ray.IsNaNOrInfinity() || Vector3Ex.IsNaNOrInfinity(target.position) || Vector3Ex.IsNaNOrInfinity(target.normal))
		{
			return;
		}
		if (target.socket != null)
		{
			if (!target.socket.female)
			{
				ownerPlayer.ChatMessage("Target socket is not female. (" + target.socket.socketName + ")");
				return;
			}
			if ((Object)(object)target.entity != (Object)null && target.entity.IsOccupied(target.socket))
			{
				ownerPlayer.ChatMessage("Target socket is occupied. (" + target.socket.socketName + ")");
				return;
			}
			if (target.onTerrain)
			{
				ownerPlayer.ChatMessage("Target on terrain is not allowed when attaching to socket. (" + target.socket.socketName + ")");
				return;
			}
		}
		if (ConVar.AntiHack.eye_protection >= 2)
		{
			Vector3 center = ownerPlayer.eyes.center;
			Vector3 position = ownerPlayer.eyes.position;
			Vector3 origin = ((Ray)(ref target.ray)).get_origin();
			Vector3 val = (((Object)(object)target.entity != (Object)null && target.socket != null) ? target.GetWorldPosition() : target.position);
			int num = 2097152;
			int num2 = (ConVar.AntiHack.build_terraincheck ? 10551296 : 2162688);
			float num3 = ((target.socket != null) ? 0f : ConVar.AntiHack.build_losradius);
			float padding = ((target.socket != null) ? 0.5f : (ConVar.AntiHack.build_losradius + 0.01f));
			int layerMask = ((target.socket != null) ? num : num2);
			if (num3 > 0f)
			{
				val += ((Vector3)(ref target.normal)).get_normalized() * num3;
			}
			if ((Object)(object)target.entity != (Object)null)
			{
				DeployShell deployShell = PrefabAttribute.server.Find<DeployShell>(target.entity.prefabID);
				if (deployShell != null)
				{
					val += ((Vector3)(ref target.normal)).get_normalized() * deployShell.LineOfSightPadding();
				}
			}
			if (!GamePhysics.LineOfSightRadius(center, position, origin, val, layerMask, num3, padding))
			{
				ownerPlayer.ChatMessage("Line of sight blocked.");
				return;
			}
		}
		Construction.lastPlacementError = "No Error";
		GameObject val2 = DoPlacement(target, component);
		if ((Object)(object)val2 == (Object)null)
		{
			ownerPlayer.ChatMessage("Can't place: " + Construction.lastPlacementError);
		}
		if (!((Object)(object)val2 != (Object)null))
		{
			return;
		}
		Deployable deployable = GetDeployable();
		if (deployable != null)
		{
			BaseEntity baseEntity = val2.ToBaseEntity();
			if (deployable.setSocketParent && (Object)(object)target.entity != (Object)null && target.entity.SupportsChildDeployables() && Object.op_Implicit((Object)(object)baseEntity))
			{
				baseEntity.SetParent(target.entity, worldPositionStays: true);
			}
			if (deployable.wantsInstanceData && GetOwnerItem().instanceData != null)
			{
				(baseEntity as IInstanceDataReceiver).ReceiveInstanceData(GetOwnerItem().instanceData);
			}
			if (deployable.copyInventoryFromItem)
			{
				StorageContainer component2 = ((Component)baseEntity).GetComponent<StorageContainer>();
				if (Object.op_Implicit((Object)(object)component2))
				{
					component2.ReceiveInventoryFromItem(GetOwnerItem());
				}
			}
			ItemModDeployable modDeployable = GetModDeployable();
			if ((Object)(object)modDeployable != (Object)null)
			{
				modDeployable.OnDeployed(baseEntity, ownerPlayer);
			}
			baseEntity.OnDeployed(baseEntity.GetParentEntity(), ownerPlayer, GetOwnerItem());
			if (deployable.placeEffect.isValid)
			{
				if (Object.op_Implicit((Object)(object)target.entity) && target.socket != null)
				{
					Effect.server.Run(deployable.placeEffect.resourcePath, ((Component)target.entity).get_transform().TransformPoint(target.socket.worldPosition), ((Component)target.entity).get_transform().get_up());
				}
				else
				{
					Effect.server.Run(deployable.placeEffect.resourcePath, target.position, target.normal);
				}
			}
		}
		PayForPlacement(ownerPlayer, component);
	}

	public GameObject DoPlacement(Construction.Target placement, Construction component)
	{
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if (!Object.op_Implicit((Object)(object)ownerPlayer))
		{
			return null;
		}
		BaseEntity baseEntity = component.CreateConstruction(placement, bNeedsValidPlacement: true);
		if (!Object.op_Implicit((Object)(object)baseEntity))
		{
			return null;
		}
		float num = 1f;
		float num2 = 0f;
		Item ownerItem = GetOwnerItem();
		if (ownerItem != null)
		{
			baseEntity.skinID = ownerItem.skin;
			if (ownerItem.hasCondition)
			{
				num = ownerItem.conditionNormalized;
			}
		}
		((Component)baseEntity).get_gameObject().AwakeFromInstantiate();
		BuildingBlock buildingBlock = baseEntity as BuildingBlock;
		if (Object.op_Implicit((Object)(object)buildingBlock))
		{
			buildingBlock.blockDefinition = PrefabAttribute.server.Find<Construction>(buildingBlock.prefabID);
			if (!buildingBlock.blockDefinition)
			{
				Debug.LogError((object)"Placing a building block that has no block definition!");
				return null;
			}
			buildingBlock.SetGrade(buildingBlock.blockDefinition.defaultGrade.gradeBase.type);
			num2 = buildingBlock.currentGrade.maxHealth;
		}
		BaseCombatEntity baseCombatEntity = baseEntity as BaseCombatEntity;
		if (Object.op_Implicit((Object)(object)baseCombatEntity))
		{
			num2 = (((Object)(object)buildingBlock != (Object)null) ? buildingBlock.currentGrade.maxHealth : baseCombatEntity.startHealth);
			baseCombatEntity.ResetLifeStateOnSpawn = false;
			baseCombatEntity.InitializeHealth(num2 * num, num2);
		}
		((Component)baseEntity).get_gameObject().SendMessage("SetDeployedBy", (object)ownerPlayer, (SendMessageOptions)1);
		baseEntity.OwnerID = ownerPlayer.userID;
		baseEntity.Spawn();
		if (Object.op_Implicit((Object)(object)buildingBlock))
		{
			Effect.server.Run("assets/bundled/prefabs/fx/build/frame_place.prefab", baseEntity, 0u, Vector3.get_zero(), Vector3.get_zero());
		}
		StabilityEntity stabilityEntity = baseEntity as StabilityEntity;
		if (Object.op_Implicit((Object)(object)stabilityEntity))
		{
			stabilityEntity.UpdateSurroundingEntities();
		}
		return ((Component)baseEntity).get_gameObject();
	}

	public void PayForPlacement(BasePlayer player, Construction component)
	{
		if (isTypeDeployable)
		{
			GetItem().UseItem();
			return;
		}
		List<Item> list = new List<Item>();
		foreach (ItemAmount item in component.defaultGrade.costToBuild)
		{
			player.inventory.Take(list, item.itemDef.itemid, (int)item.amount);
			player.Command("note.inv", item.itemDef.itemid, item.amount * -1f);
		}
		foreach (Item item2 in list)
		{
			item2.Remove();
		}
	}

	public bool CanAffordToPlace(Construction component)
	{
		if (isTypeDeployable)
		{
			return true;
		}
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if (!Object.op_Implicit((Object)(object)ownerPlayer))
		{
			return false;
		}
		foreach (ItemAmount item in component.defaultGrade.costToBuild)
		{
			if ((float)ownerPlayer.inventory.GetAmount(item.itemDef.itemid) < item.amount)
			{
				return false;
			}
		}
		return true;
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
}
