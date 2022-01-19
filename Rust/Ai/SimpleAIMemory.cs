using System.Collections.Generic;
using ConVar;
using UnityEngine;

namespace Rust.AI
{
	public class SimpleAIMemory
	{
		public struct SeenInfo
		{
			public BaseEntity Entity;

			public Vector3 Position;

			public float Timestamp;

			public float Danger;
		}

		public List<SeenInfo> All = new List<SeenInfo>();

		public List<BaseEntity> Players = new List<BaseEntity>();

		public HashSet<BaseEntity> LOS = new HashSet<BaseEntity>();

		public List<BaseEntity> Targets = new List<BaseEntity>();

		public List<BaseEntity> Threats = new List<BaseEntity>();

		public List<BaseEntity> Friendlies = new List<BaseEntity>();

		public void SetKnown(BaseEntity ent, BaseEntity owner, AIBrainSenses brainSenses)
		{
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			IAISenses iAISenses = owner as IAISenses;
			bool flag = false;
			if (iAISenses != null && iAISenses.IsThreat(ent))
			{
				flag = true;
				if (brainSenses != null)
				{
					brainSenses.LastThreatTimestamp = Time.get_realtimeSinceStartup();
				}
			}
			for (int i = 0; i < All.Count; i++)
			{
				if ((Object)(object)All[i].Entity == (Object)(object)ent)
				{
					SeenInfo value = All[i];
					value.Position = ((Component)ent).get_transform().get_position();
					value.Timestamp = Mathf.Max(Time.get_realtimeSinceStartup(), value.Timestamp);
					All[i] = value;
					return;
				}
			}
			BasePlayer basePlayer = ent as BasePlayer;
			if ((Object)(object)basePlayer != (Object)null)
			{
				if (ConVar.AI.ignoreplayers && !basePlayer.IsNpc)
				{
					return;
				}
				Players.Add(ent);
			}
			if (iAISenses != null)
			{
				if (iAISenses.IsTarget(ent))
				{
					Targets.Add(ent);
				}
				if (iAISenses.IsFriendly(ent))
				{
					Friendlies.Add(ent);
				}
				if (flag)
				{
					Threats.Add(ent);
				}
			}
			All.Add(new SeenInfo
			{
				Entity = ent,
				Position = ((Component)ent).get_transform().get_position(),
				Timestamp = Time.get_realtimeSinceStartup()
			});
		}

		public void SetLOS(BaseEntity ent, bool flag)
		{
			if (!((Object)(object)ent == (Object)null))
			{
				if (flag)
				{
					LOS.Add(ent);
				}
				else
				{
					LOS.Remove(ent);
				}
			}
		}

		public bool IsLOS(BaseEntity ent)
		{
			return LOS.Contains(ent);
		}

		public bool IsPlayerKnown(BasePlayer player)
		{
			return Players.Contains(player);
		}

		internal void Forget(float secondsOld)
		{
			for (int i = 0; i < All.Count; i++)
			{
				if (!(Time.get_realtimeSinceStartup() - All[i].Timestamp > secondsOld))
				{
					continue;
				}
				BaseEntity entity = All[i].Entity;
				if ((Object)(object)entity != (Object)null)
				{
					if (entity is BasePlayer)
					{
						Players.Remove(entity);
					}
					Targets.Remove(entity);
					Threats.Remove(entity);
					Friendlies.Remove(entity);
					LOS.Remove(entity);
				}
				All.RemoveAt(i);
				i--;
			}
		}
	}
}
