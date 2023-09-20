using UnityEngine;

public class Reacher : MonoBehaviour
{
	public Transform reachTarget;

	public IKSolverRestriction ikSolver;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnTriggerEnter(Collider hit)
	{
		reachTarget = hit.gameObject.transform;
		ikSolver.target = reachTarget;
	}

	private void OnTriggerExit(Collider hit)
	{
		ikSolver.target = ikSolver.transform;
	}
}
