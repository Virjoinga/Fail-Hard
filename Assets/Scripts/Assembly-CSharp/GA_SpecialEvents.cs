using System.Collections;
using UnityEngine;

public class GA_SpecialEvents : MonoBehaviour
{
	private float _lastLevelStartTime;

	private static int _frameCountAvg;

	private static float _lastUpdateAvg;

	private int _frameCountCrit;

	private float _lastUpdateCrit;

	public void Start()
	{
		SceneChange();
		StartCoroutine(SubmitFPSRoutine());
	}

	private IEnumerator SubmitFPSRoutine()
	{
		while (Application.isPlaying && GA_SystemTracker.GA_SYSTEMTRACKER != null && GA_SystemTracker.GA_SYSTEMTRACKER.SubmitFpsCritical)
		{
			SubmitCriticalFPS();
			yield return new WaitForSeconds(GA_SystemTracker.GA_SYSTEMTRACKER.FpsCirticalSubmitInterval);
		}
	}

	public void Update()
	{
		if (GA_SystemTracker.GA_SYSTEMTRACKER.SubmitFpsAverage)
		{
			_frameCountAvg++;
		}
		if (GA_SystemTracker.GA_SYSTEMTRACKER.SubmitFpsCritical)
		{
			_frameCountCrit++;
		}
	}

	public void OnLevelWasLoaded()
	{
		SceneChange();
	}

	public void OnApplicationQuit()
	{
	}

	public static void SubmitAverageFPS()
	{
		if (!(GA_SystemTracker.GA_SYSTEMTRACKER != null) || !GA_SystemTracker.GA_SYSTEMTRACKER.SubmitFpsAverage)
		{
			return;
		}
		float num = Time.time - _lastUpdateAvg;
		if (!(num > 1f))
		{
			return;
		}
		float num2 = (float)_frameCountAvg / num;
		_lastUpdateAvg = Time.time;
		_frameCountAvg = 0;
		if (num2 > 0f)
		{
			if (GA.SettingsGA.TrackTarget != null)
			{
				GA.API.Design.NewEvent("GA:AverageFPS", (int)num2, GA.SettingsGA.TrackTarget.position);
			}
			else
			{
				GA.API.Design.NewEvent("GA:AverageFPS", (int)num2);
			}
		}
	}

	public void SubmitCriticalFPS()
	{
		if (!(GA_SystemTracker.GA_SYSTEMTRACKER != null) || !GA_SystemTracker.GA_SYSTEMTRACKER.SubmitFpsCritical)
		{
			return;
		}
		float num = Time.time - _lastUpdateCrit;
		if (!(num > 1f))
		{
			return;
		}
		float num2 = (float)_frameCountCrit / num;
		_lastUpdateCrit = Time.time;
		_frameCountCrit = 0;
		if (num2 <= (float)GA_SystemTracker.GA_SYSTEMTRACKER.FpsCriticalThreshold)
		{
			if (GA.SettingsGA.TrackTarget != null)
			{
				GA.API.Design.NewEvent("GA:CriticalFPS", _frameCountCrit, GA.SettingsGA.TrackTarget.position);
			}
			else
			{
				GA.API.Design.NewEvent("GA:CriticalFPS", _frameCountCrit);
			}
		}
	}

	private void SceneChange()
	{
		if (GA_SystemTracker.GA_SYSTEMTRACKER.IncludeSceneChange)
		{
			if (GA.SettingsGA.TrackTarget != null)
			{
				GA.API.Design.NewEvent("GA:LevelStarted", Time.time - _lastLevelStartTime, GA.SettingsGA.TrackTarget.position);
			}
			else
			{
				GA.API.Design.NewEvent("GA:LevelStarted", Time.time - _lastLevelStartTime);
			}
		}
		_lastLevelStartTime = Time.time;
	}
}
