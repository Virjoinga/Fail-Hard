namespace UnityEngine.Advertisements.HTTPLayer
{
	internal class RetryFileRequest : RetryRequest
	{
		public RetryFileRequest(int[] delays, int maxDelay, HTTPRequest req)
			: base(delays, maxDelay, req)
		{
		}

		protected override void HTTPCallback(HTTPResponse res)
		{
			if (res.error)
			{
				if (!keepRetrying && !callbackDelivered)
				{
					failedCallback("Network error");
				}
			}
			else if (res.dataLength != 0)
			{
				keepRetrying = false;
				if (!callbackDelivered)
				{
					callbackDelivered = true;
					callback(res);
				}
			}
			else if (!keepRetrying && !callbackDelivered)
			{
				failedCallback("Error");
			}
		}
	}
}
