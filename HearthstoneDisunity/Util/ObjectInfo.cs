using System;
using System.Diagnostics;
using HearthstoneDisunity.Unity;

namespace HearthstoneDisunity.Util
{
	public class ObjectInfo
	{
		private int _version;

		public long Offset { get; set; }
		public long Length { get; set; }
		public int TypeId { get; set; }
		public short ClassId { get; set; }
		public short IsDestroyed { get; set; }

		public ObjectInfo(int version)
		{
			_version = version;
		}

		public void Read(BinaryBlock b)
		{
			Offset = b.ReadUnsignedInt();
			Length = b.ReadUnsignedInt();
			TypeId = b.ReadInt();
			ClassId = b.ReadShort();
			IsDestroyed = b.ReadShort();
			// TODO: the only way I can get this to work, align doesn't!
			if(_version > 13)
				b.ReadInt();

			// sanity check
			Debug.Assert(TypeId == ClassId || (ClassId == 114 && TypeId < 0));
		}

		public override string ToString()
		{
			return String.Format("[cid:{0} tid:{1}] {2}, {3}", ClassId, TypeId, Offset, Length);
		}
	}
}
