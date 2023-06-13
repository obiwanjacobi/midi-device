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

            thisNode.CopyTo(this);
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
    }
}
