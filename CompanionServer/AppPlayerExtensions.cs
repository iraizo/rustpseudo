using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer
{
	public static class AppPlayerExtensions
	{
		public static AppTeamInfo GetAppTeamInfo(this BasePlayer player, ulong steamId)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			AppTeamInfo obj = Pool.Get<AppTeamInfo>();
			obj.members = Pool.GetList<Member>();
			Member val = Pool.Get<Member>();
			if ((Object)(object)player != (Object)null)
			{
				Vector2 val2 = Util.WorldToMap(((Component)player).get_transform().get_position());
				val.steamId = player.userID;
				val.name = player.displayName ?? "";
				val.x = val2.x;
				val.y = val2.y;
				val.isOnline = player.IsConnected;
				val.spawnTime = player.lifeStory?.timeBorn ?? 0;
				val.isAlive = player.IsAlive();
				val.deathTime = player.previousLifeStory?.timeDied ?? 0;
			}
			else
			{
				val.steamId = steamId;
				val.name = SingletonComponent<ServerMgr>.Instance.persistance.GetPlayerName(steamId) ?? "";
				val.x = 0f;
				val.y = 0f;
				val.isOnline = false;
				val.spawnTime = 0u;
				val.isAlive = false;
				val.deathTime = 0u;
			}
			obj.members.Add(val);
			obj.leaderSteamId = 0uL;
			obj.mapNotes = GetMapNotes(player, personalNotes: true);
			obj.leaderMapNotes = GetMapNotes(null, personalNotes: false);
			return obj;
		}

		public static AppTeamInfo GetAppTeamInfo(this RelationshipManager.PlayerTeam team, ulong requesterSteamId)
		{
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			AppTeamInfo val = Pool.Get<AppTeamInfo>();
			val.members = Pool.GetList<Member>();
			BasePlayer player = null;
			BasePlayer basePlayer = null;
			for (int i = 0; i < team.members.Count; i++)
			{
				ulong num = team.members[i];
				BasePlayer basePlayer2 = RelationshipManager.FindByID(num);
				if (!Object.op_Implicit((Object)(object)basePlayer2))
				{
					basePlayer2 = null;
				}
				if (num == requesterSteamId)
				{
					player = basePlayer2;
				}
				if (num == team.teamLeader)
				{
					basePlayer = basePlayer2;
				}
				Vector2 val2 = Util.WorldToMap((basePlayer2 != null) ? ((Component)basePlayer2).get_transform().get_position() : Vector3.get_zero());
				Member val3 = Pool.Get<Member>();
				val3.steamId = num;
				val3.name = basePlayer2?.displayName ?? SingletonComponent<ServerMgr>.Instance.persistance.GetPlayerName(num) ?? "";
				val3.x = val2.x;
				val3.y = val2.y;
				val3.isOnline = basePlayer2?.IsConnected ?? false;
				val3.spawnTime = basePlayer2?.lifeStory?.timeBorn ?? 0;
				val3.isAlive = basePlayer2?.IsAlive() ?? false;
				val3.deathTime = basePlayer2?.previousLifeStory?.timeDied ?? 0;
				val.members.Add(val3);
			}
			val.leaderSteamId = team.teamLeader;
			val.mapNotes = GetMapNotes(player, personalNotes: true);
			val.leaderMapNotes = GetMapNotes((requesterSteamId != team.teamLeader) ? basePlayer : null, personalNotes: false);
			return val;
		}

		private static List<Note> GetMapNotes(BasePlayer player, bool personalNotes)
		{
			List<Note> list = Pool.GetList<Note>();
			if ((Object)(object)player != (Object)null)
			{
				if (personalNotes && player.ServerCurrentDeathNote != null)
				{
					AddMapNote(list, player.ServerCurrentDeathNote, BasePlayer.MapNoteType.Death);
				}
				if (player.ServerCurrentMapNote != null)
				{
					AddMapNote(list, player.ServerCurrentMapNote, BasePlayer.MapNoteType.PointOfInterest);
				}
			}
			return list;
		}

		private static void AddMapNote(List<Note> result, MapNote note, BasePlayer.MapNoteType type)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = Util.WorldToMap(note.worldPosition);
			Note val2 = Pool.Get<Note>();
			val2.type = (int)type;
			val2.x = val.x;
			val2.y = val.y;
			result.Add(val2);
		}
	}
}
