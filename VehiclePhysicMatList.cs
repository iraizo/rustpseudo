using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Vehicles/Physic Materials List", fileName = "Vehicle Physic Mat List")]
public class VehiclePhysicMatList : ScriptableObject
{
	public enum GroundType
	{
		None,
		HardSurface,
		Grass,
		Sand,
		Snow,
		Dirt,
		Gravel
	}

	[SerializeField]
	private PhysicMaterial defaultGroundMaterial;

	[SerializeField]
	private PhysicMaterial snowGroundMaterial;

	[SerializeField]
	private PhysicMaterial grassGroundMaterial;

	[SerializeField]
	private PhysicMaterial sandGroundMaterial;

	[SerializeField]
	private List<PhysicMaterial> dirtGroundMaterials;

	[SerializeField]
	private List<PhysicMaterial> stoneyGroundMaterials;

	public GroundType GetCurrentGroundType(bool isGrounded, RaycastHit hit)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		PhysicMaterial materialAt = defaultGroundMaterial;
		if (isGrounded && (Object)(object)((RaycastHit)(ref hit)).get_collider() != (Object)null)
		{
			materialAt = ((RaycastHit)(ref hit)).get_collider().GetMaterialAt(((RaycastHit)(ref hit)).get_point());
		}
		if (!isGrounded)
		{
			return GroundType.None;
		}
		if ((Object)(object)materialAt == (Object)null)
		{
			return GroundType.HardSurface;
		}
		string name = ((Object)materialAt).get_name();
		if (name == ((Object)grassGroundMaterial).get_name())
		{
			return GroundType.Grass;
		}
		if (name == ((Object)sandGroundMaterial).get_name())
		{
			return GroundType.Sand;
		}
		if (name == ((Object)snowGroundMaterial).get_name())
		{
			return GroundType.Snow;
		}
		for (int i = 0; i < dirtGroundMaterials.Count; i++)
		{
			if (((Object)dirtGroundMaterials[i]).get_name() == name)
			{
				return GroundType.Dirt;
			}
		}
		for (int j = 0; j < stoneyGroundMaterials.Count; j++)
		{
			if (((Object)stoneyGroundMaterials[j]).get_name() == name)
			{
				return GroundType.Gravel;
			}
		}
		return GroundType.HardSurface;
	}

	public VehiclePhysicMatList()
		: this()
	{
	}
}
