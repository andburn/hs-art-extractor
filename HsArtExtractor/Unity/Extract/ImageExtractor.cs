using System;
using System.Drawing;
using System.IO;
using AndBurn.DDSReader;
using HsArtExtractor.Image;
using HsArtExtractor.Unity.Objects;
using HsArtExtractor.Util;

namespace HsArtExtractor.Unity.Extract
{
	internal class ImageExtractor : IExtractor
	{
		private bool _alpha;
		private bool _flip;

		public ImageExtractor()
		{
			_alpha = true;
			_flip = true;
		}

		public ImageExtractor(bool alpha, bool flip)
		{
			_alpha = alpha;
			_flip = flip;
		}

		public void Extract(ObjectData data, string dir)
		{
			var unityClass = (UnityClass)data.Info.ClassId;
			if(unityClass == UnityClass.Texture2D)
			{
				var path = Path.Combine(dir, unityClass.ToString());
				Directory.CreateDirectory(path);

				var block = BinaryBlock.Create(data.Buffer);
				var texture = new Texture2D(block);
				try
				{
					SaveImage(texture, path);
				}
				catch(Exception e)
				{
					Console.WriteLine("Failed on {0} ({1})", texture.Name, e.Message);
				}
			}
		}

		private void SaveImage(Texture2D tex, string dir)
		{
			Bitmap original = null;
			if(tex.IsDDS)
			{
				original = DDS.LoadImage(tex.Image, _alpha);
				if(_flip)
					original.RotateFlip(RotateFlipType.RotateNoneFlipY);
			}
			else
			{
				// Assumimg it is TGA, alpha??
				var tga = new TargaImage(new MemoryStream(tex.Image));
				original = tga.Image;
				if(!_flip) // tga already flipped
					original.RotateFlip(RotateFlipType.RotateNoneFlipY);
			}
			original.Save(Path.Combine(dir, tex.Name + ".png"));
		}
	}
}