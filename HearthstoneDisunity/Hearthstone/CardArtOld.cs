using HearthstoneDisunity.Util;

namespace HearthstoneDisunity.Hearthstone
{
    public class CardArtOld
    {
        public string Name { get; set; }
        public string PortraitPath { get; set; }
        public CardMaterial DeckBar { get; set; }
        public CardMaterial Portrait { get; set; }

        public string PortraitName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(PortraitPath))
                {
                    return null;
                }
                else
                {
                    return StringUtils.GetFilenameNoExt(PortraitPath);
                }
            }
        }

        public string PortraitFileDir
        {
            get
            {
                if (string.IsNullOrWhiteSpace(PortraitPath))
                {
                    return null;
                }
                else
                {
                    return StringUtils.GetFilePathParentDir(PortraitPath);
                }
            }
        }

        public override string ToString()
        {
            return DebugUtils.AllPropsToString(this);
        }
    }
}