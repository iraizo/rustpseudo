using System;
using Facepunch;
using Network;
using ProtoBuf;

public class PlayerModifiers : BaseModifiers<BasePlayer>
{
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("PlayerModifiers.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override void ServerUpdate(BaseCombatEntity ownerEntity)
	{
		base.ServerUpdate(ownerEntity);
		SendChangesToClient();
	}

	public PlayerModifiers Save()
	{
		PlayerModifiers val = Pool.Get<PlayerModifiers>();
		val.modifiers = Pool.GetList<Modifier>();
		foreach (Modifier item in All)
		{
			if (item != null)
			{
				val.modifiers.Add(item.Save());
			}
		}
		return val;
	}

	public void Load(PlayerModifiers m)
	{
		RemoveAll();
		if (m == null || m.modifiers == null)
		{
			return;
		}
		foreach (Modifier modifier2 in m.modifiers)
		{
			if (modifier2 != null)
			{
				Modifier modifier = new Modifier();
				modifier.Init((Modifier.ModifierType)modifier2.type, (Modifier.ModifierSource)modifier2.source, modifier2.value, modifier2.duration, modifier2.timeRemaing);
				Add(modifier);
			}
		}
	}

	public void SendChangesToClient()
	{
		if (dirty)
		{
			SetDirty(flag: false);
			PlayerModifiers val = Save();
			try
			{
				base.baseEntity.ClientRPCPlayer<PlayerModifiers>(null, base.baseEntity, "UpdateModifiers", val);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
	}
}
