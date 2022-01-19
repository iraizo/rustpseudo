using System.Collections.Generic;
using UnityEngine;

public class TriggerComfort : TriggerBase
{
	public float triggerSize;

	public float baseComfort = 0.5f;

	public float minComfortRange = 2.5f;

	private const float perPlayerComfortBonus = 0.25f;

	private const float bonusComfort = 0f;

	private List<BasePlayer> _players = new List<BasePlayer>();

	private void OnValidate()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		triggerSize = ((Component)this).GetComponent<SphereCollider>().get_radius() * ((Component)this).get_transform().get_localScale().y;
	}

	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if ((Object)(object)obj == (Object)null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if ((Object)(object)baseEntity == (Object)null)
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		return ((Component)baseEntity).get_gameObject();
	}

	public float CalculateComfort(Vector3 position, BasePlayer forPlayer = null)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		float num = Vector3.Distance(((Component)this).get_gameObject().get_transform().get_position(), position);
		float num2 = 1f - Mathf.Clamp(num - minComfortRange, 0f, num / (triggerSize - minComfortRange));
		float num3 = 0f;
		foreach (BasePlayer player in _players)
		{
			if (!((Object)(object)player == (Object)(object)forPlayer))
			{
				num3 += 0.25f * (player.IsSleeping() ? 0.5f : 1f) * (player.IsAlive() ? 1f : 0f);
			}
		}
		float num4 = 0f + num3;
		return (baseComfort + num4) * num2;
	}

	internal override void OnEntityEnter(BaseEntity ent)
	{
		BasePlayer basePlayer = ent as BasePlayer;
		if (Object.op_Implicit((Object)(object)basePlayer))
		{
			_players.Add(basePlayer);
		}
	}

	internal override void OnEntityLeave(BaseEntity ent)
	{
		BasePlayer basePlayer = ent as BasePlayer;
		if (Object.op_Implicit((Object)(object)basePlayer))
		{
			_players.Remove(basePlayer);
		}
	}
}
