using System.Collections.Generic;
using System.IO;
using HsArtExtractor.Hearthstone;
using HsArtExtractor.Unity;
using HsArtExtractor.Unity.Extract;

namespace HsArtExtractor
{
	public static class Extract
	{
		public static void Raw(string outDir, params string[] files)
		{
			ExtractAssets(new RawExtractor(), outDir, files);
		}

		public static void Text(string outDir, params string[] files)
		{
			ExtractAssets(new TextExtractor(), outDir, files);
		}

		public static void Textures(string outDir, params string[] files)
		{
			ExtractAssets(new TextureExtractor(), outDir, files);
		}

		public static void CardArt(string outDir, string hsDir, int setId = -1)
		{
			var extractor = new CardArtExtractor(outDir, hsDir, setId);
			extractor.Extract();
		}

		// TODO this could be done better
		public static void Multiple(bool raw, bool text, bool texture,
			string outDir, params string[] files)
		{
			List<IExtractor> extractors = new List<IExtractor>();
			if (raw)
				extractors.Add(new RawExtractor());
			if (text)
				extractors.Add(new TextExtractor());
			if (texture)
				extractors.Add(new TextureExtractor());

			ExtractAssets(extractors, outDir, files);
		}

		private static void ExtractAssets(IExtractor extractor, string outDir, string[] files)
		{
			ExtractAssets(new List<IExtractor>() { extractor }, outDir, files);
		}

		private static void ExtractAssets(IEnumerable<IExtractor> extractors, string outDir, string[] files)
		{
			if (!Directory.Exists(outDir))
			{
				Directory.CreateDirectory(outDir);
			}

			foreach (var f in files)
			{
				if (File.Exists(f))
				{
					AssestFile bundle = new AssestFile(f);
					var bundleDir = Path.Combine(outDir, bundle.FlieName);
					Directory.CreateDirectory(bundleDir);
					foreach (var obj in bundle.Objects)
					{
						foreach (var ex in extractors)
						{
							ex.Extract(obj, bundleDir);
						}
					}
				}
			}
		}
	}
}