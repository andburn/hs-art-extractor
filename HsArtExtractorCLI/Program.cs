using System;
using System.IO;
using System.Linq;
using CommandLine;
using HsArtExtractor;
using HsArtExtractor.Hearthstone;
using HsArtExtractor.Util;

namespace HsArtExtractorCLI
{
	internal class Program
	{
		private static int Main(string[] args)
		{
			Logger.SetLogLevel(LogLevel.DEBUG);

			return CommandLine.Parser.Default.ParseArguments<DumpOptions, CardArtOptions>(args)
				.MapResult(
				(DumpOptions opts) => DumpCommand(opts),
				(CardArtOptions opts) => CardArtCommand(opts),
				errors => 1);
		}

		private static int CardArtCommand(CardArtOptions opts)
		{
			var dir = Directory.GetCurrentDirectory();

			Console.WriteLine("Hearthstone Dir: {0}", opts.HsDirectory);

			if (!string.IsNullOrWhiteSpace(opts.Output))
				dir = opts.Output;
			Console.WriteLine($"Saving to: {dir}");

			CardArtExtractorOptions exOptions = new CardArtExtractorOptions();
			exOptions.HearthstoneDir = opts.HsDirectory;
			exOptions.OutputDir = dir;

			if (opts.FullArtOnly)
			{
				var height = 0;
				var parsed = int.TryParse(opts.Height, out height);
				if (!parsed)
					Logger.Log(LogLevel.ERROR, "integer parse failed for: {0}", opts.Height);

				exOptions.Width = height;
				exOptions.Height = height;
			}
			else if (opts.BarArtOnly)
			{
				var width = 0;
				var height = 0;
				if (!string.IsNullOrWhiteSpace(opts.BarSize))
				{
					var splits = opts.BarSize.Split('x');
					if (splits.Length == 2)
					{
						int.TryParse(splits[0], out width);
						int.TryParse(splits[1], out height);
					}
					else
					{
						Logger.Log(LogLevel.WARN, "BarSize format incorrect: ", opts.BarSize);
					}
				}
				exOptions.BarWidth = width;
				exOptions.BarHeight = height;
			}

			exOptions.FlipY = !opts.NoFlip;
			exOptions.SaveMapFile = opts.SaveMapFile;
			exOptions.PreserveAlphaChannel = opts.KeepAlpha;
			exOptions.GroupBySet = opts.Group;
			exOptions.AddCardName = opts.Name;
			exOptions.Sets = opts.Sets.ToList();
			exOptions.Types = opts.Types.ToList();
			exOptions.NoFiltering = opts.NoFiltering;
			exOptions.MapFile = opts.MapFile;

			Extract.CardArt(exOptions);

			return 0;
		}

		private static int DumpCommand(DumpOptions opts)
		{
			var dir = Directory.GetCurrentDirectory();

			Console.WriteLine("Processing: {0}", string.Join(",", opts.InputFiles.ToArray()));

			if (!string.IsNullOrWhiteSpace(opts.Output))
				dir = opts.Output;
			Console.WriteLine("Saving to: " + dir);

			if (opts.RawAssets || opts.TextAssets || opts.TextureAssets)
			{
				Extract.Multiple(opts.RawAssets, opts.TextAssets, opts.TextureAssets,
					dir, opts.InputFiles.ToArray());
			}
			else
			{
				// default, no options
				Extract.Raw(dir, opts.InputFiles.ToArray());
			}

			return 0;
		}
	}
}