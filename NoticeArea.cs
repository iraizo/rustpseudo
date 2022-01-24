using UnityEngine;

public class NoticeArea : SingletonComponent<NoticeArea>
{
	public GameObjectRef itemPickupPrefab;

	public GameObjectRef itemPickupCondensedText;

	public GameObjectRef itemDroppedPrefab;

	private IVitalNotice[] notices;

	protected override void Awake()
	{
		((SingletonComponent)this).Awake();
		notices = ((Component)this).GetComponentsInChildren<IVitalNotice>(true);
	}
}
