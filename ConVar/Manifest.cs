using Facepunch;

namespace ConVar
{
	public class Manifest
	{
		[ClientVar]
		[ServerVar]
		public static object PrintManifest()
		{
			return Application.Manifest;
		}

		[ClientVar]
		[ServerVar]
		public static object PrintManifestRaw()
		{
			return Manifest.get_Contents();
		}
	}
}
