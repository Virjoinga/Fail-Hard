using System;
using System.Collections.Generic;
using Holoville.HOTween;
using UnityEngine;

public class LookAtMove : MonoBehaviour
{
	[Serializable]
	public class MoveTarget
	{
		public Transform Target;

		public float MoveTime;

		public float StayTime;
	}

	public List<MoveTarget> MoveTargets;

	public Transform LookAtTarget;

	private MoveTarget m_currentMoveTarget;

	private Transform m_transform;

	private void Start()
	{
		Application.targetFrameRate = 60;
		m_transform = base.transform;
		m_currentMoveTarget = MoveTargets[0];
		SetNewPosTarget(m_currentMoveTarget);
	}

	private void Update()
	{
		m_transform.LookAt(LookAtTarget);
	}

	private void SetNewPosTarget(MoveTarget moveTarget)
	{
		Tweener tweener = HOTween.To(m_transform, moveTarget.MoveTime, "localPosition", moveTarget.Target.position);
		tweener.ApplyCallback(CallbackType.OnComplete, NextTargetMove);
	}

	public void NextTargetMove()
	{
		Invoke("InvokeNextTargetMove", m_currentMoveTarget.StayTime);
		int num = MoveTargets.IndexOf(m_currentMoveTarget);
		num++;
		num %= MoveTargets.Count;
		m_currentMoveTarget = MoveTargets[num];
	}

	private void InvokeNextTargetMove()
	{
		SetNewPosTarget(m_currentMoveTarget);
	}
}
