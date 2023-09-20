using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Game;
using UnityEngine;

public class LevelIntro : MonoBehaviour
{
	public enum IntroState
	{
		Stopped = 0,
		Auto = 1,
		Manual = 2
	}

	public delegate void OnIntroStateChanged(bool playing);

	public Cameraman cam;

	public float timePerTarget;

	private int currentIndex;

	public List<Transform> levelTargets;

	private float cachedMinZoom;

	private float cachedSlerp;

	private Vector3 cachedOffset;

	public float zoomLevel;

	private IntroState m_state;

	public bool IsPlaying
	{
		get
		{
			return m_state != IntroState.Stopped;
		}
	}

	[method: MethodImpl(32)]
	public event OnIntroStateChanged IntroStateChanged;

	private void Start()
	{
		levelTargets = new List<Transform>();
		m_state = IntroState.Stopped;
	}

	public void StartIntro()
	{
		if (m_state == IntroState.Auto)
		{
			return;
		}
		cam = GameController.Instance.CurrentLevel.Cameraman;
		cachedMinZoom = ((CameraFlying)cam).minZoomLevel;
		cachedSlerp = ((CameraFlying)cam).slerpToTargetTime;
		cachedOffset = ((CameraFlying)cam).lookAtTargetOffset;
		((CameraFlying)cam).minZoomLevel = zoomLevel;
		((CameraFlying)cam).lookAtTargetOffset = Vector3.zero;
		currentIndex = -1;
		if (levelTargets.Count > 0)
		{
			cam.StartTracking(null);
			levelTargets.Clear();
		}
		foreach (TargetZone levelTarget in GameController.Instance.CurrentLevel.LevelTargets)
		{
			if (levelTarget.IsActive())
			{
				levelTargets.Add(levelTarget.transform);
			}
		}
		levelTargets.Add(GetComponent<GigStatus>().GetSpawnPoint());
		if (this.IntroStateChanged != null && m_state == IntroState.Stopped)
		{
			this.IntroStateChanged(true);
		}
		m_state = IntroState.Auto;
		ShowNextTarget();
	}

	public void EndIntro()
	{
		if (m_state != 0)
		{
			((CameraFlying)cam).minZoomLevel = cachedMinZoom;
			((CameraFlying)cam).slerpToTargetTime = cachedSlerp;
			((CameraFlying)cam).lookAtTargetOffset = cachedOffset;
			CancelInvoke();
			m_state = IntroState.Stopped;
			if (this.IntroStateChanged != null)
			{
				this.IntroStateChanged(false);
			}
		}
	}

	public void GetManualControl()
	{
		if (m_state == IntroState.Stopped)
		{
			StartIntro();
		}
		CancelInvoke();
		if (this.IntroStateChanged != null && m_state == IntroState.Stopped)
		{
			this.IntroStateChanged(true);
		}
		m_state = IntroState.Manual;
	}

	public void ResumeAutoControl()
	{
		if (this.IntroStateChanged != null && m_state == IntroState.Stopped)
		{
			this.IntroStateChanged(true);
		}
		m_state = IntroState.Auto;
		currentIndex--;
		ShowNextTarget();
	}

	private void ShowNextTarget()
	{
		currentIndex++;
		if (currentIndex < levelTargets.Count)
		{
			SetCameraTargets(currentIndex);
			Invoke("ShowNextTarget", timePerTarget);
		}
		else
		{
			EndIntro();
		}
	}

	private void SetCameraTargets(int index)
	{
		cam.StartTracking(levelTargets[index]);
	}
}
