using System;
using System.IO;
using System.Linq;
using CommandLine;
using HsArtExtractor;
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
			Console.WriteLine("CARDART");

			Console.WriteLine("HsDir: {0}", opts.HsDirectory);

			if (string.IsNullOrWhiteSpace(opts.Output))
				Console.WriteLine("Output: current directory");
			else
				Console.WriteLine("Output: " + opts.Output);

			if (opts.NoFlip)
				Console.WriteLine("+ no-flip");
			if (opts.NoAlpha)
				Console.WriteLine("+ no-alpha");
			if (opts.Group)
				Console.WriteLine("+ group");
			if (opts.Name)
				Console.WriteLine("+ name");
			if (opts.Sets.Count() > 0)
				Console.WriteLine("Sets: {0}", string.Join(",", opts.Sets.ToArray()));
			if (opts.Types.Count() > 0)
				Console.WriteLine("Types: {0}", string.Join(",", opts.Types.ToArray()));
			if (!string.IsNullOrWhiteSpace(opts.MapFile))
				Console.WriteLine("MapFile: {0}", opts.MapFile);

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