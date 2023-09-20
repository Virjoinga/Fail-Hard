using System.IO;
using UnityEngine;

public static class TextureTool
{
	private static string SCREENSHOT_FOLDER = "scr";

	public static string SaveRenderTextureToDisk(RenderTexture texture, int width, int height)
	{
		if (texture == null)
		{
			return string.Empty;
		}
		RenderTexture temporary = RenderTexture.GetTemporary(width, height);
		Graphics.Blit(texture, temporary);
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = temporary;
		Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGB24, false);
		texture2D.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
		RenderTexture.active = active;
		RenderTexture.ReleaseTemporary(temporary);
		byte[] bytes = texture2D.EncodeToPNG();
		string text = Application.persistentDataPath + "/" + SCREENSHOT_FOLDER + "/";
		string text2 = Random.Range(1, 12000) + ".png";
		int num = 20;
		int num2 = 0;
		while (File.Exists(text + text2) && num2 < num)
		{
			text2 = Random.Range(1, 12000) + ".png";
			num2++;
		}
		Directory.CreateDirectory(text);
		string text3 = text + text2;
		try
		{
			File.WriteAllBytes(text3, bytes);
		}
		catch (IOException ex)
		{
			Debug.LogWarning("Could not write texture: " + ex.Message);
			text3 = string.Empty;
		}
		return text3;
	}
}
