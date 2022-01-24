using System;

namespace UnityEngine.Rendering.PostProcessing
{
	[Serializable]
	public sealed class TextureParameter : ParameterOverride<Texture>
	{
		public TextureParameterDefault defaultState = TextureParameterDefault.Black;

		public override void Interp(Texture from, Texture to, float t)
		{
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)from == (Object)null && (Object)(object)to == (Object)null)
			{
				value = null;
				return;
			}
			if ((Object)(object)from != (Object)null && (Object)(object)to != (Object)null)
			{
				value = TextureLerper.instance.Lerp(from, to, t);
				return;
			}
			if (defaultState == TextureParameterDefault.Lut2D)
			{
				Texture lutStrip = (Texture)(object)RuntimeUtilities.GetLutStrip(((Object)(object)from != (Object)null) ? from.get_height() : to.get_height());
				if ((Object)(object)from == (Object)null)
				{
					from = lutStrip;
				}
				if ((Object)(object)to == (Object)null)
				{
					to = lutStrip;
				}
			}
			Color to2;
			switch (defaultState)
			{
			case TextureParameterDefault.Black:
				to2 = Color.get_black();
				break;
			case TextureParameterDefault.White:
				to2 = Color.get_white();
				break;
			case TextureParameterDefault.Transparent:
				to2 = Color.get_clear();
				break;
			case TextureParameterDefault.Lut2D:
			{
				Texture lutStrip2 = (Texture)(object)RuntimeUtilities.GetLutStrip(((Object)(object)from != (Object)null) ? from.get_height() : to.get_height());
				if ((Object)(object)from == (Object)null)
				{
					from = lutStrip2;
				}
				if ((Object)(object)to == (Object)null)
				{
					to = lutStrip2;
				}
				if (from.get_width() != to.get_width() || from.get_height() != to.get_height())
				{
					value = null;
				}
				else
				{
					value = TextureLerper.instance.Lerp(from, to, t);
				}
				return;
			}
			default:
				base.Interp(from, to, t);
				return;
			}
			if ((Object)(object)from == (Object)null)
			{
				value = TextureLerper.instance.Lerp(to, to2, 1f - t);
			}
			else
			{
				value = TextureLerper.instance.Lerp(from, to2, t);
			}
		}
	}
}
