using System;
using System.Diagnostics;
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
#if DEBUG
			Logger.SetLogLevel(LogLevel.DEBUG);
#else
			Logger.SetLogLevel(LogLevel.WARN);
#endif

			return CommandLine.Parser.Default.ParseArguments<DumpOptions, CardArtOptions, ImageOptions>(args)
				.MapResult(
				(DumpOptions opts) => DumpCommand(opts),
				(CardArtOptions opts) => CardArtCommand(opts),
				(ImageOptions opts) => ImageCommand(opts),
				errors => 1);
		}

		private static int ImageCommand(ImageOptions opts)
		{
			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();

			var dir = Directory.GetCurrentDirectory();

			Console.WriteLine("Source Dir: {0}", opts.SourceDirectory);
			var files = Directory.GetFiles(opts.SourceDirectory, "*.unity3d");
			Console.WriteLine("Found {0} files", files.Count());

			if (!string.IsNullOrWhiteSpace(opts.Output))
				dir = opts.Output;
			Console.WriteLine($"Saving to: {dir}");

			Extract.AllImages(files, dir, opts.Alpha, opts.Flip);

			stopWatch.Stop();
			TimeSpan ts = stopWatch.Elapsed;
			Console.WriteLine(ts);

			return 0;
		}

		private static int CardArtCommand(CardArtOptions opts)
		{
			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();

			var dir = Directory.GetCurrentDirectory();

			Console.WriteLine("Hearthstone Dir: {0}", opts.HsDirectory);

			if (!string.IsNullOrWhiteSpace(opts.Output))
				dir = opts.Output;
			Console.WriteLine($"Saving to: {dir}");

			CardArtExtractorOptions exOptions = new CardArtExtractorOptions();
			exOptions.HearthstoneDir = opts.HsDirectory;
			exOptions.OutputDir = dir;
			exOptions.FullArtOnly = opts.FullArtOnly;
			exOptions.BarArtOnly = opts.BarArtOnly;

			if (!string.IsNullOrEmpty(opts.Height))
				exOptions.Height = ParseInt(opts.Height);

			if (!string.IsNullOrEmpty(opts.BarHeight))
				exOptions.BarHeight = ParseInt(opts.BarHeight);

			exOptions.WithoutBarCoords = opts.WithoutBarCoords;
			exOptions.FlipY = !opts.NoFlip;
			exOptions.SaveMapFile = opts.SaveMapFile;
			exOptions.PreserveAlphaChannel = opts.KeepAlpha;
			exOptions.GroupBySet = opts.Group;
			exOptions.AddCardName = opts.Name;
			exOptions.Sets = opts.Sets.ToList();
			exOptions.Types = opts.Types.ToList();
			exOptions.NoFiltering = opts.NoFiltering;
			exOptions.MapFile = opts.MapFile;
			exOptions.CropHidden = opts.CropHidden;

			if (opts.ImageType != null)
				exOptions.ImageType = opts.ImageType;

			Extract.CardArt(exOptions);

			stopWatch.Stop();
			TimeSpan ts = stopWatch.Elapsed;
			Console.WriteLine(ts);

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

		private static int ParseInt(string num)
		{
			var height = 0;
			var parsed = int.TryParse(num, out height);
			if (!parsed)
				Logger.Log(LogLevel.ERROR, "integer parse failed for: {0}", num);
			return height;
		}
	}
}