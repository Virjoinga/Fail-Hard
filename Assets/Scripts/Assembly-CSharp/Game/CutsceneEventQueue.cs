using System.Collections.Generic;

namespace Game
{
	public class CutsceneEventQueue
	{
		public List<GameEvent> UnhandledEvents;

		public CutsceneEventQueue(Character character)
		{
			UnhandledEvents = new List<GameEvent>();
			character.GameEventRegistered += character_GameEventRegistered;
		}

		private void character_GameEventRegistered(GameEvent gameEvent)
		{
			GameEvent.GameEventType eventType = gameEvent.EventType;
			if (eventType == GameEvent.GameEventType.EnteringGig || eventType == GameEvent.GameEventType.VehicleBroken)
			{
				UnhandledEvents.Add(gameEvent);
			}
		}

		public bool ContainsEvents(List<GameEvent> geList)
		{
			if (geList.Count == 0)
			{
				return true;
			}
			if (UnhandledEvents == null)
			{
				return false;
			}
			foreach (GameEvent ge in geList)
			{
				if (!UnhandledEvents.Contains(ge))
				{
					return false;
				}
			}
			return true;
		}
	}
}
