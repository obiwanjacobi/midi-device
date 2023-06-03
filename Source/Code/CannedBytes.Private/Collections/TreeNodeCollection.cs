namespace CannedBytes.Collections
{
    using System.Collections.ObjectModel;

    public class TreeNodeCollection<T> : KeyedCollection<string, TreeNode<T>>
    {
        public TreeNodeCollection(TreeNode<T> parentNode)
        {
            ParentNode = parentNode;
        }

        public TreeNode<T> ParentNode { get; protected set; }

        protected override string GetKeyForItem(TreeNode<T> item)
        {
            return item.Key;
        }

        protected override void InsertItem(int index, TreeNode<T> item)
        {
            item.ParentNode = ParentNode;

            base.InsertItem(index, item);
        }
    }
}