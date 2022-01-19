using ConVar;
using Facepunch;
using Facepunch.Math;
using ProtoBuf;

namespace CompanionServer.Handlers
{
	public class Info : BaseHandler<AppEmpty>
	{
		public override void Execute()
		{
			AppInfo val = Pool.Get<AppInfo>();
			val.name = ConVar.Server.hostname;
			val.headerImage = ConVar.Server.headerimage;
			val.logoImage = ConVar.Server.logoimage;
			val.url = ConVar.Server.url;
			val.map = World.Name;
			val.mapSize = World.Size;
			val.wipeTime = (uint)Epoch.FromDateTime(SaveRestore.SaveCreatedTime.ToUniversalTime());
			val.players = (uint)BasePlayer.activePlayerList.get_Count();
			val.maxPlayers = (uint)ConVar.Server.maxplayers;
			val.queuedPlayers = (uint)SingletonComponent<ServerMgr>.Instance.connectionQueue.Queued;
			val.seed = World.Seed;
			val.salt = World.Salt;
			AppResponse val2 = Pool.Get<AppResponse>();
			val2.info = val;
			Send(val2);
		}
	}
}
