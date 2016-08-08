using HsArtExtractor.Util;

namespace HsArtExtractor.Unity
{
	public class AssetBundleEntry
	{
		public string Name { get; set; }
		public long Offset { get; set; }
		public long Size { get; set; }
		public int Status { get; set; }
		public BinaryBlock Data { get; set; }

		public AssetBundleEntry()
		{
		}

		public AssetBundleEntry(BinaryBlock b)
		{
			Name = b.ReadStringToNull();
			Offset = b.ReadUnsignedInt();
			Size = b.ReadUnsignedInt();
			Data = b;

			Logger.Log(LogLevel.DEBUG, this);
		}

		public override string ToString()
		{
			return string.Format("{0} off={1} size={2}", Name, Offset, Size);
		}

		internal void SaveData(BinaryBlock b)
		{
			b.Seek(b.BaseStream.Position + Offset);
			var rin = b.ReadBytes((int)Size);
			Data = BinaryBlock.Create(rin);
			Data.BigEndian = true;
			Data.Seek(0);
		}
	}
}