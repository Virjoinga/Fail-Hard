using UnityEngine;

public class DetachOnTrigger : MonoBehaviour
{
	private GigStatus m_gigController;

	private void Start()
	{
		m_gigController = GameObject.Find("GigController").GetComponent<GigStatus>();
	}

	private void OnTriggerEnter(Collider hit)
	{
		if (hit.gameObject.tag == "Player")
		{
			m_gigController.player.JumpOffVehicle(true);
		}
	}

	private void OnCollisionEnter(Collision hit)
	{
		if (hit.gameObject.tag == "Player")
		{
			m_gigController.player.JumpOffVehicle(true);
		}
	}
}
