using System.Collections.Generic;
using System.Linq;
using ConVar;
using UnityEngine;

public class CombatLog
{
	public struct Event
	{
		public float time;

		public uint attacker_id;

		public uint target_id;

		public string attacker;

		public string target;

		public string weapon;

		public string ammo;

		public string bone;

		public HitArea area;

		public float distance;

		public float health_old;

		public float health_new;

		public string info;

		public int proj_hits;

		public float proj_integrity;

		public float proj_travel;

		public float proj_mismatch;
	}

	private const string selfname = "you";

	private const string noname = "N/A";

	private BasePlayer player;

	private Queue<Event> storage;

	private static Dictionary<ulong, Queue<Event>> players = new Dictionary<ulong, Queue<Event>>();

	public float LastActive { get; private set; }

	public CombatLog(BasePlayer player)
	{
		this.player = player;
	}

	public void Init()
	{
		storage = Get(player.userID);
		LastActive = storage.LastOrDefault().time;
	}

	public void Save()
	{
	}

	public void Log(AttackEntity weapon, string description = null)
	{
		Log(weapon, null, description);
	}

	public void Log(AttackEntity weapon, Projectile projectile, string description = null)
	{
		Event val = default(Event);
		val.time = Time.get_realtimeSinceStartup();
		val.attacker_id = ((Object.op_Implicit((Object)(object)player) && player.net != null) ? player.net.ID : 0u);
		val.target_id = 0u;
		val.attacker = "you";
		val.target = "N/A";
		val.weapon = (Object.op_Implicit((Object)(object)weapon) ? ((Object)weapon).get_name() : "N/A");
		val.ammo = (Object.op_Implicit((Object)(object)projectile) ? ((Object)projectile).get_name() : "N/A");
		val.bone = "N/A";
		val.area = (HitArea)0;
		val.distance = 0f;
		val.health_old = 0f;
		val.health_new = 0f;
		val.info = ((description != null) ? description : string.Empty);
		Log(val);
	}

	public void Log(HitInfo info, string description = null)
	{
		float num = (Object.op_Implicit((Object)(object)info.HitEntity) ? info.HitEntity.Health() : 0f);
		Log(info, num, num, description);
	}

	public void Log(HitInfo info, float health_old, float health_new, string description = null)
	{
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		Event val = default(Event);
		val.time = Time.get_realtimeSinceStartup();
		val.attacker_id = ((Object.op_Implicit((Object)(object)info.Initiator) && info.Initiator.net != null) ? info.Initiator.net.ID : 0u);
		val.target_id = ((Object.op_Implicit((Object)(object)info.HitEntity) && info.HitEntity.net != null) ? info.HitEntity.net.ID : 0u);
		val.attacker = (((Object)(object)player == (Object)(object)info.Initiator) ? "you" : (Object.op_Implicit((Object)(object)info.Initiator) ? info.Initiator.ShortPrefabName : "N/A"));
		val.target = (((Object)(object)player == (Object)(object)info.HitEntity) ? "you" : (Object.op_Implicit((Object)(object)info.HitEntity) ? info.HitEntity.ShortPrefabName : "N/A"));
		val.weapon = (Object.op_Implicit((Object)(object)info.WeaponPrefab) ? ((Object)info.WeaponPrefab).get_name() : "N/A");
		val.ammo = (Object.op_Implicit((Object)(object)info.ProjectilePrefab) ? ((Object)info.ProjectilePrefab).get_name() : "N/A");
		val.bone = info.boneName;
		val.area = info.boneArea;
		val.distance = (info.IsProjectile() ? info.ProjectileDistance : Vector3.Distance(info.PointStart, info.HitPositionWorld));
		val.health_old = health_old;
		val.health_new = health_new;
		val.info = ((description != null) ? description : string.Empty);
		val.proj_hits = info.ProjectileHits;
		val.proj_integrity = info.ProjectileIntegrity;
		val.proj_travel = info.ProjectileTravelTime;
		val.proj_mismatch = info.ProjectileTrajectoryMismatch;
		Log(val);
	}

	public void Log(Event val)
	{
		LastActive = Time.get_realtimeSinceStartup();
		if (storage != null)
		{
			storage.Enqueue(val);
			int num = Mathf.Max(0, Server.combatlogsize);
			while (storage.Count > num)
			{
				storage.Dequeue();
			}
		}
	}

	public string Get(int count, uint filterByAttacker = 0u)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		if (storage == null)
		{
			return string.Empty;
		}
		if (storage.Count == 0)
		{
			return "Combat log empty.";
		}
		TextTable val = new TextTable();
		val.AddColumn("time");
		val.AddColumn("attacker");
		val.AddColumn("id");
		val.AddColumn("target");
		val.AddColumn("id");
		val.AddColumn("weapon");
		val.AddColumn("ammo");
		val.AddColumn("area");
		val.AddColumn("distance");
		val.AddColumn("old_hp");
		val.AddColumn("new_hp");
		val.AddColumn("info");
		val.AddColumn("hits");
		val.AddColumn("integrity");
		val.AddColumn("travel");
		val.AddColumn("mismatch");
		int num = storage.Count - count;
		int combatlogdelay = Server.combatlogdelay;
		int num2 = 0;
		foreach (Event item in storage)
		{
			if (num > 0)
			{
				num--;
			}
			else if (filterByAttacker == 0 || item.attacker_id == filterByAttacker)
			{
				float num3 = Time.get_realtimeSinceStartup() - item.time;
				if (num3 >= (float)combatlogdelay)
				{
					string text = num3.ToString("0.00s");
					string attacker = item.attacker;
					uint attacker_id = item.attacker_id;
					string text2 = attacker_id.ToString();
					string target = item.target;
					attacker_id = item.target_id;
					string text3 = attacker_id.ToString();
					string weapon = item.weapon;
					string ammo = item.ammo;
					string text4 = HitAreaUtil.Format(item.area).ToLower();
					float distance = item.distance;
					string text5 = distance.ToString("0.0m");
					distance = item.health_old;
					string text6 = distance.ToString("0.0");
					distance = item.health_new;
					string text7 = distance.ToString("0.0");
					string info = item.info;
					int proj_hits = item.proj_hits;
					string text8 = proj_hits.ToString();
					distance = item.proj_integrity;
					string text9 = distance.ToString("0.00");
					distance = item.proj_travel;
					string text10 = distance.ToString("0.00s");
					distance = item.proj_mismatch;
					string text11 = distance.ToString("0.00m");
					val.AddRow(new string[16]
					{
						text, attacker, text2, target, text3, weapon, ammo, text4, text5, text6,
						text7, info, text8, text9, text10, text11
					});
				}
				else
				{
					num2++;
				}
			}
		}
		string text12 = ((object)val).ToString();
		if (num2 > 0)
		{
			text12 = text12 + "+ " + num2 + " " + ((num2 > 1) ? "events" : "event");
			text12 = text12 + " in the last " + combatlogdelay + " " + ((combatlogdelay > 1) ? "seconds" : "second");
		}
		return text12;
	}

	public static Queue<Event> Get(ulong id)
	{
		if (players.TryGetValue(id, out var value))
		{
			return value;
		}
		value = new Queue<Event>();
		players.Add(id, value);
		return value;
	}
}
