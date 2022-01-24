using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class SharpenAndVignetteRenderer : PostProcessEffectRenderer<SharpenAndVignette>
{
	private Shader sharpenAndVigenetteShader;

	public override void Init()
	{
		base.Init();
		sharpenAndVigenetteShader = Shader.Find("Hidden/PostProcessing/SharpenAndVignette");
	}

	public override void Render(PostProcessRenderContext context)
	{
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		CommandBuffer command = context.command;
		command.BeginSample("SharpenAndVignette");
		PropertySheet propertySheet = context.propertySheets.Get(sharpenAndVigenetteShader);
		propertySheet.properties.Clear();
		bool value = base.settings.applySharpen.value;
		bool value2 = base.settings.applyVignette.value;
		if (value)
		{
			propertySheet.properties.SetFloat("_px", 1f / (float)Screen.get_width());
			propertySheet.properties.SetFloat("_py", 1f / (float)Screen.get_height());
			propertySheet.properties.SetFloat("_strength", base.settings.strength.value);
			propertySheet.properties.SetFloat("_clamp", base.settings.clamp.value);
		}
		if (value2)
		{
			propertySheet.properties.SetFloat("_sharpness", base.settings.sharpness.value * 0.01f);
			propertySheet.properties.SetFloat("_darkness", base.settings.darkness.value * 0.02f);
		}
		if (value && !value2)
		{
			command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0);
		}
		else if (value && value2)
		{
			command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 1);
		}
		else if (!value && value2)
		{
			command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 2);
		}
		else
		{
			command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0);
		}
		command.EndSample("SharpenAndVignette");
	}
}
