using System.Xml.Serialization;

namespace HearthstoneDisunity.Hearthstone.Database
{
    public class Tag
    {
        [XmlAttribute("enumID")]
        public int EnumId { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("type")]
        public string TypeString { get; set; }

        [XmlAttribute("value")]
        public int Value { get; set; }

        [XmlText]
        public string InnerValue { get; set; }
    }
}