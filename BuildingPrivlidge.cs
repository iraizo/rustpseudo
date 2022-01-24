using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class BuildingPrivlidge : StorageContainer
{
	public class UpkeepBracket
	{
		public int objectsUpTo;

		public float fraction;

		public float blocksTaxPaid;

		public UpkeepBracket(int numObjs, float frac)
		{
			objectsUpTo = numObjs;
			fraction = frac;
			blocksTaxPaid = 0f;
		}
	}

	private float cachedProtectedMinutes;

	private float nextProtectedCalcTime;

	private static UpkeepBracket[] upkeepBrackets = new UpkeepBracket[4]
	{
		new UpkeepBracket(ConVar.Decay.bracket_0_blockcount, ConVar.Decay.bracket_0_costfraction),
		new UpkeepBracket(ConVar.Decay.bracket_1_blockcount, ConVar.Decay.bracket_1_costfraction),
		new UpkeepBracket(ConVar.Decay.bracket_2_blockcount, ConVar.Decay.bracket_2_costfraction),
		new UpkeepBracket(ConVar.Decay.bracket_3_blockcount, ConVar.Decay.bracket_3_costfraction)
	};

	private List<ItemAmount> upkeepBuffer = new List<ItemAmount>();

	public List<PlayerNameID> authorizedPlayers = new List<PlayerNameID>();

	public const Flags Flag_MaxAuths = Flags.Reserved5;

	public List<ItemDefinition> allowedConstructionItems = new List<ItemDefinition>();

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("BuildingPrivlidge.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 1092560690 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - AddSelfAuthorize "));
				}
				TimeWarning val2 = TimeWarning.New("AddSelfAuthorize", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1092560690u, "AddSelfAuthorize", this, player, 3f))
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
							RPCMessage rpc2 = rPCMessage;
							AddSelfAuthorize(rpc2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in AddSelfAuthorize");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 253307592 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ClearList "));
				}
				TimeWarning val2 = TimeWarning.New("ClearList", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(253307592u, "ClearList", this, player, 3f))
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
							RPCMessage rpc3 = rPCMessage;
							ClearList(rpc3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in ClearList");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3617985969u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RemoveSelfAuthorize "));
				}
				TimeWarning val2 = TimeWarning.New("RemoveSelfAuthorize", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3617985969u, "RemoveSelfAuthorize", this, player, 3f))
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
							RPCMessage rpc4 = rPCMessage;
							RemoveSelfAuthorize(rpc4);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in RemoveSelfAuthorize");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2051750736 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_Rotate "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_Rotate", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(2051750736u, "RPC_Rotate", this, player, 3f))
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
							RPC_Rotate(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in RPC_Rotate");
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

	public float CalculateUpkeepPeriodMinutes()
	{
		if (base.isServer)
		{
			return ConVar.Decay.upkeep_period_minutes;
		}
		return 0f;
	}

	public float CalculateUpkeepCostFraction()
	{
		if (base.isServer)
		{
			return CalculateBuildingTaxRate();
		}
		return 0f;
	}

	public void CalculateUpkeepCostAmounts(List<ItemAmount> itemAmounts)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		BuildingManager.Building building = GetBuilding();
		if (building == null || !building.HasDecayEntities())
		{
			return;
		}
		float multiplier = CalculateUpkeepCostFraction();
		Enumerator<DecayEntity> enumerator = building.decayEntities.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				enumerator.get_Current().CalculateUpkeepCostAmounts(itemAmounts, multiplier);
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
	}

	public float GetProtectedMinutes(bool force = false)
	{
		if (base.isServer)
		{
			if (!force && Time.get_realtimeSinceStartup() < nextProtectedCalcTime)
			{
				return cachedProtectedMinutes;
			}
			nextProtectedCalcTime = Time.get_realtimeSinceStartup() + 60f;
			List<ItemAmount> list = Pool.GetList<ItemAmount>();
			CalculateUpkeepCostAmounts(list);
			float num = CalculateUpkeepPeriodMinutes();
			float num2 = -1f;
			if (base.inventory != null)
			{
				foreach (ItemAmount item in list)
				{
					int num3 = Enumerable.Sum<Item>((IEnumerable<Item>)base.inventory.FindItemsByItemID(item.itemid), (Func<Item, int>)((Item x) => x.amount));
					if (num3 > 0 && item.amount > 0f)
					{
						float num4 = (float)num3 / item.amount * num;
						if (num2 == -1f || num4 < num2)
						{
							num2 = num4;
						}
					}
					else
					{
						num2 = 0f;
					}
				}
				if (num2 == -1f)
				{
					num2 = 0f;
				}
			}
			Pool.FreeList<ItemAmount>(ref list);
			cachedProtectedMinutes = num2;
			return cachedProtectedMinutes;
		}
		return 0f;
	}

	public override void OnKilled(HitInfo info)
	{
		if (ConVar.Decay.upkeep_grief_protection > 0f)
		{
			PurchaseUpkeepTime(ConVar.Decay.upkeep_grief_protection * 60f);
		}
		base.OnKilled(info);
	}

	public override void DecayTick()
	{
		if (EnsurePrimary())
		{
			base.DecayTick();
		}
	}

	private bool EnsurePrimary()
	{
		BuildingManager.Building building = GetBuilding();
		if (building != null)
		{
			BuildingPrivlidge dominatingBuildingPrivilege = building.GetDominatingBuildingPrivilege();
			if ((Object)(object)dominatingBuildingPrivilege != (Object)null && (Object)(object)dominatingBuildingPrivilege != (Object)(object)this)
			{
				Kill(DestroyMode.Gib);
				return false;
			}
		}
		return true;
	}

	public void MarkProtectedMinutesDirty(float delay = 0f)
	{
		nextProtectedCalcTime = Time.get_realtimeSinceStartup() + delay;
	}

	private float CalculateBuildingTaxRate()
	{
		BuildingManager.Building building = GetBuilding();
		if (building == null)
		{
			return ConVar.Decay.bracket_0_costfraction;
		}
		if (!building.HasBuildingBlocks())
		{
			return ConVar.Decay.bracket_0_costfraction;
		}
		int count = building.buildingBlocks.get_Count();
		int num = count;
		for (int i = 0; i < upkeepBrackets.Length; i++)
		{
			UpkeepBracket upkeepBracket = upkeepBrackets[i];
			upkeepBracket.blocksTaxPaid = 0f;
			if (num > 0)
			{
				int num2 = 0;
				num2 = ((i != upkeepBrackets.Length - 1) ? Mathf.Min(num, upkeepBrackets[i].objectsUpTo) : num);
				num -= num2;
				upkeepBracket.blocksTaxPaid = (float)num2 * upkeepBracket.fraction;
			}
		}
		float num3 = 0f;
		for (int j = 0; j < upkeepBrackets.Length; j++)
		{
			UpkeepBracket upkeepBracket2 = upkeepBrackets[j];
			if (!(upkeepBracket2.blocksTaxPaid > 0f))
			{
				break;
			}
			num3 += upkeepBracket2.blocksTaxPaid;
		}
		return num3 / (float)count;
	}

	private void ApplyUpkeepPayment()
	{
		List<Item> list = Pool.GetList<Item>();
		for (int i = 0; i < upkeepBuffer.Count; i++)
		{
			ItemAmount itemAmount = upkeepBuffer[i];
			int num = (int)itemAmount.amount;
			if (num < 1)
			{
				continue;
			}
			base.inventory.Take(list, itemAmount.itemid, num);
			foreach (Item item in list)
			{
				if (IsDebugging())
				{
					Debug.Log((object)(((object)this).ToString() + ": Using " + item.amount + " of " + item.info.shortname));
				}
				item.UseItem(item.amount);
			}
			list.Clear();
			itemAmount.amount -= num;
			upkeepBuffer[i] = itemAmount;
		}
		Pool.FreeList<Item>(ref list);
	}

	private void QueueUpkeepPayment(List<ItemAmount> itemAmounts)
	{
		for (int i = 0; i < itemAmounts.Count; i++)
		{
			ItemAmount itemAmount = itemAmounts[i];
			bool flag = false;
			foreach (ItemAmount item in upkeepBuffer)
			{
				if ((Object)(object)item.itemDef == (Object)(object)itemAmount.itemDef)
				{
					item.amount += itemAmount.amount;
					if (IsDebugging())
					{
						Debug.Log((object)(((object)this).ToString() + ": Adding " + itemAmount.amount + " of " + itemAmount.itemDef.shortname + " to " + item.amount));
					}
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				if (IsDebugging())
				{
					Debug.Log((object)(((object)this).ToString() + ": Adding " + itemAmount.amount + " of " + itemAmount.itemDef.shortname));
				}
				upkeepBuffer.Add(new ItemAmount(itemAmount.itemDef, itemAmount.amount));
			}
		}
	}

	private bool CanAffordUpkeepPayment(List<ItemAmount> itemAmounts)
	{
		for (int i = 0; i < itemAmounts.Count; i++)
		{
			ItemAmount itemAmount = itemAmounts[i];
			if ((float)base.inventory.GetAmount(itemAmount.itemid, onlyUsableAmounts: true) < itemAmount.amount)
			{
				if (IsDebugging())
				{
					Debug.Log((object)(((object)this).ToString() + ": Can't afford " + itemAmount.amount + " of " + itemAmount.itemDef.shortname));
				}
				return false;
			}
		}
		return true;
	}

	public float PurchaseUpkeepTime(DecayEntity entity, float deltaTime)
	{
		float num = CalculateUpkeepCostFraction();
		float num2 = CalculateUpkeepPeriodMinutes() * 60f;
		float multiplier = num * deltaTime / num2;
		List<ItemAmount> list = Pool.GetList<ItemAmount>();
		entity.CalculateUpkeepCostAmounts(list, multiplier);
		bool num3 = CanAffordUpkeepPayment(list);
		QueueUpkeepPayment(list);
		Pool.FreeList<ItemAmount>(ref list);
		ApplyUpkeepPayment();
		if (!num3)
		{
			return 0f;
		}
		return deltaTime;
	}

	public void PurchaseUpkeepTime(float deltaTime)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		BuildingManager.Building building = GetBuilding();
		if (building == null || !building.HasDecayEntities())
		{
			return;
		}
		float num = Mathf.Min(GetProtectedMinutes(force: true) * 60f, deltaTime);
		if (!(num > 0f))
		{
			return;
		}
		Enumerator<DecayEntity> enumerator = building.decayEntities.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				DecayEntity current = enumerator.get_Current();
				float protectedSeconds = current.GetProtectedSeconds();
				if (num > protectedSeconds)
				{
					float num2 = PurchaseUpkeepTime(current, num - protectedSeconds);
					current.AddUpkeepTime(num2);
					if (IsDebugging())
					{
						Debug.Log((object)(((object)this).ToString() + " purchased upkeep time for " + ((object)current).ToString() + ": " + protectedSeconds + " + " + num2 + " = " + current.GetProtectedSeconds()));
					}
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
	}

	public override void ResetState()
	{
		base.ResetState();
		authorizedPlayers.Clear();
	}

	public bool IsAuthed(BasePlayer player)
	{
		return Enumerable.Any<PlayerNameID>((IEnumerable<PlayerNameID>)authorizedPlayers, (Func<PlayerNameID, bool>)((PlayerNameID x) => x.userid == player.userID));
	}

	public bool IsAuthed(ulong userID)
	{
		return Enumerable.Any<PlayerNameID>((IEnumerable<PlayerNameID>)authorizedPlayers, (Func<PlayerNameID, bool>)((PlayerNameID x) => x.userid == userID));
	}

	public bool AnyAuthed()
	{
		return authorizedPlayers.Count > 0;
	}

	public override bool ItemFilter(Item item, int targetSlot)
	{
		if (targetSlot >= 24 && targetSlot <= 27)
		{
			return allowedConstructionItems.Contains(item.info);
		}
		return base.ItemFilter(item, targetSlot);
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.buildingPrivilege = Pool.Get<BuildingPrivilege>();
		info.msg.buildingPrivilege.users = authorizedPlayers;
		if (!info.forDisk)
		{
			info.msg.buildingPrivilege.upkeepPeriodMinutes = CalculateUpkeepPeriodMinutes();
			info.msg.buildingPrivilege.costFraction = CalculateUpkeepCostFraction();
			info.msg.buildingPrivilege.protectedMinutes = GetProtectedMinutes();
		}
	}

	public override void PostSave(SaveInfo info)
	{
		info.msg.buildingPrivilege.users = null;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		authorizedPlayers.Clear();
		if (info.msg.buildingPrivilege != null && info.msg.buildingPrivilege.users != null)
		{
			authorizedPlayers = info.msg.buildingPrivilege.users;
			if (!info.fromDisk)
			{
				cachedProtectedMinutes = info.msg.buildingPrivilege.protectedMinutes;
			}
			info.msg.buildingPrivilege.users = null;
		}
	}

	public void BuildingDirty()
	{
		if (base.isServer)
		{
			AddDelayedUpdate();
		}
	}

	public bool AtMaxAuthCapacity()
	{
		return HasFlag(Flags.Reserved5);
	}

	public void UpdateMaxAuthCapacity()
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(serverside: true);
		if (Object.op_Implicit((Object)(object)activeGameMode) && activeGameMode.limitTeamAuths)
		{
			SetFlag(Flags.Reserved5, authorizedPlayers.Count >= activeGameMode.GetMaxRelationshipTeamSize());
		}
	}

	protected override void OnInventoryDirty()
	{
		base.OnInventoryDirty();
		AddDelayedUpdate();
	}

	public override void OnItemAddedOrRemoved(Item item, bool bAdded)
	{
		base.OnItemAddedOrRemoved(item, bAdded);
		AddDelayedUpdate();
	}

	public void AddDelayedUpdate()
	{
		if (((FacepunchBehaviour)this).IsInvoking((Action)DelayedUpdate))
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)DelayedUpdate);
		}
		((FacepunchBehaviour)this).Invoke((Action)DelayedUpdate, 1f);
	}

	public void DelayedUpdate()
	{
		MarkProtectedMinutesDirty();
		SendNetworkUpdate();
	}

	public bool CanAdministrate(BasePlayer player)
	{
		BaseLock baseLock = GetSlot(Slot.Lock) as BaseLock;
		if ((Object)(object)baseLock == (Object)null)
		{
			return true;
		}
		return baseLock.OnTryToOpen(player);
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	private void AddSelfAuthorize(RPCMessage rpc)
	{
		if (rpc.player.CanInteract() && CanAdministrate(rpc.player))
		{
			AddPlayer(rpc.player);
			SendNetworkUpdate();
		}
	}

	public void AddPlayer(BasePlayer player)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		if (!AtMaxAuthCapacity())
		{
			authorizedPlayers.RemoveAll((PlayerNameID x) => x.userid == player.userID);
			PlayerNameID val = new PlayerNameID();
			val.userid = player.userID;
			val.username = player.displayName;
			authorizedPlayers.Add(val);
			UpdateMaxAuthCapacity();
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	private void RemoveSelfAuthorize(RPCMessage rpc)
	{
		if (rpc.player.CanInteract() && CanAdministrate(rpc.player))
		{
			authorizedPlayers.RemoveAll((PlayerNameID x) => x.userid == rpc.player.userID);
			UpdateMaxAuthCapacity();
			SendNetworkUpdate();
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	private void ClearList(RPCMessage rpc)
	{
		if (rpc.player.CanInteract() && CanAdministrate(rpc.player))
		{
			authorizedPlayers.Clear();
			UpdateMaxAuthCapacity();
			SendNetworkUpdate();
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_Rotate(RPCMessage msg)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer player = msg.player;
		if (player.CanBuild() && Object.op_Implicit((Object)(object)player.GetHeldEntity()) && (Object)(object)((Component)player.GetHeldEntity()).GetComponent<Hammer>() != (Object)null && ((Object)(object)GetSlot(Slot.Lock) == (Object)null || !GetSlot(Slot.Lock).IsLocked()))
		{
			((Component)this).get_transform().set_rotation(Quaternion.LookRotation(-((Component)this).get_transform().get_forward(), ((Component)this).get_transform().get_up()));
			SendNetworkUpdate();
			Deployable component = ((Component)this).GetComponent<Deployable>();
			if (component != null && component.placeEffect.isValid)
			{
				Effect.server.Run(component.placeEffect.resourcePath, ((Component)this).get_transform().get_position(), Vector3.get_up());
			}
		}
		BaseEntity slot = GetSlot(Slot.Lock);
		if ((Object)(object)slot != (Object)null)
		{
			slot.SendNetworkUpdate();
		}
	}

	public override bool HasSlot(Slot slot)
	{
		if (slot == Slot.Lock)
		{
			return true;
		}
		return base.HasSlot(slot);
	}
}
