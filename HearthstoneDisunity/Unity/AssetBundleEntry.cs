using System;
using HearthstoneDisunity.Util;

namespace HearthstoneDisunity.Unity
{
	public class AssetBundleEntry
	{
		public String Name { get; set; }
		public long Offset { get; set; }
		public long Size { get; set; }

		public AssetBundleEntry(BinaryFileReader b)
		{
			Name = b.ReadStringToNull();
			Offset = b.ReadUnsignedInt();
			Size = b.ReadUnsignedInt();
		}

		public override string ToString()
		{
			return string.Format("{0} off={1} size={2}", Name, Offset, Size);
		}
	}
}
