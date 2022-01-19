using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class DoubleVisionRenderer : PostProcessEffectRenderer<DoubleVision>
{
	private int displaceProperty = Shader.PropertyToID("_displace");

	private int amountProperty = Shader.PropertyToID("_amount");

	private Shader doubleVisionShader;

	public override void Init()
	{
		base.Init();
		doubleVisionShader = Shader.Find("Hidden/PostProcessing/DoubleVision");
	}

	public override void Render(PostProcessRenderContext context)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		CommandBuffer command = context.command;
		command.BeginSample("DoubleVision");
		PropertySheet propertySheet = context.propertySheets.Get(doubleVisionShader);
		propertySheet.properties.Clear();
		propertySheet.properties.SetVector(displaceProperty, Vector4.op_Implicit(base.settings.displace.value));
		propertySheet.properties.SetFloat(amountProperty, base.settings.amount.value);
		command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0);
		command.EndSample("DoubleVision");
	}
}
