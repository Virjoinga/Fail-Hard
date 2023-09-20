using Game;
using UnityEngine;

public class QuickPlay : MonoBehaviour
{
	public GameObject LoadingScreenPrefab;

	private void Start()
	{
		GameController.Instance.CurrentBundle = GameController.Instance.BundleModel.Model["Tutorial"];
		string levelId = GameController.Instance.CurrentBundle.Stages[0].Levels[0].LevelId;
		Level level = GameController.Instance.LevelDatabase.LevelForName(levelId);
		while (level.IsCompleted)
		{
			int stage;
			Level level2 = GameController.Instance.CurrentBundle.NextLevel(level, false, out stage);
			if (level2 == null)
			{
				break;
			}
			level = level2;
		}
		GameController.Instance.BundleModel.SetNowPlaying(level);
	}

	private void OnClick()
	{
		GameObject gameObject = NGUITools.AddChild(base.transform.parent.parent.parent.gameObject, LoadingScreenPrefab);
		gameObject.transform.localPosition -= Vector3.forward;
	}
}
