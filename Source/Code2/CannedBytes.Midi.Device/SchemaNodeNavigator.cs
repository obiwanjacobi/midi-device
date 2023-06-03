using CannedBytes.Midi.Core;
using System.Collections.Generic;
using System.Linq;

namespace CannedBytes.Midi.Device
{
    public class SchemaNodeNavigator
    {
        private SchemaNode _rootNode;

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
            var nodes = new List<SchemaNode>();
            nodes.Add(currentNode);
            nodes.AddRange(currentNode.SelectNodes(node => node.Previous));

            var prevNode = (from n in nodes
                            where n.IsAddressMap
                            where n.Address < address
                            select n).FirstOrDefault();

            if (prevNode == null)
            {
                prevNode = currentNode;
            }

            return prevNode;
        }

        public IEnumerable<SchemaNode> SelectRange(SchemaNode startNode, SchemaNode endNode)
        {
            var range = new List<SchemaNode>();

            range.Add(startNode);

            var nodes = from n in startNode.SelectNodes(node => node.Next)
                        where n.IsAddressMap
                        where endNode == null || n.Address <= endNode.Address
                        select n;

            range.AddRange(nodes);

            return range;
        }
    }
}
