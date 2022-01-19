using ConVar;
using UnityEngine;

public class NPCDoorTriggerBox : MonoBehaviour
{
	private Door door;

	private static int playerServerLayer = -1;

	public void Setup(Door d)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		door = d;
		((Component)this).get_transform().SetParent(((Component)door).get_transform(), false);
		((Component)this).get_gameObject().set_layer(18);
		BoxCollider obj = ((Component)this).get_gameObject().AddComponent<BoxCollider>();
		((Collider)obj).set_isTrigger(true);
		obj.set_center(Vector3.get_zero());
		obj.set_size(Vector3.get_one() * AI.npc_door_trigger_size);
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((Object)(object)door == (Object)null || door.isClient || door.IsLocked() || (!door.isSecurityDoor && door.IsOpen()) || (door.isSecurityDoor && !door.IsOpen()))
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
			if ((Object)(object)component != (Object)null && component.IsNpc && !door.isSecurityDoor)
			{
				door.SetOpen(open: true);
			}
		}
	}

	public NPCDoorTriggerBox()
		: this()
	{
	}
}
