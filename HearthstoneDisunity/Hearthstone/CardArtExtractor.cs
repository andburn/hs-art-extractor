using System.Collections.Generic;
using System.Drawing;
using System.IO;
using AndBurn.DDSReader;
using HearthstoneDisunity.Hearthstone.Bundle;
using HearthstoneDisunity.Hearthstone.Xml;
using HearthstoneDisunity.Unity;
using HearthstoneDisunity.Unity.Extract;
using HearthstoneDisunity.Util;

namespace HearthstoneDisunity.Hearthstone
{
    internal class CardArtExtractor
    {
        private string _hsDataPath;
        private string _outDir;
        private string _outDirRaw;
        private string _outDirPng;

        public CardArtExtractor(string outDir, string hsDir)
        {
            Logger.Log("Initializing CardArt ({0} to {1})", hsDir, outDir);

            _hsDataPath = Path.Combine(hsDir, "Data", "Win");

            if (!Directory.Exists(_hsDataPath))
            {
                throw new DirectoryNotFoundException("The Hearthstone directory does not exist (" + _hsDataPath + ")");
            }

            _outDir = outDir;

            if (!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
            }

            _outDirRaw = Path.Combine(outDir, "Raw");
            Directory.CreateDirectory(_outDirRaw);

            _outDirPng = Path.Combine(outDir, "Png");
            Directory.CreateDirectory(_outDirPng);
        }

        public void Extract()
        {
            Dictionary<string, Card> cardDb = LoadCardDb();
            Dictionary<string, List<CardArt>> cardArtMap = LoadCards();

            // get all textures (cardtextures<n>.unity3d, shared<n>.unity3d)
            var textureFiles = new List<string>(Directory.GetFiles(_hsDataPath, "cardtextures?.unity3d"));
            textureFiles.AddRange(Directory.GetFiles(_hsDataPath, "shared?.unity3d"));

            foreach (var tf in textureFiles)
            {
                AssetBundle ab = new AssetBundle(tf);
                TexturesBundle tb = new TexturesBundle(ab, cardArtMap);
                tb.Extract(_outDirRaw);
            }

            var ddsList = Directory.GetFiles(_outDirRaw, "*.dds");
            foreach (var ddsFile in ddsList)
            {
                var id = StringUtils.GetFilenameNoExt(ddsFile);
                var bmp = DDS.LoadImage(ddsFile, false);
                bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
                bmp.Save(Path.Combine(_outDirPng, id + ".png"));
            }
        }

        private Dictionary<string, List<CardArt>> LoadCards()
        {
            Dictionary<string, List<CardArt>> map = new Dictionary<string, List<CardArt>>();

            // get all card defs (cards<n>.unity3d)
            var cardFiles = Directory.GetFiles(_hsDataPath, "cards?.unity3d");

            foreach (var cf in cardFiles)
            {
                AssetBundle ab = new AssetBundle(cf);
                CardsBundle cb = new CardsBundle(ab);

                var bundleMap = cb.CardArtByTexture;
                // Use this to join possible reuse of tex over the x bundles
                foreach (var entry in bundleMap)
                {
                    if (!map.ContainsKey(entry.Key))
                    {
                        map[entry.Key] = new List<CardArt>();
                    }
                    map[entry.Key].AddRange(entry.Value);
                }
            }

            return map;
        }

        private Dictionary<string, Card> LoadCardDb()
        {
            Dictionary<string, Card> cardDb = new Dictionary<string, Card>();

            // Extract card xml files
            var cardXml = Path.Combine(_hsDataPath, "cardxml0.unity3d");
            AssetBundle xmlBundle = new AssetBundle(cardXml);
            var extractor = new TextExtractor();
            foreach (var obj in xmlBundle.Objects)
            {
                extractor.Extract(obj, _outDir);
            }
            // Create cardDB from english xml
            var xmlFile = Path.Combine(_outDir, "cardxml0", "TextAsset", "enUS.xml");
            if (File.Exists(xmlFile))
            {
                CardDb.Read(xmlFile);
                cardDb = CardDb.All;
                Logger.Log("CardDB loaded: {0} cards", cardDb.Count);
            }
            else
            {
                Logger.Log(LogLevel.ERROR, "Card XML file not extracted: {0}", xmlFile);
            }
            return cardDb;
        }
    }
}