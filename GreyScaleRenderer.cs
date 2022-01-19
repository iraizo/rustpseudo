using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class GreyScaleRenderer : PostProcessEffectRenderer<GreyScale>
{
	private int dataProperty = Shader.PropertyToID("_data");

	private int colorProperty = Shader.PropertyToID("_color");

	private Shader greyScaleShader;

	public override void Init()
	{
		base.Init();
		greyScaleShader = Shader.Find("Hidden/PostProcessing/GreyScale");
	}

	public override void Render(PostProcessRenderContext context)
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		CommandBuffer command = context.command;
		command.BeginSample("GreyScale");
		PropertySheet propertySheet = context.propertySheets.Get(greyScaleShader);
		propertySheet.properties.Clear();
		propertySheet.properties.SetVector(dataProperty, new Vector4(base.settings.redLuminance.value, base.settings.greenLuminance.value, base.settings.blueLuminance.value, base.settings.amount.value));
		propertySheet.properties.SetColor(colorProperty, base.settings.color.value);
		context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0);
		command.EndSample("GreyScale");
	}
}
