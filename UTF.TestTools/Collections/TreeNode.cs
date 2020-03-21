using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections;

namespace UTF.TestTools.Collections
{
    public class TreeNode<T>
        : IEnumerable<TreeNode<T>>
    {
        #region Fields
        //private int _index;
        private T _value;
        private List<TreeNode<T>> _children;
        #endregion Fields

        #region Ctor
        //public TreeNode(T value, int startingIndex = 1)
        //{
        //    _index = startingIndex;
        //    _value = value;
        //    _children = new List<TreeNode<T>>();

        //    this.ElementsIndex = new LinkedList<TreeNode<T>>();
        //    this.ElementsIndex.Add(this);
        //}
        public TreeNode(T value)
        {
            _value = value;
            _children = new List<TreeNode<T>>();

            this.ElementsIndex = new LinkedList<TreeNode<T>>();
            this.ElementsIndex.Add(this);
        }
        #endregion Ctor

        #region Methods
        public override string ToString()
        {
            return Value != null ? Value.ToString() : "[data null]";
        }

        public TreeNode<T> AddChild(T child)
        {
            //var childNode = new TreeNode<T>(child, _children.Count + 1) { Parent = this };
            var childNode = new TreeNode<T>(child) { Parent = this };

            _children.Add(childNode);

            this.RegisterChildForSearch(childNode);

            return childNode;
        }

        public TreeNode<T>[] AddChildren(params T[] values)
        {
            return values.Select(AddChild).ToArray();
        }

        public bool RemoveChild(TreeNode<T> node)
        {
            return _children.Remove(node);
        }

        public void Traverse(Action<T> action)
        {
            action(Value);
            foreach (var child in _children)
                child.Traverse(action);
        }

        public IEnumerable<T> Flatten()
        {
            return new[] { Value }.Union(_children.SelectMany(x => x.Flatten()));
        }

        #region Searching
        private void RegisterChildForSearch(TreeNode<T> node)
        {
            ElementsIndex.Add(node);
            if (Parent != null)
                Parent.RegisterChildForSearch(node);
        }

        public TreeNode<T> FindTreeNode(Func<TreeNode<T>, bool> predicate)
        {
            return this.ElementsIndex.FirstOrDefault(predicate);
        }
        #endregion Searching

        #region Iterating
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<TreeNode<T>> GetEnumerator()
        {
            yield return this;
            foreach (var directChild in this.Children)
            {
                foreach (var anyChild in directChild)
                    yield return anyChild;
            }
        }

        //public void Bubble(Action<T> action)
        //{
        //    action(Value);
        //    if(this.Parent != null)
        //    {
        //        this.Parent.Bubble(action);
        //    }
        //}
        #endregion Iterating
        #endregion Methods

        #region Properties
        private ICollection<TreeNode<T>> ElementsIndex { get; set; }

        public TreeNode<T> Parent { get; protected set; }

        public T Value { get { return _value; } }

        public ICollection<TreeNode<T>> Children
        {
            get { return _children.AsReadOnly(); }
        }

        public TreeNode<T> this[int i]
        {
            get { return _children[i]; }
        }

        public Boolean IsRoot
        {
            get { return Parent == null; }
        }
        public Boolean IsLeaf
        {
            get { return Children.Count == 0; }
        }

        public int Level
        {
            get
            {
                if (this.IsRoot)
                    return 0;
                return Parent.Level + 1;
            }
        }
        #endregion Properties
    }
}
