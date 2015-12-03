using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HearthstoneDisunity.Util
{
    public class BinaryBlock : BinaryReader
    {
        private bool _bigEndian;

        public BinaryBlock(Stream input)
            : base(input)
        {
            _bigEndian = false;
        }

        public static BinaryBlock CreateFromByteArray(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream(buffer);
            return new BinaryBlock(ms);
        }

        public bool BigEndian
        {
            get
            {
                return _bigEndian;
            }
            set
            {
                Logger.Log(LogLevel.DEBUG, "Setting BigEndian to {0}", _bigEndian);
                _bigEndian = value;
            }
        }

        public void Seek(long offset)
        {
            try
            {
                BaseStream.Seek(offset, SeekOrigin.Begin);
            }
            catch (Exception)
            {
                // TODO: offset beyond eof check
            }
        }

        public short ReadShort()
        {
            if (_bigEndian)
            {
                return ReadInt16BigEndian();
            }
            else
            {
                return ReadInt16();
            }
        }

        public int ReadInt()
        {
            if (_bigEndian)
            {
                return ReadInt32BigEndian();
            }
            else
            {
                return ReadInt32();
            }
        }

        public long ReadLong()
        {
            if (_bigEndian)
            {
                return ReadInt64BigEndian();
            }
            else
            {
                return ReadInt64();
            }
        }

        public ulong ReadUnsignedLong()
        {
            if (_bigEndian)
            {
                return ReadUInt64BigEndian();
            }
            else
            {
                return ReadUInt64();
            }
        }

        public int ReadUnsignedShort()
        {
            if (_bigEndian)
            {
                return ReadUInt16BigEndian();
            }
            else
            {
                return ReadUInt16();
            }
        }

        public long ReadUnsignedInt()
        {
            if (_bigEndian)
            {
                return ReadUInt32BigEndian();
            }
            else
            {
                return ReadUInt32();
            }
        }

        public int ReadUnsignedByte()
        {
            return ReadByte() & 0xff;
        }

        public float ReadFloat()
        {
            return ReadSingle();
        }

        public string ReadStringToNull(int limit = 256)
        {
            List<byte> buffer = new List<byte>(limit);

            byte b;
            while ((b = ReadByte()) != 0)
            {
                if (buffer.Count < limit)
                {
                    buffer.Add(b);
                }
            }

            if (buffer.Count == limit)
            {
                return "";
            }
            else
            {
                return Encoding.ASCII.GetString(buffer.ToArray<byte>());
            }
        }

        // TODO: int enough?
        public BinaryBlock CopyBlock(int position, int length)
        {
            byte[] buffer = new byte[length];
            var pos = BaseStream.Position;
            BaseStream.Seek(position, SeekOrigin.Begin);
            Read(buffer, 0, length);
            MemoryStream ms = new MemoryStream(buffer);
            BaseStream.Seek(pos, SeekOrigin.Begin);
            return new BinaryBlock(ms);
        }

        public string ReadFixedString(int length)
        {
            if (length <= 0)
                return "";
            return Encoding.ASCII.GetString(ReadBytes(length));
        }

        public void Align(int align)
        {
            if (align > 0)
            {
                int rem = (int)(this.BaseStream.Position % (long)align);
                if (rem != 0)
                {
                    ReadBytes(align - rem);
                }
            }
        }

        private short ReadInt16BigEndian()
        {
            byte[] buf = ReadBytes(2);
            // TODO: loss of accuracy
            return (short)((buf[0] << 8) | buf[1]);
        }

        private int ReadUInt16BigEndian()
        {
            return ReadUInt16BigEndian() & 0xffff;
        }

        private int ReadInt32BigEndian()
        {
            byte[] buf = ReadBytes(4);
            return (buf[0] << 24) | (buf[1] << 16) | (buf[2] << 8) | buf[3];
        }

        private long ReadUInt32BigEndian()
        {
            return ReadInt32BigEndian() & 0xffffffffL;
        }

        private long ReadInt64BigEndian()
        {
            byte[] buf = ReadBytes(8);
            return (buf[0] << 56) | (buf[1] << 48) | (buf[2] << 40) | (buf[3] << 32) | (buf[4] << 24) | (buf[5] << 16) | (buf[6] << 8) | buf[7];
        }

        private ulong ReadUInt64BigEndian()
        {
            // TODO: is this right?
            return (ulong)ReadUInt32BigEndian() & 0xffffffffffffffffL;
        }
    }
}