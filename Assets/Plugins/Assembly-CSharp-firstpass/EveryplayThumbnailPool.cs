using UnityEngine;

public class EveryplayThumbnailPool : MonoBehaviour
{
	public int thumbnailCount = 4;

	public int thumbnailWidth = 128;

	public bool pixelPerfect;

	public bool takeRandomShots = true;

	public TextureFormat textureFormat = TextureFormat.RGBA32;

	public bool dontDestroyOnLoad = true;

	public bool allowOneInstanceOnly = true;

	private bool initialized;

	private int currentThumbnailTextureIndex;

	private float nextRandomShotTime;

	public Texture2D[] thumbnailTextures { get; private set; }

	public int availableThumbnailCount { get; private set; }

	public float aspectRatio { get; private set; }

	public Vector2 thumbnailScale { get; private set; }

	private void Awake()
	{
		if (allowOneInstanceOnly && Object.FindObjectsOfType(GetType()).Length > 1)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		if (dontDestroyOnLoad)
		{
			Object.DontDestroyOnLoad(base.gameObject);
		}
		Everyplay.ReadyForRecording += OnReadyForRecording;
	}

	private void Start()
	{
		if (base.enabled)
		{
			Initialize();
		}
	}

	private void OnReadyForRecording(bool ready)
	{
		if (ready)
		{
			Initialize();
		}
	}

	private void Initialize()
	{
		if (!initialized && Everyplay.IsRecordingSupported())
		{
			thumbnailWidth = Mathf.Clamp(thumbnailWidth, 32, 2048);
			aspectRatio = (float)Mathf.Min(Screen.width, Screen.height) / (float)Mathf.Max(Screen.width, Screen.height);
			int num = (int)((float)thumbnailWidth * aspectRatio);
			bool flag = false;
			flag = SystemInfo.npotSupport != NPOTSupport.None;
			int num2 = Mathf.NextPowerOfTwo(thumbnailWidth);
			int num3 = Mathf.NextPowerOfTwo(num);
			thumbnailTextures = new Texture2D[thumbnailCount];
			for (int i = 0; i < thumbnailCount; i++)
			{
				thumbnailTextures[i] = new Texture2D((!flag) ? num2 : thumbnailWidth, (!flag) ? num3 : num, textureFormat, false);
				thumbnailTextures[i].wrapMode = TextureWrapMode.Clamp;
			}
			currentThumbnailTextureIndex = 0;
			Everyplay.SetThumbnailTargetTextureId(thumbnailTextures[currentThumbnailTextureIndex].GetNativeTextureID());
			if (flag)
			{
				Everyplay.SetThumbnailTargetTextureWidth(thumbnailWidth);
				Everyplay.SetThumbnailTargetTextureHeight(num);
				thumbnailScale = new Vector2(1f, 1f);
			}
			else if (pixelPerfect)
			{
				Everyplay.SetThumbnailTargetTextureWidth(thumbnailWidth);
				Everyplay.SetThumbnailTargetTextureHeight(num);
				thumbnailScale = new Vector2((float)thumbnailWidth / (float)num2, (float)num / (float)num3);
			}
			else
			{
				Everyplay.SetThumbnailTargetTextureWidth(num2);
				Everyplay.SetThumbnailTargetTextureHeight(num3);
				thumbnailScale = new Vector2(1f, 1f);
			}
			Everyplay.ThumbnailReadyAtTextureId += OnThumbnailReady;
			Everyplay.RecordingStarted += OnRecordingStarted;
			initialized = true;
		}
	}

	private void OnRecordingStarted()
	{
		availableThumbnailCount = 0;
		currentThumbnailTextureIndex = 0;
		Everyplay.SetThumbnailTargetTextureId(thumbnailTextures[currentThumbnailTextureIndex].GetNativeTextureID());
		if (takeRandomShots)
		{
			Everyplay.TakeThumbnail();
			nextRandomShotTime = Time.time + Random.Range(3f, 15f);
		}
	}

	private void Update()
	{
		if (takeRandomShots && Everyplay.IsRecording() && !Everyplay.IsPaused() && Time.time > nextRandomShotTime)
		{
			Everyplay.TakeThumbnail();
			nextRandomShotTime = Time.time + Random.Range(3f, 15f);
		}
	}

	private void OnThumbnailReady(int id, bool portrait)
	{
		if (thumbnailTextures[currentThumbnailTextureIndex].GetNativeTextureID() == id)
		{
			currentThumbnailTextureIndex++;
			if (currentThumbnailTextureIndex >= thumbnailTextures.Length)
			{
				currentThumbnailTextureIndex = 0;
			}
			if (availableThumbnailCount < thumbnailTextures.Length)
			{
				availableThumbnailCount++;
			}
			Everyplay.SetThumbnailTargetTextureId(thumbnailTextures[currentThumbnailTextureIndex].GetNativeTextureID());
		}
	}

	private void OnDestroy()
	{
		Everyplay.ReadyForRecording -= OnReadyForRecording;
		if (!initialized)
		{
			return;
		}
		Everyplay.SetThumbnailTargetTextureId(0);
		Everyplay.RecordingStarted -= OnRecordingStarted;
		Everyplay.ThumbnailReadyAtTextureId -= OnThumbnailReady;
		Texture2D[] array = thumbnailTextures;
		foreach (Texture2D texture2D in array)
		{
			if (texture2D != null)
			{
				Object.Destroy(texture2D);
			}
		}
		thumbnailTextures = null;
		initialized = false;
	}
}
