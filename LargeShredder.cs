using System;
using UnityEngine;

public class LargeShredder : BaseEntity
{
	public Transform shredRail;

	public Transform shredRailStartPos;

	public Transform shredRailEndPos;

	public Vector3 shredRailStartRotation;

	public Vector3 shredRailEndRotation;

	public LargeShredderTrigger trigger;

	public float shredDurationRotation = 2f;

	public float shredDurationPosition = 5f;

	public float shredSwayAmount = 1f;

	public float shredSwaySpeed = 3f;

	public BaseEntity currentlyShredding;

	public GameObject[] shreddingWheels;

	public float shredRotorSpeed = 1f;

	public GameObjectRef shredSoundEffect;

	public Transform resourceSpawnPoint;

	private Quaternion entryRotation;

	private bool isShredding;

	private float shredStartTime;

	public virtual void OnEntityEnteredTrigger(BaseEntity ent)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.IsDestroyed)
		{
			Rigidbody component = ((Component)ent).GetComponent<Rigidbody>();
			if (isShredding || (Object)(object)currentlyShredding != (Object)null)
			{
				component.set_velocity(-component.get_velocity() * 3f);
				return;
			}
			((Component)shredRail).get_transform().set_position(shredRailStartPos.get_position());
			((Component)shredRail).get_transform().set_rotation(Quaternion.LookRotation(shredRailStartRotation));
			entryRotation = ((Component)ent).get_transform().get_rotation();
			Quaternion rotation = ((Component)ent).get_transform().get_rotation();
			component.set_isKinematic(true);
			currentlyShredding = ent;
			((Component)ent).get_transform().set_rotation(rotation);
			isShredding = true;
			SetShredding(isShredding: true);
			shredStartTime = Time.get_realtimeSinceStartup();
		}
	}

	public void CreateShredResources()
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)currentlyShredding == (Object)null)
		{
			return;
		}
		MagnetLiftable component = ((Component)currentlyShredding).GetComponent<MagnetLiftable>();
		if ((Object)(object)component == (Object)null)
		{
			return;
		}
		ItemAmount[] shredResources = component.shredResources;
		foreach (ItemAmount itemAmount in shredResources)
		{
			Item item = ItemManager.Create(itemAmount.itemDef, (int)itemAmount.amount, 0uL);
			float num = 0.5f;
			if ((Object)(object)item.CreateWorldObject(((Component)resourceSpawnPoint).get_transform().get_position() + new Vector3(Random.Range(0f - num, num), 1f, Random.Range(0f - num, num))) == (Object)null)
			{
				item.Remove();
			}
		}
		BaseModularVehicle component2 = ((Component)currentlyShredding).GetComponent<BaseModularVehicle>();
		if (!Object.op_Implicit((Object)(object)component2))
		{
			return;
		}
		foreach (BaseVehicleModule attachedModuleEntity in component2.AttachedModuleEntities)
		{
			if (!Object.op_Implicit((Object)(object)attachedModuleEntity.AssociatedItemDef) || !Object.op_Implicit((Object)(object)attachedModuleEntity.AssociatedItemDef.Blueprint))
			{
				continue;
			}
			foreach (ItemAmount ingredient in attachedModuleEntity.AssociatedItemDef.Blueprint.ingredients)
			{
				int num2 = Mathf.FloorToInt(ingredient.amount * 0.5f);
				if (num2 != 0)
				{
					Item item2 = ItemManager.Create(ingredient.itemDef, num2, 0uL);
					float num3 = 0.5f;
					if ((Object)(object)item2.CreateWorldObject(((Component)resourceSpawnPoint).get_transform().get_position() + new Vector3(Random.Range(0f - num3, num3), 1f, Random.Range(0f - num3, num3))) == (Object)null)
					{
						item2.Remove();
					}
				}
			}
		}
	}

	public void UpdateBonePosition(float delta)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		float num = delta / shredDurationPosition;
		float num2 = delta / shredDurationRotation;
		((Component)shredRail).get_transform().set_localPosition(Vector3.Lerp(shredRailStartPos.get_localPosition(), shredRailEndPos.get_localPosition(), num));
		((Component)shredRail).get_transform().set_rotation(Quaternion.LookRotation(Vector3.Lerp(shredRailStartRotation, shredRailEndRotation, num2)));
	}

	public void SetShredding(bool isShredding)
	{
		if (isShredding)
		{
			((FacepunchBehaviour)this).InvokeRandomized((Action)FireShredEffect, 0.25f, 0.75f, 0.25f);
		}
		else
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)FireShredEffect);
		}
	}

	public void FireShredEffect()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		Effect.server.Run(shredSoundEffect.resourcePath, ((Component)this).get_transform().get_position() + Vector3.get_up() * 3f, Vector3.get_up());
	}

	public void ServerUpdate()
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		if (base.isClient)
		{
			return;
		}
		SetFlag(Flags.Reserved10, isShredding);
		if (isShredding)
		{
			float num = Time.get_realtimeSinceStartup() - shredStartTime;
			float num2 = num / shredDurationPosition;
			float num3 = num / shredDurationRotation;
			((Component)shredRail).get_transform().set_localPosition(Vector3.Lerp(shredRailStartPos.get_localPosition(), shredRailEndPos.get_localPosition(), num2));
			((Component)shredRail).get_transform().set_rotation(Quaternion.LookRotation(Vector3.Lerp(shredRailStartRotation, shredRailEndRotation, num3)));
			MagnetLiftable component = ((Component)currentlyShredding).GetComponent<MagnetLiftable>();
			((Component)currentlyShredding).get_transform().set_position(((Component)shredRail).get_transform().get_position());
			Vector3 val = ((Component)this).get_transform().TransformDirection(component.shredDirection);
			if (Vector3.Dot(-val, ((Component)currentlyShredding).get_transform().get_forward()) > Vector3.Dot(val, ((Component)currentlyShredding).get_transform().get_forward()))
			{
				val = ((Component)this).get_transform().TransformDirection(-component.shredDirection);
			}
			bool flag = Vector3.Dot(((Component)component).get_transform().get_up(), Vector3.get_up()) >= -0.95f;
			Quaternion val2 = QuaternionEx.LookRotationForcedUp(val, flag ? (-((Component)this).get_transform().get_right()) : ((Component)this).get_transform().get_right());
			float num4 = Time.get_time() * shredSwaySpeed;
			float num5 = Mathf.PerlinNoise(num4, 0f);
			float num6 = Mathf.PerlinNoise(0f, num4 + 150f);
			val2 *= Quaternion.Euler(num5 * shredSwayAmount, 0f, num6 * shredSwayAmount);
			((Component)currentlyShredding).get_transform().set_rotation(Quaternion.Lerp(entryRotation, val2, num3));
			if (num > 5f)
			{
				CreateShredResources();
				currentlyShredding.Kill();
				currentlyShredding = null;
				isShredding = false;
				SetShredding(isShredding: false);
			}
		}
	}

	private void Update()
	{
		ServerUpdate();
	}
}
