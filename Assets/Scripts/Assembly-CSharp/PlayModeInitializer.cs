using Game;
using Game.IO;
using Game.Util;
using UnityEngine;

public class PlayModeInitializer : MonoBehaviour
{
	public string levelName;

	public int TargetFrameRate = 30;

	public bool generateId;

	private GameObject root;

	public void Awake()
	{
		root = GameObject.Find("Level:root");
		if (root.transform.childCount != 0)
		{
			Storage.ForceOffline = true;
			LocalStorage.Init();
			LocalStorage.Instance.Disabled = true;
		}
		GameController instance = GameController.Instance;
		if (instance.CurrentLevel == null)
		{
			instance.CurrentLevel = LoadSingleLevel(levelName);
			instance.CurrentBundle = instance.BundleModel.GetBundle(instance.CurrentLevel);
			CloudStorageST.GenerateId = generateId;
			instance.Character.CurrentVehicle = instance.VehicleModel.Model[1];
		}
		else
		{
			instance.CurrentLevel.Reset();
		}
		if (root.transform.childCount == 0)
		{
			LevelLoader levelLoader = new LevelLoader(new RuntimePrefabInstantiator());
			LevelParameters parameters = instance.CurrentLevel.Parameters;
			Logger.Log("Trying to load level contents: " + parameters.Name);
			if (parameters == null || parameters.Name.Length == 0)
			{
				Logger.Error("No level name is set in info!");
			}
			else
			{
				levelLoader.LoadLevel(parameters, LevelFormat.bytes);
			}
		}
		else
		{
			Logger.Warning("NOT LOADING LEVEL CONTENTS! Reason: Level:root already contains gameobjects! We're in editor? Otherwise this is a bug!");
		}
		CombineStaticMeshes();
		SetFrameRate();
	}

	private void Start()
	{
		GameObject gameObject = GameObject.Find("SceneryHandle");
		GameObject gameObject2 = GameObject.Find("/Scenery");
		if (gameObject != null && gameObject2 != null)
		{
			gameObject2.transform.rotation = gameObject.transform.rotation;
		}
	}

	private void CombineStaticMeshes()
	{
		GameObject gameObject = GameObject.Find("/Scenery/Static");
		if (gameObject != null)
		{
			StaticBatchingUtility.Combine(gameObject);
		}
		else
		{
			Debug.LogWarning("Static batching failed. Didn't find Scenery/Static path");
		}
		GameObject gameObject2 = GameObject.Find("/Level:root/Gameplay/Static");
		if ((bool)gameObject2)
		{
			StaticBatchingUtility.Combine(gameObject2);
		}
		else
		{
			Debug.LogWarning("Static batching for level objects could not be done: /Level:root/Gameplay/Static");
		}
	}

	private void SetFrameRate()
	{
		Application.targetFrameRate = TargetFrameRate;
	}

	private Level LoadSingleLevel(string name)
	{
		TextAsset textAsset = Resources.Load("Levels/" + name, typeof(TextAsset)) as TextAsset;
		Level result = null;
		if ((bool)textAsset)
		{
			ILevelReader levelReader = LevelReaderFactory.Construct(LevelFormat.bytes);
			LevelParameters parameters = levelReader.ReadInfo(textAsset);
			result = new Level(parameters);
		}
		return result;
	}
}
