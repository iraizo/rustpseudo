using System.Collections;
using ConVar;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSetup : MonoBehaviour
{
	public static bool RunOnce;

	public bool startServer = true;

	public string clientConnectCommand = "client.connect 127.0.0.1:28015";

	public bool loadMenu = true;

	public bool loadLevel;

	public string loadLevelScene = "";

	public bool loadSave;

	public string loadSaveFile = "";

	protected void Awake()
	{
		if (RunOnce)
		{
			GameManager.Destroy(((Component)this).get_gameObject());
			return;
		}
		GameManifest.Load();
		GameManifest.LoadAssets();
		RunOnce = true;
		if (Bootstrap.needsSetup)
		{
			Bootstrap.Init_Tier0();
			Bootstrap.Init_Systems();
			Bootstrap.Init_Config();
		}
		((MonoBehaviour)this).StartCoroutine(DoGameSetup());
	}

	private IEnumerator DoGameSetup()
	{
		Application.isLoading = true;
		TerrainMeta.InitNoTerrain();
		ItemManager.Initialize();
		Scene activeScene = SceneManager.GetActiveScene();
		LevelManager.CurrentLevelName = ((Scene)(ref activeScene)).get_name();
		if (loadLevel && !string.IsNullOrEmpty(loadLevelScene))
		{
			Net.sv.Reset();
			ConVar.Server.level = loadLevelScene;
			LoadingScreen.Update("LOADING SCENE");
			Application.LoadLevelAdditive(loadLevelScene);
			LoadingScreen.Update(loadLevelScene.ToUpper() + " LOADED");
		}
		if (startServer)
		{
			yield return ((MonoBehaviour)this).StartCoroutine(StartServer());
		}
		yield return null;
		Application.isLoading = false;
	}

	private IEnumerator StartServer()
	{
		ConVar.GC.collect();
		ConVar.GC.unload();
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		yield return ((MonoBehaviour)this).StartCoroutine(Bootstrap.StartServer(loadSave, loadSaveFile, allowOutOfDateSaves: true));
	}

	public GameSetup()
		: this()
	{
	}
}
