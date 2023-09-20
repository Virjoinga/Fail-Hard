using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace UnityEngine.Advertisements.HTTPLayer
{
	internal class HTTPRequest
	{
		private enum HTTPMethod
		{
			GET = 0,
			POST = 1
		}

		public string url;

		private bool networkLogging;

		private HTTPMethod method;

		private byte[] postData;

		private Dictionary<string, string> headers;

		public HTTPRequest(string newUrl)
		{
			url = newUrl;
			method = HTTPMethod.GET;
		}

		public HTTPRequest(string newMethod, string newUrl)
		{
			url = newUrl;
			method = HTTPMethod.GET;
			if (newMethod.Equals("POST"))
			{
				method = HTTPMethod.POST;
				headers = new Dictionary<string, string>();
			}
		}

		public HTTPRequest getClone()
		{
			HTTPRequest hTTPRequest = ((method != 0) ? new HTTPRequest("POST", url) : new HTTPRequest(url));
			if (postData != null && postData.Length > 0)
			{
				hTTPRequest.setPayload(postData);
			}
			if (headers != null && headers.Count > 0)
			{
				foreach (KeyValuePair<string, string> header in headers)
				{
					hTTPRequest.addHeader(header.Key, header.Value);
				}
			}
			return hTTPRequest;
		}

		public void setPayload(string payload)
		{
			postData = Encoding.UTF8.GetBytes(payload);
		}

		public void setPayload(byte[] payload)
		{
			postData = payload;
		}

		public void addHeader(string header, string value)
		{
			headers.Add(header, value);
		}

		public void execute(Action<HTTPResponse> callback)
		{
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				HTTPResponse hTTPResponse = new HTTPResponse();
				hTTPResponse.url = url;
				hTTPResponse.error = true;
				hTTPResponse.errorMsg = "Internet not reachable";
				callback(hTTPResponse);
			}
			else if (method == HTTPMethod.POST)
			{
				if (postData == null)
				{
					postData = new byte[0];
				}
				AsyncExec.runWithCallback(executePost, this, callback);
			}
			else
			{
				AsyncExec.runWithCallback(executeGet, this, callback);
			}
		}

		private IEnumerator executeGet(HTTPRequest req, Action<HTTPResponse> callback)
		{
			if (networkLogging)
			{
				printUp("GET", req.url);
			}
			WWW www = new WWW(req.url);
			yield return www;
			HTTPResponse response = processWWW(www);
			response.url = req.url;
			if (networkLogging)
			{
				printDown(req.url, www.bytes);
			}
			callback(response);
		}

		private IEnumerator executePost(HTTPRequest req, Action<HTTPResponse> callback)
		{
			WWW www2 = null;
			if (networkLogging)
			{
				printUp("POST", req.url);
			}
			Type wwwType = typeof(WWW);
			ConstructorInfo wwwConstructor = wwwType.GetConstructor(new Type[3]
			{
				typeof(string),
				typeof(byte[]),
				typeof(Dictionary<string, string>)
			});
			ConstructorInfo wwwConstructorOld = wwwType.GetConstructor(new Type[3]
			{
				typeof(string),
				typeof(byte[]),
				typeof(Hashtable)
			});
			if (wwwConstructor == null && wwwConstructorOld != null)
			{
				Hashtable tempHeaders = new Hashtable(req.headers);
				www2 = (WWW)wwwConstructorOld.Invoke(new object[3] { req.url, req.postData, tempHeaders });
			}
			else
			{
				www2 = (WWW)wwwConstructor.Invoke(new object[3] { req.url, req.postData, req.headers });
			}
			yield return www2;
			HTTPResponse response = processWWW(www2);
			response.url = req.url;
			if (networkLogging)
			{
				printDown(req.url, www2.bytes);
			}
			callback(response);
		}

		private HTTPResponse processWWW(WWW www)
		{
			HTTPResponse hTTPResponse = new HTTPResponse();
			if (!string.IsNullOrEmpty(www.error))
			{
				hTTPResponse.error = true;
				hTTPResponse.errorMsg = www.error;
			}
			else
			{
				hTTPResponse.error = false;
				hTTPResponse.errorMsg = null;
				hTTPResponse.data = www.bytes;
				hTTPResponse.dataLength = www.bytes.Length;
				hTTPResponse.headers = new Dictionary<string, string>();
				hTTPResponse.etag = string.Empty;
				if (www.responseHeaders != null)
				{
					foreach (KeyValuePair<string, string> responseHeader in www.responseHeaders)
					{
						hTTPResponse.headers.Add(responseHeader.Key.ToUpper(), responseHeader.Value);
					}
					if (hTTPResponse.headers.ContainsKey("ETAG"))
					{
						hTTPResponse.etag = hTTPResponse.headers["ETAG"];
					}
				}
			}
			return hTTPResponse;
		}

		private void printUp(string method, string url)
		{
			Debug.Log(Time.realtimeSinceStartup + " -> " + method + " [" + url + "]");
		}

		private void printDown(string url, byte[] data)
		{
			if (data.Length < 16384)
			{
				Debug.Log(Time.realtimeSinceStartup + " <- [" + url + "]: " + Encoding.UTF8.GetString(data, 0, data.Length));
			}
			else
			{
				Debug.Log(Time.realtimeSinceStartup + " <- [" + url + "]: " + data.Length + " bytes");
			}
		}
	}
}
