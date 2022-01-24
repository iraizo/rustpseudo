using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

public class FlameThrower : AttackEntity
{
	[Header("Flame Thrower")]
	public int maxAmmo = 100;

	public int ammo = 100;

	public ItemDefinition fuelType;

	public float timeSinceLastAttack;

	[FormerlySerializedAs("nextAttackTime")]
	public float nextReadyTime;

	public float flameRange = 10f;

	public float flameRadius = 2.5f;

	public ParticleSystem[] flameEffects;

	public FlameJet jet;

	public GameObjectRef fireballPrefab;

	public List<DamageTypeEntry> damagePerSec;

	public SoundDefinition flameStart3P;

	public SoundDefinition flameLoop3P;

	public SoundDefinition flameStop3P;

	public SoundDefinition pilotLoopSoundDef;

	private float tickRate = 0.25f;

	private float lastFlameTick;

	public float fuelPerSec;

	private float ammoRemainder;

	public float reloadDuration = 3.5f;

	private float lastReloadTime = -10f;

	private float nextFlameTime;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("FlameThrower.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 3381353917u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - DoReload "));
				}
				TimeWarning val2 = TimeWarning.New("DoReload", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(3381353917u, "DoReload", this, player))
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
							DoReload(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in DoReload");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3749570935u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SetFiring "));
				}
				TimeWarning val2 = TimeWarning.New("SetFiring", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(3749570935u, "SetFiring", this, player))
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
							RPCMessage firing = rPCMessage;
							SetFiring(firing);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in SetFiring");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1057268396 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - TogglePilotLight "));
				}
				TimeWarning val2 = TimeWarning.New("TogglePilotLight", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(1057268396u, "TogglePilotLight", this, player))
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
							TogglePilotLight(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in TogglePilotLight");
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

	private bool IsWeaponBusy()
	{
		return Time.get_realtimeSinceStartup() < nextReadyTime;
	}

	private void SetBusyFor(float dur)
	{
		nextReadyTime = Time.get_realtimeSinceStartup() + dur;
	}

	private void ClearBusy()
	{
		nextReadyTime = Time.get_realtimeSinceStartup() - 1f;
	}

	public void ReduceAmmo(float firingTime)
	{
		ammoRemainder += fuelPerSec * firingTime;
		if (ammoRemainder >= 1f)
		{
			int num = Mathf.FloorToInt(ammoRemainder);
			ammoRemainder -= num;
			if (ammoRemainder >= 1f)
			{
				num++;
				ammoRemainder -= 1f;
			}
			ammo -= num;
			if (ammo <= 0)
			{
				ammo = 0;
			}
		}
	}

	public void PilotLightToggle_Shared()
	{
		SetFlag(Flags.On, !HasFlag(Flags.On));
		if (base.isServer)
		{
			SendNetworkUpdateImmediate();
		}
	}

	public bool IsPilotOn()
	{
		return HasFlag(Flags.On);
	}

	public bool IsFlameOn()
	{
		return HasFlag(Flags.OnFire);
	}

	public bool HasAmmo()
	{
		return GetAmmo() != null;
	}

	public Item GetAmmo()
	{
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if (!Object.op_Implicit((Object)(object)ownerPlayer))
		{
			return null;
		}
		Item item = ownerPlayer.inventory.containerMain.FindItemsByItemName(fuelType.shortname);
		if (item == null)
		{
			item = ownerPlayer.inventory.containerBelt.FindItemsByItemName(fuelType.shortname);
		}
		return item;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.baseProjectile != null && info.msg.baseProjectile.primaryMagazine != null)
		{
			ammo = info.msg.baseProjectile.primaryMagazine.contents;
		}
	}

	public override void CollectedForCrafting(Item item, BasePlayer crafter)
	{
		ServerCommand(item, "unload_ammo", crafter);
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.baseProjectile = Pool.Get<BaseProjectile>();
		info.msg.baseProjectile.primaryMagazine = Pool.Get<Magazine>();
		info.msg.baseProjectile.primaryMagazine.contents = ammo;
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	public void SetFiring(RPCMessage msg)
	{
		bool flameState = msg.read.Bit();
		SetFlameState(flameState);
	}

	public override void ServerUse()
	{
		if (!IsOnFire())
		{
			SetFlameState(wantsOn: true);
			((FacepunchBehaviour)this).Invoke((Action)StopFlameState, 0.2f);
			base.ServerUse();
		}
	}

	public override void TopUpAmmo()
	{
		ammo = maxAmmo;
	}

	public override float AmmoFraction()
	{
		return (float)ammo / (float)maxAmmo;
	}

	public override bool ServerIsReloading()
	{
		return Time.get_time() < lastReloadTime + reloadDuration;
	}

	public override bool CanReload()
	{
		return ammo < maxAmmo;
	}

	public override void ServerReload()
	{
		if (!ServerIsReloading())
		{
			lastReloadTime = Time.get_time();
			StartAttackCooldown(reloadDuration);
			GetOwnerPlayer().SignalBroadcast(Signal.Reload);
			ammo = maxAmmo;
		}
	}

	public void StopFlameState()
	{
		SetFlameState(wantsOn: false);
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	public void DoReload(RPCMessage msg)
	{
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if (!((Object)(object)ownerPlayer == (Object)null))
		{
			Item item = null;
			while (ammo < maxAmmo && (item = GetAmmo()) != null && item.amount > 0)
			{
				int num = Mathf.Min(maxAmmo - ammo, item.amount);
				ammo += num;
				item.UseItem(num);
			}
			SendNetworkUpdateImmediate();
			ItemManager.DoRemoves();
			ownerPlayer.inventory.ServerUpdate(0f);
		}
	}

	public void SetFlameState(bool wantsOn)
	{
		if (wantsOn)
		{
			ammo--;
			if (ammo < 0)
			{
				ammo = 0;
			}
		}
		if (wantsOn && ammo <= 0)
		{
			wantsOn = false;
		}
		SetFlag(Flags.OnFire, wantsOn);
		if (IsFlameOn())
		{
			nextFlameTime = Time.get_realtimeSinceStartup() + 1f;
			lastFlameTick = Time.get_realtimeSinceStartup();
			((FacepunchBehaviour)this).InvokeRepeating((Action)FlameTick, tickRate, tickRate);
		}
		else
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)FlameTick);
		}
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	public void TogglePilotLight(RPCMessage msg)
	{
		PilotLightToggle_Shared();
	}

	public override void OnHeldChanged()
	{
		SetFlameState(wantsOn: false);
		base.OnHeldChanged();
	}

	public void FlameTick()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		float num = Time.get_realtimeSinceStartup() - lastFlameTick;
		lastFlameTick = Time.get_realtimeSinceStartup();
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if (!Object.op_Implicit((Object)(object)ownerPlayer))
		{
			return;
		}
		ReduceAmmo(num);
		SendNetworkUpdate();
		Ray val = ownerPlayer.eyes.BodyRay();
		Vector3 origin = ((Ray)(ref val)).get_origin();
		RaycastHit val2 = default(RaycastHit);
		bool num2 = Physics.SphereCast(val, 0.3f, ref val2, flameRange, 1218652417);
		if (!num2)
		{
			((RaycastHit)(ref val2)).set_point(origin + ((Ray)(ref val)).get_direction() * flameRange);
		}
		float num3 = (ownerPlayer.IsNpc ? npcDamageScale : 1f);
		float amount = damagePerSec[0].amount;
		damagePerSec[0].amount = amount * num * num3;
		DamageUtil.RadiusDamage(ownerPlayer, LookupPrefab(), ((RaycastHit)(ref val2)).get_point() - ((Ray)(ref val)).get_direction() * 0.1f, flameRadius * 0.5f, flameRadius, damagePerSec, 2279681, useLineOfSight: true);
		damagePerSec[0].amount = amount;
		if (num2 && Time.get_realtimeSinceStartup() >= nextFlameTime && ((RaycastHit)(ref val2)).get_distance() > 1.1f)
		{
			nextFlameTime = Time.get_realtimeSinceStartup() + 0.45f;
			Vector3 point = ((RaycastHit)(ref val2)).get_point();
			BaseEntity baseEntity = GameManager.server.CreateEntity(fireballPrefab.resourcePath, point - ((Ray)(ref val)).get_direction() * 0.25f);
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				baseEntity.creatorEntity = ownerPlayer;
				baseEntity.Spawn();
			}
		}
		if (ammo == 0)
		{
			SetFlameState(wantsOn: false);
		}
		GetOwnerItem()?.LoseCondition(num);
	}

	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		if (item == null || !(command == "unload_ammo"))
		{
			return;
		}
		int num = ammo;
		if (num > 0)
		{
			ammo = 0;
			SendNetworkUpdateImmediate();
			Item item2 = ItemManager.Create(fuelType, num, 0uL);
			if (!item2.MoveToContainer(player.inventory.containerMain))
			{
				item2.Drop(player.eyes.position, player.eyes.BodyForward() * 2f);
			}
		}
	}
}
