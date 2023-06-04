using System;
using System.Diagnostics;
using CannedBytes.Midi.Device.Roland.U220;
using CannedBytes.Midi.Device.Schema;
using CannedBytes.Midi.Device.UnitTests;
using Xunit;

namespace CannedBytes.Midi.Device.Message.UnitTests.U220Test
{
    
    //[DeploymentItem("U220/BulkDumpStream.bin")]
    //[DeploymentItem("U220/PatchMap1Stream.bin")]
    //[DeploymentItem("U220/SetupParamStream.bin")]
    //[DeploymentItem("U220/TempAllStream.bin")]
    //[DeploymentItem("U220/Timbre1Stream.bin")]
    //[DeploymentItem("U220/Rythm1Stream.bin")]
    //[DeploymentItem("U220/Timbre6Stream.bin")]
    //[DeploymentItem("U220/TimbreMap1Stream.bin")]
    //[DeploymentItem("U220/PatchTestStream.bin")]
    public class SchemaTest
    {
        public const string SchemaFileName = "CannedBytes.Midi.Device.Roland.U220/Roland U-220.mds";

        private MessageTester CreateMessageTester()
        {
            var messageTester = new MessageTester();
            messageTester.EnableTrace = Debugger.IsAttached;
            messageTester.CompositionContainer = DeviceHelper.CreateContainer();
            messageTester.DeviceProvider = messageTester.CompositionContainer.GetExportedValue<Roland.U220.DeviceProvider>();
            messageTester.SetCurrentMessage("DT1");

            Assert.NotNull(messageTester.CurrentPair);
            Assert.NotNull(messageTester.DeviceProvider);

            return messageTester;
        }

        [Fact]
        public void DeviceProvider_Construct_NoErrors()
        {
            var provider = new DeviceProvider();

            Assert.Equal("Roland", provider.Manufacturer);
            Assert.Equal("U-220", provider.ModelName);
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
        }

        [Fact]
        public void Roundtrip_Patch_LogicalDataEquals()
        {
            var msgTester = CreateMessageTester();

            msgTester.ReadWriteReadAllCompareLogicalData("PatchTestStream.bin");
        }

        [Fact]
        public void Roundtrip_SetupParam_LogicalDataEquals()
        {
            var msgTester = CreateMessageTester();

            msgTester.ReadWriteReadAllCompareLogicalData("SetupParamStream.bin");
        }

        //[Fact]
        public void Roundtrip_BulkDump_LogicalDataEquals()
        {
            var msgTester = CreateMessageTester();

            msgTester.ReadWriteReadAllCompareLogicalData("BulkDumpStream.bin");
        }

        //[Fact]
        public void Roundtrip_PatchMap_LogicalDataEquals()
        {
            var msgTester = CreateMessageTester();

            msgTester.ReadWriteReadAllCompareLogicalData("PatchMap1Stream.bin");
        }

        //[Fact]
        public void Roundtrip_Rythm_LogicalDataEquals()
        {
            var msgTester = CreateMessageTester();

            msgTester.ReadWriteReadAllCompareLogicalData("Rythm1Stream.bin");
        }

        //[Fact]
        public void Roundtrip_TempAll_LogicalDataEquals()
        {
            var msgTester = CreateMessageTester();

            msgTester.ReadWriteReadAllCompareLogicalData("TempAllStream.bin");
        }

        [Fact]
        public void Roundtrip_Timbre1_LogicalDataEquals()
        {
            var msgTester = CreateMessageTester();

            msgTester.ReadWriteReadAllCompareLogicalData("Timbre1Stream.bin");
        }

        [Fact]
        public void Roundtrip_Timbre6_LogicalDataEquals()
        {
            var msgTester = CreateMessageTester();

            msgTester.ReadWriteReadAllCompareLogicalData("Timbre6Stream.bin");
        }

        //[Fact]
        public void Roundtrip_TimbreMap1_LogicalDataEquals()
        {
            var msgTester = CreateMessageTester();

            msgTester.ReadWriteReadAllCompareLogicalData("TimbreMap1Stream.bin");
        }
    }
}