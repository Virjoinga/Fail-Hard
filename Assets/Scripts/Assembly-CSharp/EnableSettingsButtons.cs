using Holoville.HOTween;
using Holoville.HOTween.Core;
using UnityEngine;

public class EnableSettingsButtons : MonoBehaviour
{
	public Transform First;

	public Transform Second;

	public float OpenDistance;

	public float OpeningDuration;

	private bool m_opened;

	private void OnClick()
	{
		if (m_opened)
		{
			CloseFan();
		}
		else
		{
			OpenFan();
		}
		m_opened = !m_opened;
	}

	private void OpenFan()
	{
		TweenParms p_parms = new TweenParms().Prop("localPosition", new Vector3(0f, OpenDistance, 0f), true).OnStart(delegate
		{
			First.gameObject.SetActive(true);
		});
		HOTween.To(First, OpeningDuration, p_parms);
		p_parms = new TweenParms().Prop("localPosition", new Vector3(OpenDistance, 0f, 0f), true).OnStart(delegate
		{
			Second.gameObject.SetActive(true);
		});
		HOTween.To(Second, OpeningDuration, p_parms);
	}

	private void CloseFan()
	{
		TweenParms p_parms = new TweenParms().Prop("localPosition", new Vector3(0f, 0f - OpenDistance, 0f), true).OnComplete((TweenDelegate.TweenCallback)delegate
		{
			First.gameObject.SetActive(false);
		});
		HOTween.To(First, OpeningDuration, p_parms);
		p_parms = new TweenParms().Prop("localPosition", new Vector3(0f - OpenDistance, 0f, 0f), true).OnComplete((TweenDelegate.TweenCallback)delegate
		{
			Second.gameObject.SetActive(false);
		});
		HOTween.To(Second, OpeningDuration, p_parms);
	}
}
