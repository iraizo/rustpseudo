using UnityEngine;

public class AddToWaterMap : ProceduralObject
{
	public override void Process()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		Collider component = ((Component)this).GetComponent<Collider>();
		Bounds bounds = component.get_bounds();
		int num = TerrainMeta.WaterMap.Index(TerrainMeta.NormalizeX(((Bounds)(ref bounds)).get_min().x));
		int num2 = TerrainMeta.WaterMap.Index(TerrainMeta.NormalizeZ(((Bounds)(ref bounds)).get_max().x));
		int num3 = TerrainMeta.WaterMap.Index(TerrainMeta.NormalizeX(((Bounds)(ref bounds)).get_min().z));
		int num4 = TerrainMeta.WaterMap.Index(TerrainMeta.NormalizeZ(((Bounds)(ref bounds)).get_max().z));
		if (component is BoxCollider && ((Component)this).get_transform().get_rotation() == Quaternion.get_identity())
		{
			float num5 = TerrainMeta.NormalizeY(((Bounds)(ref bounds)).get_max().y);
			for (int i = num3; i <= num4; i++)
			{
				for (int j = num; j <= num2; j++)
				{
					float height = TerrainMeta.WaterMap.GetHeight01(j, i);
					if (num5 > height)
					{
						TerrainMeta.WaterMap.SetHeight(j, i, num5);
					}
				}
			}
		}
		else
		{
			Vector3 val = default(Vector3);
			Ray val2 = default(Ray);
			RaycastHit val3 = default(RaycastHit);
			for (int k = num3; k <= num4; k++)
			{
				float normZ = TerrainMeta.WaterMap.Coordinate(k);
				for (int l = num; l <= num2; l++)
				{
					float normX = TerrainMeta.WaterMap.Coordinate(l);
					((Vector3)(ref val))._002Ector(TerrainMeta.DenormalizeX(normX), ((Bounds)(ref bounds)).get_max().y + 1f, TerrainMeta.DenormalizeZ(normZ));
					((Ray)(ref val2))._002Ector(val, Vector3.get_down());
					if (component.Raycast(val2, ref val3, ((Bounds)(ref bounds)).get_size().y + 1f + 1f))
					{
						float num6 = TerrainMeta.NormalizeY(((RaycastHit)(ref val3)).get_point().y);
						float height2 = TerrainMeta.WaterMap.GetHeight01(l, k);
						if (num6 > height2)
						{
							TerrainMeta.WaterMap.SetHeight(l, k, num6);
						}
					}
				}
			}
		}
		GameManager.Destroy((Component)(object)this);
	}
}
