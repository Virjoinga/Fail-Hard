using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using Valinta.SimpleJSON;

namespace Valinta
{
	public class ValintaMain : MonoBehaviour
	{
		public enum PlayerState
		{
			Authenticate = 0,
			Authenticating = 1,
			StandBy = 2,
			Loading = 3,
			Playing = 4,
			Paused = 5,
			PrepareAd = 6,
			PlayingAd = 7,
			Stop = 8,
			Error = 9
		}

		public enum ErrorState
		{
			Clean = 0,
			NoAuth = 1,
			AuthFailed = 2,
			NetworkDown = 3,
			LoadFailed = 4
		}

		public delegate void ValintaPlayerInitialized();

		public delegate void ValintaPlayerPaused(bool isPaused);

		public delegate void ValintaPlayerLocked(bool isLocked);

		public delegate void ValintaStatusChanged(string s);

		public delegate void ValintaError();

		private const string m_baseURL = "http://52.30.163.145";

		private const string m_authenticationURL = "/api/v2/authentications/check";

		private const string m_genreURL = "/api/v2/genres";

		public static ValintaMain Instance;

		public PlayerState CurrentPlayerState;

		public ErrorState CurrentErrorState;

		private WWW m_songLoader;

		private AudioSource m_audioSource;

		private string m_authenticationKey = string.Empty;

		private bool m_authenticationFailed = true;

		private int m_authenticationFailCount;

		private Dictionary<string, string> m_headers;

		private int m_adFrequency;

		private string m_adInventoryHash = string.Empty;

		private string m_notification;

		private string m_socialTemplate;

		private float m_sessionDataFrequency = 120f;

		private VSession m_session;

		private float m_currentDeviceVolumeLevel = 1f;

		private float m_loadFailedGraceTime;

		private bool m_userSkipped;

		private int m_songsPlayed;

		private bool m_isPlayerLocked;

		public bool AdInProgress;

		private bool m_noAdAvailable;

		private bool m_adUserPaused;

		private bool m_adStarted;

		private bool m_adResumed;

		private bool m_adClicked;

		private VCatalogue Catalogue;

		private VAd CurrentAd;

		private VPlaylist CurrentPlaylist;

		public VSong CurrentSong;

		private bool m_appPaused;

		private float m_pauseStarted;

		private float m_pauseTimer;

		private float m_sessionStartTime;

		private float m_currentTime;

		[method: MethodImpl(32)]
		public static event ValintaPlayerInitialized OnValintaInitialized;

		[method: MethodImpl(32)]
		public static event ValintaPlayerPaused OnPlayerPaused;

		[method: MethodImpl(32)]
		public static event ValintaPlayerLocked OnPlayerLocked;

		[method: MethodImpl(32)]
		public static event ValintaStatusChanged OnStatusChanged;

