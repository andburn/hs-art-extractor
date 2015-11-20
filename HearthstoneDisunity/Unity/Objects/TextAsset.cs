using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HearthstoneDisunity.Util;

namespace HearthstoneDisunity.Unity.Objects
{
    public class TextAsset
    {
        public string Name { get; set; }
        private byte[] _text;

        public TextAsset(BinaryFileReader b)
        {
            var nameLen = b.ReadInt();
            Name = b.ReadFixedString(nameLen);
            var textLen = b.ReadInt();
            _text = b.ReadBytes(textLen); // TODO: overflow possible?
        }

        public void Save(string dir)
        {
            if (_text != null)
                File.WriteAllBytes(Path.Combine(dir, Name + ".txt"), _text);
        }
    }
}
