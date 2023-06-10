using System;
using System.Collections.Generic;
using System.Linq;
using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device;

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
        SevenBitUInt32 endAddress = address + size;
        SchemaNode startNode = _navigator.FindFirst(address);
        SchemaNode endNode = _navigator.FindLast(endAddress);

        if (startNode == null)
        {
            throw new DeviceDataException(
                $"The address '{address}' was not found in the Address Map.");
        }
        if (!startNode.IsAddressMap)
        {
            throw new DeviceDataException(
                $"The node '{startNode.Field.Name}' found on address '{address}' is not inside the Address Map.");
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

        IEnumerable<SchemaNode> nodes = CreateSchemaNodes(startNode, endNode);

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
        if (endNode?.IsAddressMap == false)
        {
            throw new ArgumentException(
                "The specified endNode is not part of an Address Map.", "endNode");
        }

        IEnumerable<AddressMapSchemaNode> parents = CreateParentNodes(startNode);
        AddressMapSchemaNode lastParent = parents.FirstOrDefault();

        IEnumerable<SchemaNode> nodes = CreateSchemaNodes(lastParent, startNode, endNode);

        return nodes;
    }

    private IEnumerable<SchemaNode> CreateSchemaNodes(AddressMapSchemaNode parent, SchemaNode startNode, SchemaNode endNode)
    {
        IEnumerable<SchemaNode> nodes = _navigator.SelectRange(startNode, endNode);

        List<SchemaNode> newNodes = new();

        AddressMapSchemaNode lastNode = null;

        foreach (SchemaNode node in nodes)
        {
            AddressMapSchemaNode newNode = new(node);

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
        IEnumerable<SchemaNode> parents = from n in startNode.SelectNodes(node => node.Parent)
                      where n.IsAddressMap
                      select n;

        List<AddressMapSchemaNode> newParents = new();

        AddressMapSchemaNode lastParent = null;

        foreach (SchemaNode parent in parents)
        {
            AddressMapSchemaNode newParent = new(parent);
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
