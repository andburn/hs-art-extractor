using HearthstoneDisunity.Util;
using System.IO;

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

        public CardDef(BinaryBlock b)
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

        //public FilePointer GameObject { get; private set; }
        //public bool Enabled { get; private set; }
        //public FilePointer MonoScript { get; private set; }
        //public string Name { get; private set; }
        //public string PortratitTexturePath { get; private set; }
        //public string PremiumPortraitMaterialPath { get; private set; }
        //public string PremiumPortraitTexturePath { get; private set; }
        //public FilePointer DeckCardBarPortrait { get; private set; }
        //public FilePointer EnchantmentPortrait { get; private set; }
        //public FilePointer HistoryTileHalfPortrait { get; private set; }
        //public FilePointer HistoryTileFullPortrait { get; private set; }

        public void Save(string dir, string name = "default")
        {
            string outFile = Name;
            if (string.IsNullOrEmpty(Name))
            {
                outFile = name;
            }
            outFile = Path.Combine(dir, outFile + ".txt");
            // TODO: duplicate check, => rename _2
            using (StreamWriter sw = new StreamWriter(outFile, false))
            {
                sw.WriteLine("CardDef");
                sw.WriteLine("\tGameObject: " + GameObject);
                sw.WriteLine("\tName: " + Name);
                sw.WriteLine("\tPortraitTexPath: " + PortratitTexturePath);
                sw.WriteLine("\tPremiumMat: " + PremiumPortraitMaterialPath);
                sw.WriteLine("\tPremiumTex: " + PremiumPortraitTexturePath);
                sw.WriteLine("\tDeckCardBar: " + DeckCardBarPortrait);
                sw.WriteLine("\tEnchantPortrait: " + EnchantmentPortrait);
                sw.WriteLine("\tHistoryTileHalfPortrait: " + HistoryTileHalfPortrait);
                sw.WriteLine("\tHistoryTileFullPortrait: " + HistoryTileFullPortrait);
            }
        }
    }
}
