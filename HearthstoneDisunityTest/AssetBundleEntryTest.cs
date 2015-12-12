using Microsoft.VisualStudio.TestTools.UnitTesting;
using HearthstoneDisunity.Unity;

namespace HearthstoneDisunityTest
{
    [TestClass]
    public class AssetBundleEntryTest
    {
        private static AssetBundleEntry _entry;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            var file = @".\data\cardtextures.unity3d";
            var abr = new AssestFile(file);
            _entry = abr.BundleEntry;
        }

        [TestMethod]
        public void TestName()
        {
            Assert.AreEqual("CAB-cardtexturesesES0", _entry.Name);
        }

        [TestMethod]
        public void TestOffset()
        {
            Assert.AreEqual(36, _entry.Offset);
        }

        [TestMethod]
        public void TestSize()
        {
            Assert.AreEqual(528808, _entry.Size);
        }
    }
}
