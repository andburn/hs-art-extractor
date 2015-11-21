using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace HearthstoneDisunity.Hearthstone.Xml
{
    public class Entity
    {
        [XmlAttribute("CardID")]
        public string CardId { get; set; }

        [XmlElement("Tag")]
        public List<Tag> Tags { get; set; } = new List<Tag>();

        public int GetTag(int tagId)
        {
            var tag = Tags.FirstOrDefault(x => x.EnumId == tagId);
            return tag?.Value ?? 0;
        }

        public string GetInnerValue(int tagId)
        {
            var tag = Tags.FirstOrDefault(x => x.EnumId == tagId);
            return tag?.InnerValue;
        }
    }
}
