using System;
using Network;
using UnityEngine;

public class DoorKnocker : BaseCombatEntity
{
	public Animator knocker1;

	public Animator knocker2;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("DoorKnocker.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public void Knock(BasePlayer player)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		ClientRPC<Vector3>(null, "ClientKnock", ((Component)player).get_transform().get_position());
	}
}
