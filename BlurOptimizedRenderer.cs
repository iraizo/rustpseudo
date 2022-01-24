using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class BlurOptimizedRenderer : PostProcessEffectRenderer<BlurOptimized>
{
	private int dataProperty = Shader.PropertyToID("_data");

	private Shader blurShader;

	public override void Init()
	{
		base.Init();
		blurShader = Shader.Find("Hidden/PostProcessing/BlurOptimized");
	}

	public override void Render(PostProcessRenderContext context)
	{
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		CommandBuffer command = context.command;
		command.BeginSample("BlurOptimized");
		int value = base.settings.downsample.value;
		float value2 = base.settings.fadeToBlurDistance.value;
		float value3 = base.settings.blurSize.value;
		int value4 = base.settings.blurIterations.value;
		BlurType value5 = base.settings.blurType.value;
		float num = 1f / (1f * (float)(1 << value));
		float num2 = 1f / Mathf.Clamp(value2, 0.001f, 10000f);
		PropertySheet propertySheet = context.propertySheets.Get(blurShader);
		propertySheet.properties.SetVector("_Parameter", new Vector4(value3 * num, (0f - value3) * num, num2, 0f));
		int num3 = context.width >> value;
		int num4 = context.height >> value;
		int num5 = Shader.PropertyToID("_BlurRT1");
		int num6 = Shader.PropertyToID("_BlurRT2");
		command.GetTemporaryRT(num5, num3, num4, 0, (FilterMode)1, context.sourceFormat, (RenderTextureReadWrite)0);
		command.BlitFullscreenTriangle(context.source, RenderTargetIdentifier.op_Implicit(num5), propertySheet, 0);
		int num7 = ((value5 != 0) ? 2 : 0);
		for (int i = 0; i < value4; i++)
		{
			float num8 = (float)i * 1f;
			propertySheet.properties.SetVector("_Parameter", new Vector4(value3 * num + num8, (0f - value3) * num - num8, num2, 0f));
			command.GetTemporaryRT(num6, num3, num4, 0, (FilterMode)1, context.sourceFormat);
			command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit(num5), RenderTargetIdentifier.op_Implicit(num6), propertySheet, 1 + num7);
			command.ReleaseTemporaryRT(num5);
			command.GetTemporaryRT(num5, num3, num4, 0, (FilterMode)1, context.sourceFormat);
			command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit(num6), RenderTargetIdentifier.op_Implicit(num5), propertySheet, 2 + num7);
			command.ReleaseTemporaryRT(num6);
		}
		if (value2 <= 0f)
		{
			command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit(num5), context.destination);
		}
		else
		{
			command.SetGlobalTexture("_Source", context.source);
			command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit(num5), context.destination, propertySheet, 5);
		}
		command.ReleaseTemporaryRT(num5);
		command.EndSample("BlurOptimized");
	}
}
