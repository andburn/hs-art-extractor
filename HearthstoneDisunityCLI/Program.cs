using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HearthstoneDisunity;
using HearthstoneDisunity.Hearthstone;
using HearthstoneDisunity.Hearthstone.CardArt;
using HearthstoneDisunity.Hearthstone.Database;
using HearthstoneDisunity.Util;

namespace HearthstoneDisunityCLI
{
    internal class Program
    {
        private static string _usage;

        private static void Main(string[] args)
        {
            Logger.SetLogLevel(LogLevel.DEBUG);

            _usage = UsageText();

            if (args.Length < 1)
            {
                PrintUsageAndExit();
            }

            var command = args[0];
            switch (command.ToLower())
            {
                case "textures":
                    ExtractTextures(args);
                    break;

                case "dump":
                    ExtractRaw(args);
                    break;

                case "cardart":
                    CardArt(args);
                    break;

                case "query":
                    QueryCardDb(args);
                    break;

                default:
                    PrintUsageAndExit();
                    break;
            }
        }

        private static void ExtractTextures(string[] args)
        {
            if (args.Length != 3)
                PrintUsageAndExit();

            var file = args[1];
            var dir = args[2];

            Logger.SetLogLocation(dir);
            Console.WriteLine("Extracting texture assets from {0} to {1}", file, dir);
            try
            {
                Extract.Textures(dir, file);
            }
            catch (Exception e)
            {
                PrintErrorAndExit(e.Message);
            }
        }

        private static void ExtractRaw(string[] args)
        {
            if (args.Length != 3)
                PrintUsageAndExit();

            var file = args[1];
            var dir = args[2];

            Logger.SetLogLocation(dir);
            Console.WriteLine("Dumping all assets from {0} to {1}", file, dir);
            try
            {
                Extract.Raw(dir, file);
            }
            catch (Exception e)
            {
                PrintErrorAndExit(e.Message);
            }
        }

        private static void CardArt(string[] args)
        {
            string hsDir = "";
            string outDir = "";
            int set = -1;

            if (args.Length < 3)
            {
                PrintUsageAndExit();
            }
            else if (args.Length >= 3)
            {
                hsDir = args[1];
                outDir = args[2];
            }

            if (args.Length >= 4)
            {
                hsDir = args[1];
                outDir = args[2];
                var success = int.TryParse(args[3], out set);
                if (success == false)
                {
                    try
                    {
                        CardSet setname = (CardSet)Enum.Parse(typeof(CardSet), args[3], true);
                        set = (int)setname;
                    }
                    catch
                    {
                        set = -1;
                    }
                }
            }

            Logger.SetLogLocation(outDir);
            Console.WriteLine("Extracting card art from {0} to {1}", hsDir, outDir);
            try
            {
                Extract.CardArt(outDir, hsDir, set);
                // TODO: debug
                // check extracted full cards (name = id) against cardb
                if (set > 0)
                {
                    List<string> ids = new List<string>();
                    var files = Directory.GetFiles(Path.Combine(outDir, "Full"));
                    foreach (var f in files)
                    {
                        ids.Add(StringUtils.GetFilenameNoExt(f));
                    }
                    var notfound = CardDb.All.Values.Where(x => x.Set == (CardSet)set).Where(x => !ids.Contains(x.Id));
                    foreach (var nf in notfound)
                    {
                        Console.WriteLine(nf.Name + ", " + nf.Id);
                    }
                }
            }
            catch (Exception e)
            {
                PrintErrorAndExit(e.Message);
            }
        }

        private static void QueryCardDb(string[] args)
        {
            if (args.Length != 3)
                PrintUsageAndExit();

            var hsDir = args[1];
            var outDir = args[2];

            Logger.SetLogLocation(outDir);
            Console.WriteLine("Generating card counts from cardxml");
            try
            {
                var cardtxt = Path.Combine(outDir, "cardxml0", "TextAsset", "enUS.txt");
                var artxml = Path.Combine(outDir, "CardArtDefs.xml");
                if (!File.Exists(cardtxt))
                {
                    // extract text if it doesn't exist
                    var cardxml = Path.Combine(hsDir, "Data", "Win", "cardxml0.unity3d");
                    Extract.Text(outDir, cardxml);
                }
                if (File.Exists(artxml))
                {
                    CardArtDb.Read(artxml);
                }
                CardDb.Read(cardtxt);
                PrintCardCounts();
            }
            catch (Exception e)
            {
                PrintErrorAndExit(e.Message);
            }
        }

        private static void PrintErrorAndExit(string message)
        {
            Console.WriteLine("Error: " + message);
            Environment.Exit(1);
        }

        private static void PrintUsageAndExit()
        {
            Console.WriteLine(_usage);
            Environment.Exit(1);
        }

        private static string UsageText()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Usage:");
            sb.AppendLine("\tcardart <hs_dir> <output_dir>");
            sb.AppendLine("\ttextures <file> <output_dir>");
            sb.AppendLine("\tdump <file> <output_dir>");
            return sb.ToString();
        }

        private static void PrintCardCounts()
        {
            var cards = CardDb.All.Values.ToList<Card>();
            var cardart = CardArtDb.All.Values.ToList<ArtCard>();

            Console.WriteLine("\nTotals:\n\tall: {0}, art: {1}\n\tcollectible: {2}\n\tcardbars: {3}",
                cards.Count, cardart.Count,
                cards.Count(x => x.IsCollectible && x.Type != CardType.HERO),
                cardart.Count(x => x.GetMaterial(MaterialType.CardBar) != null));

            PrintSetCount(cards, cardart, "Basic", CardSet.CORE);
            PrintSetCount(cards, cardart, "Classic", CardSet.EXPERT1);
            PrintSetCount(cards, cardart, "Naxx", CardSet.FP1);
            PrintSetCount(cards, cardart, "BRM", CardSet.BRM);
            PrintSetCount(cards, cardart, "LOE", CardSet.LOE);
            //cards.Where(x => x.IsCollectible && x.Set == CardSet.LOE).ToList().ForEach(t => Console.WriteLine("\t" + t.Id + ", " + t.Name));
            PrintSetCount(cards, cardart, "TGT", CardSet.TGT);
            PrintSetCount(cards, cardart, "GVG", CardSet.GVG);
            PrintSetCount(cards, cardart, "Reward", CardSet.PROMO, CardSet.REWARD);

            var filtered = CardDb.Filtered.Values.ToList<Card>();
            Console.WriteLine("\nFiltered: {0}", filtered.Count);
        }

        private static void PrintSetCount(List<Card> cards, List<ArtCard> artcards, string label, params CardSet[] sets)
        {
            Console.WriteLine("{0}:\n\tall: {1}\n\tcollectible: {2}\n\tcardbars: {3}",
                label,
                cards.Count(x => sets.Contains(x.Set)),
                cards.Count(x => x.IsCollectible && sets.Contains(x.Set) && x.Type != CardType.HERO),
                artcards.Count(x => x.GetMaterial(MaterialType.CardBar) != null
                    && (sets.Contains(CardDb.All[x.Id].Set))
                ));
        }
    }
}