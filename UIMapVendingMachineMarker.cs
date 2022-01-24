using System;
using ProtoBuf;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

public class UIMapVendingMachineMarker : MonoBehaviour
{
	public Color inStock;

	public Color outOfStock;

	public Image colorBackground;

	public string displayName;

	public Tooltip toolTip;

	public RustButton button;

	[NonSerialized]
	public bool isInStock;

	[NonSerialized]
	public EntityRef<VendingMachine> vendingMachine;

	[NonSerialized]
	public VendingMachine vendingMachineData;

	public static event Action<UIMapVendingMachineMarker> onClicked;

	public void SetOutOfStock(bool stock)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		((Graphic)colorBackground).set_color(stock ? inStock : outOfStock);
		isInStock = stock;
	}

	public void UpdateInfo(VendingMachine vendingMachineData)
	{
		vendingMachine = new EntityRef<VendingMachine>(vendingMachineData.networkID);
		VendingMachine obj = this.vendingMachineData;
		if (obj != null)
		{
			obj.Dispose();
		}
		this.vendingMachineData = vendingMachineData.Copy();
		displayName = StringEx.EscapeRichText(vendingMachineData.shopName);
		toolTip.Text = displayName;
		if (isInStock && vendingMachineData?.sellOrderContainer?.sellOrders != null && vendingMachineData.sellOrderContainer.sellOrders.Count > 0)
		{
			toolTip.Text += "\n";
			foreach (SellOrder sellOrder in vendingMachineData.sellOrderContainer.sellOrders)
			{
				if (sellOrder.inStock > 0)
				{
					string text = ItemManager.FindItemDefinition(sellOrder.itemToSellID).displayName.get_translated() + (sellOrder.itemToSellIsBP ? " (BP)" : "");
					string text2 = ItemManager.FindItemDefinition(sellOrder.currencyID).displayName.get_translated() + (sellOrder.currencyIsBP ? " (BP)" : "");
					Tooltip tooltip = toolTip;
					tooltip.Text = tooltip.Text + "\n" + sellOrder.itemToSellAmount + " " + text + " | " + sellOrder.currencyAmountPerItem + " " + text2;
					tooltip = toolTip;
					tooltip.Text = tooltip.Text + " (" + sellOrder.inStock + " Left)";
				}
			}
		}
		((Behaviour)toolTip).set_enabled(toolTip.Text != "");
		if ((Object)(object)button != (Object)null)
		{
			((RustControl)button).SetDisabled(UIMapVendingMachineMarker.onClicked == null);
		}
	}

	public void Clicked()
	{
		UIMapVendingMachineMarker.onClicked?.Invoke(this);
	}

	public static void RemoveAllHandlers()
	{
		UIMapVendingMachineMarker.onClicked = null;
	}

	public UIMapVendingMachineMarker()
		: this()
	{
	}
}
