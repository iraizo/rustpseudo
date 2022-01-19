using UnityEngine;

public class JunkPileWater : JunkPile
{
	public class JunkpileWaterWorkQueue : ObjectWorkQueue<JunkPileWater>
	{
		protected override void RunJob(JunkPileWater entity)
		{
			if (((ObjectWorkQueue<JunkPileWater>)this).ShouldAdd(entity))
			{
				entity.UpdateNearbyPlayers();
			}
		}

		protected override bool ShouldAdd(JunkPileWater entity)
		{
			if (base.ShouldAdd(entity))
			{
				return entity.IsValid();
			}
			return false;
		}
	}

	public static JunkpileWaterWorkQueue junkpileWaterWorkQueue = new JunkpileWaterWorkQueue();

	[ServerVar]
	[Help("How many milliseconds to budget for processing life story updates per frame")]
	public static float framebudgetms = 0.25f;

	public Transform[] buoyancyPoints;

	public bool debugDraw;

	private Quaternion baseRotation = Quaternion.get_identity();

	private bool first = true;

	private TimeUntil nextPlayerCheck;

	private bool hasPlayersNearby;

	public override void Spawn()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Component)this).get_transform().get_position();
		position.y = TerrainMeta.WaterMap.GetHeight(((Component)this).get_transform().get_position());
		((Component)this).get_transform().set_position(position);
		base.Spawn();
		Quaternion rotation = ((Component)this).get_transform().get_rotation();
		baseRotation = Quaternion.Euler(0f, ((Quaternion)(ref rotation)).get_eulerAngles().y, 0f);
	}

	public void FixedUpdate()
	{
		if (!base.isClient)
		{
			UpdateMovement();
		}
	}

	public void UpdateMovement()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		if (TimeUntil.op_Implicit(nextPlayerCheck) <= 0f)
		{
			nextPlayerCheck = TimeUntil.op_Implicit(Random.Range(0.5f, 1f));
			((ObjectWorkQueue<JunkPileWater>)junkpileWaterWorkQueue).Add(this);
		}
		if (isSinking || !hasPlayersNearby)
		{
			return;
		}
		float height = WaterSystem.GetHeight(((Component)this).get_transform().get_position());
		((Component)this).get_transform().set_position(new Vector3(((Component)this).get_transform().get_position().x, height, ((Component)this).get_transform().get_position().z));
		if (buoyancyPoints != null && buoyancyPoints.Length >= 3)
		{
			Vector3 position = ((Component)this).get_transform().get_position();
			Vector3 localPosition = buoyancyPoints[0].get_localPosition();
			Vector3 localPosition2 = buoyancyPoints[1].get_localPosition();
			Vector3 localPosition3 = buoyancyPoints[2].get_localPosition();
			Vector3 val = localPosition + position;
			Vector3 val2 = localPosition2 + position;
			Vector3 val3 = localPosition3 + position;
			val.y = WaterSystem.GetHeight(val);
			val2.y = WaterSystem.GetHeight(val2);
			val3.y = WaterSystem.GetHeight(val3);
			Vector3 val4 = default(Vector3);
			((Vector3)(ref val4))._002Ector(position.x, val.y - localPosition.y, position.z);
			Vector3 val5 = val2 - val;
			Vector3 val6 = Vector3.Cross(val3 - val, val5);
			Quaternion val7 = Quaternion.LookRotation(new Vector3(val6.x, val6.z, val6.y));
			Vector3 eulerAngles = ((Quaternion)(ref val7)).get_eulerAngles();
			val7 = Quaternion.Euler(0f - eulerAngles.x, 0f, 0f - eulerAngles.y);
			if (first)
			{
				Quaternion rotation = ((Component)this).get_transform().get_rotation();
				baseRotation = Quaternion.Euler(0f, ((Quaternion)(ref rotation)).get_eulerAngles().y, 0f);
				first = false;
			}
			((Component)this).get_transform().SetPositionAndRotation(val4, val7 * baseRotation);
		}
	}

	public void UpdateNearbyPlayers()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		hasPlayersNearby = BaseNetworkable.HasCloseConnections(((Component)this).get_transform().get_position(), 16f);
	}
}
