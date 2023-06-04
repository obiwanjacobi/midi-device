using System;
using CannedBytes.Midi.Device.Converters;
using CannedBytes.Midi.Device.Schema;
using CannedBytes.Midi.Device.UnitTests;
using FluentAssertions;
using Xunit;

namespace CannedBytes.Midi.Device.Message.UnitTests.BinaryMap
{
    
    //[DeploymentItem("BinaryMap/BinaryMapTest.mds")]
    //[DeploymentItem("BinaryMap/BinaryMapTest2.mds")]
    public class BinaryMapTest
    {
        private MidiDeviceBinaryMap CreateBinaryMap(string schemaFileName, string messageName)
        {
            DeviceSchema schema = DeviceHelper.OpenDeviceSchema(schemaFileName);
            RecordType message = schema.RootRecordTypes.Find(messageName);

            var container = DeviceHelper.CreateContainer();
            ConverterManager converterManager = new ConverterManager();
            converterManager.InitializeFrom(container);

            GroupConverter baseConverter = converterManager.GetConverter(message);

            MidiDeviceBinaryMap binaryMap = new MidiDeviceBinaryMap(baseConverter);

            return binaryMap;
        }

        private static void AssertAddresses(FieldNode node)
        {
            FieldNode prevNode = null;

            while (node != null)
            {
                if (prevNode != null)
                {
                    Assert.True(node.Address >= prevNode.Address,
                        String.Format("Field '{0}' has an invalid address {1}", node.FieldConverterPair.Field.Name, node.Address));
                }

                prevNode = node;
                node = node.NextNode;
            }
        }

        [Fact]
        public void Initialize_AddressMap_CorrectKeys()
        {
            var binaryMap = CreateBinaryMap("BinaryMapTest.mds", "RootMessage");

            Console.WriteLine(binaryMap.ToString());

            AssertAddresses(binaryMap.RootNode);

            binaryMap.LastNode.Key.ToString().Should().Be("0|2|1|0");
        }

        [Fact]
        public void Initialize_AddressMap_CorrectAddresses()
        {
            var binaryMap = CreateBinaryMap("BinaryMapTest2.mds", "RootMessage");

            Console.WriteLine(binaryMap.ToString());

            AssertAddresses(binaryMap.RootNode);
        }
    }
}