using HearthstoneDisunity.Util;
using System.IO;

namespace HearthstoneDisunity.Image
{
    public class DDSPixelFormat
    {
        public const int DDSPF_STRUCT_SIZE = 32;

        public const int DDPF_ALPHAPIXELS = 0x1;
        public const int DDPF_ALPHA = 0x2;
        public const int DDPF_FOURCC = 0x4;
        public const int DDPF_RGB = 0x40;
        public const int DDPF_RGBA = DDPF_RGB | DDPF_ALPHAPIXELS;

        public static readonly int PF_DXT1 = StringUtils.MakeID("DXT1");
        public static readonly int PF_DXT3 = StringUtils.MakeID("DXT3");
        public static readonly int PF_DXT5 = StringUtils.MakeID("DXT5");

        public int dwSize = DDSPF_STRUCT_SIZE;
        public int dwFlags;
        public int dwFourCC;
        public int dwRGBBitCount;
        public int dwRBitMask;
        public int dwGBitMask;
        public int dwBBitMask;
        public int dwABitMask;

        public void Read(BinaryReader input)
        {
            dwSize = input.ReadInt32();
            dwFlags = input.ReadInt32();
            dwFourCC = input.ReadInt32();
            dwRGBBitCount = input.ReadInt32();
            dwRBitMask = input.ReadInt32();
            dwGBitMask = input.ReadInt32();
            dwBBitMask = input.ReadInt32();
            dwABitMask = input.ReadInt32();
        }

        public void Write(BinaryWriter output)
        {
            output.Write(dwSize);
            output.Write(dwFlags);
            output.Write(dwFourCC);
            output.Write(dwRGBBitCount);
            output.Write(dwRBitMask);
            output.Write(dwGBitMask);
            output.Write(dwBBitMask);
            output.Write(dwABitMask);
        }
    }
}
