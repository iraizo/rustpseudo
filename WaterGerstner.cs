using System;
using UnityEngine;

public class WaterGerstner
{
	[Serializable]
	public class WaveParams
	{
		[Range(0f, 360f)]
		public float Angle;

		[Range(0f, 0.99f)]
		public float Steepness = 0.4f;

		[Range(0.01f, 1000f)]
		public float Length = 15f;

		[Range(-10f, 10f)]
		public float Speed = 0.4f;
	}

	[Serializable]
	public class ShoreWaveParams
	{
		[Range(0f, 2f)]
		public float Steepness = 0.99f;

		[Range(0f, 1f)]
		public float Amplitude = 0.2f;

		[Range(0.01f, 1000f)]
		public float Length = 20f;

		[Range(-10f, 10f)]
		public float Speed = 0.6f;

		public float[] DirectionAngles = new float[6] { 0f, 57.3f, 114.5f, 171.9f, 229.2f, 286.5f };

		public float DirectionVarFreq = 0.1f;

		public float DirectionVarAmp = 2.5f;
	}

	public struct PrecomputedWave
	{
		public float Angle;

		public Vector2 Direction;

		public float Steepness;

		public float K;

		public float C;

		public float A;

		public static PrecomputedWave Default = new PrecomputedWave
		{
			Angle = 0f,
			Direction = Vector2.get_right(),
			Steepness = 0.4f,
			K = 1f,
			C = 1f,
			A = 1f
		};
	}

	public struct PrecomputedShoreWaves
	{
		public Vector2[] Directions;

		public float Steepness;

		public float Amplitude;

		public float K;

		public float C;

		public float A;

		public float DirectionVarFreq;

		public float DirectionVarAmp;

		public static PrecomputedShoreWaves Default = new PrecomputedShoreWaves
		{
			Directions = (Vector2[])(object)new Vector2[6]
			{
				Vector2.get_right(),
				Vector2.get_right(),
				Vector2.get_right(),
				Vector2.get_right(),
				Vector2.get_right(),
				Vector2.get_right()
			},
			Steepness = 0.75f,
			Amplitude = 0.2f,
			K = 1f,
			C = 1f,
			A = 1f,
			DirectionVarFreq = 0.1f,
			DirectionVarAmp = 3f
		};
	}

	public const int WaveCount = 6;

