using System.Collections.Generic;
using Facepunch;
using ProtoBuf;

namespace CompanionServer.Handlers
{
	public class TeamChat : BaseHandler<AppEmpty>
	{
		public override void Execute()
		{
			RelationshipManager.PlayerTeam playerTeam = RelationshipManager.ServerInstance.FindPlayersTeam(base.UserId);
			if (playerTeam == null)
			{
				SendError("no_team");
				return;
			}
			AppResponse val = Pool.Get<AppResponse>();
			val.teamChat = Pool.Get<AppTeamChat>();
			val.teamChat.messages = Pool.GetList<AppChatMessage>();
			IReadOnlyList<ChatLog.Entry> history = Server.TeamChat.GetHistory(playerTeam.teamID);
			if (history != null)
			{
				foreach (ChatLog.Entry item in history)
				{
					AppChatMessage val2 = Pool.Get<AppChatMessage>();
					val2.steamId = item.SteamId;
					val2.name = item.Name;
					val2.message = item.Message;
					val2.color = item.Color;
					val2.time = item.Time;
					val.teamChat.messages.Add(val2);
				}
			}
			Send(val);
		}
	}
}
