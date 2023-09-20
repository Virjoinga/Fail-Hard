using UnityEngine;

[PBSerialize("JointConnector")]
public class JointConnector : BreakingStuff
{
	public Joint JointToConnectTo;

	[PBSerializeField]
	public Transform RigidBodyToConnect;

	[PBSerializeField]
	public bool DisconnectOnTrigger;

	private bool m_connected;

	private void Start()
	{
		if (JointToConnectTo != null && RigidBodyToConnect != null && RigidBodyToConnect.rigidbody != null)
		{
			JointToConnectTo.connectedBody = RigidBodyToConnect.rigidbody;
			m_connected = true;
		}
	}

	public override void Break()
	{
		if (m_connected)
		{
			m_connected = false;
			Object.Destroy(JointToConnectTo);
		}
	}
}
