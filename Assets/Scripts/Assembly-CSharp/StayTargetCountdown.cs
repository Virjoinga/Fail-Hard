using System.Collections.Generic;
using Game;
using Holoville.HOTween;
using UnityEngine;

public class StayTargetCountdown : MonoBehaviour
{
	public float CountdownDuration;

	public List<GameObject> Numbers;

	public StayTarget Target;

	private GigStatus m_gigStatus;

	private int m_index;

	private Cameraman m_camera;

	private UIRoot m_uiroot;

	private void Start()
	{
		m_camera = GameController.Instance.CurrentLevel.Cameraman;
		m_uiroot = NGUITools.FindInParents<UIRoot>(base.gameObject);
	}

	private void Update()
	{
		if (Target != null)
		{
			float num = 0.5f;
			if (Target.TargetType == StayTarget.StayTargetType.Both)
			{
				num = 1f;
			}
			Vector3 vector = m_camera.camera.WorldToViewportPoint(Target.transform.position + num * Vector3.up);
			Vector3 localPosition = vector;
			localPosition.z = 1f;
			float pixelSizeAdjustment = m_uiroot.GetPixelSizeAdjustment(Screen.height);
			localPosition.x = pixelSizeAdjustment * (float)Screen.width * vector.x;
			localPosition.y = pixelSizeAdjustment * (float)Screen.height * vector.y;
			base.transform.localPosition = localPosition;
		}
	}

	public void SetData(StayTarget target, int index)
	{
		m_index = index;
		Target = target;
		CountdownDuration = target.Duration * (float)(Numbers.Count - index) / (float)Numbers.Count;
		InvokeRepeating("Next", 0.01f, target.Duration / (float)Numbers.Count);
	}

	private void Next()
	{
		if (m_index > 0)
		{
			Numbers[m_index - 1].SetActive(false);
		}
		if (m_index < Numbers.Count)
		{
			AnimateNumber(Numbers[m_index]);
			m_index++;
		}
		else
		{
			CancelInvoke("Next");
		}
	}

	private void AnimateNumber(GameObject go)
	{
		go.SetActive(true);
		TweenParms p_parms = new TweenParms().Prop("localScale", 1.4f * go.transform.localScale, false).Ease(EaseType.EaseInCubic);
		HOTween.From(go.transform, 0.3f * CountdownDuration / (float)Numbers.Count, p_parms);
		go.transform.localEulerAngles = new Vector3(0f, 0f, Random.Range(-10, 10));
		TweenParms p_parms2 = new TweenParms().Prop("localRotation", new Vector3(0f, 0f, Random.Range(-5, 5)), true).Ease(EaseType.EaseInCubic);
		HOTween.To(go.transform, 0.3f * CountdownDuration / (float)Numbers.Count, p_parms2);
	}

	public void Fail()
	{
		foreach (GameObject number in Numbers)
		{
			number.SetActive(false);
		}
		CancelInvoke("Next");
		GOTools.Despawn(base.gameObject);
	}

	public void Success()
	{
		foreach (GameObject number in Numbers)
		{
			number.SetActive(false);
		}
		CancelInvoke("Next");
		GOTools.Despawn(base.gameObject);
	}
}
