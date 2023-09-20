using Game;
using UnityEngine;

public class VehicleDebug : MonoBehaviour
{
	public UISprite frontMesh;

	public UISprite frontRay;

	public UISprite rearMesh;

	public UISprite rearRay;

	private void Start()
	{
	}

	private void Update()
	{
		if (GameController.Instance.Character.Vehicle != null)
		{
			VehicleMoped vehicleMoped = (VehicleMoped)GameController.Instance.Character.Vehicle;
			if (vehicleMoped.FrontExtraCollider.IsContact)
			{
				frontMesh.color = new Color(0f, 1f, 0f);
			}
			else
			{
				frontMesh.color = new Color(1f, 0f, 0f);
			}
			if (vehicleMoped.RearExtraCollider.IsContact)
			{
				rearMesh.color = new Color(0f, 1f, 0f);
			}
			else
			{
				rearMesh.color = new Color(1f, 0f, 0f);
			}
		}
	}
}
