using HearthstoneDisunity.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HearthstoneDisunity.Hearthstone
{
    public class CardArt
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
