using System;
using System.Collections.Generic;
using HearthstoneDisunity.Util;

namespace HearthstoneDisunity.Unity
{
	public class AssetBundleHeader
	{
		public const String SIGNATURE_WEB = "UnityWeb";
		public const String SIGNATURE_RAW = "UnityRaw";

		public String Signature { get; set; }

		// 3 in Unity 3.5 and 4
		public int StreamVersion { get; set; }

		// 3.x.x for Unity 3/4
		public String UnityVersion { get; set; }

		// engine version string
		public String UnityRevision { get; set; }

		// minimum number of bytes to read for streamed bundles,
		// equal to completeFileSize for normal bundles
		public long MinimumStreamedBytes { get; set; }

		// offset to the bundle data or size of the bundle header
		public int HeaderSize { get; set; }

		// equal to 1 if it's a streamed bundle, number of levelX + mainData assets otherwise
		public int NumberOfLevelsToDownload { get; set; }

		// list of compressed and uncompressed offsets
		public List<Tuple<long, long>> LevelByteEnd = new List<Tuple<long, long>>();

		// equal to file size, sometimes equal to uncompressed data size without the header
		public long CompleteFileSize { get; set; }

		// offset to the first asset file within the data area? equals compressed
		// file size if completeFileSize contains the uncompressed data size
		public long DataHeaderSize { get; set; }

		public AssetBundleHeader(BinaryFileReader b)
		{
			Signature = b.ReadStringToNull();
			StreamVersion = b.ReadInt();
			UnityVersion = b.ReadStringToNull();
			UnityRevision = b.ReadStringToNull();
			MinimumStreamedBytes = b.ReadInt();
			HeaderSize = b.ReadInt();
			NumberOfLevelsToDownload = b.ReadInt();

			var numberOfLevels = b.ReadInt();
			if(NumberOfLevelsToDownload != numberOfLevels && NumberOfLevelsToDownload != 1)
			{
				throw new AssetException("Asset bundle has incorrect levels in the header (" + NumberOfLevelsToDownload + " != " +  numberOfLevels + ")");
			}

			for(int i = 0; i < numberOfLevels; i++)
			{
				var x = b.ReadUnsignedInt();
				var y = b.ReadUnsignedInt();
				LevelByteEnd.Add(new Tuple<long, long>(x, y));
			}

			if(StreamVersion >= 2)
			{
				CompleteFileSize = b.ReadUnsignedInt();
				if(MinimumStreamedBytes > CompleteFileSize)
				{
					throw new AssetException("Asset bundle header size mismatch (" + MinimumStreamedBytes + " > " +  CompleteFileSize + ")");
				}
			}

			DataHeaderSize = 0;
			if(StreamVersion >= 3)
			{
				DataHeaderSize = b.ReadUnsignedInt();
			}

			b.ReadByte();
		}

		public override string ToString()
		{
			return string.Format("{0} {1}, FileSize={2}", Signature, UnityRevision, CompleteFileSize);
		}
	}
}
