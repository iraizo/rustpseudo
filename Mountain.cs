using UnityEngine;

public class Mountain : TerrainPlacement
{
	public float Fade = 10f;

	protected void OnDrawGizmosSelected()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.get_up() * (0.5f * Fade);
		Gizmos.set_color(new Color(0.5f, 0.5f, 0.5f, 1f));
		Gizmos.DrawCube(((Component)this).get_transform().get_position() + val, new Vector3(size.x, Fade, size.z));
		Gizmos.DrawWireCube(((Component)this).get_transform().get_position() + val, new Vector3(size.x, Fade, size.z));
	}

	protected override void ApplyHeight(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(Vector3.get_zero());
		TextureData heightdata = new TextureData(heightmap.Get());
		Vector3 v = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(0f - extents.x, 0f, 0f - extents.z));
		Vector3 v2 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(extents.x, 0f, 0f - extents.z));
		Vector3 v3 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(0f - extents.x, 0f, extents.z));
		Vector3 v4 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(extents.x, 0f, extents.z));
		TerrainMeta.HeightMap.ForEachParallel(v, v2, v3, v4, delegate(int x, int z)
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			float normZ = TerrainMeta.HeightMap.Coordinate(z);
			float normX = TerrainMeta.HeightMap.Coordinate(x);
			Vector3 val = default(Vector3);
			((Vector3)(ref val))._002Ector(TerrainMeta.DenormalizeX(normX), 0f, TerrainMeta.DenormalizeZ(normZ));
			Vector3 val2 = ((Matrix4x4)(ref worldToLocal)).MultiplyPoint3x4(val) - offset;
			float num = position.y + offset.y + heightdata.GetInterpolatedHalf((val2.x + extents.x) / size.x, (val2.z + extents.z) / size.z) * size.y;
			float num2 = Mathf.InverseLerp(position.y, position.y + Fade, num);
			if (num2 != 0f)
			{
				float num3 = TerrainMeta.NormalizeY(num);
				num3 = Mathx.SmoothMax(TerrainMeta.HeightMap.GetHeight01(x, z), num3, 0.1f);
				TerrainMeta.HeightMap.SetHeight(x, z, num3, num2);
			}
		});
	}

	protected override void ApplySplat(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		bool should0 = ShouldSplat(1);
		bool should1 = ShouldSplat(2);
		bool should2 = ShouldSplat(4);
		bool should3 = ShouldSplat(8);
		bool should4 = ShouldSplat(16);
		bool should5 = ShouldSplat(32);
		bool should6 = ShouldSplat(64);
		bool should7 = ShouldSplat(128);
		if (!should0 && !should1 && !should2 && !should3 && !should4 && !should5 && !should6 && !should7)
		{
			return;
		}
		Vector3 position = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(Vector3.get_zero());
		TextureData heightdata = new TextureData(heightmap.Get());
		TextureData splat0data = new TextureData(splatmap0.Get());
		TextureData splat1data = new TextureData(splatmap1.Get());
		Vector3 v = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(0f - extents.x, 0f, 0f - extents.z));
		Vector3 v2 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(extents.x, 0f, 0f - extents.z));
		Vector3 v3 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(0f - extents.x, 0f, extents.z));
		Vector3 v4 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(extents.x, 0f, extents.z));
		TerrainMeta.SplatMap.ForEachParallel(v, v2, v3, v4, delegate(int x, int z)
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			float normZ = TerrainMeta.SplatMap.Coordinate(z);
			float normX = TerrainMeta.SplatMap.Coordinate(x);
			Vector3 val = default(Vector3);
			((Vector3)(ref val))._002Ector(TerrainMeta.DenormalizeX(normX), 0f, TerrainMeta.DenormalizeZ(normZ));
			Vector3 val2 = ((Matrix4x4)(ref worldToLocal)).MultiplyPoint3x4(val) - offset;
			float num = position.y + offset.y + heightdata.GetInterpolatedHalf((val2.x + extents.x) / size.x, (val2.z + extents.z) / size.z) * size.y;
			float num2 = Mathf.InverseLerp(position.y, position.y + Fade, num);
			if (num2 != 0f)
			{
				Vector4 interpolatedVector = splat0data.GetInterpolatedVector((val2.x + extents.x) / size.x, (val2.z + extents.z) / size.z);
				Vector4 interpolatedVector2 = splat1data.GetInterpolatedVector((val2.x + extents.x) / size.x, (val2.z + extents.z) / size.z);
				if (!should0)
				{
					interpolatedVector.x = 0f;
				}
				if (!should1)
				{
					interpolatedVector.y = 0f;
				}
				if (!should2)
				{
					interpolatedVector.z = 0f;
				}
				if (!should3)
				{
					interpolatedVector.w = 0f;
				}
				if (!should4)
				{
					interpolatedVector2.x = 0f;
				}
				if (!should5)
				{
					interpolatedVector2.y = 0f;
				}
				if (!should6)
				{
					interpolatedVector2.z = 0f;
				}
				if (!should7)
				{
					interpolatedVector2.w = 0f;
				}
				TerrainMeta.SplatMap.SetSplatRaw(x, z, interpolatedVector, interpolatedVector2, num2);
			}
		});
	}

	protected override void ApplyAlpha(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
	}

	protected override void ApplyBiome(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		bool should0 = ShouldBiome(1);
		bool should1 = ShouldBiome(2);
		bool should2 = ShouldBiome(4);
		bool should3 = ShouldBiome(8);
		if (!should0 && !should1 && !should2 && !should3)
		{
			return;
		}
		Vector3 position = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(Vector3.get_zero());
		TextureData heightdata = new TextureData(heightmap.Get());
		TextureData biomedata = new TextureData(biomemap.Get());
		Vector3 v = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(0f - extents.x, 0f, 0f - extents.z));
		Vector3 v2 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(extents.x, 0f, 0f - extents.z));
		Vector3 v3 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(0f - extents.x, 0f, extents.z));
		Vector3 v4 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(extents.x, 0f, extents.z));
		TerrainMeta.BiomeMap.ForEachParallel(v, v2, v3, v4, delegate(int x, int z)
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			float normZ = TerrainMeta.BiomeMap.Coordinate(z);
			float normX = TerrainMeta.BiomeMap.Coordinate(x);
			Vector3 val = default(Vector3);
			((Vector3)(ref val))._002Ector(TerrainMeta.DenormalizeX(normX), 0f, TerrainMeta.DenormalizeZ(normZ));
			Vector3 val2 = ((Matrix4x4)(ref worldToLocal)).MultiplyPoint3x4(val) - offset;
			float num = position.y + offset.y + heightdata.GetInterpolatedHalf((val2.x + extents.x) / size.x, (val2.z + extents.z) / size.z) * size.y;
			float num2 = Mathf.InverseLerp(position.y, position.y + Fade, num);
			if (num2 != 0f)
			{
				Vector4 interpolatedVector = biomedata.GetInterpolatedVector((val2.x + extents.x) / size.x, (val2.z + extents.z) / size.z);
				if (!should0)
				{
					interpolatedVector.x = 0f;
				}
				if (!should1)
				{
					interpolatedVector.y = 0f;
				}
				if (!should2)
				{
					interpolatedVector.z = 0f;
				}
				if (!should3)
				{
					interpolatedVector.w = 0f;
				}
				TerrainMeta.BiomeMap.SetBiomeRaw(x, z, interpolatedVector, num2);
			}
		});
	}

	protected override void ApplyTopology(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		TextureData topologydata = new TextureData(topologymap.Get());
		Vector3 v = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(0f - extents.x, 0f, 0f - extents.z));
		Vector3 v2 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(extents.x, 0f, 0f - extents.z));
		Vector3 v3 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(0f - extents.x, 0f, extents.z));
		Vector3 v4 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(extents.x, 0f, extents.z));
		TerrainMeta.TopologyMap.ForEachParallel(v, v2, v3, v4, delegate(int x, int z)
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Expected I4, but got Unknown
			GenerateCliffTopology.Process(x, z);
			float normZ = TerrainMeta.TopologyMap.Coordinate(z);
			float normX = TerrainMeta.TopologyMap.Coordinate(x);
			Vector3 val = default(Vector3);
			((Vector3)(ref val))._002Ector(TerrainMeta.DenormalizeX(normX), 0f, TerrainMeta.DenormalizeZ(normZ));
			Vector3 val2 = ((Matrix4x4)(ref worldToLocal)).MultiplyPoint3x4(val) - offset;
			int interpolatedInt = topologydata.GetInterpolatedInt((val2.x + extents.x) / size.x, (val2.z + extents.z) / size.z);
			if (ShouldTopology(interpolatedInt))
			{
				TerrainMeta.TopologyMap.AddTopology(x, z, interpolatedInt & TopologyMask);
			}
		});
	}

	protected override void ApplyWater(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
	}
}
