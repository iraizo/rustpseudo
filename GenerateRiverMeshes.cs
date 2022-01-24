using UnityEngine;

public class GenerateRiverMeshes : ProceduralComponent
{
	public const float NormalSmoothing = 0.1f;

	public Mesh RiverMesh;

	public Mesh[] RiverMeshes;

	public Material RiverMaterial;

	public PhysicMaterial RiverPhysicMaterial;

	public override bool RunOnCache => true;

	public override void Process(uint seed)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected O, but got Unknown
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		RiverMeshes = (Mesh[])(object)new Mesh[1] { RiverMesh };
		foreach (PathList river in TerrainMeta.Path.Rivers)
		{
			foreach (PathList.MeshObject item in river.CreateMesh(RiverMeshes, 0.1f))
			{
				GameObject val = new GameObject("River Mesh");
				val.get_transform().set_position(item.Position);
				val.set_tag("River");
				val.set_layer(4);
				GameObjectEx.SetHierarchyGroup(val, river.Name);
				val.SetActive(false);
				MeshCollider obj = val.AddComponent<MeshCollider>();
				((Collider)obj).set_sharedMaterial(RiverPhysicMaterial);
				obj.set_sharedMesh(item.Meshes[0]);
				val.AddComponent<RiverInfo>();
				val.AddComponent<WaterBody>().FishingType = WaterBody.FishingTag.River;
				val.AddComponent<AddToWaterMap>();
				val.SetActive(true);
			}
		}
	}
}
