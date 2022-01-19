using System.Collections.Generic;
using UnityEngine;

public class VehicleLight : MonoBehaviour
{
	public bool IsBrake;

	public GameObject toggleObject;

	public Renderer lightRenderer;

	[Tooltip("Index of the material on the lightRenderer to modify emission on when lights turn on/off")]
	public int lightRendererMaterialIndex;

	[ColorUsage(true, true)]
	public Color lightOnColour;

	[ColorUsage(true, true)]
	public Color brakesOnColour;

	private static MaterialPropertyBlock materialPB;

	private static int emissionColorID = Shader.PropertyToID("_EmissionColor");

	public static void SetLightVisuals(IReadOnlyList<VehicleLight> lights, bool headlightsOn, bool brakesOn)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		if (materialPB == null)
		{
			materialPB = new MaterialPropertyBlock();
		}
		foreach (VehicleLight light in lights)
		{
			if ((Object)(object)light.toggleObject != (Object)null)
			{
				light.toggleObject.SetActive(headlightsOn);
			}
			if ((Object)(object)light.lightRenderer != (Object)null)
			{
				Color val = (headlightsOn ? light.lightOnColour : ((!(light.IsBrake && brakesOn)) ? Color.get_black() : light.brakesOnColour));
				materialPB.SetColor(emissionColorID, val);
				light.lightRenderer.SetPropertyBlock(materialPB, light.lightRendererMaterialIndex);
			}
		}
	}

	public VehicleLight()
		: this()
	{
	}
}
