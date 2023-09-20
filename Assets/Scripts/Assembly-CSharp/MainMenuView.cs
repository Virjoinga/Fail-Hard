using UnityEngine;

public class MainMenuView : MonoBehaviour
{
	public GameObject GameLogo;

	public GameObject PlayButton;

	public GarageCameraController GarageCamera;

	private bool m_started;

	private void Start()
	{
		m_started = true;
		OnEnable();
	}

	private void OnEnable()
	{
		if (m_started)
		{
			GameLogo.SetActive(true);
			GarageCamera.ToStartPosition();
			PlayButton.SetActive(true);
		}
	}

	private void OnDisable()
	{
		GameLogo.SetActive(false);
	}
}
