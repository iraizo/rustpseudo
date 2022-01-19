using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class PhotoFilterRenderer : PostProcessEffectRenderer<PhotoFilter>
{
	private int rgbProperty = Shader.PropertyToID("_rgb");

	private int densityProperty = Shader.PropertyToID("_density");

	private Shader greyScaleShader;

	public override void Init()
	{
		base.Init();
		greyScaleShader = Shader.Find("Hidden/PostProcessing/PhotoFilter");
	}

	public override void Render(PostProcessRenderContext context)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		CommandBuffer command = context.command;
		command.BeginSample("PhotoFilter");
		PropertySheet propertySheet = context.propertySheets.Get(greyScaleShader);
		propertySheet.properties.Clear();
		propertySheet.properties.SetColor(rgbProperty, base.settings.color.value);
		propertySheet.properties.SetFloat(densityProperty, base.settings.density.value);
		command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0);
		command.EndSample("PhotoFilter");
	}
}
