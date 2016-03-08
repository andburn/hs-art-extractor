using System.Collections.Generic;

namespace HsArtExtractor.Hearthstone
{
	public class CardArtExtractorOptions
	{
		public bool BarArtOnly { get; set; } = false;
		public bool FullArtOnly { get; set; } = false;
		public string OutputDir { get; set; } = null;
		public string HearthstoneDir { get; set; } = null;
		public string MapFile { get; set; } = null;
		public bool PreserveAlphaChannel { get; set; } = false;
		public bool AddCardName { get; set; } = false;
		public bool GroupBySet { get; set; } = false;
		public List<string> Sets { get; set; } = new List<string>();
		public List<string> Types { get; set; } = new List<string>();
		public bool NoFiltering { get; set; } = false;
		public bool SaveMapFile { get; set; } = false;
		public int Height { get; set; } = 0;
		public bool FlipY { get; set; } = true;
		public int BarHeight { get; set; } = 0;
		public bool WithoutBarCoords { get; set; } = false;
		public string ImageType { get; set; } = "png";

		public override string ToString()
		{
			return Util.DebugUtils.AllPropsToString(this);
		}
	}
}