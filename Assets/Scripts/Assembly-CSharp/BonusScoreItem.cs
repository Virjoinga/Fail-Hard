using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Holoville.HOTween;
using UnityEngine;

public class BonusScoreItem : MonoBehaviour
{
	[Serializable]
	public class BonusItemStateData
	{
		public float TransitionTime;

		public float Duration;

		public float Alpha;

		public Vector2 Scale;

		public Vector2 Move;
	}

	public delegate void BonusHiddenDelegate(BonusScoreItem item);

	public UILabel BonusLabel;

	public UILabel TypeLabel;

	public float ShowTimeIncrease;

	public BonusItemStateData Ready;

	public BonusItemStateData Showing;

	public BonusItemStateData Hidden;

	private string m_type;

	private int m_bonus;

	private float m_scoreUpdatedDelay;

	private BonusItemStateData m_currentStateData;

	public string Type
	{
		get
		{
			return m_type;
		}
	}

	public bool IsShowing
	{
		get
		{
			return m_currentStateData == Showing;
		}
	}

	public bool IsHidden
	{
		get
		{
			return m_currentStateData == Hidden;
		}
	}

	[method: MethodImpl(32)]
	public event BonusHiddenDelegate BonusHidden = delegate
	{
	};

	private void Awake()
	{
		TransformTools.CacheTransformData(BonusLabel.cachedTransform);
		TransformTools.CacheTransformData(TypeLabel.cachedTransform);
		base.gameObject.SetActive(false);
		m_currentStateData = Hidden;
		SetCurrentState();
	}

	public void SetData(string type, int bonus)
	{
		m_type = type;
		m_bonus = bonus;
		BonusLabel.text = m_bonus.ToString();
		TypeLabel.text = m_type;
	}

	public void SetReady()
	{
		base.gameObject.SetActive(true);
		StartCoroutine(TweenToState(Ready));
	}

	public void SetHidden()
	{
		if (base.gameObject.activeSelf)
		{
			StartCoroutine(TweenToState(Hidden));
			return;
		}
		m_currentStateData = Hidden;
		SetCurrentState();
	}

	public void SetShowing()
	{
		base.gameObject.SetActive(true);
		m_scoreUpdatedDelay = 0f;
		StartCoroutine(TweenToState(Showing));
	}

	public void UpdateScore(int toAdd)
	{
		m_bonus += toAdd;
		BonusLabel.text = m_bonus.ToString();
		m_scoreUpdatedDelay = ShowTimeIncrease;
	}

	private IEnumerator TweenToState(BonusItemStateData data)
	{
		if (m_currentStateData.Duration > 0.01f)
		{
			yield return new WaitForSeconds(m_currentStateData.Duration);
			while (m_currentStateData == Showing && m_scoreUpdatedDelay > 0.01f)
			{
				float delay = m_scoreUpdatedDelay;
				m_scoreUpdatedDelay = 0f;
				yield return new WaitForSeconds(delay);
			}
		}
		m_currentStateData = data;
		TweenParms parms = new TweenParms().Prop("alpha", data.Alpha);
		HOTween.To(BonusLabel, data.TransitionTime, parms);
		if (m_currentStateData == Hidden)
		{
			parms.OnComplete(SignalHidden);
		}
		else if (m_currentStateData == Showing)
		{
			parms.OnComplete(SetHidden);
		}
		HOTween.To(TypeLabel, data.TransitionTime, parms);
		ScaleTween(data, BonusLabel.cachedTransform);
		ScaleTween(data, TypeLabel.cachedTransform);
		AdditiveMoveTween(data, BonusLabel.cachedTransform);
		AdditiveMoveTween(data, TypeLabel.cachedTransform);
	}

	private void ScaleTween(BonusItemStateData data, Transform transform)
	{
		Vector3 localScale = TransformTools.GetCachedTransformData(transform).LocalScale;
		Vector3 vector = new Vector3(localScale.x * data.Scale.x, localScale.y * data.Scale.y, localScale.z);
		TweenParms p_parms = new TweenParms().Prop("localScale", vector);
		HOTween.To(transform, data.TransitionTime, p_parms);
	}

	private void AdditiveMoveTween(BonusItemStateData data, Transform transform)
	{
		Vector3 localPosition = TransformTools.GetCachedTransformData(transform).LocalPosition;
		Vector3 vector = new Vector3(localPosition.x + data.Move.x, localPosition.y + data.Move.y, localPosition.z);
		TweenParms p_parms = new TweenParms().Prop("localPosition", vector);
		HOTween.To(transform, data.TransitionTime, p_parms);
	}

	private void SetCurrentState()
	{
		Vector3 localScale = BonusLabel.cachedTransform.localScale;
		Vector2 scale = m_currentStateData.Scale;
		BonusLabel.cachedTransform.localScale = new Vector3(localScale.x * scale.x, localScale.y * scale.y, localScale.z);
		localScale = TypeLabel.cachedTransform.localScale;
		TypeLabel.cachedTransform.localScale = new Vector3(localScale.x * scale.x, localScale.y * scale.y, localScale.z);
		BonusLabel.alpha = m_currentStateData.Alpha;
		TypeLabel.alpha = m_currentStateData.Alpha;
	}

	private void SignalHidden()
	{
		TransformTools.ApplyCachedTransformData(BonusLabel.cachedTransform);
		TransformTools.ApplyCachedTransformData(TypeLabel.cachedTransform);
		base.gameObject.SetActive(false);
		this.BonusHidden(this);
	}
}
