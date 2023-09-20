using Game;
using UnityEngine;

public class NextButton : MonoBehaviour
{
	public GameObject LoadingScreen;

	public GameObject ButtonSprite;

	public Transform LoadingScreenParent;

	private Level m_nextLevel;

	private bool m_nextOk;

	private void OnEnable()
	{
		m_nextOk = false;
		int stage;
		m_nextLevel = GameController.Instance.CurrentBundle.NextLevel(GameController.Instance.CurrentLevel, false, out stage);
		if (m_nextLevel == null)
		{
			ButtonSprite.SetActive(false);
			base.collider.enabled = false;
		}
		else
		{
			m_nextOk = true;
			ButtonSprite.SetActive(true);
			base.collider.enabled = true;
		}
	}

	private void OnClick()
	{
		if (m_nextOk)
		{
			GameController.Instance.BundleModel.SetNowPlaying(m_nextLevel);
			GameObject gameObject = NGUITools.AddChild(LoadingScreenParent.gameObject, LoadingScreen);
			gameObject.transform.localPosition -= 3f * Vector3.forward;
		}
	}
}
