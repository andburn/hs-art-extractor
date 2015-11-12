using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HearthstoneDisunity.Util
{
	// Tree structure based on http://stackoverflow.com/a/10442244/2762059
	public class TreeNode<T> where T : TreeNode<T>
	{
		private readonly List<T> _children = new List<T>();

		public T Parent { get; private set; }

		public T this[int i]
		{
			get { return _children[i]; }
		}

		public ReadOnlyCollection<T> Children
		{
			get { return _children.AsReadOnly(); }
		}

		public T AddChild(T node)
		{
			node.Parent = (T)this;
			_children.Add(node);
			return node;
		}

		public bool RemoveChild(T node)
		{
			return _children.Remove(node);
		}

		public TreeNode<T>[] AddChildren(params T[] values)
		{
			return values.Select(AddChild).ToArray();
		}

		public void Traverse(Action<TreeNode<T>> action)
		{
			action(this);
			foreach(var child in _children)
				child.Traverse(action);
		}

		public void TraverseChildren(Action<TreeNode<T>> action)
		{
			//action(this);
			foreach(var child in _children)
				child.Traverse(action);
		}

		//public IEnumerable<T> Flatten()
		//{
		//	return new[] { Value }.Union(_children.SelectMany(x => x.Flatten()));
		//}

		public bool IsEmpty()
		{
			return _children.Count <= 0;
		}

		public int Size()
		{
			return _children.Count;
		}
	}
}
