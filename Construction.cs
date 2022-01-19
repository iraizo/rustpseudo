using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using UnityEngine;
using UnityEngine.Serialization;

public class Construction : PrefabAttribute
{
	public struct Target
	{
		public bool valid;

		public Ray ray;

		public BaseEntity entity;

		public Socket_Base socket;

		public bool onTerrain;

		public Vector3 position;

		public Vector3 normal;

		public Vector3 rotation;

		public BasePlayer player;

		public bool inBuildingPrivilege;

		public Quaternion GetWorldRotation(bool female)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			Quaternion val = socket.rotation;
			if (socket.male && socket.female && female)
			{
				val = socket.rotation * Quaternion.Euler(180f, 0f, 180f);
			}
			return ((Component)entity).get_transform().get_rotation() * val;
		}

		public Vector3 GetWorldPosition()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			Matrix4x4 localToWorldMatrix = ((Component)entity).get_transform().get_localToWorldMatrix();
			return ((Matrix4x4)(ref localToWorldMatrix)).MultiplyPoint3x4(socket.position);
		}
	}

	public class Placement
	{
		public Vector3 position;

		public Quaternion rotation;
	}

	public class Grade
	{
		public BuildingGrade grade;

		public float maxHealth;

		public List<ItemAmount> costToBuild;

		public PhysicMaterial physicMaterial => grade.physicMaterial;

		public ProtectionProperties damageProtecton => grade.damageProtecton;
	}

	public static string lastPlacementError;

	public BaseEntity.Menu.Option info;

	public bool canBypassBuildingPermission;

	[FormerlySerializedAs("canRotate")]
	public bool canRotateBeforePlacement;

	[FormerlySerializedAs("canRotate")]
	public bool canRotateAfterPlacement;

	public bool checkVolumeOnRotate;

	public bool checkVolumeOnUpgrade;

	public bool canPlaceAtMaxDistance;

	public bool placeOnWater;

	public Vector3 rotationAmount = new Vector3(0f, 90f, 0f);

	public Vector3 applyStartingRotation = Vector3.get_zero();

	[Range(0f, 10f)]
	public float healthMultiplier = 1f;

	[Range(0f, 10f)]
	public float costMultiplier = 1f;

	[Range(1f, 50f)]
	public float maxplaceDistance = 4f;

	public Mesh guideMesh;

	[NonSerialized]
	public Socket_Base[] allSockets;

	[NonSerialized]
	public BuildingProximity[] allProximities;

	[NonSerialized]
	public ConstructionGrade defaultGrade;

	[NonSerialized]
	public SocketHandle socketHandle;

	[NonSerialized]
	public Bounds bounds;

	[NonSerialized]
	public bool isBuildingPrivilege;

	[NonSerialized]
	public ConstructionGrade[] grades;

	[NonSerialized]
	public Deployable deployable;

	[NonSerialized]
	public ConstructionPlaceholder placeholder;

	public bool UpdatePlacement(Transform transform, Construction common, ref Target target)
	{
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		if (!target.valid)
		{
			return false;
		}
		if (!common.canBypassBuildingPermission && !target.player.CanBuild())
		{
			lastPlacementError = "You don't have permission to build here";
			return false;
		}
		List<Socket_Base> list = Pool.GetList<Socket_Base>();
		common.FindMaleSockets(target, list);
		foreach (Socket_Base item in list)
		{
			Placement placement = null;
			if ((Object)(object)target.entity != (Object)null && target.socket != null && target.entity.IsOccupied(target.socket))
			{
				continue;
			}
			if (placement == null)
			{
				placement = item.DoPlacement(target);
			}
			if (placement == null)
			{
				continue;
			}
			if (!item.CheckSocketMods(placement))
			{
				transform.set_position(placement.position);
				transform.set_rotation(placement.rotation);
				continue;
			}
			if (!TestPlacingThroughRock(ref placement, target))
			{
				transform.set_position(placement.position);
				transform.set_rotation(placement.rotation);
				lastPlacementError = "Placing through rock";
				continue;
			}
			if (!TestPlacingThroughWall(ref placement, transform, common, target))
			{
				transform.set_position(placement.position);
				transform.set_rotation(placement.rotation);
				lastPlacementError = "Placing through wall";
				continue;
			}
			if (!TestPlacingCloseToRoad(ref placement, target))
			{
				transform.set_position(placement.position);
				transform.set_rotation(placement.rotation);
				lastPlacementError = "Placing too close to road";
				continue;
			}
			if (Vector3.Distance(placement.position, target.player.eyes.position) > common.maxplaceDistance + 1f)
			{
				transform.set_position(placement.position);
				transform.set_rotation(placement.rotation);
				lastPlacementError = "Too far away";
				continue;
			}
			DeployVolume[] volumes = PrefabAttribute.server.FindAll<DeployVolume>(prefabID);
			if (DeployVolume.Check(placement.position, placement.rotation, volumes))
			{
				transform.set_position(placement.position);
				transform.set_rotation(placement.rotation);
				lastPlacementError = "Not enough space";
				continue;
			}
			if (BuildingProximity.Check(target.player, this, placement.position, placement.rotation))
			{
				transform.set_position(placement.position);
				transform.set_rotation(placement.rotation);
				lastPlacementError = "Too close to another building";
				continue;
			}
			if (common.isBuildingPrivilege && !target.player.CanPlaceBuildingPrivilege(placement.position, placement.rotation, common.bounds))
			{
				transform.set_position(placement.position);
				transform.set_rotation(placement.rotation);
				lastPlacementError = "Cannot stack building privileges";
				continue;
			}
			bool flag = target.player.IsBuildingBlocked(placement.position, placement.rotation, common.bounds);
			if (!common.canBypassBuildingPermission && flag)
			{
				transform.set_position(placement.position);
				transform.set_rotation(placement.rotation);
				lastPlacementError = "You don't have permission to build here";
				continue;
			}
			target.inBuildingPrivilege = flag;
			transform.SetPositionAndRotation(placement.position, placement.rotation);
			Pool.FreeList<Socket_Base>(ref list);
			return true;
		}
		Pool.FreeList<Socket_Base>(ref list);
		return false;
	}

	private bool TestPlacingThroughRock(ref Placement placement, Target target)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		OBB val = default(OBB);
		((OBB)(ref val))._002Ector(placement.position, Vector3.get_one(), placement.rotation, bounds);
		Vector3 center = target.player.GetCenter(ducked: true);
		Vector3 origin = ((Ray)(ref target.ray)).get_origin();
		if (Physics.Linecast(center, origin, 65536, (QueryTriggerInteraction)1))
		{
			return false;
		}
		RaycastHit val2 = default(RaycastHit);
		Vector3 val3 = (((OBB)(ref val)).Trace(target.ray, ref val2, float.PositiveInfinity) ? ((RaycastHit)(ref val2)).get_point() : ((OBB)(ref val)).ClosestPoint(origin));
		if (Physics.Linecast(origin, val3, 65536, (QueryTriggerInteraction)1))
		{
			return false;
		}
		return true;
	}

	private static bool TestPlacingThroughWall(ref Placement placement, Transform transform, Construction common, Target target)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = placement.position - ((Ray)(ref target.ray)).get_origin();
		RaycastHit hit = default(RaycastHit);
		if (!Physics.Raycast(((Ray)(ref target.ray)).get_origin(), ((Vector3)(ref val)).get_normalized(), ref hit, ((Vector3)(ref val)).get_magnitude(), 2097152))
		{
			return true;
		}
		StabilityEntity stabilityEntity = hit.GetEntity() as StabilityEntity;
		if ((Object)(object)stabilityEntity != (Object)null && (Object)(object)target.entity == (Object)(object)stabilityEntity)
		{
			return true;
		}
		if (((Vector3)(ref val)).get_magnitude() - ((RaycastHit)(ref hit)).get_distance() < 0.2f)
		{
			return true;
		}
		lastPlacementError = "object in placement path";
		transform.SetPositionAndRotation(((RaycastHit)(ref hit)).get_point(), placement.rotation);
		return false;
	}

	private bool TestPlacingCloseToRoad(ref Placement placement, Target target)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		if ((Object)(object)heightMap == (Object)null)
		{
			return true;
		}
		if ((Object)(object)topologyMap == (Object)null)
		{
			return true;
		}
		OBB val = default(OBB);
		((OBB)(ref val))._002Ector(placement.position, Vector3.get_one(), placement.rotation, bounds);
		float num = Mathf.Abs(heightMap.GetHeight(val.position) - val.position.y);
		if (num > 9f)
		{
			return true;
		}
		float radius = Mathf.Lerp(3f, 0f, num / 9f);
		Vector3 position = val.position;
		Vector3 point = ((OBB)(ref val)).GetPoint(-1f, 0f, -1f);
		Vector3 point2 = ((OBB)(ref val)).GetPoint(-1f, 0f, 1f);
		Vector3 point3 = ((OBB)(ref val)).GetPoint(1f, 0f, -1f);
		Vector3 point4 = ((OBB)(ref val)).GetPoint(1f, 0f, 1f);
		int topology = topologyMap.GetTopology(position, radius);
		int topology2 = topologyMap.GetTopology(point, radius);
		int topology3 = topologyMap.GetTopology(point2, radius);
		int topology4 = topologyMap.GetTopology(point3, radius);
		int topology5 = topologyMap.GetTopology(point4, radius);
		if (((topology | topology2 | topology3 | topology4 | topology5) & 0x800) == 0)
		{
			return true;
		}
		return false;
	}

	public virtual bool ShowAsNeutral(Target target)
	{
		return target.inBuildingPrivilege;
	}

	public BaseEntity CreateConstruction(Target target, bool bNeedsValidPlacement = false)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = GameManager.server.CreatePrefab(fullName, Vector3.get_zero(), Quaternion.get_identity(), active: false);
		bool flag = UpdatePlacement(val.get_transform(), this, ref target);
		BaseEntity baseEntity = val.ToBaseEntity();
		if (bNeedsValidPlacement && !flag)
		{
			if (baseEntity.IsValid())
			{
				baseEntity.Kill();
			}
			else
			{
				GameManager.Destroy(val);
			}
			return null;
		}
		DecayEntity decayEntity = baseEntity as DecayEntity;
		if (Object.op_Implicit((Object)(object)decayEntity))
		{
			decayEntity.AttachToBuilding(target.entity as DecayEntity);
		}
		return baseEntity;
	}

	public bool HasMaleSockets(Target target)
	{
		Socket_Base[] array = allSockets;
		foreach (Socket_Base socket_Base in array)
		{
			if (socket_Base.male && !socket_Base.maleDummy && socket_Base.TestTarget(target))
			{
				return true;
			}
		}
		return false;
	}

	public void FindMaleSockets(Target target, List<Socket_Base> sockets)
	{
		Socket_Base[] array = allSockets;
		foreach (Socket_Base socket_Base in array)
		{
			if (socket_Base.male && !socket_Base.maleDummy && socket_Base.TestTarget(target))
			{
				sockets.Add(socket_Base);
			}
		}
	}

	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		isBuildingPrivilege = Object.op_Implicit((Object)(object)rootObj.GetComponent<BuildingPrivlidge>());
		bounds = rootObj.GetComponent<BaseEntity>().bounds;
		deployable = ((Component)this).GetComponent<Deployable>();
		placeholder = ((Component)this).GetComponentInChildren<ConstructionPlaceholder>();
		allSockets = ((Component)this).GetComponentsInChildren<Socket_Base>(true);
		allProximities = ((Component)this).GetComponentsInChildren<BuildingProximity>(true);
		socketHandle = ((Component)this).GetComponentsInChildren<SocketHandle>(true).FirstOrDefault();
		ConstructionGrade[] components = rootObj.GetComponents<ConstructionGrade>();
		grades = new ConstructionGrade[5];
		ConstructionGrade[] array = components;
		foreach (ConstructionGrade constructionGrade in array)
		{
			constructionGrade.construction = this;
			grades[(int)constructionGrade.gradeBase.type] = constructionGrade;
		}
		for (int j = 0; j < grades.Length; j++)
		{
			if (!(grades[j] == null))
			{
				defaultGrade = grades[j];
				break;
			}
		}
	}

	protected override Type GetIndexedType()
	{
		return typeof(Construction);
	}
}
