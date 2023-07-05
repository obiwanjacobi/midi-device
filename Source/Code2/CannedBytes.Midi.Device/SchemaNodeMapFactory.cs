using System.Collections.Generic;
using System.Linq;
using CannedBytes.Midi.Core;
using CannedBytes.Midi.Device.Converters;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device;

public sealed class SchemaNodeMapFactory
{
    private readonly ConverterManager _converterMgr;

    public SchemaNodeMapFactory(ConverterManager converterMgr)
    {
        Assert.IfArgumentNull(converterMgr, nameof(converterMgr));

        _converterMgr = converterMgr;
    }

    public IEnumerable<SchemaNodeMap> CreateAll(DeviceSchema schema)
    {
        Assert.IfArgumentNull(schema, nameof(schema));

        List<SchemaNodeMap> maps = new();

        foreach (var rootField in schema.VirtualRootFields)
        {
            var map = Create(rootField);

            maps.Add(map);
        }

        return maps;
    }

    public SchemaNodeMap Create(Field rootField)
    {
        Assert.IfArgumentNull(rootField, nameof(rootField));

        var rootNode = CreateNode(rootField, 0);
        rootNode.IsRoot = true;

        var lastNode = BuildNode(rootNode);

        var map = CreateMap(rootNode, lastNode);

        return map;
    }

    private SchemaNodeMap CreateMap(SchemaNode rootNode, SchemaNode lastNode)
    {
        SchemaNodeMap map = new(rootNode)
        {
            LastNode = lastNode
        };

        var thisNode = rootNode;

        while (thisNode is not null)
        {
            // initialize each node

            InitializeIsAddressMap(thisNode);

            if (thisNode.IsRecord)
            {
                InitializeRecord(thisNode);
            }
            else
            {
                InitializeData(thisNode);
            }

            // next node
            thisNode = thisNode.Next;
        }

        // find first IsAddressMap node
        map.AddressMap = (from node in map.RootNode.SelectNodes(node => node.Next)
                          where node.IsAddressMap
                          select node).FirstOrDefault();

        return map;
    }

    private void InitializeIsAddressMap(SchemaNode thisNode)
    {
        if (!thisNode.IsAddressMap &&
            thisNode.Parent is not null)
        {
            thisNode.IsAddressMap = thisNode.Parent.IsAddressMap;
        }
    }

    private void InitializeRecord(SchemaNode thisNode)
    {
        BuildKey(thisNode);

        thisNode.DataLength = thisNode.FieldConverterPair.Converter.ByteLength;

        if (thisNode.IsAddressMap)
        {
            //
            // if thisNode is not a clone and an address is specified in the schema,
            // use it as the current address
            //
            if (!thisNode.IsClone &&
                thisNode.FieldConverterPair.Field.Properties.Address != 0)
            {
                thisNode.Address = thisNode.FieldConverterPair.Field.Properties.Address;
            }
            //
            // use the size of the previous record to calculate the new address
            // if no address was specified for the record (previous condition).
            //
            else if (!thisNode.IsClone && thisNode.IsRecord &&
                     thisNode.PreviousRecord is not null &&
                     thisNode.PreviousRecord.FieldConverterPair.Field.Properties.Size != 0)
            {
                thisNode.Address = thisNode.PreviousRecord.Address +
                    thisNode.PreviousRecord.FieldConverterPair.Field.Properties.Size;
            }
            //
            // if thisNode is a clone and a size is specified in the schema,
            // use it to calculate a new address
            //
            else if (thisNode.FieldConverterPair.Field.Properties.Size != 0 &&
                     thisNode.IsClone)
            {
                thisNode.Address = thisNode.PreviousClone!.Address +
                    thisNode.FieldConverterPair.Field.Properties.Size;
            }
            //
            // if the previous node is a record, 
            // use the address specified there
            //
            else if (thisNode.Previous?.IsRecord == true)
            {
                thisNode.Address = thisNode.Previous.Address;
            }
            //
            // use the previousField's (not a record) DataLength
            // to calculate a new address
            //
            else if (thisNode.PreviousField?.IsAddressMap == true)
            {
                int dataLength = thisNode.PreviousField.DataLength;

                if (dataLength == 0)
                {
                    var prevField = thisNode.PreviousField;

                    //
                    // find a previous field that has length
                    //
                    while (prevField?.IsAddressMap == true &&
                           prevField.DataLength == 0)
                    {
                        prevField = prevField.PreviousField;
                    }

                    if (prevField is not null)
                    {
                        dataLength = prevField.DataLength;
                    }
                }

                thisNode.Address = thisNode.PreviousField.Address + dataLength;
            }
        }
    }

