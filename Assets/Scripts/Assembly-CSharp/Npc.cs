using UnityEngine;

public class Npc : MonoBehaviour
{
	public NpcState currentState;

	public GameObject ConnectedObject;

	public ConfigurableJoint ButtConnector;

	public Transform handL;

	public Transform handR;

	public Transform footL;

	public Transform footR;

	public Vector3 rootPosition;

	private void Start()
	{
		currentState = base.gameObject.AddComponent<NpcStateTpose>();
		base.gameObject.AddComponent<NpcStateIdle>();
	}

	public void AssignObject(GameObject obj)
	{
		ConnectedObject = obj;
		currentState.AssignObject(obj);
		ButtConnector.connectedBody = ConnectedObject.rigidbody;
	}

	public void StartStunt()
	{
	}

	public void EndStunt()
	{
	}

	private void Update()
	{
		currentState.UpdateFunc();
	}

	private void FixedUpdate()
	{
		currentState.FixedUpdateFunc();
	}
}
