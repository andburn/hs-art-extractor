﻿using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using HsArtExtractor.Util;

namespace HsArtExtractor.Hearthstone.CardArt
{
	/// <summary>
	/// A database for mapping card ids to bundle unity texture names and transforms.
	/// </summary>
	public static class CardArtDb
	{
		private static CardArtDefs _defs;

		public static CardArtDefs Defs
		{
			get
			{
				if (_defs == null)
					return new CardArtDefs();
				else
					return _defs;
			}
		}

		public static Dictionary<string, ArtCard> All = new Dictionary<string, ArtCard>();

		public static string GamePatch
		{
			get
			{
				if (_defs.Patch == null)
					return "0.0.0.0";
				return _defs.Patch;
			}
		}

		public static void Read(string file)
		{
			using (TextReader tr = new StreamReader(file))
			{
				var xml = new XmlSerializer(typeof(CardArtDefs));
				_defs = (CardArtDefs)xml.Deserialize(tr);
				foreach (var card in _defs.Cards)
				{
					if (All.ContainsKey(card.Id))
						Logger.Log(LogLevel.ERROR, $"{card.Id} already in CardArtDb");
					else
						All.Add(card.Id, card);
				}
			}
		}

		public static void Write(string file, CardArtDefs cards)
		{
			using (TextWriter tw = new StreamWriter(file))
			{
				var xml = new XmlSerializer(typeof(CardArtDefs));
				xml.Serialize(tw, cards);
			}
		}
	}
}