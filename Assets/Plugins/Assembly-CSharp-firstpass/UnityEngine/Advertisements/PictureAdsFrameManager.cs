using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine.Advertisements.Event;
using UnityEngine.Advertisements.HTTPLayer;
using UnityEngine.Advertisements.MiniJSON;

namespace UnityEngine.Advertisements
{
	internal class PictureAdsFrameManager : MonoBehaviour
	{
		private Dictionary<ImageOrientation, Dictionary<ImageType, Texture2D>> textures;

		private Dictionary<ImageOrientation, Dictionary<ImageType, Rect>> texturesRects;

		private bool adIsShowing;

		private bool _isClosed = true;

		private ImageOrientation screenOrientation;

		private Texture2D blackTex;

		private float offset;

		private float increase;

		private float currentTime;

		private float previousTime;

		private float animationDuration = 0.5f;

		public PictureAdsManager manager;

		private PictureAd _ad;

		public PictureAdsFrameManager()
		{
			textures = new Dictionary<ImageOrientation, Dictionary<ImageType, Texture2D>>();
			texturesRects = new Dictionary<ImageOrientation, Dictionary<ImageType, Rect>>();
		}

		public bool adIsClosed()
		{
			return _isClosed;
		}

		public void initAd(PictureAd ad)
		{
			_ad = ad;
			updateRects(ad);
			updateTextures(ad);
			_isClosed = false;
		}

		public bool isShowingAd()
		{
			return adIsShowing;
		}

		public void showAd()
		{
			currentTime = (previousTime = Time.realtimeSinceStartup);
			adIsShowing = true;
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

		private Rect rectWithPrecentage(int precentageSize, ImageOrientation imageOrientation)
		{
			bool flag = imageOrientation == ImageOrientation.Landscape;
			float num = ((Screen.width <= Screen.height) ? 1.7777778f : 0.5625f);
			float num2 = 1f / num;
			int num3 = ((Screen.width <= Screen.height) ? Screen.height : Screen.width) * precentageSize / 100;
			float num4 = (float)num3 * ((Screen.width <= Screen.height) ? num2 : num);
			num4 = ((!(num4 > (float)(((Screen.width <= Screen.height) ? Screen.width : Screen.height) * precentageSize / 100))) ? num4 : ((float)(((Screen.width <= Screen.height) ? Screen.width : Screen.height) * precentageSize / 100)));
			Rect rect = new Rect((((Screen.width <= Screen.height) ? Screen.height : Screen.width) - num3) / 2, ((float)((Screen.width <= Screen.height) ? Screen.width : Screen.height) - num4) / 2f, num3, num4);
			Rect rect2 = new Rect(((float)((Screen.width <= Screen.height) ? Screen.width : Screen.height) - num4) / 2f, (((Screen.width <= Screen.height) ? Screen.height : Screen.width) - num3) / 2, num4, num3);
			return (!flag) ? rect2 : rect;
		}

		private void showPictureAd(int windowID)
		{
			getScreenOrientation();
			Color color = GUI.color;
			GUI.color = new Color(1f, 1f, 1f, offset);
			GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), blackTex);
			GUI.DrawTexture(texturesRects[screenOrientation][ImageType.Frame], textures[screenOrientation][ImageType.Frame]);
			GUI.DrawTexture(texturesRects[screenOrientation][ImageType.Base], textures[screenOrientation][ImageType.Base]);
			GUI.DrawTexture(texturesRects[screenOrientation][ImageType.Close], textures[screenOrientation][ImageType.Close]);
			if (Advertisement.UnityDeveloperInternalTestMode)
			{
				int num = Screen.width / 10 * 2;
				int num2 = Screen.height / 10 * 2;
				Texture2D texture2D = new Texture2D(num, num2);
				for (int i = 0; i < num; i++)
				{
					for (int j = 0; j < num2; j++)
					{
						texture2D.SetPixel(i, j, Color.black);
					}
				}
				texture2D.Apply();
				GUIStyle gUIStyle = new GUIStyle();
				gUIStyle.normal.textColor = Color.red;
				gUIStyle.normal.background = texture2D;
				gUIStyle.alignment = TextAnchor.MiddleCenter;
				GUI.Label(new Rect(Screen.width / 10 * 4, Screen.height / 10 * 4, Screen.width / 10 * 2, Screen.height / 10 * 2), "INTERNAL UNITY TEST BUILD\nDO NOT USE IN PRODUCTION", gUIStyle);
			}
			GUI.color = color;
		}

		private void OnGUI()
		{
			if (adIsShowing)
			{
				Color color = GUI.color;
				GUI.color = new Color(1f, 1f, 1f, 0f);
				GUI.ModalWindow(0, new Rect(0f, 0f, Screen.width, Screen.height), showPictureAd, string.Empty);
				GUI.color = color;
			}
		}

		private bool mouseInRect(Rect rect)
		{
			return Input.mousePosition.x >= rect.x && Input.mousePosition.x <= rect.x + rect.width && (float)Screen.height - Input.mousePosition.y >= rect.y && (float)Screen.height - Input.mousePosition.y <= rect.y + rect.height;
		}

