using System.Collections.Generic;
using Rust;
using UnityEngine;

public class HelicopterDebris : ServerGib
{
	public ItemDefinition metalFragments;

	public ItemDefinition hqMetal;

	public ItemDefinition charcoal;

	[Tooltip("Divide mass by this amount to produce a scalar of resources, default = 5")]
	public float massReductionScalar = 5f;

	private ResourceDispenser resourceDispenser;

	private float tooHotUntil;

	public override void ServerInit()
	{
		base.ServerInit();
		tooHotUntil = Time.get_realtimeSinceStartup() + 480f;
	}

	public override void PhysicsInit(Mesh mesh)
	{
		base.PhysicsInit(mesh);
		if (!base.isServer)
		{
			return;
		}
		resourceDispenser = ((Component)this).GetComponent<ResourceDispenser>();
		float num = Mathf.Clamp01(((Component)this).GetComponent<Rigidbody>().get_mass() / massReductionScalar);
		resourceDispenser.containedItems = new List<ItemAmount>();
		if (num > 0.75f && (Object)(object)hqMetal != (Object)null)
		{
			resourceDispenser.containedItems.Add(new ItemAmount(hqMetal, Mathf.CeilToInt(7f * num)));
		}
		if (num > 0f)
		{
			if ((Object)(object)metalFragments != (Object)null)
			{
				resourceDispenser.containedItems.Add(new ItemAmount(metalFragments, Mathf.CeilToInt(150f * num)));
			}
			if ((Object)(object)charcoal != (Object)null)
			{
				resourceDispenser.containedItems.Add(new ItemAmount(charcoal, Mathf.CeilToInt(80f * num)));
			}
		}
		resourceDispenser.Initialize();
	}

	public bool IsTooHot()
	{
		return tooHotUntil > Time.get_realtimeSinceStartup();
	}

	public override void OnAttacked(HitInfo info)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		if (IsTooHot() && info.WeaponPrefab is BaseMelee)
		{
			if (info.Initiator is BasePlayer)
			{
				HitInfo hitInfo = new HitInfo();
				hitInfo.damageTypes.Add(DamageType.Heat, 5f);
				hitInfo.DoHitEffects = true;
				hitInfo.DidHit = true;
				hitInfo.HitBone = 0u;
				hitInfo.Initiator = this;
				hitInfo.PointStart = ((Component)this).get_transform().get_position();
				Effect.server.Run("assets/bundled/prefabs/fx/impacts/additive/fire.prefab", info.Initiator, 0u, new Vector3(0f, 1f, 0f), Vector3.get_up());
			}
		}
		else
		{
			if (Object.op_Implicit((Object)(object)resourceDispenser))
			{
				resourceDispenser.OnAttacked(info);
			}
			base.OnAttacked(info);
		}
	}
}
