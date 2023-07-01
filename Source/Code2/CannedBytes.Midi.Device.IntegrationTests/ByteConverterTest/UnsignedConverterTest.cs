using System.IO;
using CannedBytes.Midi.Device.IntegrationTests.Stubs;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CannedBytes.Midi.Device.IntegrationTests.CarryTest;

public class UnsignedConverterTest
{
    public const string Folder = "ByteConverterTest";
    public const string TestSchemaFileName = "UnsignedConverterTestSchema.mds";
    public const string TestStreamFileName = "UnsignedConverterTestStream.bin";
    public const string TestNamespace = "urn:UnsignedConverterTestSchema";
    public const string FieldNamespace = TestNamespace + ":";

    private readonly ITestOutputHelper _output;

    public UnsignedConverterTest(ITestOutputHelper output)
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
    public void Read_UnsignedValues()
    {
        DictionaryBasedLogicalStub writer = new();

        var ctx = ToLogical("UnsignedRecord", writer);

        ctx.Should().NotBeNull();
        _output.WriteLine(ctx.LogManager.ToString());

        writer.Count.Should().Be(3);

        // 71 | 32 4C 0B | 74 23 6E 5A 7F

        // 71
        writer[0].Value.Should().Be((byte)0x71);
        writer[0].Field!.Name.SchemaName.Should().Be(TestNamespace);
        writer[0].Field!.Name.Name.Should().Be("unsigned8");

        // 32 4C 0B
        writer[1].Value.Should().Be(0x0B4C32);
        writer[1].Field!.Name.SchemaName.Should().Be(TestNamespace);
        writer[1].Field!.Name.Name.Should().Be("unsigned24");

        // 74 23 6E 5A 7F
        writer[2].Value.Should().Be(0x7F5A6E2374);
        writer[2].Field!.Name.SchemaName.Should().Be(TestNamespace);
        writer[2].Field!.Name.Name.Should().Be("unsigned40");
    }

    [Fact]
    public void Write_UnsignedValues()
    {
        var reader = new DictionaryBasedLogicalStub();

        reader.AddStub(FieldNamespace + "unsigned8", 0x71);
        reader.AddStub(FieldNamespace + "unsigned24", 0x0B4C32);

        reader.AddStub(FieldNamespace + "unsigned40", 0x7F5A6E2374);

        var ctx = ToPhysical("UnsignedRecord", reader);
        var stream = ctx.StreamManager.CurrentStream;
        stream.Position = 0;

        Assert.Equal(9 + 2, stream.Length);
        Assert.Equal(0xF0, stream.ReadByte());
        Assert.Equal(0x71, stream.ReadByte());

        Assert.Equal(0x32, stream.ReadByte());
        Assert.Equal(0x4C, stream.ReadByte());
        Assert.Equal(0x0B, stream.ReadByte());

        Assert.Equal(0x74, stream.ReadByte());
        Assert.Equal(0x23, stream.ReadByte());
        Assert.Equal(0x6E, stream.ReadByte());
        Assert.Equal(0x5A, stream.ReadByte());
        Assert.Equal(0x7F, stream.ReadByte());

        Assert.Equal(0xF7, stream.ReadByte());
    }
}