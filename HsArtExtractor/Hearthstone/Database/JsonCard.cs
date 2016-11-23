namespace HsArtExtractor.Hearthstone.Database
{
	public class JsonCard
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string PlayerClass { get; set; }
		public string Set { get; set; }
		public string Type { get; set; }
		public bool Collectible { get; set; }
	}
}