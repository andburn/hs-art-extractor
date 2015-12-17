using System.Collections.Generic;
using System.Xml.Serialization;

namespace HearthstoneDisunity.Hearthstone.CardArt
{
    [XmlRoot("CardArtDefs")]
    public class CardArtDefs
    {
        public string Version { get; set; }

        [XmlElement("Card")]
        public List<ArtCard> Cards { get; set; }
    }
}