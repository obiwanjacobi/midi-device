﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CannedBytes.Midi.Core;
using CannedBytes.Midi.Device.Converters;

namespace CannedBytes.Midi.Device;

public partial class SchemaNode : ILogicalFieldInfo
{
    protected SchemaNode()
    { }

    public SchemaNode(FieldConverterPair pair, int instanceIndex)
    {
        Check.IfArgumentNull(pair, "pair");
        //Check.IfArgumentOutOfRange(instanceIndex, 0, pair.Field.ExtendedProperties.Repeats, "instanceIndex");

        FieldConverterPair = pair;
        InstanceIndex = instanceIndex;
        InstanceCount = pair.Field.ExtendedProperties.Repeats;

        IsRecord = pair.StreamConverter != null;

        if (pair.StreamConverter != null)
        {
            IsAddressMap = pair.StreamConverter.IsAddressMap;
        }
    }

    /// <summary>
    /// Next node in the flattened hierarchy.
    /// </summary>
    public SchemaNode Next { get; internal protected set; }

    /// <summary>
    /// Previous node in the flattened hierarchy.
    /// </summary>
    public SchemaNode Previous { get; internal protected set; }

    /// <summary>
    /// Next node at the same level (depth) within a record.
    /// </summary>
    public SchemaNode NextSibling { get; internal protected set; }

    /// <summary>
    /// Previous node at the same level (depth) within a record.
    /// </summary>
    public SchemaNode PreviousSibling { get; internal protected set; }

    /// <summary>
    /// A parent (record) node.
    /// </summary>
    public SchemaNode Parent { get; internal protected set; }

    private SchemaNodeCollection _children;

    /// <summary>
    /// Gets the immediate children of this (parent) node (not including clones).
    /// </summary>
    public SchemaNodeCollection Children
    {
        get
        {
            _children ??= new SchemaNodeCollection();

            return _children;
        }
    }

    /// <summary>
    /// Indicates if the node has any children (without triggering the lazy construction of the collection).
    /// </summary>
    public bool HasChildren
    {
        get { return _children?.Count > 0; }
    }

    /// <summary>
    /// Lists all children for a specific instanceIndex.
    /// </summary>
    public IEnumerable<SchemaNode> RepeatedChildren(int instanceIndex)
    {
        Check.IfArgumentOutOfRange(instanceIndex, 0,
            FieldConverterPair.Field.ExtendedProperties.Repeats, "instanceIndex");

        SchemaNode parent = this;
        int currentIndex = instanceIndex;

        while (currentIndex > 0 && parent != null)
        {
            parent = parent.NextClone;

            currentIndex--;
        }

        if (parent == null)
        {
            // internal error!
            throw new ArgumentOutOfRangeException("instanceIndex", instanceIndex,
                "Internal Error: Parameter instanceIndex is too large. No more clones found after: " + (instanceIndex - currentIndex));
        }

        if (parent.HasChildren)
        {
            return parent.Children;
        }

        return Enumerable.Empty<SchemaNode>();
    }

    /// <summary>
    /// Next duplicated node (unique InstanceIndex) at the same level (depth) within a record.
    /// </summary>
    public SchemaNode NextClone { get; internal protected set; }

    /// <summary>
    /// Previous duplicated node (unique InstanceIndex) at the same level (depth) within a record.
    /// </summary>
    public SchemaNode PreviousClone { get; internal protected set; }

