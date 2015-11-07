using HearthstoneDisunity.Util;
using System.Collections.Generic;
using System.Linq;

namespace HearthstoneDisunity.Unity.Objects
{
    public class GameObject
    {
        private List<FilePointerWithClass> _components;

        public long Layer { get; private set; }
        public string Name { get; private set; }
        public int Tag { get; private set; }
        public bool IsActive { get; private set; }

        public GameObject(BinaryFileReader b)
        {
            _components = new List<FilePointerWithClass>();
            var size = b.ReadInt();
            for (int i = 0; i < size; i++)
            {
                _components.Add(new FilePointerWithClass(b.ReadInt(), b.ReadInt(), b.ReadLong()));
            }
            Layer = b.ReadUnsignedInt();
            var nsize = b.ReadInt();
            Name = b.ReadFixedString(nsize);
            b.Align(4);
            Tag = b.ReadUnsignedShort();
            IsActive = b.ReadBoolean();
        }

        public FilePointerWithClass[] Components
        {
            get
            {
                return _components.ToArray<FilePointerWithClass>();
            }
        }

        public override string ToString()
        {
            return string.Format("{0} : {1}", Name, _components.Count);
        }
    }
}
