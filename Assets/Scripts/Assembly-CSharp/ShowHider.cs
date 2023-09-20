using Holoville.HOTween;
using UnityEngine;

public class ShowHider : MonoBehaviour
{
	public GameObject Target;

	public Vector3 ShowDiff;

	public Vector3 HideDiff;

	public float SlideInTime;

	public float ShowStateScale;

	public float HideStateScale;

	public bool StartShown;

	private bool m_shown;

	private Vector3 m_cachePos;

	private Vector3 m_cacheScale;

	private void Start()
	{
		m_cachePos = Target.transform.localPosition;
		m_cacheScale = Target.transform.localScale;
		if (StartShown)
		{
			Show();
		}
		else
		{
			Hide();
		}
	}

	public void Show()
	{
		if (!m_shown)
		{
			m_shown = true;
			Target.SetActive(m_shown);
			SetColliders(Target, m_shown);
			SlideButton(Target, m_cachePos, ShowDiff);
			ScaleButton(Target, m_cacheScale, ShowStateScale);
		}
	}

	public void Hide()
	{
		if (m_shown)
		{
			m_shown = false;
			SetColliders(Target, m_shown);
			SlideButton(Target, m_cachePos, HideDiff);
			ScaleButton(Target, m_cacheScale, HideStateScale);
		}
	}

	private void SetColliders(GameObject button, bool collisions)
	{
		Collider[] componentsInChildren = button.GetComponentsInChildren<Collider>(true);
		foreach (Collider collider in componentsInChildren)
		{
			collider.enabled = collisions;
		}
	}

	private void SlideButton(GameObject button, Vector3 origo, Vector3 slideAmount)
	{
		TweenParms p_parms = new TweenParms().Prop("localPosition", origo + slideAmount, false).Ease(EaseType.EaseOutBounce);
		HOTween.To(button.transform, SlideInTime, p_parms);
	}

	private void ScaleButton(GameObject button, Vector3 original, float diff)
	{
		TweenParms p_parms = new TweenParms().Prop("localScale", original * diff, false).Ease(EaseType.EaseOutBounce).OnComplete(EnableDisable);
		HOTween.To(button.transform, SlideInTime, p_parms);
	}

	private void EnableDisable()
	{
		Target.SetActive(m_shown);
	}
}
