namespace Valinta
{
	public class ValintaUI : VUI
	{
		public static ValintaUI Instance;

		protected override void Awake()
		{
			base.Awake();
			if (Instance == null)
			{
				Instance = this;
			}
			m_playerWindow.SetActive(false);
			m_playlistWindow.SetActive(false);
		}

		protected override void Start()
		{
			base.Start();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		public override void OnLinkClicked(string url)
		{
			base.OnLinkClicked(url);
		}

		public override void TogglePlaylistWindow()
		{
			base.TogglePlaylistWindow();
		}

		public bool IsPlaylistVisible()
		{
			return isPlaylistWindowVisible;
		}

		public bool IsPlayerVisible()
		{
			return isPlayerWindowVisible;
		}
	}
}
