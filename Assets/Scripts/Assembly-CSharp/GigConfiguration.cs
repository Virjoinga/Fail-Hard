using System.Collections.Generic;
using Game;
using UnityEngine;

public class GigConfiguration : MonoBehaviour
{
	public Transform PlayerSpawnPoint;

	public Transform VehicleSpawnPoint;

	public Camera ConfigCamera;

	private Camera m_cachedLevelCam;

	public List<Vehicle> Vehicles;

	public GameObject PlayerPrefab;

	private void Start()
	{
		Vehicles = GameController.Instance.VehicleModel.Model;
		GameObject gameObject = GameObject.Find("Level:root");
		m_cachedLevelCam = gameObject.GetComponentInChildren<Camera>();
		ConfigCamera.enabled = true;
		m_cachedLevelCam.enabled = false;
		SpawnPlayer();
		SpawnVehicle();
	}

	public void SpawnVehicle()
	{
		GameObject original = (GameObject)Resources.Load(GameController.Instance.Character.CurrentVehicle.ResourceName);
		GameObject gameObject = Object.Instantiate(original, VehicleSpawnPoint.position, VehicleSpawnPoint.rotation) as GameObject;
		gameObject.transform.parent = VehicleSpawnPoint.transform;
		gameObject.GetComponentInChildren<VehicleBase>().DisableAllDynamics();
	}

	public void SpawnPlayer()
	{
		GameObject gameObject = Object.Instantiate(PlayerPrefab, PlayerSpawnPoint.position, PlayerSpawnPoint.rotation) as GameObject;
		gameObject.transform.parent = PlayerSpawnPoint.transform;
		gameObject.GetComponentInChildren<Animation>().animation["fly"].wrapMode = WrapMode.Loop;
		gameObject.GetComponentInChildren<Animation>().Play("fly");
	}

	private void OnDestroy()
	{
		m_cachedLevelCam.enabled = true;
		ConfigCamera.enabled = false;
	}
}
