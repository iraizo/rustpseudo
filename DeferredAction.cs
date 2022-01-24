using System;
using UnityEngine;

public class DeferredAction
{
	private Object sender;

	private Action action;

	private ActionPriority priority = ActionPriority.Medium;

	public bool Idle { get; private set; }

	public int Index => (int)priority;

	public DeferredAction(Object sender, Action action, ActionPriority priority = ActionPriority.Medium)
	{
		this.sender = sender;
		this.action = action;
		this.priority = priority;
		Idle = true;
	}

	public void Action()
	{
		if (Idle)
		{
			throw new Exception("Double invocation of a deferred action.");
		}
		Idle = true;
		if (Object.op_Implicit(sender))
		{
			action();
		}
	}

	public void Invoke()
	{
		if (!Idle)
		{
			throw new Exception("Double invocation of a deferred action.");
		}
		LoadBalancer.Enqueue(this);
		Idle = false;
	}

	public static implicit operator bool(DeferredAction obj)
	{
		return obj != null;
	}

	public static void Invoke(Object sender, Action action, ActionPriority priority = ActionPriority.Medium)
	{
		new DeferredAction(sender, action, priority).Invoke();
	}
}
