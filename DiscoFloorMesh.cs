using UnityEngine;

public class DiscoFloorMesh : MonoBehaviour, IClientComponent
{
	public int GridRows = 5;

	public int GridColumns = 5;

	public float GridSize = 1f;

	[Range(0f, 10f)]
	public float TestOffset;

	public Color OffColor = Color.get_grey();

	public MeshRenderer Renderer;

	public bool DrawInEditor;

	public MeshFilter Filter;

	public AnimationCurve customCurveX = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public AnimationCurve customCurveY = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public DiscoFloorMesh()
		: this()
	{
	}//IL_001a: Unknown result type (might be due to invalid IL or missing references)
	//IL_001f: Unknown result type (might be due to invalid IL or missing references)

}
