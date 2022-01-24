using System;
using UnityEngine;
using UnityEngine.UI;

public class UIParticle : BaseMonoBehaviour
{
	public Vector2 LifeTime;

	public Vector2 Gravity = new Vector2(1000f, 1000f);

	public Vector2 InitialX;

	public Vector2 InitialY;

	public Vector2 InitialScale = Vector2.get_one();

	public Vector2 InitialDelay;

	public Vector2 ScaleVelocity;

	public Gradient InitialColor;

	private float lifetime;

	private float gravity;

	private Vector2 velocity;

	private float scaleVelocity;

	public static void Add(UIParticle particleSource, RectTransform spawnPosition, RectTransform particleCanvas)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		GameObject obj = Object.Instantiate<GameObject>(((Component)particleSource).get_gameObject());
		obj.get_transform().SetParent((Transform)(object)spawnPosition, false);
		Transform transform = obj.get_transform();
		Rect rect = spawnPosition.get_rect();
		float num = Random.Range(0f, ((Rect)(ref rect)).get_width());
		rect = spawnPosition.get_rect();
		float num2 = num - ((Rect)(ref rect)).get_width() * spawnPosition.get_pivot().x;
		rect = spawnPosition.get_rect();
		float num3 = Random.Range(0f, ((Rect)(ref rect)).get_height());
		rect = spawnPosition.get_rect();
		transform.set_localPosition(new Vector3(num2, num3 - ((Rect)(ref rect)).get_height() * spawnPosition.get_pivot().y, 0f));
		obj.get_transform().SetParent((Transform)(object)particleCanvas, true);
		obj.get_transform().set_localScale(Vector3.get_one());
		obj.get_transform().set_localRotation(Quaternion.get_identity());
	}

	private void Start()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		Transform transform = ((Component)this).get_transform();
		transform.set_localScale(transform.get_localScale() * Random.Range(InitialScale.x, InitialScale.y));
		velocity.x = Random.Range(InitialX.x, InitialX.y);
		velocity.y = Random.Range(InitialY.x, InitialY.y);
		gravity = Random.Range(Gravity.x, Gravity.y);
		scaleVelocity = Random.Range(ScaleVelocity.x, ScaleVelocity.y);
		Image component = ((Component)this).GetComponent<Image>();
		if (Object.op_Implicit((Object)(object)component))
		{
			((Graphic)component).set_color(InitialColor.Evaluate(Random.Range(0f, 1f)));
		}
		lifetime = Random.Range(InitialDelay.x, InitialDelay.y) * -1f;
		if (lifetime < 0f)
		{
			((Component)this).GetComponent<CanvasGroup>().set_alpha(0f);
		}
		((FacepunchBehaviour)this).Invoke((Action)Die, Random.Range(LifeTime.x, LifeTime.y) + lifetime * -1f);
	}

	private void Update()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		if (lifetime < 0f)
		{
			lifetime += Time.get_deltaTime();
			if (lifetime < 0f)
			{
				return;
			}
			((Component)this).GetComponent<CanvasGroup>().set_alpha(1f);
		}
		else
		{
			lifetime += Time.get_deltaTime();
		}
		Vector3 position = ((Component)this).get_transform().get_position();
		Vector3 localScale = ((Component)this).get_transform().get_localScale();
		velocity.y -= gravity * Time.get_deltaTime();
		position.x += velocity.x * Time.get_deltaTime();
		position.y += velocity.y * Time.get_deltaTime();
		localScale += Vector3.get_one() * scaleVelocity * Time.get_deltaTime();
		if (localScale.x <= 0f || localScale.y <= 0f)
		{
			Die();
			return;
		}
		((Component)this).get_transform().set_position(position);
		((Component)this).get_transform().set_localScale(localScale);
	}

	private void Die()
	{
		Object.Destroy((Object)(object)((Component)this).get_gameObject());
	}
}
