using UnityEngine;

public class WheelGroundContact : MonoBehaviour
{
	public GameObject SimpleCollider;

	public GameObject ComplexCollider;

	private bool m_newContact;

	private bool m_newSnow;

	public bool IsContact { get; private set; }

	public Vector3 ForwardDir { get; private set; }

	public bool IsSnow { get; private set; }

	private void Start()
	{
		IsContact = false;
		m_newContact = false;
		m_newSnow = false;
		SwitchToAccurateCollider(false);
	}

	private void FixedUpdate()
	{
		IsContact = m_newContact;
		m_newContact = false;
		IsSnow = m_newSnow;
		m_newSnow = false;
	}

	public void SwitchToAccurateCollider(bool isAccurate)
	{
		ComplexCollider.SetActive(isAccurate);
		SimpleCollider.SetActive(!isAccurate);
	}

	public void SetFriction(float val)
	{
		ComplexCollider.GetComponent<Collider>().material.dynamicFriction = 1f;
		SimpleCollider.GetComponent<Collider>().material.dynamicFriction = 1f;
	}

	private void RefreshContactState(Collision hit)
	{
		if (!(hit.gameObject.tag != "Vehicle") || hit.contacts.Length <= 0)
		{
			return;
		}
		m_newContact = true;
		IsContact = true;
		ContactPoint[] contacts = hit.contacts;
		for (int i = 0; i < contacts.Length; i++)
		{
			ContactPoint contactPoint = contacts[i];
			if (contactPoint.otherCollider.gameObject.tag != "Vehicle")
			{
				AJMaterial component = contactPoint.otherCollider.gameObject.GetComponent<AJMaterial>();
				if (component != null && component.surfaceMaterialType == SurfaceMaterialType.Snow)
				{
					m_newSnow = true;
					IsSnow = true;
				}
				ForwardDir = Vector3.Cross(base.transform.right, contactPoint.normal);
				break;
			}
		}
	}

	private void OnCollisionEnter(Collision hit)
	{
		RefreshContactState(hit);
	}

	private void OnCollisionStay(Collision hit)
	{
		RefreshContactState(hit);
	}
}
