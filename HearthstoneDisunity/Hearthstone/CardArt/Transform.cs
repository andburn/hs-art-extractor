using System.Xml.Serialization;

namespace HearthstoneDisunity.Hearthstone.CardArt
{
    public class Transform
    {
        [XmlAttribute("type")]
        public TransformType Type { get; set; }

        [XmlElement("Scale")]
        public CoordinateTransform Scale { get; set; }

        [XmlElement("Offset")]
        public CoordinateTransform Offset { get; set; }
    }

    public class CoordinateTransform
    {
        [XmlAttribute("x")]
        public double X { get; set; }

        [XmlAttribute("y")]
        public double Y { get; set; }

        public CoordinateTransform()
        {
            X = 0.0;
            Y = 0.0;
        }

        public CoordinateTransform(double x, double y)
        {
            X = x;
            Y = y;
        }

        public CoordinateTransform(double u)
        {
            X = u;
            Y = u;
        }
    }
}