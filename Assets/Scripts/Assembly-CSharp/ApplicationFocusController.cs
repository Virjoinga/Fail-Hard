using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Game;
using UnityEngine;

public class ApplicationFocusController : MonoBehaviour
{
	public delegate void PauseState(bool paused);

	private static bool initialized;

	private static ApplicationFocusController instance;

	private static List<Action> executeLater;

	[method: MethodImpl(32)]
	public event PauseState PauseEvent = delegate
	{
	};

	private void Awake()
	{
		if (initialized)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		instance = this;
		initialized = true;
		if (executeLater == null)
		{
			executeLater = new List<Action>();
		}
	}

	private void Update()
	{
		lock (executeLater)
		{
			foreach (Action item in executeLater)
			{
				item();
			}
			executeLater.Clear();
		}
	}

	private void OnApplicationPause(bool paused)
	{
		if (paused)
		{
			Storage.Instance.Shutdown();
		}
		else
		{
			Storage.Instance.Restore();
		}
		this.PauseEvent(paused);
	}

	public static void RegisterPauseEvent(PauseState callback)
	{
		ApplicationFocusController applicationFocusController = instance;
		applicationFocusController.PauseEvent = (PauseState)Delegate.Combine(applicationFocusController.PauseEvent, callback);
	}

	public static void UnRegisterPauseEvent(PauseState callback)
	{
		ApplicationFocusController applicationFocusController = instance;
		applicationFocusController.PauseEvent = (PauseState)Delegate.Remove(applicationFocusController.PauseEvent, callback);
	}

	public static Coroutine BeginCoroutine(IEnumerator ir)
	{
		if (instance != null)
		{
			return instance.ProcessWork(ir);
		}
		return null;
	}

	public Coroutine ProcessWork(IEnumerator ir)
	{
		return StartCoroutine(ir);
	}

	public static void ExecuteLater(Action ia)
	{
		if (executeLater == null)
		{
			executeLater = new List<Action>();
		}
		lock (executeLater)
		{
			executeLater.Add(ia);
		}
	}
}
