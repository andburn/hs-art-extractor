using HsArtExtractor.Util;

namespace HsArtExtractor.Unity.Objects
{
	public class MonoScript
	{
		public string Name { get; set; }

		public MonoScript(BinaryBlock b)
		{
			int size = b.ReadInt();
			Name = b.ReadFixedString(size);
		}
	}
}