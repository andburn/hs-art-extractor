using HearthstoneDisunity.Unity.Objects;

namespace HearthstoneDisunity.Hearthstone
{
    public class CardMaterial
    {
        public string Name { get; set; }
        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
        public float Scale { get; set; }
        public float Transition { get; set; }
        public float ValueRange { get; set; }

        public CardMaterial()
        {
        }

        public CardMaterial(Material mat)
        {
            Name = mat.Name;
            foreach (var p in mat.Floats)
            {
                switch (p.Key.ToLowerInvariant())
                {
                    case "_offsetx":
                        OffsetX = p.Value;
                        break;
                    case "_offsety":
                        OffsetY = p.Value;
                        break;
                    case "_scale":
                        Scale = p.Value;
                        break;
                    case "_transition":
                        Transition = p.Value;
                        break;
                    case "_valuerange":
                        ValueRange = p.Value;
                        break;
                    default:
                        break;
                }
            }
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}) {2}", OffsetX, OffsetY, Scale);
        }
    }
}
