using Game;
using Holoville.HOTween;
using UnityEngine;

public class BonusIndicator : MonoBehaviour
{
	public UILabel Label;

	public float ShowDuration;

	public float AppearSpeed;

	public EaseType AppearEase;

	public float DisappearSpeed;

	public EaseType DisappearEase;

	private Camera m_camera;

	private UIRoot m_uiroot;

	private Vector3 m_worldPos;

	private bool m_position;

	public void SetData(Vector3 startPosition, Vector3 targetPosition)
	{
		if (m_camera == null)
		{
			m_camera = GameController.Instance.CurrentLevel.Cameraman.camera;
		}
		if (m_uiroot == null)
		{
			m_uiroot = NGUITools.FindInParents<UIRoot>(base.gameObject);
		}
		Vector3 vector = m_camera.WorldToViewportPoint(startPosition);
		float pixelSizeAdjustment = m_uiroot.GetPixelSizeAdjustment(Screen.height);
		float x = pixelSizeAdjustment * ((float)Screen.width * vector.x - (float)Screen.width / 2f);
		float y = pixelSizeAdjustment * (0f - (float)Screen.height * vector.y);
		base.transform.localPosition = new Vector3(x, y, 1f);
		AnimateAppear();
		TweenParms tweenParms = new TweenParms().Prop("localPosition", targetPosition);
		tweenParms.OnComplete(AnimateFade);
		HOTween.To(base.transform, AppearSpeed, tweenParms);
	}

	private void AnimateAppear()
	{
		Label.alpha = 0f;
		TweenParms p_parms = new TweenParms().Prop("alpha", 0.75f).Ease(AppearEase);
		HOTween.To(Label, AppearSpeed, p_parms);
		base.transform.localScale = Vector3.one / 2f;
		p_parms = new TweenParms().Prop("localScale", Vector3.one * 0.8f).Ease(AppearEase, 8f);
		HOTween.To(base.transform, AppearSpeed, p_parms);
	}

	private void AnimateFade()
	{
		TweenParms p_parms = new TweenParms().Prop("alpha", 0f).Ease(EaseType.EaseOutQuart);
		HOTween.To(Label, DisappearSpeed / 1.3f, p_parms);
		p_parms = new TweenParms().Prop("localScale", Vector3.one * 0.5f, false).Ease(EaseType.EaseOutQuart);
		p_parms.OnComplete(Despawn);
		HOTween.To(base.transform, DisappearSpeed, p_parms);
	}

	private void Despawn()
	{
		if (base.gameObject.activeSelf)
		{
			GOTools.Despawn(base.gameObject);
		}
		else
		{
			Debug.LogWarning("Tried to despawn inactive object: " + base.gameObject.name);
		}
	}
}
