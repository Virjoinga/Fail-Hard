using Game;
using UnityEngine;

public class GigConfigPreview : MonoBehaviour
{
	public Transform PlayerSpawnPoint;

	public Transform VehicleSpawnPoint;

	public GameObject Background;

	public GameObject PlayerDeco;

	public GameObject VehicleDeco;

	public GameObject PV50Prefab;

	public GameObject PappaTPrefab;

	public GameObject VespaPrefab;

	public GameObject DirtBikePrefab;

	public GameObject StigaPrefab;

	public GameObject HillcarPrefab;

	public GameObject BuggyPrefab;

	public GameObject HDPrefab;

	public Camera ConfigCamera;

	private GameObject m_currentVehicle;

	private GameObject m_player;

	private GameObject m_headGadget;

	private Transform m_gadgetSlotHead;

	private GameObject m_backGadget;

	private Transform m_gadgetSlotBack;

	private GameObject m_vehicleGadget;

	private Transform m_vehicleSlot;

	public GameObject DefaultHeadPrefab;

	private GameObject m_defaultHead;

	private bool m_isShown;

	private Vehicle m_shownVehicle;

	public GameObject PlayerPrefab;

	public void ShowPlayerAndVehicle(bool isShown, Vehicle vehicle)
	{
		if (isShown == m_isShown)
		{
			return;
		}
		m_isShown = isShown;
		if (m_isShown)
		{
			SpawnPlayer();
			if (m_shownVehicle != vehicle)
			{
				SetVehicle(vehicle);
			}
			return;
		}
		PlayerDeco.SetActive(false);
		VehicleDeco.SetActive(false);
		if (m_currentVehicle != null)
		{
			Object.Destroy(m_currentVehicle);
		}
		if (m_player != null)
		{
			Object.Destroy(m_player);
		}
	}

	public void ShowBackground(bool isShown)
	{
		Background.SetActive(isShown);
	}

	public void SetVehicle(Vehicle vehicle)
	{
		if (m_shownVehicle == vehicle)
		{
			return;
		}
		if (m_currentVehicle != null)
		{
			Object.Destroy(m_currentVehicle);
		}
		VehicleDeco.SetActive(true);
		GameObject gameObject = null;
		gameObject = ((vehicle.ResourceName == "MopedSharp") ? PV50Prefab : ((vehicle.ResourceName == "MopedPappaT") ? PappaTPrefab : ((vehicle.ResourceName == "MopedDirtBike") ? DirtBikePrefab : ((vehicle.ResourceName == "Stiga") ? StigaPrefab : ((vehicle.ResourceName == "DownhillCar") ? HillcarPrefab : ((vehicle.ResourceName == "Buggy") ? BuggyPrefab : ((!(vehicle.ResourceName == "MopedHD")) ? VespaPrefab : HDPrefab)))))));
		m_currentVehicle = Object.Instantiate(gameObject, VehicleSpawnPoint.position, VehicleSpawnPoint.rotation) as GameObject;
		m_currentVehicle.transform.parent = VehicleSpawnPoint.transform;
		m_currentVehicle.GetComponentInChildren<VehicleBase>().DisableAllDynamics();
		m_vehicleSlot = m_currentVehicle.GetComponentInChildren<VehicleBase>().GadgetPosition;
		foreach (VehiclePart equippedGadget in vehicle.GetEquippedGadgets())
		{
			if (equippedGadget.ItemType == VehiclePartType.VehicleGadget)
			{
				SetVehicleGadget(equippedGadget);
			}
		}
	}

