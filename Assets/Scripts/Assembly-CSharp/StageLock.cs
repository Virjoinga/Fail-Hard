using System.Collections;
using Holoville.HOTween;
using UnityEngine;

public class StageLock : MonoBehaviour
{
	public UIWidget Background;

	public UIWidget Lock;

	public GameObject UnlockEffect;

	public UIWidget Star;

	public UIWidget StarCounter;

	public UIWidget StarCounterBackground;

	public void Hide()
	{
		StartCoroutine(StartDisappearAnimations());
	}

	private IEnumerator StartDisappearAnimations()
	{
		yield return new WaitForSeconds(1f);
		StarScaleTween();
		yield return new WaitForSeconds(0.1f);
		PlayUnlockEffect();
		yield return new WaitForSeconds(0.5f);
		StarDisappear();
		yield return new WaitForSeconds(0.5f);
		BackGroundDisappear();
		LockDisappear();
	}

	private void PlayUnlockEffect()
	{
		GameObject gameObject = GOTools.Instantiate(UnlockEffect, base.gameObject);
		gameObject.transform.localPosition = Star.transform.localPosition;
		gameObject.transform.localScale *= 3f;
		ParticleSystem componentInChildren = gameObject.GetComponentInChildren<ParticleSystem>();
		componentInChildren.Play();
	}

	private void ScaleTween()
	{
		Vector3 vector = base.transform.localScale * 1.07f;
		TweenParms p_parms = new TweenParms().Prop("localScale", vector).Ease(EaseType.EaseInCubic).Loops(2, LoopType.Yoyo);
		HOTween.To(base.transform, 0.1f, p_parms);
	}

	private void BackGroundDisappear()
	{
		AlphaHideTweener(Background, 0.15f);
	}

	private void StarScaleTween()
	{
		Vector3 vector = Star.transform.localScale * 3f;
		TweenParms p_parms = new TweenParms().Prop("localScale", vector).Ease(EaseType.EaseInCirc).Loops(2, LoopType.Yoyo);
		HOTween.To(Star.cachedTransform, 0.15f, p_parms);
		vector = StarCounter.cachedTransform.localScale * 1.1f;
		p_parms = new TweenParms().Prop("localScale", vector).Ease(EaseType.EaseInCirc).Loops(2, LoopType.Yoyo);
		HOTween.To(StarCounter.cachedTransform, 0.15f, p_parms);
		vector = StarCounterBackground.cachedTransform.localScale * 1.1f;
		p_parms = new TweenParms().Prop("localScale", vector).Ease(EaseType.EaseInCirc).Loops(2, LoopType.Yoyo);
		HOTween.To(StarCounterBackground.cachedTransform, 0.15f, p_parms);
	}

	private void StarDisappear()
	{
		TweenParms p_parms = new TweenParms().Prop("localScale", Vector3.zero).Ease(EaseType.EaseInQuad);
		HOTween.To(Star.cachedTransform, 0.3f, p_parms);
		AlphaHideTweener(Star, 0.05f);
		AlphaHideTweener(StarCounter, 0.03f);
		AlphaHideTweener(StarCounterBackground);
	}

	private void LockDisappear()
	{
		Vector3 vector = Lock.cachedTransform.localScale * 0.5f;
		TweenParms tweenParms = new TweenParms().Prop("localScale", vector).Ease(EaseType.EaseInCirc).Delay(0.1f);
		tweenParms.OnComplete(Deactivate);
		HOTween.To(Lock.cachedTransform, 0.1f, tweenParms);
		AlphaHideTweener(Lock, 0.2f);
	}

	private Tweener AlphaHideTweener(UIWidget target, float duration = 0.3f, float delay = 0f)
	{
		TweenParms p_parms = new TweenParms().Prop("alpha", 0f).Ease(EaseType.EaseInCirc).Delay(delay);
		return HOTween.To(target, duration, p_parms);
	}

	private void Deactivate()
	{
		base.gameObject.SetActive(false);
	}
}
