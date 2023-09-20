using UnityEngine;

namespace Game
{
	public class PlayerStatus : MonoBehaviour
	{
		private BodyPart[] bodyParts;

		public Player player;

		private void Start()
		{
			bodyParts = new BodyPart[14];
			RegisterBodyParts(base.transform.parent);
			player = base.gameObject.GetComponent<Player>();
		}

		private void RegisterBodyParts(Transform node)
		{
			foreach (Transform item in node)
			{
				BodyPart component = item.gameObject.GetComponent<BodyPart>();
				if ((bool)component)
				{
					bodyParts[(int)component.bodyPartType] = component;
					component.Impact += ImpactHandler;
				}
				RegisterBodyParts(item);
			}
		}

		public BodyPart GetBodyPart(BodyPartType id)
		{
			return bodyParts[(int)id];
		}

		private void ImpactHandler(object sender, ImpactEventArgs e)
		{
			if (e.collidingObject.tag != "Vehicle")
			{
				float num = 8f;
				int num2 = (int)(e.impactMagnitude / num * 2f);
				if (num2 > 2)
				{
					num2 = 2;
				}
				base.gameObject.GetComponent<PlayerSounds>().playPainSound(num2);
			}
			player.currentState.CollisionEnter(e.bodyPartType, e.impactMagnitude, e.collidingObject);
		}

		public bool IsContact()
		{
			for (int i = 0; i < bodyParts.GetLength(0); i++)
			{
				if (bodyParts[i].IsContact())
				{
					return true;
				}
			}
			return false;
		}

		public float GetTotalHPLevel()
		{
			return 0f;
		}
	}
}
