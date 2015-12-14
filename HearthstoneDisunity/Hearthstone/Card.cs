using HearthstoneDisunity.Hearthstone.Database;

namespace HearthstoneDisunity.Hearthstone
{
    public class Card
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public CardSet Set { get; set; }
        public CardType Type { get; set; }

        public Card(string id, string name, CardType type, CardSet set)
        {
            Id = id;
            Name = name;
            Type = type;
            Set = set;
        }

        public Card(Entity e)
        {
            Id = e.CardId;
            Name = e.GetInnerValue((int)CardTag.CARDNAME);
            var typeId = e.GetTag((int)CardTag.CARDTYPE);
            Type = (CardType)typeId;
            var setId = e.GetTag((int)CardTag.CARD_SET);
            Set = (CardSet)setId;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            Card c = (Card)obj;
            return c.Id.ToLower().Equals(Id.ToLower());
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ Name.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{1} [{0}] ({2})", Id, Name, Type);
        }
    }

    public enum CardTag
    {
        CARD_SET = 183,
        CARDNAME = 185,
        CARDTYPE = 202,
        COLLECTIBLE = 321
    }

    public enum CardType
    {
        INVALID = 0,
        GAME = 1,
        PLAYER = 2,
        HERO = 3,
        MINION = 4,
        ABILITY = 5,
        ENCHANTMENT = 6,
        WEAPON = 7,
        ITEM = 8,
        TOKEN = 9,
        HERO_POWER = 10
    }

    public enum CardSet
    {
        INVALID = 0,
        TEST_TEMPORARY = 1,
        CORE = 2,
        EXPERT1 = 3,
        REWARD = 4,
        MISSIONS = 5,
        DEMO = 6,
        NONE = 7,
        CHEAT = 8,
        BLANK = 9,
        DEBUG_SP = 10,
        PROMO = 11,
        FP1 = 12,
        PE1 = 13,
        BRM = 14,
        TGT = 15,
        CREDITS = 16,
        HERO_SKINS = 17,
        TB = 18,
        SLUSH = 19,
        LOE = 20,

        FP2 = BRM,
        PE2 = TGT
    }
}