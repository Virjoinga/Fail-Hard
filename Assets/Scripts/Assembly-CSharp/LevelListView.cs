using System.Collections.Generic;
using Game;
using Game.Progress;
using UnityEngine;

public class LevelListView : MonoBehaviour
{
	public GameObject levelDelegatePrefab;

	public GameObject lockedStagePrefab;

	public UIGrid grid;

	public UIDraggablePanel draggablePanel;

	private ILevel[] model;

	private List<string> LevelsIds;

	private List<GameObject> delegates;

	public string modelContents;

	public GameObject loadingScreen;

	private void Start()
	{
	}

	private GameObject CreateGridItem(ILevel level, bool animateUnlock)
	{
		GameObject gameObject = Object.Instantiate(levelDelegatePrefab, base.transform.position, base.transform.rotation) as GameObject;
		gameObject.transform.parent = grid.transform;
		gameObject.transform.localScale = Vector3.one;
		gameObject.GetComponent<LevelListDelegate>().SetData(level, animateUnlock);
		gameObject.GetComponent<LevelListDelegate>().ItemSelected += OnLevelSelected;
		return gameObject;
	}

	private void InitRecommendedView()
	{
		GameController instance = GameController.Instance;
		bool flag = false;
		model = instance.Agent.PromotedLevels();
		for (int i = 0; i < model.GetLength(0); i++)
		{
			if (!(instance.CurrentLevel.Parameters.Name == model[i].Parameters.Name))
			{
				GameObject gameObject = CreateGridItem(model[i], true);
				delegates.Add(gameObject);
				gameObject.name = "Level" + (999 - ((Level)model[i]).UnlockingIndex);
				if (!((Level)model[i]).UnlockShown)
				{
					flag = true;
				}
			}
		}
		if (draggablePanel != null && flag)
		{
			draggablePanel.ResetPosition();
			draggablePanel.MoveRelative(grid.cellHeight * Vector3.up);
			SpringPanel.Begin(draggablePanel.gameObject, draggablePanel.transform.localPosition - grid.cellHeight * Vector3.up, 4f);
		}
	}

	private void InitBundleView()
	{
		GameController instance = GameController.Instance;
		int currentStage = instance.CurrentBundle.CurrentStage;
		int num = 10000;
		foreach (BundleStage stage in instance.CurrentBundle.Stages)
		{
			if (stage.Index > currentStage)
			{
				GameObject gameObject = SetupLockedStageIcon(stage);
				delegates.Add(gameObject);
				gameObject.name = "LockedStage" + num++;
				continue;
			}
			foreach (BundleLevel level2 in stage.Levels)
			{
				string levelId = level2.LevelId;
				Level level = instance.LevelDatabase.LevelForName(levelId);
				if (level != null)
				{
					GameObject gameObject2 = CreateGridItem(level, false);
					delegates.Add(gameObject2);
					gameObject2.name = "Level" + num++;
				}
				else
				{
					Debug.LogError("Level not found from database: " + levelId);
				}
			}
		}
	}

	private GameObject SetupLockedStageIcon(BundleStage s)
	{
		GameObject gameObject = Object.Instantiate(lockedStagePrefab, base.transform.position, base.transform.rotation) as GameObject;
		gameObject.transform.parent = grid.transform;
		gameObject.transform.localScale = Vector3.one;
		gameObject.GetComponent<LockedStage>().SetData(s, GameController.Instance.CurrentBundle);
		return gameObject;
	}

	public void InitModel()
	{
		if (modelContents == "Recommended")
		{
			InitRecommendedView();
		}
		else
		{
			InitBundleView();
		}
		grid.Reposition();
	}

	public void AnimateUnlocks()
	{
		foreach (GameObject @delegate in delegates)
		{
			@delegate.GetComponent<LevelListDelegate>().AnimateUnlock();
		}
	}

	private Texture2D SelectBackground(ThemeCategory theme)
	{
		string path;
		switch (theme)
		{
		case ThemeCategory.Backyard:
			path = "Levels/suburban";
			break;
		case ThemeCategory.ShoppingMall:
			path = "Levels/city";
			break;
		case ThemeCategory.Movie:
			path = "Levels/beach";
			break;
		default:
			path = "Levels/suburban";
			break;
		}
		return (Texture2D)Resources.Load(path);
	}

	public void OnEnable()
	{
		if (delegates == null)
		{
			delegates = new List<GameObject>();
		}
		if (LevelsIds == null)
		{
			LevelsIds = new List<string>();
		}
		Invoke("InitModel", 0.01f);
	}

	public void OnDisable()
	{
		if (delegates != null)
		{
			foreach (GameObject @delegate in delegates)
			{
				Object.Destroy(@delegate);
			}
			delegates.Clear();
		}
		if (LevelsIds != null)
		{
			LevelsIds.Clear();
		}
	}

	public void OnLevelSelected(LevelListDelegate data)
	{
		if (!data.m_level.IsLocked || GameController.Instance.MyCurrentTheme == ThemeCategory.Development)
		{
			GameController.Instance.SetLevelToLoad(data.m_level);
			GameController.Instance.CurrentBundle = GameController.Instance.BundleModel.GetBundle((Level)data.m_level);
			GameObject gameObject = NGUITools.AddChild(base.transform.parent.parent.parent.gameObject, loadingScreen);
			gameObject.transform.localPosition -= Vector3.forward;
		}
	}
}
