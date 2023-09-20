using System.Collections.Generic;
using UnityEngine;

public class GestureNode
{
	public string name;

	public IGestureCriteria criteria;

	public List<GestureNode> nodes;

	public Gesture attachedGesture;

	private bool reset;

	public GestureNode(IGestureCriteria criteria)
	{
		this.criteria = criteria;
		nodes = new List<GestureNode>();
	}

	public void ResetCriteriasRecursively(Vector2 position)
	{
		if (reset)
		{
			reset = false;
			return;
		}
		criteria.origin = position;
		criteria.cumulativeDelta *= 0f;
		reset = true;
		foreach (GestureNode node in nodes)
		{
			node.ResetCriteriasRecursively(position);
		}
		reset = false;
	}
}
