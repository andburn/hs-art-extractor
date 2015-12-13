using System;
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
        private float _x;
        private float _y;

        [XmlAttribute("x")]
        public float X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = (float)Math.Round(value, 4);
            }
        }

        [XmlAttribute("y")]
        public float Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = (float)Math.Round(value, 4);
            }
        }

        public CoordinateTransform()
        {
            X = 0.0f;
            Y = 0.0f;
        }

        public CoordinateTransform(float x, float y)
        {
            X = x;
            Y = y;
        }

        public CoordinateTransform(float u)
        {
            X = u;
            Y = u;
        }
    }
}