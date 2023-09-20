using System.Collections.Generic;
using UnityEngine.Advertisements.Event;

namespace UnityEngine.Advertisements
{
	internal class Zone
	{
		private IntervalManager _zoneIntervals;

		private AdapterManager _adapterManager;

		public string Id { get; private set; }

		public bool precache { get; private set; }

		public bool suspendOnShow { get; private set; }

		public bool isDefault { get; private set; }

		public Zone(Dictionary<string, object> data)
		{
			Id = (string)data["id"];
			isDefault = (bool)data["default"];
			precache = (bool)data["precache"];
			suspendOnShow = (bool)data["suspendOnShow"];
			_adapterManager = new AdapterManager(Id, (List<object>)data["adapters"]);
		}

		public Adapter SelectAdapter()
		{
			if (!_zoneIntervals.IsAvailable())
			{
				EventManager.sendMediationCappedEvent(Engine.Instance.AppId, Id, null, _zoneIntervals.NextAvailable());
			}
			if (IsReady())
			{
				Adapter adapter = _adapterManager.SelectAdapter();
				if (adapter == null)
				{
					return null;
				}
				Utils.LogDebug("Consuming ad slot for zone " + Id);
				_zoneIntervals.Consume();
				if (_zoneIntervals.IsEmpty())
				{
					Utils.LogDebug("Zone " + Id + " ad interval list empty");
					ConfigManager.Instance.RequestAdSources();
				}
				return adapter;
			}
			return null;
		}

		public Dictionary<string, List<long>> GetConsumeTimes(long lastServerTimestamp)
		{
			return _adapterManager.GetConsumeTimes(lastServerTimestamp);
		}

		public void UpdateIntervals(Dictionary<string, object> adSources)
		{
			_zoneIntervals = new IntervalManager((List<object>)adSources["adIntervals"]);
			Utils.LogDebug(string.Concat("Got ", _zoneIntervals, " intervals for ", Id));
			_adapterManager.UpdateIntervals((List<object>)adSources["adapters"]);
		}

		public bool IsReady()
		{
			return _zoneIntervals != null && _zoneIntervals.IsAvailable() && _adapterManager != null && _adapterManager.IsReady();
		}
	}
}
