using System.Text;

namespace CannedBytes.Midi.Device
{
    public sealed class SchemaNodeMap
    {
        public SchemaNodeMap(SchemaNode root)
        {
            Check.IfArgumentNull(root, "root");

            RootNode = root;
            LastNode = root;
        }

        public SchemaNode RootNode { get; private set; }

        public SchemaNode LastNode { get; internal set; }

        public SchemaNode AddressMap { get; internal set; }

        public override string ToString()
        {
            StringBuilder text = new StringBuilder();

            text.AppendLine(RootNode.ToString());

            foreach (var node in RootNode.SelectNodes(node => node.Next))
            {
                text.AppendLine(node.ToString());
            }

            return text.ToString();
        }
    }
}
