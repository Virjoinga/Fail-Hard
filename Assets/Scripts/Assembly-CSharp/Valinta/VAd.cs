using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Valinta
{
	public class VAd
	{
		public delegate void AdDelegate(string s);

		public string AdSystem;

		public string AdSystemVersion;

		public string AdTitle;

		public bool Skippable;

		public Dictionary<int, string> Impressions = new Dictionary<int, string>();

		public List<VWrapper> Wrappers = new List<VWrapper>();

		public List<VCreative> Creatives = new List<VCreative>();

		public Dictionary<string, string> Extensions = new Dictionary<string, string>();

		private List<string> ConsumedEvents = new List<string>();

		public float StartTime { get; set; }

		public float TimeTick { get; set; }

		public float TimeLeft { get; set; }

		public float Duration { get; set; }

		public float CurrentTimePosition { get; set; }

		public float SkipTime { get; private set; }

		public float TotalDuration { get; private set; }

		public float FirstQuartile { get; private set; }

		public float Midpoint { get; private set; }

		public float ThirdQuartile { get; private set; }

		public float Complete { get; private set; }

		public string LinkText { get; private set; }

		[method: MethodImpl(32)]
		public event AdDelegate OnSuccess;

		[method: MethodImpl(32)]
		public event AdDelegate OnError;

		public void UpdateEventTimes()
		{
			if (!SetDuration())
			{
				Error("SetDuration", "Could not set duration");
			}
			if (!SetSkipTime())
			{
				Error("SetSkipTime", "Ad not skippable");
			}
			Duration = TotalDuration;
			TimeLeft = TotalDuration;
			FirstQuartile = TotalDuration * 0.25f;
			Midpoint = TotalDuration * 0.5f;
			ThirdQuartile = TotalDuration * 0.75f;
		}

		private bool SetDuration()
		{
			TotalDuration = ParseTimeFromString(Creatives[0].Duration);
			if (TotalDuration > 0f)
			{
				return true;
			}
			return false;
		}

		private bool SetSkipTime()
		{
			bool flag = false;
			string text = string.Empty;
			foreach (string key in Extensions.Keys)
			{
				if (key.Contains("skipTime") && !text.Equals(Extensions[key]))
				{
					flag = true;
					text = Extensions[key];
				}
			}
			if (!Creatives[0].Skippable && !flag)
			{
				return false;
			}
			SkipTime = ParseTimeFromString(string.IsNullOrEmpty(Creatives[0].SkipOffset) ? text : Creatives[0].SkipOffset);
			Skippable = true;
			if (SkipTime > 0f)
			{
				return true;
			}
			return false;
		}

		private float ParseTimeFromString(string stringToParse)
		{
			int result = 0;
			float result2 = 0f;
			float result3 = 0f;
			float result4 = 0f;
			bool flag = false;
			if (stringToParse.Contains("%"))
			{
				string s = stringToParse.Replace("%", string.Empty);
				flag = int.TryParse(s, out result);
				return (TotalDuration == 0f) ? 0f : (TotalDuration * (float)(result / 100));
			}
			bool flag2 = false;
			string[] array = stringToParse.Split(':');
			if (array.Length > 2)
			{
				flag2 = true;
				flag = float.TryParse(array[0], out result2);
				flag = float.TryParse(array[1], out result3);
			}
			return (!float.TryParse(array[flag2 ? 2 : 0], out result4)) ? 0f : (result2 * 3600f + result3 * 60f + result4);
		}

		public string GetMediaUrl()
		{
			foreach (KeyValuePair<string, string> mediaFile in Creatives[0].MediaFiles)
			{
				if (mediaFile.Key.Equals("audio/mp3") || mediaFile.Key.Equals("audio/mpeg"))
				{
					return mediaFile.Value;
				}
			}
			return string.Empty;
		}

		public List<string> GetImpressionUrls()
		{
			List<string> list = new List<string>();
			foreach (VWrapper wrapper in Wrappers)
			{
				foreach (KeyValuePair<int, string> impression in wrapper.Impressions)
				{
					list.Add(impression.Value);
				}
			}
			foreach (KeyValuePair<int, string> impression2 in Impressions)
			{
				list.Add(impression2.Value);
			}
			return list;
		}

		public void DoEventTracking(string eventToTrack)
		{
			ConsumedEvents.Add(eventToTrack);
			List<string> list = new List<string>();
			for (int i = 0; i < Wrappers.Count; i++)
			{
				foreach (VCreative creative in Wrappers[i].Creatives)
				{
					if (creative.TrackingEvents.ContainsKey(eventToTrack))
					{
						List<string> value = new List<string>();
						if (creative.TrackingEvents.TryGetValue(eventToTrack, out value))
						{
							list.AddRange(value);
						}
					}
				}
			}
			foreach (VCreative creative2 in Creatives)
			{
				if (creative2.TrackingEvents.ContainsKey(eventToTrack))
				{
					List<string> value2 = new List<string>();
					if (creative2.TrackingEvents.TryGetValue(eventToTrack, out value2))
					{
						list.AddRange(value2);
					}
				}
			}
			if (list.Count > 0)
			{
				Success(eventToTrack, list);
			}
			else
			{
				Error(eventToTrack, "No such event");
			}
		}

		public bool IsEventFired(string eventName)
		{
			if (ConsumedEvents.Contains(eventName))
			{
				return true;
			}
			return false;
		}

		public void DoClickTracking()
		{
			Debug.Log("Do click tracking");
			foreach (VWrapper wrapper in Wrappers)
			{
				ParseClickTrackingUrls(wrapper.Creatives);
			}
			ParseClickTrackingUrls(Creatives);
		}

		private void ParseClickTrackingUrls(List<VCreative> list)
		{
			List<string> list2 = new List<string>();
			foreach (VCreative item in list)
			{
				bool flag = false;
				string value = string.Empty;
				if (item.VideoClicks.ContainsKey(AdEnums.ClickTracking) && item.VideoClicks.TryGetValue(AdEnums.ClickTracking, out value))
				{
					list2.Add(value);
				}
				if (item.VideoClicks.ContainsKey(AdEnums.CustomClick) && item.VideoClicks.TryGetValue(AdEnums.CustomClick, out value))
				{
					list2.Add(value);
				}
			}
			if (list2.Count > 0)
			{
				Success("Clicks", list2);
			}
		}

		public string GetClickUrl()
		{
			DoClickTracking();
			string value = string.Empty;
			bool flag = false;
			foreach (VCreative creative in Creatives)
			{
				if (creative.VideoClicks.ContainsKey(AdEnums.ClickThrough))
				{
					if (creative.VideoClicks.TryGetValue(AdEnums.ClickThrough, out value))
					{
						return value;
					}
					break;
				}
			}
			foreach (VWrapper wrapper in Wrappers)
			{
				foreach (VCreative creative2 in wrapper.Creatives)
				{
					if (creative2.VideoClicks.ContainsKey(AdEnums.ClickThrough))
					{
						if (creative2.VideoClicks.TryGetValue(AdEnums.ClickThrough, out value))
						{
							return value;
						}
						break;
					}
				}
			}
			return value;
		}

		public void SetLinkText()
		{
			if (Extensions.ContainsKey("linkTxt") && Extensions["linkTxt"].Length > 3)
			{
				LinkText = Extensions["linkTxt"];
			}
			else
			{
				LinkText = "Advertisement";
			}
		}

		private void Error(string ev, string desc)
		{
			if (this.OnError != null)
			{
				this.OnError(ev + " -> " + desc);
			}
		}

		private void Success(string ev, List<string> urls)
		{
			foreach (string url in urls)
			{
				if (this.OnSuccess != null)
				{
					this.OnSuccess(url);
				}
			}
		}
	}
}
