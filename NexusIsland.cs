using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

public class NexusIsland : BaseEntity, INexusTransferTriggerController
{
	[Header("Nexus Island")]
	public BoxCollider TransferZone;

	public BoxCollider SpawnZone;

	public float TraceHeight = 100f;

	public LayerMask TraceLayerMask = LayerMask.op_Implicit(429990145);

	public GameObjectRef MapMarkerPrefab;

	public Transform MapMarkerLocation;

	[NonSerialized]
	public string ZoneName;

	public bool CanTransfer(BaseEntity entity)
	{
		if (!(entity is BaseBoat) && !(entity is BaseSubmarine) && !(entity is WaterInflatable))
		{
			return entity is BasePlayer;
		}
		return true;
	}

	public (string Zone, string Method) GetTransferDestination()
	{
		return (ZoneName, "ocean");
	}

	public bool TryFindPosition(out Vector3 position, float radius = 10f)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)SpawnZone == (Object)null)
		{
			Debug.LogError((object)"SpawnZone is null, cannot find a spawn position", (Object)(object)this);
			position = Vector3.get_zero();
			return false;
		}
		Transform transform = ((Component)SpawnZone).get_transform();
		Vector3 size = SpawnZone.get_size();
		RaycastHit val3 = default(RaycastHit);
		for (int i = 0; i < 10; i++)
		{
			Vector3 val = Vector3Ex.Scale(size, Random.get_value() - 0.5f, 0f, Random.get_value() - 0.5f);
			Vector3 val2 = transform.TransformPoint(val);
			if (IsValidPosition(val2, radius))
			{
				float height = WaterSystem.GetHeight(val2);
				if (!Physics.SphereCast(Vector3Ex.WithY(val2, height + TraceHeight), radius, Vector3.get_down(), ref val3, TraceHeight + radius, LayerMask.op_Implicit(TraceLayerMask), (QueryTriggerInteraction)1) || ((RaycastHit)(ref val3)).get_point().y < height)
				{
					position = Vector3Ex.WithY(val2, height);
					return true;
				}
			}
		}
		position = Vector3.get_zero();
		return false;
		static bool IsValidPosition(Vector3 center, float extent)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			if (ValidBounds.Test(center) && ValidBounds.Test(center + new Vector3(0f - extent, 0f, 0f - extent)) && ValidBounds.Test(center + new Vector3(0f - extent, 0f, extent)) && ValidBounds.Test(center + new Vector3(extent, 0f, 0f - extent)))
			{
				return ValidBounds.Test(center + new Vector3(extent, 0f, extent));
			}
			return false;
		}
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.nexusIsland != null)
		{
			ZoneName = info.msg.nexusIsland.zoneName;
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.nexusIsland = Pool.Get<NexusIsland>();
		info.msg.nexusIsland.zoneName = ZoneName;
	}

	public override void ServerInit()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		BaseEntity baseEntity = GameManager.server.CreateEntity(MapMarkerPrefab.resourcePath, MapMarkerLocation.get_position(), MapMarkerLocation.get_rotation());
		baseEntity.Spawn();
		baseEntity.SetParent(this, worldPositionStays: true);
	}
}
