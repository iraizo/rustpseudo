using UnityEngine;

public class SpaceCheckingSpawnPoint : GenericSpawnPoint
{
	public bool useCustomBoundsCheckMask;

	public LayerMask customBoundsCheckMask;

	public float customBoundsCheckScale = 1f;

	public override bool IsAvailableTo(GameObjectRef prefabRef)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		if (!base.IsAvailableTo(prefabRef))
		{
			return false;
		}
		if (useCustomBoundsCheckMask)
		{
			return SpawnHandler.CheckBounds(prefabRef.Get(), ((Component)this).get_transform().get_position(), ((Component)this).get_transform().get_rotation(), Vector3.get_one() * customBoundsCheckScale, customBoundsCheckMask);
		}
		return SingletonComponent<SpawnHandler>.Instance.CheckBounds(prefabRef.Get(), ((Component)this).get_transform().get_position(), ((Component)this).get_transform().get_rotation(), Vector3.get_one() * customBoundsCheckScale);
	}
}
