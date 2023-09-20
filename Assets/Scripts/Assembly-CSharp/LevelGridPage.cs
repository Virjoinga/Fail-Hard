using System.Collections.Generic;
using Game;
using UnityEngine;

public class LevelGridPage : MonoBehaviour
{
	public GameObject LevelDelegatePrefab;

	public GameObject LoadingScreen;

	public UILabel PageTitle;

	public GameObject StageLock;

	public UILabel LockLabel;

	private List<GameObject> m_delegates;

	private BundleStage m_stage;

	private Bundle m_bundle;

	private Transform m_loadingScreenParent;

	public void SetData(Bundle bundle, BundleStage stage, bool isLocked, Transform loadingScreenParent, int unlockCriteria = 0, int startIndex = 0)
	{
		GameController instance = GameController.Instance;
		m_stage = stage;
		m_bundle = bundle;
		m_loadingScreenParent = loadingScreenParent;
		PageTitle.text = "STAGE " + (m_stage.Index + 1);
		if (m_delegates == null)
		{
			m_delegates = new List<GameObject>();
		}
		else
		{
			m_delegates.Clear();
		}
		int num = Mathf.Min(12, stage.Levels.Count - startIndex);
		for (int i = startIndex; i < num; i++)
		{
			Level level = instance.LevelDatabase.LevelForName(stage.Levels[i].LevelId);
			if (level != null)
			{
				GameObject gameObject = NGUITools.AddChild(base.gameObject, LevelDelegatePrefab);
				gameObject.GetComponent<LevelListDelegate>().SetData(level, false);
				gameObject.GetComponent<LevelListDelegate>().ItemSelected += OnLevelSelected;
				m_delegates.Add(gameObject);
				gameObject.name = "Level" + (1000 + i);
			}
			else
			{
				Debug.LogWarning("Level not found from database: " + stage.Levels[i].LevelId);
			}
		}
		GetComponent<UIGrid>().Reposition();
		StageLock.SetActive(isLocked);
		int num2 = unlockCriteria - bundle.AchievedTargets;
		if (num2 < 0)
		{
			num2 = 0;
		}
		LockLabel.text = num2.ToString();
	}

	public void AnimateUnlock()
	{
		m_bundle.LastAnimatedStage++;
		StageLock.SetActive(true);
		StageLock.GetComponent<StageLock>().Hide();
	}

	public void OnDisable()
	{
		if (m_delegates == null)
		{
			return;
		}
		foreach (GameObject @delegate in m_delegates)
		{
			Object.Destroy(@delegate);
		}
		m_delegates.Clear();
	}

	public void OnLevelSelected(LevelListDelegate data)
	{
		if (!data.m_level.IsLocked)
		{
			GameController.Instance.BundleModel.SetNowPlaying((Level)data.m_level);
			GameObject gameObject = NGUITools.AddChild(m_loadingScreenParent.gameObject, LoadingScreen);
			if (UICamera.current != null)
			{
				UICamera.current.eventReceiverMask = Layers.Nothing;
			}
			gameObject.transform.localPosition -= Vector3.forward * 10f;
		}
	}
}
