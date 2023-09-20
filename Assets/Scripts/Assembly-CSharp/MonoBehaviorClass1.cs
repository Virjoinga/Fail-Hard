using UnityEngine;

public class MonoBehaviorClass1 : MonoBehaviour
{
	public const float PUBLIC_THRESHOLD = 10f;

	private const float THRESHOLD = 5f;

	private float m_test;

	private void Start()
	{
		if (m_test > 5f)
		{
			DoSomething();
		}
	}

	private void Update()
	{
	}

	public float NewValue()
	{
		return m_test;
	}

	private void DoSomething()
	{
		int num = 0;
		int num2 = 2;
		int num3 = num + num2;
		m_test = num3;
	}
}
