using CannedBytes.Midi.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CannedBytes.Midi.Device
{
    public partial class AddressMapManager
    {
        private SchemaNode _rootNode;
        private SchemaNodeNavigator _navigator;

        public AddressMapManager(SchemaNode rootNode)
        {
            Check.IfArgumentNull(rootNode, "rootNode");

            _rootNode = rootNode;
            _navigator = new SchemaNodeNavigator(_rootNode);
        }

        public IEnumerable<SchemaNode> CreateSchemaNodes(SevenBitUInt32 address, SevenBitUInt32 size)
        {
            var endAddress = address + size;
            var startNode = _navigator.FindFirst(address);
            var endNode = _navigator.FindLast(endAddress);

            if (startNode == null)
            {
                throw new DeviceDataException(
                    String.Format("The address '{0}' was not found in the Address Map.",
                        address));
            }
            if (!startNode.IsAddressMap)
            {
                throw new DeviceDataException(
                    String.Format("The node '{0}' found on address '{1}' is not inside the Address Map.",
                        startNode.Field.Name, address));
            }

            if (size == 0)
            {
                endNode = null;
            }

            if (endNode != null)
            {
                // we find the address after the last field we need.
                endNode = _navigator.PreviousAddress(endNode, endAddress);
            }

            var nodes = CreateSchemaNodes(startNode, endNode);

            return nodes;
        }

        public IEnumerable<SchemaNode> CreateSchemaNodes(SchemaNode startNode, SchemaNode endNode)
        {
            Check.IfArgumentNull(startNode, "startNode");
            if (!startNode.IsAddressMap)
            {
                throw new ArgumentException(
                    "The specified startNode is not part of an Address Map.", "startNode");
            }
            if (endNode != null && !endNode.IsAddressMap)
            {
                throw new ArgumentException(
                    "The specified endNode is not part of an Address Map.", "endNode");
            }

            var parents = CreateParentNodes(startNode);
            var lastParent = parents.FirstOrDefault();

            var nodes = CreateSchemaNodes(lastParent, startNode, endNode);

            return nodes;
        }

        private IEnumerable<SchemaNode> CreateSchemaNodes(AddressMapSchemaNode parent, SchemaNode startNode, SchemaNode endNode)
        {
            var nodes = _navigator.SelectRange(startNode, endNode);

            var newNodes = new List<SchemaNode>();

            AddressMapSchemaNode lastNode = null;

            foreach (var node in nodes)
            {
                var newNode = new AddressMapSchemaNode(node);

                newNodes.Add(newNode);

                if (lastNode != null)
                {
                    lastNode.SetNext(newNode);
                }
                else if (parent != null)
                {
                    newNode.SetParent(parent);
                    parent.SetNext(newNode);
                }

                lastNode = newNode;
            }

            return newNodes;
        }

        private IEnumerable<AddressMapSchemaNode> CreateParentNodes(SchemaNode startNode)
        {
            var parents = from n in startNode.SelectNodes(node => node.Parent)
                          where n.IsAddressMap
                          select n;

            var newParents = new List<AddressMapSchemaNode>();

            AddressMapSchemaNode lastParent = null;

            foreach (var parent in parents)
            {
                var newParent = new AddressMapSchemaNode(parent);
                newParents.Add(newParent);

                if (lastParent != null)
                {
                    newParent.SetParent(lastParent);
                    lastParent.SetNext(newParent);
                }

                lastParent = newParent;
            }

            return newParents;
        }
    }
}
