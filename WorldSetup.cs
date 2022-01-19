using System.Collections;
using System.Collections.Generic;
using System.IO;
using ConVar;
using Rust;
using UnityEngine;
using UnityEngine.Networking;

public class WorldSetup : SingletonComponent<WorldSetup>
{
	public bool AutomaticallySetup;

	public GameObject terrain;

	public GameObject decorPrefab;

	public GameObject grassPrefab;

	public GameObject spawnPrefab;

	private TerrainMeta terrainMeta;

	public uint EditorSeed;

	public uint EditorSalt;

	public uint EditorSize;

	public string EditorUrl = string.Empty;

	internal List<ProceduralObject> ProceduralObjects = new List<ProceduralObject>();

	internal List<MonumentNode> MonumentNodes = new List<MonumentNode>();

	private void OnValidate()
	{
		if ((Object)(object)terrain == (Object)null)
		{
			Terrain val = Object.FindObjectOfType<Terrain>();
			if ((Object)(object)val != (Object)null)
			{
				terrain = ((Component)val).get_gameObject();
			}
		}
	}

	protected override void Awake()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Expected O, but got Unknown
		((SingletonComponent)this).Awake();
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/world");
		foreach (Prefab prefab in array)
		{
			if ((Object)(object)prefab.Object.GetComponent<BaseEntity>() != (Object)null)
			{
				prefab.SpawnEntity(Vector3.get_zero(), Quaternion.get_identity()).Spawn();
			}
			else
			{
				prefab.Spawn(Vector3.get_zero(), Quaternion.get_identity());
			}
		}
		SingletonComponent[] array2 = Object.FindObjectsOfType<SingletonComponent>();
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].SingletonSetup();
		}
		if (Object.op_Implicit((Object)(object)terrain))
		{
			if (Object.op_Implicit((Object)(object)terrain.GetComponent<TerrainGenerator>()))
			{
				World.Procedural = true;
			}
			else
			{
				World.Procedural = false;
				terrainMeta = terrain.GetComponent<TerrainMeta>();
				terrainMeta.Init();
				terrainMeta.SetupComponents();
				terrainMeta.BindShaderProperties();
				terrainMeta.PostSetupComponents();
				World.InitSize(Mathf.RoundToInt(TerrainMeta.Size.x));
				CreateObject(decorPrefab);
				CreateObject(grassPrefab);
				CreateObject(spawnPrefab);
			}
		}
		World.Serialization = new WorldSerialization();
		World.Cached = false;
		World.CleanupOldFiles();
		if (AutomaticallySetup)
		{
			((MonoBehaviour)this).StartCoroutine(InitCoroutine());
		}
	}

	protected void CreateObject(GameObject prefab)
	{
		if (!((Object)(object)prefab == (Object)null))
		{
			GameObject val = Object.Instantiate<GameObject>(prefab);
			if ((Object)(object)val != (Object)null)
			{
				val.SetActive(true);
			}
		}
	}

	public IEnumerator InitCoroutine()
	{
		if (World.CanLoadFromUrl())
		{
			Debug.Log((object)("Loading custom map from " + World.Url));
		}
		else
		{
			Debug.Log((object)("Generating procedural map of size " + World.Size + " with seed " + World.Seed));
		}
		ProceduralComponent[] components = ((Component)this).GetComponentsInChildren<ProceduralComponent>(true);
		Timing downloadTimer = Timing.Start("Downloading World");
		if (World.Procedural && !World.CanLoadFromDisk() && World.CanLoadFromUrl())
		{
			LoadingScreen.Update("DOWNLOADING WORLD");
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			UnityWebRequest request = UnityWebRequest.Get(World.Url);
			request.set_downloadHandler((DownloadHandler)new DownloadHandlerBuffer());
			request.Send();
			while (!request.get_isDone())
			{
				LoadingScreen.Update("DOWNLOADING WORLD " + (request.get_downloadProgress() * 100f).ToString("0.0") + "%");
				yield return CoroutineEx.waitForEndOfFrame;
			}
			if (!request.get_isHttpError() && !request.get_isNetworkError())
			{
				File.WriteAllBytes(World.MapFolderName + "/" + World.MapFileName, request.get_downloadHandler().get_data());
			}
			else
			{
				CancelSetup("Couldn't Download Level: " + World.Name + " (" + request.get_error() + ")");
			}
		}
		downloadTimer.End();
		Timing loadTimer = Timing.Start("Loading World");
		if (World.Procedural && World.CanLoadFromDisk())
		{
			LoadingScreen.Update("LOADING WORLD");
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			World.Serialization.Load(World.MapFolderName + "/" + World.MapFileName);
			World.Cached = true;
		}
		loadTimer.End();
		if (World.Cached && 9 != World.Serialization.get_Version())
		{
			Debug.LogWarning((object)("World cache version mismatch: " + 9u + " != " + World.Serialization.get_Version()));
			World.Serialization.Clear();
			World.Cached = false;
			if (World.CanLoadFromUrl())
			{
				CancelSetup("World File Outdated: " + World.Name);
			}
		}
		if (World.Cached && string.IsNullOrEmpty(World.Checksum))
		{
			World.Checksum = World.Serialization.get_Checksum();
		}
		if (World.Cached)
		{
			World.InitSize(World.Serialization.world.size);
		}
		if (Object.op_Implicit((Object)(object)terrain))
		{
			TerrainGenerator component2 = terrain.GetComponent<TerrainGenerator>();
			if (Object.op_Implicit((Object)(object)component2))
			{
				if (World.Cached)
				{
					int heightmapResolution = Mathf.RoundToInt(Mathf.Sqrt((float)(World.GetMap("height").Length / 2)));
					int alphamapResolution = Mathf.RoundToInt(Mathf.Sqrt((float)(World.GetMap("splat").Length / 8)));
					terrain = component2.CreateTerrain(heightmapResolution, alphamapResolution);
				}
				else
				{
					terrain = component2.CreateTerrain();
				}
				terrainMeta = terrain.GetComponent<TerrainMeta>();
				terrainMeta.Init();
				terrainMeta.SetupComponents();
				CreateObject(decorPrefab);
				CreateObject(grassPrefab);
				CreateObject(spawnPrefab);
			}
		}
		Timing spawnTimer = Timing.Start("Spawning World");
		if (World.Cached)
		{
			LoadingScreen.Update("SPAWNING WORLD");
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			TerrainMeta.HeightMap.FromByteArray(World.GetMap("terrain"));
			TerrainMeta.SplatMap.FromByteArray(World.GetMap("splat"));
			TerrainMeta.BiomeMap.FromByteArray(World.GetMap("biome"));
			TerrainMeta.TopologyMap.FromByteArray(World.GetMap("topology"));
			TerrainMeta.AlphaMap.FromByteArray(World.GetMap("alpha"));
			TerrainMeta.WaterMap.FromByteArray(World.GetMap("water"));
			IEnumerator worldSpawn = World.Spawn(0.2f, delegate(string str)
			{
				LoadingScreen.Update(str);
			});
			while (worldSpawn.MoveNext())
			{
				yield return worldSpawn.Current;
			}
			TerrainMeta.Path.Clear();
			TerrainMeta.Path.Roads.AddRange(World.GetPaths("Road"));
			TerrainMeta.Path.Rivers.AddRange(World.GetPaths("River"));
			TerrainMeta.Path.Powerlines.AddRange(World.GetPaths("Powerline"));
		}
		spawnTimer.End();
		Timing procgenTimer = Timing.Start("Processing World");
		if (components.Length != 0)
		{
			for (int i = 0; i < components.Length; i++)
			{
				ProceduralComponent component = components[i];
				if (Object.op_Implicit((Object)(object)component) && component.ShouldRun())
				{
					uint seed = (uint)(World.Seed + i);
					LoadingScreen.Update(component.Description.ToUpper());
					yield return CoroutineEx.waitForEndOfFrame;
					yield return CoroutineEx.waitForEndOfFrame;
					yield return CoroutineEx.waitForEndOfFrame;
					Timing timing = Timing.Start(component.Description);
					if (Object.op_Implicit((Object)(object)component))
					{
						component.Process(seed);
					}
					timing.End();
				}
			}
		}
		procgenTimer.End();
		Timing saveTimer = Timing.Start("Saving World");
		if (ConVar.World.cache && World.Procedural && !World.Cached)
		{
			LoadingScreen.Update("SAVING WORLD");
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			World.Serialization.world.size = World.Size;
			World.AddPaths(TerrainMeta.Path.Roads);
			World.AddPaths(TerrainMeta.Path.Rivers);
			World.AddPaths(TerrainMeta.Path.Powerlines);
			World.Serialization.Save(World.MapFolderName + "/" + World.MapFileName);
		}
		saveTimer.End();
		Timing checksumTimer = Timing.Start("Calculating Checksum");
		if (string.IsNullOrEmpty(World.Serialization.get_Checksum()))
		{
			LoadingScreen.Update("CALCULATING CHECKSUM");
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			World.Serialization.CalculateChecksum();
		}
		checksumTimer.End();
		if (string.IsNullOrEmpty(World.Checksum))
		{
			World.Checksum = World.Serialization.get_Checksum();
		}
		Timing oceanTimer = Timing.Start("Ocean Patrol Paths");
		LoadingScreen.Update("OCEAN PATROL PATHS");
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		if (BaseBoat.generate_paths && (Object)(object)TerrainMeta.Path != (Object)null)
		{
			TerrainMeta.Path.OceanPatrolFar = BaseBoat.GenerateOceanPatrolPath(200f);
		}
		else
		{
			Debug.Log((object)"Skipping ocean patrol paths, baseboat.generate_paths == false");
		}
		oceanTimer.End();
		Timing finalizeTimer = Timing.Start("Finalizing World");
		LoadingScreen.Update("FINALIZING WORLD");
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		if (Object.op_Implicit((Object)(object)terrainMeta))
		{
			terrainMeta.BindShaderProperties();
			terrainMeta.PostSetupComponents();
			TerrainMargin.Create();
		}
		finalizeTimer.End();
		Timing cleaningTimer = Timing.Start("Cleaning Up");
		LoadingScreen.Update("CLEANING UP");
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		foreach (string item in FileSystem.Backend.UnloadBundles("monuments"))
		{
			GameManager.server.preProcessed.Invalidate(item);
			GameManifest.Invalidate(item);
			PrefabAttribute.server.Invalidate(StringPool.Get(item));
		}
		Resources.UnloadUnusedAssets();
		cleaningTimer.End();
		LoadingScreen.Update("DONE");
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		if (Object.op_Implicit((Object)(object)this))
		{
			GameManager.Destroy(((Component)this).get_gameObject());
		}
	}

	private void CancelSetup(string msg)
	{
		Debug.LogError((object)msg);
		Application.Quit();
	}
}
