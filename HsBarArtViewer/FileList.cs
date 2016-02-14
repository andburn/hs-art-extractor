namespace HsBarArtViewer
{
	internal class FileList
	{
		private string[] _filenames;
		private int _current;
		private int _length;

		public FileList()
		{
			_filenames = new string[0];
			_current = 0;
			_length = 0;
		}

		public FileList(string[] filenames)
		{
			_filenames = filenames;
			_current = 0;
			_length = _filenames.Length;
		}

		public string First()
		{
			return _filenames[0];
		}

		public string Last()
		{
			return _filenames[_length - 1];
		}

		public string Current()
		{
			return _filenames[_current];
		}

		public string Next()
		{
			_current++;
			if (_current >= _length)
				_current = 0;
			return _filenames[_current];
		}

		public string Previous()
		{
			_current--;
			if (_current < 0)
				_current = _length - 1;
			return _filenames[_current];
		}
	}
}