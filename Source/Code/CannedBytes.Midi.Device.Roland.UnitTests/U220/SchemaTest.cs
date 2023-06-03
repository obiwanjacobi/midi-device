using System;
using System.Diagnostics;
using CannedBytes.Midi.Device.Roland.U220;
using CannedBytes.Midi.Device.Schema;
using CannedBytes.Midi.Device.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.Device.Message.UnitTests.U220Test
{
    [TestClass]
    [DeploymentItem("U220/BulkDumpStream.bin")]
    [DeploymentItem("U220/PatchMap1Stream.bin")]
    [DeploymentItem("U220/SetupParamStream.bin")]
    [DeploymentItem("U220/TempAllStream.bin")]
    [DeploymentItem("U220/Timbre1Stream.bin")]
    [DeploymentItem("U220/Rythm1Stream.bin")]
    [DeploymentItem("U220/Timbre6Stream.bin")]
    [DeploymentItem("U220/TimbreMap1Stream.bin")]
    [DeploymentItem("U220/PatchTestStream.bin")]
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

            Assert.IsNotNull(messageTester.CurrentPair);
            Assert.IsNotNull(messageTester.DeviceProvider);

            return messageTester;
        }

        [TestMethod]
        public void DeviceProvider_Construct_NoErrors()
        {
            var provider = new DeviceProvider();

            Assert.AreEqual("Roland", provider.Manufacturer);
            Assert.AreEqual("U-220", provider.ModelName);
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
        }

        [TestMethod]
        public void Roundtrip_Patch_LogicalDataEquals()
        {
            var msgTester = CreateMessageTester();

            msgTester.ReadWriteReadAllComapareLogicalData("PatchTestStream.bin");
        }

        [TestMethod]
        public void Roundtrip_SetupParam_LogicalDataEquals()
        {
            var msgTester = CreateMessageTester();

            msgTester.ReadWriteReadAllComapareLogicalData("SetupParamStream.bin");
        }

        //[TestMethod]
        public void Roundtrip_BulkDump_LogicalDataEquals()
        {
            var msgTester = CreateMessageTester();

            msgTester.ReadWriteReadAllComapareLogicalData("BulkDumpStream.bin");
        }

        //[TestMethod]
        public void Roundtrip_PatchMap_LogicalDataEquals()
        {
            var msgTester = CreateMessageTester();

            msgTester.ReadWriteReadAllComapareLogicalData("PatchMap1Stream.bin");
        }

        //[TestMethod]
        public void Roundtrip_Rythm_LogicalDataEquals()
        {
            var msgTester = CreateMessageTester();

            msgTester.ReadWriteReadAllComapareLogicalData("Rythm1Stream.bin");
        }

        //[TestMethod]
        public void Roundtrip_TempAll_LogicalDataEquals()
        {
            var msgTester = CreateMessageTester();

            msgTester.ReadWriteReadAllComapareLogicalData("TempAllStream.bin");
        }

        [TestMethod]
        public void Roundtrip_Timbre1_LogicalDataEquals()
        {
            var msgTester = CreateMessageTester();

            msgTester.ReadWriteReadAllComapareLogicalData("Timbre1Stream.bin");
        }

        [TestMethod]
        public void Roundtrip_Timbre6_LogicalDataEquals()
        {
            var msgTester = CreateMessageTester();

            msgTester.ReadWriteReadAllComapareLogicalData("Timbre6Stream.bin");
        }

        //[TestMethod]
        public void Roundtrip_TimbreMap1_LogicalDataEquals()
        {
            var msgTester = CreateMessageTester();

            msgTester.ReadWriteReadAllComapareLogicalData("TimbreMap1Stream.bin");
        }
    }
}