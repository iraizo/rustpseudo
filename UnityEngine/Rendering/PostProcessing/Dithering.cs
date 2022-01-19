using System;
using UnityEngine.Assertions;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	[Serializable]
	[Preserve]
	internal sealed class Dithering
	{
		private int m_NoiseTextureIndex;

		internal void Render(PostProcessRenderContext context)
		{
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			Texture2D[] blueNoise = context.resources.blueNoise64;
			Assert.IsTrue(blueNoise != null && blueNoise.Length != 0);
			if (++m_NoiseTextureIndex >= blueNoise.Length)
			{
				m_NoiseTextureIndex = 0;
			}
			float value = Random.get_value();
			float value2 = Random.get_value();
			Texture2D val = blueNoise[m_NoiseTextureIndex];
			PropertySheet uberSheet = context.uberSheet;
			uberSheet.properties.SetTexture(ShaderIDs.DitheringTex, (Texture)(object)val);
			uberSheet.properties.SetVector(ShaderIDs.Dithering_Coords, new Vector4((float)context.screenWidth / (float)((Texture)val).get_width(), (float)context.screenHeight / (float)((Texture)val).get_height(), value, value2));
		}
	}
}
