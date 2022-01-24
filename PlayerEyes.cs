using UnityEngine;

public class PlayerEyes : EntityComponent<BasePlayer>
{
	public static readonly Vector3 EyeOffset = new Vector3(0f, 1.5f, 0f);

	public static readonly Vector3 DuckOffset = new Vector3(0f, -0.6f, 0f);

	public static readonly Vector3 CrawlOffset = new Vector3(0f, -1.15f, 0.175f);

	public Vector3 thirdPersonSleepingOffset = new Vector3(0.43f, 1.25f, 0.7f);

	public LazyAimProperties defaultLazyAim;

	private Vector3 viewOffset = Vector3.get_zero();

	public Vector3 worldMountedPosition
	{
		get
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			if (Object.op_Implicit((Object)(object)base.baseEntity) && base.baseEntity.isMounted)
			{
				Vector3 val = base.baseEntity.GetMounted().EyePositionForPlayer(base.baseEntity, GetLookRotation());
				if (val != Vector3.get_zero())
				{
					return val;
				}
			}
			return worldStandingPosition;
		}
	}

	public Vector3 worldStandingPosition => ((Component)this).get_transform().get_position() + EyeOffset;

	public Vector3 worldCrouchedPosition => worldStandingPosition + DuckOffset;

	public Vector3 worldCrawlingPosition => worldStandingPosition + CrawlOffset;

	public Vector3 position
	{
		get
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			if (Object.op_Implicit((Object)(object)base.baseEntity) && base.baseEntity.isMounted)
			{
				Vector3 val = base.baseEntity.GetMounted().EyePositionForPlayer(base.baseEntity, GetLookRotation());
				if (val != Vector3.get_zero())
				{
					return val;
				}
				return ((Component)this).get_transform().get_position() + ((Component)this).get_transform().get_up() * (EyeOffset.y + viewOffset.y) + BodyLeanOffset;
			}
			return ((Component)this).get_transform().get_position() + ((Component)this).get_transform().get_rotation() * (EyeOffset + viewOffset) + BodyLeanOffset;
		}
	}

	private Vector3 BodyLeanOffset => Vector3.get_zero();

	public Vector3 center
	{
		get
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			if (Object.op_Implicit((Object)(object)base.baseEntity) && base.baseEntity.isMounted)
			{
				Vector3 val = base.baseEntity.GetMounted().EyeCenterForPlayer(base.baseEntity, GetLookRotation());
				if (val != Vector3.get_zero())
				{
					return val;
				}
			}
			return ((Component)this).get_transform().get_position() + ((Component)this).get_transform().get_up() * (EyeOffset.y + DuckOffset.y);
		}
	}

	public Vector3 offset => ((Component)this).get_transform().get_up() * (EyeOffset.y + viewOffset.y);

	public Quaternion rotation
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			return parentRotation * bodyRotation;
		}
		set
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			bodyRotation = Quaternion.Inverse(parentRotation) * value;
		}
	}

	public Quaternion bodyRotation { get; set; }

	public Quaternion parentRotation
	{
		get
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			if (base.baseEntity.isMounted || !((Object)(object)((Component)this).get_transform().get_parent() != (Object)null))
			{
				return Quaternion.get_identity();
			}
			Quaternion val = ((Component)this).get_transform().get_parent().get_rotation();
			return Quaternion.Euler(0f, ((Quaternion)(ref val)).get_eulerAngles().y, 0f);
		}
	}

	public void NetworkUpdate(Quaternion rot)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (base.baseEntity.IsCrawling())
		{
			viewOffset = CrawlOffset;
		}
		else if (base.baseEntity.IsDucked())
		{
			viewOffset = DuckOffset;
		}
		else
		{
			viewOffset = Vector3.get_zero();
		}
		bodyRotation = rot;
	}

	public Vector3 MovementForward()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		Quaternion val = rotation;
		return Quaternion.Euler(new Vector3(0f, ((Quaternion)(ref val)).get_eulerAngles().y, 0f)) * Vector3.get_forward();
	}

	public Vector3 MovementRight()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		Quaternion val = rotation;
		return Quaternion.Euler(new Vector3(0f, ((Quaternion)(ref val)).get_eulerAngles().y, 0f)) * Vector3.get_right();
	}

	public Ray BodyRay()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return new Ray(position, BodyForward());
	}

	public Vector3 BodyForward()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return rotation * Vector3.get_forward();
	}

	public Vector3 BodyRight()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return rotation * Vector3.get_right();
	}

	public Vector3 BodyUp()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return rotation * Vector3.get_up();
	}

	public Ray HeadRay()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return new Ray(position, HeadForward());
	}

	public Vector3 HeadForward()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return GetLookRotation() * Vector3.get_forward();
	}

	public Vector3 HeadRight()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return GetLookRotation() * Vector3.get_right();
	}

	public Vector3 HeadUp()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return GetLookRotation() * Vector3.get_up();
	}

	public Quaternion GetLookRotation()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return rotation;
	}

	public Quaternion GetAimRotation()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return rotation;
	}
}
