using UnityEngine;

public class SendMessageToEntityOnAnimationFinish : StateMachineBehaviour
{
	public string messageToSendToEntity;

	public float repeatRate = 0.1f;

	private const float lastMessageSent = 0f;

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (0f + repeatRate > Time.get_time() || animator.IsInTransition(layerIndex) || ((AnimatorStateInfo)(ref stateInfo)).get_normalizedTime() < 1f)
		{
			return;
		}
		for (int i = 0; i < animator.get_layerCount(); i++)
		{
			if (i != layerIndex)
			{
				if (animator.IsInTransition(i))
				{
					return;
				}
				AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(i);
				if (((AnimatorStateInfo)(ref currentAnimatorStateInfo)).get_speed() > 0f && ((AnimatorStateInfo)(ref currentAnimatorStateInfo)).get_normalizedTime() < 1f)
				{
					return;
				}
			}
		}
		BaseEntity baseEntity = ((Component)animator).get_gameObject().ToBaseEntity();
		if (Object.op_Implicit((Object)(object)baseEntity))
		{
			((Component)baseEntity).SendMessage(messageToSendToEntity);
		}
	}

	public SendMessageToEntityOnAnimationFinish()
		: this()
	{
	}
}
