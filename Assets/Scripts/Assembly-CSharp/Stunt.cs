using System.Collections.Generic;
using Game;
using UnityEngine;

public class Stunt
{
	public int AchievedStayTargetCount { get; set; }

	public float? TopSpeed { get; set; }

	public float? TopHeight { get; set; }

	public Vector3 PlayerEndPosition { get; set; }

	public Vector3 VehicleEndPosition { get; set; }

	public Vector3 OutOfVehiclePosition { get; set; }

	public float TotalHPLevelAtStart { get; set; }

	public float TotalDamage { get; set; }

	public int MoneyAtStart { get; set; }

	public int HighScoreAtStart { get; set; }

	public int Coins { get; set; }

	public int Score { get; set; }

	public int DeltaCrowdMoney { get; set; }

	public int DeltaAchievementMoney
	{
		get
		{
			int score = 0;
			OldTargetsAchieved.ForEach(delegate(LevelTargetInfo x)
			{
				score += x.Score;
			});
			NewTargetsAchieved.ForEach(delegate(LevelTargetInfo x)
			{
				score += x.Bonus;
			});
			return score;
		}
	}

	public int DeltaOverallMoney
	{
		get
		{
			int num = 0;
			num += DeltaAchievementMoney;
			return num + DeltaHealthMoney;
		}
	}

	public int DeltaHealthMoney
	{
		get
		{
			return (int)TotalDamage;
		}
	}

	public List<LevelTargetInfo> OldTargetsAchieved { get; set; }

	public List<LevelTargetInfo> NewTargetsAchieved { get; set; }

	public int TargetsAchieved { get; set; }

	public int BigAirTime { get; set; }

	public int FlipCount { get; set; }

	public Stunt()
	{
		OldTargetsAchieved = new List<LevelTargetInfo>();
		NewTargetsAchieved = new List<LevelTargetInfo>();
		MoneyAtStart = GameController.Instance.Character.Coins;
		HighScoreAtStart = GameController.Instance.CurrentLevel.HighScore;
		Score = 0;
		Coins = 0;
	}
}
