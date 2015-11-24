using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HearthstoneDisunity.Util
{
	public class FileIdentifierTable
	{
		private List<FileIdentifier> _fileIds = new List<FileIdentifier>();

		public List<FileIdentifier> FileIds
		{
			get { return _fileIds; }
		}

		private int _version;

		public FileIdentifierTable(int version)
		{
			_version = version;
		}

		public void Read(BinaryBlock b)
		{
			int entries = b.ReadInt();
			for(int i = 0; i < entries; i++)
			{
				FileIdentifier fi = new FileIdentifier(_version);
				fi.Read(b);
				_fileIds.Add(fi);
			}
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			_fileIds.ForEach(fi => sb.AppendLine(fi.ToString()));
			return sb.ToString();
		}

	}

	public class FileIdentifier
	{
		public string AssetPath { get; set; }
		public int RefType { get; set; }
		public string FilePath { get; set; }
		public Guid Guid { get; set; }

		private int _version;

		public FileIdentifier(int version)
		{
			_version = version;
		}

		public void Read(BinaryBlock b)
		{
			if(_version > 5)
			{
				AssetPath = b.ReadStringToNull();
			}			
			ReadGuid(b);
			RefType = b.ReadInt();
			FilePath = b.ReadStringToNull();
		}

		private void ReadGuid(BinaryBlock b)
		{
			bool order = b.BigEndian;
			b.BigEndian = true;
			// TODO: should be BE or not
			Guid = new Guid(b.ReadInt(), b.ReadShort(), b.ReadShort(), b.ReadBytes(8));
			b.BigEndian = order;
		}

		public override string ToString()
		{
			return String.Format("{0} {1} {2}", AssetPath, FilePath, Guid);
		}
	}
}
