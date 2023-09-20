public class Class1
{
	public const float PUBLIC_THRESHOLD = 10f;

	private const float THRESHOLD = 5f;

	private float m_test;

	public float Test
	{
		get
		{
			return m_test;
		}
	}

	public Class1()
	{
		m_test = 1f;
	}

	public void SetProperty(float value)
	{
		if (m_test > 5f)
		{
			m_test = value;
		}
	}

	private void DoSomething()
	{
		int num = 0;
		int num2 = 2;
		int num3 = num + num2;
		num3 += 2;
	}
}
