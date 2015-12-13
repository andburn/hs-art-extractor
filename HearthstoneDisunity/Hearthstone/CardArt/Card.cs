using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using HearthstoneDisunity.Util;

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

        public Card()
        {
            Id = "";
            Texture = new Texture();
            Materials = new List<Material>();
        }

        public Card(string id,
            Unity.Objects.CardDef def,
            Unity.Objects.Material portrait,
            Unity.Objects.Material bar)
            : this()
        {
            Id = id;
            if (def != null)
            {
                Texture.Path = def.PortratitTexturePath;
                Texture.Name = StringUtils.GetFilenameNoExt(def.PortratitTexturePath);
            }
            if (portrait != null)
                AddMaterial(portrait, MaterialType.Portrait);
            if (bar != null)
                AddMaterial(bar, MaterialType.CardBar);
        }

        public void AddMaterial(Unity.Objects.Material material, MaterialType type)
        {
            var mat = new Material();
            mat.Type = type;
            mat.AddTransform(material.Floats);
            mat.AddTransform(material.TexEnvs);
            Materials.Add(mat);
        }

        public Material GetMaterial(MaterialType type)
        {
            return Materials.FirstOrDefault(x => x.Type == type);
        }
    }
}