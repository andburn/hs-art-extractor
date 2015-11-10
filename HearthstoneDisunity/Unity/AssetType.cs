using HearthstoneDisunity.Util;

namespace HearthstoneDisunity.Unity
{
	public class AssetType
	{
		public const int FLAG_FORCE_ALIGN = 0x4000;

		public int Size { get; set; }
		public int Index { get; set; }
		public bool IsArray { get; set; }
		public int Version { get; set; }
		public int MetaFlag { get; set; }
		public int TreeLevel { get; set; }
		public int TypeOffset { get; set; }
		public int NameOffset { get; set; }

		private int _assetVersion;
		private string _dataType;
		private string _name;

		public AssetType(int version)
		{
			_assetVersion = version;
		}

		public string FieldName
		{
			get { return _name; }
			set { _name = value; }
		}

		public string TypeName
		{
			get { return _dataType; }
			set { _dataType = value; }
		}

		public void Read(BinaryFileReader b)
		{
			if(_assetVersion > 13)
			{
				Version = b.ReadShort();
				TreeLevel = b.ReadUnsignedByte();				
				IsArray = b.ReadBoolean();
				TypeOffset = b.ReadInt();
				NameOffset = b.ReadInt();
				Size = b.ReadInt();
				Index = b.ReadInt();
				MetaFlag = b.ReadInt();
			}
			else
			{
				_dataType = b.ReadStringToNull(256);
				_name = b.ReadStringToNull(256);
				Size = b.ReadInt();
				Index = b.ReadInt();
				IsArray = b.ReadBoolean();
				Version = b.ReadInt();
				MetaFlag = b.ReadInt();
			}			
		}

		public void SetForceAlign(bool forceAlign)
		{
			if(forceAlign)
			{
				MetaFlag |= FLAG_FORCE_ALIGN;
			}
			else
			{
				MetaFlag &= ~FLAG_FORCE_ALIGN;
			}
		}

		public bool IsForceAlign()
		{
			return (MetaFlag & FLAG_FORCE_ALIGN) != 0;
		}

		public override string ToString()
		{
			return string.Format("{0}:{1}", _dataType, _name);
		}
	}
}
