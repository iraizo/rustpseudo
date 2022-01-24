using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class LiquidContainer : ContainerIOEntity
{
	public ItemDefinition defaultLiquid;

	public int startingAmount;

	public bool autofillOutputs;

	public float autofillTickRate = 2f;

	public int autofillTickAmount = 2;

	public int maxOutputFlow = 6;

	public ItemDefinition[] ValidItems;

	private int currentDrainAmount;

	private HashSet<IOEntity> connectedList = new HashSet<IOEntity>();

	private HashSet<ContainerIOEntity> pushTargets = new HashSet<ContainerIOEntity>();

	private const int maxPushTargets = 3;

	private IOEntity considerConnectedTo;

	private Action updateDrainAmountAction;

	private Action updatePushLiquidTargetsAction;

	private Action pushLiquidAction;

	private Action deductFuelAction;

	private float lastOutputDrainUpdate;

	public override bool IsGravitySource => true;

	protected override bool DisregardGravityRestrictionsOnLiquid
	{
		get
		{
			if (!HasFlag(Flags.Reserved8))
			{
				return base.DisregardGravityRestrictionsOnLiquid;
			}
			return true;
		}
	}

	public override bool BlockFluidDraining => true;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("LiquidContainer.OnRpcMessage", 0);
		try
		{
			if (rpc == 2002733690 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SVDrink "));
				}
				TimeWarning val2 = TimeWarning.New("SVDrink", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(2002733690u, "SVDrink", this, player, 3f))
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
							RPCMessage rpc2 = rPCMessage;
							SVDrink(rpc2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SVDrink");
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

	public override bool IsRootEntity()
	{
		return true;
	}

	private bool CanAcceptItem(Item item, int count)
	{
		if (ValidItems == null || ValidItems.Length == 0)
		{
			return true;
		}
		ItemDefinition[] validItems = ValidItems;
		for (int i = 0; i < validItems.Length; i++)
		{
			if ((Object)(object)validItems[i] == (Object)(object)item.info)
			{
				return true;
			}
		}
		return false;
	}

	public override void ServerInit()
	{
		updateDrainAmountAction = UpdateDrainAmount;
		pushLiquidAction = PushLiquidThroughOutputs;
		deductFuelAction = DeductFuel;
		updatePushLiquidTargetsAction = UpdatePushLiquidTargets;
		base.ServerInit();
		if (startingAmount > 0)
		{
			base.inventory.AddItem(defaultLiquid, startingAmount, 0uL);
		}
		if (autofillOutputs && HasLiquidItem())
		{
			UpdatePushLiquidTargets();
		}
		ItemContainer itemContainer = base.inventory;
		itemContainer.canAcceptItem = (Func<Item, int, bool>)Delegate.Combine(itemContainer.canAcceptItem, new Func<Item, int, bool>(CanAcceptItem));
	}

	public override void OnCircuitChanged(bool forceUpdate)
	{
		base.OnCircuitChanged(forceUpdate);
		ClearDrains();
		((FacepunchBehaviour)this).Invoke(updateDrainAmountAction, 0.1f);
		if (autofillOutputs && HasLiquidItem())
		{
			((FacepunchBehaviour)this).Invoke(updatePushLiquidTargetsAction, 0.1f);
		}
	}

	public override void OnItemAddedOrRemoved(Item item, bool added)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		base.OnItemAddedOrRemoved(item, added);
		UpdateOnFlag();
		MarkDirtyForceUpdateOutputs();
		((FacepunchBehaviour)this).Invoke(updateDrainAmountAction, 0.1f);
		if (connectedList.get_Count() > 0)
		{
			List<IOEntity> list = Pool.GetList<IOEntity>();
			Enumerator<IOEntity> enumerator = connectedList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					IOEntity current = enumerator.get_Current();
					if ((Object)(object)current != (Object)null)
					{
						list.Add(current);
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			foreach (IOEntity item2 in list)
			{
				item2.SendChangedToRoot(forceUpdate: true);
			}
			Pool.FreeList<IOEntity>(ref list);
		}
		if (HasLiquidItem() && autofillOutputs)
		{
			((FacepunchBehaviour)this).Invoke(updatePushLiquidTargetsAction, 0.1f);
		}
	}

	private void ClearDrains()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		Enumerator<IOEntity> enumerator = connectedList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				IOEntity current = enumerator.get_Current();
				if ((Object)(object)current != (Object)null)
				{
					current.SetFuelType(null, null);
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		connectedList.Clear();
	}

	public override int GetCurrentEnergy()
	{
		return Mathf.Clamp(GetLiquidCount(), 0, maxOutputFlow);
	}

	public override int CalculateCurrentEnergy(int inputAmount, int inputSlot)
	{
		if (!HasLiquidItem())
		{
			return base.CalculateCurrentEnergy(inputAmount, inputSlot);
		}
		return GetCurrentEnergy();
	}

	private void UpdateDrainAmount()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		int amount = 0;
		Item liquidItem = GetLiquidItem();
		if (liquidItem != null)
		{
			IOSlot[] array = outputs;
			foreach (IOSlot iOSlot in array)
			{
				if ((Object)(object)iOSlot.connectedTo.Get() != (Object)null)
				{
					CalculateDrain(iOSlot.connectedTo.Get(), ((Component)this).get_transform().TransformPoint(iOSlot.handlePosition), IOEntity.backtracking, ref amount, this, liquidItem?.info);
				}
			}
		}
		currentDrainAmount = Mathf.Clamp(amount, 0, maxOutputFlow);
		if (currentDrainAmount <= 0 && ((FacepunchBehaviour)this).IsInvoking(deductFuelAction))
		{
			((FacepunchBehaviour)this).CancelInvoke(deductFuelAction);
		}
		else if (currentDrainAmount > 0 && !((FacepunchBehaviour)this).IsInvoking(deductFuelAction))
		{
			((FacepunchBehaviour)this).InvokeRepeating(deductFuelAction, 0f, 1f);
		}
	}

	private void CalculateDrain(IOEntity ent, Vector3 fromSlotWorld, int depth, ref int amount, IOEntity lastEntity, ItemDefinition waterType)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)ent == (Object)(object)this || depth <= 0 || (Object)(object)ent == (Object)null || (Object)(object)lastEntity == (Object)null || ent is LiquidContainer)
		{
			return;
		}
		if (!ent.BlockFluidDraining && ent.HasFlag(Flags.On))
		{
			int num = ent.DesiredPower();
			amount += num;
			ent.SetFuelType(waterType, this);
			connectedList.Add(ent);
		}
		if (!ent.AllowLiquidPassthrough(lastEntity, fromSlotWorld))
		{
			return;
		}
		IOSlot[] array = ent.outputs;
		foreach (IOSlot iOSlot in array)
		{
			if ((Object)(object)iOSlot.connectedTo.Get() != (Object)null && (Object)(object)iOSlot.connectedTo.Get() != (Object)(object)ent)
			{
				CalculateDrain(iOSlot.connectedTo.Get(), ((Component)ent).get_transform().TransformPoint(iOSlot.handlePosition), depth - 1, ref amount, ent, waterType);
			}
		}
	}

	public override void UpdateOutputs()
	{
		base.UpdateOutputs();
		if (!(Time.get_realtimeSinceStartup() - lastOutputDrainUpdate < 0.2f))
		{
			lastOutputDrainUpdate = Time.get_realtimeSinceStartup();
			if (HasLiquidItem() && autofillOutputs)
			{
				UpdatePushLiquidTargets();
			}
			ClearDrains();
			((FacepunchBehaviour)this).Invoke(updateDrainAmountAction, 0.1f);
		}
	}

	private void DeductFuel()
	{
		if (HasLiquidItem())
		{
			Item liquidItem = GetLiquidItem();
			liquidItem.amount -= currentDrainAmount;
			liquidItem.MarkDirty();
			if (liquidItem.amount <= 0)
			{
				liquidItem.Remove();
			}
		}
	}

	protected void UpdateOnFlag()
	{
		SetFlag(Flags.On, base.inventory.itemList.Count > 0 && base.inventory.itemList[0].amount > 0);
	}

	public virtual void OpenTap(float duration)
	{
		if (!HasFlag(Flags.Reserved5))
		{
			SetFlag(Flags.Reserved5, b: true);
			((FacepunchBehaviour)this).Invoke((Action)ShutTap, duration);
			SendNetworkUpdateImmediate();
		}
	}

	public virtual void ShutTap()
	{
		SetFlag(Flags.Reserved5, b: false);
		SendNetworkUpdateImmediate();
	}

	public bool HasLiquidItem()
	{
		return GetLiquidItem() != null;
	}

	public Item GetLiquidItem()
	{
		if (base.inventory.itemList.Count == 0)
		{
			return null;
		}
		return base.inventory.itemList[0];
	}

	public int GetLiquidCount()
	{
		if (!HasLiquidItem())
		{
			return 0;
		}
		return GetLiquidItem().amount;
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void SVDrink(RPCMessage rpc)
	{
		if (!rpc.player.metabolism.CanConsume())
		{
			return;
		}
		foreach (Item item in base.inventory.itemList)
		{
			ItemModConsume component = ((Component)item.info).GetComponent<ItemModConsume>();
			if (!((Object)(object)component == (Object)null) && component.CanDoAction(item, rpc.player))
			{
				component.DoAction(item, rpc.player);
				break;
			}
		}
	}

	private void UpdatePushLiquidTargets()
	{
		pushTargets.Clear();
		if (!HasLiquidItem() || IsConnectedTo(this, IOEntity.backtracking * 2))
		{
			return;
		}
		Item liquidItem = GetLiquidItem();
		TimeWarning val = TimeWarning.New("UpdatePushTargets", 0);
		try
		{
			IOSlot[] array = outputs;
			foreach (IOSlot iOSlot in array)
			{
				if (iOSlot.type == IOType.Fluidic)
				{
					IOEntity iOEntity = iOSlot.connectedTo.Get();
					if ((Object)(object)iOEntity != (Object)null)
					{
						CheckPushLiquid(iOEntity, liquidItem, this, IOEntity.backtracking * 4);
					}
				}
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		if (pushTargets.get_Count() > 0)
		{
			((FacepunchBehaviour)this).InvokeRandomized(pushLiquidAction, 0f, autofillTickRate, autofillTickRate * 0.2f);
		}
	}

	private void PushLiquidThroughOutputs()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (!HasLiquidItem())
		{
			((FacepunchBehaviour)this).CancelInvoke(pushLiquidAction);
			return;
		}
		Item liquidItem = GetLiquidItem();
		if (pushTargets.get_Count() > 0)
		{
			int num = Mathf.Clamp(autofillTickAmount, 0, liquidItem.amount) / pushTargets.get_Count();
			if (num == 0 && liquidItem.amount > 0)
			{
				num = liquidItem.amount;
			}
			Enumerator<ContainerIOEntity> enumerator = pushTargets.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ContainerIOEntity current = enumerator.get_Current();
					if (current.inventory.CanAcceptItem(liquidItem, 0) == ItemContainer.CanAcceptResult.CanAccept && (current.inventory.CanAccept(liquidItem) || current.inventory.FindItemByItemID(liquidItem.info.itemid) != null))
					{
						int num2 = Mathf.Clamp(num, 0, current.inventory.GetMaxTransferAmount(liquidItem.info));
						current.inventory.AddItem(liquidItem.info, num2, 0uL);
						liquidItem.amount -= num2;
						liquidItem.MarkDirty();
						if (liquidItem.amount <= 0)
						{
							break;
						}
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}
		if (liquidItem.amount <= 0 || pushTargets.get_Count() == 0)
		{
			if (liquidItem.amount <= 0)
			{
				liquidItem.Remove();
			}
			((FacepunchBehaviour)this).CancelInvoke(pushLiquidAction);
		}
	}

	private void CheckPushLiquid(IOEntity connected, Item ourFuel, IOEntity fromSource, int depth)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		if (depth <= 0 || ourFuel.amount <= 0)
		{
			return;
		}
		Vector3 worldHandlePosition = Vector3.get_zero();
		IOEntity iOEntity = connected.FindGravitySource(ref worldHandlePosition, IOEntity.backtracking, ignoreSelf: true);
		if (((Object)(object)iOEntity != (Object)null && !connected.AllowLiquidPassthrough(iOEntity, worldHandlePosition)) || (Object)(object)connected == (Object)(object)this || ConsiderConnectedTo(connected))
		{
			return;
		}
		ContainerIOEntity containerIOEntity;
		if ((containerIOEntity = connected as ContainerIOEntity) != null && !pushTargets.Contains(containerIOEntity) && containerIOEntity.inventory.CanAcceptItem(ourFuel, 0) == ItemContainer.CanAcceptResult.CanAccept)
		{
			pushTargets.Add(containerIOEntity);
			return;
		}
		IOSlot[] array = connected.outputs;
		foreach (IOSlot iOSlot in array)
		{
			IOEntity iOEntity2 = iOSlot.connectedTo.Get();
			Vector3 sourceWorldPosition = ((Component)connected).get_transform().TransformPoint(iOSlot.handlePosition);
			if ((Object)(object)iOEntity2 != (Object)null && (Object)(object)iOEntity2 != (Object)(object)fromSource && iOEntity2.AllowLiquidPassthrough(connected, sourceWorldPosition))
			{
				CheckPushLiquid(iOEntity2, ourFuel, fromSource, depth - 1);
				if (pushTargets.get_Count() >= 3)
				{
					break;
				}
			}
		}
	}

	public void SetConnectedTo(IOEntity entity)
	{
		considerConnectedTo = entity;
	}

	protected override bool ConsiderConnectedTo(IOEntity entity)
	{
		return (Object)(object)entity == (Object)(object)considerConnectedTo;
	}
}
