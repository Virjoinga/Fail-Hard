using System.Collections.Generic;
using Game;
using UnityEngine;

public class GadgetBodyArmor : Gadget
{
	public List<BodyPartType> ExtraBodyPartTypes;

	public List<GameObject> ExtraParts;

	public override void PreviewEquip(Transform player)
	{
		ReparentExtraParts(player);
	}

	public override void PreviewUnequip(Transform player)
	{
		foreach (GameObject extraPart in ExtraParts)
		{
			extraPart.transform.parent = base.transform;
		}
	}

	public override void Equip(Player player, Rigidbody rb, VehiclePart vp)
	{
		base.State = GadgetState.GadgetOff;
		base.VehiclePart = vp;
		m_player = player;
		ReparentExtraParts(player.transform);
	}

	private void ReparentExtraParts(Transform root)
	{
		BodyPart[] componentsInChildren = root.GetComponentsInChildren<BodyPart>();
		BodyPart bp;
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			bp = componentsInChildren[i];
			int num = ExtraBodyPartTypes.FindIndex((BodyPartType item) => item == bp.bodyPartType);
			if (num >= 0)
			{
				ExtraParts[num].transform.parent = bp.transform;
				ExtraParts[num].transform.localPosition = Vector3.zero;
				ExtraParts[num].transform.localEulerAngles = Vector3.zero;
			}
		}
	}
}
