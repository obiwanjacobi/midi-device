using System.IO;
using CannedBytes.Midi.Device.UnitTests.Stubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.Device.UnitTests.SplitNibbleTest
{
    [TestClass]
    [DeploymentItem("SplitNibbleTest/SplitNibbleTestSchema.mds")]
    [DeploymentItem("SplitNibbleTest/SplitNibbleTestStream.bin")]
    public class SplitNibbleTest
    {
        public const string TestSchemaFileName = "SplitNibbleTestSchema.mds";
        public const string TestStreamFileName = "SplitNibbleTestStream.bin";

        public const string FieldName = "http://schemas.cannedbytes.com/MidiDeviceSchema/UnitTests/SplitNibbleTestSchema.mds:Field1[0|0]";

        [TestMethod]
        public void Read_SplitNibbleStream_LogicValues()
        {
            var writer = new DictionaryBasedLogicalStub();

            DeviceHelper.ReadLogical(TestSchemaFileName, TestStreamFileName, "splitNibbleTest", writer);

            Assert.IsTrue(writer.FieldValues.ContainsKey(FieldName));

            var actual = writer.FieldValues[FieldName];
            Assert.AreEqual("Acoust Piano", actual);
        }

        [TestMethod]
        public void Write_LogicalValues_SplitNibbleStream()
        {
            var reader = new DictionaryBasedLogicalStub();

            reader.AddValue(FieldName, -1, "Acoust Piano");

            var ctx = DeviceHelper.WritePhysical(TestSchemaFileName, "splitNibbleTest", reader);
            var stream = ctx.PhysicalStream;

            Assert.AreEqual(24 + 2, stream.Length);

            using (var fileStream = File.OpenRead(TestStreamFileName))
            {
                long pos = -1;
                Assert.IsTrue(DeviceHelper.CompareStreams(stream, fileStream, out pos));
            }
        }
    }
}