namespace Game
{
	public interface IMessageConsumer
	{
		void Consume(VMessage msg);
	}
}
