using System;
using System.Collections.Generic;
using System.Linq;
using CannedBytes.Midi.Device.Converters;
using CannedBytes.Midi.Device.Schema;
using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device.Message
{
    public class MessageTypeFactory
    {
        private ConverterManager _converterMgr;
        private MidiDeviceBinaryMap _binaryMap;
        private IDeviceSchemaProvider _schemaProvider;

        public MessageTypeFactory(ConverterManager converterMgr, MidiDeviceBinaryMap binaryMap, IDeviceSchemaProvider schemaProvider)
        {
            _converterMgr = converterMgr;
            _binaryMap = binaryMap;
            _schemaProvider = schemaProvider;
        }

        public GroupConverter CreateDynamicGroupConverter(SevenBitUInt32 address, SevenBitUInt32 size)
        {
            var endAddress = address + size;
            var startNode = _binaryMap.FindFirst(address);
            var endNode = _binaryMap.FindLast(endAddress);
            int fillerSize = 0;

            // not a valid address
            if (startNode == null) return null;

            if (endNode != null)
            {
                endNode = endNode.PreviousField;

                var totalSize = (endNode.Address + endNode.FirstOfAddress.DataLength) - startNode.Address;

                while (totalSize > size)
                {
                    endNode = endNode.PreviousField;

                    totalSize = (endNode.Address + endNode.FirstOfAddress.DataLength) - startNode.Address;
                    fillerSize = size - totalSize;
                }
            }

            return CreateDynamicGroupConverter(startNode, endNode, fillerSize);
        }

        public GroupConverter CreateDynamicGroupConverter(FieldNode startNode, FieldNode endNode)
        {
            return CreateDynamicGroupConverter(startNode, endNode, 0);
        }

        public GroupConverter CreateDynamicGroupConverter(FieldNode startNode, FieldNode endNode, int fillerSize)
        {
            Check.IfArgumentNull(startNode, "startNode");
            if (!startNode.IsAddressMap)
            {
                throw new ArgumentException("Specified Field-Node not part of the AddressMap", "startNode");
            }
            if (endNode != null && !endNode.IsAddressMap)
            {
                throw new ArgumentException("Specified Field-Node not part of the AddressMap", "endNode");
            }

            Stack<DynamicFieldConverterPair> pairStack = new Stack<DynamicFieldConverterPair>();
            DynamicFieldConverterPair rootPair = null;

            foreach (var parentNode in startNode.SelectNodes((node) => { return node.ParentNode; }).Reverse())
            {
                if (rootPair == null)
                {
                    if (parentNode.IsAddressMap)
                    {
                        var fld = new DynamicField(Field.VirtualRootField);
                        fld.DynamicRecordType = new DynamicRecordType(parentNode.FieldConverterPair.GroupConverter.RecordType);
                        var rootConverter = new DynamicGroupConverter(fld.DynamicRecordType);
                        rootPair = new DynamicFieldConverterPair(fld, rootConverter, 0, SevenBitUInt32.Zero);

                        pairStack.Push(rootPair);
                    }
                }
                else
                {
                    AddField(pairStack, parentNode);
                }
            }

            var startPair = AddField(pairStack, startNode);

            if (rootPair == null)
            {
                rootPair = startPair;
            }

            if (startNode != endNode)
            {
                foreach (var node in startNode.SelectNodes((node) => { return node.NextNode; }))
                {
                    if (node.PreviousNode.Key.Depth > node.Key.Depth)
                    {
                        int count = node.PreviousNode.Key.Depth - node.Key.Depth;

                        for (int i = 0; i < count; i++)
                        {
                            // pop
                            var poppedPair = pairStack.Pop();

                            if (startNode == null)
                            {
                                // starting node was not found yet.
                                var currentPair = pairStack.Peek();
                                currentPair.GroupConverter.FieldConverterMap.Remove(poppedPair);

                                currentPair.DynamicField.DynamicRecordType.Fields.Remove(poppedPair.Field);
                            }
                        }
                    }

                    AddField(pairStack, node);

                    if (endNode != null &&
                        node.FieldConverterPair.Field.Name.FullName == endNode.FieldConverterPair.Field.Name.FullName &&
                        node.Key.Equals(endNode.Key))
                    {
                        HandleFillerFields(rootPair, endNode, fillerSize);

                        // exit
                        break;
                    }
                }
            }

            return rootPair.GroupConverter;
        }

        private void HandleFillerFields(DynamicFieldConverterPair rootPair, FieldNode endNode, int fillerSize)
        {
            if (fillerSize > 0 && _schemaProvider != null)
            {
                // match the requested size with dummy filler byte fields (midiNull).
                var fillerType = _schemaProvider.FindDataType(Midi.Device.DeviceConstants.MidiTypesSchemaName, "midiNull");
                var fillerFld = new DynamicField(rootPair.Field.Name.FullName + "DummyFiller", fillerType);

                for (int i = 0; fillerSize > 0; i++)
                {
                    IConverter converter = _converterMgr.GetConverter(fillerType, fillerType);
                    var byteLength = rootPair.GroupConverter.CalculateByteLength(converter, null);

                    // address might not be accurate
                    var pair = new DynamicFieldConverterPair(fillerFld, converter, i, endNode.Address + byteLength);

                    rootPair.GroupConverter.FieldConverterMap.Add(pair);
                    rootPair.DynamicField.DynamicRecordType.Fields.Add(pair.Field);

                    Console.WriteLine("Adding: " + pair.ToString());

                    fillerSize -= byteLength;
                }
            }
        }

        private DynamicFieldConverterPair AddField(Stack<DynamicFieldConverterPair> pairStack, FieldNode node)
        {
            Console.WriteLine("Adding: " + node.ToString());

            var fld = new DynamicField(node.FieldConverterPair.Field);
            IConverter converter = null;
            if (node.IsRecord)
            {
                converter = _converterMgr.GetConverter(node.FieldConverterPair.Field.RecordType, fld.DynamicRecordType);
            }
            else
            {
                converter = _converterMgr.GetConverter(node.FieldConverterPair.Field.DataType, fld.DataType);
            }

            var pair = new DynamicFieldConverterPair(fld, converter, node.InstanceIndex, node.Address);

            if (pairStack.Count > 0)
            {
                var currentPair = pairStack.Peek();
                currentPair.GroupConverter.FieldConverterMap.Add(pair);

                currentPair.DynamicField.DynamicRecordType.Fields.Add(pair.Field);
            }

            if (node.IsRecord)
            {
                pairStack.Push(pair);
            }

            return pair;
        }

        public bool FindAddressRange(
            Field startField, FieldPathKey startKey,
            Field endField, FieldPathKey endKey,
            out SevenBitUInt32 address, out SevenBitUInt32 size)
        {
            var startNode = _binaryMap.Find(startField, startKey);
            var endNode = _binaryMap.Find(endField, endKey);

            if (startNode == null)
            {
                address = SevenBitUInt32.Zero;
                size = SevenBitUInt32.Zero;

                return false;
            }
            else
            {
                // make sure we start at the beginning of the physical address
                startNode = startNode.FirstOfAddress;
            }

            address = startNode.Address;
            size = GetAddressSize(startNode, endNode);
            return true;
        }

        public SevenBitUInt32 GetAddressSize(FieldNode startNode, FieldNode endNode)
        {
            if (endNode != null &&
                startNode != endNode)
            {
                var address = startNode.Address;

                // make sure we end at the last field of the physical address
                endNode = endNode.LastOfAddress;
                var firstOfEndAddress = endNode.FirstOfAddress;

                return (endNode.Address - address) + firstOfEndAddress.DataLength;
            }

            int totalLength = 0;

            //if (startNode.IsRecord)
            //{
            //    // get the total length of all fields (including clones) that share the same record
            //    var node = startNode.NextField;

            //    while (node != null &&
            //        node.IsOfParent(startNode))
            //    {
            //        totalLength += node.DataLength;

            //        node = node.NextField;
            //    }
            //}
            //else
            {
                totalLength = startNode.DataLength;
            }

            return SevenBitUInt32.FromInt32(totalLength);
        }
    }
}