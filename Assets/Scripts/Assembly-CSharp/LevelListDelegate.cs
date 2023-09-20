using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Game;
using Game.Progress;
using UnityEngine;

public class LevelListDelegate : MonoBehaviour
{
	public delegate void OnItemSelected(LevelListDelegate listItem);

	public ILevel m_level;

	public UITexture levelIcon;

	public Shader lockedShader;

	public GameObject lockedMode;

	public GameObject unlockedMode;

	public GameObject puff;

	public List<UISprite> Stars;

	private bool m_shownState;

	[method: MethodImpl(32)]
	public event OnItemSelected ItemSelected;

	private void Start()
	{
		SetState(m_shownState);
	}

	public void SetData(ILevel level, bool animateUnlock)
	{
		m_level = level;
		string path = "Levels/" + m_level.Parameters.Name + "_snapshot";
		Texture2D texture2D = (Texture2D)Resources.Load(path);
		if (texture2D == null)
		{
			path = "Levels/" + m_level.Parameters.Name.Substring(2) + "_snapshot";
			texture2D = (Texture2D)Resources.Load(path);
		}
		levelIcon.mainTexture = texture2D;
		levelIcon.transform.localPosition = 1f * Vector3.forward;
		for (int i = 0; i < ((Level)m_level).AchievedLevelTargets.Count; i++)
		{
			Stars[i].spriteName = "starbar_star";
		}
		if (!m_level.IsLocked && !((Level)m_level).UnlockShown && animateUnlock)
		{
			m_shownState = true;
		}
		else
		{
			m_shownState = m_level.IsLocked;
		}
	}

	public void OnClick()
	{
		if (this.ItemSelected != null)
		{
			this.ItemSelected(this);
		}
	}

	public void AnimateUnlock()
	{
		if (!m_level.IsLocked && !((Level)m_level).UnlockShown)
		{
			ScaleUp();
		}
	}

	private void OnDestroy()
	{
		if (!m_level.IsLocked && !((Level)m_level).UnlockShown)
		{
			((Level)m_level).UnlockShown = true;
		}
	}

	private void SetState(bool locked)
	{
		m_shownState = locked;
		lockedMode.SetActive(locked);
		foreach (Transform item in unlockedMode.transform)
		{
			item.gameObject.SetActive(!locked);
		}
		if (!locked)
		{
			levelIcon.gameObject.SetActive(true);
			((Level)m_level).UnlockShown = true;
		}
		else
		{
			levelIcon.shader = lockedShader;
		}
	}

	private void ScaleUp()
	{
		TweenScale tweenScale = TweenScale.Begin(base.gameObject, 0.1f, 1.1f * Vector3.one);
		GameObject gameObject = NGUITools.AddChild(base.gameObject, puff);
		gameObject.transform.localPosition -= Vector3.forward;
		gameObject.transform.localScale = 0.5f * Vector3.one;
		tweenScale.eventReceiver = base.gameObject;
		tweenScale.callWhenFinished = "ScaleDown";
		AudioManager.Instance.NGPlay((AudioClip)Resources.Load("Audio/fhg_radio_new_level"));
	}

	private void ScaleDown()
	{
		TweenScale.Begin(base.gameObject, 0.1f, Vector3.one);
		SetState(false);
	}
}
