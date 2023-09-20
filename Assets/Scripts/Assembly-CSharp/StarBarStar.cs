using System.Collections;
using UnityEngine;

public class StarBarStar : MonoBehaviour
{
	private const string DEACTIVE_SPRITE = "starbar_star_empty";

	private const string ACTIVATE_SPRITE = "starbar_star";

	public UISprite StarSprite;

	public GameObject StarGainEffect;

	public float StarGainEffectDelay;

	public StarGainTween GainTween;

	private bool m_active;

	public bool Active
	{
		get
		{
			return m_active;
		}
	}

	private void Start()
	{
		if (!m_active)
		{
			StarSprite.spriteName = "starbar_star_empty";
		}
	}

	public void Activate(bool silent)
	{
		if (!silent)
		{
			StartCoroutine(DelayedStarGainEffect());
			GainTween.Begin();
		}
		else
		{
			StarSprite.transform.localScale *= GainTween.EndScaleMultiplier;
		}
		StarSprite.spriteName = "starbar_star";
		m_active = true;
	}

	private IEnumerator DelayedStarGainEffect()
	{
		yield return new WaitForSeconds(StarGainEffectDelay);
		GameObject go = GOTools.SpawnAsChild(StarGainEffect, base.transform);
		go.transform.position = base.transform.position;
	}
}
