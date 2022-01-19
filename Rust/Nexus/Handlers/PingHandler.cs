using ConVar;
using Facepunch;
using ProtoBuf.Nexus;

namespace Rust.Nexus.Handlers
{
	public class PingHandler : BaseNexusRequestHandler<PingRequest>
	{
		protected override void Handle()
		{
			Response val = BaseNexusRequestHandler<PingRequest>.NewResponse();
			val.ping = Pool.Get<PingResponse>();
			val.ping.players = BasePlayer.activePlayerList.get_Count();
			val.ping.maxPlayers = ConVar.Server.maxplayers;
			val.ping.queuedPlayers = SingletonComponent<ServerMgr>.Instance.connectionQueue.Queued;
			SendSuccess(val);
		}
	}
}
