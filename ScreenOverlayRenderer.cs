using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class ScreenOverlayRenderer : PostProcessEffectRenderer<ScreenOverlay>
{
	private Shader overlayShader;

	public override void Init()
	{
		base.Init();
		overlayShader = Shader.Find("Hidden/PostProcessing/ScreenOverlay");
	}

	public override void Render(PostProcessRenderContext context)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		CommandBuffer command = context.command;
		command.BeginSample("ScreenOverlay");
		PropertySheet propertySheet = context.propertySheets.Get(overlayShader);
		propertySheet.properties.Clear();
		Vector4 val = default(Vector4);
		((Vector4)(ref val))._002Ector(1f, 0f, 0f, 1f);
		propertySheet.properties.SetVector("_UV_Transform", val);
		propertySheet.properties.SetFloat("_Intensity", (float)base.settings.intensity);
		if (Object.op_Implicit((Object)(object)TOD_Sky.get_Instance()))
		{
			propertySheet.properties.SetVector("_LightDir", Vector4.op_Implicit(((Component)context.camera).get_transform().InverseTransformDirection(TOD_Sky.get_Instance().get_LightDirection())));
			propertySheet.properties.SetColor("_LightCol", TOD_Sky.get_Instance().get_LightColor() * TOD_Sky.get_Instance().get_LightIntensity());
		}
		if (Object.op_Implicit((Object)(object)base.settings.texture.value))
		{
			propertySheet.properties.SetTexture("_Overlay", base.settings.texture.value);
		}
		if (Object.op_Implicit((Object)(object)base.settings.normals.value))
		{
			propertySheet.properties.SetTexture("_Normals", base.settings.normals.value);
		}
		context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, (int)base.settings.blendMode.value);
		command.EndSample("ScreenOverlay");
	}
}
