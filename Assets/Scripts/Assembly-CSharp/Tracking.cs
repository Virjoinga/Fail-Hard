using System;
using UnityEngine;

public class Tracking
{
	public enum Events
	{
		StuntStart = 0,
		StuntEnd = 1,
		TargetAchieved = 2,
		VehicleUpgrade = 3,
		VechicleUpgradeSkipped = 4,
		VechicleState = 5,
		LevelCompleted = 6,
		IAPPurchase = 7,
		IAPPurchaseFailed = 8,
		EnteringLevel = 9,
		ItemPurchased = 10,
		GetJarVoucherRedeemed = 11,
		SupersonicAdWatched = 12,
		ApplifierAdWatched = 13
	}

	public static void DesignEvent(Events evnt, float value, Vector3? position = null)
	{
		if (!position.HasValue)
		{
			GA.API.Design.NewEvent(Enum.GetName(typeof(Events), evnt), value);
		}
		else
		{
			GA.API.Design.NewEvent(Enum.GetName(typeof(Events), evnt), value, position.Value);
		}
	}

	public static void DesignEvent(Events evnt, string subCategory)
	{
		GA.API.Design.NewEvent(EventName(evnt, subCategory));
	}

	public static void DesignEvent(Events evnt, string subCategory, float value)
	{
		GA.API.Design.NewEvent(EventName(evnt, subCategory), value);
	}

	public static void DesignEvent(Events evnt, params string[] categories)
	{
		GA.API.Design.NewEvent(EventName(evnt, categories));
	}

	private static string EventName(Enum evnt, string subCategory = null)
	{
		string text = Enum.GetName(typeof(Events), evnt);
		if (!string.IsNullOrEmpty(subCategory))
		{
			text = text + ":" + subCategory;
		}
		return text;
	}

	private static string EventName(Enum evnt, string[] categories)
	{
		string name = Enum.GetName(typeof(Events), evnt);
		return name + ":" + string.Join(":", categories);
	}

	public static void BusinessEvent(Events evnt, string subCategory, int amount)
	{
		GA.API.Business.NewEvent(EventName(evnt, subCategory), "coins", amount);
	}

	public static void BusinessEvent(Events evnt, string subCategory, string currencyCode, int amount)
	{
		GA.API.Business.NewEvent(EventName(evnt, subCategory), currencyCode, amount);
	}
}
