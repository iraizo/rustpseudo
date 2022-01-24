using System;
using Network;
using Rust;
using UnityEngine;

public class PetBrain : BaseAIBrain<BasePet>
{
	[Header("Audio")]
	public SoundDefinition CommandGivenVocalSFX;

	[ServerVar]
	public static bool DrownInDeepWater = true;

	[ServerVar]
	public static bool IdleWhenOwnerOfflineOrDead = true;

	[ServerVar]
	public static bool IdleWhenOwnerMounted = true;

	[ServerVar]
	public static float DrownTimer = 15f;

	[ReplicatedVar]
	public static float ControlDistance = 100f;

	public static int Count;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("PetBrain.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override void AddStates()
	{
		base.AddStates();
	}

	public override void InitializeAI()
	{
		base.InitializeAI();
		base.ThinkMode = AIThinkMode.Interval;
		thinkRate = 0.25f;
		base.PathFinder = new HumanPathFinder();
		((HumanPathFinder)base.PathFinder).Init(GetEntity());
		Count++;
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		Count--;
	}

	public override void Think(float delta)
	{
		base.Think(delta);
		if (DrownInDeepWater)
		{
			BasePet entity = GetEntity();
			if ((Object)(object)entity != (Object)null && entity.WaterFactor() > 0.85f && !entity.IsDestroyed)
			{
				entity.Hurt(delta * (entity.MaxHealth() / DrownTimer), DamageType.Drowned);
			}
		}
		EvaluateLoadDefaultDesignTriggers();
	}

	private bool EvaluateLoadDefaultDesignTriggers()
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		if (loadedDesignIndex == 0)
		{
			return true;
		}
		bool flag = false;
		if (IdleWhenOwnerOfflineOrDead)
		{
			flag = (IdleWhenOwnerOfflineOrDead && (Object)(object)base.OwningPlayer == (Object)null) || base.OwningPlayer.IsSleeping() || base.OwningPlayer.IsDead();
		}
		if (IdleWhenOwnerMounted && !flag)
		{
			flag = (Object)(object)base.OwningPlayer != (Object)null && base.OwningPlayer.isMounted;
		}
		if ((Object)(object)base.OwningPlayer != (Object)null && Vector3.Distance(((Component)this).get_transform().get_position(), ((Component)base.OwningPlayer).get_transform().get_position()) > ControlDistance)
		{
			flag = true;
		}
		if (flag)
		{
			LoadDefaultAIDesign();
			return true;
		}
		return false;
	}

	public override void OnAIDesignLoadedAtIndex(int index)
	{
		base.OnAIDesignLoadedAtIndex(index);
		BasePet entity = GetEntity();
		if ((Object)(object)entity != (Object)null)
		{
			BasePlayer basePlayer = BasePlayer.FindByID(entity.OwnerID);
			if ((Object)(object)basePlayer != (Object)null)
			{
				basePlayer.SendClientPetStateIndex();
			}
			entity.ClientRPC(null, "OnCommandGiven");
		}
	}
}
