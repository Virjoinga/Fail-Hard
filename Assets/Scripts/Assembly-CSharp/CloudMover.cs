using System.Collections.Generic;
using Holoville.HOTween;
using UnityEngine;

public class CloudMover : MonoBehaviour
{
	private List<GameObject> m_clouds;

	private float m_startX;

	private float m_endX;

	public void Start()
	{
		m_clouds = GOTools.FindChildren(base.gameObject);
		UIRoot uIRoot = NGUITools.FindInParents<UIRoot>(base.gameObject);
		m_startX = (float)(-Screen.width) / 2f * uIRoot.pixelSizeAdjustment;
		m_endX = m_startX + (float)Screen.width * uIRoot.pixelSizeAdjustment;
		m_startX -= 400f;
		m_endX += 400f;
		foreach (GameObject cloud in m_clouds)
		{
			float num = Random.Range(35f, 40f);
			float num2 = (cloud.transform.localPosition.x + (0f - m_startX)) / (m_endX - m_startX);
			num = (1f - num2) * num;
			Vector3 localPosition = cloud.transform.localPosition;
			Vector3 vector = new Vector3(m_endX, localPosition.y, localPosition.z);
			TweenParms p_parms = new TweenParms().Prop("localPosition", vector).OnComplete(RelaunchCloud, cloud).Ease(EaseType.Linear);
			HOTween.To(cloud.transform, num, p_parms);
		}
	}

	private void RelaunchCloud(TweenEvent evt)
	{
		GameObject gameObject = evt.parms[0] as GameObject;
		Vector3 localPosition = gameObject.transform.localPosition;
		gameObject.transform.localPosition = new Vector3(m_startX, localPosition.y, localPosition.z);
		Vector3 vector = new Vector3(m_endX, localPosition.y, localPosition.z);
		TweenParms p_parms = new TweenParms().Prop("localPosition", vector).Loops(-1).Ease(EaseType.Linear);
		HOTween.To(gameObject.transform, Random.Range(30f, 35f), p_parms);
	}
}
