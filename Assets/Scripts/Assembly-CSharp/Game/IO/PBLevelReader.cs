using System;
using System.IO;
using UnityEngine;

namespace Game.IO
{
	public class PBLevelReader : ILevelReader
	{
		public LevelRootBlock ReadRoot(TextAsset data)
		{
			AJProtoBufSerializer aJProtoBufSerializer = new AJProtoBufSerializer();
			LevelRootBlock levelRootBlock = null;
			using (Stream source = new MemoryStream(data.bytes))
			{
				return aJProtoBufSerializer.Deserialize(source, null, typeof(LevelRootBlock)) as LevelRootBlock;
			}
		}

		public LevelParameters ReadInfo(TextAsset data)
		{
			AJProtoBufSerializer aJProtoBufSerializer = new AJProtoBufSerializer();
			LevelParameters levelParameters = null;
			using (Stream source = new MemoryStream(data.bytes))
			{
				return aJProtoBufSerializer.Deserialize(source, null, typeof(LevelParameters)) as LevelParameters;
			}
		}

		public LevelParameters ReadInfo(string path)
		{
			LevelParameters result = null;
			try
			{
				FileStream fileStream = File.OpenRead(path);
				AJProtoBufSerializer aJProtoBufSerializer = new AJProtoBufSerializer();
				result = aJProtoBufSerializer.Deserialize(fileStream, null, typeof(LevelParameters)) as LevelParameters;
				fileStream.Close();
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.ToString());
			}
			return result;
		}
	}
}
