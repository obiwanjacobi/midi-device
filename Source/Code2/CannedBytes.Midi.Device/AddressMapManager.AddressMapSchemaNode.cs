namespace CannedBytes.Midi.Device
{
    partial class AddressMapManager
    {
        /// <summary>
        /// This schema node type is used to be able to build a different 
        /// schema node order for repeating nodes inside an address map.
        /// </summary>
        /// <remarks>
        /// This class may get replaced by the LogicalSchemaNode class.
        /// </remarks>
        private sealed class AddressMapSchemaNode : SchemaNode
        {
            public AddressMapSchemaNode(SchemaNode thisNode)
            {
                OriginalNode = thisNode;

                CopyFrom(thisNode);
            }

            public SchemaNode OriginalNode { get; private set; }

            public void SetParent(SchemaNode parent)
            {
                this.Parent = parent;
                this.Parent.Children.Add(this);
            }

            public void SetNext(SchemaNode next)
            {
                this.Next = next;
                this.Next.Previous = this;
            }

            private void CopyFrom(SchemaNode copyNode)
            {
                this.Address = copyNode.Address;
                this.DataLength = copyNode.DataLength;
                this.FieldConverterPair = copyNode.FieldConverterPair;
                this.InstanceCount = copyNode.InstanceCount;
                this.InstanceIndex = copyNode.InstanceIndex;
                this.IsAddressMap = copyNode.IsAddressMap;
                this.IsClone = copyNode.IsClone;
                this.IsRecord = copyNode.IsRecord;
                this.IsRoot = copyNode.IsRoot;
                this.Key = copyNode.Key;
                this.Next = copyNode.Next;
                this.NextClone = copyNode.NextClone;
                this.NextSibling = copyNode.NextSibling;
                this.Parent = copyNode.Parent;
                this.Previous = copyNode.Previous;
                this.PreviousClone = copyNode.PreviousClone;
                this.PreviousSibling = copyNode.PreviousSibling;
            }
        }
    }
}
