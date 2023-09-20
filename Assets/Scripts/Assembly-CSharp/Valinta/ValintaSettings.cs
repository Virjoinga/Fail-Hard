using UnityEngine;

namespace Valinta
{
	public class ValintaSettings : MonoBehaviour
	{
		private ValintaUI ValintaUi;

		[SerializeField]
		private string AuthenticationKey;

		private void Awake()
		{
			if (Object.FindObjectsOfType<ValintaSettings>().Length > 1)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			Object.DontDestroyOnLoad(base.gameObject);
			base.gameObject.AddComponent<ValintaMain>();
			ValintaUi = GetComponent<ValintaUI>();
			ValintaMain.OnValintaInitialized += ValintaMain_OnValintaReady;
		}

		private void Start()
		{
			ValintaMain.Instance.Initialize(AuthenticationKey);
		}

		private void OnDestroy()
		{
			ValintaMain.OnValintaInitialized -= ValintaMain_OnValintaReady;
		}

		private void ValintaMain_OnValintaReady()
		{
			ValintaUi.Initialize();
		}
	}
}
