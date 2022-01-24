using UnityEngine;
using UnityEngine.UI;

public class ToggleTerrainTrees : MonoBehaviour
{
	public Toggle toggleControl;

	public Text textControl;

	protected void OnEnable()
	{
		if (Object.op_Implicit((Object)(object)Terrain.get_activeTerrain()))
		{
			toggleControl.set_isOn(Terrain.get_activeTerrain().get_drawTreesAndFoliage());
		}
	}

	public void OnToggleChanged()
	{
		if (Object.op_Implicit((Object)(object)Terrain.get_activeTerrain()))
		{
			Terrain.get_activeTerrain().set_drawTreesAndFoliage(toggleControl.get_isOn());
		}
	}

	protected void OnValidate()
	{
		if (Object.op_Implicit((Object)(object)textControl))
		{
			textControl.set_text("Terrain Trees");
		}
	}

	public ToggleTerrainTrees()
		: this()
	{
	}
}
