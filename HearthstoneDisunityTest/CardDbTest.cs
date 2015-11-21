using System.Collections.Generic;
using HearthstoneDisunity.Hearthstone;
using HearthstoneDisunity.Hearthstone.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HearthstoneDisunityTest
{
    [TestClass]
    public class CardDbTest
    {
        private static Dictionary<string, Card> _cards;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            CardDb.Read(@"data\cardxml.txt");
            _cards = CardDb.All;
        }

        [TestMethod]
        public void CardDbCount()
        {
            Assert.AreEqual(3, _cards.Count);
        }

        [TestMethod]
        public void CardId()
        {
            Assert.AreEqual("CS1h_001", _cards["CS1h_001"].Id);
        }

        [TestMethod]
        public void CardName()
        {
            Assert.AreEqual("Goldshire Footman", _cards["CS1_042"].Name);
        }

        [TestMethod]
        public void CardTypeAbility()
        {
            Assert.AreEqual(CardType.ABILITY, _cards["CS1_112"].Type);
        }

        [TestMethod]
        public void CardTypeMinion()
        {
            Assert.AreEqual(CardType.MINION, _cards["CS1_042"].Type);
        }

        [TestMethod]
        public void CardTypeHeroPower()
        {
            Assert.AreEqual(CardType.HERO_POWER, _cards["CS1h_001"].Type);
        }

    }
}
