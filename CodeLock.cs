using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class CodeLock : BaseLock
{
	public GameObjectRef keyEnterDialog;

	public GameObjectRef effectUnlocked;

	public GameObjectRef effectLocked;

	public GameObjectRef effectDenied;

	public GameObjectRef effectCodeChanged;

	public GameObjectRef effectShock;

	private bool hasCode;

	public const Flags Flag_CodeEntryBlocked = Flags.Reserved11;

	public static readonly Phrase blockwarning = new Phrase("codelock.blockwarning", "Further failed attempts will block code entry for some time");

	[ServerVar]
	public static float maxFailedAttempts = 8f;

	[ServerVar]
	public static float lockoutCooldown = 900f;

	private bool hasGuestCode;

	private string code = string.Empty;

	private string guestCode = string.Empty;

	public List<ulong> whitelistPlayers = new List<ulong>();

	public List<ulong> guestPlayers = new List<ulong>();

	private int wrongCodes;

	private float lastWrongTime = float.NegativeInfinity;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("CodeLock.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 4013784361u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_ChangeCode "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_ChangeCode", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(4013784361u, "RPC_ChangeCode", this, player, 3f))
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
							RPC_ChangeCode(rpc2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_ChangeCode");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2626067433u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - TryLock "));
				}
				TimeWarning val2 = TimeWarning.New("TryLock", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(2626067433u, "TryLock", this, player, 3f))
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
							TryLock(rpc3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in TryLock");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1718262 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - TryUnlock "));
				}
				TimeWarning val2 = TimeWarning.New("TryUnlock", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(1718262u, "TryUnlock", this, player, 3f))
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
							TryUnlock(rpc4);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in TryUnlock");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 418605506 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - UnlockWithCode "));
				}
				TimeWarning val2 = TimeWarning.New("UnlockWithCode", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(418605506u, "UnlockWithCode", this, player, 3f))
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
							RPCMessage rpc5 = rPCMessage;
							UnlockWithCode(rpc5);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in UnlockWithCode");
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

	public bool IsCodeEntryBlocked()
	{
		return HasFlag(Flags.Reserved11);
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.codeLock == null)
		{
			return;
		}
		hasCode = info.msg.codeLock.hasCode;
		hasGuestCode = info.msg.codeLock.hasGuestCode;
		if (info.msg.codeLock.pv != null)
		{
			code = info.msg.codeLock.pv.code;
			whitelistPlayers = info.msg.codeLock.pv.users;
			guestCode = info.msg.codeLock.pv.guestCode;
			guestPlayers = info.msg.codeLock.pv.guestUsers;
			if (guestCode == null || guestCode.Length != 4)
			{
				hasGuestCode = false;
				guestCode = string.Empty;
				guestPlayers.Clear();
			}
		}
	}

	internal void DoEffect(string effect)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Effect.server.Run(effect, this, 0u, Vector3.get_zero(), Vector3.get_forward());
	}

	public override bool OnTryToOpen(BasePlayer player)
	{
		if (!IsLocked())
		{
			return true;
		}
		if (whitelistPlayers.Contains(player.userID) || guestPlayers.Contains(player.userID))
		{
			DoEffect(effectUnlocked.resourcePath);
			return true;
		}
		DoEffect(effectDenied.resourcePath);
		return false;
	}

	public override bool OnTryToClose(BasePlayer player)
	{
		if (!IsLocked())
		{
			return true;
		}
		if (whitelistPlayers.Contains(player.userID) || guestPlayers.Contains(player.userID))
		{
			DoEffect(effectUnlocked.resourcePath);
			return true;
		}
		DoEffect(effectDenied.resourcePath);
		return false;
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.codeLock = Pool.Get<CodeLock>();
		info.msg.codeLock.hasGuestCode = guestCode.Length > 0;
		info.msg.codeLock.hasCode = code.Length > 0;
		if (info.forDisk)
		{
			info.msg.codeLock.pv = Pool.Get<Private>();
			info.msg.codeLock.pv.code = code;
			info.msg.codeLock.pv.users = Pool.Get<List<ulong>>();
			info.msg.codeLock.pv.users.AddRange(whitelistPlayers);
			info.msg.codeLock.pv.guestCode = guestCode;
			info.msg.codeLock.pv.guestUsers = Pool.Get<List<ulong>>();
			info.msg.codeLock.pv.guestUsers.AddRange(guestPlayers);
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	private void RPC_ChangeCode(RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		string text = rpc.read.String(256);
		bool flag = rpc.read.Bit();
		if (!IsLocked() && text.Length == 4 && StringEx.IsNumeric(text) && !(!hasCode && flag))
		{
			if (!hasCode && !flag)
			{
				SetFlag(Flags.Locked, b: true);
			}
			if (!flag)
			{
				code = text;
				hasCode = code.Length > 0;
				whitelistPlayers.Clear();
				whitelistPlayers.Add(rpc.player.userID);
			}
			else
			{
				guestCode = text;
				hasGuestCode = guestCode.Length > 0;
				guestPlayers.Clear();
				guestPlayers.Add(rpc.player.userID);
			}
			DoEffect(effectCodeChanged.resourcePath);
			SendNetworkUpdate();
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	private void TryUnlock(RPCMessage rpc)
	{
		if (rpc.player.CanInteract() && IsLocked() && !IsCodeEntryBlocked())
		{
			if (whitelistPlayers.Contains(rpc.player.userID))
			{
				DoEffect(effectUnlocked.resourcePath);
				SetFlag(Flags.Locked, b: false);
				SendNetworkUpdate();
			}
			else
			{
				ClientRPCPlayer(null, rpc.player, "EnterUnlockCode");
			}
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	private void TryLock(RPCMessage rpc)
	{
		if (rpc.player.CanInteract() && !IsLocked() && code.Length == 4 && whitelistPlayers.Contains(rpc.player.userID))
		{
			DoEffect(effectLocked.resourcePath);
			SetFlag(Flags.Locked, b: true);
			SendNetworkUpdate();
		}
	}

	public void ClearCodeEntryBlocked()
	{
		SetFlag(Flags.Reserved11, b: false);
		wrongCodes = 0;
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	private void UnlockWithCode(RPCMessage rpc)
	{
		if (!rpc.player.CanInteract() || !IsLocked() || IsCodeEntryBlocked())
		{
			return;
		}
		string text = rpc.read.String(256);
		bool flag = text == guestCode;
		bool flag2 = text == code;
		if (!(text == code) && (!hasGuestCode || !(text == guestCode)))
		{
			if (Time.get_realtimeSinceStartup() > lastWrongTime + 60f)
			{
				wrongCodes = 0;
			}
			DoEffect(effectDenied.resourcePath);
			DoEffect(effectShock.resourcePath);
			rpc.player.Hurt((float)(wrongCodes + 1) * 5f, DamageType.ElectricShock, this, useProtection: false);
			wrongCodes++;
			if (wrongCodes > 5)
			{
				rpc.player.ShowToast(1, blockwarning);
			}
			if ((float)wrongCodes >= maxFailedAttempts)
			{
				SetFlag(Flags.Reserved11, b: true);
				((FacepunchBehaviour)this).Invoke((Action)ClearCodeEntryBlocked, lockoutCooldown);
			}
			lastWrongTime = Time.get_realtimeSinceStartup();
			return;
		}
		SendNetworkUpdate();
		if (flag2)
		{
			if (!whitelistPlayers.Contains(rpc.player.userID))
			{
				DoEffect(effectCodeChanged.resourcePath);
				whitelistPlayers.Add(rpc.player.userID);
				wrongCodes = 0;
			}
		}
		else if (flag && !guestPlayers.Contains(rpc.player.userID))
		{
			DoEffect(effectCodeChanged.resourcePath);
			guestPlayers.Add(rpc.player.userID);
		}
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		SetFlag(Flags.Reserved11, b: false);
	}
}
