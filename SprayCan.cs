using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class SprayCan : HeldEntity
{
	private enum SprayFailReason
	{
		None,
		MountedBlocked,
		IOConnection,
		LineOfSight,
		SkinNotOwned,
		InvalidItem
	}

	private struct ContainerSet
	{
		public int ContainerIndex;

		public uint PrefabId;
	}

	private struct ChildPreserveInfo
	{
		public BaseEntity TargetEntity;

		public uint TargetBone;

		public Vector3 LocalPosition;

		public Quaternion LocalRotation;
	}

	public const float MaxFreeSprayDistanceFromStart = 10f;

	public const float MaxFreeSprayStartingDistance = 3f;

	private SprayCanSpray_Freehand paintingLine;

	public SoundDefinition SpraySound;

	public GameObjectRef SkinSelectPanel;

	public float SprayCooldown = 2f;

	public float ConditionLossPerSpray = 10f;

	public float ConditionLossPerReskin = 10f;

	public GameObjectRef LinePrefab;

	public Color[] SprayColours = (Color[])(object)new Color[0];

	public float[] SprayWidths = new float[3] { 0.1f, 0.2f, 0.3f };

	public ParticleSystem FreehandWorldSpray;

	public ParticleSystem OneShotWorldSpray;

	public GameObjectRef ReskinEffect;

	private Rigidbody resetRigidbody;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("SprayCan.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 3490735573u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - BeginFreehandSpray "));
				}
				TimeWarning val2 = TimeWarning.New("BeginFreehandSpray", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(3490735573u, "BeginFreehandSpray", this, player))
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
							rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							RPCMessage msg2 = rPCMessage;
							BeginFreehandSpray(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in BeginFreehandSpray");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 151738090 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ChangeItemSkin "));
				}
				TimeWarning val2 = TimeWarning.New("ChangeItemSkin", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(151738090u, "ChangeItemSkin", this, player))
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
							rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							RPCMessage msg3 = rPCMessage;
							ChangeItemSkin(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in ChangeItemSkin");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 396000799 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - CreateSpray "));
				}
				TimeWarning val2 = TimeWarning.New("CreateSpray", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(396000799u, "CreateSpray", this, player))
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
							rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							RPCMessage msg4 = rPCMessage;
							CreateSpray(msg4);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in CreateSpray");
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
	private void BeginFreehandSpray(RPCMessage msg)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		if (!IsBusy())
		{
			Vector3 val = msg.read.Vector3();
			Vector3 atNormal = msg.read.Vector3();
			int num = msg.read.Int32();
			int num2 = msg.read.Int32();
			if (num >= 0 && num < SprayColours.Length && num2 >= 0 && num2 < SprayWidths.Length && !(Vector3.Distance(val, ((Component)GetOwnerPlayer()).get_transform().get_position()) > 3f))
			{
				SprayCanSpray_Freehand sprayCanSpray_Freehand = GameManager.server.CreateEntity(LinePrefab.resourcePath, val, Quaternion.get_identity()) as SprayCanSpray_Freehand;
				sprayCanSpray_Freehand.AddInitialPoint(atNormal);
				sprayCanSpray_Freehand.SetColour(SprayColours[num]);
				sprayCanSpray_Freehand.SetWidth(SprayWidths[num2]);
				sprayCanSpray_Freehand.EnableChanges(msg.player);
				sprayCanSpray_Freehand.Spawn();
				paintingLine = sprayCanSpray_Freehand;
				SetFlag(Flags.Busy, b: true);
				ClientRPC(null, "Client_OnFreeSprayBegins", num);
			}
		}
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	private void ChangeItemSkin(RPCMessage msg)
	{
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_053f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0557: Unknown result type (might be due to invalid IL or missing references)
		if (IsBusy())
		{
			return;
		}
		uint uid = msg.read.UInt32();
		BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(uid);
		int targetSkin = msg.read.Int32();
		if ((Object)(object)msg.player == (Object)null || !msg.player.CanBuild())
		{
			return;
		}
		bool flag = false;
		if (targetSkin != 0 && !flag && !msg.player.blueprints.CheckSkinOwnership(targetSkin, msg.player.userID))
		{
			SprayFailResponse(SprayFailReason.SkinNotOwned);
			return;
		}
		BaseEntity baseEntity2;
		if ((Object)(object)baseNetworkable != (Object)null && (baseEntity2 = baseNetworkable as BaseEntity) != null)
		{
			OBB val = baseEntity2.WorldSpaceBounds();
			Vector3 position = ((OBB)(ref val)).ClosestPoint(msg.player.eyes.position);
			if (!msg.player.IsVisible(position, 3f))
			{
				SprayFailResponse(SprayFailReason.LineOfSight);
				return;
			}
			if (!GetItemDefinitionForEntity(baseEntity2, out var def))
			{
				SprayFailResponse(SprayFailReason.InvalidItem);
				return;
			}
			ItemDefinition itemDefinition = null;
			ulong num = ItemDefinition.FindSkin(def.itemid, targetSkin);
			ItemSkinDirectory.Skin skin = def.skins.FirstOrDefault((ItemSkinDirectory.Skin x) => x.id == targetSkin);
			ItemSkin itemSkin;
			if ((Object)(object)skin.invItem != (Object)null && (itemSkin = skin.invItem as ItemSkin) != null)
			{
				if ((Object)(object)itemSkin.Redirect != (Object)null)
				{
					itemDefinition = itemSkin.Redirect;
				}
				else if (GetItemDefinitionForEntity(baseEntity2, out def, useRedirect: false) && (Object)(object)def.isRedirectOf != (Object)null)
				{
					itemDefinition = def.isRedirectOf;
				}
			}
			else if ((Object)(object)def.isRedirectOf != (Object)null || (GetItemDefinitionForEntity(baseEntity2, out def, useRedirect: false) && (Object)(object)def.isRedirectOf != (Object)null))
			{
				itemDefinition = def.isRedirectOf;
			}
			if ((Object)(object)itemDefinition == (Object)null)
			{
				baseEntity2.skinID = num;
				baseEntity2.SendNetworkUpdate();
			}
			else
			{
				if (!CanEntityBeRespawned(baseEntity2, out var reason2))
				{
					SprayFailResponse(reason2);
					return;
				}
				if (!GetEntityPrefabPath(itemDefinition, out var resourcePath))
				{
					Debug.LogWarning((object)("Cannot find resource path of redirect entity to spawn! " + ((Object)((Component)itemDefinition).get_gameObject()).get_name()));
					SprayFailResponse(SprayFailReason.InvalidItem);
					return;
				}
				Vector3 position2 = ((Component)baseEntity2).get_transform().get_position();
				Quaternion rotation = ((Component)baseEntity2).get_transform().get_rotation();
				BaseEntity entity = baseEntity2.GetParentEntity();
				float health = baseEntity2.Health();
				Rigidbody component = ((Component)baseEntity2).GetComponent<Rigidbody>();
				bool flag2 = baseEntity2 is Door;
				Dictionary<ContainerSet, List<Item>> dictionary2 = new Dictionary<ContainerSet, List<Item>>();
				SaveEntityStorage(baseEntity2, dictionary2, 0);
				List<ChildPreserveInfo> list = Pool.GetList<ChildPreserveInfo>();
				if (flag2)
				{
					foreach (BaseEntity child in baseEntity2.children)
					{
						list.Add(new ChildPreserveInfo
						{
							TargetEntity = child,
							TargetBone = child.parentBone,
							LocalPosition = ((Component)child).get_transform().get_localPosition(),
							LocalRotation = ((Component)child).get_transform().get_localRotation()
						});
					}
					foreach (ChildPreserveInfo item in list)
					{
						item.TargetEntity.SetParent(null, worldPositionStays: true);
					}
				}
				else
				{
					for (int i = 0; i < baseEntity2.children.Count; i++)
					{
						SaveEntityStorage(baseEntity2.children[i], dictionary2, i + 1);
					}
				}
				baseEntity2.Kill();
				baseEntity2 = GameManager.server.CreateEntity(resourcePath, position2, rotation);
				Rigidbody val2 = default(Rigidbody);
				if ((Object)(object)component != (Object)null && ((Component)baseEntity2).TryGetComponent<Rigidbody>(ref val2) && !val2.get_isKinematic() && val2.get_useGravity())
				{
					val2.set_useGravity(false);
					resetRigidbody = val2;
					((FacepunchBehaviour)this).Invoke((Action)RestoreRigidbody, 0.1f);
				}
				baseEntity2.SetParent(entity);
				if (GetItemDefinitionForEntity(baseEntity2, out var def2, useRedirect: false) && (Object)(object)def2.isRedirectOf != (Object)null)
				{
					baseEntity2.skinID = 0uL;
				}
				else
				{
					baseEntity2.skinID = num;
				}
				baseEntity2.Spawn();
				BaseCombatEntity baseCombatEntity;
				if ((baseCombatEntity = baseEntity2 as BaseCombatEntity) != null)
				{
					baseCombatEntity.SetHealth(health);
				}
				if (dictionary2.Count > 0)
				{
					RestoreEntityStorage(baseEntity2, 0, dictionary2);
					if (!flag2)
					{
						for (int j = 0; j < baseEntity2.children.Count; j++)
						{
							RestoreEntityStorage(baseEntity2.children[j], j + 1, dictionary2);
						}
					}
					foreach (KeyValuePair<ContainerSet, List<Item>> item2 in dictionary2)
					{
						foreach (Item item3 in item2.Value)
						{
							Debug.Log((object)$"Deleting {item3} as it has no new container");
							item3.Remove();
						}
					}
				}
				if (flag2)
				{
					foreach (ChildPreserveInfo item4 in list)
					{
						item4.TargetEntity.SetParent(baseEntity2, item4.TargetBone, worldPositionStays: true);
						((Component)item4.TargetEntity).get_transform().set_localPosition(item4.LocalPosition);
						((Component)item4.TargetEntity).get_transform().set_localRotation(item4.LocalRotation);
						item4.TargetEntity.SendNetworkUpdate();
					}
				}
				Pool.FreeList<ChildPreserveInfo>(ref list);
			}
			ClientRPC(null, "Client_ReskinResult", 1, baseEntity2.net.ID);
		}
		LoseCondition(ConditionLossPerReskin);
		SetFlag(Flags.Busy, b: true);
		((FacepunchBehaviour)this).Invoke((Action)ClearBusy, SprayCooldown);
		static void RestoreEntityStorage(BaseEntity baseEntity, int index, Dictionary<ContainerSet, List<Item>> copy)
		{
			IItemContainerEntity itemContainerEntity;
			if ((itemContainerEntity = baseEntity as IItemContainerEntity) != null)
			{
				ContainerSet containerSet = default(ContainerSet);
				containerSet.ContainerIndex = index;
				containerSet.PrefabId = ((index != 0) ? baseEntity.prefabID : 0u);
				ContainerSet key = containerSet;
				if (copy.ContainsKey(key))
				{
					foreach (Item item5 in copy[key])
					{
						item5.MoveToContainer(itemContainerEntity.inventory);
					}
					copy.Remove(key);
				}
			}
		}
		static void SaveEntityStorage(BaseEntity baseEntity, Dictionary<ContainerSet, List<Item>> dictionary, int index)
		{
			IItemContainerEntity itemContainerEntity2;
			if ((itemContainerEntity2 = baseEntity as IItemContainerEntity) != null)
			{
				ContainerSet containerSet2 = default(ContainerSet);
				containerSet2.ContainerIndex = index;
				containerSet2.PrefabId = ((index != 0) ? baseEntity.prefabID : 0u);
				ContainerSet key2 = containerSet2;
				dictionary.Add(key2, new List<Item>());
				foreach (Item item6 in itemContainerEntity2.inventory.itemList)
				{
					dictionary[key2].Add(item6);
				}
				foreach (Item item7 in dictionary[key2])
				{
					item7.RemoveFromContainer();
				}
			}
		}
		void SprayFailResponse(SprayFailReason reason)
		{
			ClientRPC(null, "Client_ReskinResult", 0, (int)reason);
		}
	}

	private void RestoreRigidbody()
	{
		if ((Object)(object)resetRigidbody != (Object)null)
		{
			resetRigidbody.set_useGravity(true);
		}
		resetRigidbody = null;
	}

	private bool GetEntityPrefabPath(ItemDefinition def, out string resourcePath)
	{
		resourcePath = string.Empty;
		ItemModDeployable itemModDeployable = default(ItemModDeployable);
		if (((Component)def).TryGetComponent<ItemModDeployable>(ref itemModDeployable))
		{
			resourcePath = itemModDeployable.entityPrefab.resourcePath;
			return true;
		}
		ItemModEntity itemModEntity = default(ItemModEntity);
		if (((Component)def).TryGetComponent<ItemModEntity>(ref itemModEntity))
		{
			resourcePath = itemModEntity.entityPrefab.resourcePath;
			return true;
		}
		ItemModEntityReference itemModEntityReference = default(ItemModEntityReference);
		if (((Component)def).TryGetComponent<ItemModEntityReference>(ref itemModEntityReference))
		{
			resourcePath = itemModEntityReference.entityPrefab.resourcePath;
			return true;
		}
		return false;
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	private void CreateSpray(RPCMessage msg)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		if (IsBusy())
		{
			return;
		}
		Vector3 val = msg.read.Vector3();
		Vector3 val2 = msg.read.Vector3();
		Vector3 val3 = msg.read.Vector3();
		Plane val4 = default(Plane);
		((Plane)(ref val4))._002Ector(val2, val);
		Vector3 val5 = ((Plane)(ref val4)).ClosestPointOnPlane(val3) - val;
		Quaternion val6 = Quaternion.LookRotation(((Vector3)(ref val5)).get_normalized(), val2);
		val6 *= Quaternion.Euler(0f, 0f, 90f);
		if (GetItem().contents.itemList.Count < 1)
		{
			return;
		}
		Item item = GetItem().contents.itemList[0];
		if (item != null)
		{
			ItemModSpray component = ((Component)item.info).GetComponent<ItemModSpray>();
			if ((Object)(object)component == (Object)null)
			{
				Debug.LogWarning((object)"Missing ItemModSpray on spray");
				return;
			}
			BaseEntity baseEntity = GameManager.server.CreateEntity(component.SprayPrefab.resourcePath, val, val6);
			baseEntity.skinID = item.skin;
			baseEntity.Spawn();
		}
		LoseCondition(ConditionLossPerSpray);
		SetFlag(Flags.Busy, b: true);
		((FacepunchBehaviour)this).Invoke((Action)ClearBusy, SprayCooldown);
	}

	private void LoseCondition(float amount)
	{
		GetOwnerItem()?.LoseCondition(amount);
	}

	public void ClearBusy()
	{
		SetFlag(Flags.Busy, b: false);
	}

	public override void OnHeldChanged()
	{
		if (IsDisabled())
		{
			ClearBusy();
			if ((Object)(object)paintingLine != (Object)null)
			{
				paintingLine.Kill();
			}
			paintingLine = null;
		}
	}

	public void ClearPaintingLine()
	{
		paintingLine = null;
	}

	private bool CanEntityBeRespawned(BaseEntity targetEntity, out SprayFailReason reason)
	{
		BaseMountable baseMountable;
		if ((baseMountable = targetEntity as BaseMountable) != null && baseMountable.IsMounted())
		{
			reason = SprayFailReason.MountedBlocked;
			return false;
		}
		BaseVehicle baseVehicle;
		if (targetEntity.isServer && (baseVehicle = targetEntity as BaseVehicle) != null && (baseVehicle.HasDriver() || baseVehicle.AnyMounted()))
		{
			reason = SprayFailReason.MountedBlocked;
			return false;
		}
		IOEntity iOEntity;
		if ((iOEntity = targetEntity as IOEntity) != null && (iOEntity.GetConnectedInputCount() != 0 || iOEntity.GetConnectedOutputCount() != 0))
		{
			reason = SprayFailReason.IOConnection;
			return false;
		}
		reason = SprayFailReason.None;
		return true;
	}

	public static bool GetItemDefinitionForEntity(BaseEntity be, out ItemDefinition def, bool useRedirect = true)
	{
		def = null;
		BaseCombatEntity baseCombatEntity;
		if ((baseCombatEntity = be as BaseCombatEntity) != null)
		{
			if (baseCombatEntity.pickup.enabled && (Object)(object)baseCombatEntity.pickup.itemTarget != (Object)null)
			{
				def = baseCombatEntity.pickup.itemTarget;
			}
			else if (baseCombatEntity.repair.enabled && (Object)(object)baseCombatEntity.repair.itemTarget != (Object)null)
			{
				def = baseCombatEntity.repair.itemTarget;
			}
		}
		if (useRedirect && (Object)(object)def != (Object)null && (Object)(object)def.isRedirectOf != (Object)null)
		{
			def = def.isRedirectOf;
		}
		return (Object)(object)def != (Object)null;
	}
}
