using System.Collections.Generic;
using Game;
using UnityEngine;

public class BundleListModel
{
	private static BundleListModel s_instance;

	public Dictionary<string, Bundle> Model;

	public static BundleListModel Instance
	{
		get
		{
			if (s_instance == null)
			{
				s_instance = new BundleListModel();
			}
			return s_instance;
		}
	}

	private BundleListModel()
	{
		Model = new Dictionary<string, Bundle>();
		InitWithServerData();
	}

	private void InitWithServerData()
	{
		List<BundleParameters> list = Storage.Instance.LoadBundleParameters();
		foreach (BundleParameters item in list)
		{
			BundleInfo info = Storage.Instance.LoadBundleInfo(item.Id);
			Model.Add(item.Id, new Bundle(item, info));
		}
	}

	private void InitWithDummyData()
	{
		BundleParameters bundleParameters = new BundleParameters();
		bundleParameters.Id = "PV50Beginner";
		bundleParameters.Name = "Learning the Basics";
		bundleParameters.Price = new Price(0);
		BundleInfo info = new BundleInfo();
		Bundle bundle = new Bundle(bundleParameters, info);
		BundleStage bundleStage = new BundleStage(1f);
		bundleStage.Levels.Add(new BundleLevel("GravelPitLoop", 1000, 10f, 200f, 300f));
		bundle.AddStage(bundleStage);
		bundleStage = new BundleStage(1f);
		bundleStage.Levels.Add(new BundleLevel("SuburbTutorial1", 1000, 10f, 200f, 300f));
		bundle.AddStage(bundleStage);
		Model.Add(bundle.Id, bundle);
		bundleParameters = new BundleParameters();
		bundleParameters.Id = "PV50Beach";
		bundleParameters.Name = "Career";
		bundleParameters.Price = new Price(10);
		info = new BundleInfo();
		bundle = new Bundle(bundleParameters, info);
		bundleStage = new BundleStage(0.8f);
		bundleStage.Levels.Add(new BundleLevel("GravelPitForkLift", 1000, 0.3f, 0.5f, 0.9f));
		bundle.AddStage(bundleStage);
		bundleStage = new BundleStage(0.8f);
		bundleStage.Levels.Add(new BundleLevel("GravelPitHighJump", 1000, 0.3f, 0.5f, 0.9f));
		bundle.AddStage(bundleStage);
		Model.Add(bundle.Id, bundle);
	}

	public void RefreshBundleStates()
	{
		foreach (KeyValuePair<string, Bundle> item in Model)
		{
			item.Value.InitStageUnlockAnimationState();
		}
	}

	public void UpdateBundleData(Level level)
	{
		using (Dictionary<string, Bundle>.Enumerator enumerator = Model.GetEnumerator())
		{
			while (enumerator.MoveNext() && !enumerator.Current.Value.UpdateDataForLevel(level))
			{
			}
		}
	}

	public void SetNowPlaying(Level level)
	{
		foreach (KeyValuePair<string, Bundle> item in Model)
		{
			foreach (BundleStage stage in item.Value.Stages)
			{
				if (stage.ContainsLevel(level.Parameters.Name))
				{
					item.Value.NowPlayingStage = stage;
					item.Value.NowPlayingLevel = level.Parameters.Name;
					GameController.Instance.SetLevelToLoad(level);
					GameController.Instance.CurrentBundle = item.Value;
					return;
				}
			}
		}
		Debug.LogError("SetNowPlaying failed. Invalid level id " + level.Parameters.Name);
	}

	public Bundle GetBundle(Level level)
	{
		foreach (KeyValuePair<string, Bundle> item in Model)
		{
			foreach (BundleStage stage in item.Value.Stages)
			{
				if (stage.ContainsLevel(level.Parameters.Name))
				{
					return item.Value;
				}
			}
		}
		return null;
	}

	public void Dump()
	{
		foreach (KeyValuePair<string, Bundle> item in Model)
		{
			int num = 0;
			foreach (BundleStage stage in item.Value.Stages)
			{
				num += stage.TotalTargets;
			}
		}
	}
}
