public static class Version
{
	private const int MAJOR = 12;

	private const int MINOR = 557;

	private const int m_clientVersionX = 1;

	private const int m_clientVersionY = 0;

	private const int m_clientVersionZ = 19;

	public static readonly string PROTOCOL;

	public static readonly string CLIENT;

	public static readonly int[] PROTOCOL_A;

	static Version()
	{
		PROTOCOL = 12 + "." + 557;
		PROTOCOL_A = new int[2] { 12, 557 };
		CLIENT = 1 + "." + 0 + "." + 19;
	}
}
