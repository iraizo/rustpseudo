using System;
using ConVar;
using Network;
using Rust;
using Rust.Ai;
using UnityEngine;
using UnityEngine.Assertions;

public class ThrownWeapon : AttackEntity
{
	[Header("Throw Weapon")]
	public GameObjectRef prefabToThrow;

	public float maxThrowVelocity = 10f;

	public float tumbleVelocity;

	public Vector3 overrideAngle = Vector3.get_zero();

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("ThrownWeapon.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 1513023343 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - DoDrop "));
				}
				TimeWarning val2 = TimeWarning.New("DoDrop", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(1513023343u, "DoDrop", this, player))
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
							DoDrop(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in DoDrop");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1974840882 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - DoThrow "));
				}
				TimeWarning val2 = TimeWarning.New("DoThrow", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(1974840882u, "DoThrow", this, player))
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
							DoThrow(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in DoThrow");
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

	public override Vector3 GetInheritedVelocity(BasePlayer player)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return player.GetInheritedThrowVelocity();
	}

	public void ServerThrow(Vector3 targetPosition)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		if (base.isClient || !HasItemAmount() || HasAttackCooldown())
		{
			return;
		}
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if ((Object)(object)ownerPlayer == (Object)null)
		{
			return;
		}
		Vector3 position = ownerPlayer.eyes.position;
		Vector3 val = ownerPlayer.eyes.BodyForward();
		float num = 1f;
		SignalBroadcast(Signal.Throw, string.Empty);
		BaseEntity baseEntity = GameManager.server.CreateEntity(prefabToThrow.resourcePath, position, Quaternion.LookRotation((overrideAngle == Vector3.get_zero()) ? (-val) : overrideAngle));
		if ((Object)(object)baseEntity == (Object)null)
		{
			return;
		}
		baseEntity.creatorEntity = ownerPlayer;
		Vector3 val2 = val + Quaternion.AngleAxis(10f, Vector3.get_right()) * Vector3.get_up();
		float num2 = GetThrowVelocity(position, targetPosition, val2);
		if (float.IsNaN(num2))
		{
			val2 = val + Quaternion.AngleAxis(20f, Vector3.get_right()) * Vector3.get_up();
			num2 = GetThrowVelocity(position, targetPosition, val2);
			if (float.IsNaN(num2))
			{
				num2 = 5f;
			}
		}
		baseEntity.SetVelocity(val2 * num2 * num);
		if (tumbleVelocity > 0f)
		{
			baseEntity.SetAngularVelocity(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * tumbleVelocity);
		}
		baseEntity.Spawn();
		StartAttackCooldown(repeatDelay);
		UseItemAmount(1);
		TimedExplosive timedExplosive = baseEntity as TimedExplosive;
		Sensation sensation;
		if ((Object)(object)timedExplosive != (Object)null)
		{
			float num3 = 0f;
			foreach (DamageTypeEntry damageType in timedExplosive.damageTypes)
			{
				num3 += damageType.amount;
			}
			sensation = default(Sensation);
			sensation.Type = SensationType.ThrownWeapon;
			sensation.Position = ((Component)ownerPlayer).get_transform().get_position();
			sensation.Radius = 50f;
			sensation.DamagePotential = num3;
			sensation.InitiatorPlayer = ownerPlayer;
			sensation.Initiator = ownerPlayer;
			sensation.UsedEntity = timedExplosive;
			Sense.Stimulate(sensation);
		}
		else
		{
			sensation = default(Sensation);
			sensation.Type = SensationType.ThrownWeapon;
			sensation.Position = ((Component)ownerPlayer).get_transform().get_position();
			sensation.Radius = 50f;
			sensation.DamagePotential = 0f;
			sensation.InitiatorPlayer = ownerPlayer;
			sensation.Initiator = ownerPlayer;
			sensation.UsedEntity = this;
			Sense.Stimulate(sensation);
		}
	}

	private float GetThrowVelocity(Vector3 throwPos, Vector3 targetPos, Vector3 aimDir)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = targetPos - throwPos;
		Vector2 val2 = new Vector2(val.x, val.z);
		float magnitude = ((Vector2)(ref val2)).get_magnitude();
		float y = val.y;
		val2 = new Vector2(aimDir.x, aimDir.z);
		float magnitude2 = ((Vector2)(ref val2)).get_magnitude();
		float y2 = aimDir.y;
		float y3 = Physics.get_gravity().y;
		return Mathf.Sqrt(0.5f * y3 * magnitude * magnitude / (magnitude2 * (magnitude2 * y - y2 * magnitude)));
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	private void DoThrow(RPCMessage msg)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		if (!HasItemAmount() || HasAttackCooldown())
		{
			return;
		}
		Vector3 val = msg.read.Vector3();
		Vector3 val2 = msg.read.Vector3();
		Vector3 normalized = ((Vector3)(ref val2)).get_normalized();
		float num = Mathf.Clamp01(msg.read.Float());
		if (msg.player.isMounted || msg.player.HasParent())
		{
			val = msg.player.eyes.position;
		}
		else if (!ValidateEyePos(msg.player, val))
		{
			return;
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(prefabToThrow.resourcePath, val, Quaternion.LookRotation((overrideAngle == Vector3.get_zero()) ? (-normalized) : overrideAngle));
		if ((Object)(object)baseEntity == (Object)null)
		{
			return;
		}
		baseEntity.creatorEntity = msg.player;
		baseEntity.skinID = skinID;
		baseEntity.SetVelocity(GetInheritedVelocity(msg.player) + normalized * maxThrowVelocity * num + msg.player.estimatedVelocity * 0.5f);
		if (tumbleVelocity > 0f)
		{
			baseEntity.SetAngularVelocity(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * tumbleVelocity);
		}
		baseEntity.Spawn();
		SetUpThrownWeapon(baseEntity);
		StartAttackCooldown(repeatDelay);
		UseItemAmount(1);
		BasePlayer player = msg.player;
		if (!((Object)(object)player != (Object)null))
		{
			return;
		}
		TimedExplosive timedExplosive = baseEntity as TimedExplosive;
		Sensation sensation;
		if ((Object)(object)timedExplosive != (Object)null)
		{
			float num2 = 0f;
			foreach (DamageTypeEntry damageType in timedExplosive.damageTypes)
			{
				num2 += damageType.amount;
			}
			sensation = default(Sensation);
			sensation.Type = SensationType.ThrownWeapon;
			sensation.Position = ((Component)player).get_transform().get_position();
			sensation.Radius = 50f;
			sensation.DamagePotential = num2;
			sensation.InitiatorPlayer = player;
			sensation.Initiator = player;
			sensation.UsedEntity = timedExplosive;
			Sense.Stimulate(sensation);
		}
		else
		{
			sensation = default(Sensation);
			sensation.Type = SensationType.ThrownWeapon;
			sensation.Position = ((Component)player).get_transform().get_position();
			sensation.Radius = 50f;
			sensation.DamagePotential = 0f;
			sensation.InitiatorPlayer = player;
			sensation.Initiator = player;
			sensation.UsedEntity = this;
			Sense.Stimulate(sensation);
		}
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	private void DoDrop(RPCMessage msg)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		if (!HasItemAmount() || HasAttackCooldown())
		{
			return;
		}
		Vector3 val = msg.read.Vector3();
		Vector3 val2 = msg.read.Vector3();
		Vector3 normalized = ((Vector3)(ref val2)).get_normalized();
		if (msg.player.isMounted || msg.player.HasParent())
		{
			val = msg.player.eyes.position;
		}
		else if (!ValidateEyePos(msg.player, val))
		{
			return;
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(prefabToThrow.resourcePath, val, Quaternion.LookRotation(Vector3.get_up()));
		if ((Object)(object)baseEntity == (Object)null)
		{
			return;
		}
		RaycastHit hit = default(RaycastHit);
		if (Physics.SphereCast(new Ray(val, normalized), 0.05f, ref hit, 1.5f, 1236478737))
		{
			Vector3 point = ((RaycastHit)(ref hit)).get_point();
			Vector3 normal = ((RaycastHit)(ref hit)).get_normal();
			BaseEntity entity = hit.GetEntity();
			Collider collider = ((RaycastHit)(ref hit)).get_collider();
			if (Object.op_Implicit((Object)(object)entity) && entity is StabilityEntity && baseEntity is TimedExplosive)
			{
				entity = entity.ToServer<BaseEntity>();
				TimedExplosive obj = baseEntity as TimedExplosive;
				obj.onlyDamageParent = true;
				obj.DoStick(point, normal, entity, collider);
			}
			else
			{
				baseEntity.SetVelocity(normalized);
			}
		}
		else
		{
			baseEntity.SetVelocity(normalized);
		}
		baseEntity.creatorEntity = msg.player;
		baseEntity.skinID = skinID;
		baseEntity.Spawn();
		SetUpThrownWeapon(baseEntity);
		StartAttackCooldown(repeatDelay);
		UseItemAmount(1);
	}

	protected virtual void SetUpThrownWeapon(BaseEntity ent)
	{
	}
}
