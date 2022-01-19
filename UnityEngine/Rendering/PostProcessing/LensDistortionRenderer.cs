using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	[Preserve]
	internal sealed class LensDistortionRenderer : PostProcessEffectRenderer<LensDistortion>
	{
		public override void Render(PostProcessRenderContext context)
		{
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			PropertySheet uberSheet = context.uberSheet;
			float val = 1.6f * Math.Max(Mathf.Abs(base.settings.intensity.value), 1f);
			float num = (float)Math.PI / 180f * Math.Min(160f, val);
			float num2 = 2f * Mathf.Tan(num * 0.5f);
			Vector4 val2 = default(Vector4);
			((Vector4)(ref val2))._002Ector(base.settings.centerX.value, base.settings.centerY.value, Mathf.Max(base.settings.intensityX.value, 0.0001f), Mathf.Max(base.settings.intensityY.value, 0.0001f));
			Vector4 val3 = default(Vector4);
			((Vector4)(ref val3))._002Ector((base.settings.intensity.value >= 0f) ? num : (1f / num), num2, 1f / base.settings.scale.value, base.settings.intensity.value);
			uberSheet.EnableKeyword("DISTORT");
			uberSheet.properties.SetVector(ShaderIDs.Distortion_CenterScale, val2);
			uberSheet.properties.SetVector(ShaderIDs.Distortion_Amount, val3);
		}
	}
}
