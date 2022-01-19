using System;
using System.IO;
using Network;
using Network.Visibility;
using UnityEngine;

public static class EffectNetwork
{
	public static void Send(Effect effect)
	{
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		if (Net.sv == null || !Net.sv.IsConnected())
		{
			return;
		}
		TimeWarning val = TimeWarning.New("EffectNetwork.Send", 0);
		try
		{
			Group val2 = null;
			if (!string.IsNullOrEmpty(effect.pooledString))
			{
				((EffectData)effect).pooledstringid = StringPool.Get(effect.pooledString);
			}
			if (((EffectData)effect).pooledstringid == 0)
			{
				Debug.Log((object)("String ID is 0 - unknown effect " + effect.pooledString));
				return;
			}
			if (effect.broadcast)
			{
				if (((BaseNetwork)Net.sv).get_write().Start())
				{
					((BaseNetwork)Net.sv).get_write().PacketID((Type)13);
					((EffectData)effect).WriteToStream((Stream)(object)((BaseNetwork)Net.sv).get_write());
					((BaseNetwork)Net.sv).get_write().Send(new SendInfo(BaseNetworkable.GlobalNetworkGroup.subscribers));
				}
				return;
			}
			if (((EffectData)effect).entity != 0)
			{
				BaseEntity baseEntity = BaseNetworkable.serverEntities.Find(((EffectData)effect).entity) as BaseEntity;
				if (!baseEntity.IsValid())
				{
					return;
				}
				val2 = baseEntity.net.group;
				goto IL_0113;
			}
			val2 = Net.sv.visibility.GetGroup(effect.worldPos);
			goto IL_0113;
			IL_0113:
			if (val2 != null)
			{
				((BaseNetwork)Net.sv).get_write().Start();
				((BaseNetwork)Net.sv).get_write().PacketID((Type)13);
				((EffectData)effect).WriteToStream((Stream)(object)((BaseNetwork)Net.sv).get_write());
				((BaseNetwork)Net.sv).get_write().Send(new SendInfo(val2.subscribers));
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static void Send(Effect effect, Connection target)
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		((EffectData)effect).pooledstringid = StringPool.Get(effect.pooledString);
		if (((EffectData)effect).pooledstringid == 0)
		{
			Debug.LogWarning((object)("EffectNetwork.Send - unpooled effect name: " + effect.pooledString));
			return;
		}
		((BaseNetwork)Net.sv).get_write().Start();
		((BaseNetwork)Net.sv).get_write().PacketID((Type)13);
		((EffectData)effect).WriteToStream((Stream)(object)((BaseNetwork)Net.sv).get_write());
		((BaseNetwork)Net.sv).get_write().Send(new SendInfo(target));
	}
}
