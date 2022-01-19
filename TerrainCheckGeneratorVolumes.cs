using UnityEngine;

public class TerrainCheckGeneratorVolumes : MonoBehaviour, IEditorComponent
{
	public float PlacementRadius;

	protected void OnDrawGizmosSelected()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.set_color(new Color(0.5f, 0.5f, 0.5f, 1f));
		GizmosUtil.DrawWireCircleY(((Component)this).get_transform().get_position(), PlacementRadius);
	}

	public TerrainCheckGeneratorVolumes()
		: this()
	{
	}
}
