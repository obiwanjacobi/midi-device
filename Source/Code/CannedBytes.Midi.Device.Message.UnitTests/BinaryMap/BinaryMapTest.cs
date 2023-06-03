using System;
using CannedBytes.Midi.Device.Converters;
using CannedBytes.Midi.Device.Schema;
using CannedBytes.Midi.Device.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.Device.Message.UnitTests.BinaryMap
{
    [TestClass]
    [DeploymentItem("BinaryMap/BinaryMapTest.mds")]
    [DeploymentItem("BinaryMap/BinaryMapTest2.mds")]
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
                    Assert.IsTrue(node.Address >= prevNode.Address,
                        String.Format("Field '{0}' has an invalid address {1}", node.FieldConverterPair.Field.Name, node.Address));
                }

                prevNode = node;
                node = node.NextNode;
            }
        }

        [TestMethod]
        public void Initialize_AddressMap_CorectKeys()
        {
            var binaryMap = CreateBinaryMap("BinaryMapTest.mds", "RootMessage");

            Console.WriteLine(binaryMap.ToString());

            AssertAddresses(binaryMap.RootNode);

            Assert.AreEqual("0|2|1|0", binaryMap.LastNode.Key.ToString());
        }

        [TestMethod]
        public void Initialize_AddressMap_CorectAddressses()
        {
            var binaryMap = CreateBinaryMap("BinaryMapTest2.mds", "RootMessage");

            Console.WriteLine(binaryMap.ToString());

            AssertAddresses(binaryMap.RootNode);
        }
    }
}