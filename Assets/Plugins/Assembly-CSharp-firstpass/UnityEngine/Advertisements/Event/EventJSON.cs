using System.Collections.Generic;
using UnityEngine.Advertisements.MiniJSON;

namespace UnityEngine.Advertisements.Event
{
	internal class EventJSON
	{
		private Dictionary<string, object> data;

		public EventJSON(string json)
		{
			object obj = Json.Deserialize(json);
			if (obj is Dictionary<string, object>)
			{
				data = (Dictionary<string, object>)obj;
			}
		}

		public bool hasString(string key)
		{
			if (data == null)
			{
				return false;
			}
			if (data.ContainsKey(key))
			{
				return true;
			}
			return false;
		}

		public string getString(string key)
		{
			return (string)data[key];
		}

		public bool hasInt(string key)
		{
			if (data == null)
			{
				return false;
			}
			if (data.ContainsKey(key))
			{
				return true;
			}
			return false;
		}

		public int getInt(string key)
		{
			long num = (long)data[key];
			return (int)num;
		}

		public bool hasBool(string key)
		{
			if (data == null)
			{
				return false;
			}
			if (data.ContainsKey(key))
			{
				return true;
			}
			return false;
		}

		public bool getBool(string key)
		{
			return (bool)data[key];
		}
	}
}
