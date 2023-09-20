using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class LevelTargetDisplay : MonoBehaviour
{
	private List<GameObject> targets;

	public GameObject targetPrefab;

	public GameObject SecondaryTargets;

	private int animationCounter;

	private int m_animatedIndex;

	public Level level;

	private bool m_skipped;

	public void SetLevel(Level data, int targetsAtGigStart, int targetSize)
	{
		level = data;
		if (targets == null)
		{
			targets = new List<GameObject>();
		}
		foreach (GameObject target in targets)
		{
			UnityEngine.Object.Destroy(target);
		}
		targets.Clear();
		int num = level.TargetCount();
		float num2 = (float)targetSize / 55f;
		while (targets.Count < num)
		{
			GameObject gameObject = NGUITools.AddChild(base.gameObject, targetPrefab);
			targets.Add(gameObject);
			gameObject.transform.localScale *= num2;
		}
		GetComponent<UIGrid>().cellWidth = 55f * num2;
		GetComponent<UIGrid>().cellHeight = GetComponent<UIGrid>().cellWidth;
		base.transform.localPosition -= Vector3.right * num / 2f * GetComponent<UIGrid>().cellWidth - Vector3.right * 0.5f * GetComponent<UIGrid>().cellWidth;
		GetComponent<UIGrid>().Reposition();
		while (targets.Count < level.TargetCount())
		{
			GameObject gameObject2 = NGUITools.AddChild(SecondaryTargets, targetPrefab);
			targets.Add(gameObject2);
			gameObject2.transform.localScale *= num2;
		}
		SecondaryTargets.GetComponent<UIGrid>().cellWidth = 55f * num2;
		SecondaryTargets.GetComponent<UIGrid>().cellHeight = SecondaryTargets.GetComponent<UIGrid>().cellWidth;
		float num3 = GetComponent<UIGrid>().cellWidth - SecondaryTargets.GetComponent<UIGrid>().cellWidth;
		SecondaryTargets.transform.localPosition += Vector3.right * ((float)num * GetComponent<UIGrid>().cellWidth - 0.5f * num3);
		SecondaryTargets.transform.localPosition -= Vector3.up * 0.5f * num3;
		SecondaryTargets.GetComponent<UIGrid>().Reposition();
		animationCounter = 0;
		UpdateWithoutAnimation(targetsAtGigStart);
	}

	private IEnumerator WaitAndAnimate(int currentTotal)
	{
		for (int i = animationCounter; i < currentTotal; i++)
		{
			animationCounter++;
			targets[m_animatedIndex].GetComponent<LevelTargetIndicator>().SetTargetState(LevelTargetIndicator.TargetState.Achieved);
			if (!m_skipped)
			{
				targets[m_animatedIndex].GetComponent<LevelTargetIndicator>().TriggerNewTargetAnimation(0.6f);
				yield return new WaitForSeconds(0.8f);
			}
			else
			{
				targets[m_animatedIndex].GetComponent<LevelTargetIndicator>().TriggerNewTargetAnimation(0.3f);
				yield return new WaitForSeconds(0.3f);
			}
			m_animatedIndex++;
		}
		SendMessageUpwards("OnTargetsAnimated");
	}

	public void SkipAnimation()
	{
		m_skipped = true;
	}

	public void UpdateWithoutAnimation()
	{
		UpdateWithoutAnimation(level.AchievedLevelTargets.Count);
	}

	public void UpdateWithoutAnimation(int count)
	{
		for (int i = 0; i < count; i++)
		{
			targets[i].GetComponent<LevelTargetIndicator>().SetTargetState(LevelTargetIndicator.TargetState.Achieved);
			m_animatedIndex++;
		}
	}

	public void UpdateTargetStatus(int currentTotal, bool animated = true)
	{
		if (currentTotal <= animationCounter)
		{
			if (animated)
			{
				SendMessageUpwards("OnTargetsAnimated");
			}
		}
		else if (animated)
		{
			m_skipped = false;
			StartCoroutine(WaitAndAnimate(currentTotal));
		}
		else
		{
			UpdateWithoutAnimation(currentTotal);
		}
	}

	private Vector3 CalculateGridPosition(int i, int total, int itemWidth)
	{
		float num = Mathf.Sin((float)Math.PI / 180f * ((float)(90 - total * 5) + (float)(i * total) * 10f / (float)(total - 1))) - 0.5f;
		return Vector3.right * (-0.5f * (float)itemWidth * (float)total + 0.5f * (float)itemWidth + (float)(itemWidth * i)) + num * 200f * Vector3.up;
	}
}
