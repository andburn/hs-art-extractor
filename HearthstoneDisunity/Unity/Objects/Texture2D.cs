using System;
using System.IO;
using HearthstoneDisunity.Image;
using HearthstoneDisunity.Util;

namespace HearthstoneDisunity.Unity.Objects
{
    public enum TextureFormat
    {
        Alpha8,
        ARGB4444,
        RGB24,
        RGBA32,
        ARGB32,
        RGB565,
        RGBA4444,
        BGRA32,
        DXT1,
        DXT5
    }

    public class Texture2D
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Size { get; set; }
        public TextureFormat Format { get; set; } // TODO: enum?
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

        private BinaryBlock _buffer;
        private long _dataPos;

        public Texture2D(BinaryBlock b)
        {
            _buffer = b;
            var nameLen = b.ReadInt();
            Name = b.ReadFixedString(nameLen);
            b.Align(4); // padding
            Width = b.ReadInt();
            Height = b.ReadInt();
            Size = b.ReadInt();
            Format = FormatToEnum(b.ReadInt());
            MipMap = b.ReadByte() == 1 ? true : false;
            IsReadable = b.ReadByte() == 1 ? true : false;
            IsReadAllowed = b.ReadByte() == 1 ? true : false;
            b.ReadByte(); // unknown?
            ImageCount = b.ReadInt();
            Dimension = b.ReadInt();
            // -- Texture Settings
            FilterMode = b.ReadInt();
            Aniso = b.ReadInt();
            MipBias = b.ReadInt();
            WrapMode = b.ReadInt();
            // -- Texture Settings
            LightmapFormat = b.ReadInt();
            ColorSpace = b.ReadInt();
            var dataSize = b.ReadInt();

            _dataPos = b.BaseStream.Position;

            if (dataSize != Size)
                throw new AssetException("Texture2D size mismatch [" + dataSize + "," + Size + "]");
        }

        private TextureFormat FormatToEnum(int format)
        {
            switch (format)
            {
                case 1: return TextureFormat.Alpha8;
                case 2: return TextureFormat.ARGB4444;
                case 3: return TextureFormat.RGB24;
                case 5: return TextureFormat.ARGB32;
                case 7: return TextureFormat.RGB565;
                case 13: return TextureFormat.RGBA4444;
                case 37: return TextureFormat.BGRA32;
                case 10: return TextureFormat.DXT1;
                case 12: return TextureFormat.DXT5;
                case 4:
                default: return TextureFormat.RGBA32;
            }
        }

        public void Save(string outputDir, string name = null)
        {
            _buffer.Seek(_dataPos);

            var filename = name;
            if (filename == null)
            {
                filename = Name;
            }
            // TODO: handle save overwriting
            // this wont work if overwriting!
            var tries = 0;
            var filenamedups = Directory.GetFiles(outputDir, filename + "*");
            while (filenamedups.Length > 0)
            {
                tries++;
                filenamedups = Directory.GetFiles(outputDir, filename + "_" + tries + "*");
            }
            if (tries > 0)
                filename += "_" + tries;

            // for HS check if DDS, otherwise assume TGA
            if (Format == TextureFormat.DXT1 || Format == TextureFormat.DXT5)
            {
                SaveAsDDS(outputDir, filename);
            }
            else
            {
                Console.WriteLine("Not a DDS file! Format = " + Format);
                SaveAsTGA(outputDir, filename);
            }
        }

        private void SaveAsDDS(string outputDir, string filename)
        {
            DDSHeader header = new DDSHeader();
            header.dwWidth = Width;
            header.dwHeight = Height;
            if (Format == TextureFormat.DXT1)
            {
                header.ddspf.dwFourCC = DDSPixelFormat.PF_DXT1;
            }
            else if (Format == TextureFormat.DXT5)
            {
                header.ddspf.dwFourCC = DDSPixelFormat.PF_DXT5;
            }
            else
            {
                Console.WriteLine("Not a dds file! - " + Name);
            }

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
                {
                    header.dwPitchOrLinearSize /= 2;
                }

                header.ddspf.dwFlags |= DDSPixelFormat.DDPF_FOURCC;
            }
            else
            {
                header.dwPitchOrLinearSize = (Width * Height * header.ddspf.dwRGBBitCount) / 8;
            }

