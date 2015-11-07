using HearthstoneDisunity.Util;

namespace HearthstoneDisunity.Unity.Objects
{
    public class CardDef
    {
        public FilePointer GameObject { get; private set; }
        public bool Enabled { get; private set; }
        public FilePointer MonoScript { get; private set; }
        public string Name { get; private set; }
        public string PortratitTexturePath { get; private set; }
        public string PremiumPortraitMaterialPath { get; private set; }
        public string PremiumPortraitTexturePath { get; private set; }
        public FilePointer DeckCardBarPortrait { get; private set; }
        public FilePointer EnchantmentPortrait { get; private set; }
        public FilePointer HistoryTileHalfPortrait { get; private set; }
        public FilePointer HistoryTileFullPortrait { get; private set; }

        public CardDef(BinaryFileReader b)
        {
            GameObject = new FilePointer(b.ReadInt(), b.ReadLong());
            Enabled = b.ReadUnsignedByte() == 1 ? true : false;
            b.Align(4);

            MonoScript = new FilePointer(b.ReadInt(), b.ReadLong());
            //b.Align(4);

            int size = b.ReadInt();
            Name = b.ReadFixedString(size);
            b.Align(4);

            size = b.ReadInt();
            PortratitTexturePath = b.ReadFixedString(size);
            b.Align(4);

            size = b.ReadInt();
            PremiumPortraitMaterialPath = b.ReadFixedString(size);
            b.Align(4);

            size = b.ReadInt();
            PremiumPortraitTexturePath = b.ReadFixedString(size);
            b.Align(4);

            DeckCardBarPortrait = new FilePointer(b.ReadInt(), b.ReadLong());
            //b.Align(4);

            EnchantmentPortrait = new FilePointer(b.ReadInt(), b.ReadLong());
            //b.Align(4);

            HistoryTileHalfPortrait = new FilePointer(b.ReadInt(), b.ReadLong());
            //b.Align(4);

            HistoryTileFullPortrait = new FilePointer(b.ReadInt(), b.ReadLong());
            //b.Align(4);

            // Ignore rest of the file
        }

        public override string ToString()
        {
            return string.Format("Name: '{0}' Tex: '{1}'", PortratitTexturePath);
        }
    }
}
