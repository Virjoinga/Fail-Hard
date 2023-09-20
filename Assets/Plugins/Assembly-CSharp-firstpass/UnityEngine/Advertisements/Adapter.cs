using System;
using System.Collections.Generic;

namespace UnityEngine.Advertisements
{
	public abstract class Adapter
	{
		public enum EventType
		{
			initStart = 0,
			initFailed = 1,
			initComplete = 2,
			adAvailable = 3,
			adWillOpen = 4,
			adDidOpen = 5,
			adWillClose = 6,
			adDidClose = 7,
			adStarted = 8,
			adSkipped = 9,
			adFinished = 10,
			adClicked = 11,
			error = 12
		}

		private string _adapterId;

		private Dictionary<EventType, Delegate> _events = new Dictionary<EventType, Delegate>();

		public virtual string Id
		{
			get
			{
				return _adapterId;
			}
		}

		protected Adapter(string adapterId)
		{
			_adapterId = adapterId;
			EventType[] array = (EventType[])Enum.GetValues(typeof(EventType));
			foreach (EventType key in array)
			{
				_events.Add(key, null);
			}
		}

		public virtual void Subscribe(EventType eventType, EventHandler handler)
		{
			lock (_events)
			{
				_events[eventType] = (EventHandler)Delegate.Combine((EventHandler)_events[eventType], handler);
			}
		}

		public virtual void Unsubscribe(EventType eventType, EventHandler handler)
		{
			lock (_events)
			{
				_events[eventType] = (EventHandler)Delegate.Remove((EventHandler)_events[eventType], handler);
			}
		}

		public abstract void Initialize(string zoneId, string adapterId, Dictionary<string, object> configuration);

		public abstract void RefreshAdPlan();

		public abstract void StartPrecaching();

		public abstract void StopPrecaching();

		public abstract bool isReady(string zoneId, string adapterId);

		public abstract void Show(string zoneId, string adapterId, ShowOptions options = null);

		public abstract bool isShowing();

		protected virtual void triggerEvent(EventType eventType, EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)_events[eventType];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}
	}
}
