using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using CompanionServer;
using ConVar;
using Facepunch;
using Facepunch.Network;
using Facepunch.Network.Raknet;
using Facepunch.Utility;
using Network;
using Rust;
using Rust.Ai;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Bootstrap : SingletonComponent<Bootstrap>
{
	internal static bool bootstrapInitRun;

	public static bool isErrored;

	public string messageString = "Loading...";

	public CanvasGroup BootstrapUiCanvas;

	public GameObject errorPanel;

	public TextMeshProUGUI errorText;

	public TextMeshProUGUI statusText;

	private static string lastWrittenValue;

	public static bool needsSetup => !bootstrapInitRun;

	public static bool isPresent
	{
		get
		{
			if (bootstrapInitRun)
			{
				return true;
			}
			if (Enumerable.Count<GameSetup>((IEnumerable<GameSetup>)Object.FindObjectsOfType<GameSetup>()) > 0)
			{
				return true;
			}
			return false;
		}
	}

	public static void RunDefaults()
	{
		Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
		Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
		Application.set_targetFrameRate(256);
		Time.set_fixedDeltaTime(0.0625f);
		Time.set_maximumDeltaTime(0.125f);
	}

	public static void Init_Tier0()
	{
		RunDefaults();
		GameSetup.RunOnce = true;
		bootstrapInitRun = true;
		Index.Initialize(ConsoleGen.All);
		UnityButtons.Register();
		Output.Install();
		Pool.ResizeBuffer<Networkable>(65536);
		Pool.ResizeBuffer<EntityLink>(65536);
		Pool.FillBuffer<Networkable>(int.MaxValue);
		Pool.FillBuffer<EntityLink>(int.MaxValue);
		SteamNetworking.SetDebugFunction();
		if (CommandLine.HasSwitch("-swnet"))
		{
			NetworkInitSteamworks(enableSteamDatagramRelay: false);
		}
		else if (CommandLine.HasSwitch("-sdrnet"))
		{
			NetworkInitSteamworks(enableSteamDatagramRelay: true);
		}
		else
		{
			NetworkInitRaknet();
		}
		if (!Application.get_isEditor())
		{
			string text = CommandLine.get_Full().Replace(CommandLine.GetSwitch("-rcon.password", CommandLine.GetSwitch("+rcon.password", "RCONPASSWORD")), "******");
			WriteToLog("Command Line: " + text);
		}
	}

	public static void Init_Systems()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Expected O, but got Unknown
		Global.Init();
		Application.Initialize((BaseIntegration)new Integration());
		Performance.GetMemoryUsage = () => SystemInfoEx.systemMemoryUsed;
	}

	public static void Init_Config()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		ConsoleNetwork.Init();
		ConsoleSystem.UpdateValuesFromCommandLine();
		ConsoleSystem.Run(Option.get_Server(), "server.readcfg", Array.Empty<object>());
		ServerUsers.Load();
	}

	public static void NetworkInitRaknet()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Expected O, but got Unknown
		Net.sv = (Server)new Server();
	}

	public static void NetworkInitSteamworks(bool enableSteamDatagramRelay)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		Net.sv = (Server)new Server(enableSteamDatagramRelay);
	}

	private IEnumerator Start()
	{
		WriteToLog("Bootstrap Startup");
		BenchmarkTimer.Enabled = CommandLine.get_Full().Contains("+autobench");
		BenchmarkTimer timer = BenchmarkTimer.New("bootstrap");
		if (!Application.get_isEditor())
		{
			ExceptionReporter.InitializeFromUrl("https://83df169465e84da091c1a3cd2fbffeee:3671b903f9a840ecb68411cf946ab9b6@sentry.io/51080");
			ExceptionReporter.set_Disabled(!CommandLine.get_Full().Contains("-official") && !CommandLine.get_Full().Contains("-server.official") && !CommandLine.get_Full().Contains("+official") && !CommandLine.get_Full().Contains("+server.official"));
			BuildInfo current = BuildInfo.get_Current();
			if (current.get_Scm().get_Branch() != null && current.get_Scm().get_Branch().StartsWith("main"))
			{
				ExceptionReporter.InitializeFromUrl("https://0654eb77d1e04d6babad83201b6b6b95:d2098f1d15834cae90501548bd5dbd0d@sentry.io/1836389");
				ExceptionReporter.set_Disabled(false);
			}
		}
		BenchmarkTimer val;
		BenchmarkTimer val2;
		if (AssetBundleBackend.get_Enabled())
		{
			AssetBundleBackend newBackend = new AssetBundleBackend();
			val = BenchmarkTimer.New("bootstrap;bundles");
			try
			{
				yield return ((MonoBehaviour)this).StartCoroutine(LoadingUpdate("Opening Bundles"));
				newBackend.Load("Bundles/Bundles");
				FileSystem.Backend = (FileSystemBackend)(object)newBackend;
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
			if (FileSystem.Backend.isError)
			{
				ThrowError(FileSystem.Backend.loadingError);
				yield break;
			}
			val2 = BenchmarkTimer.New("bootstrap;bundlesindex");
			try
			{
				newBackend.BuildFileIndex();
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		if (FileSystem.Backend.isError)
		{
			ThrowError(FileSystem.Backend.loadingError);
			yield break;
		}
		if (!Application.get_isEditor())
		{
			WriteToLog(SystemInfoGeneralText.currentInfo);
		}
		Texture.SetGlobalAnisotropicFilteringLimits(1, 16);
		if (isErrored)
		{
			yield break;
		}
		val = BenchmarkTimer.New("bootstrap;gamemanifest");
		try
		{
			yield return ((MonoBehaviour)this).StartCoroutine(LoadingUpdate("Loading Game Manifest"));
			GameManifest.Load();
			yield return ((MonoBehaviour)this).StartCoroutine(LoadingUpdate("DONE!"));
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		val = BenchmarkTimer.New("bootstrap;selfcheck");
		try
		{
			yield return ((MonoBehaviour)this).StartCoroutine(LoadingUpdate("Running Self Check"));
			SelfCheck.Run();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		if (isErrored)
		{
			yield break;
		}
		yield return ((MonoBehaviour)this).StartCoroutine(LoadingUpdate("Bootstrap Tier0"));
		val2 = BenchmarkTimer.New("bootstrap;tier0");
		try
		{
			Init_Tier0();
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		val2 = BenchmarkTimer.New("bootstrap;commandlinevalues");
		try
		{
			ConsoleSystem.UpdateValuesFromCommandLine();
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		yield return ((MonoBehaviour)this).StartCoroutine(LoadingUpdate("Bootstrap Systems"));
		val2 = BenchmarkTimer.New("bootstrap;init_systems");
		try
		{
			Init_Systems();
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		yield return ((MonoBehaviour)this).StartCoroutine(LoadingUpdate("Bootstrap Config"));
		val2 = BenchmarkTimer.New("bootstrap;init_config");
		try
		{
			Init_Config();
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		if (isErrored)
		{
			yield break;
		}
		yield return ((MonoBehaviour)this).StartCoroutine(LoadingUpdate("Loading Items"));
		val2 = BenchmarkTimer.New("bootstrap;itemmanager");
		try
		{
			ItemManager.Initialize();
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		if (!isErrored)
		{
			yield return ((MonoBehaviour)this).StartCoroutine(DedicatedServerStartup());
			if (timer != null)
			{
				timer.Dispose();
			}
			GameManager.Destroy(((Component)this).get_gameObject());
		}
	}

	private IEnumerator DedicatedServerStartup()
	{
		Application.isLoading = true;
		WriteToLog("Skinnable Warmup");
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		GameManifest.LoadAssets();
		WriteToLog("Loading Scene");
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		Physics.set_solverIterationCount(3);
		int @int = PlayerPrefs.GetInt("UnityGraphicsQuality");
		QualitySettings.SetQualityLevel(0);
		PlayerPrefs.SetInt("UnityGraphicsQuality", @int);
		Object.DontDestroyOnLoad((Object)(object)((Component)this).get_gameObject());
		Object.DontDestroyOnLoad((Object)(object)GameManager.server.CreatePrefab("assets/bundled/prefabs/system/server_console.prefab"));
		StartupShared();
		World.InitSize(ConVar.Server.worldsize);
		World.InitSeed(ConVar.Server.seed);
		World.InitSalt(ConVar.Server.salt);
		World.Url = ConVar.Server.levelurl;
		World.Transfer = ConVar.Server.leveltransfer;
		LevelManager.LoadLevel(ConVar.Server.level);
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		yield return ((MonoBehaviour)this).StartCoroutine(FileSystem_Warmup.Run(2f, WriteToLog, "Asset Warmup ({0}/{1})"));
		yield return ((MonoBehaviour)this).StartCoroutine(StartServer(!CommandLine.HasSwitch("-skipload"), "", allowOutOfDateSaves: false));
		if (!Object.op_Implicit((Object)(object)Object.FindObjectOfType<Performance>()))
		{
			Object.DontDestroyOnLoad((Object)(object)GameManager.server.CreatePrefab("assets/bundled/prefabs/system/performance.prefab"));
		}
		Pool.Clear();
		Rust.GC.Collect();
		Application.isLoading = false;
	}

	public static IEnumerator StartServer(bool doLoad, string saveFileOverride, bool allowOutOfDateSaves)
	{
		float timeScale = Time.get_timeScale();
		if (Time.pausewhileloading)
		{
			Time.set_timeScale(0f);
		}
		RCon.Initialize();
		BaseEntity.Query.Server = new BaseEntity.Query.EntityTree(8096f);
		if (Object.op_Implicit((Object)(object)SingletonComponent<WorldSetup>.Instance))
		{
			yield return ((MonoBehaviour)SingletonComponent<WorldSetup>.Instance).StartCoroutine(SingletonComponent<WorldSetup>.Instance.InitCoroutine());
		}
		if (Object.op_Implicit((Object)(object)SingletonComponent<DynamicNavMesh>.Instance) && ((Behaviour)SingletonComponent<DynamicNavMesh>.Instance).get_enabled() && !AiManager.nav_disable)
		{
			yield return ((MonoBehaviour)SingletonComponent<DynamicNavMesh>.Instance).StartCoroutine(SingletonComponent<DynamicNavMesh>.Instance.UpdateNavMeshAndWait());
		}
		if (Object.op_Implicit((Object)(object)SingletonComponent<AiManager>.Instance) && ((Behaviour)SingletonComponent<AiManager>.Instance).get_enabled())
		{
			SingletonComponent<AiManager>.Instance.Initialize();
			if (!AiManager.nav_disable && AI.npc_enable && (Object)(object)TerrainMeta.Path != (Object)null)
			{
				foreach (MonumentInfo monument in TerrainMeta.Path.Monuments)
				{
					if (monument.HasNavmesh)
					{
						yield return ((MonoBehaviour)monument).StartCoroutine(monument.GetMonumentNavMesh().UpdateNavMeshAndWait());
					}
				}
				if (Object.op_Implicit((Object)(object)TerrainMeta.Path) && Object.op_Implicit((Object)(object)TerrainMeta.Path.DungeonGridRoot))
				{
					DungeonNavmesh dungeonNavmesh = TerrainMeta.Path.DungeonGridRoot.AddComponent<DungeonNavmesh>();
					dungeonNavmesh.NavMeshCollectGeometry = (NavMeshCollectGeometry)1;
					dungeonNavmesh.LayerMask = LayerMask.op_Implicit(65537);
					yield return ((MonoBehaviour)dungeonNavmesh).StartCoroutine(dungeonNavmesh.UpdateNavMeshAndWait());
				}
				else
				{
					Debug.LogError((object)"Failed to find DungeonGridRoot, NOT generating Dungeon navmesh");
				}
				if (Object.op_Implicit((Object)(object)TerrainMeta.Path) && Object.op_Implicit((Object)(object)TerrainMeta.Path.DungeonBaseRoot))
				{
					DungeonNavmesh dungeonNavmesh2 = TerrainMeta.Path.DungeonBaseRoot.AddComponent<DungeonNavmesh>();
					dungeonNavmesh2.NavmeshResolutionModifier = 0.3f;
					dungeonNavmesh2.NavMeshCollectGeometry = (NavMeshCollectGeometry)1;
					dungeonNavmesh2.LayerMask = LayerMask.op_Implicit(65537);
					yield return ((MonoBehaviour)dungeonNavmesh2).StartCoroutine(dungeonNavmesh2.UpdateNavMeshAndWait());
				}
				else
				{
					Debug.LogError((object)"Failed to find DungeonBaseRoot , NOT generating Dungeon navmesh");
				}
				GenerateDungeonBase.SetupAI();
			}
		}
		GameObject val = GameManager.server.CreatePrefab("assets/bundled/prefabs/system/server.prefab");
		Object.DontDestroyOnLoad((Object)(object)val);
		ServerMgr serverMgr = val.GetComponent<ServerMgr>();
		yield return NexusServer.Initialize();
		if (NexusServer.FailedToStart)
		{
			Debug.LogError((object)"Nexus server failed to start, terminating");
			Application.Quit();
			yield break;
		}
		bool saveWasLoaded = serverMgr.Initialize(doLoad, saveFileOverride, allowOutOfDateSaves);
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		SaveRestore.InitializeEntityLinks();
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		SaveRestore.InitializeEntitySupports();
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		SaveRestore.InitializeEntityConditionals();
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		SaveRestore.GetSaveCache();
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		BaseGameMode.CreateGameMode();
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		if (NexusServer.Started)
		{
			NexusServer.UpdateIslands();
			if (saveWasLoaded)
			{
				NexusServer.RestoreUnsavedState();
			}
		}
		serverMgr.OpenConnection();
		CompanionServer.Server.Initialize();
		BenchmarkTimer val2 = BenchmarkTimer.New("Boombox.LoadStations");
		try
		{
			BoomBox.LoadStations();
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		if (Time.pausewhileloading)
		{
			Time.set_timeScale(timeScale);
		}
		WriteToLog("Server startup complete");
	}

	private void StartupShared()
	{
		ItemManager.Initialize();
	}

	public void ThrowError(string error)
	{
		Debug.Log((object)("ThrowError: " + error));
		errorPanel.SetActive(true);
		((TMP_Text)errorText).set_text(error);
		isErrored = true;
	}

	public void ExitGame()
	{
		Debug.Log((object)"Exiting due to Exit Game button on bootstrap error panel");
		Application.Quit();
	}

	public static IEnumerator LoadingUpdate(string str)
	{
		if (Object.op_Implicit((Object)(object)SingletonComponent<Bootstrap>.Instance))
		{
			SingletonComponent<Bootstrap>.Instance.messageString = str;
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
		}
	}

	public static void WriteToLog(string str)
	{
		if (!(lastWrittenValue == str))
		{
			DebugEx.Log((object)str, (StackTraceLogType)0);
		}
	}
}
