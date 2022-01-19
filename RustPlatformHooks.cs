using System;
using System.Net;
using ConVar;
using Network;
using Rust;
using Rust.Platform.Common;

public class RustPlatformHooks : IPlatformHooks
{
	public static readonly RustPlatformHooks Instance = new RustPlatformHooks();

	public uint SteamAppId => Defines.appID;

	public ServerParameters? ServerParameters
	{
		get
		{
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			if (Net.sv == null)
			{
				return null;
			}
			IPAddress iPAddress = null;
			if (!string.IsNullOrEmpty(ConVar.Server.ip))
			{
				iPAddress = IPAddress.Parse(ConVar.Server.ip);
			}
			bool flag = !Net.sv.get_AllowPassthroughMessages() || (ConVar.Server.queryport > 0 && ConVar.Server.queryport != ConVar.Server.port);
			if (flag && (ConVar.Server.queryport <= 0 || ConVar.Server.queryport == ConVar.Server.port))
			{
				throw new Exception("Query port isn't set up properly");
			}
			return new ServerParameters("rust", "Rust", 2326.ToString(), ConVar.Server.secure, iPAddress, (ushort)Net.sv.port, (ushort)(flag ? ((ushort)ConVar.Server.queryport) : 0));
		}
	}

	public void Abort()
	{
		Application.Quit();
	}

	public void OnItemDefinitionsChanged()
	{
		ItemManager.InvalidateWorkshopSkinCache();
	}

	public void AuthSessionValidated(ulong userId, ulong ownerUserId, AuthResponse response)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		SingletonComponent<ServerMgr>.Instance.OnValidateAuthTicketResponse(userId, ownerUserId, response);
	}
}
