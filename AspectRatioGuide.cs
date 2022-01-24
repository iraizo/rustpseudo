using Rust.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AspectRatioGuide : MonoBehaviour
{
	public AspectRatioFitter aspectRatioFitter;

	public RustText label;

	public float aspect;

	public float ratio;

	private void Populate()
	{
		aspect = CameraMan.GuideAspect;
		ratio = Mathf.Max(CameraMan.GuideRatio, 1f);
		aspectRatioFitter.set_aspectRatio(aspect / ratio);
		((TMP_Text)label).set_text($"{aspect}:{ratio}");
	}

	public void Awake()
	{
		Populate();
	}

	public void Update()
	{
		Populate();
	}

	public AspectRatioGuide()
		: this()
	{
	}
}
