using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using CannedBytes.Midi.Device.Converters;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device;

[Export]
public sealed class SchemaNodeMapFactory
{
    private readonly ConverterManager _converterMgr;

    [ImportingConstructor]
    public SchemaNodeMapFactory([Import] ConverterManager converterMgr)
    {
        Check.IfArgumentNull(converterMgr, "converterMgr");

        _converterMgr = converterMgr;
    }

    public IEnumerable<SchemaNodeMap> CreateAll(DeviceSchema schema)
    {
        Check.IfArgumentNull(schema, "schema");

        List<SchemaNodeMap> maps = new();

        foreach (Field rootField in schema.VirtualRootFields)
        {
            SchemaNodeMap map = Create(rootField);

            maps.Add(map);
        }

        return maps;
    }

    public SchemaNodeMap Create(Field rootField)
    {
        Check.IfArgumentNull(rootField, "rootField");

        SchemaNode rootNode = CreateNode(rootField, 0);
        rootNode.IsRoot = true;

        SchemaNode lastNode = BuildNode(rootNode);

        SchemaNodeMap map = CreateMap(rootNode, lastNode);

        return map;
    }

    private SchemaNodeMap CreateMap(SchemaNode rootNode, SchemaNode lastNode)
    {
        SchemaNodeMap map = new(rootNode)
        {
            LastNode = lastNode
        };

        Carry carry = new();
        SchemaNode thisNode = rootNode;

        while (thisNode != null)
        {
            // initialize each node

            InitializeIsAddressMap(thisNode);

            if (thisNode.IsRecord)
            {
                carry.Clear();
                InitializeRecord(thisNode);
            }
            else
            {
                InitializeData(thisNode, carry);
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
            thisNode.Parent != null)
        {
            thisNode.IsAddressMap = thisNode.Parent.IsAddressMap;
        }
    }

    private void InitializeRecord(SchemaNode thisNode)
    {
        BuildKey(thisNode, thisNode.InstanceIndex);

        thisNode.DataLength = thisNode.FieldConverterPair.Converter.ByteLength;

        if (thisNode.IsAddressMap)
        {
            //
            // if thisNode is not a clone and an address is specified in the schema,
            // use it as the current address
            //
            if (!thisNode.IsClone &&
                thisNode.FieldConverterPair.Field.ExtendedProperties.Address != 0)
            {
                thisNode.Address = thisNode.FieldConverterPair.Field.ExtendedProperties.Address;
            }
            //
            // use the size of the previous record to calculate the new address
            // if no address was specified for the record (previous condition).
            //
            else if (!thisNode.IsClone && thisNode.IsRecord &&
                     thisNode.PreviousRecord != null &&
                     thisNode.PreviousRecord.FieldConverterPair.Field.ExtendedProperties.Size != 0)
            {
                thisNode.Address = thisNode.PreviousRecord.Address +
                    thisNode.PreviousRecord.FieldConverterPair.Field.ExtendedProperties.Size;
            }
            //
            // if thisNode is a clone and a size is specified in the schema,
            // use it to calculate a new address
            //
            else if (thisNode.FieldConverterPair.Field.ExtendedProperties.Size != 0 &&
                     thisNode.IsClone)
            {
                thisNode.Address = thisNode.PreviousClone.Address +
                    thisNode.FieldConverterPair.Field.ExtendedProperties.Size;
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
                    SchemaNode prevField = thisNode.PreviousField;

                    //
                    // find a previous field that has length
                    //
                    while (prevField?.IsAddressMap == true &&
                           prevField.DataLength == 0)
                    {
                        prevField = prevField.PreviousField;
                    }

                    if (prevField != null)
                    {
                        dataLength = prevField.DataLength;
                    }
                }

                thisNode.Address = thisNode.PreviousField.Address + dataLength;
            }
        }
    }

    private void InitializeData(SchemaNode thisNode, Carry carry)
    {
        // data fields have always a key of 0.
        BuildKey(thisNode, 0);

        CalculateDataLength(thisNode, carry);

        if (thisNode.Parent.IsAddressMap)
        {
            if (thisNode.IsFirstInRecord)
            {
                thisNode.Address = thisNode.Parent.Address;
            }
            else
            {
                thisNode.Address = thisNode.PreviousField.Address;

                int dataLength = thisNode.PreviousField.DataLength;

                if (dataLength == 0)
                {
                    SchemaNode prevFieldNode = thisNode.PreviousField;

                    while (prevFieldNode?.DataLength == 0)
                    {
                        prevFieldNode = prevFieldNode.PreviousField;
                    }

                    if (prevFieldNode != null)
                    {
                        dataLength = prevFieldNode.DataLength;
                    }
                }

                thisNode.Address += dataLength;
            }
        }
    }

    private static void CalculateDataLength(SchemaNode thisNode, Carry carry)
    {
        // TODO: How to stack streamConver's calcs?

        int byteLength = thisNode.FieldConverterPair.Converter.ByteLength;

        if (byteLength < 0)
        {
            BitFlags flags = (BitFlags)Math.Abs(byteLength);

            byteLength = carry.ReadFrom(null, flags, out ushort temp);
        }
        else
        {
            // clear
            carry.Clear();
        }

        thisNode.DataLength = byteLength;
    }

    private static void BuildKey(SchemaNode thisNode, int instanceIndex)
    {
        if (!thisNode.IsRoot)
        {
            thisNode.Key = new InstancePathKey(instanceIndex);

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

            SchemaNode parentNode = thisNode;
            SchemaNode lastSibling = null;

            FieldIterator iterator = new(thisNode.FieldConverterPair.Field);
            int lastIndex = 0;

            foreach (FieldInfo fieldInfo in iterator)
            {
                // insert additional record/parent clones
                if (lastIndex < fieldInfo.InstanceIndex)
                {
                    lastIndex = fieldInfo.InstanceIndex;

                    CreateClonedParent(ref thisNode, ref parentNode, lastIndex);

                    // start a new set of siblings
                    lastSibling = null;
                }

                SchemaNode newNode = CreateNewNode(thisNode, parentNode,
                    fieldInfo.Field, fieldInfo.InstanceIndex);

                ManageSiblings(ref lastSibling, newNode);

                ManageClones(clones, newNode);

                thisNode = BuildNode(newNode);
            }
        }

        return thisNode;
    }

    private static void ManageSiblings(ref SchemaNode lastSibling, SchemaNode newNode)
    {
        if (lastSibling != null)
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
            SchemaNode clone = clones[newNode.FieldConverterPair.Field.Name.FullName];

            newNode.PreviousClone = clone;
            clone.NextClone = newNode;
        }

        clones[newNode.FieldConverterPair.Field.Name.FullName] = newNode;
    }

    private SchemaNode CreateNewNode(SchemaNode thisNode, SchemaNode parentNode, Field field, int instanceIndex)
    {
        SchemaNode newNode = CreateNode(field, instanceIndex);

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
        FieldConverterPair pair = _converterMgr.GetFieldConverterPair(field);

        SchemaNode node = new(pair, instanceIndex);

        return node;
    }
}
