using HearthstoneDisunity.Hearthstone.Xml;

namespace HearthstoneDisunity.Hearthstone
{
    public class Card
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public CardType Type { get; set; }

        public Card(string id, string name, CardType type)
        {
            Id = id;
            Name = name;
            Type = type;
        }

        public Card(Entity e)
        {
            Id = e.CardId;
            Name = e.GetInnerValue((int)CardTag.CARDNAME);
            var typeId = e.GetTag((int)CardTag.CARDTYPE);
            Type = (CardType)typeId;
        }

        public override bool Equals(object obj)
        {
            if(obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Card c = (Card)obj;
            return c.Name.ToLower().Equals(Name.ToLower());
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ Name.GetHashCode();
        }
    }

    public enum CardTag
    {
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
}
