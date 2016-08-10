﻿using System;
using System.IO;
using HsArtExtractor.Util;

namespace HsArtExtractor.Unity.Objects
{
	public class CardDef
	{
		public FilePointer GameObject { get; private set; }
		public bool Enabled { get; private set; }
		public FilePointer MonoScript { get; private set; }
		public string Name { get; private set; }
		public string PortratitTexturePath { get; private set; }
		public string PremiumPortraitMaterialPath { get; private set; }
		public string PremiumPortraitTexturePath { get; private set; }
		public FilePointer DeckCardBarPortrait { get; private set; }
		public FilePointer EnchantmentPortrait { get; private set; }
		public FilePointer HistoryTileHalfPortrait { get; private set; }
		public FilePointer HistoryTileFullPortrait { get; private set; }
		public bool FailedToLoad { get; private set; } = false;

		public CardDef(BinaryBlock b)
		{
			GameObject = new FilePointer(b.ReadInt(), b.ReadLong());
			Enabled = b.ReadUnsignedByte() == 1 ? true : false;
			b.Align(4);
			MonoScript = new FilePointer(b.ReadInt(), b.ReadLong());

			try
			{
				var unknown = b.ReadInt();

				int size = b.ReadInt();
				PortratitTexturePath = b.ReadFixedString(size);
				b.Align(4);

				size = b.ReadInt();
				PremiumPortraitMaterialPath = b.ReadFixedString(size);
				b.Align(4);
				var unknown2 = b.ReadInt();

				DeckCardBarPortrait = new FilePointer(b.ReadInt(), b.ReadLong());

				EnchantmentPortrait = new FilePointer(b.ReadInt(), b.ReadLong());

				HistoryTileHalfPortrait = new FilePointer(b.ReadInt(), b.ReadLong());

				HistoryTileFullPortrait = new FilePointer(b.ReadInt(), b.ReadLong());

				// Ignore rest of the file
			}
			catch (Exception e)
			{
				Logger.Log(LogLevel.DEBUG, $"CardDef Load failed {GameObject.PathID}, {MonoScript.PathID} ({e})");
				FailedToLoad = true;
			}
		}

		public override string ToString()
		{
			return string.Format("Name: '{0}' Tex: '{1}'", PortratitTexturePath);
		}

		public void Save(string dir, string name = "default")
		{
			string outFile = Name;
			if (string.IsNullOrEmpty(Name))
				outFile = name;
			outFile = StringUtils.GetFilenameNoOverwrite(
				Path.Combine(dir, outFile + ".txt"));

			using (StreamWriter sw = new StreamWriter(outFile, false))
			{
				sw.WriteLine("CardDef");
				sw.WriteLine("\tGameObject: " + GameObject);
				sw.WriteLine("\tName: " + Name);
				sw.WriteLine("\tPortraitTexPath: " + PortratitTexturePath);
				sw.WriteLine("\tPremiumMat: " + PremiumPortraitMaterialPath);
				sw.WriteLine("\tPremiumTex: " + PremiumPortraitTexturePath);
				sw.WriteLine("\tDeckCardBar: " + DeckCardBarPortrait);
				sw.WriteLine("\tEnchantPortrait: " + EnchantmentPortrait);
				sw.WriteLine("\tHistoryTileHalfPortrait: " + HistoryTileHalfPortrait);
				sw.WriteLine("\tHistoryTileFullPortrait: " + HistoryTileFullPortrait);
			}
		}
	}
}