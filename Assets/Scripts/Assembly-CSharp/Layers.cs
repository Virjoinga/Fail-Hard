public class Layers
{
	private enum Masks
	{
		Nothing = 0,
		UILayer = 0x100,
		Trampoline = 0x200
	}

	public static int Nothing
	{
		get
		{
			return 0;
		}
	}

	public static int UILayer
	{
		get
		{
			return 256;
		}
	}

	public static int Trampoline
	{
		get
		{
			return 512;
		}
	}
}
