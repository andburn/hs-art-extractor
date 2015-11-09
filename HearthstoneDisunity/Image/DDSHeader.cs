using HearthstoneDisunity.Util;
using System;
using System.IO;

namespace HearthstoneDisunity.Image
{
    public class DDSHeader
    {
        public static readonly int DDS_MAGIC = StringUtils.MakeID("DDS ");
        public const int DDS_STRUCT_SIZE = 124;

        public const int SIZE = DDS_STRUCT_SIZE + 4;

        public const int DDS_FOURCC = 0x00000004; // DDPF_FOURCC
        public const int DDS_RGB = 0x00000040; // DDPF_RGB
        public const int DDS_RGBA = 0x00000041; // DDPF_RGB | DDPF_ALPHAPIXELS
        public const int DDS_LUMINANCE = 0x00020000; // DDPF_LUMINANCE
        public const int DDS_LUMINANCEA = 0x00020001; // DDPF_LUMINANCE | DDPF_ALPHAPIXELS
        public const int DDS_ALPHA = 0x00000002; // DDPF_ALPHA
        public const int DDS_PAL8 = 0x00000020; // DDPF_PALETTEINDEXED8

        public const int DDS_HEADER_FLAGS_TEXTURE = 0x00001007; // DDSD_CAPS | DDSD_HEIGHT | DDSD_WIDTH | DDSD_PIXELFORMAT
        public const int DDS_HEADER_FLAGS_MIPMAP = 0x00020000; // DDSD_MIPMAPCOUNT
        public const int DDS_HEADER_FLAGS_VOLUME = 0x00800000; // DDSD_DEPTH
        public const int DDS_HEADER_FLAGS_PITCH = 0x00000008; // DDSD_PITCH
        public const int DDS_HEADER_FLAGS_LINEARSIZE = 0x00080000; // DDSD_LINEARSIZE

        public const int DDS_HEIGHT = 0x00000002; // DDSD_HEIGHT
        public const int DDS_WIDTH = 0x00000004; // DDSD_WIDTH

        public const int DDS_SURFACE_FLAGS_TEXTURE = 0x00001000; // DDSCAPS_TEXTURE
        public const int DDS_SURFACE_FLAGS_MIPMAP = 0x00400008; // DDSCAPS_COMPLEX | DDSCAPS_MIPMAP
        public const int DDS_SURFACE_FLAGS_CUBEMAP = 0x00000008; // DDSCAPS_COMPLEX

        public const int DDS_CUBEMAP_POSITIVEX = 0x00000600; // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_POSITIVEX
        public const int DDS_CUBEMAP_NEGATIVEX = 0x00000a00; // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_NEGATIVEX
        public const int DDS_CUBEMAP_POSITIVEY = 0x00001200; // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_POSITIVEY
        public const int DDS_CUBEMAP_NEGATIVEY = 0x00002200; // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_NEGATIVEY
        public const int DDS_CUBEMAP_POSITIVEZ = 0x00004200; // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_POSITIVEZ
        public const int DDS_CUBEMAP_NEGATIVEZ = 0x00008200; // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_NEGATIVEZ

        public const int DDS_CUBEMAP_ALLFACES = (DDS_CUBEMAP_POSITIVEX | DDS_CUBEMAP_NEGATIVEX | DDS_CUBEMAP_POSITIVEY | DDS_CUBEMAP_NEGATIVEY | DDS_CUBEMAP_POSITIVEZ | DDS_CUBEMAP_NEGATIVEZ);

        public const int DDS_CUBEMAP = 0x00000200; // DDSCAPS2_CUBEMAP

        public const int DDS_FLAGS_VOLUME = 0x00200000; // DDSCAPS2_VOLUME

        public int dwMagic = DDS_MAGIC;
        public int dwSize = DDS_STRUCT_SIZE;
        public int dwFlags = DDS_HEADER_FLAGS_TEXTURE;
        public int dwHeight;
        public int dwWidth;
        public int dwPitchOrLinearSize;
        public int dwDepth;
        public int dwMipMapCount;
        public int[] dwReserved1 = new int[11];
        public DDSPixelFormat ddspf = new DDSPixelFormat();
        public int dwCaps = DDS_SURFACE_FLAGS_TEXTURE;
        public int dwCaps2;
        public int dwCaps3;
        public int dwCaps4;
        public int dwReserved2;

        public void read(BinaryReader input)
        {
            dwMagic = input.ReadInt32();
            dwSize = input.ReadInt32();
            dwFlags = input.ReadInt32();
            dwHeight = input.ReadInt32();
            dwWidth = input.ReadInt32();
            dwPitchOrLinearSize = input.ReadInt32();
            dwDepth = input.ReadInt32();
            dwMipMapCount = input.ReadInt32();

            for (int i = 0; i < dwReserved1.Length; i++)
            {
                dwReserved1[i] = input.ReadInt32();
            }

            ddspf.Read(input);

            dwCaps = input.ReadInt32();
            dwCaps2 = input.ReadInt32();
            dwCaps3 = input.ReadInt32();
            dwCaps4 = input.ReadInt32();
            dwReserved2 = input.ReadInt32();
        }

        public void write(BinaryWriter output)
        {
            output.Write(dwMagic);
            output.Write(dwSize);
            output.Write(dwFlags);
            output.Write(dwHeight);
            output.Write(dwWidth);
            output.Write(dwPitchOrLinearSize);
            output.Write(dwDepth);
            output.Write(dwMipMapCount);

            for (int i = 0; i < dwReserved1.Length; i++)
            {
                output.Write(dwReserved1[i]);
            }

            ddspf.Write(output);

            output.Write(dwCaps);
            output.Write(dwCaps2);
            output.Write(dwCaps3);
            output.Write(dwCaps4);
            output.Write(dwReserved2);
        }

        public int MipMapCount
        {
            get
            {
                int mipMapCount = 1;
                for (int dim = Math.Max(dwWidth, dwHeight); dim > 1; dim /= 2)
                {
                    mipMapCount++;
                }
                return mipMapCount;
            }
        }
    }
}
