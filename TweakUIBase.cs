using System;
using UnityEngine;

public class TweakUIBase : MonoBehaviour
{
	public string convarName = "effects.motionblur";

	public bool ApplyImmediatelyOnChange = true;

	internal Command conVar;

	private void Awake()
	{
		Init();
	}

	protected virtual void Init()
	{
		conVar = Client.Find(convarName);
		if (conVar == null)
		{
			Debug.LogWarning((object)("TweakUI Convar Missing: " + convarName), (Object)(object)((Component)this).get_gameObject());
		}
		else
		{
			conVar.add_OnValueChanged((Action<Command>)OnConVarChanged);
		}
	}

	public virtual void OnApplyClicked()
	{
		if (!ApplyImmediatelyOnChange)
		{
			SetConvarValue();
		}
	}

	public virtual void UnapplyChanges()
	{
		if (!ApplyImmediatelyOnChange)
		{
			ResetToConvar();
		}
	}

	protected virtual void OnConVarChanged(Command obj)
	{
		ResetToConvar();
	}

	public virtual void ResetToConvar()
	{
	}

	protected virtual void SetConvarValue()
	{
	}

	private void OnDestroy()
	{
		if (conVar != null)
		{
			conVar.remove_OnValueChanged((Action<Command>)OnConVarChanged);
		}
	}

	public TweakUIBase()
		: this()
	{
	}
}
