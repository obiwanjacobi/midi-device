using CannedBytes.Midi.Device.UnitTests;
using CannedBytes.Midi.Device.UnitTests.Stubs;
using Xunit;

namespace CannedBytes.Midi.Device.Roland.UnitTests.AddressBETest
{
    
    //[DeploymentItem("AddressBETest/AddressTestStream.bin")]
    public class RolandAddressBETest
    {
        public const string SchemaFileName = "CannedBytes.Midi.Device.Roland/Roland.mds";
        public const string StreamFileName = "AddressTestStream.bin";

        public const string AddressField = "http://schemas.cannedbytes.com/midi-device-schema/Roland/10:Address[0]";
        public const string SizeField = "http://schemas.cannedbytes.com/midi-device-schema/Roland/10:Size[0]";

        [Fact]
        public void Read_AddressBE3_BigEndian()
        {
            var writer = new DictionaryBasedLogicalStub();

            DeviceHelper.ReadLogical(SchemaFileName, StreamFileName, "AddressBE3", writer);

            Assert.Equal(0x010203L, writer.FieldValues[AddressField]);
        }

        [Fact]
        public void Read_AddressSizeBE3_BigEndian()
        {
            var writer = new DictionaryBasedLogicalStub();

            DeviceHelper.ReadLogical(SchemaFileName, StreamFileName, "AddressSizeBE3", writer);

            Assert.Equal(0x010203L, writer.FieldValues[AddressField]);
            Assert.Equal(0x040506L, writer.FieldValues[SizeField]);
        }
    }
}