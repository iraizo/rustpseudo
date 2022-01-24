using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class GodRaysRenderer : PostProcessEffectRenderer<GodRays>
{
	private const int PASS_SCREEN = 0;

	private const int PASS_ADD = 1;

	public Shader GodRayShader;

	public Shader ScreenClearShader;

	public Shader SkyMaskShader;

	public override void Init()
	{
		if (!Object.op_Implicit((Object)(object)GodRayShader))
		{
			GodRayShader = Shader.Find("Hidden/PostProcessing/GodRays");
		}
		if (!Object.op_Implicit((Object)(object)ScreenClearShader))
		{
			ScreenClearShader = Shader.Find("Hidden/PostProcessing/ScreenClear");
		}
		if (!Object.op_Implicit((Object)(object)SkyMaskShader))
		{
			SkyMaskShader = Shader.Find("Hidden/PostProcessing/SkyMask");
		}
	}

	private void DrawBorder(PostProcessRenderContext context, RenderTargetIdentifier buffer1)
	{
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		PropertySheet propertySheet = context.propertySheets.Get(ScreenClearShader);
		Rect value = default(Rect);
		((Rect)(ref value))._002Ector(0f, (float)(context.height - 1), (float)context.width, 1f);
		Rect value2 = default(Rect);
		((Rect)(ref value2))._002Ector(0f, 0f, (float)context.width, 1f);
		Rect value3 = default(Rect);
		((Rect)(ref value3))._002Ector(0f, 0f, 1f, (float)context.height);
		Rect value4 = default(Rect);
		((Rect)(ref value4))._002Ector((float)(context.width - 1), 0f, 1f, (float)context.height);
		context.command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)0), buffer1, propertySheet, 0, clear: false, value);
		context.command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)0), buffer1, propertySheet, 0, clear: false, value2);
		context.command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)0), buffer1, propertySheet, 0, clear: false, value3);
		context.command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)0), buffer1, propertySheet, 0, clear: false, value4);
	}

	private int GetSkyMask(PostProcessRenderContext context, ResolutionType resolution, Vector3 lightPos, int blurIterations, float blurRadius, float maxRadius)
	{
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Invalid comparison between Unknown and I4
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		CommandBuffer command = context.command;
		Camera camera = context.camera;
		PropertySheet propertySheet = context.propertySheets.Get(SkyMaskShader);
		command.BeginSample("GodRays");
		int num;
		int num2;
		int num3;
		switch (resolution)
		{
		case ResolutionType.High:
			num = context.screenWidth;
			num2 = context.screenHeight;
			num3 = 0;
			break;
		case ResolutionType.Normal:
			num = context.screenWidth / 2;
			num2 = context.screenHeight / 2;
			num3 = 0;
			break;
		default:
			num = context.screenWidth / 4;
			num2 = context.screenHeight / 4;
			num3 = 0;
			break;
		}
		int num4 = Shader.PropertyToID("buffer1");
		int num5 = Shader.PropertyToID("buffer2");
		command.GetTemporaryRT(num4, num, num2, num3);
		propertySheet.properties.SetVector("_BlurRadius4", new Vector4(1f, 1f, 0f, 0f) * blurRadius);
		propertySheet.properties.SetVector("_LightPosition", new Vector4(lightPos.x, lightPos.y, lightPos.z, maxRadius));
		if ((camera.get_depthTextureMode() & 1) != 0)
		{
			command.BlitFullscreenTriangle(context.source, RenderTargetIdentifier.op_Implicit(num4), propertySheet, 1);
		}
		else
		{
			command.BlitFullscreenTriangle(context.source, RenderTargetIdentifier.op_Implicit(num4), propertySheet, 2);
		}
		if ((int)camera.get_stereoActiveEye() == 2)
		{
			DrawBorder(context, RenderTargetIdentifier.op_Implicit(num4));
		}
		float num6 = blurRadius * 0.0013020834f;
		propertySheet.properties.SetVector("_BlurRadius4", new Vector4(num6, num6, 0f, 0f));
		propertySheet.properties.SetVector("_LightPosition", new Vector4(lightPos.x, lightPos.y, lightPos.z, maxRadius));
		for (int i = 0; i < blurIterations; i++)
		{
			command.GetTemporaryRT(num5, num, num2, num3);
			command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit(num4), RenderTargetIdentifier.op_Implicit(num5), propertySheet, 0);
			command.ReleaseTemporaryRT(num4);
			num6 = blurRadius * (((float)i * 2f + 1f) * 6f) / 768f;
			propertySheet.properties.SetVector("_BlurRadius4", new Vector4(num6, num6, 0f, 0f));
			command.GetTemporaryRT(num4, num, num2, num3);
			command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit(num5), RenderTargetIdentifier.op_Implicit(num4), propertySheet, 0);
			command.ReleaseTemporaryRT(num5);
			num6 = blurRadius * (((float)i * 2f + 2f) * 6f) / 768f;
			propertySheet.properties.SetVector("_BlurRadius4", new Vector4(num6, num6, 0f, 0f));
		}
		command.EndSample("GodRays");
		return num4;
	}

	public override void Render(PostProcessRenderContext context)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		Camera camera = context.camera;
		TOD_Sky instance = TOD_Sky.get_Instance();
		Vector3 val = camera.WorldToViewportPoint(instance.get_Components().get_LightTransform().get_position());
		CommandBuffer command = context.command;
		PropertySheet propertySheet = context.propertySheets.Get(GodRayShader);
		int skyMask = GetSkyMask(context, base.settings.Resolution.value, val, base.settings.BlurIterations.value, base.settings.BlurRadius.value, base.settings.MaxRadius.value);
		Color val2 = Color.get_black();
		if ((double)val.z >= 0.0)
		{
			val2 = ((!instance.get_IsDay()) ? (base.settings.Intensity.value * instance.get_MoonVisibility() * instance.get_MoonRayColor()) : (base.settings.Intensity.value * instance.get_SunVisibility() * instance.get_SunRayColor()));
		}
		propertySheet.properties.SetColor("_LightColor", val2);
		command.SetGlobalTexture("_SkyMask", RenderTargetIdentifier.op_Implicit(skyMask));
		if (base.settings.BlendMode.value == BlendModeType.Screen)
		{
			context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0);
		}
		else
		{
			context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 1);
		}
		command.ReleaseTemporaryRT(skyMask);
	}
}
