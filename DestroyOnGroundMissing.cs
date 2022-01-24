using UnityEngine;

public class DestroyOnGroundMissing : MonoBehaviour, IServerComponent
{
	private void OnGroundMissing()
	{
		BaseEntity baseEntity = ((Component)this).get_gameObject().ToBaseEntity();
		if ((Object)(object)baseEntity != (Object)null)
		{
			BaseCombatEntity baseCombatEntity = baseEntity as BaseCombatEntity;
			if ((Object)(object)baseCombatEntity != (Object)null)
			{
				baseCombatEntity.Die();
			}
			else
			{
				baseEntity.Kill(BaseNetworkable.DestroyMode.Gib);
			}
		}
	}

	public DestroyOnGroundMissing()
		: this()
	{
	}
}
