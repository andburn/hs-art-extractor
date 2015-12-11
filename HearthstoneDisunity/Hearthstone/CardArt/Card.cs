using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace HearthstoneDisunity.Hearthstone.CardArt
{
    public class Card
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlElement("Texture")]
        public Texture Texture { get; set; }

        [XmlElement("Material")]
        public List<Material> Materials { get; set; }

        public Material GetMaterial(MaterialType type)
        {
            return Materials.FirstOrDefault(x => x.Type == type);
        }
    }
}