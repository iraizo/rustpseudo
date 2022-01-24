using UnityEngine;

public class ScaleRenderer : MonoBehaviour
{
	public bool useRandomScale;

	public float scaleMin = 1f;

	public float scaleMax = 1f;

	private float lastScale = -1f;

	protected bool hasInitialValues;

	public Renderer myRenderer;

	private bool ScaleDifferent(float newScale)
	{
		return newScale != lastScale;
	}

	public void Start()
	{
		if (useRandomScale)
		{
			SetScale(Random.Range(scaleMin, scaleMax));
		}
	}

	public void SetScale(float scale)
	{
		if (!hasInitialValues)
		{
			GatherInitialValues();
		}
		if (ScaleDifferent(scale) || (scale > 0f && !myRenderer.get_enabled()))
		{
			SetRendererEnabled(scale != 0f);
			SetScale_Internal(scale);
		}
	}

	public virtual void SetScale_Internal(float scale)
	{
		lastScale = scale;
	}

	public virtual void SetRendererEnabled(bool isEnabled)
	{
		if (Object.op_Implicit((Object)(object)myRenderer) && myRenderer.get_enabled() != isEnabled)
		{
			myRenderer.set_enabled(isEnabled);
		}
	}

	public virtual void GatherInitialValues()
	{
		hasInitialValues = true;
	}

	public ScaleRenderer()
		: this()
	{
	}
}
