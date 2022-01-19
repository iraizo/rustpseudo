using Facepunch;

namespace Rust.Nexus.Handlers
{
	public interface INexusRequestHandler : IPooled
	{
		void Execute();
	}
}
