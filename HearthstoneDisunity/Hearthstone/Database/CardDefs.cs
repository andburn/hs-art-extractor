﻿using System.Collections.Generic;
using System.Xml.Serialization;

namespace HearthstoneDisunity.Hearthstone.Database
{
    [XmlRoot("CardDefs")]
    public class CardDefs
    {
        [XmlElement("Entity")]
        public List<Entity> Entites { get; set; }
    }
}