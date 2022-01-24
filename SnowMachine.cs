using UnityEngine;

public class SnowMachine : FogMachine
{
	public AdaptMeshToTerrain snowMesh;

	public TriggerTemperature tempTrigger;

	public override bool MotionModeEnabled()
	{
		return false;
	}

	public override void EnableFogField()
	{
		base.EnableFogField();
		((Component)tempTrigger).get_gameObject().SetActive(true);
	}

	public override void FinishFogging()
	{
		base.FinishFogging();
		((Component)tempTrigger).get_gameObject().SetActive(false);
	}
}
