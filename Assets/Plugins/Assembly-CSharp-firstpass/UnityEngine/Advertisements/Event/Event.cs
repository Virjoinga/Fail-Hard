using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.Advertisements.HTTPLayer;

namespace UnityEngine.Advertisements.Event
{
	internal class Event
	{
		private static string reqIdBase;

		private static int reqIndex;

		private static DateTime unixEpoch;

		private string url;

		private int[] retryDelayTable;

		private string jsonData;

		private Action<bool> callback;

		private int deadlineDelay = 1200;

		public Event(string eventUrl, int[] delays, bool useReqId, string eventJson, string infoJson)
		{
			url = eventUrl;
			retryDelayTable = delays;
			prepareJsonData(useReqId, eventJson, infoJson);
		}

		private void prepareJsonData(bool useReqId, string eventJson, string infoJson)
		{
			StringBuilder stringBuilder = new StringBuilder("{ ");
			if (useReqId)
			{
				stringBuilder.Append("\"req_id\": \"");
				stringBuilder.Append(reqIdBase);
				stringBuilder.Append("-");
				stringBuilder.Append((int)(DateTime.UtcNow - unixEpoch).TotalMilliseconds);
				stringBuilder.Append("-");
				stringBuilder.Append(reqIndex++);
				stringBuilder.Append("\", ");
			}
			stringBuilder.Append("\"event\": ");
			stringBuilder.Append(eventJson);
			stringBuilder.Append(", \"info\": ");
			stringBuilder.Append(infoJson);
			stringBuilder.Append(" }");
			jsonData = stringBuilder.ToString();
		}

		public static void init()
		{
			reqIndex = 1;
			unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			string text = DeviceInfo.bundleID();
			string text2 = DeviceInfo.deviceID();
			string s;
			if (text.Length > 0 && text2.Length > 0)
			{
				reqIdBase = "a-";
				s = text + "-" + text2;
			}
			else
			{
				System.Random random = new System.Random();
				reqIdBase = "b-";
				s = (int)(DateTime.UtcNow - unixEpoch).TotalMilliseconds + "-" + random.Next();
			}
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			byte[] array = mD5CryptoServiceProvider.ComputeHash(bytes);
			string text3 = BitConverter.ToString(array).Replace("-", string.Empty);
			reqIdBase = reqIdBase + text3 + "-";
		}

		public void execute(Action<bool> eventCallback)
		{
			callback = eventCallback;
			HTTPRequest hTTPRequest = new HTTPRequest("POST", url);
			hTTPRequest.addHeader("Content-Type", "application/json");
			hTTPRequest.setPayload(jsonData);
			HTTPManager.sendRequest(hTTPRequest, HTTPCallback, retryDelayTable, deadlineDelay);
		}

		private void HTTPCallback(HTTPResponse res)
		{
			if (res.error)
			{
				callback(false);
			}
			else
			{
				callback(true);
			}
		}
	}
}
