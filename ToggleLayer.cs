using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToggleLayer : MonoBehaviour, IClientComponent
{
	public Toggle toggleControl;

	public TextMeshProUGUI textControl;

	public LayerSelect layer;

	protected void OnEnable()
	{
		if (Object.op_Implicit((Object)(object)MainCamera.mainCamera))
		{
			toggleControl.set_isOn((MainCamera.mainCamera.get_cullingMask() & layer.Mask) != 0);
		}
	}

	public void OnToggleChanged()
	{
		if (Object.op_Implicit((Object)(object)MainCamera.mainCamera))
		{
			if (toggleControl.get_isOn())
			{
				Camera mainCamera = MainCamera.mainCamera;
				mainCamera.set_cullingMask(mainCamera.get_cullingMask() | layer.Mask);
			}
			else
			{
				Camera mainCamera2 = MainCamera.mainCamera;
				mainCamera2.set_cullingMask(mainCamera2.get_cullingMask() & ~layer.Mask);
			}
		}
	}

	protected void OnValidate()
	{
		if (Object.op_Implicit((Object)(object)textControl))
		{
			((TMP_Text)textControl).set_text(layer.Name);
		}
	}

	public ToggleLayer()
		: this()
	{
	}
}
