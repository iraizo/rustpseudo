using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class LensDirtinessRenderer : PostProcessEffectRenderer<LensDirtinessEffect>
{
	private enum Pass
	{
		Threshold,
		Kawase,
		Compose
	}

	private int dataProperty = Shader.PropertyToID("_data");

	private Shader lensDirtinessShader;

	public override void Init()
	{
		base.Init();
		lensDirtinessShader = Shader.Find("Hidden/PostProcessing/LensDirtiness");
	}

	public override void Render(PostProcessRenderContext context)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		float value = base.settings.bloomSize.value;
		float value2 = base.settings.gain.value;
		float value3 = base.settings.threshold.value;
		float value4 = base.settings.dirtiness.value;
		Color value5 = base.settings.bloomColor.value;
		Texture value6 = base.settings.dirtinessTexture.value;
		bool value7 = base.settings.sceneTintsBloom.value;
		CommandBuffer command = context.command;
		command.BeginSample("LensDirtinessEffect");
		if (value7)
		{
			command.EnableShaderKeyword("_SCENE_TINTS_BLOOM");
		}
		PropertySheet propertySheet = context.propertySheets.Get(lensDirtinessShader);
		RenderTargetIdentifier source = context.source;
		RenderTargetIdentifier destination = context.destination;
		int width = context.width;
		int height = context.height;
		int num = Shader.PropertyToID("_RTT_BloomThreshold");
		int num2 = Shader.PropertyToID("_RTT_1");
		int num3 = Shader.PropertyToID("_RTT_2");
		int num4 = Shader.PropertyToID("_RTT_3");
		int num5 = Shader.PropertyToID("_RTT_4");
		int num6 = Shader.PropertyToID("_RTT_Bloom_1");
		int num7 = Shader.PropertyToID("_RTT_Bloom_2");
		propertySheet.properties.SetFloat("_Gain", value2);
		propertySheet.properties.SetFloat("_Threshold", value3);
		command.GetTemporaryRT(num, width / 2, height / 2, 0, (FilterMode)1, context.sourceFormat);
		command.BlitFullscreenTriangle(source, RenderTargetIdentifier.op_Implicit(num), propertySheet, 0);
		propertySheet.properties.SetVector("_Offset", new Vector4(1f / (float)width, 1f / (float)height, 0f, 0f) * 2f);
		command.GetTemporaryRT(num2, width / 2, height / 2, 0, (FilterMode)1, context.sourceFormat);
		command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit(num), RenderTargetIdentifier.op_Implicit(num2), propertySheet, 1);
		command.ReleaseTemporaryRT(num);
		command.GetTemporaryRT(num3, width / 4, height / 4, 0, (FilterMode)1, context.sourceFormat);
		command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit(num2), RenderTargetIdentifier.op_Implicit(num3), propertySheet, 1);
		command.ReleaseTemporaryRT(num2);
		command.GetTemporaryRT(num4, width / 8, height / 8, 0, (FilterMode)1, context.sourceFormat);
		command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit(num3), RenderTargetIdentifier.op_Implicit(num4), propertySheet, 1);
		command.ReleaseTemporaryRT(num3);
		command.GetTemporaryRT(num5, width / 16, height / 16, 0, (FilterMode)1, context.sourceFormat);
		command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit(num4), RenderTargetIdentifier.op_Implicit(num5), propertySheet, 1);
		command.ReleaseTemporaryRT(num4);
		command.GetTemporaryRT(num6, width / 16, height / 16, 0, (FilterMode)1, context.sourceFormat);
		command.GetTemporaryRT(num7, width / 16, height / 16, 0, (FilterMode)1, context.sourceFormat);
		command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit(num5), RenderTargetIdentifier.op_Implicit(num6));
		command.ReleaseTemporaryRT(num5);
		for (int i = 1; i <= 8; i++)
		{
			float num8 = value * (float)i / (float)width;
			float num9 = value * (float)i / (float)height;
			propertySheet.properties.SetVector("_Offset", new Vector4(num8, num9, 0f, 0f));
			command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit(num6), RenderTargetIdentifier.op_Implicit(num7), propertySheet, 1);
			command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit(num7), RenderTargetIdentifier.op_Implicit(num6), propertySheet, 1);
		}
		command.SetGlobalTexture("_Bloom", RenderTargetIdentifier.op_Implicit(num7));
		propertySheet.properties.SetFloat("_Dirtiness", value4);
		propertySheet.properties.SetColor("_BloomColor", value5);
		propertySheet.properties.SetTexture("_DirtinessTexture", value6);
		command.BlitFullscreenTriangle(source, destination, propertySheet, 2);
		command.ReleaseTemporaryRT(num6);
		command.ReleaseTemporaryRT(num7);
		command.EndSample("LensDirtinessEffect");
	}
}
