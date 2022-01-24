using System;
using UnityEngine;

[ExecuteInEditMode]
public class WaterBody : MonoBehaviour
{
	[Flags]
	public enum FishingTag
	{
		MoonPool = 0x1,
		River = 0x2,
		Ocean = 0x4,
		Swamp = 0x8
	}

	public WaterBodyType Type = WaterBodyType.Lake;

	public Renderer Renderer;

	public Collider[] Triggers;

	public bool IsOcean;

	public FishingTag FishingType;

	public Transform Transform { get; private set; }

	private void Awake()
	{
		Transform = ((Component)this).get_transform();
	}

	private void OnEnable()
	{
		WaterSystem.RegisterBody(this);
	}

	private void OnDisable()
	{
		WaterSystem.UnregisterBody(this);
	}

	public void OnOceanLevelChanged(float newLevel)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (IsOcean)
		{
			Collider[] triggers = Triggers;
			foreach (Collider obj in triggers)
			{
				Vector3 position = ((Component)obj).get_transform().get_position();
				position.y = newLevel;
				((Component)obj).get_transform().set_position(position);
			}
		}
	}

	public WaterBody()
		: this()
	{
	}
}
