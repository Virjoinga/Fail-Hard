using System.Collections.Generic;
using Game;
using Game.Util;
using UnityEngine;

public class LevelRoot : MonoBehaviour
{
	private Cameraman cameraMan;

	private Transform playerSpawnPoint;

	private void Start()
	{
		GameController instance = GameController.Instance;
		Level currentLevel = instance.CurrentLevel;
		if (!currentLevel.SpawnPoint)
		{
			Logger.Error("No spawn point for level!?!");
		}
		else
		{
			playerSpawnPoint = currentLevel.SpawnPoint;
		}
		if (currentLevel.Cameraman == null)
		{
			Logger.Log("Camera not found for level!");
		}
		else
		{
			cameraMan = currentLevel.Cameraman;
		}
	}

	public Cameraman GetLevelCamera()
	{
		return cameraMan;
	}

	public Transform GetPlayerSpawnPoint()
	{
		return playerSpawnPoint;
	}

	public IEnumerable<TargetZone> LevelTargets()
	{
		return GameController.Instance.CurrentLevel.LevelTargets;
	}
}
