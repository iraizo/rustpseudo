using UnityEngine;

public class ExplosionDemoGUI : MonoBehaviour
{
	public GameObject[] Prefabs;

	public float reactivateTime = 4f;

	public Light Sun;

	private int currentNomber;

	private GameObject currentInstance;

	private GUIStyle guiStyleHeader = new GUIStyle();

	private float sunIntensity;

	private float dpiScale;

	private void Start()
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		if (Screen.get_dpi() < 1f)
		{
			dpiScale = 1f;
		}
		if (Screen.get_dpi() < 200f)
		{
			dpiScale = 1f;
		}
		else
		{
			dpiScale = Screen.get_dpi() / 200f;
		}
		guiStyleHeader.set_fontSize((int)(15f * dpiScale));
		guiStyleHeader.get_normal().set_textColor(new Color(0.15f, 0.15f, 0.15f));
		currentInstance = Object.Instantiate<GameObject>(Prefabs[currentNomber], ((Component)this).get_transform().get_position(), default(Quaternion));
		currentInstance.AddComponent<ExplosionDemoReactivator>().TimeDelayToReactivate = reactivateTime;
		sunIntensity = Sun.get_intensity();
	}

	private void OnGUI()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		if (GUI.Button(new Rect(10f * dpiScale, 15f * dpiScale, 135f * dpiScale, 37f * dpiScale), "PREVIOUS EFFECT"))
		{
			ChangeCurrent(-1);
		}
		if (GUI.Button(new Rect(160f * dpiScale, 15f * dpiScale, 135f * dpiScale, 37f * dpiScale), "NEXT EFFECT"))
		{
			ChangeCurrent(1);
		}
		sunIntensity = GUI.HorizontalSlider(new Rect(10f * dpiScale, 70f * dpiScale, 285f * dpiScale, 15f * dpiScale), sunIntensity, 0f, 0.6f);
		Sun.set_intensity(sunIntensity);
		GUI.Label(new Rect(300f * dpiScale, 70f * dpiScale, 30f * dpiScale, 30f * dpiScale), "SUN INTENSITY", guiStyleHeader);
		GUI.Label(new Rect(400f * dpiScale, 15f * dpiScale, 100f * dpiScale, 20f * dpiScale), "Prefab name is \"" + ((Object)Prefabs[currentNomber]).get_name() + "\"  \r\nHold any mouse button that would move the camera", guiStyleHeader);
	}

	private void ChangeCurrent(int delta)
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		currentNomber += delta;
		if (currentNomber > Prefabs.Length - 1)
		{
			currentNomber = 0;
		}
		else if (currentNomber < 0)
		{
			currentNomber = Prefabs.Length - 1;
		}
		if ((Object)(object)currentInstance != (Object)null)
		{
			Object.Destroy((Object)(object)currentInstance);
		}
		currentInstance = Object.Instantiate<GameObject>(Prefabs[currentNomber], ((Component)this).get_transform().get_position(), default(Quaternion));
		currentInstance.AddComponent<ExplosionDemoReactivator>().TimeDelayToReactivate = reactivateTime;
	}

	public ExplosionDemoGUI()
		: this()
	{
	}//IL_000c: Unknown result type (might be due to invalid IL or missing references)
	//IL_0016: Expected O, but got Unknown

}
