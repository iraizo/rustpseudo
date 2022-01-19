using UnityEngine;

public class PathSpeedZone : MonoBehaviour
{
	public Bounds bounds;

	public OBB obbBounds;

	public float maxVelocityPerSec = 5f;

	public OBB WorldSpaceBounds()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		return new OBB(((Component)this).get_transform().get_position(), ((Component)this).get_transform().get_lossyScale(), ((Component)this).get_transform().get_rotation(), bounds);
	}

	public float GetMaxSpeed()
	{
		return maxVelocityPerSec;
	}

	public virtual void OnDrawGizmosSelected()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.set_matrix(((Component)this).get_transform().get_localToWorldMatrix());
		Gizmos.set_color(new Color(1f, 0.5f, 0f, 0.5f));
		Gizmos.DrawCube(((Bounds)(ref bounds)).get_center(), ((Bounds)(ref bounds)).get_size());
		Gizmos.set_color(new Color(1f, 0.7f, 0f, 1f));
		Gizmos.DrawWireCube(((Bounds)(ref bounds)).get_center(), ((Bounds)(ref bounds)).get_size());
	}

	public PathSpeedZone()
		: this()
	{
	}
}
