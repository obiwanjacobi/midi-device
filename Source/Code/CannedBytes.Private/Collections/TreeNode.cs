namespace CannedBytes.Collections
{
    public class TreeNode<T>
    {
        public TreeNode(T item)
        {
            Item = item;
            Children = new TreeNodeCollection<T>(this);
        }

        private string _key;

        public string Key
        {
            get
            {
                if (_key == null)
                {
                    Key = GetKeyForItem(Item);
                }

                return _key;
            }
            protected set
            {
                _key = value;
            }
        }

        protected virtual string GetKeyForItem(T Item)
        {
            return Item.ToString();
        }

        public T Item { get; protected set; }

        public TreeNode<T> ParentNode { get; set; }

        public TreeNodeCollection<T> Children { get; protected set; }
    }
}