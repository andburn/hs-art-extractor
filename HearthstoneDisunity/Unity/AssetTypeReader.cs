using System.Collections.Generic;
using HearthstoneDisunity.Util;

namespace HearthstoneDisunity.Unity
{
	public class AssetTypeReader
	{
		private int _version;

		public AssetTypeReader(int version)
		{
			_version = version;
		}

		public void Read(BinaryFileReader b, AssetTypeNode node)
		{
			if(_version > 13)
			{
				ReadNew(b, node, new StringTable());
			}
			else
			{
				ReadOld(b, node, 0);
			}
		}

		private void ReadNew(BinaryFileReader b, AssetTypeNode node, StringTable table)
		{
			int numFields = b.ReadInt();
			int stringTableLen = b.ReadInt();

			// read types
			List<AssetType> types = new List<AssetType>(numFields);
			for (int j = 0; j < numFields; j++) {
				AssetType type = new AssetType(_version);
				type.Read(b);
				types.Add(type);
			}

			// read string table
			byte[] stringTable = new byte[stringTableLen];
			b.Read(stringTable, 0, stringTableLen);

			// assign strings
			StringTable stExt = new StringTable();
			stExt.LoadStrings(stringTable);
			foreach (AssetType field in types) {
				int nameOffset = field.NameOffset;
				string name = stExt.GetString(nameOffset);
				if (name == null) {
					name = table.GetString(nameOffset);
				}
				field.FieldName = name;
            
				int typeOffset = field.TypeOffset;
				string type = stExt.GetString(typeOffset);
				if (type == null) {
					type = table.GetString(typeOffset);
				}
				field.TypeName = type;
			}
                
			// convert list to tree structure
			AssetTypeNode nodePrev = null;
			foreach(AssetType type in types)
			{
				if(nodePrev == null)
				{
					node.Type = type;
					nodePrev = node;
					continue;
				}

				AssetTypeNode nodeCurr = new AssetTypeNode();
				nodeCurr.Type = type;

				int levels = nodePrev.Type.TreeLevel - type.TreeLevel;
				if(levels >= 0)
				{
					// move down in tree hierarchy if required
					for(int i = 0; i < levels; i++)
					{
						nodePrev = nodePrev.Parent;
					}

					nodePrev.Parent.AddChild(nodeCurr);
				}
				else
				{
					// can move only one level up at a time, so simply add the node
					nodePrev.AddChild(nodeCurr);
				}

				nodePrev = nodeCurr;
			}
		}

		private void ReadOld(BinaryFileReader b, AssetTypeNode node, int level)
		{
			AssetType type = new AssetType(_version);
			type.Read(b);

			node.Type = type;

			int numChildren = b.ReadInt();
			for(int i = 0; i < numChildren; i++)
			{
				AssetTypeNode child = new AssetTypeNode();
				// recursive call
				ReadOld(b, child, level + 1);
				node.AddChild(child);
			}
		}
	}
}
