using System;
using System.IO;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class PatternFirework : MortarFirework
{
	public enum FuseLength
	{
		Short = 0,
		Medium = 1,
		Long = 2,
		Max = 2
	}

	public const int CurrentVersion = 1;

	[Header("PatternFirework")]
	public GameObjectRef FireworkDesignerDialog;

	public int MaxStars = 25;

	public float ShellFuseLengthShort = 3f;

	public float ShellFuseLengthMed = 5.5f;

	public float ShellFuseLengthLong = 8f;

	[NonSerialized]
	public Design Design;

	[NonSerialized]
	public FuseLength ShellFuseLength;

	public override void DestroyShared()
	{
		base.DestroyShared();
		Design design = Design;
		if (design != null)
		{
			design.Dispose();
		}
		Design = null;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		ShellFuseLength = FuseLength.Medium;
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	[RPC_Server.CallsPerSecond(5uL)]
	private void StartOpenDesigner(RPCMessage rpc)
	{
		if (PlayerCanModify(rpc.player))
		{
			ClientRPCPlayer(null, rpc.player, "OpenDesigner");
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	[RPC_Server.CallsPerSecond(5uL)]
	private void ServerSetFireworkDesign(RPCMessage rpc)
	{
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		if (!PlayerCanModify(rpc.player))
		{
			return;
		}
		Design val = Design.Deserialize((Stream)(object)rpc.read);
		if (val?.stars != null)
		{
			while (val.stars.Count > MaxStars)
			{
				int index = val.stars.Count - 1;
				val.stars[index].Dispose();
				val.stars.RemoveAt(index);
			}
			foreach (Star star in val.stars)
			{
				star.position = new Vector2(Mathf.Clamp(star.position.x, -1f, 1f), Mathf.Clamp(star.position.y, -1f, 1f));
				star.color = new Color(Mathf.Clamp01(star.color.r), Mathf.Clamp01(star.color.g), Mathf.Clamp01(star.color.b), 1f);
			}
		}
		Design design = Design;
		if (design != null)
		{
			design.Dispose();
		}
		Design = val;
		SendNetworkUpdateImmediate();
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	[RPC_Server.CallsPerSecond(5uL)]
	private void SetShellFuseLength(RPCMessage rpc)
	{
		if (PlayerCanModify(rpc.player))
		{
			ShellFuseLength = (FuseLength)Mathf.Clamp(rpc.read.Int32(), 0, 2);
			SendNetworkUpdateImmediate();
		}
	}

	private bool PlayerCanModify(BasePlayer player)
	{
		if ((Object)(object)player == (Object)null || !player.CanInteract())
		{
			return false;
		}
		BuildingPrivlidge buildingPrivilege = GetBuildingPrivilege();
		if ((Object)(object)buildingPrivilege != (Object)null && !buildingPrivilege.CanAdministrate(player))
		{
			return false;
		}
		return true;
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.patternFirework = Pool.Get<PatternFirework>();
		PatternFirework patternFirework = info.msg.patternFirework;
		Design design = Design;
		patternFirework.design = ((design != null) ? design.Copy() : null);
		info.msg.patternFirework.shellFuseLength = (int)ShellFuseLength;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.patternFirework != null)
		{
			Design design = Design;
			if (design != null)
			{
				design.Dispose();
			}
			Design design2 = info.msg.patternFirework.design;
			Design = ((design2 != null) ? design2.Copy() : null);
			ShellFuseLength = (FuseLength)info.msg.patternFirework.shellFuseLength;
		}
	}

	private float GetShellFuseLength()
	{
		return ShellFuseLength switch
		{
			FuseLength.Short => ShellFuseLengthShort, 
			FuseLength.Medium => ShellFuseLengthMed, 
			FuseLength.Long => ShellFuseLengthLong, 
			_ => ShellFuseLengthMed, 
		};
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("PatternFirework.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 3850129568u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ServerSetFireworkDesign "));
				}
				TimeWarning val2 = TimeWarning.New("ServerSetFireworkDesign", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(3850129568u, "ServerSetFireworkDesign", this, player, 5uL))
						{
							return true;
						}
						if (!RPC_Server.IsVisible.Test(3850129568u, "ServerSetFireworkDesign", this, player, 3f))
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
							ServerSetFireworkDesign(rpc2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in ServerSetFireworkDesign");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2132764204 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SetShellFuseLength "));
				}
				TimeWarning val2 = TimeWarning.New("SetShellFuseLength", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(2132764204u, "SetShellFuseLength", this, player, 5uL))
						{
							return true;
						}
						if (!RPC_Server.IsVisible.Test(2132764204u, "SetShellFuseLength", this, player, 3f))
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
							RPCMessage shellFuseLength = rPCMessage;
							SetShellFuseLength(shellFuseLength);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in SetShellFuseLength");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2760408151u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - StartOpenDesigner "));
				}
				TimeWarning val2 = TimeWarning.New("StartOpenDesigner", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(2760408151u, "StartOpenDesigner", this, player, 5uL))
						{
							return true;
						}
						if (!RPC_Server.IsVisible.Test(2760408151u, "StartOpenDesigner", this, player, 3f))
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
							StartOpenDesigner(rpc3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in StartOpenDesigner");
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
}
