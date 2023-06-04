using CannedBytes.Midi.Device.UnitTests.Stubs;
using Xunit;

namespace CannedBytes.Midi.Device.UnitTests.LittleEndianTest
{
    
    //[DeploymentItem("BigEndianTest/BigEndianTestSchema.mds")]
    //[DeploymentItem("BigEndianTest/BigEndianTestStream.bin")]
    public class BigEndianTest
    {
        public static readonly string TestSchemaFileName = "BigEndianTestSchema.mds";
        public static readonly string TestStreamFileName = "BigEndianTestStream.bin";

        public const string FieldName = "http://schemas.cannedbytes.com/MidiDeviceSchema/UnitTests/BigEndianTestSchema.mds:SwappedChars";
        public const string FieldNameIndexed = "http://schemas.cannedbytes.com/MidiDeviceSchema/UnitTests/BigEndianTestSchema.mds:SwappedChars[0]";

        [Fact]
        public void Read_BigEndianWidth2_StringCharsSwapped()
        {
            var writer = new DictionaryBasedLogicalStub();

            DeviceHelper.ReadLogical(TestSchemaFileName, TestStreamFileName, "bigEndianTest", writer);

            Assert.True(writer.FieldValues.ContainsKey(FieldNameIndexed));

            var actual = writer.FieldValues[FieldNameIndexed];
            Assert.Equal("cAuots", actual);
        }

        [Fact]
        public void Read_BigEndianFieldWidth3_StringCharsSwapped()
        {
            var writer = new DictionaryBasedLogicalStub();

            DeviceHelper.ReadLogical(TestSchemaFileName, TestStreamFileName, "bigEndianFieldTest", writer);

            Assert.True(writer.FieldValues.ContainsKey(FieldNameIndexed));

            var actual = writer.FieldValues[FieldNameIndexed];
            Assert.Equal("ocAtsu", actual);
        }

        [Fact]
        public void Write_BigEndianWidth2_StringCharsSwapped()
        {
            var stub = new DictionaryBasedLogicalStub();
            stub.AddValue(FieldName, 0, "Acoust");

            var ctx = DeviceHelper.WritePhysical(TestSchemaFileName, "bigEndianTest", stub);
            var stream = ctx.PhysicalStream;
            stream.Position = 1; // skip F0

            Assert.Equal(8, stream.Length);

            var reader = new MidiBinaryStreamReader(stream);

            var value = reader.ReadString(6);

            Assert.Equal("cAuots", value);
        }
    }
}