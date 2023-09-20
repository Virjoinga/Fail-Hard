using System.Collections.Generic;
using Game;
using Holoville.HOTween;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
	public GameObject Oneliner;

	public List<GameObject> TutorialPrefabs;

	public GameObject Loading;

	private int m_progress;

	public UILabel ProgressLabel;

	public Transform Rotate;

	private AsyncOperation m_asyncLoad;

	private void Start()
	{
		HardCodedTutorialSelection();
	}

	private void OnEnable()
	{
		Invoke("Load", 0.1f);
		RotateTween();
	}

	private void Update()
	{
		if (m_asyncLoad != null)
		{
			if ((int)(100f * m_asyncLoad.progress) > m_progress)
			{
				m_progress = (int)(100f * m_asyncLoad.progress);
				ProgressLabel.text = m_progress + "%";
			}
			if (m_asyncLoad.isDone)
			{
				Object.Destroy(base.transform.root.gameObject);
			}
		}
	}

	private void Load()
	{
		m_asyncLoad = GameController.Instance.EnterPlayMode(true);
		if (m_asyncLoad != null)
		{
			m_asyncLoad.allowSceneActivation = true;
		}
	}

	private void RotateTween()
	{
		Rotate.transform.localRotation = Quaternion.identity;
		TweenParms p_parms = new TweenParms().Prop("localRotation", new Vector3(0f, 0f, 360f), true).Loops(-1).Ease(EaseType.Linear);
		HOTween.To(Rotate, 1f, p_parms);
	}

	private void HardCodedTutorialSelection()
	{
		bool flag = false;
		Level currentLevel = GameController.Instance.CurrentLevel;
		if (currentLevel.Parameters.Name == "GravelPitLoop")
		{
			NGUITools.AddChild(base.gameObject, TutorialPrefabs[0]);
			flag = true;
		}
		else if (currentLevel.Parameters.Name == "GravelPitCrateTruck")
		{
			NGUITools.AddChild(base.gameObject, TutorialPrefabs[1]);
			flag = true;
		}
		else if (currentLevel.Parameters.Name == "GravelPitMattresses")
		{
			NGUITools.AddChild(base.gameObject, TutorialPrefabs[2]);
			flag = true;
		}
		else if (currentLevel.Parameters.Name == "GravelPitGutterDown")
		{
			NGUITools.AddChild(base.gameObject, TutorialPrefabs[3]);
			flag = true;
		}
		else if (currentLevel.Parameters.Name == "SnowSkiJump1")
		{
			NGUITools.AddChild(base.gameObject, TutorialPrefabs[4]);
			flag = true;
		}
		if (flag)
		{
			Oneliner.SetActive(false);
		}
	}
}
