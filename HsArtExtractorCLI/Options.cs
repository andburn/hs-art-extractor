using System.Collections.Generic;
using CommandLine;

namespace HsArtExtractorCLI
{
	[Verb("cardart", HelpText = "Extract Hearthstone card art.")]
	internal class CardArtOptions
	{
		// full art specific options

		[Option("full-only",
			HelpText = "Extract only the full size card art.",
			SetName = "full")]
		public bool FullArtOnly { get; set; }

		[Option("image-type",
			HelpText = "Specify the image type; png or jpg (or dds not compatible with height or flip).",
			SetName = "full")]
		public string ImageType { get; set; }

		[Option("without-bar-coords-only",
			SetName = "full",
			HelpText = "Only output images without any card bar coordinates.")]
		public bool WithoutBarCoords { get; set; }

		// card bar specific options

		[Option("bar-only",
			HelpText = "Extract only the card bar images.",
			SetName = "bar")]
		public bool BarArtOnly { get; set; }

		// dimensions

		[Option("bar-height",
			HelpText = "The desired height of the card bar images.")]
		public string BarHeight { get; set; }

		[Option("height",
			HelpText = "Specify the height of full size card art (height == width).")]
		public string Height { get; set; }

		[Option("crop-hidden",
			HelpText = "Crop the card bar image to remove the portion to the left that is usually hidden.")]
		public bool CropHidden { get; set; }

		// general options

		[Option('a', "keep-alpha",
			HelpText = "Keep the alpha channel in the exported images (removed by default).")]
		public bool KeepAlpha { get; set; }

		[Option('f', "no-flip",
			HelpText = "Do not flip the full images over the y-axis (does not effect card bar images).")]
		public bool NoFlip { get; set; }

		[Option('g', "group-by-set",
			HelpText = "Group the images into directories by card set.")]
		public bool Group { get; set; }

		[Option('n', "name",
			HelpText = "Add the card name to the exported file names.")]
		public bool Name { get; set; }

		[Option('s', "include-sets",
			HelpText = "Include cards from the specified sets only.")]
		public IEnumerable<string> Sets { get; set; }

		[Option('t', "include-types",
			HelpText = "Include cards of the specified types only.")]
		public IEnumerable<string> Types { get; set; }

		[Option("no-filters",
			HelpText = "Exports all cards, ignores default filtering.")]
		public bool NoFiltering { get; set; }

		[Option("save-map-file",
			HelpText = "Save the generated card to texture mapping file (ignored if using a map file as input).")]
		public bool SaveMapFile { get; set; }

		[Option('m', "map-file",
			HelpText = "Use a previously generated card to texture mapping file.")]
		public string MapFile { get; set; }

		[Option('o', "output-dir",
			HelpText = "The directory where the extracted files will be saved to.")]
		public string Output { get; set; }

		[Value(0,
			MetaName = "hearthstone dir",
			HelpText = "The hearthstone installation directory.",
			Required = true)]
		public string HsDirectory { get; set; }
	}

	[Verb("dump", HelpText = "Dump assets from unity3d files.")]
	internal class DumpOptions
	{
		[Option('o', "output-dir",
			HelpText = "The directory to save the extracted files to.")]
		public string Output { get; set; }

		[Option("text",
			HelpText = "Extract all text assets.")]
		public bool TextAssets { get; set; }

		[Option("texture",
			HelpText = "Extract all texture assets.")]
		public bool TextureAssets { get; set; }

		[Option("raw",
			HelpText = "Extract all assets in their raw binary state.")]
		public bool RawAssets { get; set; }

		[Value(0,
			Min = 1,
			MetaName = "input files",
			HelpText = "Input files to be processed.",
			Required = true)]
		public IEnumerable<string> InputFiles { get; set; }
	}

	[Verb("images", HelpText = "Extract images from all unity3d files in a directory.")]
	internal class ImageOptions
	{
		[Option('o', "output-dir",
			HelpText = "The directory to save the extracted files to.")]
		public string Output { get; set; }

		[Option('a', "alpha",
			HelpText = "Keep the alpha channel in the images.")]
		public bool Alpha { get; set; }

		[Option('f', "flip",
			HelpText = "Flip the images on the y-axis.")]
		public bool Flip { get; set; }

		[Option('x', "exclude",
			HelpText = "Exclude the given files")]
		public string Exclude { get; set; }


		[Value(0,
			MetaName = "source directory",
			HelpText = "The directory containing the unity files.",
			Required = true)]
		public string SourceDirectory { get; set; }
	}
}