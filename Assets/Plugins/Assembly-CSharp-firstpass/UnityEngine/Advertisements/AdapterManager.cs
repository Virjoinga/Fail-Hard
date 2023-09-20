using System;
using System.Collections.Generic;
using UnityEngine.Advertisements.Event;

namespace UnityEngine.Advertisements
{
	internal class AdapterManager
	{
		private string _zoneId;

		private List<KeyValuePair<string, Adapter>> _adapters = new List<KeyValuePair<string, Adapter>>();

		private Dictionary<string, IntervalManager> _adapterIntervals = new Dictionary<string, IntervalManager>();

		private Dictionary<string, KeyValuePair<long, long>> _adapterRefreshFreqs = new Dictionary<string, KeyValuePair<long, long>>();

		private Dictionary<string, List<long>> _adapterConsumeTimes = new Dictionary<string, List<long>>();

		public AdapterManager(string zoneId, List<object> data)
		{
			new VideoAdAdapter("VideoAdAdapter");
			new PictureAdAdapter("PictureAdAdapter");
			_zoneId = zoneId;
			foreach (object datum in data)
			{
				if (datum == null)
				{
					continue;
				}
				Dictionary<string, object> dictionary = (Dictionary<string, object>)datum;
				string adapterId = (string)dictionary["id"];
				string text = (string)dictionary["namespace"];
				string text2 = (string)dictionary["className"];
				Dictionary<string, object> configuration = Utils.Optional<Dictionary<string, object>>(dictionary, "parameters");
				long value = (long)dictionary["refreshAdPlanFreq"];
				Type type = Type.GetType(text + "." + text2);
				if (type != null)
				{
					Adapter adapter = (Adapter)Activator.CreateInstance(type, adapterId);
					adapter.Subscribe(Adapter.EventType.adAvailable, delegate
					{
						EventManager.sendMediationHasFillEvent(Engine.Instance.AppId, zoneId, adapterId);
					});
					adapter.Initialize(zoneId, adapterId, configuration);
					_adapters.Add(new KeyValuePair<string, Adapter>(adapterId, adapter));
					_adapterIntervals.Add(adapterId, null);
					_adapterRefreshFreqs.Add(adapterId, new KeyValuePair<long, long>((long)Math.Round(Time.realtimeSinceStartup), value));
					_adapterConsumeTimes.Add(adapterId, new List<long>());
				}
			}
		}

		public Adapter SelectAdapter()
		{
			HashSet<string> hashSet = NonCappedAdapters();
			foreach (KeyValuePair<string, Adapter> adapter in _adapters)
			{
				string key = adapter.Key;
				Adapter value = adapter.Value;
				if (!_adapterIntervals[key].IsAvailable())
				{
					EventManager.sendMediationCappedEvent(Engine.Instance.AppId, _zoneId, key, _adapterIntervals[key].NextAvailable());
				}
				if (!value.isReady(_zoneId, key))
				{
					long key2 = _adapterRefreshFreqs[key].Key;
					long value2 = _adapterRefreshFreqs[key].Value;
					if ((long)Math.Round(Time.realtimeSinceStartup) >= key2 + value2)
					{
						value.RefreshAdPlan();
						_adapterRefreshFreqs[key] = new KeyValuePair<long, long>((long)Math.Round(Time.realtimeSinceStartup), value2);
					}
				}
				if (hashSet.Contains(key) && value.isReady(_zoneId, key))
				{
					_adapterIntervals[key].Consume();
					_adapterConsumeTimes[key].Add(ConfigManager.Instance.serverTimestamp + (long)Math.Round(Time.realtimeSinceStartup));
					if (AllAdaptersConsumed())
					{
						ConfigManager.Instance.RequestAdSources();
					}
					Utils.LogDebug("Selecting adapter '" + key + "' from zone '" + _zoneId + "'");
					return value;
				}
			}
			return null;
		}

		public void UpdateIntervals(List<object> adSources)
		{
			foreach (object adSource in adSources)
			{
				Dictionary<string, object> dictionary = (Dictionary<string, object>)adSource;
				string text = (string)dictionary["id"];
				if (_adapterIntervals.ContainsKey(text))
				{
					_adapterConsumeTimes[text].Clear();
					_adapterIntervals[text] = new IntervalManager((List<object>)dictionary["adIntervals"]);
					Utils.LogDebug(string.Concat("Got intervals ", _adapterIntervals[text], " for ", text));
				}
			}
		}

		public Dictionary<string, List<long>> GetConsumeTimes(long lastServerTimestamp)
		{
			Dictionary<string, List<long>> dictionary = new Dictionary<string, List<long>>();
			foreach (KeyValuePair<string, List<long>> adapterConsumeTime in _adapterConsumeTimes)
			{
				List<long> list = new List<long>();
				foreach (long item in adapterConsumeTime.Value)
				{
					list.Add(item - ConfigManager.Instance.localTimestamp - lastServerTimestamp);
				}
				dictionary.Add(adapterConsumeTime.Key, list);
			}
			return dictionary;
		}

		private HashSet<string> NonCappedAdapters()
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (KeyValuePair<string, IntervalManager> adapterInterval in _adapterIntervals)
			{
				IntervalManager value = adapterInterval.Value;
				if (value.IsAvailable())
				{
					hashSet.Add(adapterInterval.Key);
				}
			}
			return hashSet;
		}

		private bool AllAdaptersConsumed()
		{
			foreach (KeyValuePair<string, IntervalManager> adapterInterval in _adapterIntervals)
			{
				IntervalManager value = adapterInterval.Value;
				if (!value.IsEmpty())
				{
					return false;
				}
			}
			return true;
		}

		public bool IsReady()
		{
			HashSet<string> hashSet = NonCappedAdapters();
			foreach (KeyValuePair<string, Adapter> adapter in _adapters)
			{
				if (hashSet.Contains(adapter.Key) && adapter.Value.isReady(_zoneId, adapter.Key))
				{
					return true;
				}
			}
			return false;
		}
	}
}
