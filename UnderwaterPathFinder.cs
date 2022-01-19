using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public class UnderwaterPathFinder : BasePathFinder
{
	private BaseEntity npc;

	public void Init(BaseEntity npc)
	{
		this.npc = npc;
	}

	public override Vector3 GetBestRoamPosition(BaseNavigator navigator, Vector3 fallbackPos, float minRange, float maxRange)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		List<Vector3> list = Pool.GetList<Vector3>();
		float height = TerrainMeta.WaterMap.GetHeight(((Component)navigator).get_transform().get_position());
		float height2 = TerrainMeta.HeightMap.GetHeight(((Component)navigator).get_transform().get_position());
		for (int i = 0; i < 8; i++)
		{
			Vector3 pointOnCircle = BasePathFinder.GetPointOnCircle(fallbackPos, Random.Range(1f, navigator.MaxRoamDistanceFromHome), Random.Range(0f, 359f));
			pointOnCircle.y += Random.Range(-2f, 2f);
			pointOnCircle.y = Mathf.Clamp(pointOnCircle.y, height2, height);
			list.Add(pointOnCircle);
		}
		float num = -1f;
		int num2 = -1;
		for (int j = 0; j < list.Count; j++)
		{
			Vector3 val = list[j];
			if (npc.IsVisible(val))
			{
				float num3 = 0f;
				Vector3 val2 = Vector3Ex.Direction2D(val, ((Component)navigator).get_transform().get_position());
				float num4 = Vector3.Dot(((Component)navigator).get_transform().get_forward(), val2);
				num3 += Mathf.InverseLerp(0.25f, 0.8f, num4) * 5f;
				float num5 = Mathf.Abs(val.y - ((Component)navigator).get_transform().get_position().y);
				num3 += 1f - Mathf.InverseLerp(1f, 3f, num5) * 5f;
				if (num3 > num || num2 == -1)
				{
					num = num3;
					num2 = j;
				}
			}
		}
		Vector3 result = list[num2];
		Pool.FreeList<Vector3>(ref list);
		return result;
	}

	public override bool GetBestFleePosition(BaseNavigator navigator, AIBrainSenses senses, BaseEntity fleeFrom, Vector3 fallbackPos, float minRange, float maxRange, out Vector3 result)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)fleeFrom == (Object)null)
		{
			result = ((Component)navigator).get_transform().get_position();
			return false;
		}
		Vector3 val = Vector3Ex.Direction2D(((Component)navigator).get_transform().get_position(), ((Component)fleeFrom).get_transform().get_position());
		result = ((Component)navigator).get_transform().get_position() + val * Random.Range(minRange, maxRange);
		return true;
	}
}
