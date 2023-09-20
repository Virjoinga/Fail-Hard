using UnityEngine;

public class LevelIntroCameraControls : MonoBehaviour
{
	private const float CLICK_THRESHOLD = 0.2f;

	public float PanningFieldOfView = 60f;

	public float ScreenSwipeUnit = 10f;

	public float ManualReleaseTimeout = 1.5f;

	private LevelIntro m_levelIntro;

	private GigStatus m_gigStatus;

	private Transform m_panTarget;

	private CameraFlying m_flyingCamera;

	private Transform m_backupCameraTarget;

	private float m_pressDownTime;

	private float m_releaseTime;

	private bool m_readyForRelease;

	private void Start()
	{
		GameObject gameObject = GameObject.Find("Level:root");
		m_gigStatus = gameObject.GetComponentInChildren<GigStatus>();
		m_levelIntro = m_gigStatus.GetComponent<LevelIntro>();
		if (m_levelIntro != null)
		{
			m_levelIntro.IntroStateChanged += OnIntroStateChanged;
		}
	}

	private void OnClick()
	{
		if (m_levelIntro.IsPlaying)
		{
			m_levelIntro.EndIntro();
		}
		else
		{
			m_levelIntro.StartIntro();
		}
	}

	public void OnIntroStateChanged(bool playing)
	{
		if (!playing && m_panTarget != null)
		{
			Object.Destroy(m_panTarget.gameObject);
			m_panTarget = null;
			m_readyForRelease = false;
		}
	}

	private void OnPress(bool isDown)
	{
		if (isDown)
		{
			m_readyForRelease = false;
			m_pressDownTime = Time.time;
			if (!(m_panTarget == null))
			{
				return;
			}
			m_flyingCamera = GetCamera();
			if ((bool)m_flyingCamera)
			{
				m_levelIntro.GetManualControl();
				m_backupCameraTarget = m_flyingCamera.LookAtTarget();
				if (m_backupCameraTarget == null)
				{
					m_backupCameraTarget = m_gigStatus.GetSpawnPoint();
				}
				m_panTarget = new GameObject().transform;
				m_panTarget.gameObject.name = "temp-camera-target";
				m_panTarget.position = m_flyingCamera.CurrentLookAtPosition();
				m_flyingCamera.StartTracking(m_panTarget);
			}
		}
		else if (Time.time - m_pressDownTime < 0.2f)
		{
			if (m_levelIntro.IsPlaying)
			{
				m_levelIntro.EndIntro();
			}
			else
			{
				m_levelIntro.StartIntro();
			}
		}
		else
		{
			m_releaseTime = Time.time;
			m_readyForRelease = true;
		}
	}

	private void Update()
	{
		if (m_readyForRelease && Time.time - m_releaseTime > ManualReleaseTimeout)
		{
			m_readyForRelease = false;
			if ((bool)m_flyingCamera)
			{
				m_flyingCamera.StartTracking(m_backupCameraTarget);
			}
			m_levelIntro.ResumeAutoControl();
			if (m_panTarget != null)
			{
				Object.Destroy(m_panTarget.gameObject);
			}
			m_panTarget = null;
		}
	}

	private void OnDrag(Vector2 delta)
	{
		Vector2 vector = GestureUtil.ScreenPercentage(delta);
		Vector3 translation = new Vector3(vector.x * ScreenSwipeUnit, vector.y * ScreenSwipeUnit, 0f);
		bool left;
		bool right;
		bool up;
		bool down;
		if (GetCamera().LimitsCheck(out left, out right, out up, out down))
		{
			if (left && translation.x < 0f)
			{
				translation.x = 0f;
			}
			if (right && translation.x > 0f)
			{
				translation.x = 0f;
			}
			if (down && translation.y < 0f)
			{
				translation.y = 0f;
			}
			if (up && translation.y > 0f)
			{
				translation.y = 0f;
			}
		}
		m_panTarget.Translate(translation, Space.Self);
	}

	private CameraFlying GetCamera()
	{
		Camera main = Camera.main;
		return main.GetComponent<CameraFlying>();
	}
}
