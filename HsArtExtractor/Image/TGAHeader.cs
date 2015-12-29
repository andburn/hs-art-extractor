using System.IO;

namespace HsArtExtractor.Image
{
    public class TGAHeader
    {
        public const int SIZE = 18;
        public byte idLength;
        public byte colorMapType;
        public byte imageType;
        public int cmFirstIndex;
        public int cmLength;
        public byte cmEntrySize;
        public int originX;
        public int originY;
        public int imageWidth;
        public int imageHeight;
        public byte pixelDepth;
        public byte imageDesc;

        public void read(BinaryReader input)
        {
            idLength = input.ReadByte();
            colorMapType = input.ReadByte();
            imageType = input.ReadByte();
            cmFirstIndex = input.ReadUInt16();
            cmLength = input.ReadUInt16();
            cmEntrySize = input.ReadByte();
            originX = input.ReadUInt16();
            originY = input.ReadUInt16();
            imageWidth = input.ReadUInt16();
            imageHeight = input.ReadUInt16();
            pixelDepth = input.ReadByte();
            imageDesc = input.ReadByte();
        }

        public void write(BinaryWriter output)
        {
            output.Write(idLength);
            output.Write(colorMapType);
            output.Write(imageType);
            output.Write((short)cmFirstIndex);
            output.Write((short)cmLength);
            output.Write(cmEntrySize);
            output.Write((short)originX);
            output.Write((short)originY);
            output.Write((short)imageWidth);
            output.Write((short)imageHeight);
            output.Write(pixelDepth);
            output.Write(imageDesc);
        }
    }
}
