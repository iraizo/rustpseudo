using UnityEngine;

public class RadialSpawnPoint : BaseSpawnPoint
{
	public float radius = 10f;

	public override void GetLocation(out Vector3 pos, out Quaternion rot)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = Random.get_insideUnitCircle() * radius;
		pos = ((Component)this).get_transform().get_position() + new Vector3(val.x, 0f, val.y);
		rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
		DropToGround(ref pos, ref rot);
	}

	public override void ObjectSpawned(SpawnPointInstance instance)
	{
	}

	public override void ObjectRetired(SpawnPointInstance instance)
	{
	}
}