            // TODO: make note of duplicates? overwritten by defaults
            using (BinaryWriter bw = new BinaryWriter(new FileStream(Path.Combine(outputDir, filename + ".dds"), FileMode.Create)))
            {
                header.write(bw);

                int remaining = Size;
                int chunk = 1024;
                int read = chunk;
                //byte[] buffer = new byte[chunk];

                byte[] bytes = new byte[Size + 1024];
                int numBytesToRead = Size;
                int numBytesRead = 0;
                do
                {
                    // TODO: use of buffer resetting position!! Need cache this anyway,
                    // or copy/rename file on disk!
                    // ~ infinite loop otherwise on multiple saves on same object
                    int n = _buffer.Read(bytes, numBytesRead, 1024);
                    numBytesRead += n;
                    numBytesToRead -= n;
                } while (numBytesToRead > 0);

                bw.Write(bytes, 0, Size);
            }
            //byte[] data = b.ReadBytes(dataSize);
            //Console.WriteLine("{0} {1}", Name, Size);
            //File.WriteAllBytes(Path.Combine(outputDir, name + ".bin"), data);
        }

        private void SaveAsTGA(string outputDir, string filename)
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
                    throw new Exception("Invalid texture format for TGA: " + Format);
            }

            Console.WriteLine("b4: " + _buffer.BaseStream.Length);
            convertToRGBA32();
            Console.WriteLine("ater: " + _buffer.BaseStream.Length);

            int mipMapCount = 1;

            if (MipMap)
            {
                mipMapCount = GetMipMapCount(Width, Height);
            }

            for (int i = 0; i < ImageCount; i++)
            {
                header.imageWidth = Width;
                header.imageHeight = Height;

                // only want first largest mipmap
                for (int j = 0; j < 1; j++)
                {
                    int imageSize = header.imageWidth * header.imageHeight * header.pixelDepth / 8;
                    Console.WriteLine("imagesize: " + imageSize);

                    if (tgaSaveMipMaps || j == 0)
                    {
                        var outname = Path.Combine(outputDir, filename + ".tga");
                        // TODO: make note of duplicates? overwritten by defaults
                        using (BinaryWriter bw = new BinaryWriter(new FileStream(outname, FileMode.Create)))
                        {
                            //ByteBuffer bbTga = ByteBuffer.allocateDirect(TGAHeader.SIZE + imageSize);
                            //bbTga.order(ByteOrder.LITTLE_ENDIAN);

                            header.write(bw);
                            // write image data
                            int remaining = imageSize;
                            int chunk = 1024;
                            int read = chunk;
                            //byte[] buffer = new byte[chunk];

                            byte[] bytes = new byte[imageSize + 1024];
                            int numBytesToRead = imageSize;
                            int numBytesRead = 0;
                            do
                            {
                                int n = _buffer.Read(bytes, numBytesRead, 1024);
                                numBytesRead += n;
                                numBytesToRead -= n;
                            } while (numBytesToRead > 0);

                            bw.Write(bytes, 0, imageSize);
                        }

                        var tga = new TargaImage(outname);
                        var bmp = tga.Image;
                        bmp.Save("E:\\Dump\\ImpMaster.png");

                        //	// write image data
                        //	_buffer.limit(_buffer.position() + imageSize);
                        //	bbTga.put(_buffer);
                        //	_buffer.limit(_buffer.capacity());

                        //	assert !bbTga.hasRemaining();

                        //	// write file
                        //	bbTga.rewind();

                        //	String fileName = tex.name;

                        //	if (tex.imageCount > 1) {
                        //		fileName += "_" + i;
                        //	}

                        //	if (tex.mipMap && tgaSaveMipMaps) {
                        //		fileName += "_mip_" + j;
                        //	}

                        //	setOutputFileName(fileName);
                        //	setOutputFileExtension("tga");
                        //	writeData(bbTga);
                        //} else {
                        //	_buffer.position(_buffer.position() + imageSize);
                        //}

                        // prepare for the next mip map
                        if (header.imageWidth > 1)
                        {
                            header.imageWidth /= 2;
                        }
                        if (header.imageHeight > 1)
                        {
                            header.imageHeight /= 2;
                        }
                    }
                }
            }
            //assert !_buffer.hasRemaining();
        }

        private void convertToRGBA32()
        {
            BinaryBlock imageBufferNew;
            if (Format == TextureFormat.RGB565)
            {
                // convert 16 bit RGB to 24 bit

                // TODO: int cast ?
                int bufferSize = (int)_buffer.BaseStream.Length;
                int newImageSize = (bufferSize / 2) * 3;
                Console.WriteLine("newSize: " + newImageSize);
                MemoryStream ms = new MemoryStream(newImageSize);
                Console.WriteLine("halfsize: " + (bufferSize / 2));
                byte[] pixel = new byte[3];
                for (int i = 0; i < (bufferSize / 2); i++)
                {
                    try
                    {
                        if (i > bufferSize / 2 - 32)
                            Console.WriteLine(i);
                        short pixelOld = _buffer.ReadShort();

                        pixel[0] = (byte)((pixelOld & 0xf800) >> 11);
                        pixel[1] = (byte)((pixelOld & 0x07e0) >> 5);
                        pixel[2] = (byte)(pixelOld & 0x001f);

                        // fix color mapping (http://stackoverflow.com/a/9069480)
                        pixel[0] = (byte)((pixel[0] * 527 + 23) >> 6);
                        pixel[1] = (byte)((pixel[1] * 259 + 33) >> 6);
                        pixel[2] = (byte)((pixel[2] * 527 + 23) >> 6);

                        ms.Write(pixel, 0, 3);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("caught, i=" + i + "[" + e.Message + "]");
                        //throw e;
                        break;
                    }
                }

                //assert !imageBuffer.hasRemaining();
                //assert !imageBufferNew.hasRemaining();

                //imageBufferNew.rewind();
                //imageBuffer = imageBufferNew;
                imageBufferNew = new BinaryBlock(ms);
                imageBufferNew.BaseStream.Position = 0;
            }
            else
            {
                // error for hs
                throw new Exception("unknown format");
            }

            _buffer = imageBufferNew;
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