using HsArtExtractor.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HsArtExtractorTest
{
	[TestClass]
	public class AssetHeaderTest
	{
		private static AssetHeader _header;

		[ClassInitialize]
		public static void ClassInitialize(TestContext context)
		{
			var file = @".\Data\cardtextures.unity3d";
			var abr = new AssestFile(file);
			_header = abr.AssetHeader;
		}

		[TestMethod]
		public void TestAssetVersion()
		{
			Assert.AreEqual(15, _header.AssetVersion);
		}

		[TestMethod]
		public void TestFileSize()
		{
			Assert.AreEqual(528808, _header.FileSize);
		}

		[TestMethod]
		public void TestDataOffset()
		{
			Assert.AreEqual(4096, _header.DataOffset);
		}

		[TestMethod]
		public void TestEndianess()
		{
			Assert.AreEqual(0, _header.Endianness);
		}

		[TestMethod]
		public void TestMetadataSize()
		{
			Assert.AreEqual(2234, _header.MetadataSize);
		}
	}
}