using System;

using HearthstoneDisunity;

namespace HearthstoneDisunityCLI
{
    internal class Program
    {
        private static void Main(string[] args)
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
                case "text":
                    ExtractText(fileDir, outDir);
                    break;

                case "texture":
                    ExtractTextures(fileDir, outDir);
                    break;

                case "dump":
                    ExtractRaw(fileDir, outDir);
                    break;

                case "cardart":
                    CardArt(fileDir, outDir);
                    break;

                default:
                    PrintUsageAndExit();
                    break;
            }
        }

        private static void ExtractText(string file, string outDir)
        {
            Console.WriteLine("Extracting TextAssets from {0} to {1}", file, outDir);
            try
            {
                Extract.Text(outDir, file);
            }
            catch (Exception e)
            {
                PrintErrorAndExit(e.Message);
            }
        }

        private static void ExtractTextures(string file, string outDir)
        {
            Console.WriteLine("Extracting Textures from {0} to {1}", file, outDir);
            try
            {
                Extract.Textures(outDir, file);
            }
            catch (Exception e)
            {
                PrintErrorAndExit(e.Message);
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