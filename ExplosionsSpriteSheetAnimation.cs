using System;
using System.Collections;
using UnityEngine;

internal class ExplosionsSpriteSheetAnimation : MonoBehaviour
{
	public int TilesX = 4;

	public int TilesY = 4;

	public float AnimationFPS = 30f;

	public bool IsInterpolateFrames;

	public int StartFrameOffset;

	public bool IsLoop = true;

	public float StartDelay;

	public AnimationCurve FrameOverTime = AnimationCurve.Linear(0f, 1f, 1f, 1f);

	private bool isInizialised;

	private int index;

	private int count;

	private int allCount;

	private float animationLifeTime;

	private bool isVisible;

	private bool isCorutineStarted;

	private Renderer currentRenderer;

	private Material instanceMaterial;

	private float currentInterpolatedTime;

	private float animationStartTime;

	private bool animationStoped;

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
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
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
		animationStoped = false;
		animationLifeTime = (float)(TilesX * TilesY) / AnimationFPS;
		count = TilesY * TilesX;
		index = TilesX - 1;
		Vector3 zero = Vector3.get_zero();
		StartFrameOffset -= StartFrameOffset / count * count;
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(1f / (float)TilesX, 1f / (float)TilesY);
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
		animationStartTime = Time.get_time();
		while (isVisible && (IsLoop || !animationStoped))
		{
			UpdateFrame();
			if (!IsLoop && animationStoped)
			{
				break;
			}
			float num = (Time.get_time() - animationStartTime) / animationLifeTime;
			float num2 = FrameOverTime.Evaluate(Mathf.Clamp01(num));
			yield return (object)new WaitForSeconds(1f / (AnimationFPS * num2));
		}
		isCorutineStarted = false;
		currentRenderer.set_enabled(false);
	}

	private void UpdateFrame()
	{
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		allCount++;
		index++;
		if (index >= count)
		{
			index = 0;
		}
		if (count == allCount)
		{
			animationStartTime = Time.get_time();
			allCount = 0;
			animationStoped = true;
		}
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector((float)index / (float)TilesX - (float)(index / TilesX), 1f - (float)(index / TilesX) / (float)TilesY);
		if ((Object)(object)currentRenderer != (Object)null)
		{
			instanceMaterial.SetTextureOffset("_MainTex", val);
		}
		if (IsInterpolateFrames)
		{
			currentInterpolatedTime = 0f;
		}
	}

	private void Update()
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		if (IsInterpolateFrames)
		{
			currentInterpolatedTime += Time.get_deltaTime();
			int num = index + 1;
			if (allCount == 0)
			{
				num = index;
			}
			Vector4 val = default(Vector4);
			((Vector4)(ref val))._002Ector(1f / (float)TilesX, 1f / (float)TilesY, (float)num / (float)TilesX - (float)(num / TilesX), 1f - (float)(num / TilesX) / (float)TilesY);
			if ((Object)(object)currentRenderer != (Object)null)
			{
				instanceMaterial.SetVector("_MainTex_NextFrame", val);
				float num2 = (Time.get_time() - animationStartTime) / animationLifeTime;
				float num3 = FrameOverTime.Evaluate(Mathf.Clamp01(num2));
				instanceMaterial.SetFloat("InterpolationValue", Mathf.Clamp01(currentInterpolatedTime * AnimationFPS * num3));
			}
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

	public ExplosionsSpriteSheetAnimation()
		: this()
	{
	}
}
