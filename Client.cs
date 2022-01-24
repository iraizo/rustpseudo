public class Client : SingletonComponent<Client>
{
	public static Phrase loading_loading = new Phrase("loading.loading", "Loading");

	public static Phrase loading_connecting = new Phrase("loading.connecting", "Connecting");

	public static Phrase loading_connectionaccepted = new Phrase("loading.connectionaccepted", "Connection Accepted");

	public static Phrase loading_connecting_negotiate = new Phrase("loading.connecting.negotiate", "Negotiating Connection");

	public static Phrase loading_level = new Phrase("loading.loadinglevel", "Loading Level");

	public static Phrase loading_skinnablewarmup = new Phrase("loading.skinnablewarmup", "Skinnable Warmup");

	public static Phrase loading_preloadcomplete = new Phrase("loading.preloadcomplete", "Preload Complete");

	public static Phrase loading_openingscene = new Phrase("loading.openingscene", "Opening Scene");

	public static Phrase loading_clientready = new Phrase("loading.clientready", "Client Ready");

	public static Phrase loading_prefabwarmup = new Phrase("loading.prefabwarmup", "Warming Prefabs [{0}/{1}]");
}
