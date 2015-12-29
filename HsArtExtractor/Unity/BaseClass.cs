using HsArtExtractor.Unity.Objects;

namespace HsArtExtractor.Unity
{
    public class BaseClass
	{
		public int ClassID { get; private set; }
		public AssetTypeNode TypeTree { get; set; }
		public UnityHash128 ScriptID { get; set; }
		public UnityHash128 OldTypeHash { get; set; }

		public BaseClass(int id)
		{
			ClassID = id;
		}
	}
}
