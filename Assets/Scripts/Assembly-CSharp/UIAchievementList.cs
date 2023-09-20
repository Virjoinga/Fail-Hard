using Game;
using UnityEngine;

[RequireComponent(typeof(UIGrid))]
public class UIAchievementList : MonoBehaviour
{
	public GameObject iconPrefab;

	private UIGrid m_grid;

	private Level currentLevel;

	public void Awake()
	{
		m_grid = GetComponent<UIGrid>();
	}

	public void Start()
	{
		GameController instance = GameController.Instance;
		currentLevel = instance.CurrentLevel;
		currentLevel.NewTargetAchieved += ShowNewStar;
	}

	public void OnDisable()
	{
		foreach (Transform item in base.transform)
		{
			Object.Destroy(item.gameObject);
		}
	}

	public void OnDestroy()
	{
		currentLevel.NewTargetAchieved -= ShowNewStar;
	}

	private void ShowNewStar(LevelTargetInfo tgtInfo)
	{
		GOTools.InstantiateWithOriginalScale(base.gameObject, iconPrefab);
		m_grid.Reposition();
	}
}
