using Game;
using UnityEngine;

public class CoinsUpdater : MonoBehaviour
{
	private NumberLabelAnimator label;

	private ParticleSystem glowEffect;

	private void Start()
	{
		glowEffect = GetComponentInChildren<ParticleSystem>();
		label = GetComponent<NumberLabelAnimator>();
		label.SkipTo(GameController.Instance.Character.Coins, false);
	}

	private void Update()
	{
		if (label.targetValue != GameController.Instance.Character.Coins)
		{
			if (label.targetValue < GameController.Instance.Character.Coins && (bool)glowEffect)
			{
				glowEffect.Play();
			}
			label.targetValue = GameController.Instance.Character.Coins;
		}
	}
}
