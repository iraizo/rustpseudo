using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class WiggleRenderer : PostProcessEffectRenderer<Wiggle>
{
	private int timerProperty = Shader.PropertyToID("_timer");

	private int scaleProperty = Shader.PropertyToID("_scale");

	private Shader wiggleShader;

	private float timer;

	public override void Init()
	{
		base.Init();
		wiggleShader = Shader.Find("Hidden/PostProcessing/Wiggle");
	}

	public override void Render(PostProcessRenderContext context)
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		CommandBuffer command = context.command;
		command.BeginSample("Wiggle");
		timer += base.settings.speed.value * Time.get_deltaTime();
		PropertySheet propertySheet = context.propertySheets.Get(wiggleShader);
		propertySheet.properties.Clear();
		propertySheet.properties.SetFloat(timerProperty, timer);
		propertySheet.properties.SetFloat(scaleProperty, base.settings.scale.value);
		context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0);
		command.EndSample("Wiggle");
	}
}
