using System.IO;
using UnityEngine;

public class SocialShare
{
	private const string SHARE_SUBJECT = "Fail Hard: I earned {0} coins!";

	private const string SHARE_TEXT = "#FailHard Why don't you try to fail harder: {1}";

	public static void ShareScreenshot(Texture2D texture, int earnedCoins)
	{
		byte[] array = texture.EncodeToPNG();
		string temporaryCachePath = Application.temporaryCachePath;
		string text = temporaryCachePath + "/failhard_screenshot.png";
		Debug.Log(text);
		if (File.Exists(text))
		{
			File.Delete(text);
		}
		using (FileStream fileStream = File.Create(text))
		{
			fileStream.Write(array, 0, array.Length);
			fileStream.Flush();
		}
		ShareScreenshotFile(string.Format("Fail Hard: I earned {0} coins!", earnedCoins), string.Format("#FailHard Why don't you try to fail harder: {1}", earnedCoins, ExternalURL.MAGIC_URL), text);
	}

	private static void ShareScreenshotFile(string subject, string text, string screenshotPath)
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.viimagames.utilities.ContentSharer"))
		{
			androidJavaClass.CallStatic("shareScreenshot", subject, text, screenshotPath);
		}
	}

	private static byte[] RenderTexture2PNG(RenderTexture texture)
	{
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = texture;
		Texture2D texture2D = new Texture2D(texture.width, texture.height);
		texture2D.ReadPixels(new Rect(0f, 0f, texture.width, texture.height), 0, 0);
		texture2D.Apply();
		RenderTexture.active = active;
		byte[] result = texture2D.EncodeToPNG();
		Object.Destroy(texture2D);
		return result;
	}
}
