using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.CinematicEffects
{
	public class RenderTextureUtility
	{
		private List<RenderTexture> m_TemporaryRTs = new List<RenderTexture>();

		public RenderTexture GetTemporaryRenderTexture(int width, int height, int depthBuffer = 0, RenderTextureFormat format = 2, FilterMode filterMode = 1)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			RenderTexture temporary = RenderTexture.GetTemporary(width, height, depthBuffer, format);
			((Texture)temporary).set_filterMode(filterMode);
			((Texture)temporary).set_wrapMode((TextureWrapMode)1);
			((Object)temporary).set_name("RenderTextureUtilityTempTexture");
			m_TemporaryRTs.Add(temporary);
			return temporary;
		}

		public void ReleaseTemporaryRenderTexture(RenderTexture rt)
		{
			if (!((Object)(object)rt == (Object)null))
			{
				if (!m_TemporaryRTs.Contains(rt))
				{
					Debug.LogErrorFormat("Attempting to remove texture that was not allocated: {0}", new object[1] { rt });
				}
				else
				{
					m_TemporaryRTs.Remove(rt);
					RenderTexture.ReleaseTemporary(rt);
				}
			}
		}

		public void ReleaseAllTemporaryRenderTextures()
		{
			for (int i = 0; i < m_TemporaryRTs.Count; i++)
			{
				RenderTexture.ReleaseTemporary(m_TemporaryRTs[i]);
			}
			m_TemporaryRTs.Clear();
		}
	}
}
