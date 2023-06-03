using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CannedBytes.Midi.Device.Converters;
using CannedBytes.Midi.Device.Schema;
using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device.Message
{
    /// <summary>
    /// Represents a node in the <see cref="MidiDeviceBinaryMap"/> for a <see cref="FieldConverterPair"/>
    /// that is interconnected (in various ways) to other nodes.
    /// </summary>
    public class FieldNode
    {
        /// <summary>
        /// Constructs a new instance for the specified <paramref name="fieldConverterPair"/>.
        /// </summary>
        /// <param name="fieldConverterPair">Must not be null.</param>
        public FieldNode(FieldConverterPair fieldConverterPair, int instanceIndex)
        {
            Check.IfArgumentNull(fieldConverterPair, "fieldConverterPair");

            InstanceIndex = instanceIndex;
            FieldConverterPair = fieldConverterPair;
            IsRecord = FieldConverterPair.GroupConverter != null;
            IsAddressMap = FieldConverterPair.GroupConverter is AddressMapGroupConverter;
        }

        /// <summary>
        /// Gets a unique key value for the current node (Field position).
        /// </summary>
        public FieldPathKey Key { get; protected set; }

        /// <summary>
        /// Gets the <see cref="Field"/> - <see cref="IConverter"/> pair for this node.
        /// </summary>
        public FieldConverterPair FieldConverterPair { get; protected set; }

        /// <summary>
        /// Gets an indication if node is a RecordType/GroupConverter (true) or DataType/Converter (false).
        /// </summary>
        public bool IsRecord { get; protected set; }

        /// <summary>
        /// Gets the processed byte length of this field in the physical stream.
        /// </summary>
        public int DataLength { get; protected set; }

        /// <summary>
        /// Gets the index of this instance.
        /// </summary>
        public int InstanceIndex { get; protected set; }

        /// <summary>
        /// Gets the address of the field-converter pair (inside address map)
        /// </summary>
        public SevenBitUInt32 Address { get; protected set; }

        // field part of address map
        public bool IsAddressMap { get; protected set; }

        /// <summary>
        /// Indicates if the field node is the first in a record (also for clones).
        /// </summary>
        public bool IsFirstInRecord
        {
            get
            {
                if (PreviousNode == ParentNode) return true;

                foreach (var node in SelectNodes((node) => { return node.PreviousClone; }))
                {
                    if (node.PreviousNode == node.ParentNode)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Next node in the flattened hierarchy.
        /// </summary>
        public FieldNode NextNode { get; set; }

        /// <summary>
        /// Previous node in the flattened hierarchy.
        /// </summary>
        public FieldNode PreviousNode { get; set; }

        /// <summary>
        /// Next node at the same level (depth) within a record.
        /// </summary>
        public FieldNode NextSibling { get; set; }

        /// <summary>
        /// Previous node at the same level (depth) within a record.
        /// </summary>
        public FieldNode PreviousSibling { get; set; }

        /// <summary>
        /// A parent (record) node.
        /// </summary>
        public FieldNode ParentNode { get; set; }

        /// <summary>
        /// Next duplicated node (unique InstanceIndex) at the same level (depth) within a record.
        /// </summary>
        public FieldNode NextClone { get; set; }

        /// <summary>
        /// Previous duplicated node (unique InstanceIndex) at the same level (depth) within a record.
        /// </summary>
        public FieldNode PreviousClone { get; set; }

        /// <summary>
        /// Next node in the flattened hierarchy that is not a record.
        /// </summary>
        public FieldNode NextField
        {
            get
            {
                if (NextNode != null)
                {
                    if (NextNode.IsRecord)
                    {
                        return NextNode.NextField;
                    }

                    return NextNode;
                }

                return null;
            }
        }

        /// <summary>
        /// Previous node in the flattened hierarchy that is not a record.
        /// </summary>
        public FieldNode PreviousField
        {
            get
            {
                if (PreviousNode != null)
                {
                    if (PreviousNode.IsRecord)
                    {
                        return PreviousNode.PreviousField;
                    }

                    return PreviousNode;
                }

                return null;
            }
        }

        public FieldNode FirstOfAddress
        {
            get
            {
                if (PreviousField != null &&
                    PreviousField.Address == Address)
                {
                    return PreviousField.FirstOfAddress;
                }

                return this;
            }
        }

        public FieldNode LastOfAddress
        {
            get
            {
                if (NextField != null &&
                    NextField.Address == Address)
                {
                    return NextField.LastOfAddress;
                }

                return this;
            }
        }

        public bool IsOfParent(FieldNode parentNode)
        {
            return (from node in this.SelectNodes((node) => { return node.ParentNode; })
                    where node == parentNode
                    select node).Any();
        }

        public IEnumerable<FieldNode> SelectNodes(Func<FieldNode, FieldNode> newNodeFunc)
        {
            var node = newNodeFunc(this);

            while (node != null)
            {
                yield return node;
                node = newNodeFunc(node);
            }
        }

        public FieldNode Last(Func<FieldNode, FieldNode> newNodeFunc)
        {
            var node = newNodeFunc(this);
            var lastNode = node;

            while (node != null)
            {
                node = newNodeFunc(node);
                lastNode = node;
            }

            return lastNode;
        }

        public FieldNode Find(FieldConverterPair fieldConverterPair, Func<FieldNode, FieldNode> newNodeFunc)
        {
            return (from node in SelectNodes(newNodeFunc)
                    where node.FieldConverterPair.Field.Name.FullName == fieldConverterPair.Field.Name.FullName
                    select node).FirstOrDefault();
        }

        public FieldNode FindFirst(Field field, Func<FieldNode, FieldNode> newNodeFunc)
        {
            return (from node in SelectNodes(newNodeFunc)
                    where node.FieldConverterPair.Field.Name.FullName == field.Name.FullName
                    select node).FirstOrDefault();
        }

        public FieldNode Find(Field field, FieldPathKey key, Func<FieldNode, FieldNode> newNodeFunc)
        {
            return (from node in SelectNodes(newNodeFunc)
                    where node.FieldConverterPair.Field.Name.FullName == field.Name.FullName
                    where node.Key.Equals(key)
                    select node).FirstOrDefault();
        }

        public FieldNode FindFirst(SevenBitUInt32 address, Func<FieldNode, FieldNode> newNodeFunc)
        {
            return (from node in SelectNodes(newNodeFunc)
                    where node.IsAddressMap
                    where node.Address == address
                    select node).FirstOrDefault();
        }

        public FieldNode FindLast(SevenBitUInt32 address, Func<FieldNode, FieldNode> newNodeFunc)
        {
            FieldNode lastNode = null;

            foreach (var node in SelectNodes(newNodeFunc))
            {
                if (node.IsAddressMap)
                {
                    if (node.Address >= address)
                    {
                        return node;
                    }
                }

                lastNode = node;
            }

            return null;
        }

        public void Initialize(MidiDeviceDataContext context, GroupConverter parentConverter)
        {
            // TODO: does not stop at the correct position.
            if (PreviousNode != null &&
                PreviousNode.IsAddressMap)
            {
                IsAddressMap = true;
            }

            if (IsRecord)
            {
                BuildKey(this.InstanceIndex);

                DataLength = FieldConverterPair.Converter.ByteLength;

                if (this.PreviousClone == null &&
                    !String.IsNullOrEmpty(FieldConverterPair.Field.Address))
                {
                    //Address = new HexValue(FieldConverterPair.Field.Address).Value;
                    Address = new SevenBitUInt32(FieldConverterPair.Field.Address);
                }
                else if (!String.IsNullOrEmpty(FieldConverterPair.Field.Size) &&
                    PreviousClone != null)
                {
                    //var size = new HexValue(FieldConverterPair.Field.Size).Value;
                    var size = new SevenBitUInt32(FieldConverterPair.Field.Size);

                    Address = this.PreviousClone.Address + size;
                }
                else if (PreviousNode != null &&
                    PreviousNode.IsRecord &&
                    PreviousNode.IsAddressMap)
                {
                    Address = PreviousNode.Address;
                }
                else if (PreviousField != null &&
                         PreviousField.IsAddressMap)
                {
                    if (PreviousField.DataLength == 0)
                    {
                        var node = PreviousField;

                        while (node != null &&
                               node.DataLength == 0)
                        {
                            node = node.PreviousField;
                        }

                        Address = PreviousField.Address + node.DataLength;
                    }
                    else
                    {
                        Address = PreviousField.Address + PreviousField.DataLength;
                    }
                }
            }
            else
            {
                BuildKey(0);

                DataLength = parentConverter.CalculateByteLength(FieldConverterPair.Converter, context.Carry);

                if (!String.IsNullOrEmpty(this.ParentNode.FieldConverterPair.Field.Size) &&
                    PreviousClone != null &&
                    PreviousClone.IsFirstInRecord)
                {
                    //var size = new HexValue(this.ParentNode.FieldConverterPair.Field.Size).Value;
                    var size = new SevenBitUInt32(this.ParentNode.FieldConverterPair.Field.Size);

                    Address = this.ParentNode.Address + (InstanceIndex * size);
                }
                else if (PreviousNode.IsRecord &&
                    (PreviousNode.Address != 0 || PreviousNode.IsAddressMap))
                {
                    Address = PreviousNode.Address;
                }
                else if (PreviousField != null &&
                         (PreviousField.Address != 0 || PreviousNode.IsAddressMap))
                {
                    if (DataLength == 0)
                    {
                        Address = PreviousField.Address;
                    }
                    else if (PreviousField.DataLength == 0)
                    {
                        var node = PreviousField;

                        while (node != null &&
                               node.DataLength == 0)
                        {
                            node = node.PreviousField;
                        }

                        Address = PreviousField.Address + node.DataLength;
                    }
                    else
                    {
                        Address = PreviousField.Address + PreviousField.DataLength;
                    }
                }
            }
        }

        private void BuildKey(int initialIndex)
        {
            if (this.ParentNode != null)
            {
                this.Key = new FieldPathKey(initialIndex);

                foreach (var parentNode in SelectNodes((node) => { return node.ParentNode; }))
                {
                    if (parentNode.ParentNode != null)
                    {
                        // don't add the index of the root (always 0).
                        this.Key.Add(parentNode.InstanceIndex);
                    }
                }
            }
            else
            {
                this.Key = new FieldPathKey();
            }
        }

        public override string ToString()
        {
            StringBuilder text = new StringBuilder();

            text.Append(new string(' ', Key.Depth * 2));
            //text.Append("> ");
            text.Append(Address.ToString("X"));
            text.Append(" (");
            text.Append(DataLength);
            text.Append("): ");
            text.Append(FieldConverterPair.Field.ToString());
            text.Append("[");
            text.Append(Key.ToString());
            text.Append("]");

            return text.ToString();
        }
    }
}