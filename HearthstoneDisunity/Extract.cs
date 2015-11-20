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
			var hsDataPath = Path.Combine(outDir, "Data", "Win");

			if(!Directory.Exists(outDir))
			{
				Directory.CreateDirectory(outDir);
			}

			foreach(var f in files)
			{
				if(File.Exists(f))
				{
					AssetBundle ab = new AssetBundle(f);
					ab.ExtractText(outDir);
				}
			}
		}

        public static void Textures(string outDir, params string[] files)
        {

        }

        // TODO: add set filter, include/exclude?
        public static void CardArt(string outDir, string hsDir)
        {
            var hsDataPath = Path.Combine(hsDir, "Data", "Win");

            if (!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
            }

            Dictionary<string, List<CardArt>> map = new Dictionary<string, List<CardArt>>();

            // get all card defs (cards<n>.unity3d)
            var cardFiles = Directory.GetFiles(hsDataPath, "cards?.unity3d");

            foreach (var cf in cardFiles)
            {
                AssetBundle ab = new AssetBundle(cf);
                var m = ab.ExtractCards(outDir);
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
                ab.ExtractCardTextures(map, outDir);
            }
        }

    }
}
