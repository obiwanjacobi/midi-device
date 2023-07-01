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
    public const string TestNamespace = "urn:CarryTestSchema";
    public const string FieldNamespace = TestNamespace + ":";

    private readonly ITestOutputHelper _output;

    public BitConverterTest(ITestOutputHelper output)
        => _output = output;

    private DeviceDataContext ToLogical(string virtualRootNode, IMidiLogicalWriter writer)
    {
        var serviceProvider = ServiceHelper.CreateServices();
        var ctx = DeviceHelper.ToLogical(serviceProvider,
            Path.Combine(Folder, TestSchemaFileName),
            Path.Combine(Folder, TestStreamFileName), virtualRootNode, writer);

        return ctx;
    }

    private DeviceDataContext ToPhysical(string virtualRootNode, IMidiLogicalReader reader)
    {
        var serviceProvider = ServiceHelper.CreateServices();
        var ctx = DeviceHelper.ToPhysical(serviceProvider,
            Path.Combine(Folder, TestSchemaFileName), virtualRootNode, reader);

        return ctx;
    }

    [Fact]
    public void Read_SchemaWithCarry_ByteAndWordValues()
    {
        DictionaryBasedLogicalStub writer = new();

        var ctx = ToLogical("RangeDataTypeTest", writer);

        ctx.Should().NotBeNull();
        _output.WriteLine(ctx.LogManager.ToString());

        AssertWriter(writer);
    }

    [Fact]
    public void ReadBits_SchemaWithRange_ByteAndWordValues()
    {
        DictionaryBasedLogicalStub writer = new();

        var ctx = ToLogical("RangeFieldTest", writer);
        
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
        writer[0].Field!.Name.SchemaName.Should().Be(TestNamespace);
        writer[0].Field!.Name.Name.Should().Be("loByte");

        // 32 => (12:15) => 03
        writer[1].Value.Should().Be((byte)0x03);
        writer[1].Field!.Name.SchemaName.Should().Be(TestNamespace);
        writer[1].Field!.Name.Name.Should().Be("hiByte");

        // 4C => (0:4) => 0C
        writer[2].Value.Should().Be((byte)0x0C);
        writer[2].Field!.Name.SchemaName.Should().Be(TestNamespace);
        writer[2].Field!.Name.Name.Should().Be("loPart");

        // 4C-0B => (6:10) => 14
        // (01)00_1100 0000_1(011)
        writer[3].Value.Should().Be((byte)0b01101);
        writer[3].Field!.Name.SchemaName.Should().Be(TestNamespace);
        writer[3].Field!.Name.Name.Should().Be("midPart");

        // 0B => (12:15) => 0
        writer[4].Value.Should().Be((byte)0x00);
        writer[4].Field!.Name.SchemaName.Should().Be(TestNamespace);
        writer[4].Field!.Name.Name.Should().Be("hiPart");

        // 74 => (0:3) => 04
        writer[5].Value.Should().Be((byte)0x04);
        writer[5].Field!.Name.SchemaName.Should().Be(TestNamespace);
        writer[5].Field!.Name.Name.Should().Be("firstLo");

        // 23 => (0:3) => 03
        writer[6].Value.Should().Be((byte)0x03);
        writer[6].Field!.Name.SchemaName.Should().Be(TestNamespace);
        writer[6].Field!.Name.Name.Should().Be("secondLo");

        // 23 6E => (12:15) => 06
        writer[7].Value.Should().Be((byte)0x06);
        writer[7].Field!.Name.SchemaName.Should().Be(TestNamespace);
        writer[7].Field!.Name.Name.Should().Be("firstHi");

        // 5A 7F => (12:15) => 07
        writer[8].Value.Should().Be((byte)0x07);
        writer[8].Field!.Name.SchemaName.Should().Be(TestNamespace);
        writer[8].Field!.Name.Name.Should().Be("secondHi");
    }

    [Fact]
    public void Write_SchemaWithCarry_ByteAndWordValues()
    {
        var reader = new DictionaryBasedLogicalStub();

        reader.AddStub(FieldNamespace + "loByte", 0x71);
        reader.AddStub(FieldNamespace + "hiByte", 0x03);

        reader.AddStub(FieldNamespace + "loPart", 0x0C);
        reader.AddStub(FieldNamespace + "midPart", 0b01101);
        reader.AddStub(FieldNamespace + "hiPart", 0x00);

        reader.AddStub(FieldNamespace + "firstLo", 0x04);
        reader.AddStub(FieldNamespace + "secondLo", 0x03);

        reader.AddStub(FieldNamespace + "firstHi", 0x06);
        reader.AddStub(FieldNamespace + "secondHi", 0x07);

        var ctx = ToPhysical("RangeDataTypeTest", reader);
        var stream = ctx.StreamManager.CurrentStream;
        stream.Position = 0;

        Assert.Equal(9 + 2, stream.Length);
        Assert.Equal(0xF0, stream.ReadByte());

        Assert.Equal(0x71, stream.ReadByte());
        Assert.Equal(0x30, stream.ReadByte());

        // 000(0_1100) 000(0_1101) 0000_0000
        // (011|01)0(0_1100)
        Assert.Equal(0x4C, stream.ReadByte());
        Assert.Equal(0x03, stream.ReadByte());

        Assert.Equal(0x04, stream.ReadByte());
        Assert.Equal(0x03, stream.ReadByte());

        // 0000_(0110) => 0110_0000
        Assert.Equal(0x60, stream.ReadByte());

        Assert.Equal(0x00, stream.ReadByte());
        Assert.Equal(0x70, stream.ReadByte());

        Assert.Equal(0xF7, stream.ReadByte());
    }
}