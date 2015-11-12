using HearthstoneDisunity.Unity;

namespace HearthstoneDisunity.Util
{
	public class ObjectData
	{
		private long _id;

		public ObjectData(long id)
		{
			_id = id;
		}

		public long Id 
		{
			get { return _id; }
		}

		public ObjectInfo Info { get; set; }

		public AssetTypeNode TypeTree { get; set; }

		public byte[] Buffer { get; set; }

		public void SaveRaw(string filename)
		{
			System.IO.File.WriteAllBytes(filename, Buffer);
		}

		public override string ToString()
		{
			return _id + Info.ToString();
		}
	}
}
