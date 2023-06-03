using System;
using System.Diagnostics;
using CannedBytes.Midi.Device.Roland.D110;
using CannedBytes.Midi.Device.Schema;
using CannedBytes.Midi.Device.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.Device.Message.UnitTests.D110Test
{
    [TestClass]
    public class SchemaTest
    {
        public const string SchemaFileName = "CannedBytes.Midi.Device.Roland.D110/Roland D-110.mds";

        private MessageTester CreateMessageTester()
        {
            var messageTester = new MessageTester();
            messageTester.EnableTrace = Debugger.IsAttached;
            messageTester.CompositionContainer = DeviceHelper.CreateContainer();
            messageTester.DeviceProvider = messageTester.CompositionContainer.GetExportedValue<Roland.D110.DeviceProvider>();
            messageTester.SetCurrentMessage("DT1");

            Assert.IsNotNull(messageTester.CurrentPair);
            Assert.IsNotNull(messageTester.DeviceProvider);

            return messageTester;
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
        public void DeviceProvider_Construct_NoErrors()
        {
            var provider = new DeviceProvider();

            Assert.AreEqual("Roland", provider.Manufacturer);
            Assert.AreEqual("D-110", provider.ModelName);
        }

        [TestMethod]
        public void LoadSchema_SchemaValidation()
        {
            DeviceSchema schema = DeviceHelper.OpenDeviceSchema(SchemaFileName);

            Assert.AreEqual(2, schema.RootRecordTypes.Count);
        }

        [TestMethod]
        public void LoadBinaryMap_Validation()
        {
            var messageTester = CreateMessageTester();
            messageTester.SetCurrentMessage("DT1");

            Console.WriteLine(messageTester.CurrentBinaryMap.ToString());

            AssertAddresses(messageTester.CurrentBinaryMap.RootNode);
        }
    }
}