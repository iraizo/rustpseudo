using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TweakUIMultiSelect : TweakUIBase
{
	public ToggleGroup toggleGroup;

	protected override void Init()
	{
		base.Init();
		UpdateToggleGroup();
	}

	protected void OnEnable()
	{
		UpdateToggleGroup();
	}

	public void OnChanged()
	{
		UpdateConVar();
	}

	private void UpdateToggleGroup()
	{
		if (conVar != null)
		{
			string @string = conVar.get_String();
			Toggle[] componentsInChildren = ((Component)toggleGroup).GetComponentsInChildren<Toggle>();
			foreach (Toggle obj in componentsInChildren)
			{
				obj.set_isOn(((Object)obj).get_name() == @string);
			}
		}
	}

	private void UpdateConVar()
	{
		if (conVar != null)
		{
			Toggle val = (from x in ((Component)toggleGroup).GetComponentsInChildren<Toggle>()
				where x.get_isOn()
				select x).FirstOrDefault();
			if (!((Object)(object)val == (Object)null) && !(conVar.get_String() == ((Object)val).get_name()))
			{
				conVar.Set(((Object)val).get_name());
			}
		}
	}
}
