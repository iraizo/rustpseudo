using System;
using UnityEngine;

public class WaterCatcher : LiquidContainer
{
	[Header("Water Catcher")]
	public ItemDefinition itemToCreate;

	public float maxItemToCreate = 10f;

	[Header("Outside Test")]
	public Vector3 rainTestPosition = new Vector3(0f, 1f, 0f);

	public float rainTestSize = 1f;

	private const float collectInterval = 60f;

	public override void ServerInit()
	{
		base.ServerInit();
		AddResource(1);
		((FacepunchBehaviour)this).InvokeRandomized((Action)CollectWater, 60f, 60f, 6f);
	}

	private void CollectWater()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (!IsFull())
		{
			float num = 0.25f;
			num += Climate.GetFog(((Component)this).get_transform().get_position()) * 2f;
			if (TestIsOutside())
			{
				num += Climate.GetRain(((Component)this).get_transform().get_position());
				num += Climate.GetSnow(((Component)this).get_transform().get_position()) * 0.5f;
			}
			AddResource(Mathf.CeilToInt(maxItemToCreate * num));
		}
	}

	private bool IsFull()
	{
		if (base.inventory.itemList.Count == 0)
		{
			return false;
		}
		if (base.inventory.itemList[0].amount < base.inventory.maxStackSize)
		{
			return false;
		}
		return true;
	}

	private bool TestIsOutside()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Matrix4x4 localToWorldMatrix = ((Component)this).get_transform().get_localToWorldMatrix();
		return !Physics.SphereCast(new Ray(((Matrix4x4)(ref localToWorldMatrix)).MultiplyPoint3x4(rainTestPosition), Vector3.get_up()), rainTestSize, 256f, 161546513);
	}

	private void AddResource(int iAmount)
	{
		base.inventory.AddItem(itemToCreate, iAmount, 0uL);
		UpdateOnFlag();
	}
}
