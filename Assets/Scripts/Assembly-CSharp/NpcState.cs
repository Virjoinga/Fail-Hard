using UnityEngine;

public class NpcState : MonoBehaviour
{
	protected Transform root;

	public PlayerAnimationController pac;

	public Npc Npc;

	private void Start()
	{
		root = FindRoot(base.transform);
		pac = root.GetComponentInChildren<PlayerAnimationController>();
		Npc = root.GetComponentInChildren<Npc>();
	}

	public virtual void Enter()
	{
	}

	public virtual void Exit()
	{
	}

	public virtual void UpdateFunc()
	{
	}

	public virtual void FixedUpdateFunc()
	{
	}

	public virtual void CollisionEnter(BodyPartType bodyPartType, float impactMagnitude)
	{
	}

	public virtual void AssignObject(GameObject obj)
	{
		Debug.LogWarning("AssignObject dummy implementation called.");
	}

	private Transform FindRoot(Transform node)
	{
		while (node != null)
		{
			if (node.parent == null)
			{
				return node;
			}
			node = node.parent;
		}
		return node;
	}
}
