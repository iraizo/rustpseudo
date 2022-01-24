using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using Network.Visibility;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class ComputerStation : BaseMountable
{
	[Header("Computer")]
	public GameObjectRef menuPrefab;

	public ComputerMenu computerMenu;

	public EntityRef currentlyControllingEnt;

	public Dictionary<string, uint> controlBookmarks = new Dictionary<string, uint>();

	public Transform leftHandIKPosition;

	public Transform rightHandIKPosition;

	public SoundDefinition turnOnSoundDef;

	public SoundDefinition turnOffSoundDef;

	public SoundDefinition onLoopSoundDef;

	public bool isStatic;

	public float autoGatherRadius;

	private float nextAddTime;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("ComputerStation.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 481778085 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - AddBookmark "));
				}
				TimeWarning val2 = TimeWarning.New("AddBookmark", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.get_read();
						RPCMessage msg2 = rPCMessage;
						AddBookmark(msg2);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
					player.Kick("RPC Error in AddBookmark");
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 552248427 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - BeginControllingBookmark "));
				}
				TimeWarning val2 = TimeWarning.New("BeginControllingBookmark", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.get_read();
						RPCMessage msg3 = rPCMessage;
						BeginControllingBookmark(msg3);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex2)
				{
					Debug.LogException(ex2);
					player.Kick("RPC Error in BeginControllingBookmark");
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2498687923u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - DeleteBookmark "));
				}
				TimeWarning val2 = TimeWarning.New("DeleteBookmark", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.get_read();
						RPCMessage msg4 = rPCMessage;
						DeleteBookmark(msg4);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex3)
				{
					Debug.LogException(ex3);
					player.Kick("RPC Error in DeleteBookmark");
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2139261430 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Server_DisconnectControl "));
				}
				TimeWarning val2 = TimeWarning.New("Server_DisconnectControl", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.get_read();
						RPCMessage msg5 = rPCMessage;
						Server_DisconnectControl(msg5);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex4)
				{
					Debug.LogException(ex4);
					player.Kick("RPC Error in Server_DisconnectControl");
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

	public static bool IsValidIdentifier(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return false;
		}
		if (str.Length > 32)
		{
			return false;
		}
		return StringEx.IsAlphaNumeric(str);
	}

	public override void DestroyShared()
	{
		if (base.isServer && Object.op_Implicit((Object)(object)GetMounted()))
		{
			StopControl(GetMounted());
		}
		base.DestroyShared();
	}

	public override void ServerInit()
	{
		base.ServerInit();
		((FacepunchBehaviour)this).Invoke((Action)GatherStaticCameras, 5f);
	}

	public void GatherStaticCameras()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (Application.isLoadingSave)
		{
			((FacepunchBehaviour)this).Invoke((Action)GatherStaticCameras, 1f);
		}
		else
		{
			if (!isStatic || !(autoGatherRadius > 0f))
			{
				return;
			}
			List<BaseEntity> list = Pool.GetList<BaseEntity>();
			Vis.Entities(((Component)this).get_transform().get_position(), autoGatherRadius, list, 256, (QueryTriggerInteraction)1);
			foreach (BaseEntity item in list)
			{
				IRemoteControllable component = ((Component)item).GetComponent<IRemoteControllable>();
				if (component != null)
				{
					CCTV_RC component2 = ((Component)item).GetComponent<CCTV_RC>();
					if ((!Object.op_Implicit((Object)(object)component2) || component2.IsStatic()) && !controlBookmarks.ContainsKey(component.GetIdentifier()))
					{
						ForceAddBookmark(component.GetIdentifier());
					}
				}
			}
			Pool.FreeList<BaseEntity>(ref list);
		}
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		GatherStaticCameras();
	}

	public void SetPlayerSecondaryGroupFor(BaseEntity ent)
	{
		BasePlayer mounted = _mounted;
		if (Object.op_Implicit((Object)(object)mounted))
		{
			mounted.net.SwitchSecondaryGroup(Object.op_Implicit((Object)(object)ent) ? ent.net.group : null);
		}
	}

	public void StopControl(BasePlayer ply)
	{
		BaseEntity baseEntity = currentlyControllingEnt.Get(serverside: true);
		if (Object.op_Implicit((Object)(object)baseEntity))
		{
			((Component)baseEntity).GetComponent<IRemoteControllable>().StopControl();
			if (Object.op_Implicit((Object)(object)ply))
			{
				ply.net.SwitchSecondaryGroup((Group)null);
			}
		}
		currentlyControllingEnt.uid = 0u;
		SendNetworkUpdate();
		SendControlBookmarks(ply);
		((FacepunchBehaviour)this).CancelInvoke((Action)ControlCheck);
	}

	public bool IsPlayerAdmin(BasePlayer player)
	{
		return (Object)(object)player == (Object)(object)_mounted;
	}

	[RPC_Server]
	public void DeleteBookmark(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!IsPlayerAdmin(player) || isStatic)
		{
			return;
		}
		string text = msg.read.String(256);
		if (IsValidIdentifier(text) && controlBookmarks.ContainsKey(text))
		{
			uint num = controlBookmarks[text];
			controlBookmarks.Remove(text);
			SendControlBookmarks(player);
			if (num == currentlyControllingEnt.uid)
			{
				currentlyControllingEnt.Set(null);
				SendNetworkUpdate();
			}
		}
	}

	[RPC_Server]
	public void Server_DisconnectControl(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (IsPlayerAdmin(player))
		{
			StopControl(player);
		}
	}

	[RPC_Server]
	public void BeginControllingBookmark(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!IsPlayerAdmin(player))
		{
			return;
		}
		string text = msg.read.String(256);
		if (!IsValidIdentifier(text) || !controlBookmarks.ContainsKey(text))
		{
			return;
		}
		uint uid = controlBookmarks[text];
		BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(uid);
		if ((Object)(object)baseNetworkable == (Object)null)
		{
			return;
		}
		IRemoteControllable component = ((Component)baseNetworkable).GetComponent<IRemoteControllable>();
		if (component.CanControl() && !(component.GetIdentifier() != text))
		{
			BaseEntity baseEntity = currentlyControllingEnt.Get(serverside: true);
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				((Component)baseEntity).GetComponent<IRemoteControllable>()?.StopControl();
			}
			player.net.SwitchSecondaryGroup(baseNetworkable.net.group);
			currentlyControllingEnt.uid = baseNetworkable.net.ID;
			SendNetworkUpdateImmediate();
			SendControlBookmarks(player);
			component.InitializeControl(player);
			((FacepunchBehaviour)this).InvokeRepeating((Action)ControlCheck, 0f, 0f);
		}
	}

	public bool CanAddBookmark(BasePlayer player)
	{
		if (!IsPlayerAdmin(player))
		{
			return false;
		}
		if (isStatic)
		{
			return false;
		}
		if (Time.get_realtimeSinceStartup() < nextAddTime)
		{
			return false;
		}
		if (controlBookmarks.Count > 3)
		{
			player.ChatMessage("Too many bookmarks, delete some");
			return false;
		}
		return true;
	}

	public void ForceAddBookmark(string identifier)
	{
		if (controlBookmarks.Count >= 128 || !IsValidIdentifier(identifier))
		{
			return;
		}
		foreach (KeyValuePair<string, uint> controlBookmark in controlBookmarks)
		{
			if (controlBookmark.Key == identifier)
			{
				return;
			}
		}
		uint num = 0u;
		bool flag = false;
		foreach (IRemoteControllable allControllable in RemoteControlEntity.allControllables)
		{
			if (allControllable != null && allControllable.GetIdentifier() == identifier)
			{
				if (!((Object)(object)allControllable.GetEnt() == (Object)null))
				{
					num = allControllable.GetEnt().net.ID;
					flag = true;
					break;
				}
				Debug.LogWarning((object)"Computer station added bookmark with missing ent, likely a static CCTV (wipe the server)");
			}
		}
		if (!flag)
		{
			return;
		}
		BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(num);
		if ((Object)(object)baseNetworkable == (Object)null)
		{
			return;
		}
		IRemoteControllable component = ((Component)baseNetworkable).GetComponent<IRemoteControllable>();
		if (component != null)
		{
			string identifier2 = component.GetIdentifier();
			if (identifier == identifier2)
			{
				controlBookmarks.Add(identifier, num);
			}
		}
	}

	[RPC_Server]
	public void AddBookmark(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!IsPlayerAdmin(player) || isStatic)
		{
			return;
		}
		if (Time.get_realtimeSinceStartup() < nextAddTime)
		{
			player.ChatMessage("Slow down...");
			return;
		}
		if (controlBookmarks.Count >= 128)
		{
			player.ChatMessage("Too many bookmarks, delete some");
			return;
		}
		nextAddTime = Time.get_realtimeSinceStartup() + 1f;
		string text = msg.read.String(256);
		if (!IsValidIdentifier(text))
		{
			return;
		}
		foreach (KeyValuePair<string, uint> controlBookmark in controlBookmarks)
		{
			if (controlBookmark.Key == text)
			{
				return;
			}
		}
		uint num = 0u;
		bool flag = false;
		foreach (IRemoteControllable allControllable in RemoteControlEntity.allControllables)
		{
			if (allControllable != null && allControllable.GetIdentifier() == text)
			{
				if (!((Object)(object)allControllable.GetEnt() == (Object)null))
				{
					num = allControllable.GetEnt().net.ID;
					flag = true;
					break;
				}
				Debug.LogWarning((object)"Computer station added bookmark with missing ent, likely a static CCTV (wipe the server)");
			}
		}
		if (!flag)
		{
			return;
		}
		BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(num);
		if ((Object)(object)baseNetworkable == (Object)null)
		{
			return;
		}
		IRemoteControllable component = ((Component)baseNetworkable).GetComponent<IRemoteControllable>();
		if (component != null)
		{
			string identifier = component.GetIdentifier();
			if (text == identifier)
			{
				controlBookmarks.Add(text, num);
			}
			SendControlBookmarks(player);
		}
	}

	public void ControlCheck()
	{
		bool flag = false;
		BaseEntity baseEntity = currentlyControllingEnt.Get(base.isServer);
		if (Object.op_Implicit((Object)(object)baseEntity))
		{
			IRemoteControllable component = ((Component)baseEntity).GetComponent<IRemoteControllable>();
			if (component != null && component.CanControl())
			{
				flag = true;
				if ((Object)(object)_mounted != (Object)null)
				{
					_mounted.net.SwitchSecondaryGroup(baseEntity.net.group);
				}
			}
		}
		if (!flag)
		{
			StopControl(_mounted);
		}
	}

	public string GenerateControlBookmarkString()
	{
		string text = "";
		foreach (KeyValuePair<string, uint> controlBookmark in controlBookmarks)
		{
			text += controlBookmark.Key;
			text += ":";
			text += controlBookmark.Value;
			text += ";";
		}
		return text;
	}

	public void SendControlBookmarks(BasePlayer player)
	{
		if (!((Object)(object)player == (Object)null))
		{
			string arg = GenerateControlBookmarkString();
			ClientRPCPlayer(null, player, "ReceiveBookmarks", arg);
		}
	}

	public override void OnPlayerMounted()
	{
		BasePlayer mounted = _mounted;
		if (Object.op_Implicit((Object)(object)mounted))
		{
			SendControlBookmarks(mounted);
		}
		SetFlag(Flags.On, b: true);
	}

	public override void OnPlayerDismounted(BasePlayer player)
	{
		base.OnPlayerDismounted(player);
		StopControl(player);
		SetFlag(Flags.On, b: false);
	}

	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		base.PlayerServerInput(inputState, player);
		if (currentlyControllingEnt.IsValid(serverside: true))
		{
			((Component)currentlyControllingEnt.Get(serverside: true)).GetComponent<IRemoteControllable>().UserInput(inputState, player);
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
		{
			info.msg.ioEntity = Pool.Get<IOEntity>();
			info.msg.ioEntity.genericEntRef1 = currentlyControllingEnt.uid;
		}
		else
		{
			info.msg.computerStation = Pool.Get<ComputerStation>();
			info.msg.computerStation.bookmarks = GenerateControlBookmarkString();
		}
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (!info.fromDisk)
		{
			if (info.msg.ioEntity != null)
			{
				currentlyControllingEnt.uid = info.msg.ioEntity.genericEntRef1;
			}
		}
		else
		{
			if (info.msg.computerStation == null)
			{
				return;
			}
			string[] array = info.msg.computerStation.bookmarks.Split(new char[1] { ';' });
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[1] { ':' });
				if (array2.Length >= 2)
				{
					string text = array2[0];
					uint.TryParse(array2[1], out var result);
					if (IsValidIdentifier(text))
					{
						controlBookmarks.Add(text, result);
					}
					continue;
				}
				break;
			}
		}
	}
}
