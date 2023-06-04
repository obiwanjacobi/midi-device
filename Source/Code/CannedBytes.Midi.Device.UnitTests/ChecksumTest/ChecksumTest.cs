using CannedBytes.Midi.Device.UnitTests.Stubs;
using Xunit;

namespace CannedBytes.Midi.Device.UnitTests.ChecksumTest
{
    
    //[DeploymentItem("ChecksumTest/ChecksumTestSchema.mds")]
    //[DeploymentItem("ChecksumTest/ChecksumTestStream.bin")]
    public class ChecksumTest
    {
        public static readonly string ChecksumSchemaFileName = "ChecksumTestSchema.mds";
        public static readonly string ChecksumTestStreamFileName = "ChecksumTestStream.bin";

        [Fact]
        public void ChecksumReadTest()
        {
            DeviceHelper.ReadLogical(ChecksumSchemaFileName, ChecksumTestStreamFileName, "checksumTest", null);
        }

        [Fact]
        public void ChecksumWriteTest()
        {
            var reader = new DictionaryBasedLogicalStub();
            // fill reader fields
            reader.AddValue<byte>("SysExData", 0, 0x41);
            reader.AddValue<byte>("ChecksumData1", 0, 0x01);
            reader.AddValue<byte>("ChecksumData2", 0, 0x02);
            reader.AddValue<byte>("ChecksumData3", 0, 0x04);
            reader.AddValue<byte>("ChecksumData4", 0, 0x08);

            DeviceHelper.WritePhysical(ChecksumSchemaFileName, "checksumTest", reader);
        }
    }
}