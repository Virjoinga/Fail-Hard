using System;
using System.Collections.Generic;
using UnityEngine;

public class WaterSurfaceObjectFloater : MonoBehaviour
{
	private List<FloatingObject> floatingBodies;

	public float surfaceLevelYOriginal;

	private float surfaceLevelY;

	private float animationPhase;

	public float animationSpeed;

	public float animationAmplitude;

	public float maxBlastEffect;

	public float maxBlastDistance;

	private void Start()
	{
		floatingBodies = new List<FloatingObject>();
	}

	private void FixedUpdate()
	{
		animationPhase += Time.fixedDeltaTime * animationSpeed * 2f * (float)Math.PI;
		int num = 0;
		foreach (FloatingObject floatingBody in floatingBodies)
		{
			num++;
			surfaceLevelY = surfaceLevelYOriginal + animationAmplitude * Mathf.Sin(animationPhase + (float)num);
			float num2 = surfaceLevelY - floatingBody.rigidbody.position.y;
			if (!(num2 < 0f))
			{
				float num3 = Mathf.Clamp(num2 / floatingBody.floatingDepth, 0f, 1.5f);
				Vector3 force = -Physics.gravity * floatingBody.rigidbody.mass * num3 * floatingBody.buoyancy;
				floatingBody.rigidbody.AddForce(force);
			}
		}
	}

	private void CreateRestriction(FloatingObject fo)
	{
		if (!fo.GetComponent<ConfigurableJoint>())
		{
			ConfigurableJoint configurableJoint = fo.gameObject.AddComponent<ConfigurableJoint>();
			configurableJoint.anchor = Vector3.zero;
			if (fo.restrictions.x > 0f)
			{
				configurableJoint.angularXMotion = ConfigurableJointMotion.Limited;
				SoftJointLimit lowAngularXLimit = configurableJoint.lowAngularXLimit;
				lowAngularXLimit.limit = 0f - fo.restrictions.x;
				lowAngularXLimit.spring = 10f;
				configurableJoint.lowAngularXLimit = lowAngularXLimit;
				lowAngularXLimit = configurableJoint.highAngularXLimit;
				lowAngularXLimit.limit = fo.restrictions.x;
				lowAngularXLimit.spring = 10f;
				configurableJoint.highAngularXLimit = lowAngularXLimit;
			}
			if (fo.restrictions.y > 0f)
			{
				configurableJoint.angularYMotion = ConfigurableJointMotion.Limited;
				SoftJointLimit angularYLimit = configurableJoint.angularYLimit;
				angularYLimit.limit = fo.restrictions.y;
				angularYLimit.spring = 10f;
				configurableJoint.angularYLimit = angularYLimit;
			}
			if (fo.restrictions.z > 0f)
			{
				configurableJoint.angularZMotion = ConfigurableJointMotion.Limited;
				SoftJointLimit angularZLimit = configurableJoint.angularZLimit;
				angularZLimit.limit = fo.restrictions.z;
				angularZLimit.spring = 10f;
				configurableJoint.angularZLimit = angularZLimit;
			}
		}
	}

	private void OnTriggerEnter(Collider coll)
	{
		FloatingObject componentInChildren = coll.GetComponentInChildren<FloatingObject>();
		if (componentInChildren != null)
		{
			if (floatingBodies.Contains(componentInChildren))
			{
				return;
			}
			floatingBodies.Add(componentInChildren);
			if (componentInChildren.restrictions.sqrMagnitude > 0f)
			{
				CreateRestriction(componentInChildren);
			}
		}
		if ((!(coll.gameObject.tag == "Player") && !(coll.gameObject.tag == "Vehicle")) || !(coll.rigidbody != null) || !(coll.rigidbody.velocity.y < -2f))
		{
			return;
		}
		foreach (FloatingObject floatingBody in floatingBodies)
		{
			Vector3 vector = floatingBody.rigidbody.position - coll.rigidbody.position;
			float magnitude = vector.magnitude;
			if (!(magnitude < 0.1f))
			{
				magnitude = Mathf.Clamp(magnitude, 0f, maxBlastDistance);
				Vector3 vector2 = vector.normalized * maxBlastEffect * (1f - magnitude / maxBlastDistance);
				floatingBody.rigidbody.AddForce(vector2 * floatingBody.rigidbody.mass);
			}
		}
	}

	private void OnTriggerExit(Collider coll)
	{
		FloatingObject componentInChildren = coll.GetComponentInChildren<FloatingObject>();
		if (componentInChildren != null)
		{
			floatingBodies.Remove(componentInChildren);
		}
	}
}
