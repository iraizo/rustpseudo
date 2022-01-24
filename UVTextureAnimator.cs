using System;
using System.Collections;
using UnityEngine;

internal class UVTextureAnimator : MonoBehaviour
{
	public int Rows = 4;

	public int Columns = 4;

	public float Fps = 20f;

	public int OffsetMat;

	public bool IsLoop = true;

	public float StartDelay;

	private bool isInizialised;

	private int index;

	private int count;

	private int allCount;

	private float deltaFps;

	private bool isVisible;

	private bool isCorutineStarted;

	private Renderer currentRenderer;

	private Material instanceMaterial;

	private void Start()
	{
		currentRenderer = ((Component)this).GetComponent<Renderer>();
		InitDefaultVariables();
		isInizialised = true;
		isVisible = true;
		Play();
	}

	private void InitDefaultVariables()
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		currentRenderer = ((Component)this).GetComponent<Renderer>();
		if ((Object)(object)currentRenderer == (Object)null)
		{
			throw new Exception("UvTextureAnimator can't get renderer");
		}
		if (!currentRenderer.get_enabled())
		{
			currentRenderer.set_enabled(true);
		}
		allCount = 0;
		deltaFps = 1f / Fps;
		count = Rows * Columns;
		index = Columns - 1;
		Vector3 zero = Vector3.get_zero();
		OffsetMat -= OffsetMat / count * count;
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(1f / (float)Columns, 1f / (float)Rows);
		if ((Object)(object)currentRenderer != (Object)null)
		{
			instanceMaterial = currentRenderer.get_material();
			instanceMaterial.SetTextureScale("_MainTex", val);
			instanceMaterial.SetTextureOffset("_MainTex", Vector2.op_Implicit(zero));
		}
	}

	private void Play()
	{
		if (!isCorutineStarted)
		{
			if (StartDelay > 0.0001f)
			{
				((MonoBehaviour)this).Invoke("PlayDelay", StartDelay);
			}
			else
			{
				((MonoBehaviour)this).StartCoroutine(UpdateCorutine());
			}
			isCorutineStarted = true;
		}
	}

	private void PlayDelay()
	{
		((MonoBehaviour)this).StartCoroutine(UpdateCorutine());
	}

	private void OnEnable()
	{
		if (isInizialised)
		{
			InitDefaultVariables();
			isVisible = true;
			Play();
		}
	}

	private void OnDisable()
	{
		isCorutineStarted = false;
		isVisible = false;
		((MonoBehaviour)this).StopAllCoroutines();
		((MonoBehaviour)this).CancelInvoke("PlayDelay");
	}

	private IEnumerator UpdateCorutine()
	{
		while (isVisible && (IsLoop || allCount != count))
		{
			UpdateCorutineFrame();
			if (!IsLoop && allCount == count)
			{
				break;
			}
			yield return (object)new WaitForSeconds(deltaFps);
		}
		isCorutineStarted = false;
		currentRenderer.set_enabled(false);
	}

	private void UpdateCorutineFrame()
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		allCount++;
		index++;
		if (index >= count)
		{
			index = 0;
		}
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector((float)index / (float)Columns - (float)(index / Columns), 1f - (float)(index / Columns) / (float)Rows);
		if ((Object)(object)currentRenderer != (Object)null)
		{
			instanceMaterial.SetTextureOffset("_MainTex", val);
		}
	}

	private void OnDestroy()
	{
		if ((Object)(object)instanceMaterial != (Object)null)
		{
			Object.Destroy((Object)(object)instanceMaterial);
			instanceMaterial = null;
		}
	}

	public UVTextureAnimator()
		: this()
	{
	}
}
