using UnityEngine;

namespace Rust.Modular
{
	public class ItemModVehicleModule : ItemMod, VehicleModuleInformationPanel.IVehicleModuleInfo
	{
		public GameObjectRef entityPrefab;

		[Range(1f, 2f)]
		public int socketsTaken = 1;

		public bool doNonUserSpawn;

		public int SocketsTaken => socketsTaken;

		public BaseVehicleModule CreateModuleEntity(BaseEntity parent, Vector3 position, Quaternion rotation)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			if (!entityPrefab.isValid)
			{
				Debug.LogError((object)"Invalid entity prefab for module");
				return null;
			}
			BaseEntity baseEntity = GameManager.server.CreateEntity(entityPrefab.resourcePath, position, rotation);
			BaseVehicleModule baseVehicleModule = null;
			if ((Object)(object)baseEntity != (Object)null)
			{
				if ((Object)(object)parent != (Object)null)
				{
					baseEntity.SetParent(parent, worldPositionStays: true);
				}
				baseEntity.Spawn();
				baseVehicleModule = ((Component)baseEntity).GetComponent<BaseVehicleModule>();
				if (doNonUserSpawn)
				{
					doNonUserSpawn = false;
					baseVehicleModule.NonUserSpawn();
				}
			}
			return baseVehicleModule;
		}
	}
}
