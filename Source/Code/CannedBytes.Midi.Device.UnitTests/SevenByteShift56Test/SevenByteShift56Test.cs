using System.IO;
using CannedBytes.Midi.Device.UnitTests.Stubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.Device.UnitTests.SevenByteShift56Test
{
    [TestClass]
    [DeploymentItem("SevenByteShift56Test/SevenByteShift56TestSchema.mds")]
    [DeploymentItem("SevenByteShift56Test/SevenByteShift56TestStream.bin")]
    public class SevenByteShift56Test
    {
        public const string TestSchemaFileName = "SevenByteShift56TestSchema.mds";
        public const string TestStreamFileName = "SevenByteShift56TestStream.bin";

        public const string Field1Name = "http://schemas.cannedbytes.com/MidiDeviceSchema/UnitTests/SevenByteShift56TestSchema.mds:Field1[0|0]";
        public const string Field2Name = "http://schemas.cannedbytes.com/MidiDeviceSchema/UnitTests/SevenByteShift56TestSchema.mds:Field2[0|0]";

        [TestMethod]
        public void Read_SevenByteShift56Stream_LogicValues()
        {
            var writer = new DictionaryBasedLogicalStub();

            DeviceHelper.ReadLogical(TestSchemaFileName, TestStreamFileName, "sevenByteTest", writer);

            Assert.IsTrue(writer.FieldValues.ContainsKey(Field1Name));

            var actual = writer.FieldValues[Field1Name];
            Assert.AreEqual("012345", actual);
        }

        [TestMethod]
        public void Write_LogicalValues_SevenByteShift56Stream()
        {
            var reader = new DictionaryBasedLogicalStub();

            reader.AddValue(Field1Name, -1, "012345");
            reader.AddValue(Field2Name, -1, 0x36);

            var ctx = DeviceHelper.WritePhysical(TestSchemaFileName, "sevenByteTest", reader);
            var stream = ctx.PhysicalStream;

            Assert.AreEqual(8 + 2, stream.Length);

            using (var fileStream = File.OpenRead(TestStreamFileName))
            {
                long pos = -1;
                Assert.IsTrue(DeviceHelper.CompareStreams(stream, fileStream, out pos));
            }
        }
    }
}