using HsArtExtractor.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HsArtExtractorTest
{
    [TestClass]
    public class AssetBundleHeaderTest
    {
        private static AssetBundleHeader _header;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            var file = @".\data\cardtextures.unity3d";
            var abr = new AssestFile(file);
            _header = abr.Header;
        }

        [TestMethod]
        public void TestSignature()
        {
            Assert.AreEqual("UnityRaw", _header.Signature);
        }

        [TestMethod]
        public void TestUnityVersion()
        {
            Assert.AreEqual("5.x.x", _header.UnityVersion);
        }

        [TestMethod]
        public void TestUnityEngineVersion()
        {
            Assert.AreEqual("5.1.3p2", _header.UnityRevision);
        }

        [TestMethod]
        public void TestFileSize()
        {
            Assert.AreEqual(528904, _header.CompleteFileSize);
        }

        [TestMethod]
        public void TestHeaderSize()
        {
            Assert.AreEqual(60, _header.HeaderSize);
        }

        [TestMethod]
        public void TestDataHeaderSize()
        {
            Assert.AreEqual(36, _header.DataHeaderSize);
        }
    }
}
