using UnityEngine;

namespace Game
{
	public class JumpBooster : MonoBehaviour
	{
		public float JumpBoostStrength = 1f;

		private bool m_isBoosting;

		private VehicleBase m_boostedVehicle;

		private void Start()
		{
		}

		private void FixedUpdate()
		{
			if (!m_isBoosting && m_boostedVehicle != null && m_boostedVehicle != null)
			{
				m_boostedVehicle.JumpDirectionModifier = Vector3.zero;
				m_boostedVehicle.InJumpZone = false;
				m_boostedVehicle = null;
			}
			m_isBoosting = false;
		}

		private void OnTriggerEnter(Collider coll)
		{
			if (coll.gameObject.tag == "Vehicle" && m_boostedVehicle == null)
			{
				m_isBoosting = true;
				m_boostedVehicle = FindVehicle(coll.gameObject);
				if (m_boostedVehicle != null)
				{
					m_boostedVehicle.JumpDirectionModifier = JumpBoostStrength * base.transform.right;
					m_boostedVehicle.InJumpZone = true;
				}
			}
		}

		private void OnTriggerStay(Collider coll)
		{
			if (coll.gameObject.tag == "Vehicle")
			{
				m_isBoosting = true;
			}
		}

		private VehicleBase FindVehicle(GameObject go)
		{
			Transform parent = go.transform;
			while (parent.parent != null)
			{
				parent = parent.parent;
			}
			return parent.gameObject.GetComponentInChildren<VehicleBase>();
		}
	}
}
