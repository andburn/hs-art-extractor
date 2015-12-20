using System.IO;
using HearthstoneDisunity.Hearthstone;
using HearthstoneDisunity.Unity;
using HearthstoneDisunity.Unity.Extract;
using HearthstoneDisunity.Util;

namespace HearthstoneDisunity
{
    public static class Extract
    {
        static Extract()
        {
            Logger.SetLogLevel(LogLevel.DEBUG);
            Logger.SetLogLocation(@"E:\Dump\_extract_test_");
        }

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

        private static void ExtractAssets(IExtractor extractor, string outDir, params string[] files)
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
                        extractor.Extract(obj, bundleDir);
                    }
                }
            }
        }
    }
}