using Game;
using Game.Progress;
using UnityEngine;

public class LockedStage : MonoBehaviour
{
	public UILabel Label;

	public UITexture LevelIcon;

	public void SetData(BundleStage stage, Bundle bundle)
	{
		ILevel level = GameController.Instance.LevelDatabase.LevelForName(stage.Levels[0].LevelId);
		if (level != null)
		{
			string path = "Levels/" + level.Parameters.Name + "_snapshot";
			Texture2D mainTexture = (Texture2D)Resources.Load(path);
			LevelIcon.mainTexture = mainTexture;
		}
		Label.text = Mathf.Max(bundle.StageCriteria(stage.Index - 1) - bundle.AchievedTargets, 0).ToString();
	}
}
