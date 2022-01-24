using Facepunch;
using ProtoBuf;
using UnityEngine;

public class SphereEntity : BaseEntity
{
	public float currentRadius = 1f;

	public float lerpRadius = 1f;

	public float lerpSpeed = 1f;

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.sphereEntity = Pool.Get<SphereEntity>();
		info.msg.sphereEntity.radius = currentRadius;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (base.isServer)
		{
			if (info.msg.sphereEntity != null)
			{
				currentRadius = (lerpRadius = info.msg.sphereEntity.radius);
			}
			UpdateScale();
		}
	}

	public void LerpRadiusTo(float radius, float speed)
	{
		lerpRadius = radius;
		lerpSpeed = speed;
	}

	protected void UpdateScale()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).get_transform().set_localScale(new Vector3(currentRadius, currentRadius, currentRadius));
	}

	protected void Update()
	{
		if (currentRadius != lerpRadius && base.isServer)
		{
			currentRadius = Mathf.MoveTowards(currentRadius, lerpRadius, Time.get_deltaTime() * lerpSpeed);
			UpdateScale();
			SendNetworkUpdate();
		}
	}
}
