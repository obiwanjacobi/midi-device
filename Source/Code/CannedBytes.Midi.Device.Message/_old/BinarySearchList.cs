namespace CannedBytes.Midi.Device.Message
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using CannedBytes.Collections;
    using CannedBytes.Midi.Device.Converters;
    using CannedBytes.Midi.Device.Schema;

    /// <summary>
    /// Maintains a tree structure for field values for all root record types (as GroupConverters).
    /// Used to retrieve a RecordType/GroupConverter for a specific sequence of bytes.
    /// </summary>
    public class BinarySearchList
    {
        public BinarySearchList(DeviceSchema schema)
        {
            Schema = schema;
        }

        public DeviceSchema Schema { get; protected set; }

        public void InitializeFrom(ConverterManager converterMgr)
        {
            Check.IfArgumentNull(converterMgr, "converterMgr");

            foreach (var recordType in this.Schema.RootRecordTypes)
            {
                var converter = converterMgr.GetConverter(recordType);
                Add(converter);
            }
        }

        private TreeNode<ByteValueItem> _root = new TreeNode<ByteValueItem>(new ByteValueItem(0xF0));

        public void Add(FieldConverterPair fieldConverterPair)
        {
            Check.IfArgumentNull(fieldConverterPair, "fieldConverterPair");

            if (Schema.SchemaName != fieldConverterPair.Field.Schema.SchemaName)
            {
                throw new ArgumentException("Field does not belong to this schema.", "fieldConverterPair");
            }
            if (fieldConverterPair.Field.RecordType == null)
            {
                throw new ArgumentException("Field is not a record", "fieldConverterPair");
            }

            Add((GroupConverter)fieldConverterPair.Converter);
        }

        public void Add(GroupConverter converter)
        {
            byte[] values = GetValuesFor(converter);

            NavigateValuePathAndAdd(converter, values);
        }

        public IEnumerable<GroupConverter> Find(byte[] values)
        {
            return Find(values, values.Length);
        }

        public IEnumerable<GroupConverter> Find(byte[] values, int count)
        {
            TreeNode<ByteValueItem> current = null;

            for (int i = 0; i < count; i++)
            {
                byte value = values[i];

                if (current == null)
                {
                    if (value == _root.Item.Value)
                    {
                        current = _root;
                    }
                    else
                    {
                        //throw new ArgumentException("SOX not found.", "values");
                        return null;
                    }
                }
                else
                {
                    string textValue = value.ToString();

                    if (current.Children.Contains(textValue))
                    {
                        current = current.Children[textValue];
                    }
                    else
                    {
                        // search for a blank spot - a place holder for any value
                        textValue = ByteValueItem.NoValue.ToString();

                        if (current.Children.Contains(textValue))
                        {
                            current = current.Children[textValue];
                        }
                        else
                        {
                            // value not found, and no blank spot found.
                            return null;
                        }
                    }
                }

                // exit on the first unique RecordType found
                if (current != null && current.Item.Converters.Count == 1)
                {
                    break;
                }
            }

            return current.Item.Converters;
        }

        private void NavigateValuePathAndAdd(GroupConverter converter, byte[] values)
        {
            TreeNode<ByteValueItem> current = null;

            foreach (byte value in values)
            {
                if (current == null)
                {
                    if (value == _root.Item.Value)
                    {
                        current = _root;
                    }
                    else
                    {
                        //throw new ArgumentException("SOX not found.", "values");
                        break;
                    }
                }
                else
                {
                    if (current.Children.Contains(value.ToString()))
                    {
                        current = current.Children[value.ToString()];
                    }
                    else
                    {
                        TreeNode<ByteValueItem> newNode = new TreeNode<ByteValueItem>(new ByteValueItem(value));
                        current.Children.Add(newNode);
                        current = newNode;
                    }
                }

                current.Item.Converters.Add(converter);
            }
        }

        private byte[] GetValuesFor(GroupConverter converter)
        {
            List<byte> values = new List<byte>();
            var carry = new Carry();

            foreach (FieldConverterPair pair in converter.FieldConverterMap)
            {
                if (pair.Field.RecordType != null)
                {
                    values.AddRange(GetValuesFor((GroupConverter)pair.Converter));
                }
                else
                {
                    // TODO: rewrite!!
                    // byte length processing is flawed.

                    int count = converter.CalculateByteLength(pair.Converter, carry);

                    Constraint fixedConstraint = pair.Field.Constraints.Find(ConstraintType.FixedValue);

                    if (fixedConstraint != null)
                    {
                        values.Add(fixedConstraint.GetValue<byte>());
                    }
                    else
                    {
                        var enums = pair.Field.Constraints.FindAll(ConstraintType.Enumeration);

                        if (enums != null && enums.Count() == 1)
                        {
                            values.Add(enums.First().GetValue<byte>());
                        }
                        else
                        {
                            if (count > 0)
                            {
                                for (int i = 0; i < count; i++)
                                {
                                    values.Add(ByteValueItem.NoValue);
                                }
                            }
                            else
                            {
                                values.Add(ByteValueItem.NoValue);
                            }
                        }
                    }
                }
            }

            return values.ToArray();
        }

        public override string ToString()
        {
            StringBuilder text = new StringBuilder();

            text.Append("Search Tree for \"");
            text.Append(Schema.SchemaName);
            text.AppendLine("\"");

            DumpNode(_root, text, "");

            return text.ToString();
        }

        private void DumpNode(TreeNode<ByteValueItem> node, StringBuilder text, string indent)
        {
            text.Append(indent);
            text.Append("Byte: " + node.Item.Value + " => ");

            foreach (GroupConverter converter in node.Item.Converters)
            {
                text.Append(converter.Name);
                text.Append(" ");
            }

            text.AppendLine();

            foreach (TreeNode<ByteValueItem> childNode in node.Children)
            {
                DumpNode(childNode, text, indent + " ");
            }
        }

        //---------------------------------------------------------------------

        private class ByteValueItem
        {
            public const byte NoValue = 0xFF;

            public ByteValueItem(byte value)
            {
                Value = value;
                Converters = new List<GroupConverter>();
            }

            public byte Value { get; private set; }

            public List<GroupConverter> Converters { get; private set; }

            public override string ToString()
            {
                if (Value == NoValue)
                {
                    return "*";
                }

                return Value.ToString();
            }
        }
    }
}