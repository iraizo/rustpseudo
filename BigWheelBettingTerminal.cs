using System;
using Network;
using UnityEngine;

public class BigWheelBettingTerminal : StorageContainer
{
	public BigWheelGame bigWheel;

	public Vector3 seatedPlayerOffset = Vector3.get_forward();

	public float offsetCheckRadius = 0.4f;

	public SoundDefinition winSound;

	public SoundDefinition loseSound;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("BigWheelBettingTerminal.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public new void OnDrawGizmos()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.set_color(Color.get_yellow());
		Gizmos.DrawSphere(((Component)this).get_transform().TransformPoint(seatedPlayerOffset), offsetCheckRadius);
		base.OnDrawGizmos();
	}

	public bool IsPlayerValid(BasePlayer player)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (!player.isMounted || !(player.GetMounted() is BaseChair))
		{
			return false;
		}
		Vector3 val = ((Component)this).get_transform().TransformPoint(seatedPlayerOffset);
		if (Vector3Ex.Distance2D(((Component)player).get_transform().get_position(), val) > offsetCheckRadius)
		{
			return false;
		}
		return true;
	}

	public override bool PlayerOpenLoot(BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		if (!IsPlayerValid(player))
		{
			return false;
		}
		return base.PlayerOpenLoot(player, panelToOpen);
	}
}
