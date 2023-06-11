namespace CannedBytes.Midi.Device;

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

        public SchemaNode OriginalNode { get; }

        public void SetParent(SchemaNode parent)
        {
            Parent = parent;
            Parent.Children.Add(this);
        }

        public void SetNext(SchemaNode next)
        {
            Next = next;
            Next.Previous = this;
        }

        private void CopyFrom(SchemaNode copyNode)
        {
            Address = copyNode.Address;
            DataLength = copyNode.DataLength;
            FieldConverterPair = copyNode.FieldConverterPair;
            InstanceCount = copyNode.InstanceCount;
            InstanceIndex = copyNode.InstanceIndex;
            IsAddressMap = copyNode.IsAddressMap;
            IsClone = copyNode.IsClone;
            IsRecord = copyNode.IsRecord;
            IsRoot = copyNode.IsRoot;
            Key = copyNode.Key;
            Next = copyNode.Next;
            NextClone = copyNode.NextClone;
            NextSibling = copyNode.NextSibling;
            Parent = copyNode.Parent;
            Previous = copyNode.Previous;
            PreviousClone = copyNode.PreviousClone;
            PreviousSibling = copyNode.PreviousSibling;
        }
    }
}