	public static void UpdatePrecomputedWaves(WaveParams[] waves, ref PrecomputedWave[] precomputed)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (precomputed == null || precomputed.Length != 6)
		{
			precomputed = new PrecomputedWave[6];
		}
		Debug.Assert(precomputed.Length == waves.Length);
		for (int i = 0; i < 6; i++)
		{
			float num = waves[i].Angle * ((float)Math.PI / 180f);
			precomputed[i].Angle = num;
			precomputed[i].Direction = new Vector2(Mathf.Cos(num), Mathf.Sin(num));
			precomputed[i].Steepness = waves[i].Steepness;
			precomputed[i].K = (float)Math.PI * 2f / waves[i].Length;
			precomputed[i].C = Mathf.Sqrt(9.8f / precomputed[i].K) * waves[i].Speed * WaterSystem.WaveTime;
			precomputed[i].A = waves[i].Steepness / precomputed[i].K;
		}
	}

	public static void UpdatePrecomputedShoreWaves(ShoreWaveParams shoreWaves, ref PrecomputedShoreWaves precomputed)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if (precomputed.Directions == null || precomputed.Directions.Length != 6)
		{
			precomputed.Directions = (Vector2[])(object)new Vector2[6];
		}
		Debug.Assert(precomputed.Directions.Length == shoreWaves.DirectionAngles.Length);
		for (int i = 0; i < 6; i++)
		{
			float num = shoreWaves.DirectionAngles[i] * ((float)Math.PI / 180f);
			precomputed.Directions[i] = new Vector2(Mathf.Cos(num), Mathf.Sin(num));
		}
		precomputed.Steepness = shoreWaves.Steepness;
		precomputed.Amplitude = shoreWaves.Amplitude;
		precomputed.K = (float)Math.PI * 2f / shoreWaves.Length;
		precomputed.C = Mathf.Sqrt(9.8f / precomputed.K) * shoreWaves.Speed * WaterSystem.WaveTime;
		precomputed.A = shoreWaves.Steepness / precomputed.K;
		precomputed.DirectionVarFreq = shoreWaves.DirectionVarFreq;
		precomputed.DirectionVarAmp = shoreWaves.DirectionVarAmp;
	}

	public static void UpdateWaveArray(PrecomputedWave[] precomputed, ref Vector4[] array)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if (array == null || array.Length != 6)
		{
			array = (Vector4[])(object)new Vector4[6];
		}
		Debug.Assert(array.Length == precomputed.Length);
		for (int i = 0; i < 6; i++)
		{
			array[i] = new Vector4(precomputed[i].Angle, precomputed[i].Steepness, precomputed[i].K, precomputed[i].C);
		}
	}

	public static void UpdateShoreWaveArray(PrecomputedShoreWaves precomputed, ref Vector4[] array)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		Debug.Assert(precomputed.Directions.Length == 6);
		if (array == null || array.Length != 3)
		{
			array = (Vector4[])(object)new Vector4[3];
		}
		Debug.Assert(array.Length == 3);
		Vector2[] directions = precomputed.Directions;
		array[0] = new Vector4(directions[0].x, directions[0].y, directions[1].x, directions[1].y);
		array[1] = new Vector4(directions[2].x, directions[2].y, directions[3].x, directions[3].y);
		array[2] = new Vector4(directions[4].x, directions[4].y, directions[5].x, directions[5].y);
	}

	private static void GerstnerWave(PrecomputedWave wave, Vector2 pos, Vector2 shoreVec, ref float outH)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		Vector2 direction = wave.Direction;
		float num = Mathf.Sin(wave.K * (direction.x * pos.x + direction.y * pos.y - wave.C));
		outH += wave.A * num;
	}

	private static void GerstnerWave(PrecomputedWave wave, Vector2 pos, Vector2 shoreVec, ref Vector3 outP)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		Vector2 direction = wave.Direction;
		float num = wave.K * (direction.x * pos.x + direction.y * pos.y - wave.C);
		float num2 = Mathf.Cos(num);
		float num3 = Mathf.Sin(num);
		outP.x += direction.x * wave.A * num2;
		outP.y += wave.A * num3;
		outP.z += direction.y * wave.A * num2;
	}

	private static void GerstnerShoreWave(PrecomputedShoreWaves wave, Vector2 waveDir, Vector2 pos, Vector2 shoreVec, float variation_t, ref float outH)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.Clamp01(waveDir.x * shoreVec.x + waveDir.y * shoreVec.y);
		num *= num;
		float num2 = wave.K * (waveDir.x * pos.x + waveDir.y * pos.y - wave.C + variation_t);
		Mathf.Cos(num2);
		float num3 = Mathf.Sin(num2);
		outH += wave.A * wave.Amplitude * num3 * num;
	}

	private static void GerstnerShoreWave(PrecomputedShoreWaves wave, Vector2 waveDir, Vector2 pos, Vector2 shoreVec, float variation_t, ref Vector3 outP)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.Clamp01(waveDir.x * shoreVec.x + waveDir.y * shoreVec.y);
		num *= num;
		float num2 = wave.K * (waveDir.x * pos.x + waveDir.y * pos.y - wave.C + variation_t);
		float num3 = Mathf.Cos(num2);
		float num4 = Mathf.Sin(num2);
		outP.x += waveDir.x * wave.A * num3 * num;
		outP.y += wave.A * wave.Amplitude * num4 * num;
		outP.z += waveDir.y * wave.A * num3 * num;
	}

	public static Vector3 SampleDisplacement(WaterSystem instance, Vector3 location, Vector3 shore)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		PrecomputedWave[] precomputedWaves = instance.PrecomputedWaves;
		PrecomputedShoreWaves precomputedShoreWaves = instance.PrecomputedShoreWaves;
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(location.x, location.z);
		Vector2 shoreVec = default(Vector2);
		((Vector2)(ref shoreVec))._002Ector(shore.x, shore.y);
		float num = 1f - Mathf.Clamp01(shore.z * instance.ShoreWavesRcpFadeDistance);
		float num2 = Mathf.Clamp01(shore.z * instance.TerrainRcpFadeDistance);
		float num3 = Mathf.Cos(val.x * precomputedShoreWaves.DirectionVarFreq) * precomputedShoreWaves.DirectionVarAmp;
		float num4 = Mathf.Cos(val.y * precomputedShoreWaves.DirectionVarFreq) * precomputedShoreWaves.DirectionVarAmp;
		float variation_t = num3 + num4;
		Vector3 outP = Vector3.get_zero();
		Vector3 outP2 = Vector3.get_zero();
		for (int i = 0; i < 6; i++)
		{
			GerstnerWave(precomputedWaves[i], val, shoreVec, ref outP);
			GerstnerShoreWave(precomputedShoreWaves, precomputedShoreWaves.Directions[i], val, shoreVec, variation_t, ref outP2);
		}
		return Vector3.Lerp(outP, outP2, num) * num2;
	}

	private static float SampleHeightREF(WaterSystem instance, Vector3 location, Vector3 shore)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		PrecomputedWave[] precomputedWaves = instance.PrecomputedWaves;
		PrecomputedShoreWaves precomputedShoreWaves = instance.PrecomputedShoreWaves;
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(location.x, location.z);
		Vector2 shoreVec = default(Vector2);
		((Vector2)(ref shoreVec))._002Ector(shore.x, shore.y);
		float num = 1f - Mathf.Clamp01(shore.z * instance.ShoreWavesRcpFadeDistance);
		float num2 = Mathf.Clamp01(shore.z * instance.TerrainRcpFadeDistance);
		float num3 = Mathf.Cos(val.x * precomputedShoreWaves.DirectionVarFreq) * precomputedShoreWaves.DirectionVarAmp;
		float num4 = Mathf.Cos(val.y * precomputedShoreWaves.DirectionVarFreq) * precomputedShoreWaves.DirectionVarAmp;
		float variation_t = num3 + num4;
		float outH = 0f;
		float outH2 = 0f;
		for (int i = 0; i < 6; i++)
		{
			GerstnerWave(precomputedWaves[i], val, shoreVec, ref outH);
			GerstnerShoreWave(precomputedShoreWaves, precomputedShoreWaves.Directions[i], val, shoreVec, variation_t, ref outH2);
		}
		return Mathf.Lerp(outH, outH2, num) * num2;
	}

	private static void SampleHeightArrayREF(WaterSystem instance, Vector2[] location, Vector3[] shore, float[] height)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		Debug.Assert(location.Length == height.Length);
		Vector3 location2 = default(Vector3);
		for (int i = 0; i < location.Length; i++)
		{
			((Vector3)(ref location2))._002Ector(location[i].x, 0f, location[i].y);
			height[i] = SampleHeight(instance, location2, shore[i]);
		}
	}

	public static float SampleHeight(WaterSystem instance, Vector3 location, Vector3 shore)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		PrecomputedWave[] precomputedWaves = instance.PrecomputedWaves;
		Vector2[] directions = instance.PrecomputedShoreWaves.Directions;
		Vector4 global = instance.Global0;
		Vector4 global2 = instance.Global1;
		float x = global2.x;
		float y = global2.y;
		float z = global2.z;
		float w = global2.w;
		float num = x / z;
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(location.x, location.z);
		Vector2 val2 = default(Vector2);
		((Vector2)(ref val2))._002Ector(shore.x, shore.y);
		float num2 = 1f - Mathf.Clamp01(shore.z * global.x);
		float num3 = Mathf.Clamp01(shore.z * global.y);
		float num4 = Mathf.Cos(val.x * global.z) * global.w;
		float num5 = Mathf.Cos(val.y * global.z) * global.w;
		float num6 = num4 + num5;
		float num7 = 0f;
		float num8 = 0f;
		for (int i = 0; i < 6; i++)
		{
			Vector2 direction = precomputedWaves[i].Direction;
			float c = precomputedWaves[i].C;
			float k = precomputedWaves[i].K;
			float a = precomputedWaves[i].A;
			float num9 = Mathf.Sin(k * (direction.x * val.x + direction.y * val.y - c));
			num7 += a * num9;
			Vector2 val3 = directions[i];
			float num10 = Mathf.Clamp01(val3.x * val2.x + val3.y * val2.y);
			num10 *= num10;
			float num11 = Mathf.Sin(z * (val3.x * val.x + val3.y * val.y - w + num6));
			num8 += num * y * num11 * num10;
		}
		return Mathf.Lerp(num7, num8, num2) * num3;
	}

	public static void SampleHeightArray(WaterSystem instance, Vector2[] location, Vector3[] shore, float[] height)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		Debug.Assert(location.Length == height.Length);
		PrecomputedWave[] precomputedWaves = instance.PrecomputedWaves;
		Vector2[] directions = instance.PrecomputedShoreWaves.Directions;
		Vector4 global = instance.Global0;
		Vector4 global2 = instance.Global1;
		float x = global2.x;
		float y = global2.y;
		float z = global2.z;
		float w = global2.w;
		float num = x / z;
		Vector2 val = default(Vector2);
		Vector2 val2 = default(Vector2);
		for (int i = 0; i < location.Length; i++)
		{
			((Vector2)(ref val))._002Ector(location[i].x, location[i].y);
			((Vector2)(ref val2))._002Ector(shore[i].x, shore[i].y);
			float num2 = 1f - Mathf.Clamp01(shore[i].z * global.x);
			float num3 = Mathf.Clamp01(shore[i].z * global.y);
			float num4 = Mathf.Cos(val.x * global.z) * global.w;
			float num5 = Mathf.Cos(val.y * global.z) * global.w;
			float num6 = num4 + num5;
			float num7 = 0f;
			float num8 = 0f;
			for (int j = 0; j < 6; j++)
			{
				Vector2 direction = precomputedWaves[j].Direction;
				float c = precomputedWaves[j].C;
				float k = precomputedWaves[j].K;
				float a = precomputedWaves[j].A;
				float num9 = Mathf.Sin(k * (direction.x * val.x + direction.y * val.y - c));
				num7 += a * num9;
				Vector2 val3 = directions[j];
				float num10 = Mathf.Clamp01(val3.x * val2.x + val3.y * val2.y);
				num10 *= num10;
				float num11 = Mathf.Sin(z * (val3.x * val.x + val3.y * val.y - w + num6));
				num8 += num * y * num11 * num10;
			}
			height[i] = Mathf.Lerp(num7, num8, num2) * num3;
		}
	}
}
