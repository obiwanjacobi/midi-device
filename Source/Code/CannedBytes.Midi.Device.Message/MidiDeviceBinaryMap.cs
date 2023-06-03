using System.Collections.Generic;
using System.Text;
using CannedBytes.Midi.Device.Converters;
using CannedBytes.Midi.Device.Schema;
using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device.Message
{
    /// <summary>
    /// Manages binary information for a data context.
    /// </summary>
    public class MidiDeviceBinaryMap
    {
        public MidiDeviceBinaryMap(GroupConverter rootConverter)
        {
            Check.IfArgumentNull(rootConverter, "rootConverter");

            Initialize(rootConverter);
        }

        private void Initialize(GroupConverter rootConverter)
        {
            var context = new MidiDeviceDataContext(rootConverter);
            var pair = new FieldConverterPair(Field.VirtualRootField, rootConverter);

            RootNode = new FieldNode(pair, 0);
            RootNode.Initialize(context, null);

            LastNode = Add(context, RootNode);
        }

        private FieldNode Add(MidiDeviceDataContext context, FieldNode thisNode)
        {
            var groupConverter = thisNode.FieldConverterPair.Converter as GroupConverter;

            if (groupConverter != null && groupConverter.HasConverters)
            {
                var clones = new Dictionary<string, FieldNode>();

                using (var enumFields = context.GetEnumerator(groupConverter))
                {
                    FieldNode parentNode = thisNode;
                    FieldNode lastSibling = null;

                    while (enumFields.MoveNext())
                    {
                        // insert additional record clones
                        if (enumFields.IsFirst)
                        {
                            var clonedParent = new FieldNode(parentNode.FieldConverterPair,
                                GetInstanceIndex(context, parentNode.FieldConverterPair));
                            thisNode.NextNode = clonedParent;
                            clonedParent.PreviousNode = thisNode;
                            clonedParent.ParentNode = parentNode.ParentNode;

                            parentNode.NextClone = clonedParent;
                            clonedParent.PreviousClone = parentNode;

                            clonedParent.Initialize(context, parentNode.ParentNode.FieldConverterPair.GroupConverter);

                            parentNode = clonedParent;
                            thisNode = clonedParent;
                        }

                        var newNode = new FieldNode(enumFields.Current, 0);
                        thisNode.NextNode = newNode;
                        newNode.PreviousNode = thisNode;
                        newNode.ParentNode = parentNode;

                        if (lastSibling != null)
                        {
                            lastSibling.NextSibling = newNode;
                            newNode.PreviousSibling = lastSibling;
                        }

                        if (clones.ContainsKey(newNode.FieldConverterPair.Field.Name.FullName))
                        {
                            var clone = clones[newNode.FieldConverterPair.Field.Name.FullName];

                            newNode.PreviousClone = clone;
                            clone.NextClone = newNode;
                        }

                        clones[newNode.FieldConverterPair.Field.Name.FullName] = newNode;

                        lastSibling = newNode;

                        newNode.Initialize(context, groupConverter);

                        thisNode = Add(context, newNode);
                    }
                }
            }

            return thisNode;
        }

        private static int GetInstanceIndex(MidiDeviceDataContext context, FieldConverterPair fieldConverterPair)
        {
            var dynamicPair = fieldConverterPair as DynamicFieldConverterPair;

            if (dynamicPair != null)
            {
                return dynamicPair.InstanceIndex;
            }
            else
            {
                return context.CurrentInstanceIndex;
            }
        }

        public FieldNode RootNode { get; protected set; }

        public FieldNode LastNode { get; protected set; }

        public FieldNode Find(FieldConverterPair fieldConverterPair)
        {
            return RootNode.Find(fieldConverterPair, (node) => { return node.NextNode; });
        }

        public FieldNode FindFirst(Field field)
        {
            return RootNode.FindFirst(field, (node) => { return node.NextNode; });
        }

        public FieldNode Find(Field field, FieldPathKey key)
        {
            if (key == null)
            {
                return FindFirst(field);
            }

            return RootNode.Find(field, key, (node) => { return node.NextNode; });
        }

        public FieldNode FindFirst(SevenBitUInt32 address)
        {
            return RootNode.FindFirst(address, (node) => { return node.NextNode; });
        }

        public FieldNode FindLast(SevenBitUInt32 address)
        {
            return RootNode.FindLast(address, (node) => { return node.NextNode; });
        }

        public override string ToString()
        {
            StringBuilder text = new StringBuilder();

            text.AppendLine(RootNode.ToString());

            foreach (var node in RootNode.SelectNodes((node) => { return node.NextNode; }))
            {
                text.AppendLine(node.ToString());
            }

            return text.ToString();
        }
    }
}