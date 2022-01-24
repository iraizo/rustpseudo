using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public class DeployVolumeEntityBoundsReverse : DeployVolume
{
	private Bounds bounds = new Bounds(Vector3.get_zero(), Vector3.get_one());

	private int layer;

	protected override bool Check(Vector3 position, Quaternion rotation, int mask = -1)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		position += rotation * ((Bounds)(ref bounds)).get_center();
		OBB test = default(OBB);
		((OBB)(ref test))._002Ector(position, ((Bounds)(ref bounds)).get_size(), rotation);
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities(position, ((Vector3)(ref test.extents)).get_magnitude(), list, LayerMask.op_Implicit(layers) & mask, (QueryTriggerInteraction)2);
		foreach (BaseEntity item in list)
		{
			DeployVolume[] volumes = PrefabAttribute.server.FindAll<DeployVolume>(item.prefabID);
			if (DeployVolume.Check(((Component)item).get_transform().get_position(), ((Component)item).get_transform().get_rotation(), volumes, test, 1 << layer))
			{
				Pool.FreeList<BaseEntity>(ref list);
				return true;
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
		return false;
	}

	protected override bool Check(Vector3 position, Quaternion rotation, OBB test, int mask = -1)
	{
		return false;
	}

	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		bounds = rootObj.GetComponent<BaseEntity>().bounds;
		layer = rootObj.get_layer();
	}
}
