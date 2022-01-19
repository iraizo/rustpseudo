using System.Collections.Generic;
using UnityEngine;

public class NexusTransferTrigger : BaseMonoBehaviour, IServerComponent
{
	[Tooltip("Must implement INexusTransferTriggerController!")]
	public MonoBehaviour Controller;

	private static readonly HashSet<BaseEntity> PendingEntities = new HashSet<BaseEntity>();

	private INexusTransferTriggerController _controller;

	protected void Start()
	{
		_controller = Controller as INexusTransferTriggerController;
		if (_controller == null)
		{
			Debug.LogError((object)"NexusTransferTrigger doesn't have a valid controller assigned!", (Object)(object)this);
		}
	}

	protected void OnTriggerEnter(Collider other)
	{
		if (_controller == null)
		{
			return;
		}
		var (zoneName, method) = _controller.GetTransferDestination();
		if (string.IsNullOrEmpty(zoneName))
		{
			return;
		}
		BaseEntity entity = ((Component)other).get_gameObject().ToBaseEntity();
		if (!((Object)(object)entity == (Object)null))
		{
			BaseEntity baseEntity = NexusServer.FindRootEntity(entity);
			if (_controller.CanTransfer(baseEntity) && PendingEntities.Add(baseEntity))
			{
				TransferAndWait();
			}
		}
		async void TransferAndWait()
		{
			try
			{
				await NexusServer.TransferEntity(entity, zoneName, method);
			}
			finally
			{
				PendingEntities.Remove(entity);
			}
		}
	}
}
