using UnityEngine;

public class GenerateRoadMeshes : ProceduralComponent
{
	public const float NormalSmoothing = 0f;

	public Mesh RoadMesh;

	public Mesh[] RoadMeshes;

	public Material RoadMaterial;

	public Material RoadRingMaterial;

	public PhysicMaterial RoadPhysicMaterial;

	public override bool RunOnCache => true;

	public override void Process(uint seed)
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		if (RoadMeshes == null || RoadMeshes.Length == 0)
		{
			RoadMeshes = (Mesh[])(object)new Mesh[1] { RoadMesh };
		}
		foreach (PathList road in TerrainMeta.Path.Roads)
		{
			if (road.IsExtraNarrow)
			{
				continue;
			}
			foreach (PathList.MeshObject item in road.CreateMesh(RoadMeshes, 0f))
			{
				GameObject val = new GameObject("Road Mesh");
				val.get_transform().set_position(item.Position);
				val.set_layer(16);
				GameObjectEx.SetHierarchyGroup(val, road.Name);
				val.SetActive(false);
				MeshCollider obj = val.AddComponent<MeshCollider>();
				((Collider)obj).set_sharedMaterial(RoadPhysicMaterial);
				obj.set_sharedMesh(item.Meshes[0]);
				val.AddComponent<AddToHeightMap>();
				val.SetActive(true);
			}
		}
	}
}
