using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HearthstoneDisunity;

namespace HearthstoneDisunityCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                PrintUsageAndExit();
            }

            var command = args[0];
            var fileDir = args[1];
            var outDir = args[2];

            switch (command.ToLower())
            {
                case "dump":
                    ExtractRaw(fileDir, outDir);
                    break;
                case "extract":
                    ExtractAll(fileDir, outDir);
                    break;
                case "cardart":
                    CardArt(fileDir, outDir);
                    break;
                default:
                    PrintUsageAndExit();
                    break;
            }
        }

        private static void CardArt(string hsDir, string outDir)
        {
            Console.WriteLine("Extracting Card Art from {0} to {1}", hsDir, outDir);
            try
            {
                Extract.CardArt(outDir, hsDir);
            }
            catch (Exception e)
            {
                PrintErrorAndExit(e.Message);
            }
        }

        private static void ExtractAll(string file, string outDir)
        {
            Console.WriteLine("Extracting Assets from {0} to {1}", file, outDir);
            try
            {
                Extract.All(outDir, file);
            }
            catch (Exception e)
            {
                PrintErrorAndExit(e.Message);
            }
        }

        private static void ExtractRaw(string file, string outDir)
        {
            Console.WriteLine("Extracting Raw Assets from {0} to {1}", file, outDir);
            try
            {
                Extract.Raw(outDir, file);
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
            Console.WriteLine("Usage: ");
            Environment.Exit(1);
        }
    }
}
