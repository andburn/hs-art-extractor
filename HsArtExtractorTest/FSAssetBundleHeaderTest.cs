using HsArtExtractor.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HsArtExtractorTest
{
	[TestClass]
	public class FSAssetBundleHeaderTest
	{
		private static AssetBundleHeader _header;

		[ClassInitialize]
		public static void ClassInitialize(TestContext context)
		{
			var file = @".\Data\bundle_5.3_fs.unity3d";
			var abr = new AssestFile(file);
			_header = abr.Header;
		}

		[TestMethod]
		public void TestSignature()
		{
			Assert.AreEqual("UnityFS", _header.Signature);
		}

		[TestMethod]
		public void TestUnityVersion()
		{
			Assert.AreEqual("5.x.x", _header.UnityVersion);
		}

		[TestMethod]
		public void TestUnityEngineVersion()
		{
			Assert.AreEqual("5.3.4p4", _header.UnityRevision);
		}

		[TestMethod]
		public void TestFileSize()
		{
			Assert.AreEqual(528974, _header.CompleteFileSize);
		}

		[TestMethod]
		public void TestHeaderSize()
		{
			Assert.AreEqual(110, _header.HeaderSize);
		}

		[TestMethod]
		public void TestNumberOfFiles()
		{
			Assert.AreEqual(1, _header.NumberOfFiles);
		}

		[TestMethod]
		public void TestCompressedBlockSize()
		{
			Assert.AreEqual(64, _header.CompressedBlockSize);
		}

		[TestMethod]
		public void TestUnCompressedBlockSize()
		{
			Assert.AreEqual(91, _header.UnCompressedBlockSize);
		}
	}
}