		private void showAnimation()
		{
			if (offset + increase < 1f)
			{
				offset += increase;
			}
			else
			{
				offset = 1f;
			}
		}

		private void hideAnimation()
		{
			if (offset - increase > 0f)
			{
				offset -= increase;
				return;
			}
			adIsShowing = false;
			textures.Clear();
			texturesRects.Clear();
			manager.pictureAdDidClosed();
			offset = 0f;
		}

		private void Update()
		{
			if (!adIsShowing)
			{
				return;
			}
			float num = currentTime - previousTime;
			increase = num / animationDuration;
			if (!_isClosed)
			{
				showAnimation();
			}
			else
			{
				hideAnimation();
			}
			if (Input.GetMouseButtonDown(0) && offset == 1f)
			{
				if (mouseInRect(texturesRects[screenOrientation][ImageType.CloseActiveArea]))
				{
					EventManager.sendCloseEvent(Engine.Instance.AppId, _ad.id, false);
					_isClosed = true;
					manager.pictureAdWillBeClosed();
					return;
				}
				if (mouseInRect(texturesRects[screenOrientation][ImageType.Base]))
				{
					EventManager.sendClickEvent(Engine.Instance.AppId, _ad.id);
					manager.pictureAdClicked();
					HTTPRequest hTTPRequest = new HTTPRequest("POST", _ad.clickActionUrl);
					hTTPRequest.addHeader("Content-Type", "application/json");
					hTTPRequest.setPayload(DeviceInfo.adRequestJSONPayload(manager.network));
					hTTPRequest.execute(delegate(HTTPResponse response)
					{
						if (response != null && response.data != null)
						{
							string @string = Encoding.UTF8.GetString(response.data, 0, response.dataLength);
							object obj = Json.Deserialize(@string);
							if (obj != null && obj is Dictionary<string, object>)
							{
								Dictionary<string, object> dictionary = (Dictionary<string, object>)obj;
								string text = (string)dictionary["clickUrl"];
								if (text != null)
								{
									Application.OpenURL(text);
								}
							}
						}
					});
					return;
				}
			}
			previousTime = currentTime;
			currentTime = Time.realtimeSinceStartup;
		}

		private void initRect(ImageOrientation imageOrientation, ImageType imageType, Rect rect)
		{
			texturesRects[imageOrientation][imageType] = rect;
		}

		private void initTexture(ImageOrientation imageOrientation, ImageType imageType, PictureAd ad)
		{
			textures[imageOrientation][imageType] = textureFromBytes(textureBytesForFrame(ad.getLocalImageURL(imageOrientation, imageType)));
		}

		private void initRectsForOrientation(PictureAd ad, ImageOrientation imageOrientation)
		{
			Rect rect = rectWithPrecentage(ad.getImageSpace(ImageType.Frame), imageOrientation);
			Rect rect2 = rectWithPrecentage(ad.getImageSpace(ImageType.Base), imageOrientation);
			Rect rect3 = rectWithPrecentage(ad.getImageSpace(ImageType.Close), imageOrientation);
			float num = Math.Max(rect3.width, rect3.height);
			rect3 = new Rect(rect2.x + rect2.width - rect3.width / 2f, rect2.y - rect3.height / 2f, num, num);
			Rect rect4 = new Rect((float)((double)rect3.x - (double)rect3.width * 0.3), (float)((double)rect3.y - (double)rect3.height * 0.3), (float)((double)rect3.width + (double)rect3.width * 0.3), (float)((double)rect3.height + (double)rect3.height * 0.3));
			initRect(imageOrientation, ImageType.Base, rect2);
			initRect(imageOrientation, ImageType.Frame, rect);
			initRect(imageOrientation, ImageType.Close, rect3);
			initRect(imageOrientation, ImageType.CloseActiveArea, rect4);
		}

		private void initTextureForOrientation(PictureAd ad, ImageOrientation imageOrientation)
		{
			blackTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
			blackTex.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.7f));
			blackTex.Apply();
			initTexture(imageOrientation, ImageType.Base, ad);
			initTexture(imageOrientation, ImageType.Frame, ad);
			initTexture(imageOrientation, ImageType.Close, ad);
		}

		private void updateRects(PictureAd ad)
		{
			texturesRects.Clear();
			texturesRects[ImageOrientation.Landscape] = new Dictionary<ImageType, Rect>();
			texturesRects[ImageOrientation.Portrait] = new Dictionary<ImageType, Rect>();
			initRectsForOrientation(ad, ImageOrientation.Landscape);
			initRectsForOrientation(ad, ImageOrientation.Portrait);
			getScreenOrientation();
		}

		private ImageOrientation getScreenOrientation()
		{
			return screenOrientation = ((Screen.width <= Screen.height) ? ImageOrientation.Portrait : ImageOrientation.Landscape);
		}

		private void updateTextures(PictureAd ad)
		{
			textures.Clear();
			textures[ImageOrientation.Landscape] = new Dictionary<ImageType, Texture2D>();
			textures[ImageOrientation.Portrait] = new Dictionary<ImageType, Texture2D>();
			initTextureForOrientation(ad, ImageOrientation.Landscape);
			initTextureForOrientation(ad, ImageOrientation.Portrait);
		}
	}
}
