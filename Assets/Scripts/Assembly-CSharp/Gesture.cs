using System.Collections.Generic;

public class Gesture
{
	public string name;

	public List<GestureNode> nodes;

	public TrackGesture callback;

	public Gesture(string na, TrackGesture cb, params GestureNode[] nodes)
	{
		callback = cb;
		name = na;
		this.nodes = new List<GestureNode>();
		foreach (GestureNode item in nodes)
		{
			this.nodes.Add(item);
		}
	}

	public Gesture()
	{
		callback = null;
		name = string.Empty;
		nodes = new List<GestureNode>();
	}

	public bool IsSameAs(Gesture n)
	{
		if (n.nodes.Count != nodes.Count)
		{
			return false;
		}
		for (int i = 0; i < nodes.Count; i++)
		{
			if (!n.nodes[i].Equals(nodes[i]))
			{
				return false;
			}
		}
		return true;
	}
}
