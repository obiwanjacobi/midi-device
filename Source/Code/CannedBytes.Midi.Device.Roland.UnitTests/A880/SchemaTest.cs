using System;
using System.Diagnostics;
using CannedBytes.Midi.Device.Roland.A880;
using CannedBytes.Midi.Device.Schema;
using CannedBytes.Midi.Device.UnitTests;
using Xunit;

namespace CannedBytes.Midi.Device.Message.UnitTests.A880Test
{
    
    public class SchemaTest
    {
        public const string SchemaFileName = "CannedBytes.Midi.Device.Roland.A880/Roland A-880.mds";

        private MessageTester CreateMessageTester()
        {
            var messageTester = new MessageTester();
            messageTester.EnableTrace = Debugger.IsAttached;
            messageTester.CompositionContainer = DeviceHelper.CreateContainer();
            messageTester.DeviceProvider = messageTester.CompositionContainer.GetExportedValue<Roland.A880.DeviceProvider>();
            messageTester.SetCurrentMessage("DT1");

            Assert.NotNull(messageTester.CurrentPair);
            Assert.NotNull(messageTester.DeviceProvider);

            return messageTester;
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
        public void DeviceProvider_Construct_NoErrors()
        {
            var provider = new DeviceProvider();

            Assert.Equal("Roland", provider.Manufacturer);
            Assert.Equal("A-880", provider.ModelName);
        }

        [Fact]
        public void LoadSchema_SchemaValidation()
        {
            DeviceSchema schema = DeviceHelper.OpenDeviceSchema(SchemaFileName);

            Assert.Equal(2, schema.RootRecordTypes.Count);
        }

        [Fact]
        public void LoadBinaryMap_Validation()
        {
            var messageTester = CreateMessageTester();
            messageTester.SetCurrentMessage("DT1");

            Console.WriteLine(messageTester.CurrentBinaryMap.ToString());

            AssertAddresses(messageTester.CurrentBinaryMap.RootNode);
        }
    }
}