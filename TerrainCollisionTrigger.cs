using UnityEngine;

public class TerrainCollisionTrigger : EnvironmentVolumeTrigger
{
	protected void OnTriggerEnter(Collider other)
	{
		if (Object.op_Implicit((Object)(object)TerrainMeta.Collision) && !other.get_isTrigger())
		{
			UpdateCollider(other, state: true);
		}
	}

	protected void OnTriggerExit(Collider other)
	{
		if (Object.op_Implicit((Object)(object)TerrainMeta.Collision) && !other.get_isTrigger())
		{
			UpdateCollider(other, state: false);
		}
	}

	private void UpdateCollider(Collider other, bool state)
	{
		TerrainMeta.Collision.SetIgnore(other, base.volume.trigger, state);
		TerrainCollisionProxy component = ((Component)other).GetComponent<TerrainCollisionProxy>();
		if (Object.op_Implicit((Object)(object)component))
		{
			for (int i = 0; i < component.colliders.Length; i++)
			{
				TerrainMeta.Collision.SetIgnore((Collider)(object)component.colliders[i], base.volume.trigger, state);
			}
		}
	}
}
