using System.Collections.Generic;

namespace HsArtExtractor.Hearthstone
{
	public class CardArtExtractorOptions
	{
		public string OutputDir { get; set; }
		public string HearthstoneDir { get; set; }
		public string MapFile { get; set; }
		public bool PreserveAlphaChannel { get; set; }
		public bool AddCardName { get; set; }
		public bool GroupBySet { get; set; }
		public List<string> Sets { get; set; }
		public List<string> Types { get; set; }
		public bool NoFiltering { get; set; }
		public bool SaveMapFile { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public bool FlipY { get; set; } = true;
		public int BarWidth { get; set; }
		public int BarHeight { get; set; }

		public override string ToString()
		{
			return Util.DebugUtils.AllPropsToString(this);
		}
	}
}