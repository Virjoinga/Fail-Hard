using System.Collections.Generic;
using UnityEngine;

public class GestureRecognizer
{
	public static bool enabled;

	private int fingersActive;

	private List<GestureNode> startNodes;

	private List<GestureNode> activeNodes;

	private List<Gesture> currentGestures;

	private List<Gesture> registeredGestures;

	private bool active;

	public GestureRecognizer()
	{
		startNodes = new List<GestureNode>();
		activeNodes = new List<GestureNode>();
		currentGestures = new List<Gesture>();
		registeredGestures = new List<Gesture>();
	}

	public void Reset()
	{
		startNodes.Clear();
		activeNodes.Clear();
		currentGestures.Clear();
	}

	public void RegisterGesture(Gesture g)
	{
		startNodes.Add(g.nodes[0]);
		registeredGestures.Add(g);
	}

	private void StartGesture(Vector2 position)
	{
		activeNodes.Clear();
		currentGestures.Clear();
		activeNodes.AddRange(startNodes);
		foreach (GestureNode activeNode in activeNodes)
		{
			activeNode.ResetCriteriasRecursively(position);
		}
		foreach (Gesture registeredGesture in registeredGestures)
		{
			foreach (GestureNode node in registeredGesture.nodes)
			{
				node.attachedGesture = null;
			}
		}
	}

	public void FeedInputEvents(Touch[] touches)
	{
		if (!enabled)
		{
			return;
		}
		List<GestureNode> list = new List<GestureNode>();
		if (active && touches.Length == 0)
		{
			foreach (GestureNode activeNode in activeNodes)
			{
				activeNode.criteria.Ended();
				if (activeNode.criteria.Reached())
				{
					HandleReachedNode(activeNode);
					list.Add(activeNode);
				}
			}
			active = false;
			fingersActive = 0;
		}
		else if (!active && touches.Length > 0)
		{
			foreach (GestureNode activeNode2 in activeNodes)
			{
				activeNode2.criteria.Began();
			}
			active = true;
		}
		for (int i = 0; i < touches.Length; i++)
		{
			Touch touch = touches[i];
			if (touch.phase == TouchPhase.Ended)
			{
				fingersActive--;
			}
			if (fingersActive <= 0 && touch.phase != TouchPhase.Ended)
			{
				StartGesture(touch.position);
			}
			if (touch.fingerId + 1 > fingersActive && touch.phase != TouchPhase.Ended)
			{
				fingersActive = touch.fingerId + 1;
			}
			foreach (GestureNode activeNode3 in activeNodes)
			{
				activeNode3.criteria.fingerCount = fingersActive;
				switch (touch.phase)
				{
				case TouchPhase.Moved:
					activeNode3.criteria.Moved(touch.deltaPosition);
					break;
				case TouchPhase.Stationary:
					activeNode3.criteria.Stationary();
					break;
				}
				if (activeNode3.criteria.Reached())
				{
					HandleReachedNode(activeNode3);
					list.Add(activeNode3);
					break;
				}
			}
		}
		if (list.Count <= 0)
		{
			return;
		}
		foreach (GestureNode item in list)
		{
			activeNodes.Remove(item);
			activeNodes.AddRange(item.nodes);
		}
	}

	private void HandleReachedNode(GestureNode node)
	{
		if (node.attachedGesture == null)
		{
			node.attachedGesture = new Gesture();
		}
		node.attachedGesture.nodes.Add(node);
		if (!currentGestures.Contains(node.attachedGesture))
		{
			currentGestures.Add(node.attachedGesture);
		}
		foreach (Gesture registeredGesture in registeredGestures)
		{
			if (registeredGesture.IsSameAs(node.attachedGesture))
			{
				registeredGesture.callback(registeredGesture);
			}
		}
		foreach (GestureNode node2 in node.nodes)
		{
			node2.attachedGesture = node.attachedGesture;
			node2.criteria.origin = node.criteria.origin + node.criteria.target;
			node2.criteria.cumulativeDelta = node.criteria.cumulativeDelta - node.criteria.target;
		}
	}
}
