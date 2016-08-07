using HsArtExtractor.Unity;
using HsArtExtractor.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HsArtExtractorTest
{
	[TestClass]
	public class FSObjectInfoTableTest
	{
		private static ObjectInfoTable _info;

		[ClassInitialize]
		public static void ClassInitialize(TestContext context)
		{
			var file = @".\Data\bundle_5.3_fs.unity3d";
			var abr = new AssestFile(file);
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
			Assert.AreEqual(174864, obj.Length);
			Assert.AreEqual(28, obj.TypeId);
		}

		[TestMethod]
		public void TestObejct_HS4006D()
		{
			Assert.IsTrue(_info.InfoMap.ContainsKey(9036051721011664614));
			var obj = _info.InfoMap[9036051721011664614];
			Assert.AreEqual(349640, obj.Length);
			Assert.AreEqual(28, obj.TypeId);
		}
	}
}