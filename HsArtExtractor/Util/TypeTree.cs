using HsArtExtractor.Unity;
using HsArtExtractor.Unity.Objects;
using System.Collections.Generic;

namespace HsArtExtractor.Util
{
    public class TypeTree
	{
		private int _version;
		private string _revision;
		private Dictionary<int, BaseClass> _typeMap;
		
		public int Attributes { get; set; }
		public bool Embedded { get; set; }

		public TypeTree(int version)
		{
			_version = version;
			_typeMap = new Dictionary<int, BaseClass>();
		}

		public Dictionary<int, BaseClass> TypeMap
		{
			get
			{
				if (_typeMap == null)
					return new Dictionary<int, BaseClass>();
				return _typeMap;
			}
		}

		public void Read(BinaryBlock b)
		{
			if(_version > 6)
			{
				_revision = b.ReadStringToNull();
				Attributes = b.ReadInt();
			}

			if (_version > 13)
			{
				Embedded = b.ReadBoolean();
				int numBaseClasses = b.ReadInt();

				for (int i = 0; i < numBaseClasses; i++) {
					int classID = b.ReadInt();
					BaseClass baseClass = new BaseClass(classID);
                
					if (classID < 0)
					{
						UnityHash128 scriptID = new UnityHash128(_version);
						scriptID.Read(b);
						baseClass.ScriptID = scriptID;
					}

					UnityHash128 oldTypeHash = new UnityHash128(_version);
					oldTypeHash.Read(b);
					baseClass.OldTypeHash = oldTypeHash;

					if (Embedded) {
						AssetTypeNode node = new AssetTypeNode();
						AssetTypeReader nodeReader = new AssetTypeReader(_version);
						nodeReader.Read(b, node);
						baseClass.TypeTree = node;
					}
                
					_typeMap.Add(classID, baseClass);
				}
			}
			else
			{				
				int numBaseClasses = b.ReadInt();

				for(int i = 0; i < numBaseClasses; i++)
				{
					int classID = b.ReadInt();
					BaseClass baseClass = new BaseClass(classID);

					AssetTypeNode node = new AssetTypeNode();
					AssetTypeReader nodeReader = new AssetTypeReader(_version);
					nodeReader.Read(b, node);
					baseClass.TypeTree = node;

					_typeMap.Add(classID, baseClass);
				}
				Embedded = numBaseClasses > 0;
				
				if(_version > 6)
					b.ReadInt(); // padding
			}					
		}
	}
}
