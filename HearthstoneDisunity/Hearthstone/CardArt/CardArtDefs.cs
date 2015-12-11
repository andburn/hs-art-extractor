using System.Collections.Generic;
using System.Xml.Serialization;

namespace HearthstoneDisunity.Hearthstone.CardArt
{
    [XmlRoot("CardArtDefs")]
    public class CardArtDefs
    {
        public string Patch { get; set; }

        [XmlElement("Card")]
        public List<Card> Cards { get; set; }
    }
}