    /// <summary>
    /// Previous IsRecord node at the same level (using clones and siblings).
    /// </summary>
    public SchemaNode PreviousRecord
    {
        get
        {
            if (this.IsRecord)
            {
                if (this.PreviousClone != null)
                {
                    return this.PreviousClone;
                }

                if (this.PreviousSibling != null)
                {
                    return this.PreviousSibling.Last(node => node.NextClone);
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Next node in the flattened hierarchy that is not a record.
    /// </summary>
    public SchemaNode NextField
    {
        get
        {
            if (Next != null)
            {
                if (Next.IsRecord)
                {
                    return Next.NextField;
                }

                return Next;
            }

            return null;
        }
    }

    /// <summary>
    /// Previous node in the flattened hierarchy that is not a record.
    /// </summary>
    public SchemaNode PreviousField
    {
        get
        {
            if (Previous != null)
            {
                if (Previous.IsRecord)
                {
                    return Previous.PreviousField;
                }

                return Previous;
            }

            return null;
        }
    }

    /// <summary>
    /// First field-node of the address that this node is also part of.
    /// </summary>
    public SchemaNode FirstFieldOfAddress
    {
        get
        {
            if (PreviousField != null &&
                PreviousField.Address == Address)
            {
                return PreviousField.FirstFieldOfAddress;
            }

            return this;
        }
    }

    /// <summary>
    /// Last field-node of the address that this node is also part of.
    /// </summary>
    public SchemaNode LastFieldOfAddress
    {
        get
        {
            if (NextField != null &&
                NextField.Address == Address)
            {
                return NextField.LastFieldOfAddress;
            }

            return this;
        }
    }

    public bool IsOfParent(SchemaNode parentNode)
    {
        return (from node in this.SelectNodes(node => node.Parent)
                where node == parentNode
                select node).Any();
    }

    public IEnumerable<SchemaNode> SelectNodes(Func<SchemaNode, SchemaNode> newNodeFunc)
    {
        SchemaNode node = newNodeFunc(this);

        while (node != null)
        {
            yield return node;
            node = newNodeFunc(node);
        }
    }

    public SchemaNode Last(Func<SchemaNode, SchemaNode> nextNodeFunc)
    {
        SchemaNode node = nextNodeFunc(this);
        SchemaNode lastNode = node;

        while (node != null)
        {
            node = nextNodeFunc(node);

            if (node != null)
            {
                lastNode = node;
            }
        }

        return lastNode;
    }

    /// <summary>
    /// Gets an indication if node is the root of the message.
    /// </summary>
    public bool IsRoot { get; internal protected set; }

    /// <summary>
    /// Gets an indication if node is a RecordType/GroupConverter (true) or DataType/Converter (false).
    /// </summary>
    public bool IsRecord { get; protected set; }

    /// <summary>
    /// Indicates if the field part of the address map
    /// </summary>
    public bool IsAddressMap { get; internal protected set; }

    /// <summary>
    /// Gets an indication if node is a clone of another field (with a different InstanceIndex).
    /// </summary>
    public bool IsClone { get; internal protected set; }

    /// <summary>
    /// Indicates if the field node is the first in a record (also for clones).
    /// </summary>
    public bool IsFirstInRecord
    {
        get
        {
            if (Previous == Parent) return true;

            foreach (SchemaNode node in SelectNodes(node => node.PreviousClone))
            {
                if (node.Previous == node.Parent)
                {
                    return true;
                }
            }

            return false;
        }
    }

    /// <summary>
    /// Gets a unique key value for the current node (Field position).
    /// </summary>
    public InstancePathKey Key { get; internal protected set; }

    /// <summary>
    /// Gets the <see cref="Field"/> - <see cref="IConverter"/> pair for this node.
    /// </summary>
    public FieldConverterPair FieldConverterPair { get; protected set; }

    /// <summary>
    /// Gets the address of the field-converter pair (inside address map)
    /// </summary>
    public SevenBitUInt32 Address { get; internal protected set; }

    /// <summary>
    /// Gets the processed byte length of this field in the physical stream.
    /// </summary>
    public int DataLength { get; internal protected set; }

    /// <summary>
    /// Gets the index of this instance.
    /// </summary>
    public int InstanceIndex { get; protected set; }

    /// <summary>
    /// Gets the count of instance repeats.
    /// </summary>
    public int InstanceCount { get; protected set; }

    /// <summary>
    /// Returns a string that represents the node in a readable format.
    /// </summary>
    /// <returns>Never returns null.</returns>
    public override string ToString()
    {
        StringBuilder text = new();

        if (Key != null)
        {
            text.Append(new string(' ', Key.Depth * 2));
        }

        text.Append("A:");
        text.Append(Address.ToString("X"));
        text.Append(" (L:");
        text.Append(DataLength);
        text.Append("): ");

        text.Append(FieldConverterPair.Field.ToString());

        if (IsClone)
        {
            text.Append('\'');
        }

        if (Key != null)
        {
            text.Append(" [");
            text.Append(Key.ToString());
            text.Append(']');
        }

        return text.ToString();
    }

    protected void CopyTo(SchemaNode targetNode)
    {
        Check.IfArgumentNull(targetNode, "targetNode");

        targetNode.Address = this.Address;
        targetNode.DataLength = this.DataLength;
        targetNode.FieldConverterPair = this.FieldConverterPair;
        targetNode.InstanceCount = this.InstanceCount;
        targetNode.InstanceIndex = this.InstanceIndex;
        targetNode.IsAddressMap = this.IsAddressMap;
        targetNode.IsClone = this.IsClone;
        targetNode.IsRecord = this.IsRecord;
        targetNode.IsRoot = this.IsRoot;
        targetNode.Key = this.Key;
        targetNode.Next = this.Next;
        targetNode.NextClone = this.NextClone;
        targetNode.NextSibling = this.NextSibling;
        targetNode.Parent = this.Parent;
        targetNode.Previous = this.Previous;
        targetNode.PreviousClone = this.PreviousClone;
        targetNode.PreviousSibling = this.PreviousSibling;
    }

    #region ILogicalFieldInfo members

    public Schema.Field Field
    {
        get { return FieldConverterPair.Field; }
    }

    ILogicalFieldInfo ILogicalFieldInfo.Parent
    {
        get { return this.Parent; }
    }

    #endregion
}
