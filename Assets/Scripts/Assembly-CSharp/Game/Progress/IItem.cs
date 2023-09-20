using System.Collections;

namespace Game.Progress
{
	public interface IItem : IId
	{
		ArrayList Capabilities { get; set; }

		int Price { get; }

		bool Bought { get; set; }

		Price PriceTag { get; }
	}
}
