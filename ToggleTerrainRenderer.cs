using UnityEngine;
using UnityEngine.UI;

public class ToggleTerrainRenderer : MonoBehaviour
{
	public Toggle toggleControl;

	public Text textControl;

	protected void OnEnable()
	{
		if (Object.op_Implicit((Object)(object)Terrain.get_activeTerrain()))
		{
			toggleControl.set_isOn(Terrain.get_activeTerrain().get_drawHeightmap());
		}
	}

	public void OnToggleChanged()
	{
		if (Object.op_Implicit((Object)(object)Terrain.get_activeTerrain()))
		{
			Terrain.get_activeTerrain().set_drawHeightmap(toggleControl.get_isOn());
		}
	}

	protected void OnValidate()
	{
		if (Object.op_Implicit((Object)(object)textControl))
		{
			textControl.set_text("Terrain Renderer");
		}
	}

	public ToggleTerrainRenderer()
		: this()
	{
	}
}
