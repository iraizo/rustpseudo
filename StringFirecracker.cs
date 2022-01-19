using UnityEngine;

public class StringFirecracker : TimedExplosive
{
	public Rigidbody serverRigidBody;

	public Rigidbody clientMiddleBody;

	public Rigidbody[] clientParts;

	public SpringJoint serverClientJoint;

	public Transform clientFirecrackerTransform;

	public override void InitShared()
	{
		base.InitShared();
		if (!base.isServer)
		{
			return;
		}
		Rigidbody[] array = clientParts;
		foreach (Rigidbody val in array)
		{
			if ((Object)(object)val != (Object)null)
			{
				val.set_isKinematic(true);
			}
		}
	}

	public void CreatePinJoint()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)serverClientJoint != (Object)null))
		{
			serverClientJoint = ((Component)this).get_gameObject().AddComponent<SpringJoint>();
			((Joint)serverClientJoint).set_connectedBody(clientMiddleBody);
			((Joint)serverClientJoint).set_autoConfigureConnectedAnchor(false);
			((Joint)serverClientJoint).set_anchor(Vector3.get_zero());
			((Joint)serverClientJoint).set_connectedAnchor(Vector3.get_zero());
			serverClientJoint.set_minDistance(0f);
			serverClientJoint.set_maxDistance(1f);
			serverClientJoint.set_damper(1000f);
			serverClientJoint.set_spring(5000f);
			((Joint)serverClientJoint).set_enableCollision(false);
			((Joint)serverClientJoint).set_enablePreprocessing(false);
		}
	}
}
