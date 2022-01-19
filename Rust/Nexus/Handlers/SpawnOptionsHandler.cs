using Facepunch;
using ProtoBuf;
using ProtoBuf.Nexus;

namespace Rust.Nexus.Handlers
{
	public class SpawnOptionsHandler : BaseNexusRequestHandler<SpawnOptionsRequest>
	{
		protected override void Handle()
		{
			Response val = BaseNexusRequestHandler<SpawnOptionsRequest>.NewResponse();
			val.spawnOptions = Pool.Get<SpawnOptionsResponse>();
			val.spawnOptions.spawnOptions = Pool.GetList<SpawnOptions>();
			BasePlayer.GetRespawnOptionsForPlayer(val.spawnOptions.spawnOptions, base.Request.userId);
			SendSuccess(val);
		}
	}
}
