using UnityEngine;

public class ToolgunBeam : MonoBehaviour
{
	public LineRenderer electricalBeam;

	public float scrollSpeed = -8f;

	private Color fadeColor = new Color(1f, 1f, 1f, 1f);

	public float fadeSpeed = 4f;

	public void Update()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		if (fadeColor.a <= 0f)
		{
			Object.Destroy((Object)(object)((Component)this).get_gameObject());
			return;
		}
		((Renderer)electricalBeam).get_sharedMaterial().SetTextureOffset("_MainTex", new Vector2(Time.get_time() * scrollSpeed, 0f));
		fadeColor.a -= Time.get_deltaTime() * fadeSpeed;
		electricalBeam.set_startColor(fadeColor);
		electricalBeam.set_endColor(fadeColor);
	}

	public ToolgunBeam()
		: this()
	{
	}//IL_0020: Unknown result type (might be due to invalid IL or missing references)
	//IL_0025: Unknown result type (might be due to invalid IL or missing references)

}
