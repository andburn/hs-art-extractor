using HearthstoneDisunity.Unity.Objects;
using HearthstoneDisunity.Util;

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
        public Vector2F TexOffset { get; set; }
        public Vector2F TexScale { get; set; }

        public CardMaterial()
        {
        }

        public CardMaterial(Material mat)
        {
            Name = mat.Name;
            foreach (var p in mat.Floats)
            {
                // TODO: check, is more than this now?
                switch (p.Key.ToLower())
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

            foreach (var p in mat.TexEnvs)
            {
                if (p.Key.ToLower() == "_maintex")
                {
                    TexScale = p.Value.Scale;
                    TexOffset = p.Value.Offset;
                }
            }
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}) {2} : {3} : {4}", OffsetX, OffsetY, Scale, TexScale, TexOffset);
        }
    }
}