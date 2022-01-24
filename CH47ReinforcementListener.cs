using UnityEngine;

public class CH47ReinforcementListener : BaseEntity
{
	public string listenString;

	public GameObjectRef heliPrefab;

	public float startDist = 300f;

	public override void OnEntityMessage(BaseEntity from, string msg)
	{
		if (msg == listenString)
		{
			Call();
		}
	}

	public void Call()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		CH47HelicopterAIController component = ((Component)GameManager.server.CreateEntity(heliPrefab.resourcePath)).GetComponent<CH47HelicopterAIController>();
		if (Object.op_Implicit((Object)(object)component))
		{
			_ = TerrainMeta.Size;
			CH47LandingZone closest = CH47LandingZone.GetClosest(((Component)this).get_transform().get_position());
			Vector3 zero = Vector3.get_zero();
			zero.y = ((Component)closest).get_transform().get_position().y;
			Vector3 val = Vector3Ex.Direction2D(((Component)closest).get_transform().get_position(), zero);
			Vector3 position = ((Component)closest).get_transform().get_position() + val * startDist;
			position.y = ((Component)closest).get_transform().get_position().y;
			((Component)component).get_transform().set_position(position);
			component.SetLandingTarget(((Component)closest).get_transform().get_position());
			component.Spawn();
		}
	}
}
