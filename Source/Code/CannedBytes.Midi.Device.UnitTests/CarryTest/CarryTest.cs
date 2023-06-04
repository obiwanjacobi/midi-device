using CannedBytes.Midi.Device.UnitTests.Stubs;
using Xunit;

namespace CannedBytes.Midi.Device.UnitTests.CarryTest
{
    
    //[DeploymentItem("CarryTest/CarryTestSchema.mds")]
    //[DeploymentItem("CarryTest/CarryTestStream.bin")]
    public class CarryTest
    {
        public const string TestSchemaFileName = "CarryTestSchema.mds";
        public const string TestStreamFileName = "CarryTestStream.bin";

        public const string FieldNamespace = "http://schemas.cannedbytes.com/MidiDeviceSchema/UnitTests/CarryTestSchema.mds:";

        [Fact]
        public void Read_SchemaWithCarry_ByteAndWordValues()
        {
            var writer = new DictionaryBasedLogicalStub();

            DeviceHelper.ReadLogical(TestSchemaFileName, TestStreamFileName, "carryTest", writer);

            Assert.Equal(9, writer.FieldValues.Count);
            Assert.Equal((byte)0x21, writer.FieldValues[FieldNamespace + "loByte[0]"]);
            Assert.Equal((byte)0x0F, writer.FieldValues[FieldNamespace + "hiByte[0]"]);

            Assert.Equal((byte)0x0C, writer.FieldValues[FieldNamespace + "loPart[0]"]);
            Assert.Equal((byte)0x0D, writer.FieldValues[FieldNamespace + "midPart[0]"]);
            Assert.Equal((byte)0x00, writer.FieldValues[FieldNamespace + "hiPart[0]"]);

            Assert.Equal((byte)0x74, writer.FieldValues[FieldNamespace + "firstLo[0]"]);
            Assert.Equal((byte)0x03, writer.FieldValues[FieldNamespace + "secondLo[0]"]);

            Assert.Equal((byte)0x0E, writer.FieldValues[FieldNamespace + "firstHi[0]"]);
            Assert.Equal((byte)0x01, writer.FieldValues[FieldNamespace + "secondHi[0]"]);
        }

        [Fact]
        public void Write_SchemaWithCarry_ByteAndWordValues()
        {
            var reader = new DictionaryBasedLogicalStub();
            reader.AddValue(FieldNamespace + "loByte", 0, 0x21);
            reader.AddValue(FieldNamespace + "hiByte", 0, 0x0F);

            reader.AddValue(FieldNamespace + "loPart", 0, 0x0C);
            reader.AddValue(FieldNamespace + "midPart", 0, 0x0D);
            reader.AddValue(FieldNamespace + "hiPart", 0, 0x06);

            reader.AddValue(FieldNamespace + "firstLo", 0, 0x74);
            reader.AddValue(FieldNamespace + "secondLo", 0, 0x03);

            reader.AddValue(FieldNamespace + "firstHi", 0, 0x0E);
            reader.AddValue(FieldNamespace + "secondHi", 0, 0x01);

            var ctx = DeviceHelper.WritePhysical(TestSchemaFileName, "carryTest", reader);
            var stream = ctx.PhysicalStream;
            stream.Position = 0;

            // 21-F0-4C-63-74-03-E0-00-10

            Assert.Equal(9 + 2, stream.Length);
            Assert.Equal(0xF0, stream.ReadByte());

            Assert.Equal(0x21, stream.ReadByte());
            Assert.Equal(0xF0, stream.ReadByte());

            Assert.Equal(0x4C, stream.ReadByte());
            Assert.Equal(0x63, stream.ReadByte());

            Assert.Equal(0x74, stream.ReadByte());
            Assert.Equal(0x03, stream.ReadByte());

            Assert.Equal(0xE0, stream.ReadByte());

            Assert.Equal(0x00, stream.ReadByte());
            Assert.Equal(0x10, stream.ReadByte());

            Assert.Equal(0xF7, stream.ReadByte());
        }
    }
}