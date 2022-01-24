using ConVar;
using UnityEngine;

public class NPCBarricadeTriggerBox : MonoBehaviour
{
	private Barricade target;

	private static int playerServerLayer = -1;

	public void Setup(Barricade t)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		target = t;
		((Component)this).get_transform().SetParent(((Component)target).get_transform(), false);
		((Component)this).get_gameObject().set_layer(18);
		BoxCollider obj = ((Component)this).get_gameObject().AddComponent<BoxCollider>();
		((Collider)obj).set_isTrigger(true);
		obj.set_center(Vector3.get_zero());
		obj.set_size(Vector3.get_one() * AI.npc_door_trigger_size + Vector3.get_right() * ((Bounds)(ref target.bounds)).get_size().x);
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((Object)(object)target == (Object)null || target.isClient)
		{
			return;
		}
		if (playerServerLayer < 0)
		{
			playerServerLayer = LayerMask.NameToLayer("Player (Server)");
		}
		if ((((Component)other).get_gameObject().get_layer() & playerServerLayer) > 0)
		{
			BasePlayer component = ((Component)other).get_gameObject().GetComponent<BasePlayer>();
			if ((Object)(object)component != (Object)null && component.IsNpc && !(component is BasePet))
			{
				target.Kill(BaseNetworkable.DestroyMode.Gib);
			}
		}
	}

	public NPCBarricadeTriggerBox()
		: this()
	{
	}
}
