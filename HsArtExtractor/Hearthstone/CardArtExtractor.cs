using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using HsArtExtractor.Hearthstone.Bundle;
using HsArtExtractor.Hearthstone.CardArt;
using HsArtExtractor.Hearthstone.Database;
using HsArtExtractor.Unity;
using HsArtExtractor.Unity.Extract;
using HsArtExtractor.Unity.Objects;
using HsArtExtractor.Util;

namespace HsArtExtractor.Hearthstone
{
	internal class CardArtExtractor
	{
		private static string _hsDataPath;

		public static void Extract(CardArtExtractorOptions opts)
		{
			ValidateDirectories(opts.OutputDir, opts.HearthstoneDir);

			// Init set and type filters
			List<CardSet> includeSets = CardEnumConverter.SetIds(opts.Sets);
			List<CardType> includeTypes = CardEnumConverter.TypeIds(opts.Types);

			// Load card data
			LoadCardDb(opts.OutputDir);

			// If a map file was supplied, use that instead of parsing cards files
			CardArtDefs defs = null;
			if (File.Exists(opts.MapFile))
			{
				Logger.Log(LogLevel.INFO, "using map file: {0}", opts.MapFile);
				CardArtDb.Read(opts.MapFile);
				defs = CardArtDb.Defs;
			}
			else
			{
				// map file not found, using default method
				Logger.Log(LogLevel.WARN, "map file not found: {0}", opts.MapFile);

				// Load card art data (cards<n>.unity3d)
				var cardsFiles = new List<string>(Directory.GetFiles(_hsDataPath, "cards?.unity3d"));
				var artCards = new CardsBundle(cardsFiles);
				// create CardArtDb defs
				defs = new CardArtDefs();
				defs.Patch = GetPatchVersion(opts.HearthstoneDir);
				defs.Cards = artCards.Cards;
			}

			// Create the list of cards we want to output
			List<ArtCard> filteredCards = null;
			if (opts.NoFiltering)
			{
				// don't use default filter, or option filters, just return all
				filteredCards = defs.Cards.Where(x => CardDb.All.ContainsKey(x.Id)).ToList();
			}
			else if (includeSets.Count > 0 || includeTypes.Count > 0)
			{
				// filter db list using options
				var filteredDb = CardDb.FilterBy(includeSets, includeTypes);
				// filter art card defs by only including those in filteredDb
				filteredCards = defs.Cards.Where(x => filteredDb.ContainsKey(x.Id)).ToList();
			}
			else
			{
				// filter art card defs by only including those that are in the default filtered db
				filteredCards = defs.Cards.Where(x => CardDb.Filtered.ContainsKey(x.Id)).ToList();
			}
			Logger.Log("Filtered art cards: " + filteredCards.Count);

			// get all textures (cardtextures<n>.unity3d)
			var textureFiles = new List<string>(Directory.GetFiles(_hsDataPath, "cardtextures?.unity3d"));
			// First pass over texture bundles, collect texture path data
			var textureRefs = LoadTextureInfo(textureFiles);
			// Add shared bundles to texture list (shared<n>.unity3d)
			textureFiles.AddRange(Directory.GetFiles(_hsDataPath, "shared?.unity3d"));
			// Extract all the texture files using the refs and required cards
			var tb = new TexturesBundle(textureFiles, opts);
			var bundleMap = tb.Extract(textureRefs, filteredCards);
			// Update the cardartdb with found bundle names, and save
			if (opts.SaveMapFile)
				UpdateAndSaveCardArtDefs(bundleMap, defs, opts.OutputDir);
			// Delete TextAsset directory
			Directory.Delete(Path.Combine(opts.OutputDir, "TextAsset"), true);
		}

		private static void UpdateAndSaveCardArtDefs(Dictionary<string, string> map, CardArtDefs defs, string dir)
		{
			// bundleMap will only contain cards matching previous filtering
			foreach (var d in defs.Cards)
			{
				// to get entries for all cards, need to use NoFilter option
				if (map.ContainsKey(d.Id))
					d.Texture.Bundle = StringUtils.GetFilenameNoExt(map[d.Id]);
			}
			// TODO is this format we want for map file? and what about json
			// save to disk
			CardArt.CardArtDb.Write(Path.Combine(dir, "CardArtDefs.xml"), defs);
		}

		private static void ValidateDirectories(string outDir, string hsDir)
		{
			Logger.Log("Initializing CardArt ({0} to {1})", hsDir, outDir);

			_hsDataPath = Path.Combine(hsDir, "Data", "Win");

			if (!Directory.Exists(_hsDataPath))
			{
				throw new DirectoryNotFoundException("The Hearthstone directory does not exist (" + _hsDataPath + ")");
			}

			if (!Directory.Exists(outDir))
			{
				Directory.CreateDirectory(outDir);
			}
		}

		// First pass over texture bundles, collect texture path data
		private static Dictionary<long, string> LoadTextureInfo(List<string> textureFiles)
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

		// Load a simple card db from "cardxml0.unity3d"
		private static void LoadCardDb(string outDir)
		{
			// Extract card xml files
			var cardXml = Path.Combine(_hsDataPath, "cardxml0.unity3d");
			if (File.Exists(cardXml))
			{
				AssestFile xmlBundle = new AssestFile(cardXml);
				var extractor = new TextExtractor();
				foreach (var obj in xmlBundle.Objects)
				{
					extractor.Extract(obj, outDir);
				}
				// Create cardDB from english xml
				var xmlFile = Path.Combine(outDir, "TextAsset", "enUS.txt");
				if (File.Exists(xmlFile))
				{
					CardDb.Read(xmlFile);
					Logger.Log("CardDB loaded: {0} cards", CardDb.All.Count);
					Logger.Log("CardDB loaded: {0} cards (filtered)", CardDb.Filtered.Count);
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
		}

		// Grab the HS version
		private static string GetPatchVersion(string hsDir)
		{
			var version = new Version();
			var hsExe = Path.Combine(hsDir, "Hearthstone.exe");
			if (File.Exists(hsExe))
			{
				var vi = FileVersionInfo.GetVersionInfo(hsExe);
				// NOTE: FileVersion & ProductVersion strings are same, need to use parts
				version = new Version(vi.FileMajorPart, vi.FileMinorPart, vi.FileBuildPart, vi.FilePrivatePart);
			}

			return version.ToString();
		}
	}
}