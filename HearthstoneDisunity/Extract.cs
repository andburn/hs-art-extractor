using HearthstoneDisunity.Hearthstone;
using HearthstoneDisunity.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HearthstoneDisunity
{
    public static class Extract
    {
        public static void Raw(string outDir, params string[] files)
        {
            if (!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
            }

            foreach (var f in files)
            {
                if (File.Exists(f))
                {
                    AssetBundle ab = new AssetBundle(f);
                    ab.ExtractRaw(outDir);
                }
            }
        }

        public static void All(string outDir, params string[] files)
        {
            if (!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
            }

            foreach (var f in files)
            {
                if (File.Exists(f))
                {
                    AssetBundle ab = new AssetBundle(f);
                    ab.ExtractFull(outDir);
                }
            }
        }

        public static void Text(string outDir, params string[] files)
        {

        }

        public static void Textures(string outDir, params string[] files)
        {

        }

        public static void CardArt(string outDir, string hsDir, params string[] cardIds)
        {
            if (!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
            }

            foreach (var f in new string[] { hsDir })
            {
                if (File.Exists(f))
                {
                    AssetBundle ab = new AssetBundle(f);
                    Dictionary<string, List<CardArt>> map = ab.ExtractCards();
                    Console.WriteLine(map.Count);
                    foreach (var p in map)
                    {
                        Console.WriteLine(p.Key + ": " + p.Value.Count);
                    }
                }
            }

            // get all card defs (cards<n>.unity3d)

            // get all textures (cardtextures<n>.unity3d, shared<n>.unity3d)
        }

    }
}
