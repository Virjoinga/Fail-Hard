using Holoville.HOTween;
using UnityEngine;

public class ConnectingLabel : MonoBehaviour
{
	public Transform Rotate;

	private Tweener m_tween;

	private void Start()
	{
		RotateTween();
	}

	private void RotateTween()
	{
		Rotate.transform.localRotation = Quaternion.identity;
		TweenParms p_parms = new TweenParms().Prop("localRotation", new Vector3(0f, 0f, 360f), true).Loops(-1).Ease(EaseType.Linear);
		m_tween = HOTween.To(Rotate, 1f, p_parms);
	}

	private void OnDisable()
	{
		if (m_tween != null)
		{
			m_tween.Kill();
		}
	}
}
