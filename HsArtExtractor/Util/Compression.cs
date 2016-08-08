using System;
using LZ4;

namespace HsArtExtractor.Util
{
	public class Compression
	{
		public static byte[] Decompress(byte[] data, int size, CompressionType compression)
		{
			if (compression == CompressionType.LZ4 || compression == CompressionType.LZ4HC)
				return LZ4Codec.Decode(data, 0, data.Length, size);
			else
				throw new NotImplementedException($"Unimplemented compression method: {compression}");
		}
	}

	public enum CompressionType
	{
		NONE,
		LZMA,
		LZ4,
		LZ4HC,
		LZHAM
	}
}