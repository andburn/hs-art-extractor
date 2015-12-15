using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using HearthstoneDisunity.Hearthstone.Bundle;
using HearthstoneDisunity.Hearthstone.Database;
using HearthstoneDisunity.Unity;
using HearthstoneDisunity.Unity.Extract;
using HearthstoneDisunity.Unity.Objects;
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
            // TODO: may not need this as dictionary
            Dictionary<string, GameCard> cardDb = LoadCardDb();

            var defs = new CardArt.CardArtDefs();
            defs.Patch = GetPatchVersion(_hsPath);
            defs.Cards = LoadCards();
            // TODO: remove
            CardArt.CardArtDb.Write(Path.Combine(_outDir, "CardArtDefs.xml"), defs);
            // Filter CardArts
            List<ArtCard> ucards = defs.Cards.Where(x => cardDb.ContainsKey(x.Id)).ToList();

            // get all textures (cardtextures<n>.unity3d, shared<n>.unity3d)
            var textureFiles = new List<string>(Directory.GetFiles(_hsDataPath, "cardtextures?.unity3d"));
            //textureFiles.AddRange(Directory.GetFiles(_hsDataPath, "shared?.unity3d"));

            // First pass over texture bundles, collect texture path data
            var textureRefs = LoadTextureInfo(textureFiles);

            // Add shared bundles to texture list
            textureFiles.AddRange(Directory.GetFiles(_hsDataPath, "shared?.unity3d"));
            // Load all the texture files using the refs
            LoadTextures(textureFiles, textureRefs);
        }

        // First pass over texture bundles, collect texture path data
        private Dictionary<long, string> LoadTextureInfo(List<string> textureFiles)
        {
            var textureRefs = new Dictionary<long, string>();
            foreach (var file in textureFiles)
            {
                AssestFile af = new AssestFile(file);
                foreach (var obj in af.Objects)
                {
                    var unityClass = (UnityClass)obj.Info.ClassId;
                    // only interested in AssetBundle class, on this pass to collect refs
                    if (unityClass == UnityClass.AssetBundle)
                    {
                        var data = BinaryBlock.Create(obj.Buffer);
                        var ab = new AssetBundle(data);
                        foreach (var kp in ab.Container)
                        {
                            var text = kp.Key;
                            var id = kp.Value.PathID;
                            if (!textureRefs.ContainsKey(id))
                            {
                                textureRefs[id] = text;
                            }
                            else
                            {
                                Logger.Log("Path entry duplicated: " + text + " (" + id + ")");
                            }
                        }
                    }
                }
            }
            Logger.Log("{0} tex refs found.", textureRefs.Count);

            return textureRefs;
        }

        // Second pass over all texture bundles, guided by textureRefs from pass one
        private void LoadTextures(List<string> textureFiles, Dictionary<long, string> textureRefs)
        {
            var countFound = 0;
            foreach (var tf in textureFiles)
            {
                AssestFile af = new AssestFile(tf);

                foreach (var obj in af.Objects)
                {
                    // check to see if obj id is found in texrefs
                    var unityClass = (UnityClass)obj.Info.ClassId;
                    if (unityClass == UnityClass.Texture2D && textureRefs.ContainsKey(obj.Id))
                    {
                        countFound++;
                        var refPath = textureRefs[obj.Id];
                        var refName = StringUtils.GetFilenameNoExt(refPath).ToUpper();
                        var refCardId = StringUtils.GetFilePathParentDir(refPath).ToUpper();
                        Texture2D tex = new Texture2D(BinaryBlock.Create(obj.Buffer));
                        if (tex.Name.ToUpper() == refName)
                        {
                            tex.Save(_outDirRaw, refCardId);
                        }
                        else
                        {
                            Logger.Log("TexName mismatch: {0} != {1}", tex.Name, refName);
                        }
                    }
                }
            }
            Logger.Log("Refs matched: " + countFound);
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

            Logger.Log("ArtCards loaded: {0} cards", cards.Count);
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
                    cardDb = CardDb.Filtered;
                    Logger.Log("CardDB loaded: {0} cards (filtered)", cardDb.Count);
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