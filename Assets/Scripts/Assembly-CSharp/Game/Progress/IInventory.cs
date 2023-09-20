using System.Collections;

namespace Game.Progress
{
	public interface IInventory
	{
		ArrayList Contents { get; set; }

		void AddItem(IItem item);

		ArrayList GetItemsWithCapability(ArrayList capabilities);

		ArrayList GetItemsWithCategory(ArrayList categories);
	}
}
