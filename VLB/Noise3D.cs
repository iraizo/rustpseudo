using System;
using UnityEngine;

namespace VLB
{
	public static class Noise3D
	{
		private static bool ms_IsSupportedChecked;

		private static bool ms_IsSupported;

		private static Texture3D ms_NoiseTexture;

		private const HideFlags kHideFlags = 61;

		private const int kMinShaderLevel = 35;

		public static bool isSupported
		{
			get
			{
				if (!ms_IsSupportedChecked)
				{
					ms_IsSupported = SystemInfo.get_graphicsShaderLevel() >= 35;
					if (!ms_IsSupported)
					{
						Debug.LogWarning((object)isNotSupportedString);
					}
					ms_IsSupportedChecked = true;
				}
				return ms_IsSupported;
			}
		}

		public static bool isProperlyLoaded => (Object)(object)ms_NoiseTexture != (Object)null;

		public static string isNotSupportedString => $"3D Noise requires higher shader capabilities (Shader Model 3.5 / OpenGL ES 3.0), which are not available on the current platform: graphicsShaderLevel (current/required) = {SystemInfo.get_graphicsShaderLevel()} / {35}";

		[RuntimeInitializeOnLoadMethod]
		private static void OnStartUp()
		{
			LoadIfNeeded();
		}

		public static void LoadIfNeeded()
		{
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			if (!isSupported)
			{
				return;
			}
			if ((Object)(object)ms_NoiseTexture == (Object)null)
			{
				ms_NoiseTexture = LoadTexture3D(Config.Instance.noise3DData, Config.Instance.noise3DSize);
				if (Object.op_Implicit((Object)(object)ms_NoiseTexture))
				{
					((Object)ms_NoiseTexture).set_hideFlags((HideFlags)61);
				}
			}
			Shader.SetGlobalTexture("_VLB_NoiseTex3D", (Texture)(object)ms_NoiseTexture);
			Shader.SetGlobalVector("_VLB_NoiseGlobal", Config.Instance.globalNoiseParam);
		}

		private static Texture3D LoadTexture3D(TextAsset textData, int size)
		{
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Expected O, but got Unknown
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)textData == (Object)null)
			{
				Debug.LogErrorFormat("Fail to open Noise 3D Data", Array.Empty<object>());
				return null;
			}
			byte[] bytes = textData.get_bytes();
			Debug.Assert(bytes != null);
			int num = Mathf.Max(0, size * size * size);
			if (bytes.Length != num)
			{
				Debug.LogErrorFormat("Noise 3D Data file has not the proper size {0}x{0}x{0}", new object[1] { size });
				return null;
			}
			Texture3D val = new Texture3D(size, size, size, (TextureFormat)1, false);
			Color[] array = (Color[])(object)new Color[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = Color32.op_Implicit(new Color32((byte)0, (byte)0, (byte)0, bytes[i]));
			}
			val.SetPixels(array);
			val.Apply();
			return val;
		}
	}
}
