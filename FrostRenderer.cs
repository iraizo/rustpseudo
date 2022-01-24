using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class FrostRenderer : PostProcessEffectRenderer<Frost>
{
	private int scaleProperty = Shader.PropertyToID("_scale");

	private int sharpnessProperty = Shader.PropertyToID("_sharpness");

	private int darknessProperty = Shader.PropertyToID("_darkness");

	private Shader frostShader;

	public override void Init()
	{
		base.Init();
		frostShader = Shader.Find("Hidden/PostProcessing/Frost");
	}

	public override void Render(PostProcessRenderContext context)
	{
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		CommandBuffer command = context.command;
		command.BeginSample("Frost");
		PropertySheet propertySheet = context.propertySheets.Get(frostShader);
		propertySheet.properties.Clear();
		propertySheet.properties.SetFloat(scaleProperty, base.settings.scale.value);
		propertySheet.properties.SetFloat(sharpnessProperty, base.settings.sharpness.value * 0.01f);
		propertySheet.properties.SetFloat(darknessProperty, base.settings.darkness.value * 0.02f);
		command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, base.settings.enableVignette.value ? 1 : 0);
		command.EndSample("Frost");
	}
}
