using System.IO;
using CannedBytes.Midi.Device.IntegrationTests;
using CannedBytes.Midi.Device.IntegrationTests.Stubs;
using Xunit;

namespace CannedBytes.Midi.Device.UnitTests.LittleEndianTest
{
    public class BigEndianTest
    {
        public const string Folder = "BigEndianTest";
        public const string TestSchemaFileName = "BigEndianTestSchema.mds";
        public const string TestStreamFileName = "BigEndianTestStream.bin";

        //public const string StringFieldName = "http://schemas.cannedbytes.com/MidiDeviceSchema/UnitTests/BigEndianTestSchema.mds:SwappedChars";
        public const string StringFieldNameIndexed = "http://schemas.cannedbytes.com/MidiDeviceSchema/UnitTests/BigEndianTestSchema.mds:SwappedChars[0]";
        public const string Int1FieldNameIndexed = "http://schemas.cannedbytes.com/MidiDeviceSchema/UnitTests/BigEndianTestSchema.mds:Int1[0]";
        public const string Int2FieldNameIndexed = "http://schemas.cannedbytes.com/MidiDeviceSchema/UnitTests/BigEndianTestSchema.mds:Int2[0]";
        public const string Int3FieldNameIndexed = "http://schemas.cannedbytes.com/MidiDeviceSchema/UnitTests/BigEndianTestSchema.mds:Int3[0]";

        private static void ReadLogical(string virtualRootName, IMidiLogicalWriter writer)
        {
            var serviceProvider = ServiceHelper.CreateServices();
            DeviceHelper.ToLogical(serviceProvider,
                Path.Combine(Folder, TestSchemaFileName),
                Path.Combine(Folder, TestStreamFileName), virtualRootName, writer);
        }

        [Fact]
        public void Read_BigEndianWidth2_StringCharsSwapped()
        {
            var writer = new DictionaryBasedLogicalStub();

            ReadLogical("bigEndianTest", writer);

            Assert.True(writer.Contains(StringFieldNameIndexed));

            var actual = writer[StringFieldNameIndexed].Value;
            Assert.Equal("cAuots", actual);
        }

        [Fact]
        public void Read_BigEndianFieldWidth3_StringCharsSwapped()
        {
            var writer = new DictionaryBasedLogicalStub();

            ReadLogical("bigEndianFieldTest", writer);

            Assert.True(writer.Contains(StringFieldNameIndexed));

            var actual = writer[StringFieldNameIndexed].Value;
            Assert.Equal("ocAtsu", actual);
        }

        // first implement the UnsignedConverter.
        //[Fact]
        public void Read_BigEndianWidth2_Unsigned()
        {
            var writer = new DictionaryBasedLogicalStub();

            ReadLogical("bigEndianIntTest", writer);

            Assert.True(writer.Contains(Int1FieldNameIndexed));
            Assert.True(writer.Contains(Int2FieldNameIndexed));
            Assert.True(writer.Contains(Int2FieldNameIndexed));

            var actual = writer[Int1FieldNameIndexed].Value;
            Assert.Equal(0x6341, actual);

            actual = writer[Int2FieldNameIndexed].Value;
            Assert.Equal(0x75F6, actual);

            actual = writer[Int3FieldNameIndexed].Value;
            Assert.Equal(0x7473, actual);
        }

        //[Fact]
        //public void Write_BigEndianWidth2_StringCharsSwapped()
        //{
        //    var stub = new DictionaryBasedLogicalStub();
        //    stub.Add(StringFieldName, "Acoust");

        //    var ctx = DeviceHelper.WritePhysical(TestSchemaFileName, "bigEndianTest", stub);
        //    var stream = ctx.PhysicalStream;
        //    stream.Position = 1; // skip F0

        //    Assert.Equal(8, stream.Length);

        //    var reader = new MidiBinaryStreamReader(stream);

        //    var value = reader.ReadString(6);

        //    Assert.Equal("cAuots", value);
        //}
    }
}