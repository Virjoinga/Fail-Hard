using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Valinta
{
	public class VAds : MonoBehaviour
	{
		public delegate void AdReady(VAd receivedAd);

		public delegate void AdError(string error);

		public static VAds Instance;

		private List<VAd> ValintaAds;

		private List<VWrapper> Wrappers;

		private int m_wrapperCount;

		private Dictionary<string, string> Headers;

		private string UserAgent;

		private bool m_isPrimaryFetched;

		private bool m_isSecondaryFetched;

		public string PrimaryAdURL { get; set; }

		public string SecondaryAdURL { get; set; }

		[method: MethodImpl(32)]
		public event AdReady OnAdReady;

		[method: MethodImpl(32)]
		public event AdError OnAdError;

		private void Awake()
		{
			Instance = this;
			ValintaAds = new List<VAd>();
			Wrappers = new List<VWrapper>();
			Headers = new Dictionary<string, string>();
			GetDeviceInfo();
		}

		private void GetDeviceInfo()
		{
			string text = SystemInfo.operatingSystem + "; ";
			string deviceModel = SystemInfo.deviceModel;
			string value = "Mozilla/5.0 (Linux; Android 4.0.4; Galaxy Nexus Build/IMM76B) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.133 Mobile Safari/535.19";
			UserAgent = "Mozilla/5.0 (Linux; Android 4.0.4; Galaxy Nexus Build/IMM76B) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.133 Mobile Safari/535.19";
			Headers.Add("User-Agent", value);
		}

		public void RequestAd()
		{
			Debug.Log("Request ad");
			string url = string.Empty;
			if (!m_isPrimaryFetched && !string.IsNullOrEmpty(PrimaryAdURL))
			{
				Debug.Log("Primary");
				url = PrimaryAdURL;
				m_isPrimaryFetched = true;
			}
			else if (!m_isSecondaryFetched && !string.IsNullOrEmpty(SecondaryAdURL))
			{
				Debug.Log("Secondary");
				url = SecondaryAdURL;
				m_isSecondaryFetched = true;
			}
			StartCoroutine(GetVast(url));
		}

		private void CreateAdURL()
		{
			string url = "http://ad.instreamatic.com/web/794.xml?preview=12";
			StartCoroutine(GetVast(url));
		}

		private IEnumerator GetVast(string url)
		{
			if (string.IsNullOrEmpty(url))
			{
				Error("Could not fetch ads");
				yield break;
			}
			WWW www = new WWW(url, null, Headers);
			yield return www;
			if (string.IsNullOrEmpty(www.error))
			{
				if (Wrappers.Count > 5)
				{
					StartCoroutine(ReportError());
					Error("Possible infinite loop in ad wrappers");
					yield break;
				}
				if (www.text.Contains("VAST"))
				{
					string t = www.text.Replace('\'', '"');
					XmlParser parser = new XmlParser(t);
					StartCoroutine(ParseAds(parser.GetVAST()));
					yield break;
				}
				if (Wrappers != null && Wrappers.Count > 0)
				{
					StartCoroutine(ReportError());
				}
				if (!m_isPrimaryFetched || !m_isSecondaryFetched)
				{
					RequestAd();
					yield break;
				}
				m_isPrimaryFetched = false;
				m_isSecondaryFetched = false;
				Error("Not VAST response");
			}
			else if (!m_isPrimaryFetched || !m_isSecondaryFetched)
			{
				RequestAd();
			}
			else
			{
				m_isPrimaryFetched = false;
				m_isSecondaryFetched = false;
				Error("Could not resolve host");
			}
		}

		private IEnumerator ReportError()
		{
			yield return new WWW(Wrappers[0].ErrorURI, null, Headers);
		}

		private IEnumerator ParseAds(VAST xml)
		{
			if (xml == null)
			{
				Error("Couldn't parse VAST");
				yield break;
			}
			if (xml.Ads == null)
			{
				Error("No ads in VAST or XML parsing failed!");
				yield break;
			}
			Ad[] ads = xml.Ads;
			foreach (Ad a in ads)
			{
				if (a.InLines != null)
				{
					InLine[] inLines = a.InLines;
					foreach (InLine inline in inLines)
					{
						VAd ad = ParseInline(inline);
						if (Wrappers.Count > 0)
						{
							ad.Wrappers.AddRange(Wrappers);
						}
						ValintaAds.Add(ad);
						Wrappers.Clear();
						yield return null;
					}
				}
				if (a.Wrappers != null)
				{
					Wrapper[] wrappers = a.Wrappers;
					int num = 0;
					if (num < wrappers.Length)
					{
						Wrapper wrapper = wrappers[num];
						ExtractWrapper(wrapper);
						yield break;
					}
				}
			}
			Success();
		}

		private VAd ParseInline(InLine inline)
		{
			VAd vAd = new VAd();
			vAd.AdSystem = inline.AdSystem.Text;
			vAd.AdSystemVersion = inline.AdSystem.Version;
			vAd.AdTitle = inline.AdTitle.Text;
			for (int i = 0; i < inline.Impressions.Length; i++)
			{
				vAd.Impressions.Add(i, inline.Impressions[i].URL);
			}
			for (int j = 0; j < inline.Creatives.Creative.Length; j++)
			{
				VCreative vCreative = new VCreative();
				vCreative.ID = inline.Creatives.Creative[j].Id;
				vCreative.Sequence = inline.Creatives.Creative[j].Sequence;
				if (inline.Creatives.Creative[j].LinearAds != null)
				{
					Linear[] linearAds = inline.Creatives.Creative[j].LinearAds;
					foreach (Linear linear in linearAds)
					{
						vCreative.Type = "Linear";
						if (linear.SkipOffset != null)
						{
							vCreative.SkipOffset = linear.SkipOffset;
							vCreative.Skippable = true;
						}
						vCreative.Duration = linear.Duration;
						Tracking[] tracking = linear.TrackingEvents.Tracking;
						for (int l = 0; l < tracking.Length; l++)
						{
							if (vCreative.TrackingEvents.ContainsKey(tracking[l].EventType))
							{
								vCreative.TrackingEvents[tracking[l].EventType].Add(tracking[l].EventURL);
								continue;
							}
							vCreative.TrackingEvents.Add(tracking[l].EventType, new List<string> { tracking[l].EventURL });
						}
						VideoClicks videoClicks = linear.VideoClicks;
						if (videoClicks != null)
						{
							if (videoClicks.ClickThrough != null && !string.IsNullOrEmpty(videoClicks.ClickThrough.Text))
							{
								vCreative.VideoClicks.Add(AdEnums.ClickThrough, videoClicks.ClickThrough.Text);
							}
							if (videoClicks.ClickTracking != null && !string.IsNullOrEmpty(videoClicks.ClickTracking.Text))
							{
								vCreative.VideoClicks.Add(AdEnums.ClickTracking, videoClicks.ClickTracking.Text);
							}
							if (videoClicks.CustomClick != null && !string.IsNullOrEmpty(videoClicks.CustomClick.Text))
							{
								vCreative.VideoClicks.Add(AdEnums.CustomClick, videoClicks.CustomClick.Text);
							}
						}
						MediaFile[] mediaFile = linear.MediaFiles.MediaFile;
						if (mediaFile != null)
						{
							for (int m = 0; m < mediaFile.Length; m++)
							{
								vCreative.MediaFiles.Add(mediaFile[m].MimeType, mediaFile[m].MediaUrl.Trim());
							}
						}
					}
				}
				else if (inline.Creatives.Creative[j].NonLinearAds != null)
				{
					vCreative.Type = "NonLinear";
					Debug.Log("NonLinearAds not supported");
				}
				else if (inline.Creatives.Creative[j].CompanionAds != null)
				{
					vCreative.Type = "CompanionAd";
					Debug.Log("CompanionAds not supported");
				}
				vAd.Creatives.Add(vCreative);
			}
			if (inline.Extensions != null && inline.Extensions.Extension.Length > 0)
			{
				for (int n = 0; n < inline.Extensions.Extension.Length; n++)
				{
					vAd.Extensions.Add(inline.Extensions.Extension[n].Type, inline.Extensions.Extension[n].Value);
				}
			}
			return vAd;
		}

		private void ExtractWrapper(Wrapper wrapper)
		{
			VWrapper vWrapper = new VWrapper();
			vWrapper.AdSystem = wrapper.AdSystem.Text;
			vWrapper.VASTAdTagURI = wrapper.AdTagURI.Text;
			if (wrapper.Error != null)
			{
				vWrapper.ErrorURI = wrapper.Error.Text;
			}
			for (int i = 0; i < wrapper.Impressions.Length; i++)
			{
				vWrapper.Impressions.Add(i, wrapper.Impressions[i].URL);
			}
			for (int j = 0; j < wrapper.Creatives.Creative.Length; j++)
			{
				VCreative vCreative = new VCreative();
				vCreative.ID = string.Empty;
				vCreative.Sequence = string.Empty;
				if (wrapper.Creatives.Creative[j].LinearAds[0] != null)
				{
					vCreative.Type = "Linear";
					vCreative.Duration = string.Empty;
					if (wrapper.Creatives != null && wrapper.Creatives.Creative[j] != null)
					{
						if (wrapper.Creatives.Creative[j].TrackingEvents != null)
						{
							for (int k = 0; k < wrapper.Creatives.Creative[j].TrackingEvents.Length; k++)
							{
								Tracking[] tracking = wrapper.Creatives.Creative[j].TrackingEvents[k].Tracking;
								for (int l = 0; l < tracking.Length; l++)
								{
									if (vCreative.TrackingEvents.ContainsKey(tracking[l].EventType))
									{
										vCreative.TrackingEvents[tracking[l].EventType].Add(tracking[l].EventURL);
										continue;
									}
									vCreative.TrackingEvents.Add(tracking[l].EventType, new List<string> { tracking[l].EventURL });
								}
							}
						}
						if (wrapper.Creatives.Creative[j].VideoClicks != null)
						{
							VideoClicks videoClicks = wrapper.Creatives.Creative[j].VideoClicks;
							if (videoClicks.ClickThrough != null && !string.IsNullOrEmpty(videoClicks.ClickThrough.Text))
							{
								vCreative.VideoClicks.Add(AdEnums.ClickThrough, videoClicks.ClickThrough.Text);
							}
							if (videoClicks.ClickTracking != null && !string.IsNullOrEmpty(videoClicks.ClickTracking.Text))
							{
								vCreative.VideoClicks.Add(AdEnums.ClickTracking, videoClicks.ClickTracking.Text);
							}
							if (videoClicks.CustomClick != null && !string.IsNullOrEmpty(videoClicks.CustomClick.Text))
							{
								vCreative.VideoClicks.Add(AdEnums.CustomClick, videoClicks.CustomClick.Text);
							}
						}
					}
				}
				else if (wrapper.Creatives.Creative[j].NonLinearAds[0] != null)
				{
					vCreative.Type = "NonLinear";
					vCreative.Duration = string.Empty;
				}
				else if (wrapper.Creatives.Creative[j].CompanionAds[0] != null)
				{
					vCreative.Type = "CompanionAd";
					vCreative.Duration = "0";
				}
				vWrapper.Creatives.Add(vCreative);
			}
			Wrappers.Add(vWrapper);
			m_wrapperCount++;
			StartCoroutine(GetVast(vWrapper.VASTAdTagURI));
		}

		public void DiscardAd(VAd adToRemove)
		{
			ValintaAds.Remove(adToRemove);
		}

		private void Success()
		{
			m_isPrimaryFetched = false;
			m_isSecondaryFetched = false;
			ValintaAds[0].SetLinkText();
			ValintaAds[0].UpdateEventTimes();
			if (this.OnAdReady != null)
			{
				this.OnAdReady(ValintaAds[0]);
			}
		}

		private void Error(string s)
		{
			if (this.OnAdError != null)
			{
				this.OnAdError("Error fetching ad: " + s);
			}
		}
	}
}
