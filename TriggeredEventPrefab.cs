using UnityEngine;

public class TriggeredEventPrefab : TriggeredEvent
{
	public GameObjectRef targetPrefab;

	private void RunEvent()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		Debug.Log((object)("[event] " + targetPrefab.resourcePath));
		BaseEntity baseEntity = GameManager.server.CreateEntity(targetPrefab.resourcePath);
		if (Object.op_Implicit((Object)(object)baseEntity))
		{
			((Component)baseEntity).SendMessage("TriggeredEventSpawn", (SendMessageOptions)1);
			baseEntity.Spawn();
		}
	}
}
