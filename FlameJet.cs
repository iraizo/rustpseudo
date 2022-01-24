using UnityEngine;

public class FlameJet : MonoBehaviour
{
	public LineRenderer line;

	public float tesselation = 0.025f;

	private float length;

	public float maxLength = 2f;

	public float drag;

	private int numSegments;

	private float spacing;

	public bool on;

	private Vector3[] lastWorldSegments;

	private Vector3[] currentSegments = (Vector3[])(object)new Vector3[0];

	public Color startColor;

	public Color endColor;

	public Color currentColor;

	private void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		currentColor = startColor;
		tesselation = 0.1f;
		numSegments = Mathf.CeilToInt(maxLength / tesselation);
		spacing = maxLength / (float)numSegments;
		if (currentSegments.Length != numSegments)
		{
			currentSegments = (Vector3[])(object)new Vector3[numSegments];
		}
	}

	private void Awake()
	{
		Initialize();
	}

	public void LateUpdate()
	{
		UpdateLine();
	}

	public void SetOn(bool isOn)
	{
		on = isOn;
	}

	private float curve(float x)
	{
		return x * x;
	}

	private void UpdateLine()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		currentColor.a = Mathf.Lerp(currentColor.a, on ? 1f : 0f, Time.get_deltaTime() * 40f);
		line.SetColors(currentColor, endColor);
		Vector3 val3 = default(Vector3);
		for (int i = 0; i < currentSegments.Length; i++)
		{
			float num = 0f;
			float num2 = 0f;
			if (lastWorldSegments != null && lastWorldSegments[i] != Vector3.get_zero())
			{
				Vector3 val = ((Component)this).get_transform().InverseTransformPoint(lastWorldSegments[i]);
				float num3 = (float)i / (float)currentSegments.Length;
				Vector3 val2 = Vector3.Lerp(val, Vector3.get_zero(), Time.get_deltaTime() * drag);
				val2 = Vector3.Lerp(Vector3.get_zero(), val2, Mathf.Sqrt(num3));
				num = val2.x;
				num2 = val2.y;
			}
			if (i == 0)
			{
				num = (num2 = 0f);
			}
			((Vector3)(ref val3))._002Ector(num, num2, (float)i * spacing);
			currentSegments[i] = val3;
			if (lastWorldSegments == null)
			{
				lastWorldSegments = (Vector3[])(object)new Vector3[numSegments];
			}
			lastWorldSegments[i] = ((Component)this).get_transform().TransformPoint(val3);
		}
		line.SetVertexCount(numSegments);
		line.SetPositions(currentSegments);
	}

	public FlameJet()
		: this()
	{
	}
}
