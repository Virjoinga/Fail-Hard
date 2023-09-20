using System;
using System.Text;
using UnityEngine.Advertisements.Event;

namespace UnityEngine.Advertisements.HTTPLayer
{
	internal class RetryRequest
	{
		protected int retryPosition;

		protected int[] retryDelayTable;

		protected HTTPRequest request;

		protected Action<HTTPResponse> callback;

		protected bool keepRetrying;

		protected bool callbackDelivered;

		protected bool useDeadline;

		protected float retryDeadline;

		protected int deadlineDelay;

		public RetryRequest(int[] delays, int maxDelay, HTTPRequest req)
		{
			retryPosition = 0;
			retryDelayTable = delays;
			if (maxDelay > 0)
			{
				deadlineDelay = maxDelay;
				useDeadline = true;
			}
			request = req;
		}

		public void execute(Action<HTTPResponse> eventCallback)
		{
			callback = eventCallback;
			keepRetrying = true;
			callbackDelivered = false;
			if (useDeadline)
			{
				retryDeadline = Time.realtimeSinceStartup + (float)deadlineDelay;
			}
			retry();
			if (useDeadline)
			{
				AsyncExec.runWithDelay(deadlineDelay, executeDeadline);
			}
		}

		protected virtual void HTTPCallback(HTTPResponse res)
		{
			if (res.error)
			{
				if (!keepRetrying && !callbackDelivered)
				{
					failedCallback("Network error");
				}
				return;
			}
			EventJSON eventJSON = new EventJSON(Encoding.UTF8.GetString(res.data, 0, res.data.Length));
			if (eventJSON.hasInt("status") && eventJSON.getInt("status") == 200)
			{
				keepRetrying = false;
				if (!callbackDelivered)
				{
					callbackDelivered = true;
					callback(res);
				}
			}
			else if (eventJSON.hasBool("retryable") && !eventJSON.getBool("retryable"))
			{
				keepRetrying = false;
				if (!callbackDelivered)
				{
					failedCallback("Retrying forbidden by remote server");
				}
			}
			else if (!keepRetrying && !callbackDelivered)
			{
				failedCallback("Error");
			}
		}

		protected void retry()
		{
			if (!keepRetrying)
			{
				return;
			}
			HTTPRequest clone = request.getClone();
			clone.execute(HTTPCallback);
			if (retryPosition < retryDelayTable.Length && (!useDeadline || Time.realtimeSinceStartup < retryDeadline))
			{
				int num = retryDelayTable[retryPosition++];
				if (num > 0)
				{
					AsyncExec.runWithDelay(num, retry);
				}
				else
				{
					keepRetrying = false;
				}
			}
			else
			{
				keepRetrying = false;
			}
		}

		protected void executeDeadline()
		{
			keepRetrying = false;
			if (!callbackDelivered)
			{
				failedCallback("Retry deadline exceeded");
			}
		}

		protected void failedCallback(string msg)
		{
			callbackDelivered = true;
			HTTPResponse hTTPResponse = new HTTPResponse();
			hTTPResponse.url = request.url;
			hTTPResponse.error = true;
			hTTPResponse.errorMsg = msg;
			callback(hTTPResponse);
		}
	}
}
