using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace HearthstoneDisunity.Hearthstone.Xml
{
    public static class CardDb
    {
        public static Dictionary<string, Card> All = new Dictionary<string, Card>();

        public static void Read(string file)
        {
            using(TextReader tr = new StreamReader(file))
            {
                var xml = new XmlSerializer(typeof(CardDefs));
                var cardDefs = (CardDefs)xml.Deserialize(tr);
                foreach(var entity in cardDefs.Entites)
                {
                    var card = new Card(entity);
                    All.Add(entity.CardId, card);
                }
            }
        }
    }
}
