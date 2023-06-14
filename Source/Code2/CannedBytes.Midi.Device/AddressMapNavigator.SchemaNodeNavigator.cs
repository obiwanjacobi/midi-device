using System.Collections.Generic;
using System.Linq;
using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device;

partial class AddressMapManager
{
    private sealed class SchemaNodeNavigator
    {
        private readonly SchemaNode _rootNode;

        public SchemaNodeNavigator(SchemaNode rootNode)
        {
            _rootNode = rootNode;
        }

        public SchemaNode FindFirst(SevenBitUInt32 address)
        {
            var firstNode = (from n in _rootNode.SelectNodes(node => node.Next)
                             where n.IsAddressMap
                             where n.Address == address
                             select n).FirstOrDefault();

            return firstNode;
        }

        public SchemaNode FindLast(SevenBitUInt32 address)
        {
            var lastNode = (from n in _rootNode.SelectNodes(node => node.Next)
                            where n.Address <= address
                            where n.IsAddressMap
                            select n).LastOrDefault();

            if (lastNode != null)
            {
                lastNode = lastNode.LastFieldOfAddress;
            }

            return lastNode;
        }

        public SchemaNode PreviousAddress(SchemaNode currentNode, SevenBitUInt32 address)
        {
            List<SchemaNode> nodes = new()
            {
                currentNode
            };

            nodes.AddRange(currentNode.SelectNodes(node => node.Previous));

            var prevNode = (from n in nodes
                            where n.IsAddressMap
                            where n.Address < address
                            select n).FirstOrDefault();

            prevNode ??= currentNode;

            return prevNode;
        }

        public IEnumerable<SchemaNode> SelectRange(SchemaNode startNode, SchemaNode endNode)
        {
            List<SchemaNode> range = new()
            {
                startNode
            };

            var nodes = from n in startNode.SelectNodes(node => node.Next)
                        where n.IsAddressMap
                        where endNode == null || n.Address <= endNode.Address
                        select n;

            range.AddRange(nodes);

            return range;
        }
    }
}
