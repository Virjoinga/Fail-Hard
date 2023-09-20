using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Valinta
{
	public class VPlayerStatus : MonoBehaviour
	{
		[SerializeField]
		private Scrollbar m_statusScroller;

		[SerializeField]
		private Text m_statusText;

		private float lerpTime = 3f;

		private float currentLerpTime;

		private float startValue;

		private float endValue = 1f;

		private bool scrolling;

		private bool isReversed;

		private void Update()
		{
			if (scrolling)
			{
				currentLerpTime += Time.deltaTime;
				if (currentLerpTime > lerpTime)
				{
					currentLerpTime = lerpTime;
				}
				float t = currentLerpTime / lerpTime;
				m_statusScroller.value = Mathf.Lerp(startValue, endValue, t);
				if (Mathf.Abs(m_statusScroller.value - endValue) < float.Epsilon)
				{
					scrolling = false;
					StartCoroutine(AutoScroll());
				}
			}
		}

		private void OnEnable()
		{
			ResetScroller();
			StartCoroutine(AutoScroll());
		}

		private void OnDisable()
		{
			StopCoroutine(AutoScroll());
			ResetScroller();
		}

		public void UpdateText(string s)
		{
			m_statusText.text = s;
			ResetScroller();
			if (base.isActiveAndEnabled)
			{
				StartCoroutine(AutoScroll());
			}
		}

		private IEnumerator AutoScroll()
		{
			yield return new WaitForSeconds(0.3f);
			currentLerpTime = 0f;
			if (isReversed)
			{
				startValue = 1f;
				endValue = 0f;
			}
			else
			{
				startValue = 0f;
				endValue = 1f;
			}
			scrolling = true;
			isReversed = !isReversed;
		}

		private void ResetScroller()
		{
			StopCoroutine(AutoScroll());
			isReversed = false;
			scrolling = false;
			startValue = 0f;
			endValue = 1f;
			currentLerpTime = 0f;
			m_statusScroller.value = 0f;
		}
	}
}
