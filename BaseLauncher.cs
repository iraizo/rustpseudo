using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class BaseLauncher : BaseProjectile
{
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("BaseLauncher.OnRpcMessage", 0);
		try
		{
			if (rpc == 853319324 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SV_Launch "));
				}
				TimeWarning val2 = TimeWarning.New("SV_Launch", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(853319324u, "SV_Launch", this, player))
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
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							RPCMessage msg2 = rPCMessage;
							SV_Launch(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SV_Launch");
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

	public override bool ForceSendMagazine(SaveInfo saveInfo)
	{
		return true;
	}

	public override void ServerUse()
	{
		ServerUse(1f);
	}

	public override void ServerUse(float damageModifier, Transform originOverride = null)
	{
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		ItemModProjectile component = ((Component)primaryMagazine.ammoType).GetComponent<ItemModProjectile>();
		if (!Object.op_Implicit((Object)(object)component))
		{
			return;
		}
		if (primaryMagazine.contents <= 0)
		{
			SignalBroadcast(Signal.DryFire);
			StartAttackCooldown(1f);
			return;
		}
		if (!Object.op_Implicit((Object)(object)component.projectileObject.Get().GetComponent<ServerProjectile>()))
		{
			base.ServerUse(damageModifier, originOverride);
			return;
		}
		primaryMagazine.contents--;
		if (primaryMagazine.contents < 0)
		{
			primaryMagazine.contents = 0;
		}
		Vector3 val = ((Component)MuzzlePoint).get_transform().get_forward();
		Vector3 position = ((Component)MuzzlePoint).get_transform().get_position();
		float num = GetAimCone() + component.projectileSpread;
		if (num > 0f)
		{
			val = AimConeUtil.GetModifiedAimConeDirection(num, val);
		}
		float num2 = 1f;
		RaycastHit val2 = default(RaycastHit);
		if (Physics.Raycast(position, val, ref val2, num2, 1236478737))
		{
			num2 = ((RaycastHit)(ref val2)).get_distance() - 0.1f;
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(component.projectileObject.resourcePath, position + val * num2);
		if (!((Object)(object)baseEntity == (Object)null))
		{
			BasePlayer ownerPlayer = GetOwnerPlayer();
			bool flag = (Object)(object)ownerPlayer != (Object)null && ownerPlayer.IsNpc;
			ServerProjectile component2 = ((Component)baseEntity).GetComponent<ServerProjectile>();
			if (Object.op_Implicit((Object)(object)component2))
			{
				component2.InitializeVelocity(val * component2.speed);
			}
			((Component)baseEntity).SendMessage("SetDamageScale", (object)(flag ? npcDamageScale : turretDamageScale));
			baseEntity.Spawn();
			StartAttackCooldown(ScaleRepeatDelay(repeatDelay));
			SignalBroadcast(Signal.Attack, string.Empty);
			GetOwnerItem()?.LoseCondition(Random.Range(1f, 2f));
		}
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	private void SV_Launch(RPCMessage msg)
	{
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer player = msg.player;
		if (!VerifyClientAttack(player))
		{
			SendNetworkUpdate();
			return;
		}
		if (reloadFinished && HasReloadCooldown())
		{
			AntiHack.Log(player, AntiHackType.ProjectileHack, "Reloading (" + base.ShortPrefabName + ")");
			player.stats.combat.Log(this, "reload_cooldown");
			return;
		}
		reloadStarted = false;
		reloadFinished = false;
		if (primaryMagazine.contents <= 0)
		{
			AntiHack.Log(player, AntiHackType.ProjectileHack, "Magazine empty (" + base.ShortPrefabName + ")");
			player.stats.combat.Log(this, "magazine_empty");
			return;
		}
		primaryMagazine.contents--;
		SignalBroadcast(Signal.Attack, string.Empty, player.net.get_connection());
		Vector3 val = msg.read.Vector3();
		Vector3 val2 = msg.read.Vector3();
		Vector3 val3 = ((Vector3)(ref val2)).get_normalized();
		bool num = msg.read.Bit();
		BaseEntity mounted = player.GetParentEntity();
		if ((Object)(object)mounted == (Object)null)
		{
			mounted = player.GetMounted();
		}
		if (num)
		{
			if ((Object)(object)mounted != (Object)null)
			{
				val = ((Component)mounted).get_transform().TransformPoint(val);
				val3 = ((Component)mounted).get_transform().TransformDirection(val3);
			}
			else
			{
				val = player.eyes.position;
				val3 = player.eyes.BodyForward();
			}
		}
		if (!ValidateEyePos(player, val))
		{
			return;
		}
		ItemModProjectile component = ((Component)primaryMagazine.ammoType).GetComponent<ItemModProjectile>();
		if (!Object.op_Implicit((Object)(object)component))
		{
			AntiHack.Log(player, AntiHackType.ProjectileHack, "Item mod not found (" + base.ShortPrefabName + ")");
			player.stats.combat.Log(this, "mod_missing");
			return;
		}
		float num2 = GetAimCone() + component.projectileSpread;
		if (num2 > 0f)
		{
			val3 = AimConeUtil.GetModifiedAimConeDirection(num2, val3);
		}
		float num3 = 1f;
		RaycastHit val4 = default(RaycastHit);
		if (Physics.Raycast(val, val3, ref val4, num3, 1236478737))
		{
			num3 = ((RaycastHit)(ref val4)).get_distance() - 0.1f;
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(component.projectileObject.resourcePath, val + val3 * num3);
		if (!((Object)(object)baseEntity == (Object)null))
		{
			baseEntity.creatorEntity = player;
			ServerProjectile component2 = ((Component)baseEntity).GetComponent<ServerProjectile>();
			if (Object.op_Implicit((Object)(object)component2))
			{
				component2.InitializeVelocity(GetInheritedVelocity(player) + val3 * component2.speed);
			}
			baseEntity.Spawn();
			StartAttackCooldown(ScaleRepeatDelay(repeatDelay));
			GetOwnerItem()?.LoseCondition(Random.Range(1f, 2f));
		}
	}
}
