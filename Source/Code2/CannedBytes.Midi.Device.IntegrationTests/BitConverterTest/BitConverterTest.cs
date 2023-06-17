using System.IO;
using CannedBytes.Midi.Device.IntegrationTests.Stubs;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CannedBytes.Midi.Device.IntegrationTests.CarryTest;

public class BitConverterTest
{
    public const string Folder = "BitConverterTest/";
    public const string TestSchemaFileName = "BitConverterTestSchema.mds";
    public const string TestStreamFileName = "BitConverterTestStream.bin";

    public const string TestNamespace =
        "http://schemas.cannedbytes.com/MidiDeviceSchema/IntegrationTests/CarryTestSchema";

    private readonly ITestOutputHelper _output;

    public BitConverterTest(ITestOutputHelper output)
        => _output = output;

    [Fact]
    public void Read_SchemaWithCarry_ByteAndWordValues()
    {
        DictionaryBasedLogicalStub writer = new();

        var serviceProvider = ServiceHelper.CreateServices();
        var ctx = DeviceHelper.ToLogical(serviceProvider,
            Path.Combine(Folder, TestSchemaFileName),
            Path.Combine(Folder, TestStreamFileName), "RangeDataTypeTest", writer);

        ctx.Should().NotBeNull();
        _output.WriteLine(ctx.LogManager.ToString());

        AssertWriter(writer);
    }

    [Fact]
    public void ReadBits_SchemaWithRange_ByteAndWordValues()
    {
        DictionaryBasedLogicalStub writer = new();

        var serviceProvider = ServiceHelper.CreateServices();
        var ctx = DeviceHelper.ToLogical(serviceProvider,
            Path.Combine(Folder, TestSchemaFileName),
            Path.Combine(Folder, TestStreamFileName), "RangeFieldTest", writer);
        
        ctx.Should().NotBeNull();
        _output.WriteLine(ctx.LogManager.ToString());

        AssertWriter(writer);
    }

    private static void AssertWriter(DictionaryBasedLogicalStub writer)
    {
        writer.Count.Should().Be(9);

        // 71 32 4C 0B 74 23 6E 5A 17 7F

        // 71 => (0:6) => 71
        writer[0].Value.Should().Be((byte)0x71);
        writer[0].Field.Name.SchemaName.Should().Be(TestNamespace);
        writer[0].Field.Name.Name.Should().Be("loByte");

        // 32 => (12:15) => 03
        writer[1].Value.Should().Be((byte)0x03);
        writer[1].Field.Name.SchemaName.Should().Be(TestNamespace);
        writer[1].Field.Name.Name.Should().Be("hiByte");

        // 4C => (0:4) => 0C
        writer[2].Value.Should().Be((byte)0x0C);
        writer[2].Field.Name.SchemaName.Should().Be(TestNamespace);
        writer[2].Field.Name.Name.Should().Be("loPart");

        // 4C-0B => (6:10) => 14
        // (01)00_1100 0000_1(011)
        writer[3].Value.Should().Be((byte)0b01101);
        writer[3].Field.Name.SchemaName.Should().Be(TestNamespace);
        writer[3].Field.Name.Name.Should().Be("midPart");

        // 0B => (12:15) => 0
        writer[4].Value.Should().Be((byte)0x00);
        writer[4].Field.Name.SchemaName.Should().Be(TestNamespace);
        writer[4].Field.Name.Name.Should().Be("hiPart");

        // 74 => (0:3) => 04
        writer[5].Value.Should().Be((byte)0x04);
        writer[5].Field.Name.SchemaName.Should().Be(TestNamespace);
        writer[5].Field.Name.Name.Should().Be("firstLo");

        // 23 => (0:3) => 03
        writer[6].Value.Should().Be((byte)0x03);
        writer[6].Field.Name.SchemaName.Should().Be(TestNamespace);
        writer[6].Field.Name.Name.Should().Be("secondLo");

        // 23 6E => (12:15) => 06
        writer[7].Value.Should().Be((byte)0x06);
        writer[7].Field.Name.SchemaName.Should().Be(TestNamespace);
        writer[7].Field.Name.Name.Should().Be("firstHi");

        // 5A 7F => (12:15) => 07
        writer[8].Value.Should().Be((byte)0x07);
        writer[8].Field.Name.SchemaName.Should().Be(TestNamespace);
        writer[8].Field.Name.Name.Should().Be("secondHi");
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