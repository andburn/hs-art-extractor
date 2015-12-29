using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HsArtExtractor.Unity;
using HsArtExtractor.Util;

namespace HsArtExtractorTest
{
    [TestClass]
    public class TypeTreeTest
    {
        private static TypeTree _tree;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            var file = @".\data\cardtextures.unity3d";
            var abr = new AssestFile(file);
            _tree = abr.TypeTree;
        }

        [TestMethod]
        public void TestAttributes()
        {
            Assert.AreEqual(5, _tree.Attributes);
        }

        [TestMethod]
        public void TestEmbedded()
        {
            Assert.IsTrue(_tree.Embedded);
        }

        [TestMethod]
        public void TestTypeMap()
        {
            Assert.AreEqual(2, _tree.TypeMap.Count);
        }

        [TestMethod]
        public void TestTypeMapTypes()
        {
            // AssetBundle
            Assert.IsTrue(_tree.TypeMap.ContainsKey(142));
            // Texture2D
            Assert.IsTrue(_tree.TypeMap.ContainsKey(28));
        }
    }
}
