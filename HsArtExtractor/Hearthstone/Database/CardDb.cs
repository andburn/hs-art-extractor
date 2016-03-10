using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace HsArtExtractor.Hearthstone.Database
{
	/// <summary>
	/// A basic card database, for card ids, types and sets.
	/// </summary>

	public static class CardDb
	{
		private static Dictionary<string, Card> _cards = new Dictionary<string, Card>();

		private static List<CardSet> CardSetExclusions = new List<CardSet>() {
			CardSet.INVALID,
			CardSet.TEST_TEMPORARY,
			CardSet.DEMO,
			CardSet.CHEAT,
			CardSet.BLANK,
			CardSet.DEBUG_SP,
			CardSet.SLUSH
		};

		private static List<CardType> CardTypeExclusions = new List<CardType>() {
			CardType.INVALID,
			CardType.GAME,
			CardType.PLAYER,
			CardType.ENCHANTMENT,
			CardType.ITEM,
			CardType.TOKEN
		};

		public static Dictionary<string, Card> All
		{
			get
			{
				return _cards;
			}
		}

		// filtered based on default exclusions
		public static Dictionary<string, Card> Filtered
		{
			get
			{
				return _cards.Where(x =>
					!CardSetExclusions.Contains(x.Value.Set)
					&& !CardTypeExclusions.Contains(x.Value.Type))
					.ToDictionary(x => x.Key, x => x.Value);
			}
		}

		// return the carddb filtered by type and set
		public static Dictionary<string, Card> FilterBy(
			List<CardSet> sets, List<CardType> types)
		{
			// use count > 0 so empty lists still return all cards,
			// only want TT and FF to be T
			return _cards.Where(x =>
					(!(sets.Count > 0 ^ sets.Contains(x.Value.Set)))
					&& (!(types.Count > 0 ^ types.Contains(x.Value.Type))))
					.ToDictionary(x => x.Key, y => y.Value);
		}

		public static void Read(string file)
		{
			_cards.Clear();
			using (TextReader tr = new StreamReader(file))
			{
				var xml = new XmlSerializer(typeof(CardDefs));
				var cardDefs = (CardDefs)xml.Deserialize(tr);
				foreach (var entity in cardDefs.Entites)
				{
					var card = new Card(entity);
					_cards.Add(entity.CardId, card);
				}
			}
		}
	}
}