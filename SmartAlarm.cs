using System;
using CompanionServer;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class SmartAlarm : AppIOEntity, ISubscribable
{
	public const Flags Flag_HasCustomMessage = Flags.Reserved6;

	public static readonly Phrase DefaultNotificationTitle = new Phrase("app.alarm.title", "Alarm");

	public static readonly Phrase DefaultNotificationBody = new Phrase("app.alarm.body", "Your base is under attack!");

	[Header("Smart Alarm")]
	public GameObjectRef SetupNotificationDialog;

	public Animator Animator;

	private readonly NotificationList _subscriptions = new NotificationList();

	private string _notificationTitle = "";

	private string _notificationBody = "";

	private float _lastSentTime;

	public override AppEntityType Type => (AppEntityType)2;

	public override bool Value { get; set; }

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("SmartAlarm.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 3292290572u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SetNotificationTextImpl "));
				}
				TimeWarning val2 = TimeWarning.New("SetNotificationTextImpl", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(3292290572u, "SetNotificationTextImpl", this, player, 5uL))
						{
							return true;
						}
						if (!RPC_Server.IsVisible.Test(3292290572u, "SetNotificationTextImpl", this, player, 3f))
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
							RPCMessage notificationTextImpl = rPCMessage;
							SetNotificationTextImpl(notificationTextImpl);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SetNotificationTextImpl");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 4207149767u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - StartSetupNotification "));
				}
				TimeWarning val2 = TimeWarning.New("StartSetupNotification", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(4207149767u, "StartSetupNotification", this, player, 5uL))
						{
							return true;
						}
						if (!RPC_Server.IsVisible.Test(4207149767u, "StartSetupNotification", this, player, 3f))
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
							StartSetupNotification(rpc2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in StartSetupNotification");
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

	public bool AddSubscription(ulong steamId)
	{
		return _subscriptions.AddSubscription(steamId);
	}

	public bool RemoveSubscription(ulong steamId)
	{
		return _subscriptions.RemoveSubscription(steamId);
	}

	public bool HasSubscription(ulong steamId)
	{
		return _subscriptions.HasSubscription(steamId);
	}

	public override void InitShared()
	{
		base.InitShared();
		_notificationTitle = DefaultNotificationTitle.get_translated();
		_notificationBody = DefaultNotificationBody.get_translated();
	}

	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		Value = inputAmount > 0;
		if (Value == IsOn())
		{
			return;
		}
		SetFlag(Flags.On, Value);
		BroadcastValueChange();
		float num = Mathf.Max(App.alarmcooldown, 15f);
		if (Value && Time.get_realtimeSinceStartup() - _lastSentTime >= num)
		{
			BuildingPrivlidge buildingPrivilege = GetBuildingPrivilege();
			if ((Object)(object)buildingPrivilege != (Object)null)
			{
				_subscriptions.IntersectWith(buildingPrivilege.authorizedPlayers);
			}
			_subscriptions.SendNotification(NotificationChannel.SmartAlarm, _notificationTitle, _notificationBody, "alarm");
			_lastSentTime = Time.get_realtimeSinceStartup();
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			info.msg.smartAlarm = Pool.Get<SmartAlarm>();
			info.msg.smartAlarm.notificationTitle = _notificationTitle;
			info.msg.smartAlarm.notificationBody = _notificationBody;
			info.msg.smartAlarm.subscriptions = _subscriptions.ToList();
		}
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk && info.msg.smartAlarm != null)
		{
			_notificationTitle = info.msg.smartAlarm.notificationTitle;
			_notificationBody = info.msg.smartAlarm.notificationBody;
			_subscriptions.LoadFrom(info.msg.smartAlarm.subscriptions);
		}
	}

	protected override void OnPairedWithPlayer(BasePlayer player)
	{
		if (!((Object)(object)player == (Object)null) && !AddSubscription(player.userID))
		{
			player.ClientRPCPlayer(null, player, "HandleCompanionPairingResult", 7);
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	[RPC_Server.CallsPerSecond(5uL)]
	private void StartSetupNotification(RPCMessage rpc)
	{
		if (rpc.player.CanInteract())
		{
			BuildingPrivlidge buildingPrivilege = GetBuildingPrivilege();
			if (!((Object)(object)buildingPrivilege != (Object)null) || buildingPrivilege.CanAdministrate(rpc.player))
			{
				ClientRPCPlayer(null, rpc.player, "SetupNotification", _notificationTitle, _notificationBody);
			}
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	[RPC_Server.CallsPerSecond(5uL)]
	private void SetNotificationTextImpl(RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		BuildingPrivlidge buildingPrivilege = GetBuildingPrivilege();
		if (!((Object)(object)buildingPrivilege != (Object)null) || buildingPrivilege.CanAdministrate(rpc.player))
		{
			string text = rpc.read.String(128);
			string text2 = rpc.read.String(512);
			if (!string.IsNullOrWhiteSpace(text))
			{
				_notificationTitle = text;
			}
			if (!string.IsNullOrWhiteSpace(text2))
			{
				_notificationBody = text2;
			}
			SetFlag(Flags.Reserved6, b: true);
		}
	}
}
