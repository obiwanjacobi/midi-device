using CannedBytes.Midi.Device.IntegrationTests.Stubs;
using Xunit;
using FluentAssertions;
using Xunit.Abstractions;

namespace CannedBytes.Midi.Device.IntegrationTests.CarryTest
{
    
    //[DeploymentItem(Folder + TestSchemaFileName)]
    //[DeploymentItem(Folder + TestStreamFileName)]
    public class CarryTest
    {
        public const string Folder = "CarryTest/";
        public const string TestSchemaFileName = "CarryTestSchema.mds";
        public const string TestStreamFileName = "CarryTestStream.bin";

        public const string TestNamespace = 
            "http://schemas.cannedbytes.com/MidiDeviceSchema/IntegrationTests/CarryTestSchema";
        
        private readonly ITestOutputHelper _output;

        public CarryTest(ITestOutputHelper output)
            => _output = output;

        [Fact]
        public void Read_SchemaWithCarry_ByteAndWordValues()
        {
            var compositionCtx = CompositionHelper.CreateCompositionContext();
            var writer = new DictionaryBasedLogicalStub();

            var ctx = DeviceHelper.ToLogical(compositionCtx, 
                TestSchemaFileName, TestStreamFileName, "carryTest", writer);

            writer.Count.Should().Be(9);
            
            writer[0].Value.Should().Be((byte)0x21);
            writer[0].Field.Name.SchemaName.Should().Be(TestNamespace);
            writer[0].Field.Name.Name.Should().Be("loByte");

            writer[1].Value.Should().Be((byte)0x0F);
            writer[1].Field.Name.SchemaName.Should().Be(TestNamespace);
            writer[1].Field.Name.Name.Should().Be("hiByte");

            writer[2].Value.Should().Be((byte)0x0C);
            writer[2].Field.Name.SchemaName.Should().Be(TestNamespace);
            writer[2].Field.Name.Name.Should().Be("loPart");

            writer[3].Value.Should().Be((byte)0x0D);
            writer[3].Field.Name.SchemaName.Should().Be(TestNamespace);
            writer[3].Field.Name.Name.Should().Be("midPart");

            writer[4].Value.Should().Be((byte)0x00);
            writer[4].Field.Name.SchemaName.Should().Be(TestNamespace);
            writer[4].Field.Name.Name.Should().Be("hiPart");

            writer[5].Value.Should().Be((byte)0x74);
            writer[5].Field.Name.SchemaName.Should().Be(TestNamespace);
            writer[5].Field.Name.Name.Should().Be("firstLo");

            writer[6].Value.Should().Be((byte)0x03);
            writer[6].Field.Name.SchemaName.Should().Be(TestNamespace);
            writer[6].Field.Name.Name.Should().Be("secondLo");

            writer[7].Value.Should().Be((byte)0x0E);
            writer[7].Field.Name.SchemaName.Should().Be(TestNamespace);
            writer[7].Field.Name.Name.Should().Be("firstHi");

            writer[8].Value.Should().Be((byte)0x01);
            writer[8].Field.Name.SchemaName.Should().Be(TestNamespace);
            writer[8].Field.Name.Name.Should().Be("secondHi");

            ctx.Should().NotBeNull();
            _output.WriteLine(ctx.RecordManager.ToString());
        }

        /*
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

            Assert.AreEqual(9 + 2, stream.Length);
            Assert.AreEqual(0xF0, stream.ReadByte());

            Assert.AreEqual(0x21, stream.ReadByte());
            Assert.AreEqual(0xF0, stream.ReadByte());

            Assert.AreEqual(0x4C, stream.ReadByte());
            Assert.AreEqual(0x63, stream.ReadByte());

            Assert.AreEqual(0x74, stream.ReadByte());
            Assert.AreEqual(0x03, stream.ReadByte());

            Assert.AreEqual(0xE0, stream.ReadByte());

            Assert.AreEqual(0x00, stream.ReadByte());
            Assert.AreEqual(0x10, stream.ReadByte());

            Assert.AreEqual(0xF7, stream.ReadByte());
        }
        */
    }
}