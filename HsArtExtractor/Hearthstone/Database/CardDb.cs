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
        // TODO: does this need to be a dictionary
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
            CardType.HERO,
            CardType.HERO_POWER,
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