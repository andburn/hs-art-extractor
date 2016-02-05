using System.Collections.Generic;

namespace HsArtExtractor.Hearthstone
{
	// Convert card set and type for CardArtExtractor
	// TODO needs overhaul, less maintenance
	// TODO use proper converter?
	internal class CardEnumConverter
	{
		internal static Dictionary<CardSet, string> FriendlySetName =
			new Dictionary<CardSet, string> {
				{ CardSet.CORE, "Basic" },
				{ CardSet.EXPERT1, "Classic" },
				{ CardSet.REWARD, "Reward" },
				{ CardSet.MISSIONS, "Missions" },
				{ CardSet.CHEAT, "Cheat" },
				{ CardSet.PROMO, "Promo" },
				{ CardSet.FP1, "Curse of Naxxramas" },
				{ CardSet.PE1, "Goblins vs Gnomes" },
				{ CardSet.BRM, "Black Rock Mountain" },
				{ CardSet.TGT, "The Grand Tournament" },
				{ CardSet.CREDITS, "Credits" },
				{ CardSet.HERO_SKINS, "Hero Skins" },
				{ CardSet.TB, "Tavern Brawl" },
				{ CardSet.LOE, "League of Explorers" }
			};

		internal static List<CardSet> SetIds(List<string> sets)
		{
			List<CardSet> setIds = new List<CardSet>();
			foreach (var s in sets)
			{
				setIds.Add(MatchCardSet(s));
			}
			return setIds;
		}

		internal static CardSet MatchCardSet(string set)
		{
			switch (set.ToLower())
			{
				case "basic":
				case "core":
					return CardSet.CORE;

				case "classic":
				case "expert":
					return CardSet.EXPERT1;

				case "naxx":
					return CardSet.FP1;

				case "gvg":
					return CardSet.GVG;

				case "brm":
					return CardSet.BRM;

				case "tgt":
					return CardSet.TGT;

				case "loe":
					return CardSet.LOE;

				case "reward":
					return CardSet.REWARD;

				case "promo":
					return CardSet.PROMO;

				default:
					return CardSet.INVALID;
			}
		}

		internal static List<CardType> TypeIds(List<string> types)
		{
			List<CardType> typeIds = new List<CardType>();
			foreach (var t in types)
			{
				typeIds.Add(MatchCardType(t));
			}
			return typeIds;
		}

		internal static CardType MatchCardType(string type)
		{
			switch (type.ToLower())
			{
				case "hero":
					return CardType.HERO;

				case "hero_power":
				case "power":
					return CardType.HERO_POWER;

				case "minion":
				case "minions":
					return CardType.MINION;

				case "ability":
				case "spell":
				case "spells":
					return CardType.ABILITY;

				case "weapon":
				case "weapons":
					return CardType.WEAPON;

				case "token":
					return CardType.TOKEN;

				default:
					return CardType.INVALID;
			}
		}
	}
}