using UnityEngine;

public class AddToHeightMap : ProceduralObject
{
	public bool DestroyGameObject;

	public override void Process()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		Collider component = ((Component)this).GetComponent<Collider>();
		Bounds bounds = component.get_bounds();
		int num = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeX(((Bounds)(ref bounds)).get_min().x));
		int num2 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeZ(((Bounds)(ref bounds)).get_max().x));
		int num3 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeX(((Bounds)(ref bounds)).get_min().z));
		int num4 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeZ(((Bounds)(ref bounds)).get_max().z));
		Vector3 val = default(Vector3);
		Ray val2 = default(Ray);
		RaycastHit val3 = default(RaycastHit);
		for (int i = num3; i <= num4; i++)
		{
			float normZ = TerrainMeta.HeightMap.Coordinate(i);
			for (int j = num; j <= num2; j++)
			{
				float normX = TerrainMeta.HeightMap.Coordinate(j);
				((Vector3)(ref val))._002Ector(TerrainMeta.DenormalizeX(normX), ((Bounds)(ref bounds)).get_max().y, TerrainMeta.DenormalizeZ(normZ));
				((Ray)(ref val2))._002Ector(val, Vector3.get_down());
				if (component.Raycast(val2, ref val3, ((Bounds)(ref bounds)).get_size().y))
				{
					float num5 = TerrainMeta.NormalizeY(((RaycastHit)(ref val3)).get_point().y);
					float height = TerrainMeta.HeightMap.GetHeight01(j, i);
					if (num5 > height)
					{
						TerrainMeta.HeightMap.SetHeight(j, i, num5);
					}
				}
			}
		}
		if (DestroyGameObject)
		{
			GameManager.Destroy(((Component)this).get_gameObject());
		}
		else
		{
			GameManager.Destroy((Component)(object)this);
		}
	}
}
