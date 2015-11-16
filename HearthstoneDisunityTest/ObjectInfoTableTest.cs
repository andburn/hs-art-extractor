using HearthstoneDisunity.Unity;
using HearthstoneDisunity.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HearthstoneDisunityTest
{
    [TestClass]
    public class ObjectInfoTableTest
    {
        private static ObjectInfoTable _info;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            var file = @".\data\cardtextures.unity3d";
            var abr = new AssetBundleReader();
            abr.Read(file);
            _info = abr.InfoTable;
        }

        [TestMethod]
        public void TestNumberOfObjects()
        {
            Assert.AreEqual(3, _info.Size);
        }

        [TestMethod]
        public void TestObejct_HS4022D()
        {
            Assert.IsTrue(_info.InfoMap.ContainsKey(-5083015312843089395));
            var obj = _info.InfoMap[-5083015312843089395];
            Assert.AreEqual(174848, obj.Length);
            Assert.AreEqual(28, obj.TypeId);
        }

        [TestMethod]
        public void TestObejct_HS4006D()
        {
            Assert.IsTrue(_info.InfoMap.ContainsKey(9036051721011664614));
            var obj = _info.InfoMap[9036051721011664614];
            Assert.AreEqual(349624, obj.Length);
            Assert.AreEqual(28, obj.TypeId);
        }
    }
}
