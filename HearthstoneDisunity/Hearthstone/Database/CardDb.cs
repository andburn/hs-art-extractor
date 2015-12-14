using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace HearthstoneDisunity.Hearthstone.Database
{
    /// <summary>
    /// A basic card database, for card ids, types and sets.
    /// </summary>
    public static class CardDb
    {
        public static Dictionary<string, Card> All = new Dictionary<string, Card>();

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

        public static void Read(string file)
        {
            using (TextReader tr = new StreamReader(file))
            {
                var xml = new XmlSerializer(typeof(CardDefs));
                var cardDefs = (CardDefs)xml.Deserialize(tr);
                foreach (var entity in cardDefs.Entites)
                {
                    var card = new Card(entity);
                    All.Add(entity.CardId, card);
                }
            }
        }
    }
}