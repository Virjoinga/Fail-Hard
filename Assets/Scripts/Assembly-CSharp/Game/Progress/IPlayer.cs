namespace Game.Progress
{
	public interface IPlayer : IId
	{
		IInventory Inventory { get; set; }

		IShop Shop { get; set; }

		int Cash { get; set; }
	}
}
