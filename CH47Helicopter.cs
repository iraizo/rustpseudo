using UnityEngine;

public class CH47Helicopter : BaseHelicopterVehicle
{
	public GameObjectRef mapMarkerEntityPrefab;

	private BaseEntity mapMarkerInstance;

	public override void ServerInit()
	{
		rigidBody.set_isKinematic(false);
		base.ServerInit();
		CreateMapMarker();
	}

	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		base.PlayerServerInput(inputState, player);
	}

	public void CreateMapMarker()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)mapMarkerInstance))
		{
			mapMarkerInstance.Kill();
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(mapMarkerEntityPrefab.resourcePath, Vector3.get_zero(), Quaternion.get_identity());
		baseEntity.Spawn();
		baseEntity.SetParent(this);
		mapMarkerInstance = baseEntity;
	}

	protected override bool CanPushNow(BasePlayer pusher)
	{
		return false;
	}
}
