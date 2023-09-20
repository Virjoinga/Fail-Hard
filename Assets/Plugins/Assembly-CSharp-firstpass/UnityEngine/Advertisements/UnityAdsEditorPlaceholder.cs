using System.IO;

namespace UnityEngine.Advertisements
{
	internal class UnityAdsEditorPlaceholder : MonoBehaviour
	{
		private Texture2D placeHolderLandscape;

		private Texture2D placeHolderPortrait;

		private Texture2D blackTex;

		private bool showing;

		public void init()
		{
			blackTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
			blackTex.SetPixel(0, 0, new Color(0f, 0f, 0f, 1f));
			blackTex.Apply();
			placeHolderLandscape = textureFromFile(Application.dataPath + "/Standard Assets/UnityAds/Textures/test_unit_800.png");
			placeHolderPortrait = textureFromFile(Application.dataPath + "/Standard Assets/UnityAds/Textures/test_unit_600.png");
		}

		private Texture2D textureFromFile(string filePath)
		{
			return textureFromBytes(textureBytesForFrame(filePath));
		}

		private Texture2D textureFromBytes(byte[] bytes)
		{
			Texture2D texture2D = new Texture2D(1, 1);
			texture2D.LoadImage(bytes);
			return texture2D;
		}

		private byte[] textureBytesForFrame(string imageURL)
		{
			byte[] array = null;
			using (FileStream fileStream = File.OpenRead(imageURL))
			{
				int num = (int)fileStream.Length;
				array = new byte[num];
				int num2 = 0;
				int num3;
				while ((num3 = fileStream.Read(array, num2, num - num2)) > 0)
				{
					num2 += num3;
				}
				return array;
			}
		}

		private void windowFunc(int id)
		{
			GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), blackTex);
			if (Screen.width > Screen.height)
			{
				float num = (float)placeHolderLandscape.width / (float)placeHolderLandscape.height;
				float num2 = Screen.width;
				float num3 = num2 / num;
				Rect position = new Rect(0f, ((float)Screen.height - num3) / 2f, num2, num3);
				GUI.DrawTexture(position, placeHolderLandscape);
			}
			else
			{
				float num4 = (float)placeHolderPortrait.width / (float)placeHolderPortrait.height;
				float num5 = Screen.width;
				float num6 = num5 / num4;
				Rect position2 = new Rect(0f, ((float)Screen.height - num6) / 2f, num5, num6);
				GUI.DrawTexture(position2, placeHolderPortrait);
			}
			if (GUI.Button(new Rect(Screen.width - 150, 0f, 150f, 50f), "Close"))
			{
				Hide();
			}
		}

		public void OnGUI()
		{
			if (showing)
			{
				Color color = GUI.color;
				GUI.color = new Color(1f, 1f, 1f, 0f);
				GUI.ModalWindow(0, new Rect(0f, 0f, Screen.width, Screen.height), windowFunc, string.Empty);
				GUI.color = color;
			}
		}

		public void Show()
		{
			showing = true;
		}

		public void Hide()
		{
			UnityAds.SharedInstance.onVideoCompleted("(null);false");
			UnityAds.SharedInstance.onHide();
			showing = false;
		}
	}
}
