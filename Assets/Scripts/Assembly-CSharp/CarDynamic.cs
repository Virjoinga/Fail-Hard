using System;
using System.Collections.Generic;
using UnityEngine;

public class CarDynamic : MonoBehaviour
{
	public float WheelRadius;

	public List<Transform> WheelMeshes;

	public Vector3 COMAdjust;

	private void Start()
	{
		base.rigidbody.centerOfMass += COMAdjust;
	}

	private void Update()
	{
		float num = (float)Math.PI * 2f * WheelRadius;
		float num2 = Vector3.Dot(base.rigidbody.velocity, base.transform.forward);
		float num3 = num2 / num * 60f;
		for (int i = 0; i < WheelMeshes.Count; i++)
		{
			WheelMeshes[i].Rotate(Vector3.right * 6f * num3 * Time.deltaTime, Space.Self);
		}
	}
}
