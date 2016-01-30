using System.IO;
using HsArtExtractor.Util;

namespace HsArtExtractor.Unity.Objects
{
	public class TextAsset
	{
		public string Name { get; set; }
		private byte[] _text;

		public TextAsset(BinaryBlock b)
		{
			var nameLen = b.ReadInt();
			Name = b.ReadFixedString(nameLen);
			b.Align(4);
			var textLen = b.ReadInt();
			_text = b.ReadBytes(textLen);
		}

		public void Save(string dir)
		{
			if (_text != null)
				File.WriteAllBytes(Path.Combine(dir, Name + ".txt"), _text);
		}
	}
}