using System.Text;
using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device;

public sealed class SchemaNodeMap
{
    public SchemaNodeMap(SchemaNode root)
    {
        Assert.IfArgumentNull(root, nameof(root));

        RootNode = root;
        LastNode = root;
    }

    public SchemaNode RootNode { get; }

    public SchemaNode LastNode { get; internal set; }

    public SchemaNode AddressMap { get; internal set; }

    public override string ToString()
    {
        StringBuilder text = new();

        text.AppendLine(RootNode.ToString());

        foreach (SchemaNode node in RootNode.SelectNodes(node => node.Next))
        {
            text.AppendLine(node.ToString());
        }

        return text.ToString();
    }
}
