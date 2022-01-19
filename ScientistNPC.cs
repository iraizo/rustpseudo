using System;
using UnityEngine;

public class ScientistNPC : HumanNPC, IAIMounted
{
	public enum RadioChatterType
	{
		NONE,
		Idle,
		Alert
	}

	public GameObjectRef[] RadioChatterEffects;

	public GameObjectRef[] DeathEffects;

	public string deathStatName = "kill_scientist";

	public Vector2 IdleChatterRepeatRange = new Vector2(10f, 15f);

	public RadioChatterType radioChatterType;

	protected float lastAlertedTime = -100f;

	public void SetChatterType(RadioChatterType newType)
	{
		if (newType != radioChatterType)
		{
			if (newType == RadioChatterType.Idle)
			{
				QueueRadioChatter();
			}
			else
			{
				((FacepunchBehaviour)this).CancelInvoke((Action)PlayRadioChatter);
			}
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		SetChatterType(RadioChatterType.Idle);
		((FacepunchBehaviour)this).InvokeRandomized((Action)IdleCheck, 0f, 20f, 1f);
	}

	public void IdleCheck()
	{
		if (Time.get_time() > lastAlertedTime + 20f)
		{
			SetChatterType(RadioChatterType.Idle);
		}
	}

	public void QueueRadioChatter()
	{
		if (IsAlive() && !base.IsDestroyed)
		{
			((FacepunchBehaviour)this).Invoke((Action)PlayRadioChatter, Random.Range(IdleChatterRepeatRange.x, IdleChatterRepeatRange.y));
		}
	}

	public override bool ShotTest(float targetDist)
	{
		bool result = base.ShotTest(targetDist);
		if (Time.get_time() - lastGunShotTime < 5f)
		{
			Alert();
		}
		return result;
	}

	public void Alert()
	{
		lastAlertedTime = Time.get_time();
		SetChatterType(RadioChatterType.Alert);
	}

	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		Alert();
	}

	public override void OnKilled(HitInfo info)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		base.OnKilled(info);
		SetChatterType(RadioChatterType.NONE);
		if (DeathEffects.Length != 0)
		{
			Effect.server.Run(DeathEffects[Random.Range(0, DeathEffects.Length)].resourcePath, ServerPosition, Vector3.get_up());
		}
		if (info != null && (Object)(object)info.InitiatorPlayer != (Object)null && !info.InitiatorPlayer.IsNpc)
		{
			info.InitiatorPlayer.stats.Add(deathStatName, 1, (Stats)5);
		}
	}

	public void PlayRadioChatter()
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if (RadioChatterEffects.Length != 0)
		{
			if (base.IsDestroyed || (Object)(object)((Component)this).get_transform() == (Object)null)
			{
				((FacepunchBehaviour)this).CancelInvoke((Action)PlayRadioChatter);
				return;
			}
			Effect.server.Run(RadioChatterEffects[Random.Range(0, RadioChatterEffects.Length)].resourcePath, this, StringPool.Get("head"), Vector3.get_zero(), Vector3.get_zero());
			QueueRadioChatter();
		}
	}

	public override void EquipWeapon()
	{
		base.EquipWeapon();
		HeldEntity heldEntity = GetHeldEntity();
		if (!((Object)(object)heldEntity != (Object)null))
		{
			return;
		}
		Item item = heldEntity.GetItem();
		if (item == null || item.contents == null)
		{
			return;
		}
		if (Random.Range(0, 3) == 0)
		{
			Item item2 = ItemManager.CreateByName("weapon.mod.flashlight", 1, 0uL);
			if (!item2.MoveToContainer(item.contents))
			{
				item2.Remove();
				return;
			}
			lightsOn = false;
			((FacepunchBehaviour)this).InvokeRandomized((Action)base.LightCheck, 0f, 30f, 5f);
			LightCheck();
		}
		else
		{
			Item item3 = ItemManager.CreateByName("weapon.mod.lasersight", 1, 0uL);
			if (!item3.MoveToContainer(item.contents))
			{
				item3.Remove();
			}
			LightToggle();
			lightsOn = true;
		}
	}

	public bool IsMounted()
	{
		return base.isMounted;
	}

	protected override string OverrideCorpseName()
	{
		return "Scientist";
	}
}
