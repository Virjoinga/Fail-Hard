using Holoville.HOTween;
using UnityEngine;

public class GigUIHelpOverlay : MonoBehaviour
{
	public GameObject ThrottleHelp;

	public UILabel ThrottleTipLabel;

	public GameObject BrakeHelp;

	public UILabel BrakeTipLabel;

	public GameObject JumpRightHelp;

	public UILabel JumpRightTipLabel;

	public bool ShowThrottle;

	public bool ShowBrake;

	public bool ShowJumpRight;

	private Tweener m_fader;

	public void SetTooltip(ButtonControlScheme.VehicleControl button, string text)
	{
		bool flag = true;
		if (text == string.Empty)
		{
			flag = false;
		}
		switch (button)
		{
		case ButtonControlScheme.VehicleControl.Right:
			ThrottleTipLabel.text = text;
			ShowThrottle = flag;
			break;
		case ButtonControlScheme.VehicleControl.Left:
			BrakeTipLabel.text = text;
			ShowBrake = flag;
			break;
		case ButtonControlScheme.VehicleControl.Jump:
			JumpRightTipLabel.text = text;
			ShowJumpRight = flag;
			break;
		}
	}

	private void Start()
	{
	}

	public void Show(float delay = 0f)
	{
		GetComponent<UIPanel>().alpha = 0f;
		if (ShowThrottle)
		{
			ThrottleHelp.SetActive(true);
		}
		if (ShowBrake)
		{
			BrakeHelp.SetActive(true);
		}
		if (ShowJumpRight)
		{
			JumpRightHelp.SetActive(true);
		}
		if (delay > 0f)
		{
			Invoke("DelayedShow", delay);
		}
		else
		{
			DelayedShow();
		}
	}

	private void DelayedShow()
	{
		TweenParms p_parms = new TweenParms().Prop("alpha", 1, false);
		m_fader = HOTween.To(GetComponent<UIPanel>(), 1f, p_parms);
	}

	public void Hide()
	{
		CancelInvoke("DelayedShow");
		if (m_fader != null)
		{
			m_fader.Kill();
		}
		TweenParms p_parms = new TweenParms().Prop("alpha", 0, false);
		HOTween.To(GetComponent<UIPanel>(), 0.5f, p_parms);
	}
}
