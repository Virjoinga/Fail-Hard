using System;

namespace UnityEngine.Advertisements.HTTPLayer
{
	internal class HTTPManager
	{
		public static void sendFileRequest(HTTPRequest request, Action<HTTPResponse> callback, int[] delays, int maxDelay)
		{
			RetryFileRequest retryFileRequest = new RetryFileRequest(delays, maxDelay, request);
			retryFileRequest.execute(callback);
		}

		public static void sendRequest(HTTPRequest request, Action<HTTPResponse> callback, int[] delays, int maxDelay)
		{
			RetryRequest retryRequest = new RetryRequest(delays, maxDelay, request);
			retryRequest.execute(callback);
		}

		public static void sendRequest(HTTPRequest request, Action<HTTPResponse> callback, int retries, int delay, int maxDelay)
		{
			int[] array = new int[retries];
			for (int i = 0; i < retries; i++)
			{
				array[i] = delay;
			}
			sendRequest(request, callback, array, maxDelay);
		}
	}
}
