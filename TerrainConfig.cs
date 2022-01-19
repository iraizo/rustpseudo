using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Rust/Terrain Config")]
public class TerrainConfig : ScriptableObject
{
	[Serializable]
	public class SplatOverlay
	{
		public Color Color = new Color(1f, 1f, 1f, 0f);

		[Range(0f, 1f)]
		public float Smoothness;

		[Range(0f, 1f)]
		public float NormalIntensity = 1f;

		[Range(0f, 8f)]
		public float BlendFactor = 0.5f;

		[Range(0.01f, 32f)]
		public float BlendFalloff = 0.5f;
	}

	[Serializable]
	public class SplatType
	{
		public string Name = "";

		[FormerlySerializedAs("WarmColor")]
		public Color AridColor = Color.get_white();

		public SplatOverlay AridOverlay = new SplatOverlay();

		[FormerlySerializedAs("Color")]
		public Color TemperateColor = Color.get_white();

		public SplatOverlay TemperateOverlay = new SplatOverlay();

		[FormerlySerializedAs("ColdColor")]
		public Color TundraColor = Color.get_white();

		public SplatOverlay TundraOverlay = new SplatOverlay();

		[FormerlySerializedAs("ColdColor")]
		public Color ArcticColor = Color.get_white();

		public SplatOverlay ArcticOverlay = new SplatOverlay();

		public PhysicMaterial Material;

		public float SplatTiling = 5f;

		[Range(0f, 1f)]
		public float UVMIXMult = 0.15f;

		public float UVMIXStart;

		public float UVMIXDist = 100f;
	}

	public bool CastShadows = true;

	public LayerMask GroundMask = LayerMask.op_Implicit(0);

	public LayerMask WaterMask = LayerMask.op_Implicit(0);

	public PhysicMaterial GenericMaterial;

	public Material Material;

	public Material MarginMaterial;

	public Texture[] AlbedoArrays = (Texture[])(object)new Texture[3];

	public Texture[] NormalArrays = (Texture[])(object)new Texture[3];

	public float HeightMapErrorMin = 5f;

	public float HeightMapErrorMax = 100f;

	public float BaseMapDistanceMin = 100f;

	public float BaseMapDistanceMax = 500f;

	public float ShaderLodMin = 100f;

	public float ShaderLodMax = 600f;

	public SplatType[] Splats = new SplatType[8];

	public Texture AlbedoArray => AlbedoArrays[Mathf.Clamp(QualitySettings.get_masterTextureLimit(), 0, 2)];

	public Texture NormalArray => NormalArrays[Mathf.Clamp(QualitySettings.get_masterTextureLimit(), 0, 2)];

	public PhysicMaterial[] GetPhysicMaterials()
	{
		PhysicMaterial[] array = (PhysicMaterial[])(object)new PhysicMaterial[Splats.Length];
		for (int i = 0; i < Splats.Length; i++)
		{
			array[i] = Splats[i].Material;
		}
		return array;
	}

