using UnityEngine.UI;

public class TweakUIToggle : TweakUIBase
{
	public Toggle toggleControl;

	public bool inverse;

	public static string lastConVarChanged;

	public static TimeSince timeSinceLastConVarChange;

	protected override void Init()
	{
		base.Init();
		ResetToConvar();
	}

	protected void OnEnable()
	{
		ResetToConvar();
	}

	public void OnToggleChanged()
	{
		if (ApplyImmediatelyOnChange)
		{
			SetConvarValue();
		}
	}

	protected override void SetConvarValue()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		base.SetConvarValue();
		if (conVar != null)
		{
			bool flag = toggleControl.get_isOn();
			if (inverse)
			{
				flag = !flag;
			}
			if (conVar.get_AsBool() != flag)
			{
				lastConVarChanged = conVar.FullName;
				timeSinceLastConVarChange = TimeSince.op_Implicit(0f);
				conVar.Set(flag);
			}
		}
	}

	public override void ResetToConvar()
	{
		base.ResetToConvar();
		if (conVar != null)
		{
			bool flag = conVar.get_AsBool();
			if (inverse)
			{
				flag = !flag;
			}
			toggleControl.set_isOn(flag);
		}
	}
}
