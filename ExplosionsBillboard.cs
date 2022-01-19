using UnityEngine;

public class ExplosionsBillboard : MonoBehaviour
{
	public Camera Camera;

	public bool Active = true;

	public bool AutoInitCamera = true;

	private GameObject myContainer;

	private Transform t;

	private Transform camT;

	private Transform contT;

	private void Awake()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Expected O, but got Unknown
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		if (AutoInitCamera)
		{
			Camera = Camera.get_main();
			Active = true;
		}
		t = ((Component)this).get_transform();
		Vector3 localScale = ((Component)t.get_parent()).get_transform().get_localScale();
		localScale.z = localScale.x;
		((Component)t.get_parent()).get_transform().set_localScale(localScale);
		camT = ((Component)Camera).get_transform();
		Transform parent = t.get_parent();
		GameObject val = new GameObject();
		((Object)val).set_name("Billboard_" + ((Object)((Component)t).get_gameObject()).get_name());
		myContainer = val;
		contT = myContainer.get_transform();
		contT.set_position(t.get_position());
		t.set_parent(myContainer.get_transform());
		contT.set_parent(parent);
	}

	private void Update()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (Active)
		{
			contT.LookAt(contT.get_position() + camT.get_rotation() * Vector3.get_back(), camT.get_rotation() * Vector3.get_up());
		}
	}

	public ExplosionsBillboard()
		: this()
	{
	}
}
