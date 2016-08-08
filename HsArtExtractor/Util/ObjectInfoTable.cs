using System.Collections.Generic;

namespace HsArtExtractor.Util
{
	public class ObjectInfoTable
	{
		private int _version;
		private Dictionary<long, ObjectInfo> _infoMap;

		public ObjectInfoTable(int version)
		{
			_version = version;
		}

		public void Read(BinaryBlock b)
		{
			_infoMap = new Dictionary<long, ObjectInfo>();

			int entries = b.ReadInt();

			if (_version > 13)
				b.Align(4);

			for (int i = 0; i < entries; i++)
			{
				long pathID;
				if (_version > 13)
				{
					pathID = b.ReadLong();
				}
				else
				{
					pathID = b.ReadInt();
				}

				ObjectInfo info = new ObjectInfo(_version);
				info.Read(b);
				_infoMap.Add(pathID, info);
			}
		}

		public Dictionary<long, ObjectInfo> InfoMap
		{
			get { return _infoMap; }
		}

		public int Size
		{
			get { return _infoMap.Count; }
		}
	}
}