using Facepunch;
using ProtoBuf;

namespace CompanionServer.Handlers
{
	public interface IHandler : IPooled
	{
		AppRequest Request { get; }

		ValidationResult Validate();

		void Execute();

		void SendError(string code);
	}
}
