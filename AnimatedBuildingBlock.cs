using Rust;
using UnityEngine;

public class AnimatedBuildingBlock : StabilityEntity
{
	private bool animatorNeedsInitializing = true;

	private bool animatorIsOpen = true;

	private bool isAnimating;

	public override void ServerInit()
	{
		base.ServerInit();
		if (!Application.isLoadingSave)
		{
			UpdateAnimationParameters(init: true);
		}
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		UpdateAnimationParameters(init: true);
	}

	public override void OnFlagsChanged(Flags old, Flags next)
	{
		base.OnFlagsChanged(old, next);
		UpdateAnimationParameters(init: false);
	}

	protected void UpdateAnimationParameters(bool init)
	{
		if (!Object.op_Implicit((Object)(object)model) || !Object.op_Implicit((Object)(object)model.animator) || !model.animator.get_isInitialized())
		{
			return;
		}
		bool num = animatorNeedsInitializing || animatorIsOpen != IsOpen() || (init && isAnimating);
		bool flag = animatorNeedsInitializing || init;
		if (num)
		{
			isAnimating = true;
			((Behaviour)model.animator).set_enabled(true);
			model.animator.SetBool("open", animatorIsOpen = IsOpen());
			if (flag)
			{
				model.animator.set_fireEvents(false);
				if (((Behaviour)model.animator).get_isActiveAndEnabled())
				{
					model.animator.Update(0f);
					model.animator.Update(20f);
				}
				PutAnimatorToSleep();
			}
			else
			{
				model.animator.set_fireEvents(base.isClient);
				if (base.isServer)
				{
					SetFlag(Flags.Busy, b: true);
				}
			}
		}
		else if (flag)
		{
			PutAnimatorToSleep();
		}
		animatorNeedsInitializing = false;
	}

	protected void OnAnimatorFinished()
	{
		if (!isAnimating)
		{
			PutAnimatorToSleep();
		}
		isAnimating = false;
	}

	private void PutAnimatorToSleep()
	{
		if (!Object.op_Implicit((Object)(object)model) || !Object.op_Implicit((Object)(object)model.animator))
		{
			Debug.LogWarning((object)(((Component)this).get_transform().GetRecursiveName() + " has missing model/animator"), (Object)(object)((Component)this).get_gameObject());
			return;
		}
		((Behaviour)model.animator).set_enabled(false);
		if (base.isServer)
		{
			SetFlag(Flags.Busy, b: false);
		}
		OnAnimatorDisabled();
	}

	protected virtual void OnAnimatorDisabled()
	{
	}

	public override bool SupportsChildDeployables()
	{
		return false;
	}
}
