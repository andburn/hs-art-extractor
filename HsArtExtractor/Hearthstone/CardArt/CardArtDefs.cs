using System.Collections.Generic;
using System.Xml.Serialization;

namespace HsArtExtractor.Hearthstone.CardArt
{
    [XmlRoot("CardArtDefs")]
    public class CardArtDefs
    {
        public string Patch { get; set; }

        [XmlElement("Card")]
        public List<ArtCard> Cards { get; set; }
    }
}