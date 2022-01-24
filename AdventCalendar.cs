using System;
using System.Collections.Generic;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class AdventCalendar : BaseCombatEntity
{
	[Serializable]
	public class DayReward
	{
		public ItemAmount[] rewards;
	}

	public int startMonth;

	public int startDay;

	public DayReward[] days;

	public GameObject[] crosses;

	public static List<AdventCalendar> all = new List<AdventCalendar>();

	public static Dictionary<ulong, List<int>> playerRewardHistory = new Dictionary<ulong, List<int>>();

	public static readonly Phrase CheckLater = new Phrase("adventcalendar.checklater", "You've already claimed today's gift. Come back tomorrow.");

	public static readonly Phrase EventOver = new Phrase("adventcalendar.eventover", "The Advent Calendar event is over. See you next year.");

	public GameObjectRef giftEffect;

	public GameObjectRef boxCloseEffect;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("AdventCalendar.OnRpcMessage", 0);
		try
		{
			if (rpc == 1911254136 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_RequestGift "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_RequestGift", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(1911254136u, "RPC_RequestGift", this, player, 1uL))
						{
							return true;
						}
						if (!RPC_Server.IsVisible.Test(1911254136u, "RPC_RequestGift", this, player, 3f))
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
							RPC_RequestGift(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_RequestGift");
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

	public override void ServerInit()
	{
		base.ServerInit();
		all.Add(this);
	}

	public override void DestroyShared()
	{
		all.Remove(this);
		base.DestroyShared();
	}

	public void AwardGift(BasePlayer player)
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		DateTime now = DateTime.Now;
		int num = now.Day - startDay;
		if (now.Month == startMonth && num >= 0 && num < days.Length)
		{
			if (!playerRewardHistory.ContainsKey(player.userID))
			{
				playerRewardHistory.Add(player.userID, new List<int>());
			}
			playerRewardHistory[player.userID].Add(num);
			Effect.server.Run(giftEffect.resourcePath, ((Component)player).get_transform().get_position());
			if (num >= 0 && num < crosses.Length)
			{
				Effect.server.Run(boxCloseEffect.resourcePath, ((Component)this).get_transform().get_position() + Vector3.get_up() * 1.5f);
			}
			DayReward dayReward = days[num];
			for (int i = 0; i < dayReward.rewards.Length; i++)
			{
				ItemAmount itemAmount = dayReward.rewards[i];
				player.GiveItem(ItemManager.CreateByItemID(itemAmount.itemid, Mathf.CeilToInt(itemAmount.amount), 0uL), GiveItemReason.PickedUp);
			}
		}
	}

	public bool WasAwardedTodaysGift(BasePlayer player)
	{
		if (!playerRewardHistory.ContainsKey(player.userID))
		{
			return false;
		}
		DateTime now = DateTime.Now;
		if (now.Month != startMonth)
		{
			return true;
		}
		int num = now.Day - startDay;
		if (num < 0 || num >= days.Length)
		{
			return true;
		}
		if (playerRewardHistory[player.userID].Contains(num))
		{
			return true;
		}
		return false;
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	[RPC_Server.CallsPerSecond(1uL)]
	public void RPC_RequestGift(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (WasAwardedTodaysGift(player))
		{
			player.ShowToast(1, CheckLater);
		}
		else
		{
			AwardGift(player);
		}
	}
}
