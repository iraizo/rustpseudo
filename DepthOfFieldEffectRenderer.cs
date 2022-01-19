using System;
using ConVar;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class DepthOfFieldEffectRenderer : PostProcessEffectRenderer<DepthOfFieldEffect>
{
	private float focalDistance01 = 10f;

	private float internalBlurWidth = 1f;

	private Shader dofShader;

	public override void Init()
	{
		dofShader = Shader.Find("Hidden/PostProcessing/DepthOfFieldEffect");
	}

	private float FocalDistance01(Camera cam, float worldDist)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		return cam.WorldToViewportPoint((worldDist - cam.get_nearClipPlane()) * ((Component)cam).get_transform().get_forward() + ((Component)cam).get_transform().get_position()).z / (cam.get_farClipPlane() - cam.get_nearClipPlane());
	}

	private void WriteCoc(PostProcessRenderContext context, PropertySheet sheet)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		CommandBuffer command = context.command;
		RenderTargetIdentifier source = context.source;
		RenderTextureFormat sourceFormat = context.sourceFormat;
		float num = 1f;
		int num2 = context.width / 2;
		int num3 = context.height / 2;
		int num4 = Shader.PropertyToID("DOFtemp1");
		int num5 = Shader.PropertyToID("DOFtemp2");
		command.GetTemporaryRT(num5, num2, num3, 0, (FilterMode)1, sourceFormat);
		command.BlitFullscreenTriangle(source, RenderTargetIdentifier.op_Implicit(num5), sheet, 1);
		float num6 = internalBlurWidth * num;
		sheet.properties.SetVector("_Offsets", new Vector4(0f, num6, 0f, num6));
		command.GetTemporaryRT(num4, num2, num3, 0, (FilterMode)1, sourceFormat);
		command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit(num5), RenderTargetIdentifier.op_Implicit(num4), sheet, 0);
		command.ReleaseTemporaryRT(num5);
		sheet.properties.SetVector("_Offsets", new Vector4(num6, 0f, 0f, num6));
		command.GetTemporaryRT(num5, num2, num3, 0, (FilterMode)1, sourceFormat);
		command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit(num4), RenderTargetIdentifier.op_Implicit(num5), sheet, 0);
		command.ReleaseTemporaryRT(num4);
		command.SetGlobalTexture("_FgOverlap", RenderTargetIdentifier.op_Implicit(num5));
		command.BlitFullscreenTriangle(source, source, sheet, 3, (RenderBufferLoadAction)0);
		command.ReleaseTemporaryRT(num5);
	}

	public override void Render(PostProcessRenderContext context)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		PropertySheet propertySheet = context.propertySheets.Get(dofShader);
		CommandBuffer command = context.command;
		int width = context.width;
		int height = context.height;
		RenderTextureFormat sourceFormat = context.sourceFormat;
		bool value = base.settings.highResolution.value;
		DOFBlurSampleCountParameter blurSampleCount = base.settings.blurSampleCount;
		float value2 = base.settings.focalSize.value;
		float value3 = base.settings.focalLength.value;
		float value4 = base.settings.aperture.value;
		float value5 = base.settings.maxBlurSize.value;
		int num = Shader.PropertyToID("DOFrtLow");
		int num2 = Shader.PropertyToID("DOFrtLow2");
		value4 = Math.Max(value4, 0f);
		value5 = Math.Max(value5, 0.1f);
		value2 = Mathf.Clamp(value2, 0f, 2f);
		internalBlurWidth = Mathf.Max(value5, 0f);
		focalDistance01 = FocalDistance01(context.camera, value3);
		propertySheet.properties.SetVector("_CurveParams", new Vector4(1f, value2, value4 / 10f, focalDistance01));
		if (value)
		{
			internalBlurWidth *= 2f;
		}
		WriteCoc(context, propertySheet);
		if (Graphics.dof_debug)
		{
			command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 5);
			return;
		}
		command.GetTemporaryRT(num, width >> 1, height >> 1, 0, (FilterMode)1, sourceFormat);
		command.GetTemporaryRT(num2, width >> 1, height >> 1, 0, (FilterMode)1, sourceFormat);
		int pass = (((DOFBlurSampleCount)blurSampleCount == DOFBlurSampleCount.High || (DOFBlurSampleCount)blurSampleCount == DOFBlurSampleCount.Medium) ? 4 : 2);
		propertySheet.properties.SetVector("_Offsets", new Vector4(0f, internalBlurWidth, 0.025f, internalBlurWidth));
		command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, pass);
		command.ReleaseTemporaryRT(num);
		command.ReleaseTemporaryRT(num2);
	}
}
