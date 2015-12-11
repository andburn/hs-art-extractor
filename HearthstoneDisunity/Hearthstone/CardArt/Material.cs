using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace HearthstoneDisunity.Hearthstone.CardArt
{
    public class Material
    {
        [XmlAttribute("type")]
        public MaterialType Type { get; set; }

        [XmlElement("Transform")]
        public List<Transform> Transforms { get; set; }

        public Transform GetTransform(TransformType type)
        {
            return Transforms.FirstOrDefault(x => x.Type == type);
        }
    }
}