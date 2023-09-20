using System;
using Game;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class GarageCameraController : MonoBehaviour
{
	[Serializable]
	public class CameraSetup
	{
		public Transform Position;
	}

	public CameraSetup startPosition;

	public CameraSetup upgradingPosition;

	public CameraSetup levelPosition;

	private Transform currentTarget;

	private Camera cam;

	private bool m_gameInitialized;

	private void Start()
	{
		GameController.Instance.LevelDatabase.LevelDatabasePopulated += LevelsReady;
		if (GameController.Instance.LevelDatabase.Initialized)
		{
			m_gameInitialized = true;
		}
		currentTarget = startPosition.Position;
		cam = GetComponent<Camera>();
	}

	private void Update()
	{
		if (m_gameInitialized)
		{
			cam.transform.position = Vector3.Lerp(cam.transform.position, currentTarget.position, 0.1f);
			cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, currentTarget.rotation, 0.1f);
		}
	}

	public void ToStartPosition()
	{
		currentTarget = startPosition.Position;
	}

	public void ToUpgradingPosition()
	{
		currentTarget = upgradingPosition.Position;
	}

	public void ToLevelPosition()
	{
		currentTarget = levelPosition.Position;
	}

	private void OnDestroy()
	{
		GameController.Instance.LevelDatabase.LevelDatabasePopulated -= LevelsReady;
	}

	private void LevelsReady(LevelDatabase db)
	{
		m_gameInitialized = true;
	}
}
