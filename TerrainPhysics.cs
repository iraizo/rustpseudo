using UnityEngine;

public class TerrainPhysics : TerrainExtension
{
	private TerrainSplatMap splat;

	private PhysicMaterial[] materials;

	public override void Setup()
	{
		splat = ((Component)terrain).GetComponent<TerrainSplatMap>();
		materials = config.GetPhysicMaterials();
	}

	public PhysicMaterial GetMaterial(Vector3 worldPos)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return materials[splat.GetSplatMaxIndex(worldPos)];
	}
}
