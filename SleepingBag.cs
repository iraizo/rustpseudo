using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class SleepingBag : DecayEntity
{
	[NonSerialized]
	public ulong deployerUserID;

	public GameObject renameDialog;

	public GameObject assignDialog;

	public float secondsBetweenReuses = 300f;

	public string niceName = "Unnamed Bag";

	public Vector3 spawnOffset = Vector3.get_zero();

	public RespawnType RespawnType = (RespawnType)1;

	public bool isStatic;

	public bool canBePublic;

	public const Flags IsPublicFlag = Flags.Reserved3;

	internal float unlockTime;

	private static List<SleepingBag> sleepingBags = new List<SleepingBag>();

	public virtual float unlockSeconds
	{
		get
		{
			if (unlockTime < Time.get_realtimeSinceStartup())
			{
				return 0f;
			}
			return unlockTime - Time.get_realtimeSinceStartup();
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("SleepingBag.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 3057055788u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - AssignToFriend "));
				}
				TimeWarning val2 = TimeWarning.New("AssignToFriend", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3057055788u, "AssignToFriend", this, player, 3f))
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
							AssignToFriend(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in AssignToFriend");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1335950295 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Rename "));
				}
				TimeWarning val2 = TimeWarning.New("Rename", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1335950295u, "Rename", this, player, 3f))
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
							Rename(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in Rename");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 42669546 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_MakeBed "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_MakeBed", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(42669546u, "RPC_MakeBed", this, player, 3f))
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
							RPC_MakeBed(msg4);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in RPC_MakeBed");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 393812086 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_MakePublic "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_MakePublic", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(393812086u, "RPC_MakePublic", this, player, 3f))
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
							RPCMessage msg5 = rPCMessage;
							RPC_MakePublic(msg5);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in RPC_MakePublic");
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

	public bool IsPublic()
	{
		return HasFlag(Flags.Reserved3);
	}

	public virtual float GetUnlockSeconds(ulong playerID)
	{
		return unlockSeconds;
	}

	public virtual bool ValidForPlayer(ulong playerID, bool ignoreTimers)
	{
		if (deployerUserID == playerID)
		{
			if (!ignoreTimers)
			{
				return unlockTime < Time.get_realtimeSinceStartup();
			}
			return true;
		}
		return false;
	}

	public static SleepingBag[] FindForPlayer(ulong playerID, bool ignoreTimers)
	{
		return Enumerable.ToArray<SleepingBag>(Enumerable.Where<SleepingBag>((IEnumerable<SleepingBag>)sleepingBags, (Func<SleepingBag, bool>)((SleepingBag x) => x.ValidForPlayer(playerID, ignoreTimers))));
	}

	public static SleepingBag FindForPlayer(ulong playerID, uint sleepingBagID, bool ignoreTimers)
	{
		return Enumerable.FirstOrDefault<SleepingBag>((IEnumerable<SleepingBag>)sleepingBags, (Func<SleepingBag, bool>)((SleepingBag x) => x.deployerUserID == playerID && x.net.ID == sleepingBagID && (ignoreTimers || x.unlockTime < Time.get_realtimeSinceStartup())));
	}

	public static bool SpawnPlayer(BasePlayer player, uint sleepingBag)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		SleepingBag[] array = FindForPlayer(player.userID, ignoreTimers: true);
		SleepingBag sleepingBag2 = Enumerable.FirstOrDefault<SleepingBag>((IEnumerable<SleepingBag>)array, (Func<SleepingBag, bool>)((SleepingBag x) => x.ValidForPlayer(player.userID, ignoreTimers: false) && x.net.ID == sleepingBag && x.unlockTime < Time.get_realtimeSinceStartup()));
		if ((Object)(object)sleepingBag2 == (Object)null)
		{
			return false;
		}
		if (sleepingBag2.IsOccupied(player.userID))
		{
			return false;
		}
		sleepingBag2.GetSpawnPos(out var pos, out var rot);
		player.RespawnAt(pos, rot);
		sleepingBag2.PostPlayerSpawn(player);
		SleepingBag[] array2 = array;
		foreach (SleepingBag sleepingBag3 in array2)
		{
			if (Vector3.Distance(pos, ((Component)sleepingBag3).get_transform().get_position()) <= Server.respawnresetrange)
			{
				sleepingBag3.SetUnlockTime(Time.get_realtimeSinceStartup() + sleepingBag3.secondsBetweenReuses);
			}
		}
		return true;
	}

	public static bool TrySpawnPlayer(BasePlayer player, uint sleepingBag, out string errorMessage)
	{
		if (!player.IsDead())
		{
			errorMessage = "Couldn't spawn - player is not dead!";
			return false;
		}
		if (player.CanRespawn())
		{
			if (SpawnPlayer(player, sleepingBag))
			{
				player.MarkRespawn();
				errorMessage = null;
				return true;
			}
			errorMessage = "Couldn't spawn in sleeping bag!";
			return false;
		}
		errorMessage = "You can't respawn again so quickly, wait a while";
		return false;
	}

	public virtual void SetUnlockTime(float newTime)
	{
		unlockTime = newTime;
	}

	public static bool DestroyBag(ulong userID, uint sleepingBag)
	{
		SleepingBag sleepingBag2 = FindForPlayer(userID, sleepingBag, ignoreTimers: false);
		if ((Object)(object)sleepingBag2 == (Object)null)
		{
			return false;
		}
		if (sleepingBag2.canBePublic)
		{
			sleepingBag2.SetPublic(isPublic: true);
			sleepingBag2.deployerUserID = 0uL;
		}
		else
		{
			sleepingBag2.Kill();
		}
		BasePlayer basePlayer = BasePlayer.FindByID(userID);
		if ((Object)(object)basePlayer != (Object)null)
		{
			basePlayer.SendRespawnOptions();
		}
		return true;
	}

	public static void ResetTimersForPlayer(BasePlayer player)
	{
		SleepingBag[] array = FindForPlayer(player.userID, ignoreTimers: true);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].unlockTime = 0f;
		}
	}

	public virtual void GetSpawnPos(out Vector3 pos, out Quaternion rot)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		pos = ((Component)this).get_transform().get_position() + spawnOffset;
		Quaternion rotation = ((Component)this).get_transform().get_rotation();
		rot = Quaternion.Euler(0f, ((Quaternion)(ref rotation)).get_eulerAngles().y, 0f);
	}

	public void SetPublic(bool isPublic)
	{
		SetFlag(Flags.Reserved3, isPublic);
	}

	private void SetDeployedBy(BasePlayer player)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)player == (Object)null)
		{
			return;
		}
		deployerUserID = player.userID;
		float realtimeSinceStartup = Time.get_realtimeSinceStartup();
		SleepingBag[] array = Enumerable.ToArray<SleepingBag>(Enumerable.Where<SleepingBag>((IEnumerable<SleepingBag>)sleepingBags, (Func<SleepingBag, bool>)((SleepingBag x) => x.deployerUserID == player.userID && x.unlockTime > Time.get_realtimeSinceStartup())));
		foreach (SleepingBag sleepingBag in array)
		{
			if (sleepingBag.unlockTime > realtimeSinceStartup && Vector3.Distance(((Component)sleepingBag).get_transform().get_position(), ((Component)this).get_transform().get_position()) <= Server.respawnresetrange)
			{
				realtimeSinceStartup = sleepingBag.unlockTime;
			}
		}
		unlockTime = Mathf.Max(realtimeSinceStartup, Time.get_realtimeSinceStartup() + secondsBetweenReuses);
		SendNetworkUpdate();
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (!sleepingBags.Contains(this))
		{
			sleepingBags.Add(this);
		}
	}

	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		sleepingBags.RemoveAll((SleepingBag x) => (Object)(object)x == (Object)(object)this);
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.sleepingBag = Pool.Get<SleepingBag>();
		info.msg.sleepingBag.name = niceName;
		info.msg.sleepingBag.deployerID = deployerUserID;
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void Rename(RPCMessage msg)
	{
		if (msg.player.CanInteract())
		{
			string str = msg.read.String(256);
			str = WordFilter.Filter(str);
			if (string.IsNullOrEmpty(str))
			{
				str = "Unnamed Sleeping Bag";
			}
			if (str.Length > 24)
			{
				str = str.Substring(0, 22) + "..";
			}
			niceName = str;
			SendNetworkUpdate();
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void AssignToFriend(RPCMessage msg)
	{
		if (msg.player.CanInteract() && deployerUserID == msg.player.userID)
		{
			ulong num = msg.read.UInt64();
			if (num != 0L)
			{
				deployerUserID = num;
				SendNetworkUpdate();
			}
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public virtual void RPC_MakePublic(RPCMessage msg)
	{
		if (!canBePublic || !msg.player.CanInteract() || (deployerUserID != msg.player.userID && !msg.player.CanBuild()))
		{
			return;
		}
		bool flag = msg.read.Bit();
		if (flag != IsPublic())
		{
			SetPublic(flag);
			if (!IsPublic())
			{
				deployerUserID = msg.player.userID;
			}
			SendNetworkUpdate();
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_MakeBed(RPCMessage msg)
	{
		if (canBePublic && IsPublic() && msg.player.CanInteract())
		{
			deployerUserID = msg.player.userID;
			SendNetworkUpdate();
		}
	}

	protected virtual void PostPlayerSpawn(BasePlayer p)
	{
	}

	public virtual bool IsOccupied(ulong userID)
	{
		return false;
	}

	public override string Admin_Who()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(base.Admin_Who());
		stringBuilder.AppendLine($"Assigned bag ID: {deployerUserID}");
		stringBuilder.AppendLine("Assigned player name: " + Admin.GetPlayerName(deployerUserID));
		stringBuilder.AppendLine("Bag Name:" + niceName);
		return stringBuilder.ToString();
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.sleepingBag != null)
		{
			niceName = info.msg.sleepingBag.name;
			deployerUserID = info.msg.sleepingBag.deployerID;
		}
	}

	public override bool CanPickup(BasePlayer player)
	{
		if (base.CanPickup(player))
		{
			return player.userID == deployerUserID;
		}
		return false;
	}
}
