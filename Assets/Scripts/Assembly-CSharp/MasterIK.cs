using UnityEngine;

public class MasterIK : MonoBehaviour
{
	public Transform target;

	public Transform b0;

	public Transform b1;

	public Transform b2;

	public Transform b3;

	public Vector3 b0_v;

	public Vector3 b1_v;

	public float b0_len;

	public float b1_len;

	public float ikThreshold = 0.1f;

	private void Start()
	{
		b1_v = b2.position - b1.position;
		b1_len = b1_v.magnitude;
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.R))
		{
			ResetIK();
		}
	}

	private void FixedUpdate()
	{
		float magnitude = (target.position - b1.position).magnitude;
		float magnitude2 = (b3.position - b1.position).magnitude;
		CheckLimit(b0, b1);
		CheckLimit(b1, b2);
		if (magnitude2 - magnitude > ikThreshold)
		{
			b0.GetComponent<BoneOrientter>().targetPoint.localPosition -= 0.5f * Time.deltaTime * b1.forward;
		}
		else if (magnitude2 - magnitude < 0f - ikThreshold)
		{
			b0.GetComponent<BoneOrientter>().targetPoint.localPosition += 0.5f * Time.deltaTime * b1.forward;
		}
	}

	public Vector3 CheckLimit(Transform start, Transform end)
	{
		Vector3 zero = Vector3.zero;
		Vector3 vector = end.right - Vector3.Project(end.right, start.up);
		float num = Vector3.Angle(start.right, vector);
		if (Vector3.Dot(vector, start.forward) > 0f)
		{
			num = 0f - num;
		}
		zero.y = num;
		Vector3 vector2 = end.right - Vector3.Project(end.right, start.forward);
		num = Vector3.Angle(start.right, vector2);
		if (Vector3.Dot(vector2, start.up) > 0f)
		{
			num = 0f - num;
		}
		zero.z = num;
		return zero;
	}

	public void ResetIK()
	{
		Vector3 normalized = (target.position - b1.position).normalized;
		b0.GetComponent<BoneOrientter>().targetPoint.localPosition = b1.localPosition + b1_len * normalized;
	}
}