		[method: MethodImpl(32)]
		public static event ValintaError OnPlayerError;

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
			}
			Catalogue = new VCatalogue();
			base.gameObject.AddComponent<VAds>();
			VAds.Instance.OnAdReady += OnAdFetched;
			VAds.Instance.OnAdError += OnAdFetchError;
			m_audioSource = base.gameObject.AddComponent<AudioSource>();
			m_audioSource.playOnAwake = false;
		}

		public void Initialize(string authKey)
		{
			m_authenticationKey = authKey;
			CurrentPlayerState = PlayerState.Authenticate;
		}

		private void StartSession()
		{
			StatusUpdate("Choose your music");
			StartCoroutine(GetPlaylistsFromServer());
			string @string = PlayerPrefs.GetString("ValintaSessionAnalytics");
			if (!string.IsNullOrEmpty(@string))
			{
				StartCoroutine(SendSessionData(@string));
			}
			m_session = new VSession(m_authenticationKey, GetUserId());
			m_sessionStartTime = Time.realtimeSinceStartup;
			ChangePlayerState(PlayerState.StandBy);
		}

		private IEnumerator Authenticate()
		{
			if (m_authenticationFailCount > 2)
			{
				yield break;
			}
			StatusUpdate("Authenticating...");
			if (m_headers == null)
			{
				m_headers = new Dictionary<string, string>();
			}
			else
			{
				m_headers.Clear();
			}
			m_headers.Add("AUTHORIZATION", m_authenticationKey);
			m_headers.Add("CLIENT", GetUserId());
			WWW www = new WWW("http://52.30.163.145" + "/api/v2/authentications/check", null, m_headers);
			yield return www;
			if (www.responseHeaders.ContainsKey("STATUS"))
			{
				string status = string.Empty;
				www.responseHeaders.TryGetValue("STATUS", out status);
				if (status.Contains("200"))
				{
					m_authenticationFailed = false;
					m_authenticationFailCount = 0;
					GetExtra(www.text);
					StartSession();
					yield break;
				}
				if (status.Contains("40"))
				{
					m_authenticationFailed = true;
					ChangeErrorState(ErrorState.NoAuth);
					ChangePlayerState(PlayerState.Error);
					yield break;
				}
			}
			m_authenticationFailCount++;
			m_authenticationFailed = true;
			if (www.responseHeaders.Count == 0)
			{
				yield return new WaitForSeconds(4f);
				ChangeErrorState(ErrorState.NoAuth);
				ChangePlayerState(PlayerState.Error);
			}
			yield return new WaitForSeconds(4f);
			StartCoroutine(Authenticate());
		}

		private void GetExtra(string json)
		{
			JSONNode jSONNode = Valinta.SimpleJSON.JSON.Parse(json);
			JSONNode jSONNode2 = jSONNode["ad-freq"];
			if (jSONNode2 != null)
			{
				int.TryParse(jSONNode2.Value, out m_adFrequency);
			}
			else
			{
				m_adFrequency = 0;
			}
			JSONNode jSONNode3 = jSONNode["session-freq"];
			if (jSONNode3 != null)
			{
				float.TryParse(jSONNode3.Value, out m_sessionDataFrequency);
			}
			else
			{
				m_sessionDataFrequency = 120f;
			}
			JSONNode jSONNode4 = jSONNode["social-template"];
			if (jSONNode4 != null)
			{
				m_socialTemplate = jSONNode4.Value;
			}
			else
			{
				m_socialTemplate = "Found this cool song \"{0}\" by {1} when playing the game {2}! Get it from apollomusic.dk/game-music\n[valinta] by @Zemeho - your game, your music.";
			}
			JSONNode jSONNode5 = jSONNode["notification"];
			if (jSONNode5 != null)
			{
				m_notification = jSONNode5.Value;
			}
			else
			{
				m_notification = string.Empty;
			}
			JSONNode jSONNode6 = jSONNode["primary-ad-url"];
			if (jSONNode6 != null)
			{
				VAds.Instance.PrimaryAdURL = jSONNode6.Value;
			}
			JSONNode jSONNode7 = jSONNode["secondary-ad-url"];
			if (jSONNode7 != null)
			{
				VAds.Instance.SecondaryAdURL = jSONNode7.Value;
			}
		}

		private void OnApplicationQuit()
		{
			if (m_session != null)
			{
				m_session.Stop();
				SaveSessionData(false);
			}
		}

		private void OnApplicationPause(bool pauseStatus)
		{
			m_appPaused = pauseStatus;
			if (pauseStatus)
			{
				return;
			}
			m_adClicked = false;
			if (m_session != null)
			{
				if (Time.realtimeSinceStartup - m_sessionStartTime > m_sessionDataFrequency)
				{
					SaveSessionData(true);
				}
				m_sessionStartTime = Time.realtimeSinceStartup;
			}
		}

		private void OnApplicationFocus(bool focus)
		{
			if (focus)
			{
				if (m_appPaused)
				{
					OnApplicationPause(false);
				}
			}
			else
			{
				OnApplicationPause(true);
			}
		}

		public void FetchMobileVolume()
		{
			VMobileUtils.GetDeviceVolume(base.gameObject, "OnMobileVolumeFetched");
		}

		private void OnMobileVolumeFetched(string vLevel)
		{
			float result = 0f;
			if (float.TryParse(vLevel, out result))
			{
				m_currentDeviceVolumeLevel = result;
			}
			else
			{
				Debug.Log("Couldn't parse volume");
			}
		}

		private void Update()
		{
			m_currentTime = Time.realtimeSinceStartup - m_sessionStartTime;
			if (m_currentTime > m_sessionDataFrequency)
			{
				SaveSessionData(true);
				m_sessionStartTime = Time.realtimeSinceStartup;
			}
			switch (CurrentPlayerState)
			{
			case PlayerState.Authenticate:
				m_authenticationFailCount = 0;
				m_authenticationFailed = true;
				CurrentPlayerState = PlayerState.Authenticating;
				StartCoroutine(Authenticate());
				break;
			case PlayerState.Authenticating:
				if (m_authenticationFailed && m_authenticationFailCount > 2)
				{
					StopAllCoroutines();
					ChangeErrorState(ErrorState.AuthFailed);
					ChangePlayerState(PlayerState.Error);
				}
				break;
			case PlayerState.StandBy:
				break;
			case PlayerState.Loading:
				m_loadFailedGraceTime += Time.deltaTime;
				if (CheckForDelay(20f))
				{
					m_songLoader = null;
					ChangeErrorState(ErrorState.LoadFailed);
					ChangePlayerState(PlayerState.Error);
				}
				break;
			case PlayerState.Paused:
				m_pauseTimer = Time.realtimeSinceStartup - m_pauseStarted;
				if (!m_adUserPaused && !m_adClicked && AdInProgress && m_currentDeviceVolumeLevel > 0f && !m_adResumed)
				{
					Play();
					m_adResumed = true;
				}
				break;
			case PlayerState.Playing:
				if (m_isPlayerLocked && !AdInProgress)
				{
					LockPlayer(false);
				}
				if (!m_audioSource.isPlaying)
				{
					ChangePlayerState(PlayerState.StandBy);
					m_audioSource.Stop();
					if (!m_userSkipped)
					{
						Play();
					}
				}
				break;
			case PlayerState.PrepareAd:
				StatusUpdate("Loading...");
				m_loadFailedGraceTime += Time.deltaTime;
				if (CheckForDelay(30f))
				{
					m_songLoader = null;
					ChangeErrorState(ErrorState.LoadFailed);
					ChangePlayerState(PlayerState.Error);
					LockPlayer(false);
				}
				if (m_noAdAvailable)
				{
					DiscardCurrentAd();
					ChangePlayerState(PlayerState.StandBy);
					Play();
				}
				else if (CurrentAd != null)
				{
					Stop();
					AdInProgress = true;
					PlayAd();
					ChangePlayerState(PlayerState.PlayingAd);
				}
				break;
			case PlayerState.PlayingAd:
				if (m_audioSource.isPlaying)
				{
					CheckAdState();
				}
				else if (!m_audioSource.isPlaying && !m_adStarted)
				{
					CurrentAd.DoEventTracking(AdEnums.Start);
					m_audioSource.Play();
					m_adStarted = true;
				}
				break;
			case PlayerState.Error:
				HandleError();
				break;
			case PlayerState.Stop:
				break;
			}
		}

		private void ChangePlayerState(PlayerState state)
		{
			if (CurrentPlayerState == PlayerState.StandBy || (state != 0 && state != PlayerState.Authenticating))
			{
				CurrentPlayerState = state;
			}
		}

		private void ChangeErrorState(ErrorState state)
		{
			CurrentErrorState = state;
		}

		private void ResetLoadingFailedNotification()
		{
			ChangeErrorState(ErrorState.Clean);
			m_loadFailedGraceTime = 0f;
		}

		private void HandleError()
		{
			switch (CurrentErrorState)
			{
			case ErrorState.NoAuth:
				StatusUpdate("Player authentication failed");
				break;
			case ErrorState.AuthFailed:
				StatusUpdate("Authentication failed");
				break;
			case ErrorState.NetworkDown:
				StatusUpdate("Check internet connection and try again");
				break;
			case ErrorState.LoadFailed:
				StatusUpdate("Loading failed, try again");
				break;
			}
			ChangePlayerState(PlayerState.StandBy);
			if (ValintaMain.OnPlayerError != null)
			{
				ValintaMain.OnPlayerError();
			}
			ResetLoadingFailedNotification();
		}

		private void StatusUpdate(string s)
		{
			if (ValintaMain.OnStatusChanged != null)
			{
				ValintaMain.OnStatusChanged(s);
			}
		}

		private IEnumerator GetPlaylistsFromServer()
		{
			WWW www = new WWW("http://52.30.163.145" + "/api/v2/genres", null, m_headers);
			yield return www;
			if (!string.IsNullOrEmpty(www.text))
			{
				JSONNode N = Valinta.SimpleJSON.JSON.Parse(www.text);
				int itemCount = N["genres"].Count;
				for (int i = 0; i < itemCount; i++)
				{
					Catalogue.AddPlaylist(new VPlaylist(N["genres"][i]["id"].AsInt, N["genres"][i]["title"].Value));
				}
				StartCoroutine(PopulatePlaylists());
			}
			else
			{
				StatusUpdate("Unknown error");
				ChangePlayerState(PlayerState.StandBy);
				Debug.Log("Error getting playlists");
			}
		}

		private IEnumerator PopulatePlaylists()
		{
			foreach (VPlaylist playlist in Catalogue.GetAllPlaylists())
			{
				WWW www = new WWW("http://52.30.163.145" + "/api/v2/genres" + "/" + playlist.Id + "/musics", null, m_headers);
				yield return www;
				if (!string.IsNullOrEmpty(www.text))
				{
					JSONNode N = Valinta.SimpleJSON.JSON.Parse(www.text);
					int itemCount = N["musics"].Count;
					for (int i = 0; i < itemCount; i++)
					{
						Catalogue.GetPlaylistByID(playlist.Id).AddSong(new VSong
						{
							Id = N["musics"][i]["id"].Value,
							Title = N["musics"][i]["title"].Value,
							Artist = N["musics"][i]["artist"].Value,
							Site = N["musics"][i]["site"].Value,
							Album = N["musics"][i]["album"].Value,
							Duration = N["musics"][i]["duration"].Value
						});
					}
				}
				yield return null;
			}
			if (ValintaMain.OnValintaInitialized != null)
			{
				ValintaMain.OnValintaInitialized();
			}
		}

		public string GetUserId()
		{
			return SystemInfo.deviceUniqueIdentifier;
		}

		public string GetAuthenticationKey()
		{
			return m_authenticationKey;
		}

		private void SaveSessionData(bool sendImmediately)
		{
			string empty = string.Empty;
			empty += "{\"music\":{";
			empty += string.Format("\"session_start\":{0},", m_session.GetStartTime());
			empty += string.Format("\"session_end\":{0},", m_session.GetEndTime());
			empty += "\"skipped_songs\":[";
			if (m_session.GetSkippedSongList().Count > 0)
			{
				for (int i = 0; i < m_session.GetSkippedSongList().Count; i++)
				{
					string id = m_session.GetSkippedSongList()[i].Id;
					empty += string.Format("{0}", id);
					if (i < m_session.GetSkippedSongList().Count - 1)
					{
						empty += ",";
					}
				}
			}
			empty += "],";
			empty += string.Format("\"paused_time\":{0}", m_session.GetPausedTime());
			empty += "}}";
			m_session.Reset();
			if (sendImmediately)
			{
				StartCoroutine(SendSessionData(empty));
			}
			else
			{
				PlayerPrefs.SetString("ValintaSessionAnalytics", empty);
			}
		}

		private IEnumerator SendSessionData(string s)
		{
			if (!string.IsNullOrEmpty(s))
			{
				string baseAddress = "http://52.30.163.145" + "/api/v2/musics/analytics";
				HttpWebRequest http = (HttpWebRequest)WebRequest.Create(new Uri(baseAddress));
				http.Accept = "application/json";
				http.ContentType = "application/json";
				http.Method = "POST";
				http.Headers.Add("AUTHORIZATION", m_authenticationKey);
				http.Headers.Add("CLIENT", GetUserId());
				ASCIIEncoding encoding = new ASCIIEncoding();
				byte[] bytes = encoding.GetBytes(s);
				Stream newStream = http.GetRequestStream();
				newStream.Write(bytes, 0, bytes.Length);
				newStream.Close();
				PlayerPrefs.DeleteKey("ValintaSessionAnalytics");
			}
			yield break;
		}

		public string GetInventoryHash()
		{
			return m_adInventoryHash;
		}

		public int GetAdFrequency()
		{
			return m_adFrequency;
		}

		private void ResetPauseTimer()
		{
			if (m_pauseTimer > 0f)
			{
				m_session.AddToPausedTime(m_pauseTimer);
				m_pauseTimer = 0f;
				m_pauseStarted = 0f;
			}
		}

		public void Play(VPlaylist playlist)
		{
			CurrentPlaylist = playlist;
			if (AdInProgress)
			{
				Skip();
			}
			else
			{
				Play();
			}
		}

		public void Play()
		{
			ResetPauseTimer();
			if (m_authenticationFailed)
			{
				ChangePlayerState(PlayerState.Authenticate);
			}
			else if (CurrentPlayerState == PlayerState.Paused)
			{
				if (AdInProgress)
				{
					CurrentAd.DoEventTracking(AdEnums.Resume);
					m_adUserPaused = false;
					m_adResumed = true;
					CurrentAd.StartTime = Time.realtimeSinceStartup;
				}
				m_audioSource.Play();
				ChangePlayerState(AdInProgress ? PlayerState.PlayingAd : PlayerState.Playing);
				Paused(false);
			}
			else if (IsAdNext() && CurrentAd == null)
			{
				VAds.Instance.RequestAd();
				ChangePlayerState(PlayerState.PrepareAd);
				LockPlayer(true);
			}
			else
			{
				m_songsPlayed++;
				StopAllCoroutines();
				StartCoroutine(PrepareSongStream());
				Paused(false);
				m_userSkipped = false;
			}
		}

		public void Pause()
		{
			if (CurrentPlayerState == PlayerState.StandBy)
			{
				Play();
				return;
			}
			m_audioSource.Pause();
			if (AdInProgress)
			{
				m_adResumed = false;
				m_adUserPaused = true;
				CurrentAd.DoEventTracking(AdEnums.Pause);
				CurrentAd.Duration = CurrentAd.TimeLeft;
			}
			m_pauseStarted = Time.realtimeSinceStartup;
			ChangePlayerState(PlayerState.Paused);
			Paused(true);
		}

		public void Stop()
		{
			m_audioSource.Stop();
		}

		public void Skip()
		{
			Stop();
			m_userSkipped = true;
			if (AdInProgress)
			{
				CurrentAd.DoEventTracking(AdEnums.Skip);
				DiscardCurrentAd();
			}
			if (CurrentPlaylist != null && CurrentSong != null)
			{
				VSession.Instance.AddSkippedSong(CurrentSong);
			}
			Play();
		}

		public void Share()
		{
			if (CurrentSong != null)
			{
				string b = string.Format(m_socialTemplate, CurrentSong.Title, CurrentSong.Artist, "Fail Hard");
				VMobileUtils.ShareText("[V]", b);
			}
		}

		private void Paused(bool isPaused)
		{
			if (ValintaMain.OnPlayerPaused != null)
			{
				ValintaMain.OnPlayerPaused(isPaused);
			}
		}

		private void LockPlayer(bool locked)
		{
			m_isPlayerLocked = locked;
			if (ValintaMain.OnPlayerLocked != null)
			{
				ValintaMain.OnPlayerLocked(locked);
			}
		}

		private void UpdateSong()
		{
			ResetLoadingFailedNotification();
			m_audioSource.Play();
			if (AdInProgress)
			{
				StatusUpdate(CurrentAd.LinkText);
				ChangePlayerState(PlayerState.PlayingAd);
				CurrentAd.StartTime = Time.realtimeSinceStartup;
				CurrentAd.UpdateEventTimes();
			}
			else
			{
				StatusUpdate(string.Format("{0}: {1} - {2}", CurrentPlaylist.Name, CurrentSong.Artist, CurrentSong.Title));
				ChangePlayerState(PlayerState.Playing);
			}
		}

		private IEnumerator CallWWW(string url)
		{
			WWW www = new WWW(url);
			yield return www;
			if (!string.IsNullOrEmpty(www.error) && !www.error.Contains("40"))
			{
				ChangeErrorState(ErrorState.NetworkDown);
				ChangePlayerState(PlayerState.Error);
			}
		}

		private IEnumerator PrepareSongStream()
		{
			StatusUpdate("Loading...");
			if (CurrentPlaylist == null)
			{
				CurrentPlaylist = Catalogue.GetPlaylistByIndex(0);
			}
			CurrentSong = CurrentPlaylist.GetNextSong();
			WWW www = new WWW("http://52.30.163.145" + "/api/v2/genres" + "/" + CurrentPlaylist.Id + "/musics/" + CurrentSong.Id, null, m_headers);
			ChangePlayerState(PlayerState.Loading);
			yield return www;
			try
			{
				StartCoroutine(StartStream(GetTemporaryURL(www.text), AudioType.MPEG));
			}
			catch (NullReferenceException e)
			{
				Debug.Log("Could not get url string: " + e);
				ChangeErrorState(ErrorState.NetworkDown);
				ChangePlayerState(PlayerState.Error);
			}
		}

		private IEnumerator StartStream(string url, AudioType audioType)
		{
			if (m_audioSource.clip != null)
			{
				m_audioSource.Stop();
				UnityEngine.Object.Destroy(m_audioSource.clip);
				m_audioSource.clip = null;
			}
			m_songLoader = new WWW(url);
			ChangePlayerState(PlayerState.Loading);
			yield return m_songLoader;
			if (m_songLoader != null && m_songLoader.isDone)
			{
				m_audioSource.clip = m_songLoader.GetAudioClip(false, true, audioType);
				m_audioSource.loop = false;
				m_audioSource.Play();
				UpdateSong();
			}
		}

		private string GetTemporaryURL(string s)
		{
			JSONNode jSONNode = Valinta.SimpleJSON.JSON.Parse(s);
			return jSONNode["music"]["source"].Value;
		}

		private bool CheckForDelay(float allowedDelay)
		{
			if (CurrentPlayerState != PlayerState.PrepareAd)
			{
				ChangePlayerState(PlayerState.Loading);
			}
			if (m_loadFailedGraceTime >= allowedDelay)
			{
				return true;
			}
			return false;
		}

		private void OnAdFetchError(string desc)
		{
			m_noAdAvailable = true;
		}

		private void OnAdFetched(VAd ad)
		{
			m_noAdAvailable = false;
			CurrentAd = ad;
			CurrentAd.OnError += CurrentAd_OnError;
			CurrentAd.OnSuccess += CurrentAd_OnSuccess;
		}

		private void CheckAdState()
		{
			CurrentAd.TimeTick = Time.realtimeSinceStartup - CurrentAd.StartTime;
			CurrentAd.TimeLeft = CurrentAd.Duration - CurrentAd.TimeTick;
			if (m_currentDeviceVolumeLevel <= 0f)
			{
				PauseAd();
			}
			CurrentAd.CurrentTimePosition = CurrentAd.TotalDuration - CurrentAd.TimeLeft;
			if (m_isPlayerLocked && CurrentAd.Skippable && CurrentAd.CurrentTimePosition > CurrentAd.SkipTime)
			{
				LockPlayer(false);
			}
			if (CurrentAd.CurrentTimePosition > CurrentAd.FirstQuartile && !CurrentAd.IsEventFired(AdEnums.FirstQuartile))
			{
				CurrentAd.DoEventTracking(AdEnums.FirstQuartile);
			}
			if (CurrentAd.CurrentTimePosition > CurrentAd.Midpoint && !CurrentAd.IsEventFired(AdEnums.Midpoint))
			{
				CurrentAd.DoEventTracking(AdEnums.Midpoint);
			}
			if (CurrentAd.CurrentTimePosition > CurrentAd.ThirdQuartile && !CurrentAd.IsEventFired(AdEnums.ThirdQuartile))
			{
				CurrentAd.DoEventTracking(AdEnums.ThirdQuartile);
			}
			if (CurrentAd.CurrentTimePosition > CurrentAd.TotalDuration && !CurrentAd.IsEventFired(AdEnums.Complete))
			{
				CurrentAd.DoEventTracking(AdEnums.Complete);
				DiscardCurrentAd();
				ChangePlayerState(PlayerState.StandBy);
				Play();
			}
		}

		public void PlayAd()
		{
			CurrentSong = null;
			LockPlayer(true);
			List<string> impressionUrls = CurrentAd.GetImpressionUrls();
			string mediaUrl = CurrentAd.GetMediaUrl();
			foreach (string item in impressionUrls)
			{
				StartCoroutine(CallWWW(item));
			}
			StartCoroutine(StartStream(mediaUrl, AudioType.MPEG));
		}

		public void PauseAd()
		{
			m_audioSource.Pause();
			if (AdInProgress)
			{
				m_adResumed = false;
				CurrentAd.DoEventTracking(AdEnums.Pause);
				CurrentAd.Duration = CurrentAd.TimeLeft;
			}
			m_pauseStarted = Time.realtimeSinceStartup;
			ChangePlayerState(PlayerState.Paused);
			Paused(true);
		}

		public string GetCurrentAdUrl()
		{
			m_adClicked = true;
			PauseAd();
			return CurrentAd.GetClickUrl();
		}

		private void CurrentAd_OnSuccess(string s)
		{
			StartCoroutine(CallWWW(s));
		}

		private void CurrentAd_OnError(string s)
		{
			Debug.Log("Error: " + s);
		}

		private void DiscardCurrentAd()
		{
			ChangePlayerState(PlayerState.StandBy);
			LockPlayer(false);
			if (CurrentAd != null)
			{
				CurrentAd.OnError -= CurrentAd_OnError;
				CurrentAd.OnSuccess -= CurrentAd_OnSuccess;
				VAds.Instance.DiscardAd(CurrentAd);
				CurrentAd = null;
			}
			m_songsPlayed = 0;
			AdInProgress = false;
			m_noAdAvailable = false;
			m_adStarted = false;
		}

		private bool IsAdNext()
		{
			if (m_songsPlayed <= 0 || m_adFrequency == 0)
			{
				return false;
			}
			if (m_songsPlayed % m_adFrequency == 0)
			{
				return true;
			}
			return false;
		}

		public VCatalogue GetCatalogue()
		{
			return Catalogue;
		}

		public AudioSource GetAudioSource()
		{
			return m_audioSource;
		}
	}
}
