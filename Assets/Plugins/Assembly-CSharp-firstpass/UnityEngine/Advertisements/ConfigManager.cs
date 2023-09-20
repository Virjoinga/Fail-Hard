using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Advertisements.Event;
using UnityEngine.Advertisements.HTTPLayer;
using UnityEngine.Advertisements.MiniJSON;

namespace UnityEngine.Advertisements
{
	internal class ConfigManager
	{
		private static readonly ConfigManager _sharedInstance = new ConfigManager();

		private int[] retryDelays = new int[4] { 15, 30, 90, 240 };

		private bool _requestingConfig;

		private bool _requestingAdSources;

		public string configId { get; private set; }

		public long adSourceTtl { get; private set; }

		public long serverTimestamp { get; private set; }

		public long localTimestamp { get; private set; }

		public IntervalManager globalIntervals { get; private set; }

		public bool isInitialized { get; private set; }

		public static ConfigManager Instance
		{
			get
			{
				return _sharedInstance;
			}
		}

		private ConfigManager()
		{
		}

		public bool IsReady()
		{
			if (globalIntervals != null)
			{
				if ((long)Math.Round(Time.realtimeSinceStartup) >= localTimestamp + adSourceTtl && !_requestingAdSources)
				{
					Utils.LogDebug("Ad Source TTL expired");
					RequestAdSources();
				}
				return globalIntervals.IsAvailable();
			}
			return false;
		}

		public void RequestConfig()
		{
			if (!_requestingConfig)
			{
				_requestingConfig = true;
				string text = Settings.mediationEndpoint + "/v1/games/" + Engine.Instance.AppId + "/config";
				HTTPRequest hTTPRequest = new HTTPRequest("POST", text);
				hTTPRequest.addHeader("Content-Type", "application/json");
				if (configId != null)
				{
					hTTPRequest.addHeader("If-None-Match", configId);
				}
				hTTPRequest.setPayload(DeviceInfo.getJson());
				Utils.LogDebug("Requesting new config from " + text);
				HTTPManager.sendRequest(hTTPRequest, HandleConfigResponse, retryDelays, 1200);
			}
		}

		private void HandleConfigResponse(HTTPResponse response)
		{
			if (response.error)
			{
				if (!string.IsNullOrEmpty(response.errorMsg))
				{
					Utils.LogDebug("ConfigManager config request error: " + response.errorMsg);
				}
				else
				{
					Utils.LogDebug("ConfigManager config request error: unknown error");
				}
				return;
			}
			Dictionary<string, object> dictionary;
			try
			{
				dictionary = ParseResponse(response);
			}
			catch (Exception)
			{
				return;
			}
			if (dictionary != null)
			{
				Utils.LogDebug("Received config response");
				if (response.headers != null && response.headers.ContainsKey("ETAG"))
				{
					string text = response.headers["ETAG"];
					configId = text.Substring(3, text.Length - 4);
				}
				List<object> zones = (List<object>)dictionary["zones"];
				GatherNetworks(zones);
				ZoneManager.Instance.ResetZones(zones);
				adSourceTtl = (long)dictionary["adSourceTtl"];
				serverTimestamp = (long)dictionary["serverTimestamp"];
				RequestAdSources();
				_requestingConfig = false;
			}
		}

		private void GatherNetworks(List<object> zones)
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (object zone in zones)
			{
				List<object> list = (List<object>)((Dictionary<string, object>)zone)["adapters"];
				foreach (object item2 in list)
				{
					Dictionary<string, object> dictionary = (Dictionary<string, object>)item2;
					string text = (string)dictionary["className"];
					if (text.Equals("VideoAdAdapter"))
					{
						Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary["parameters"];
						string item = (string)dictionary2["network"];
						hashSet.Add(item);
					}
				}
			}
			UnityAds.setNetworks(hashSet);
		}

		public void RequestAdSources(List<string> zoneIds = null)
		{
			if (!_requestingAdSources)
			{
				_requestingAdSources = true;
				string url = Settings.mediationEndpoint + "/v1/games/" + Engine.Instance.AppId + "/adSources";
				url = Utils.addUrlParameters(url, new Dictionary<string, object>
				{
					{
						"zones",
						(zoneIds == null) ? null : string.Join(",", zoneIds.ToArray())
					},
					{ "config", configId }
				});
				HTTPRequest hTTPRequest = new HTTPRequest("POST", url);
				hTTPRequest.addHeader("Content-Type", "application/json");
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("lastServerTimestamp", Instance.serverTimestamp);
				dictionary.Add("adTimes", ZoneManager.Instance.GetConsumeTimes(Instance.serverTimestamp));
				string text = Json.Serialize(dictionary);
				hTTPRequest.setPayload(text);
				Utils.LogDebug("Requesting new ad sources from " + url + " with payload: " + text);
				EventManager.sendMediationAdSourcesEvent(Engine.Instance.AppId);
				HTTPManager.sendRequest(hTTPRequest, HandleAdSourcesResponse, retryDelays, 1200);
			}
		}

		private void HandleAdSourcesResponse(HTTPResponse response)
		{
			Dictionary<string, object> dictionary;
			try
			{
				dictionary = ParseResponse(response);
			}
			catch (Exception)
			{
				return;
			}
			if (dictionary != null)
			{
				Utils.LogDebug("Received ad sources response");
				globalIntervals = new IntervalManager((List<object>)dictionary["adIntervals"]);
				Utils.LogDebug(string.Concat("Got ", globalIntervals, " intervals for global"));
				serverTimestamp = (long)dictionary["serverTimestamp"];
				localTimestamp = (long)Math.Round(Time.realtimeSinceStartup);
				ZoneManager.Instance.UpdateIntervals((Dictionary<string, object>)dictionary["adSources"]);
				_requestingAdSources = false;
				isInitialized = true;
			}
		}

		private Dictionary<string, object> ParseResponse(HTTPResponse response)
		{
			if (response == null)
			{
				return null;
			}
			string @string = Encoding.UTF8.GetString(response.data, 0, response.dataLength);
			object obj = Json.Deserialize(@string);
			if (obj != null && obj is Dictionary<string, object>)
			{
				Dictionary<string, object> dictionary = (Dictionary<string, object>)obj;
				long num = (long)dictionary["status"];
				if (num != 200)
				{
					string message = (string)dictionary["error"];
					throw new Exception(message);
				}
				return (Dictionary<string, object>)dictionary["data"];
			}
			return null;
		}
	}
}
