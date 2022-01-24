using UnityEngine;

public class FollowCamera : MonoBehaviour, IClientComponent
{
	private void LateUpdate()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)MainCamera.mainCamera == (Object)null))
		{
			((Component)this).get_transform().set_position(MainCamera.position);
		}
	}

	public FollowCamera()
		: this()
	{
	}
}