    private void InitializeData(SchemaNode thisNode)
    {
        BuildKey(thisNode);

        thisNode.DataLength = CalculateDataLength(thisNode);

        if (thisNode.Parent?.IsAddressMap == true)
        {
            if (thisNode.IsFirstInRecord)
            {
                thisNode.Address = thisNode.Parent.Address;
            }
            else
            {
                thisNode.Address = thisNode.PreviousField?.Address ?? SevenBitUInt32.Zero;

                int dataLength = thisNode.PreviousField?.DataLength ?? 0;

                if (dataLength == 0)
                {
                    var prevFieldNode = thisNode.PreviousField;

                    while (prevFieldNode?.DataLength == 0)
                    {
                        prevFieldNode = prevFieldNode.PreviousField;
                    }

                    if (prevFieldNode is not null)
                    {
                        dataLength = prevFieldNode.DataLength;
                    }
                }

                thisNode.Address += dataLength;
            }
        }
    }

    private static int CalculateDataLength(SchemaNode thisNode)
    {
        int byteLength = thisNode.FieldConverterPair.Converter.ByteLength;

        if (byteLength < 0)
            throw new DeviceException("Negative ByteLength (BitFlags) is not supported anymore.");

        return byteLength;
    }

    private static void BuildKey(SchemaNode thisNode)
    {
        if (!thisNode.IsRoot)
        {
            thisNode.Key = new InstancePathKey(thisNode.InstanceIndex);

            foreach (SchemaNode parentNode in thisNode.SelectNodes(node => node.Parent))
            {
                if (!parentNode.IsRoot)
                {
                    // don't add the index of the root (always 0).
                    thisNode.Key.Add(parentNode.InstanceIndex);
                }
            }
        }
        else
        {
            // root node
            thisNode.Key = new InstancePathKey();
        }
    }

    private SchemaNode BuildNode(SchemaNode thisNode)
    {
        if (thisNode.IsRecord)
        {
            Dictionary<string, SchemaNode> clones = new();

            var parentNode = thisNode;
            SchemaNode? lastSibling = null;
            int lastIndex = 0;

            foreach (FieldInfo fieldInfo in new FieldIterator(thisNode.Field))
            {
                // insert additional record/parent clones
                if (lastIndex < fieldInfo.InstanceIndex)
                {
                    lastIndex = fieldInfo.InstanceIndex;

                    CreateClonedParent(ref thisNode, ref parentNode, lastIndex);

                    // start a new set of siblings
                    lastSibling = null;
                }

                // the first (real/not-clone) node at this level is always index 0
                var newNode = CreateNewNode(thisNode, parentNode, fieldInfo.Field, 0);

                ManageSiblings(ref lastSibling, newNode);

                ManageClones(clones, newNode);

                thisNode = BuildNode(newNode);
            }
        }

        return thisNode;
    }

    private static void ManageSiblings(ref SchemaNode? lastSibling, SchemaNode newNode)
    {
        if (lastSibling is not null)
        {
            lastSibling.NextSibling = newNode;
            newNode.PreviousSibling = lastSibling;
        }

        lastSibling = newNode;
    }

    private static void ManageClones(Dictionary<string, SchemaNode> clones, SchemaNode newNode)
    {
        if (clones.ContainsKey(newNode.FieldConverterPair.Field.Name.FullName))
        {
            var clone = clones[newNode.FieldConverterPair.Field.Name.FullName];

            newNode.PreviousClone = clone;
            clone.NextClone = newNode;
        }

        clones[newNode.FieldConverterPair.Field.Name.FullName] = newNode;
    }

    private SchemaNode CreateNewNode(SchemaNode thisNode, SchemaNode parentNode, Field field, int instanceIndex)
    {
        var newNode = CreateNode(field, instanceIndex);

        thisNode.Next = newNode;
        newNode.Previous = thisNode;
        newNode.Parent = parentNode;
        parentNode.Children.Add(newNode);

        newNode.IsClone = parentNode.IsClone;

        return newNode;
    }

    private static void CreateClonedParent(ref SchemaNode thisNode, ref SchemaNode parentNode, int instanceIndex)
    {
        SchemaNode clonedParent = new(
            parentNode.FieldConverterPair, instanceIndex)
        {
            IsClone = true
        };

        thisNode.Next = clonedParent;
        clonedParent.Previous = thisNode;

        clonedParent.Parent = parentNode.Parent;
        // Add clones to parent also? NO!
        //parentNode.Parent.Children.Add(clonedParent); 

        parentNode.NextClone = clonedParent;
        clonedParent.PreviousClone = parentNode;

        parentNode = clonedParent;
        thisNode = clonedParent;
    }

    private SchemaNode CreateNode(Field field, int instanceIndex)
    {
        var pair = _converterMgr.GetFieldConverterPair(field);

        var node = new SchemaNode(pair, instanceIndex);

        return node;
    }
}
