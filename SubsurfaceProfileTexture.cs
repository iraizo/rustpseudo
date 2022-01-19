using System.Collections.Generic;
using UnityEngine;

public class SubsurfaceProfileTexture
{
	private struct SubsurfaceProfileEntry
	{
		public SubsurfaceProfileData data;

		public SubsurfaceProfile profile;

		public SubsurfaceProfileEntry(SubsurfaceProfileData data, SubsurfaceProfile profile)
		{
			this.data = data;
			this.profile = profile;
		}
	}

	public const int SUBSURFACE_RADIUS_SCALE = 1024;

	public const int SUBSURFACE_KERNEL_SIZE = 3;

	private List<SubsurfaceProfileEntry> entries = new List<SubsurfaceProfileEntry>(16);

	private Texture2D texture;

	public Texture2D Texture
	{
		get
		{
			if (!((Object)(object)texture == (Object)null))
			{
				return texture;
			}
			return CreateTexture();
		}
	}

	public SubsurfaceProfileTexture()
	{
		AddProfile(SubsurfaceProfileData.Default, null);
	}

	public int FindEntryIndex(SubsurfaceProfile profile)
	{
		for (int i = 0; i < entries.Count; i++)
		{
			if ((Object)(object)entries[i].profile == (Object)(object)profile)
			{
				return i;
			}
		}
		return -1;
	}

	public int AddProfile(SubsurfaceProfileData data, SubsurfaceProfile profile)
	{
		int num = -1;
		for (int i = 0; i < entries.Count; i++)
		{
			if ((Object)(object)entries[i].profile == (Object)(object)profile)
			{
				num = i;
				entries[num] = new SubsurfaceProfileEntry(data, profile);
				break;
			}
		}
		if (num < 0)
		{
			num = entries.Count;
			entries.Add(new SubsurfaceProfileEntry(data, profile));
		}
		ReleaseTexture();
		return num;
	}

	public void UpdateProfile(int id, SubsurfaceProfileData data)
	{
		if (id >= 0)
		{
			entries[id] = new SubsurfaceProfileEntry(data, entries[id].profile);
			ReleaseTexture();
		}
	}

	public void RemoveProfile(int id)
	{
		if (id >= 0)
		{
			entries[id] = new SubsurfaceProfileEntry(SubsurfaceProfileData.Invalid, null);
			CheckReleaseTexture();
		}
	}

	public static Color ColorClamp(Color color, float min = 0f, float max = 1f)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		Color result = default(Color);
		result.r = Mathf.Clamp(color.r, min, max);
		result.g = Mathf.Clamp(color.g, min, max);
		result.b = Mathf.Clamp(color.b, min, max);
		result.a = Mathf.Clamp(color.a, min, max);
		return result;
	}

	private Texture2D CreateTexture()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		if (entries.Count > 0)
		{
			int num = 32;
			int num2 = Mathf.Max(entries.Count, 64);
			ReleaseTexture();
			texture = new Texture2D(num, num2, (TextureFormat)17, false, true);
			((Object)texture).set_name("SubsurfaceProfiles");
			((Texture)texture).set_wrapMode((TextureWrapMode)1);
			((Texture)texture).set_filterMode((FilterMode)1);
			Color[] pixels = texture.GetPixels(0);
			for (int i = 0; i < pixels.Length; i++)
			{
				pixels[i] = Color.get_clear();
			}
			Color[] array = (Color[])(object)new Color[num];
			for (int j = 0; j < entries.Count; j++)
			{
				SubsurfaceProfileData data = entries[j].data;
				data.SubsurfaceColor = ColorClamp(data.SubsurfaceColor);
				data.FalloffColor = ColorClamp(data.FalloffColor, 0.009f);
				array[0] = data.SubsurfaceColor;
				array[0].a = 0f;
				SeparableSSS.CalculateKernel(array, 1, 13, data.SubsurfaceColor, data.FalloffColor);
				SeparableSSS.CalculateKernel(array, 14, 9, data.SubsurfaceColor, data.FalloffColor);
				SeparableSSS.CalculateKernel(array, 23, 6, data.SubsurfaceColor, data.FalloffColor);
				int num3 = num * (num2 - j - 1);
				for (int k = 0; k < 29; k++)
				{
					Color val = array[k] * new Color(1f, 1f, 1f, 0.33333334f);
					val.a *= data.ScatterRadius / 1024f;
					pixels[num3 + k] = val;
				}
			}
			texture.SetPixels(pixels, 0);
			texture.Apply(false, false);
			return texture;
		}
		return null;
	}

	private void CheckReleaseTexture()
	{
		int num = 0;
		for (int i = 0; i < entries.Count; i++)
		{
			num += (((Object)(object)entries[i].profile == (Object)null) ? 1 : 0);
		}
		if (entries.Count == num)
		{
			ReleaseTexture();
		}
	}

	private void ReleaseTexture()
	{
		if ((Object)(object)texture != (Object)null)
		{
			Object.DestroyImmediate((Object)(object)texture);
			texture = null;
		}
	}
}
