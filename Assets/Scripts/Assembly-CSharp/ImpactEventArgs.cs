using System;
using UnityEngine;

public class ImpactEventArgs : EventArgs
{
	public BodyPartType bodyPartType;

	public float impactMagnitude;

	public GameObject collidingObject;

	public ImpactEventArgs(BodyPartType bodyPartType, float impactMagnitude, GameObject collidingObject)
	{
		this.bodyPartType = bodyPartType;
		this.impactMagnitude = impactMagnitude;
		this.collidingObject = collidingObject;
	}
}
