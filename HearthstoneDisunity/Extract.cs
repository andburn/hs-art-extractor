using System;
using System.Collections.Generic;
using System.IO;
using HearthstoneDisunity.Hearthstone;
using HearthstoneDisunity.Hearthstone.Bundle;
using HearthstoneDisunity.Hearthstone.Xml;
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

        // TODO: add set filter, include/exclude?
        public static void CardArt(string outDir, string hsDir)
        {
            var hsDataPath = Path.Combine(hsDir, "Data", "Win");

            if (!Directory.Exists(hsDataPath))
            {
                throw new DirectoryNotFoundException("The Hearthstone directory does not exist (" + hsDataPath + ")");
            }

            if (!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
            }

            Dictionary<string, List<CardArt>> map = new Dictionary<string, List<CardArt>>();

            // extract card xml
            var cardXml = Path.Combine(hsDataPath, "cardxml0.unity3d");
            AssetBundle xmlBundle = new AssetBundle(cardXml);
            // create a temp dir
            var xmlDir = Directory.CreateDirectory(Path.Combine(outDir, "cardxml"));
            //xmlBundle.ExtractText(xmlDir.FullName);
            // TODO: check it exists
            CardDb.Read(Path.Combine(xmlDir.FullName, "enUS.txt"));
            var cards = CardDb.All;
            Console.WriteLine("CardDB loaded: " + cards.Count);

            // get all card defs (cards<n>.unity3d)
            var cardFiles = Directory.GetFiles(hsDataPath, "cards?.unity3d");

            foreach (var cf in cardFiles)
            {
                AssetBundle ab = new AssetBundle(cf);
                CardsBundle cb = new CardsBundle(ab);

                var m = cb.CardArtByTexture;
                //ab.ExtractCards(outDir);
                // Use this to join possible reuse of tex over bundles
                foreach (var entry in m)
                {
                    if (!map.ContainsKey(entry.Key))
                    {
                        map[entry.Key] = new List<CardArt>();
                    }
                    map[entry.Key].AddRange(entry.Value);
                }
            }

            // get all textures (cardtextures<n>.unity3d, shared<n>.unity3d)
            var textureFiles = new List<string>(Directory.GetFiles(hsDataPath, "cardtextures?.unity3d"));
            textureFiles.AddRange(Directory.GetFiles(hsDataPath, "shared?.unity3d"));

            foreach (var tf in textureFiles)
            {
                AssetBundle ab = new AssetBundle(tf);
                TexturesBundle tb = new TexturesBundle(ab, map);
                tb.Extract(outDir);
            }
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
                    AssetBundle bundle = new AssetBundle(f);
                    var bundleDir = Path.Combine(outDir, bundle.BundleFileName);
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