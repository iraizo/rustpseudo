using System;
using UnityEngine;

public class PlayerBelt
{
	public static int SelectedSlot = -1;

	protected BasePlayer player;

	public static int MaxBeltSlots => 6;

	public PlayerBelt(BasePlayer player)
	{
		this.player = player;
	}

	public void DropActive(Vector3 position, Vector3 velocity)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		Item activeItem = player.GetActiveItem();
		if (activeItem != null)
		{
			TimeWarning val = TimeWarning.New("PlayerBelt.DropActive", 0);
			try
			{
				activeItem.Drop(position, velocity);
				player.svActiveItemID = 0u;
				player.SendNetworkUpdate();
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
	}

	public Item GetItemInSlot(int slot)
	{
		if ((Object)(object)player == (Object)null)
		{
			return null;
		}
		if ((Object)(object)player.inventory == (Object)null)
		{
			return null;
		}
		if (player.inventory.containerBelt == null)
		{
			return null;
		}
		return player.inventory.containerBelt.GetSlot(slot);
	}
}
