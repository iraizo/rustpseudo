using UnityEngine;

public class rottest : MonoBehaviour
{
	public Transform turretBase;

	public Vector3 aimDir;

	private void Start()
	{
	}

	private void Update()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		aimDir = new Vector3(0f, 45f * Mathf.Sin(Time.get_time() * 6f), 0f);
		UpdateAiming();
	}

	public void UpdateAiming()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (!(aimDir == Vector3.get_zero()))
		{
			Quaternion val = Quaternion.Euler(0f, aimDir.y, 0f);
			if (((Component)this).get_transform().get_localRotation() != val)
			{
				((Component)this).get_transform().set_localRotation(Quaternion.Lerp(((Component)this).get_transform().get_localRotation(), val, Time.get_deltaTime() * 8f));
			}
		}
	}

	public rottest()
		: this()
	{
	}
}
