using ConVar;
using UnityEngine;

public abstract class BaseMonoBehaviour : FacepunchBehaviour
{
	public enum LogEntryType
	{
		General,
		Network,
		Hierarchy,
		Serialization
	}

	public virtual bool IsDebugging()
	{
		return false;
	}

	public virtual string GetLogColor()
	{
		return "yellow";
	}

	public void LogEntry(LogEntryType log, int level, string str, object arg1)
	{
		if (IsDebugging() || Global.developer >= level)
		{
			string text = string.Format(str, arg1);
			Debug.Log((object)string.Format("<color=white>{0}</color>[<color={3}>{1}</color>] {2}", log.ToString().PadRight(10), ((object)this).ToString(), text, GetLogColor()), (Object)(object)((Component)this).get_gameObject());
		}
	}

	public void LogEntry(LogEntryType log, int level, string str, object arg1, object arg2)
	{
		if (IsDebugging() || Global.developer >= level)
		{
			string text = string.Format(str, arg1, arg2);
			Debug.Log((object)string.Format("<color=white>{0}</color>[<color={3}>{1}</color>] {2}", log.ToString().PadRight(10), ((object)this).ToString(), text, GetLogColor()), (Object)(object)((Component)this).get_gameObject());
		}
	}

	public void LogEntry(LogEntryType log, int level, string str)
	{
		if (IsDebugging() || Global.developer >= level)
		{
			Debug.Log((object)string.Format("<color=white>{0}</color>[<color={3}>{1}</color>] {2}", log.ToString().PadRight(10), ((object)this).ToString(), str, GetLogColor()), (Object)(object)((Component)this).get_gameObject());
		}
	}

	protected BaseMonoBehaviour()
		: this()
	{
	}
}
