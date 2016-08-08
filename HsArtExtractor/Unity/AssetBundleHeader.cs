using System;
using System.Collections.Generic;
using HsArtExtractor.Util;

namespace HsArtExtractor.Unity
{
	public class AssetBundleHeader
	{
		public const string SIGNATURE_WEB = "UnityWeb";
		public const string SIGNATURE_RAW = "UnityRaw";
		public const string SIGNATURE_FS = "UnityFS";

		public string Signature { get; set; }

		// 3 in Unity 3.5 and 4
		public int StreamVersion { get; set; }

		// 3.x.x for Unity 3/4
		public string UnityVersion { get; set; }

		// engine version string
		public string UnityRevision { get; set; }

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

		// the number of files in bundle
		public long NumberOfFiles { get; private set; }

		// unityfs
		public long CompressedBlockSize { get; private set; }

		// unityfs
		public long UnCompressedBlockSize { get; private set; }

		public List<AssetBundleEntry> BundleEntries = new List<AssetBundleEntry>();

		public AssetBundleHeader(BinaryBlock b)
		{
			Signature = b.ReadStringToNull();
			StreamVersion = b.ReadInt();
			UnityVersion = b.ReadStringToNull();
			UnityRevision = b.ReadStringToNull();

			if (Signature == SIGNATURE_FS)
				LoadFS(b);
			else
				LoadRAW(b);
		}

		private void LoadFS(BinaryBlock b)
		{
			CompleteFileSize = b.ReadLong();
			CompressedBlockSize = b.ReadUnsignedInt();
			UnCompressedBlockSize = b.ReadUnsignedInt();
			var flags = b.ReadUnsignedInt();
			var compression = (CompressionType)(flags & 0x3F);
			var data = ReadCompressedData(b, compression);
			HeaderSize = (int)b.BaseStream.Position;

			var blk = BinaryBlock.Create(data);
			blk.BigEndian = true;
			var guid = blk.ReadBytes(16);
			var num_blocks = blk.ReadInt();

			var blocks = new List<Tuple<int, int, int>>();
			for (int i = 0; i < num_blocks; i++)
			{
				var bcsize = blk.ReadInt();
				var busize = blk.ReadInt();
				var bflags = blk.ReadShort();
				blocks.Add(new Tuple<int, int, int>(bcsize, busize, bflags));
			}

			NumberOfFiles = blk.ReadInt();
			for (int i = 0; i < NumberOfFiles; i++)
			{
				var entry = new AssetBundleEntry() {
					Offset = blk.ReadLong(),
					Size = blk.ReadLong(),
					Status = blk.ReadInt(),
					Name = blk.ReadStringToNull()
				};
				entry.SaveData(b);
				BundleEntries.Add(entry);
			}
		}

		private void LoadRAW(BinaryBlock b)
		{
			MinimumStreamedBytes = b.ReadInt();
			HeaderSize = b.ReadInt();
			NumberOfLevelsToDownload = b.ReadInt();

			var numberOfLevels = b.ReadInt();
			if (NumberOfLevelsToDownload != numberOfLevels && NumberOfLevelsToDownload != 1)
			{
				throw new AssetException("Asset bundle has incorrect levels in the header (" + NumberOfLevelsToDownload + " != " + numberOfLevels + ")");
			}

			for (int i = 0; i < numberOfLevels; i++)
			{
				var x = b.ReadUnsignedInt();
				var y = b.ReadUnsignedInt();
				LevelByteEnd.Add(new Tuple<long, long>(x, y));
			}

			if (StreamVersion >= 2)
			{
				CompleteFileSize = b.ReadUnsignedInt();
				if (MinimumStreamedBytes > CompleteFileSize)
				{
					throw new AssetException("Asset bundle header size mismatch (" + MinimumStreamedBytes + " > " + CompleteFileSize + ")");
				}
			}

			DataHeaderSize = 0;
			if (StreamVersion >= 3)
			{
				DataHeaderSize = b.ReadUnsignedInt();
			}

			b.ReadByte(); // unused
			NumberOfFiles = b.ReadUnsignedInt();

			BundleEntries.Add(new AssetBundleEntry(b));

			Logger.Log(LogLevel.DEBUG, this);
		}

		private byte[] ReadCompressedData(BinaryBlock b, CompressionType compression)
		{
			var data = b.ReadBytes((int)CompressedBlockSize);
			if (compression == CompressionType.NONE)
				return data;

			return Compression.Decompress(data, (int)UnCompressedBlockSize, compression);
		}

		public override string ToString()
		{
			return string.Format("{0} {1}, FileSize={2}", Signature, UnityRevision, CompleteFileSize);
		}
	}
}