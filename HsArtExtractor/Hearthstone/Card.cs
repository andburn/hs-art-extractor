using System;
using HsArtExtractor.Hearthstone.Database;

namespace HsArtExtractor.Hearthstone
{
	public class Card
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public CardSet Set { get; set; }
		public CardType Type { get; set; }
		public bool IsCollectible { get; set; }

		public Card(string id, string name, CardType type, CardSet set, bool collectible)
		{
			Id = id;
			Name = name;
			Type = type;
			Set = set;
			IsCollectible = collectible;
		}

		public Card(Entity e)
		{
			Id = e.CardId;
			Name = e.GetInnerValue((int)GameTag.CARDNAME);
			var typeId = e.GetTag((int)GameTag.CARDTYPE);
			Type = (CardType)typeId;
			var setId = e.GetTag((int)GameTag.CARD_SET);
			Set = (CardSet)setId;
			var collectible = e.GetTag((int)GameTag.COLLECTIBLE);
			IsCollectible = collectible == 1;
		}

		public Card(JsonCard jc)
		{
			Id = jc.Id;
			Name = jc.Name;
			CardType pType;
			if (Enum.TryParse(jc.Type, true, out pType))
				Type = pType;
			CardSet pSet;
			if (Enum.TryParse(jc.Set, true, out pSet))
				Set = pSet;
			IsCollectible = jc.Collectible;
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
}