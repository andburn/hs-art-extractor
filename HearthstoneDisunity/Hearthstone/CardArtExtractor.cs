using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using HearthstoneDisunity.Hearthstone.Bundle;
using HearthstoneDisunity.Hearthstone.CardArt;
using HearthstoneDisunity.Hearthstone.Database;
using HearthstoneDisunity.Unity;
using HearthstoneDisunity.Unity.Extract;
using HearthstoneDisunity.Unity.Objects;
using HearthstoneDisunity.Util;

namespace HearthstoneDisunity.Hearthstone
{
    internal class CardArtExtractor
    {
        private string _hsPath;
        private string _hsDataPath;
        private string _outDir;
        private int _set;

        public CardArtExtractor(string outDir, string hsDir, int setId)
        {
            Logger.Log("Initializing CardArtOld ({0} to {1})", hsDir, outDir);

            _hsDataPath = Path.Combine(hsDir, "Data", "Win");
            _hsPath = hsDir;
            _set = setId;

            if (!Directory.Exists(_hsDataPath))
            {
                throw new DirectoryNotFoundException("The Hearthstone directory does not exist (" + _hsDataPath + ")");
            }

            _outDir = outDir;

            if (!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
            }
        }

        public void Extract()
        {
            // Load selected card data
            // TODO: why is this a dictionary
            Dictionary<string, Card> cardDb = LoadCardDb();
            // Load card art data
            var defs = new CardArt.CardArtDefs();
            defs.Patch = GetPatchVersion();
            defs.Cards = LoadCards();
            // TODO: remove
            CardArt.CardArtDb.Write(Path.Combine(_outDir, "CardArtDefs.xml"), defs);
            // Filter CardArts, only those cards in the cardDb

            List<ArtCard> filteredCards = defs.Cards.Where(x => cardDb.ContainsKey(x.Id)).ToList();
            if (_set >= 0)
            {
                var setDb = cardDb.Where(x => (int)x.Value.Set == _set).ToDictionary(x => x.Key, y => y.Value);
                filteredCards = defs.Cards.Where(x => setDb.ContainsKey(x.Id)).ToList();
            }
            Logger.Log("Filtered art cards: " + filteredCards.Count);

            // get all textures (cardtextures<n>.unity3d)
            var textureFiles = new List<string>(Directory.GetFiles(_hsDataPath, "cardtextures?.unity3d"));
            // First pass over texture bundles, collect texture path data
            var textureRefs = LoadTextureInfo(textureFiles);
            // Add shared bundles to texture list (shared<n>.unity3d)
            textureFiles.AddRange(Directory.GetFiles(_hsDataPath, "shared?.unity3d"));
            // Extract all the texture files using the refs and required cards
            var tb = new TexturesBundle(textureFiles, _outDir);
            tb.Extract(textureRefs, filteredCards);
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
        private Dictionary<string, Card> LoadCardDb()
        {
            Dictionary<string, Card> cardDb = new Dictionary<string, Card>();
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

        // Grab the HS version
        private string GetPatchVersion()
        {
            var hsExe = Path.Combine(_hsPath, "Hearthstone.exe");
            // TODO: check it exists
            var vi = FileVersionInfo.GetVersionInfo(hsExe);
            // NOTE: FileVersion & ProductVersion strings are same, need to use parts
            Version version = new Version(
                vi.FileMajorPart, vi.FileMinorPart, vi.FileBuildPart, vi.FilePrivatePart);
            return version.ToString();
        }
    }
}