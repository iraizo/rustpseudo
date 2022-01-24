namespace ConVar
{
	[Factory("voice")]
	public class Voice : ConsoleSystem
	{
		[ClientVar(Saved = true)]
		public static bool loopback;

		public Voice()
			: this()
		{
		}
	}
}
