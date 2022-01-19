using UnityEngine;

public abstract class BaseSpawnPoint : MonoBehaviour, IServerComponent
{
	public abstract void GetLocation(out Vector3 pos, out Quaternion rot);

	public abstract void ObjectSpawned(SpawnPointInstance instance);

	public abstract void ObjectRetired(SpawnPointInstance instance);

	public virtual bool IsAvailableTo(GameObjectRef prefabRef)
	{
		return ((Component)this).get_gameObject().get_activeSelf();
	}

	protected void DropToGround(ref Vector3 pos, ref Quaternion rot)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)TerrainMeta.HeightMap) && Object.op_Implicit((Object)(object)TerrainMeta.Collision) && !TerrainMeta.Collision.GetIgnore(pos))
		{
			float height = TerrainMeta.HeightMap.GetHeight(pos);
			pos.y = Mathf.Max(pos.y, height);
		}
		if (TransformUtil.GetGroundInfo(pos, out var hitOut, 20f, LayerMask.op_Implicit(1235288065)))
		{
			pos = ((RaycastHit)(ref hitOut)).get_point();
			rot = Quaternion.LookRotation(rot * Vector3.get_forward(), ((RaycastHit)(ref hitOut)).get_normal());
		}
	}

	protected BaseSpawnPoint()
		: this()
	{
	}
}
