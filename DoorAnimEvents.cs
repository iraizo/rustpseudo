using Rust;
using UnityEngine;

public class DoorAnimEvents : MonoBehaviour, IClientComponent
{
	public GameObjectRef openStart;

	public GameObjectRef openEnd;

	public GameObjectRef closeStart;

	public GameObjectRef closeEnd;

	public bool checkAnimSpeed;

	public Animator animator => ((Component)this).GetComponent<Animator>();

	private void DoorOpenStart()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (Application.isLoading || !openStart.isValid || animator.IsInTransition(0))
		{
			return;
		}
		AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
		if (((AnimatorStateInfo)(ref currentAnimatorStateInfo)).get_normalizedTime() > 0.5f)
		{
			return;
		}
		if (checkAnimSpeed)
		{
			currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
			if (((AnimatorStateInfo)(ref currentAnimatorStateInfo)).get_speed() < 0f)
			{
				return;
			}
		}
		Effect.client.Run(openStart.resourcePath, ((Component)this).get_gameObject());
	}

	private void DoorOpenEnd()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (Application.isLoading || !openEnd.isValid || animator.IsInTransition(0))
		{
			return;
		}
		AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
		if (((AnimatorStateInfo)(ref currentAnimatorStateInfo)).get_normalizedTime() < 0.5f)
		{
			return;
		}
		if (checkAnimSpeed)
		{
			currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
			if (((AnimatorStateInfo)(ref currentAnimatorStateInfo)).get_speed() < 0f)
			{
				return;
			}
		}
		Effect.client.Run(openEnd.resourcePath, ((Component)this).get_gameObject());
	}

	private void DoorCloseStart()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (Application.isLoading || !closeStart.isValid || animator.IsInTransition(0))
		{
			return;
		}
		AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
		if (((AnimatorStateInfo)(ref currentAnimatorStateInfo)).get_normalizedTime() > 0.5f)
		{
			return;
		}
		if (checkAnimSpeed)
		{
			currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
			if (((AnimatorStateInfo)(ref currentAnimatorStateInfo)).get_speed() > 0f)
			{
				return;
			}
		}
		Effect.client.Run(closeStart.resourcePath, ((Component)this).get_gameObject());
	}

	private void DoorCloseEnd()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (Application.isLoading || !closeEnd.isValid || animator.IsInTransition(0))
		{
			return;
		}
		AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
		if (((AnimatorStateInfo)(ref currentAnimatorStateInfo)).get_normalizedTime() < 0.5f)
		{
			return;
		}
		if (checkAnimSpeed)
		{
			currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
			if (((AnimatorStateInfo)(ref currentAnimatorStateInfo)).get_speed() > 0f)
			{
				return;
			}
		}
		Effect.client.Run(closeEnd.resourcePath, ((Component)this).get_gameObject());
	}

	public DoorAnimEvents()
		: this()
	{
	}
}
