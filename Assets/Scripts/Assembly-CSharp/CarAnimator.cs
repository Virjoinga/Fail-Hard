using System.Collections.Generic;
using UnityEngine;

public class CarAnimator : MonoBehaviour
{
	public List<Transform> CheckPoints;

	public float CruiseSpeed;

	private float maxCruiseSpeed;

	private float currentCruiseSpeed;

	public float TurnRadius;

	public int CurrentTarget;

	public List<Transform> Tires;

	public Vector3 TireRotationAxis;

	private bool obstacleAhead;

	private bool accelerate;

	private float speedAdjustStatus;

	private Transform target;

	private int tCount;

	private void Start()
	{
		maxCruiseSpeed = 0.75f * CruiseSpeed + CruiseSpeed * Random.value * 0.5f;
	}

	private void Update()
	{
		if (obstacleAhead)
		{
			if (target == null)
			{
				tCount = 0;
				obstacleAhead = false;
				currentCruiseSpeed = CruiseSpeed;
				speedAdjustStatus = 0f;
			}
			else
			{
				speedAdjustStatus += Time.deltaTime / 0.3f;
				CruiseSpeed = Mathf.Lerp(currentCruiseSpeed, 0f, speedAdjustStatus);
			}
		}
		else
		{
			speedAdjustStatus += Time.deltaTime / 5f;
			CruiseSpeed = Mathf.Lerp(currentCruiseSpeed, maxCruiseSpeed, speedAdjustStatus);
		}
		Vector3 normalized = (CheckPoints[CurrentTarget].position - base.rigidbody.position).normalized;
		normalized = Vector3.Slerp(base.transform.forward, normalized, 5f * Time.deltaTime);
		base.transform.position += normalized * Time.deltaTime * CruiseSpeed;
		base.transform.LookAt(base.rigidbody.position + normalized, Vector3.up);
		if ((CheckPoints[CurrentTarget].position - base.rigidbody.position).magnitude < TurnRadius)
		{
			CurrentTarget++;
			if (CurrentTarget == CheckPoints.Count)
			{
				CurrentTarget = 0;
			}
		}
		foreach (Transform tire in Tires)
		{
			tire.Rotate(50f * CruiseSpeed * Time.deltaTime * TireRotationAxis);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.isTrigger)
		{
			return;
		}
		target = other.transform.root;
		if (other.transform.root.name == "ActionJackson(Clone)" || other.transform.root.name == "MopedSharp(Clone)")
		{
			if (++tCount == 1)
			{
				obstacleAhead = true;
				currentCruiseSpeed = CruiseSpeed;
				speedAdjustStatus = 0f;
			}
		}
		else
		{
			obstacleAhead = true;
			currentCruiseSpeed = CruiseSpeed;
			speedAdjustStatus = 0f;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.isTrigger)
		{
			return;
		}
		if (other.transform.root.name == "ActionJackson(Clone)" || other.transform.root.name == "MopedSharp(Clone)")
		{
			if (--tCount == 0)
			{
				obstacleAhead = false;
				currentCruiseSpeed = CruiseSpeed;
				speedAdjustStatus = 0f;
			}
		}
		else
		{
			obstacleAhead = false;
			currentCruiseSpeed = CruiseSpeed;
			speedAdjustStatus = 0f;
		}
	}
}
