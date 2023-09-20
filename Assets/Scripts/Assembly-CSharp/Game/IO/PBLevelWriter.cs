using System.IO;

namespace Game.IO
{
	public class PBLevelWriter : ILevelWriter
	{
		public void WriteInfo(LevelParameters data, string path)
		{
			AJProtoBufSerializer aJProtoBufSerializer = new AJProtoBufSerializer();
			using (FileStream dest = File.Create(path))
			{
				aJProtoBufSerializer.Serialize(dest, data);
			}
		}

		public void WriteRoot(LevelRootBlock data, string path)
		{
			AJProtoBufSerializer aJProtoBufSerializer = new AJProtoBufSerializer();
			using (FileStream dest = File.Create(path))
			{
				aJProtoBufSerializer.Serialize(dest, data);
			}
		}
	}
}
