using HsArtExtractor.Util;

namespace HsArtExtractor.Unity
{
	public class AssetTypeNode : TreeNode<AssetTypeNode>
	{
		public AssetType Type { get; set; }

		//public AssetTypeNode()
		//	: base(null)
		//{
		//}

		//public AssetTypeNode(AssetTypeNode n)
		//	: base(n)
		//{
		//}

		public override string ToString()
		{
			if (Parent != null)
				return Type.ToString() + " <- (" + Parent.Type.ToString() + ")";
			else
				return Type.ToString() + " <- ()";
		}
	}
}
