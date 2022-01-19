using System.Collections.Generic;
using UnityEngine;

public class CH47PathFinder : BasePathFinder
{
	public List<Vector3> visitedPatrolPoints = new List<Vector3>();

	public override Vector3 GetRandomPatrolPoint()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.get_zero();
		MonumentInfo monumentInfo = null;
		if ((Object)(object)TerrainMeta.Path != (Object)null && TerrainMeta.Path.Monuments != null && TerrainMeta.Path.Monuments.Count > 0)
		{
			int count = TerrainMeta.Path.Monuments.Count;
			int num = Random.Range(0, count);
			for (int i = 0; i < count; i++)
			{
				int num2 = i + num;
				if (num2 >= count)
				{
					num2 -= count;
				}
				MonumentInfo monumentInfo2 = TerrainMeta.Path.Monuments[num2];
				if (monumentInfo2.Type == MonumentType.Cave || monumentInfo2.Type == MonumentType.WaterWell || monumentInfo2.Tier == MonumentTier.Tier0 || (monumentInfo2.Tier & MonumentTier.Tier0) > (MonumentTier)0)
				{
					continue;
				}
				bool flag = false;
				foreach (Vector3 visitedPatrolPoint in visitedPatrolPoints)
				{
					if (Vector3Ex.Distance2D(((Component)monumentInfo2).get_transform().get_position(), visitedPatrolPoint) < 100f)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					monumentInfo = monumentInfo2;
					break;
				}
			}
			if ((Object)(object)monumentInfo == (Object)null)
			{
				visitedPatrolPoints.Clear();
				monumentInfo = GetRandomValidMonumentInfo();
			}
		}
		if ((Object)(object)monumentInfo != (Object)null)
		{
			visitedPatrolPoints.Add(((Component)monumentInfo).get_transform().get_position());
			val = ((Component)monumentInfo).get_transform().get_position();
		}
		else
		{
			float x = TerrainMeta.Size.x;
			float y = 30f;
			val = Vector3Ex.Range(-1f, 1f);
			val.y = 0f;
			((Vector3)(ref val)).Normalize();
			val *= x * Random.Range(0f, 0.75f);
			val.y = y;
		}
		float num3 = Mathf.Max(TerrainMeta.WaterMap.GetHeight(val), TerrainMeta.HeightMap.GetHeight(val));
		float num4 = num3;
		RaycastHit val2 = default(RaycastHit);
		if (Physics.SphereCast(val + new Vector3(0f, 200f, 0f), 20f, Vector3.get_down(), ref val2, 300f, 1218511105))
		{
			num4 = Mathf.Max(((RaycastHit)(ref val2)).get_point().y, num3);
		}
		val.y = num4 + 30f;
		return val;
	}

	private MonumentInfo GetRandomValidMonumentInfo()
	{
		int count = TerrainMeta.Path.Monuments.Count;
		int num = Random.Range(0, count);
		for (int i = 0; i < count; i++)
		{
			int num2 = i + num;
			if (num2 >= count)
			{
				num2 -= count;
			}
			MonumentInfo monumentInfo = TerrainMeta.Path.Monuments[num2];
			if (monumentInfo.Type != 0 && monumentInfo.Type != MonumentType.WaterWell && monumentInfo.Tier != MonumentTier.Tier0)
			{
				return monumentInfo;
			}
		}
		return null;
	}
}
