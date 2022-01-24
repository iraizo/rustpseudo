using EZhex1991.EZSoftBone;
using UnityEngine;

[RequireComponent(typeof(EZSoftBone))]
public class GhostSheetSystemSpaceUpdater : MonoBehaviour, IClientComponent
{
	private EZSoftBone[] ezSoftBones;

	private BasePlayer player;

	public void Awake()
	{
		ezSoftBones = ((Component)this).GetComponents<EZSoftBone>();
		player = ((Component)this).get_gameObject().ToBaseEntity() as BasePlayer;
	}

	public void Update()
	{
		if (ezSoftBones == null || ezSoftBones.Length == 0 || (Object)(object)player == (Object)null)
		{
			return;
		}
		BaseMountable mounted = player.GetMounted();
		if ((Object)(object)mounted != (Object)null)
		{
			SetSimulateSpace(((Component)mounted).get_transform(), collisionEnabled: false);
			return;
		}
		BaseEntity parentEntity = player.GetParentEntity();
		if ((Object)(object)parentEntity != (Object)null)
		{
			SetSimulateSpace(((Component)parentEntity).get_transform(), collisionEnabled: true);
		}
		else
		{
			SetSimulateSpace(null, collisionEnabled: true);
		}
	}

	private void SetSimulateSpace(Transform transform, bool collisionEnabled)
	{
		for (int i = 0; i < ezSoftBones.Length; i++)
		{
			EZSoftBone obj = ezSoftBones[i];
			obj.set_simulateSpace(transform);
			obj.collisionEnabled = collisionEnabled;
		}
	}

	public GhostSheetSystemSpaceUpdater()
		: this()
	{
	}
}
