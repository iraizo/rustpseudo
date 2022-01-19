using System;
using Network;

public class CommunityEntity : PointEntity
{
	public static CommunityEntity ServerInstance;

	public static CommunityEntity ClientInstance;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("CommunityEntity.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override void InitShared()
	{
		if (base.isServer)
		{
			ServerInstance = this;
		}
		else
		{
			ClientInstance = this;
		}
		base.InitShared();
	}

	public override void DestroyShared()
	{
		base.DestroyShared();
		if (base.isServer)
		{
			ServerInstance = null;
		}
		else
		{
			ClientInstance = null;
		}
	}
}
