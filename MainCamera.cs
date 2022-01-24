using UnityEngine;

[ExecuteInEditMode]
public class MainCamera : RustCamera<MainCamera>
{
	public static Camera mainCamera;

	public static Transform mainCameraTransform;

	public static bool isValid
	{
		get
		{
			if ((Object)(object)mainCamera != (Object)null)
			{
				return ((Behaviour)mainCamera).get_enabled();
			}
			return false;
		}
	}

	public static Vector3 velocity { get; private set; }

	public static Vector3 position
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			return mainCameraTransform.get_position();
		}
		set
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			mainCameraTransform.set_position(value);
		}
	}

	public static Vector3 forward
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			return mainCameraTransform.get_forward();
		}
		set
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (((Vector3)(ref value)).get_sqrMagnitude() > 0f)
			{
				mainCameraTransform.set_forward(value);
			}
		}
	}

	public static Vector3 right
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			return mainCameraTransform.get_right();
		}
		set
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (((Vector3)(ref value)).get_sqrMagnitude() > 0f)
			{
				mainCameraTransform.set_right(value);
			}
		}
	}

	public static Vector3 up
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			return mainCameraTransform.get_up();
		}
		set
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			if (((Vector3)(ref value)).get_sqrMagnitude() > 0f)
			{
				((Component)mainCamera).get_transform().set_up(value);
			}
		}
	}

	public static Quaternion rotation
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			return mainCameraTransform.get_rotation();
		}
		set
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			mainCameraTransform.set_rotation(value);
		}
	}

	public static Ray Ray => new Ray(position, forward);
}
