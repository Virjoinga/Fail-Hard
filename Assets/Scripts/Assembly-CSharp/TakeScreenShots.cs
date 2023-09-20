using Game;
using UnityEngine;

public class TakeScreenShots : MonoBehaviour
{
	private const int TAKE_LIMIT = 1;

	private Player m_player;

	private GigStatus m_gigStatus;

	private int m_takenCount;

	private bool m_connected;

	private Collider m_uiCollider;

	private Level m_currentLevel;

	public AudioClip m_shutterClip;

	private void Start()
	{
		m_uiCollider = GetComponent<BoxCollider>();
		m_uiCollider.enabled = false;
		m_gigStatus = GameController.Instance.CurrentLevel.GigController;
		m_player = m_gigStatus.player;
		ConnectToEvents();
	}

	private void OnEnable()
	{
		if (m_player != null)
		{
			ConnectToEvents();
		}
	}

	private void OnDisable()
	{
		DisconnectEvents();
	}

	private void OnClick()
	{
		TakeShot(true);
	}

	private void ConnectToEvents()
	{
		if (!m_connected)
		{
			m_connected = true;
			m_currentLevel = GameController.Instance.CurrentLevel;
			m_gigStatus.StateChanged += m_gigStatus_StateChanged;
			m_currentLevel.NewTargetAchieved += TakeLevelTargetHitShot;
			if (m_player != null)
			{
				ConnectPlayerEvents();
			}
		}
	}

	private void m_gigStatus_StateChanged(GigStatus.GigState newState)
	{
		switch (newState)
		{
		case GigStatus.GigState.StuntActive:
			m_uiCollider.enabled = true;
			ConnectPlayerEvents();
			break;
		case GigStatus.GigState.StuntOver:
			m_uiCollider.enabled = false;
			TakeShot();
			break;
		}
	}

	private void ConnectPlayerEvents()
	{
		if (m_player != m_gigStatus.player)
		{
			m_player = m_gigStatus.player;
		}
		m_player.PlayerJumped += TakeAutomaticShot;
	}

	private void DisconnectEvents()
	{
		if (m_connected)
		{
			m_connected = false;
			m_currentLevel.NewTargetAchieved -= TakeLevelTargetHitShot;
			m_gigStatus.StateChanged -= m_gigStatus_StateChanged;
			if (m_player != null)
			{
				DisconnectPlayerEvents();
			}
		}
	}

	private void DisconnectPlayerEvents()
	{
		m_player.PlayerCrashed -= TakeAutomaticShot;
		m_player.PlayerJumped -= TakeAutomaticShot;
		m_player.PlayerJumpedOff -= TakeAutomaticShot;
	}

	private void TakeLevelTargetHitShot(LevelTargetInfo info)
	{
		TakeShot();
	}

	private void TakeAutomaticShot()
	{
		TakeShot();
	}

	private void TakeShot(bool forceMode = false)
	{
		if (m_takenCount < 1 || forceMode)
		{
			if (forceMode)
			{
				AudioManager.Instance.NGPlay(m_shutterClip);
			}
			ScreenShotRenderTexture screenShotRenderTexture = FindScreenShotter();
			if ((bool)screenShotRenderTexture)
			{
				screenShotRenderTexture.TakeScreenShot = true;
				m_takenCount++;
			}
		}
	}

	private void Reset()
	{
		ScreenShotRenderTexture screenShotRenderTexture = FindScreenShotter();
		if ((bool)screenShotRenderTexture)
		{
			screenShotRenderTexture.Clear();
			screenShotRenderTexture.TakeScreenShot = false;
		}
		m_takenCount = 0;
	}

	private ScreenShotRenderTexture FindScreenShotter()
	{
		Camera main = Camera.main;
		return main.GetComponent<ScreenShotRenderTexture>();
	}
}
