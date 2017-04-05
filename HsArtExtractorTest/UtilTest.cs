using System;
using HsArtExtractor.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HsArtExtractorTest
{
	[TestClass]
	public class UtilTest
	{
		[TestMethod]
		public void TestMakeIDCorrectSize()
		{
			var id = StringUtils.MakeID("DDS1");
			Assert.AreEqual(827540548, id);
		}

		[TestMethod]
		public void TestMakeIDWrongSize()
		{
			try
			{
				StringUtils.MakeID("DDS");
				Assert.Fail();
			}
			catch (ArgumentException)
			{
				Assert.IsTrue(true);
			}
		}

		[TestMethod]
		public void Test_FilenameNoExt()
		{
			var result = StringUtils.GetFilenameNoExt("assets/abc/card.psd");
			Assert.AreEqual("card", result);
		}

		[TestMethod]
		public void Test_FilenameNoExt_Error()
		{
			var result = StringUtils.GetFilenameNoExt("assets/abc/\0");
			Assert.IsNull(result);
		}

		[TestMethod]
		public void Test_FilePathParentDir()
		{
			var result = StringUtils.GetFilePathParentDir("assets/abc/card.psd");
			Assert.AreEqual("abc", result);
		}

		[TestMethod]
		public void Test_FilePathParentDir_Error()
		{
			var result = StringUtils.GetFilePathParentDir("assets/abc/\0");
			Assert.IsNull(result);
		}
	}
}