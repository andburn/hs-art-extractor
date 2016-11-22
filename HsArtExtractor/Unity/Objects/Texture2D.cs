using System;
using System.IO;
using HsArtExtractor.Image;
using HsArtExtractor.Util;

namespace HsArtExtractor.Unity.Objects
{
	public class Texture2D
	{
		public string Name { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public int Size { get; set; }
		public TextureFormat Format { get; set; }
		public bool MipMap { get; set; }
		public bool IsReadable { get; set; }
		public bool IsReadAllowed { get; set; }
		public int ImageCount { get; set; }
		public int Dimension { get; set; }
		public int LightmapFormat { get; set; }
		public int ColorSpace { get; set; }
		public int FilterMode { get; set; }
		public int Aniso { get; set; }
		public int MipBias { get; set; }
		public int WrapMode { get; set; }

		private BinaryBlock _data;
		private byte[] _image;

		public byte[] Image
		{
			get
			{
				return _image;
			}
			private set
			{
				_image = value;
			}
		}

		public Texture2D(BinaryBlock b)
		{
			var nameLen = b.ReadInt();
			Name = b.ReadFixedString(nameLen);
			b.Align(4);
			Width = b.ReadInt();
			Height = b.ReadInt();
			Size = b.ReadInt();
			Format = (TextureFormat)b.ReadInt();
			var mmb = b.ReadInt();
			MipMap = mmb == 1 ? true : false;
			var irb = b.ReadByte();
			IsReadable = irb == 1 ? true : false;
			var irab = b.ReadByte();
			IsReadAllowed = irab == 1 ? true : false;
			b.Align(4); // unknown
			ImageCount = b.ReadInt();
			Dimension = b.ReadInt();
			FilterMode = b.ReadInt();
			Aniso = b.ReadInt();
			MipBias = b.ReadInt();
			WrapMode = b.ReadInt();
			LightmapFormat = b.ReadInt();
			ColorSpace = b.ReadInt();
			var dataSize = b.ReadInt();

			if (dataSize != Size)
				throw new AssetException("Texture2D size mismatch [" + dataSize + "," + Size + "]");

			_data = BinaryBlock.Create(b.ReadBytes(dataSize));
			Image = CreateImage();
		}

		public bool IsDDS
		{
			get
			{
				return Format == TextureFormat.DXT1 || Format == TextureFormat.DXT5;
			}
		}

		private byte[] CreateImage()
		{
			var image = new byte[0];
			try
			{
				if (Format == TextureFormat.DXT1Crunched || Format == TextureFormat.DXT5Crunched)
				{
					throw new TextureException("DXT Crunched not supported: " + Name);
				}
				else if (Format == TextureFormat.DXT1 || Format == TextureFormat.DXT5)
				{
					image = CreateDDS();
				}
				// for HS card art is either DDS (possibly crunched) or TGA
				else
				{
					Logger.Log("Not a DDS file! Format = " + Format + " (assuming TGA)");
					image = CreateTGA();
				}
			}
			catch (TextureException te)
			{
				var message = $"Image decoding failed for texture {Name}";
				Console.WriteLine(message);
				Logger.Log(LogLevel.ERROR, $"{message} ({te.Message})");
			}
			return image;
		}

		private byte[] CreateTGA()
		{
			bool tgaSaveMipMaps = false;
			TGAHeader header = new TGAHeader();

			switch (Format)
			{
				case TextureFormat.Alpha8:
					header.imageType = 3;
					header.pixelDepth = 8;
					break;

				case TextureFormat.RGBA32:
				case TextureFormat.ARGB32:
				case TextureFormat.BGRA32:
				case TextureFormat.RGBA4444:
				case TextureFormat.ARGB4444:
					header.imageType = 2;
					header.pixelDepth = 32;
					break;

				case TextureFormat.RGB24:
				case TextureFormat.RGB565:
					header.imageType = 2;
					header.pixelDepth = 24;
					break;

				default:
					throw new TextureException("Invalid format for TGA: " + Format + "(" + Name + ")");
			}

			// Convert image to RGBA32
			ConvertToRGBA32();

			int mipMapCount = 1;
			if (MipMap)
				mipMapCount = GetMipMapCount(Width, Height);

			for (int i = 0; i < ImageCount; i++)
			{
				header.imageWidth = Width;
				header.imageHeight = Height;

				// only want first (largest) mipmap
				for (int j = 0; j < 1; j++)
				{
					int imageSize = header.imageWidth * header.imageHeight * header.pixelDepth / 8;
					if (tgaSaveMipMaps || j == 0)
					{
						//ByteBuffer bbTga = ByteBuffer.allocateDirect(TGAHeader.SIZE + imageSize);
						//bbTga.order(ByteOrder.LITTLE_ENDIAN);

						_data.ResetPos();
						using (var ms = new MemoryStream())
						using (BinaryWriter bw = new BinaryWriter(ms))
						{
							byte[] bytes = new byte[imageSize];
							_data.Read(bytes, 0, imageSize);

							header.write(bw);
							bw.Write(bytes);

							return ms.ToArray();
						}
					}
				}
			}
			// something went wrong, if get here
			Logger.Log(LogLevel.ERROR, "TGA creation failed for {0}", Name);
			return new byte[0];
		}

		private byte[] CreateDDS()
		{
			DDSHeader header = new DDSHeader();
			header.dwWidth = Width;
			header.dwHeight = Height;

			if (Format == TextureFormat.DXT1)
				header.ddspf.dwFourCC = DDSPixelFormat.PF_DXT1;
			else if (Format == TextureFormat.DXT5)
				header.ddspf.dwFourCC = DDSPixelFormat.PF_DXT5;
			else
				throw new TextureException("Unsupported DDS format " + Format + " (" + Name + ")");

			if (MipMap)
			{
				header.dwFlags |= DDSHeader.DDS_HEADER_FLAGS_MIPMAP;
				header.dwCaps |= DDSHeader.DDS_SURFACE_FLAGS_MIPMAP;
				header.dwMipMapCount = header.MipMapCount;
			}

			header.dwFlags |= DDSHeader.DDS_HEADER_FLAGS_LINEARSIZE;
			if (header.ddspf.dwFourCC != 0)
			{
				header.dwPitchOrLinearSize = header.dwWidth * header.dwHeight;
				if (Format == TextureFormat.DXT1)
					header.dwPitchOrLinearSize /= 2;
				header.ddspf.dwFlags |= DDSPixelFormat.DDPF_FOURCC;
			}
			else
			{
				header.dwPitchOrLinearSize = (Width * Height * header.ddspf.dwRGBBitCount) / 8;
			}

			_data.ResetPos();
			using (var ms = new MemoryStream())
			using (BinaryWriter bw = new BinaryWriter(ms))
			{
				byte[] bytes = new byte[Size];
				_data.Read(bytes, 0, Size);

				header.write(bw);
				bw.Write(bytes);

				return ms.ToArray();
			}
		}

		public void Save(string outputDir, string name = null)
		{
			var filename = name;
			if (filename == null)
				filename = Name;

			var fileExt = ".tga";
			if (Format == TextureFormat.DXT1 || Format == TextureFormat.DXT5)
				fileExt = ".dds";

			filename += fileExt;
			var outPath = Path.Combine(outputDir, filename);

			// check for file existing
			// TODO: maybe add StringUtils.GetFilenameNoOverwrite
			if (File.Exists(outPath))
				Logger.Log(LogLevel.WARN, "Saving texture, file already exists ({0}) it will be overwritten.", filename);

			File.WriteAllBytes(outPath, _image);
		}

		private void ConvertToRGBA32()
		{
			BinaryBlock imageBuffer = null;
			if (Format == TextureFormat.RGBA32 || Format == TextureFormat.ARGB32)
			{
				// convert ARGB and RGBA directly by swapping the bytes to get BGRA
				int bufferSize = (int)_data.BaseStream.Length;
				MemoryStream ms = new MemoryStream(bufferSize);

				byte[] pixelOld = new byte[4];
				byte[] pixelNew = new byte[4];
				for (int i = 0; i < bufferSize / 4; i++)
				{
					_data.Read(pixelOld, 0, pixelOld.Length);

					if (Format == TextureFormat.ARGB32)
					{
						// ARGB -> BGRA
						pixelNew[0] = pixelOld[3];
						pixelNew[1] = pixelOld[2];
						pixelNew[2] = pixelOld[1];
						pixelNew[3] = pixelOld[0];
					}
					else
					{
						// RGBA -> BGRA
						pixelNew[0] = pixelOld[2];
						pixelNew[1] = pixelOld[1];
						pixelNew[2] = pixelOld[0];
						pixelNew[3] = pixelOld[3];
					}

					ms.Write(pixelNew, 0, 4);
				}

				imageBuffer = new BinaryBlock(ms);
			}
			else if (Format == TextureFormat.RGB565)
			{
				// Convert 16 bit RGB to 24 bit
				int bufferSize = (int)_data.BaseStream.Length;
				int halfSize = bufferSize / 2;
				int newImageSize = halfSize * 3;

				MemoryStream ms = new MemoryStream(newImageSize);
				byte[] pixel = new byte[3];
				for (int i = 0; i < halfSize; i++)
				{
					try
					{
						short pixelOld = _data.ReadShort();
						pixel[0] = (byte)((pixelOld & 0xf800) >> 11);
						pixel[1] = (byte)((pixelOld & 0x07e0) >> 5);
						pixel[2] = (byte)(pixelOld & 0x001f);
						// fix color mapping http://stackoverflow.com/a/9069480
						pixel[0] = (byte)((pixel[0] * 527 + 23) >> 6);
						pixel[1] = (byte)((pixel[1] * 259 + 33) >> 6);
						pixel[2] = (byte)((pixel[2] * 527 + 23) >> 6);
						ms.Write(pixel, 0, 3);
					}
					catch (Exception e)
					{
						Logger.Log(LogLevel.ERROR, "Failed RGBA32 conversion ({0}), {1}", Name, e.Message);
						return;
					}
				}
				imageBuffer = new BinaryBlock(ms);
			}
			else if (Format == TextureFormat.ARGB4444 || Format == TextureFormat.RGBA4444)
			{
				// convert 16 bit RGBA/ARGB to 32 bit BGRA
				int bufferSize = (int)_data.BaseStream.Length;
				int newImageSize = bufferSize * 2;
				MemoryStream ms = new MemoryStream(newImageSize);

				byte[] pixelOld = new byte[4];
				byte[] pixelNew = new byte[4];
				for (int i = 0; i < bufferSize / 2; i++)
				{
					int pixelOldShort = _data.ReadShort();

					pixelOld[0] = (byte)((pixelOldShort & 0xf000) >> 12);
					pixelOld[1] = (byte)((pixelOldShort & 0x0f00) >> 8);
					pixelOld[2] = (byte)((pixelOldShort & 0x00f0) >> 4);
					pixelOld[3] = (byte)(pixelOldShort & 0x000f);

					// convert range
					pixelOld[0] <<= 4;
					pixelOld[1] <<= 4;
					pixelOld[2] <<= 4;
					pixelOld[3] <<= 4;

					if (Format == TextureFormat.ARGB4444)
					{
						// ARBG -> BGRA
						pixelNew[0] = pixelOld[3];
						pixelNew[1] = pixelOld[2];
						pixelNew[2] = pixelOld[1];
						pixelNew[3] = pixelOld[0];
					}
					else
					{
						// RBGA -> BGRA
						pixelNew[0] = pixelOld[2];
						pixelNew[1] = pixelOld[1];
						pixelNew[2] = pixelOld[0];
						pixelNew[3] = pixelOld[3];
					}

					ms.Write(pixelNew, 0, 4);
				}
				imageBuffer = new BinaryBlock(ms);
			}
			else if (Format == TextureFormat.RGB24)
			{
				int bufferSize = (int)_data.BaseStream.Length;
				int thirdSize = bufferSize / 3;
				MemoryStream ms = new MemoryStream(bufferSize);

				// convert RGB directly to BGR
				byte[] pixelOld = new byte[3];
				byte[] pixelNew = new byte[3];
				for (int i = 0; i < thirdSize; i++)
				{
					_data.Read(pixelOld, 0, 3);

					pixelNew[0] = pixelOld[2];
					pixelNew[1] = pixelOld[1];
					pixelNew[2] = pixelOld[0];

					ms.Write(pixelNew, 0, 3);
				}
				imageBuffer = new BinaryBlock(ms);
			}
			else
			{
				// TODO: do nothing, add check to caller
				return;
			}

			_data = imageBuffer;
		}

		private int GetMipMapCount(int width, int height)
		{
			int mipMapCount = 1;
			for (int dim = Math.Max(width, height); dim > 1; dim /= 2)
			{
				mipMapCount++;
			}
			return mipMapCount;
		}
	}
}