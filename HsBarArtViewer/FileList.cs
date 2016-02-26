using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using HsArtExtractor.Hearthstone.CardArt;
using HsArtExtractor.Util;

namespace HsBarArtViewer
{
	internal class FileList
	{
		private FileObject[] _objects;
		private int _current;
		private int _length;

		public FileList()
		{
			_objects = new FileObject[0];
			_current = 0;
			_length = 0;
		}

		public FileList(string[] filenames)
		{
			_current = 0;
			_length = filenames.Length;
			_objects = new FileObject[_length];
			for (var i = 0; i < _length; i++)
			{
				_objects[i] = new FileObject(filenames[i]);
			}
		}

		public int Size
		{
			get { return _length; }
		}

		public int Index
		{
			get { return _current; }
		}

		public FileObject First
		{
			get
			{
				if (IsEmpty())
					return null;
				return _objects[0];
			}
		}

		public FileObject Last
		{
			get
			{
				if (IsEmpty())
					return null;
				return _objects[_length - 1];
			}
		}

		public FileObject Current
		{
			get
			{
				if (IsEmpty())
					return null;
				return _objects[_current];
			}
		}

		public FileObject Next
		{
			get
			{
				if (IsEmpty())
					return null;

				_current++;
				if (_current >= _length)
					_current = 0;

				return _objects[_current];
			}
		}

		public FileObject Previous
		{
			get
			{
				if (IsEmpty())
					return null;

				_current--;
				if (_current < 0)
					_current = _length - 1;

				return _objects[_current];
			}
		}

		// peek at object n elements from current, without moving index
		public FileObject Peek(int fromCurrent)
		{
			if (IsEmpty())
				return null;

			var i = (_current + fromCurrent) % _length;
			return _objects[i];
		}

		public void UpdateBars()
		{
			foreach (var obj in _objects)
				obj.UpdateBar();
		}

		public bool IsEmpty()
		{
			return _objects.Length <= 0;
		}
	}

	internal class FileObject
	{
		public string FilePath { get; set; }
		public string CardId { get; set; }
		public ArtCardBarWrapper CardBar { get; set; }

		public BitmapImage Image
		{
			get
			{
				if (string.IsNullOrEmpty(FilePath))
					return null;
				var bmp = new Bitmap(FilePath);
				// force resize
				if (bmp.Width > 512)
					bmp = new Bitmap(bmp, 512, 512);
				return BitmapToImageSource(bmp);
			}
		}

		public FileObject(string filename)
		{
			FilePath = filename;
			CardId = StringUtils.GetFilenameNoExt(filename);
			UpdateBar();
		}

		public void UpdateBar()
		{
			if (CardArtDb.All.ContainsKey(CardId))
				CardBar = new ArtCardBarWrapper(CardArtDb.All[CardId]);
			else
				CardBar = new ArtCardBarWrapper();
		}

		private BitmapImage BitmapToImageSource(Bitmap bitmap)
		{
			using (MemoryStream memory = new MemoryStream())
			{
				bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
				memory.Position = 0;
				BitmapImage bitmapimage = new BitmapImage();
				bitmapimage.BeginInit();
				bitmapimage.StreamSource = memory;
				bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapimage.EndInit();

				return bitmapimage;
			}
		}
	}
}