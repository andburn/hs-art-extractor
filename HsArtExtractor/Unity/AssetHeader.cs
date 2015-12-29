using HsArtExtractor.Util;

namespace HsArtExtractor.Unity
{
	public class AssetHeader
	{
		public long MetadataSize { get; set; }

		public long FileSize { get; set; }

		public long DataOffset { get; set; }

		public byte Endianness { get; set; }

		public int AssetVersion { get; set; }

		public AssetHeader(BinaryBlock b)
		{
			MetadataSize = b.ReadInt();
			FileSize = b.ReadUnsignedInt();
			AssetVersion = b.ReadInt();
			DataOffset = b.ReadUnsignedInt();

			if(AssetVersion >= 9)
			{
				Endianness = b.ReadByte();
				b.ReadBytes(3); // unused
			}			
		}

		public override string ToString()
		{
			return string.Format("version={1} size={2} off={0}", FileSize, AssetVersion, DataOffset);
		}
	}
}
