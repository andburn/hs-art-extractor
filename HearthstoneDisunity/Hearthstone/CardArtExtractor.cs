using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using HearthstoneDisunity.Hearthstone.Bundle;
using HearthstoneDisunity.Hearthstone.Database;
using HearthstoneDisunity.Unity;
using HearthstoneDisunity.Unity.Extract;
using HearthstoneDisunity.Util;
using ArtCard = HearthstoneDisunity.Hearthstone.CardArt.Card;
using GameCard = HearthstoneDisunity.Hearthstone.Card;

namespace HearthstoneDisunity.Hearthstone
{
    internal class CardArtExtractor
    {
        private string _hsPath;
        private string _hsDataPath;
        private string _outDir;
        private string _outDirRaw;
        private string _outDirPng;

        public CardArtExtractor(string outDir, string hsDir)
        {
            Logger.Log("Initializing CardArtOld ({0} to {1})", hsDir, outDir);

            _hsDataPath = Path.Combine(hsDir, "Data", "Win");
            _hsPath = hsDir;

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
            Dictionary<string, GameCard> cardDb = LoadCardDb();

            var defs = new CardArt.CardArtDefs();
            defs.Patch = GetPatchVersion(_hsPath);
            defs.Cards = LoadCards();
            // TODO: remove
            CardArt.CardArtDb.Write(Path.Combine(_outDir, "CardArtDefs.xml"), defs);

            // get all textures (cardtextures<n>.unity3d, shared<n>.unity3d)
            var textureFiles = new List<string>(Directory.GetFiles(_hsDataPath, "cardtextures?.unity3d"));
            textureFiles.AddRange(Directory.GetFiles(_hsDataPath, "shared?.unity3d"));

            foreach (var tf in textureFiles)
            {
                AssestFile ab = new AssestFile(tf);
                TexturesBundle tb = new TexturesBundle(ab, _outDirRaw);
            }

            //var ddsList = Directory.GetFiles(_outDirRaw, "*.dds");
            //foreach (var ddsFile in ddsList)
            //{
            //    var id = StringUtils.GetFilenameNoExt(ddsFile);
            //    var bmp = DDS.LoadImage(ddsFile, false);
            //    bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            //    bmp.Save(Path.Combine(_outDirPng, id + ".png"));
            //}
        }

        //
        private Dictionary<string, long> LoadTextureInfo(List<ObjectData> objects)
        {
            Dictionary<string, long> info = new Dictionary<string, long>();

            return info;
        }

        // Load the card art info from "cards?.unity3d"
        private List<ArtCard> LoadCards()
        {
            List<ArtCard> cards = new List<ArtCard>();

            // get all card defs (cards<n>.unity3d)
            var cardFiles = Directory.GetFiles(_hsDataPath, "cards?.unity3d");

            foreach (var cf in cardFiles)
            {
                // create a standard bundle
                AssestFile ab = new AssestFile(cf);
                // process the bundle as a HS card bundle
                CardsBundle cb = new CardsBundle(ab);
                // add this bundles cards to the full list
                cards.AddRange(cb.Cards);
            }

            return cards;
        }

        // Load a simple card db from "cardxml0.unity3d"
        private Dictionary<string, GameCard> LoadCardDb()
        {
            Dictionary<string, GameCard> cardDb = new Dictionary<string, Card>();
            // Extract card xml files
            var cardXml = Path.Combine(_hsDataPath, "cardxml0.unity3d");
            if (File.Exists(cardXml))
            {
                AssestFile xmlBundle = new AssestFile(cardXml);
                var extractor = new TextExtractor();
                foreach (var obj in xmlBundle.Objects)
                {
                    extractor.Extract(obj, _outDir);
                }
                // Create cardDB from english xml
                var xmlFile = Path.Combine(_outDir, "TextAsset", "enUS.txt");
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
            }
            else
            {
                Logger.Log(LogLevel.ERROR, "{0} does not exist.", cardXml);
            }
            return cardDb;
        }

        // Quick way to grab the HS patch version.
        private string GetPatchVersion(string hsDir)
        {
            // TODO: don't know how reliable existence of this file is.
            var agentFile = Path.Combine(hsDir, ".agent.db");
            var localRegex = new Regex("\"local_version\" : \"([0-9\\.]+)\"");
            try
            {
                if (File.Exists(agentFile))
                {
                    var text = File.ReadAllText(agentFile);
                    var match = localRegex.Match(text);
                    if (match.Success)
                        return match.Groups[1].Captures[0].Value;
                }
                else
                {
                    Logger.Log(LogLevel.WARN, "Version not found, missing ({0})", agentFile);
                }
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.ERROR, "GetVersion failed. {0}", e.Message);
            }
            return "0.0.0.0";
        }
    }
}