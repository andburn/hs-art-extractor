using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HearthstoneDisunity.Util;

namespace HearthstoneDisunity.Unity
{
	public class FieldNode : TreeNode<FieldNode>
	{
		public AssetType Type { get; set; }
		public Object Value { get; set; }

		public FieldNode GetChild(string name)
		{
			FieldNode node = null;
			Traverse(x => FindChildByName(name, x, node));
			return node;
		}

		public object GetChildValue(string name)
		{
			var child = GetChild(name);
			if(child == null)
				return null;

			return child.Value;
		}

		public void SetChildValue(string name, object value)
		{
			var child = GetChild(name);
			if(child == null)
			{
				throw new Exception("Field " + name + " doesn't exist");
			}
			child.Value = value;
		}

		public int GetSInt32(string name)
		{
			var value = GetChildValue(name);
			if(value == null)
				throw new Exception("Null found instead of int - " + name);
			if(!value.GetType().IsAssignableFrom(typeof(int)))
			{
				throw new Exception("Type mismatch: " + value.GetType() + " should be " + typeof(int));
			}
			return (int)value;
		}

		public byte[] GetByteArrayData() {
			FieldNode arrayField = GetChild("Array");
			if (arrayField == null) {
				throw new Exception("Field is not an array");
			}
        
			return (byte[])arrayField.GetChildValue("data");
		}

		private void FindChildByName(string name, TreeNode<FieldNode> x, FieldNode node)
		{
			var fn = (FieldNode)x;
			if(name == fn.Type.FieldName)
			{
				node = fn;
			}
		}


	}
}
