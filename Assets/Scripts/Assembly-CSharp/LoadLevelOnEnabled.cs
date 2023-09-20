using Game;
using Holoville.HOTween;
using UnityEngine;

public class LoadLevelOnEnabled : MonoBehaviour
{
	public UILabel ProgressLabel;

	public Transform Rotate;

	private AsyncOperation m_asyncLoad;

	private void OnEnable()
	{
		Invoke("Load", 0.1f);
		RotateTween();
	}

	private void Update()
	{
		if (m_asyncLoad != null)
		{
			ProgressLabel.text = (int)(100f * m_asyncLoad.progress) + "%";
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
}
