using System.Collections.Generic;

namespace UnityEngine.Advertisements
{
	internal class ZoneManager
	{
		private Zone defaultZone;

		private Dictionary<string, Zone> _zones = new Dictionary<string, Zone>();

		private static readonly ZoneManager _sharedInstance = new ZoneManager();

		public static ZoneManager Instance
		{
			get
			{
				return _sharedInstance;
			}
		}

		private ZoneManager()
		{
		}

		public void ResetZones(List<object> zones)
		{
			_zones.Clear();
			foreach (object zone2 in zones)
			{
				Zone zone = new Zone((Dictionary<string, object>)zone2);
				if (zone.isDefault)
				{
					defaultZone = zone;
				}
				_zones.Add(zone.Id, zone);
			}
		}

		public Zone GetDefaultZone()
		{
			return defaultZone;
		}

		public Zone GetZone(string zoneId)
		{
			if (zoneId == null)
			{
				return defaultZone;
			}
			if (_zones.ContainsKey(zoneId))
			{
				return _zones[zoneId];
			}
			return null;
		}

		public bool IsReady(string zoneId)
		{
			if (zoneId == null && defaultZone != null)
			{
				return defaultZone.IsReady();
			}
			if (zoneId != null && _zones.ContainsKey(zoneId))
			{
				return _zones[zoneId].IsReady();
			}
			return false;
		}

		public List<Zone> GetZones()
		{
			return new List<Zone>(_zones.Values);
		}

		public List<string> GetZoneIds()
		{
			return new List<string>(_zones.Keys);
		}

		public void UpdateIntervals(Dictionary<string, object> adSources)
		{
			foreach (KeyValuePair<string, object> adSource in adSources)
			{
				if (_zones.ContainsKey(adSource.Key))
				{
					_zones[adSource.Key].UpdateIntervals((Dictionary<string, object>)adSource.Value);
				}
			}
		}

		public Dictionary<string, Dictionary<string, List<long>>> GetConsumeTimes(long lastServerTimestamp)
		{
			Dictionary<string, Dictionary<string, List<long>>> dictionary = new Dictionary<string, Dictionary<string, List<long>>>();
			foreach (KeyValuePair<string, Zone> zone in _zones)
			{
				dictionary.Add(zone.Key, zone.Value.GetConsumeTimes(lastServerTimestamp));
			}
			return dictionary;
		}
	}
}
