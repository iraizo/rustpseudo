using UnityEngine;

namespace UnityStandardAssets.CinematicEffects
{
	public static class ImageEffectHelper
	{
		public static bool supportsDX11
		{
			get
			{
				if (SystemInfo.get_graphicsShaderLevel() >= 50)
				{
					return SystemInfo.get_supportsComputeShaders();
				}
				return false;
			}
		}

		public static bool IsSupported(Shader s, bool needDepth, bool needHdr, MonoBehaviour effect)
		{
			if ((Object)(object)s == (Object)null || !s.get_isSupported())
			{
				Debug.LogWarningFormat("Missing shader for image effect {0}", new object[1] { effect });
				return false;
			}
			if (!SystemInfo.get_supportsImageEffects() || !SystemInfo.get_supportsRenderTextures())
			{
				Debug.LogWarningFormat("Image effects aren't supported on this device ({0})", new object[1] { effect });
				return false;
			}
			if (needDepth && !SystemInfo.SupportsRenderTextureFormat((RenderTextureFormat)1))
			{
				Debug.LogWarningFormat("Depth textures aren't supported on this device ({0})", new object[1] { effect });
				return false;
			}
			if (needHdr && !SystemInfo.SupportsRenderTextureFormat((RenderTextureFormat)2))
			{
				Debug.LogWarningFormat("Floating point textures aren't supported on this device ({0})", new object[1] { effect });
				return false;
			}
			return true;
		}

		public static Material CheckShaderAndCreateMaterial(Shader s)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			if ((Object)(object)s == (Object)null || !s.get_isSupported())
			{
				return null;
			}
			Material val = new Material(s);
			((Object)val).set_hideFlags((HideFlags)52);
			return val;
		}
	}
}
