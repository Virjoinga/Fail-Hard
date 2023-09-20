using System;
using System.Collections.Generic;
using UnityEngine.Advertisements.MiniJSON;

namespace UnityEngine.Advertisements.Event
{
	internal class EventManager
	{
		private static EventManager impl;

		private static bool initialized = false;

		private string endpointUrl;

		private static int[] retryDelays = new int[4] { 15, 30, 90, 240 };

		private static object initLock = new object();

		private static EventManager getImpl()
		{
			lock (initLock)
			{
				if (!initialized)
				{
					impl = new EventManager();
					Event.init();
					initialized = true;
				}
			}
			return impl;
		}

		public static void sendEvent(bool retryable, string eventObject)
		{
			getImpl().executeEvent(retryable, eventObject, DeviceInfo.getJson(), dummyCallback);
		}

		public static void sendEvent(bool retryable, string eventObject, Action<bool> callback)
		{
			getImpl().executeEvent(retryable, eventObject, DeviceInfo.getJson(), callback);
		}

		private static void dummyCallback(bool result)
		{
		}

		public static void sendStartEvent(string appId)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("category", "ucmsdk");
			dictionary.Add("action", "start");
			dictionary.Add("application", appId);
			string eventObject = Json.Serialize(dictionary);
			sendEvent(false, eventObject);
		}

		public static void sendAdreqEvent(string gameId)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("category", "ucasdk");
			dictionary.Add("action", "adreq");
			dictionary.Add("application", gameId);
			string eventObject = Json.Serialize(dictionary);
			sendEvent(false, eventObject);
		}

		public static void sendAdplanEvent(string gameId)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("category", "ucasdk");
			dictionary.Add("action", "adplan");
			dictionary.Add("application", gameId);
			string eventObject = Json.Serialize(dictionary);
			sendEvent(false, eventObject);
		}

		public static void sendViewEvent(string gameId, string campaignId)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("category", "ucasdk");
			dictionary.Add("action", "view");
			dictionary.Add("application", gameId);
			dictionary.Add("source", "picture");
			dictionary.Add("target", campaignId);
			string eventObject = Json.Serialize(dictionary);
			sendEvent(true, eventObject);
		}

		public static void sendClickEvent(string gameId, string campaignId)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("category", "ucasdk");
			dictionary.Add("action", "click");
			dictionary.Add("application", gameId);
			dictionary.Add("source", "picture");
			dictionary.Add("target", campaignId);
			string eventObject = Json.Serialize(dictionary);
			sendEvent(true, eventObject);
		}

		public static void sendCloseEvent(string gameId, string campaignId, bool autoClosed)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("category", "ucasdk");
			dictionary.Add("action", "close");
			dictionary.Add("application", gameId);
			dictionary.Add("source", (!autoClosed) ? "picture_user" : "picture_auto");
			dictionary.Add("target", campaignId);
			string eventObject = Json.Serialize(dictionary);
			sendEvent(false, eventObject);
		}

		public static void sendMediationInitEvent(string appId)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("category", "ucmsdk");
			dictionary.Add("action", "init");
			dictionary.Add("application", appId);
			string eventObject = Json.Serialize(dictionary);
			sendEvent(false, eventObject);
		}

		public static void sendMediationAdSourcesEvent(string appId)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("category", "ucmsdk");
			dictionary.Add("action", "adsources");
			dictionary.Add("application", appId);
			string eventObject = Json.Serialize(dictionary);
			sendEvent(false, eventObject);
		}

		public static void sendMediationHasFillEvent(string appId, string zoneId, string source)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("category", "ucmsdk");
			dictionary.Add("action", "hasfill");
			dictionary.Add("application", appId);
			dictionary.Add("zone", zoneId);
			dictionary.Add("source", source);
			string eventObject = Json.Serialize(dictionary);
			sendEvent(false, eventObject);
		}

		public static void sendMediationCappedEvent(string appId, string zoneId, string source, long time)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("category", "ucmsdk");
			dictionary.Add("action", "capped");
			dictionary.Add("application", appId);
			dictionary.Add("zone", zoneId);
			dictionary.Add("source", source);
			dictionary.Add("value", time);
			string eventObject = Json.Serialize(dictionary);
			sendEvent(false, eventObject);
		}

		public static void sendMediationShowEvent(string appId, string zoneId, string adapterId)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("category", "ucmsdk");
			dictionary.Add("action", "show");
			dictionary.Add("application", appId);
			dictionary.Add("zone", zoneId);
			dictionary.Add("source", adapterId);
			string eventObject = Json.Serialize(dictionary);
			sendEvent(false, eventObject);
		}

		private void executeEvent(bool retryable, string eventObject, string infoObject, Action<bool> callback)
		{
			Event @event = new Event(Settings.eventEndpoint, retryDelays, retryable, eventObject, infoObject);
			@event.execute(callback);
		}
	}
}