	public void SpawnPlayer()
	{
		m_isShown = true;
		PlayerDeco.SetActive(true);
		m_player = Object.Instantiate(PlayerPrefab, PlayerSpawnPoint.position, PlayerSpawnPoint.rotation) as GameObject;
		m_player.transform.parent = PlayerSpawnPoint.transform;
		m_gadgetSlotHead = RecursiveFind(base.transform, "GadgetSlotHead", 0);
		m_gadgetSlotBack = RecursiveFind(base.transform, "GadgetSlotBack", 0);
		m_player.GetComponentInChildren<Animation>().animation["idle_animation"].wrapMode = WrapMode.Loop;
		m_player.GetComponentInChildren<Animation>().animation["idle_animation"].speed = 0.4f;
		m_player.GetComponentInChildren<Animation>().Play("idle_animation");
		foreach (VehiclePart equippedGadget in GameController.Instance.Character.GetEquippedGadgets())
		{
			if (equippedGadget.ItemType == VehiclePartType.PlayerGadgetBack || equippedGadget.ItemType == VehiclePartType.PlayerGadgetHead)
			{
				SetPlayerGadget(equippedGadget);
			}
		}
		if (m_headGadget == null)
		{
			SetPlayerGadget(null);
		}
	}

	public void RemoveGadget(VehiclePartType removedType)
	{
		switch (removedType)
		{
		case VehiclePartType.PlayerGadgetHead:
			if (m_headGadget != null)
			{
				m_headGadget.GetComponent<Gadget>().PreviewUnequip(m_player.transform);
				Object.Destroy(m_headGadget);
			}
			SetPlayerGadget(null);
			break;
		case VehiclePartType.PlayerGadgetBack:
			if (m_backGadget != null)
			{
				m_backGadget.GetComponent<Gadget>().PreviewUnequip(m_player.transform);
				Object.Destroy(m_backGadget);
			}
			break;
		default:
			if (m_vehicleGadget != null)
			{
				Object.Destroy(m_vehicleGadget);
			}
			break;
		}
	}

	public void SetPlayerGadget(VehiclePart gadget)
	{
		GameObject gameObject = null;
		gameObject = ((gadget != null) ? ((GameObject)Resources.Load(gadget.GadgetPrefabResource)) : DefaultHeadPrefab);
		if (gadget == null || gadget.ItemType == VehiclePartType.PlayerGadgetHead)
		{
			if (m_headGadget != null)
			{
				m_headGadget.GetComponent<Gadget>().PreviewUnequip(m_player.transform);
				Object.Destroy(m_headGadget);
			}
			m_headGadget = Object.Instantiate(gameObject, m_gadgetSlotHead.position, m_gadgetSlotHead.rotation) as GameObject;
			m_headGadget.transform.parent = m_gadgetSlotHead.parent;
			m_headGadget.GetComponent<Gadget>().PreviewEquip(m_player.transform);
		}
		else if (gadget.ItemType == VehiclePartType.PlayerGadgetBack)
		{
			if (m_backGadget != null)
			{
				m_backGadget.GetComponent<Gadget>().PreviewUnequip(m_player.transform);
				Object.Destroy(m_backGadget);
			}
			m_backGadget = Object.Instantiate(gameObject, m_gadgetSlotBack.position, m_gadgetSlotBack.rotation) as GameObject;
			m_backGadget.transform.parent = m_gadgetSlotBack.parent;
			m_backGadget.GetComponent<Gadget>().PreviewEquip(m_player.transform);
		}
	}

	public void SetVehicleGadget(VehiclePart gadget)
	{
		GameObject original = (GameObject)Resources.Load(gadget.GadgetPrefabResource);
		if (m_vehicleGadget != null)
		{
			Object.Destroy(m_vehicleGadget);
		}
		m_vehicleGadget = Object.Instantiate(original, m_vehicleSlot.position, m_vehicleSlot.rotation) as GameObject;
		m_vehicleGadget.transform.parent = m_vehicleSlot.parent;
	}

	private Transform RecursiveFind(Transform node, string name, int depth)
	{
		if (depth > 15)
		{
			return null;
		}
		if (node.gameObject.name == name)
		{
			return node;
		}
		for (int i = 0; i < node.childCount; i++)
		{
			Transform transform = RecursiveFind(node.GetChild(i), name, depth + 1);
			if (transform != null)
			{
				return transform;
			}
		}
		return null;
	}
}
