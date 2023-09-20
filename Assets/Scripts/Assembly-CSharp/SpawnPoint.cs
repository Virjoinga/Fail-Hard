using Game;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
	private void Start()
	{
		GameController instance = GameController.Instance;
		Level currentLevel = instance.CurrentLevel;
		currentLevel.SetSpawnPoint(base.gameObject.transform);
	}
}
