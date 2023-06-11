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

        public const string FieldName = "http://schemas.cannedbytes.com/MidiDeviceSchema/UnitTests/BigEndianTestSchema.mds:SwappedChars";
        public const string FieldNameIndexed = "http://schemas.cannedbytes.com/MidiDeviceSchema/UnitTests/BigEndianTestSchema.mds:SwappedChars[0]";

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

            Assert.True(writer.Contains(FieldNameIndexed));

            var actual = writer[FieldNameIndexed].Value;
            Assert.Equal("cAuots", actual);
        }

        [Fact]
        public void Read_BigEndianFieldWidth3_StringCharsSwapped()
        {
            var writer = new DictionaryBasedLogicalStub();

            ReadLogical("bigEndianFieldTest", writer);

            Assert.True(writer.Contains(FieldNameIndexed));

            var actual = writer[FieldNameIndexed].Value;
            Assert.Equal("ocAtsu", actual);
        }

        //[Fact]
        //public void Write_BigEndianWidth2_StringCharsSwapped()
        //{
        //    var stub = new DictionaryBasedLogicalStub();
        //    stub.Add(FieldName, "Acoust");

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