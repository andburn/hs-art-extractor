using System.Collections.Generic;
using CommandLine;

namespace HsArtExtractorCLI
{
	[Verb("dump", HelpText = "Dump assets from unity3d files.")]
	internal class DumpOptions
	{
		// default --verbose, no short -v
		[Option(
			HelpText = "Show detailed information.")]
		public bool Verbose { get; set; }

		[Value(0,
			MetaName = "input files",
			HelpText = "Input files to be processed.",
			Required = true)]
		public IEnumerable<string> InputFiles { get; set; }
	}
}