	public Color[] GetAridColors()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Color[] array = (Color[])(object)new Color[Splats.Length];
		for (int i = 0; i < Splats.Length; i++)
		{
			array[i] = Splats[i].AridColor;
		}
		return array;
	}

	public void GetAridOverlayConstants(out Color[] color, out Vector4[] param)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		color = (Color[])(object)new Color[Splats.Length];
		param = (Vector4[])(object)new Vector4[Splats.Length];
		for (int i = 0; i < Splats.Length; i++)
		{
			SplatOverlay aridOverlay = Splats[i].AridOverlay;
			color[i] = ((Color)(ref aridOverlay.Color)).get_linear();
			param[i] = new Vector4(aridOverlay.Smoothness, aridOverlay.NormalIntensity, aridOverlay.BlendFactor, aridOverlay.BlendFalloff);
		}
	}

	public Color[] GetTemperateColors()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Color[] array = (Color[])(object)new Color[Splats.Length];
		for (int i = 0; i < Splats.Length; i++)
		{
			array[i] = Splats[i].TemperateColor;
		}
		return array;
	}

	public void GetTemperateOverlayConstants(out Color[] color, out Vector4[] param)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		color = (Color[])(object)new Color[Splats.Length];
		param = (Vector4[])(object)new Vector4[Splats.Length];
		for (int i = 0; i < Splats.Length; i++)
		{
			SplatOverlay temperateOverlay = Splats[i].TemperateOverlay;
			color[i] = ((Color)(ref temperateOverlay.Color)).get_linear();
			param[i] = new Vector4(temperateOverlay.Smoothness, temperateOverlay.NormalIntensity, temperateOverlay.BlendFactor, temperateOverlay.BlendFalloff);
		}
	}

	public Color[] GetTundraColors()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Color[] array = (Color[])(object)new Color[Splats.Length];
		for (int i = 0; i < Splats.Length; i++)
		{
			array[i] = Splats[i].TundraColor;
		}
		return array;
	}

	public void GetTundraOverlayConstants(out Color[] color, out Vector4[] param)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		color = (Color[])(object)new Color[Splats.Length];
		param = (Vector4[])(object)new Vector4[Splats.Length];
		for (int i = 0; i < Splats.Length; i++)
		{
			SplatOverlay tundraOverlay = Splats[i].TundraOverlay;
			color[i] = ((Color)(ref tundraOverlay.Color)).get_linear();
			param[i] = new Vector4(tundraOverlay.Smoothness, tundraOverlay.NormalIntensity, tundraOverlay.BlendFactor, tundraOverlay.BlendFalloff);
		}
	}

	public Color[] GetArcticColors()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Color[] array = (Color[])(object)new Color[Splats.Length];
		for (int i = 0; i < Splats.Length; i++)
		{
			array[i] = Splats[i].ArcticColor;
		}
		return array;
	}

	public void GetArcticOverlayConstants(out Color[] color, out Vector4[] param)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		color = (Color[])(object)new Color[Splats.Length];
		param = (Vector4[])(object)new Vector4[Splats.Length];
		for (int i = 0; i < Splats.Length; i++)
		{
			SplatOverlay arcticOverlay = Splats[i].ArcticOverlay;
			color[i] = ((Color)(ref arcticOverlay.Color)).get_linear();
			param[i] = new Vector4(arcticOverlay.Smoothness, arcticOverlay.NormalIntensity, arcticOverlay.BlendFactor, arcticOverlay.BlendFalloff);
		}
	}

	public float[] GetSplatTiling()
	{
		float[] array = new float[Splats.Length];
		for (int i = 0; i < Splats.Length; i++)
		{
			array[i] = Splats[i].SplatTiling;
		}
		return array;
	}

	public float GetMaxSplatTiling()
	{
		float num = float.MinValue;
		for (int i = 0; i < Splats.Length; i++)
		{
			if (Splats[i].SplatTiling > num)
			{
				num = Splats[i].SplatTiling;
			}
		}
		return num;
	}

	public float GetMinSplatTiling()
	{
		float num = float.MaxValue;
		for (int i = 0; i < Splats.Length; i++)
		{
			if (Splats[i].SplatTiling < num)
			{
				num = Splats[i].SplatTiling;
			}
		}
		return num;
	}

	public Vector3[] GetPackedUVMIX()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		Vector3[] array = (Vector3[])(object)new Vector3[Splats.Length];
		for (int i = 0; i < Splats.Length; i++)
		{
			array[i] = new Vector3(Splats[i].UVMIXMult, Splats[i].UVMIXStart, Splats[i].UVMIXDist);
		}
		return array;
	}

	public TerrainConfig()
		: this()
	{
	}//IL_0009: Unknown result type (might be due to invalid IL or missing references)
	//IL_000e: Unknown result type (might be due to invalid IL or missing references)
	//IL_0015: Unknown result type (might be due to invalid IL or missing references)
	//IL_001a: Unknown result type (might be due to invalid IL or missing references)

}
