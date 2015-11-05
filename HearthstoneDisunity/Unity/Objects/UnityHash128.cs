using System;
using System.IO;

namespace HearthstoneDisunity.Unity.Objects
{
    public class UnityHash128
    {
        private int _version;

        private readonly byte[] _hash = new byte[16];

        public UnityHash128(int version)
        {
            _version = version;
        }

        public byte[] Hash
        {
            get { return _hash; }
        }

        public void Read(BinaryReader b)
        {
            b.Read(_hash, 0, _hash.Length);
        }

        public void Write(BinaryWriter b)
        {
            b.Write(_hash);
        }

        public override String ToString()
        {
            return BitConverter.ToString(_hash).Replace("-", "");
        }
    }
}
