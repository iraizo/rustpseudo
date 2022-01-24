using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseMagnet : MonoBehaviour
{
	public TriggerMagnet magnetTrigger;

	public FixedJoint fixedJoint;

	public Rigidbody kinematicAttachmentBody;

	public float magnetForce;

	public Transform attachDepthPoint;

	public GameObjectRef attachEffect;

	public bool isMagnetOn;

	public GameObject colliderSource;

	public bool HasConnectedObject()
	{
		if ((Object)(object)((Joint)fixedJoint).get_connectedBody() != (Object)null)
		{
			return isMagnetOn;
		}
		return false;
	}

	public OBB GetConnectedOBB(float scale = 1f)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)((Joint)fixedJoint).get_connectedBody() == (Object)null)
		{
			Debug.LogError((object)"BaseMagnet returning fake OBB because no connected body!");
			return new OBB(Vector3.get_zero(), Vector3.get_one(), Quaternion.get_identity());
		}
		BaseEntity component = ((Component)((Joint)fixedJoint).get_connectedBody()).get_gameObject().GetComponent<BaseEntity>();
		Bounds bounds = component.bounds;
		((Bounds)(ref bounds)).set_extents(((Bounds)(ref bounds)).get_extents() * scale);
		return new OBB(((Component)component).get_transform().get_position(), ((Component)component).get_transform().get_rotation(), bounds);
	}

	public void SetCollisionsEnabled(GameObject other, bool wants)
	{
		Collider[] componentsInChildren = other.GetComponentsInChildren<Collider>();
		Collider[] componentsInChildren2 = colliderSource.GetComponentsInChildren<Collider>();
		Collider[] array = componentsInChildren;
		foreach (Collider val in array)
		{
			Collider[] array2 = componentsInChildren2;
			foreach (Collider val2 in array2)
			{
				Physics.IgnoreCollision(val, val2, !wants);
			}
		}
	}

	public virtual void SetMagnetEnabled(bool wantsOn)
	{
		if (isMagnetOn != wantsOn)
		{
			isMagnetOn = wantsOn;
			if (isMagnetOn)
			{
				OnMagnetEnabled();
			}
			else
			{
				OnMagnetDisabled();
			}
		}
	}

	public virtual void OnMagnetEnabled()
	{
	}

	public virtual void OnMagnetDisabled()
	{
		if (Object.op_Implicit((Object)(object)((Joint)fixedJoint).get_connectedBody()))
		{
			SetCollisionsEnabled(((Component)((Joint)fixedJoint).get_connectedBody()).get_gameObject(), wants: true);
			Rigidbody connectedBody = ((Joint)fixedJoint).get_connectedBody();
			((Joint)fixedJoint).set_connectedBody((Rigidbody)null);
			connectedBody.WakeUp();
		}
	}

	public bool IsMagnetOn()
	{
		return isMagnetOn;
	}

	public void MagnetThink(float delta)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		if (!isMagnetOn)
		{
			return;
		}
		Vector3 position = ((Component)magnetTrigger).get_transform().get_position();
		if (magnetTrigger.entityContents == null)
		{
			return;
		}
		Enumerator<BaseEntity> enumerator = magnetTrigger.entityContents.GetEnumerator();
		try
		{
			OBB val = default(OBB);
			while (enumerator.MoveNext())
			{
				BaseEntity current = enumerator.get_Current();
				if (!current.syncPosition)
				{
					continue;
				}
				Rigidbody component = ((Component)current).GetComponent<Rigidbody>();
				if ((Object)(object)component == (Object)null || component.get_isKinematic() || current.isClient)
				{
					continue;
				}
				((OBB)(ref val))._002Ector(((Component)current).get_transform().get_position(), ((Component)current).get_transform().get_rotation(), current.bounds);
				if (((OBB)(ref val)).Contains(attachDepthPoint.get_position()))
				{
					((Component)current).GetComponent<MagnetLiftable>().SetMagnetized(wantsOn: true, this);
					if ((Object)(object)((Joint)fixedJoint).get_connectedBody() == (Object)null)
					{
						Effect.server.Run(attachEffect.resourcePath, attachDepthPoint.get_position(), -attachDepthPoint.get_up());
						((Joint)fixedJoint).set_connectedBody(component);
						SetCollisionsEnabled(((Component)component).get_gameObject(), wants: false);
						continue;
					}
				}
				if ((Object)(object)((Joint)fixedJoint).get_connectedBody() == (Object)null)
				{
					Vector3 position2 = ((Component)current).get_transform().get_position();
					float num = Vector3.Distance(position2, position);
					Vector3 val2 = Vector3Ex.Direction(position, position2);
					float num2 = 1f / Mathf.Max(1f, num);
					component.AddForce(val2 * magnetForce * num2, (ForceMode)5);
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
	}

	public BaseMagnet()
		: this()
	{
	}
}
