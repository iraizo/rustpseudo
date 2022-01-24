using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
	[ExecuteInEditMode]
	[AddComponentMenu("Image Effects/Other/Scope Overlay")]
	public class ScopeEffect : PostEffectsBase, IImageEffect
	{
		public Material overlayMaterial;

		public override bool CheckResources()
		{
			return true;
		}

		public bool IsActive()
		{
			if (((Behaviour)this).get_enabled())
			{
				return ((PostEffectsBase)this).CheckResources();
			}
			return false;
		}

		public void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			overlayMaterial.SetVector("_Screen", Vector4.op_Implicit(new Vector2((float)Screen.get_width(), (float)Screen.get_height())));
			Graphics.Blit((Texture)(object)source, destination, overlayMaterial);
		}

		public ScopeEffect()
			: this()
		{
		}
	}
}
