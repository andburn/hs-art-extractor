using System;
using System.IO;
using System.Linq;
using System.Text;
using HearthstoneDisunity;
using HearthstoneDisunity.Hearthstone;
using HearthstoneDisunity.Hearthstone.Database;

namespace HearthstoneDisunityCLI
{
    internal class Program
    {
        private static string _usage;

        private static void Main(string[] args)
        {
            _usage = UsuageText();

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
                    set = -1;
            }

            Console.WriteLine("Extracting card art from {0} to {1}", hsDir, outDir);
            try
            {
                Extract.CardArt(outDir, hsDir, set);
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

            Console.WriteLine("Generating card counts from cardxml");
            try
            {
                var cardxml = Path.Combine(hsDir, "cardxml0.unity3d");
                Extract.Text(outDir, cardxml);
                var cardtxt = Path.Combine(outDir, "TextAsset", "enUS.txt");
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

        private static string UsuageText()
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

            Console.WriteLine("\nTotals:\n\tall: {0}\n\tcollectible: {1}",
                cards.Count, cards.Count(x => x.IsCollectible && x.Type != CardType.HERO));
            Console.WriteLine("Basic:\n\tall: {0}\n\tcollectible: {1}",
                cards.Count(x => x.Set == CardSet.CORE),
                cards.Count(x => x.IsCollectible && x.Set == CardSet.CORE && x.Type != CardType.HERO));
            Console.WriteLine("Classic:\n\tall: {0}\n\tcollectible: {1}",
                cards.Count(x => x.Set == CardSet.EXPERT1),
                cards.Count(x => x.IsCollectible && x.Set == CardSet.EXPERT1));
            Console.WriteLine("Naxx:\n\tall: {0}\n\tcollectible: {1}",
                cards.Count(x => x.Set == CardSet.FP1),
                cards.Count(x => x.IsCollectible && x.Set == CardSet.FP1));
            Console.WriteLine("BRM:\n\tall: {0}\n\tcollectible: {1}",
                cards.Count(x => x.Set == CardSet.BRM),
                cards.Count(x => x.IsCollectible && x.Set == CardSet.BRM));
            Console.WriteLine("LOE:\n\tall: {0}\n\tcollectible: {1}",
                cards.Count(x => x.Set == CardSet.LOE),
                cards.Count(x => x.IsCollectible && x.Set == CardSet.LOE));
            Console.WriteLine("TGT:\n\tall: {0}\n\tcollectible: {1}",
                cards.Count(x => x.Set == CardSet.TGT),
                cards.Count(x => x.IsCollectible && x.Set == CardSet.TGT));
            Console.WriteLine("GVG:\n\tall: {0}\n\tcollectible: {1}",
                cards.Count(x => x.Set == CardSet.GVG),
                cards.Count(x => x.IsCollectible && x.Set == CardSet.GVG));
            Console.WriteLine("Reward:\n\tall: {0}\n\tcollectible: {1}",
                cards.Count(x => x.Set == CardSet.REWARD || x.Set == CardSet.PROMO),
                cards.Count(x => x.IsCollectible && (x.Set == CardSet.REWARD || x.Set == CardSet.PROMO)));

            var filtered = CardDb.Filtered.Values.ToList<Card>();
            Console.WriteLine("\nFiltered: {0}", filtered.Count);
        }
    }
}