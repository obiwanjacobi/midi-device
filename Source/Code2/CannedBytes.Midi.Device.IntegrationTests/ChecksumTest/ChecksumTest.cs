using CannedBytes.Midi.Device.IntegrationTests.Stubs;
using Xunit;
using FluentAssertions;
using Xunit.Abstractions;
using System.IO;

namespace CannedBytes.Midi.Device.IntegrationTests.ChecksumTest;

public class ChecksumTest
{
    public const string Folder = "ChecksumTest/";
    public const string ChecksumSchemaFileName = "ChecksumTestSchema.mds";
    public const string ChecksumTestStreamFileName = "ChecksumTestStream.bin";
    
    private readonly ITestOutputHelper _output;

    public ChecksumTest(ITestOutputHelper output)
        => _output = output;

    [Fact]
    public void ChecksumReadTest()
    {
        DictionaryBasedLogicalStub writer = new();

        var serviceProvider = ServiceHelper.CreateServices();
        var ctx = DeviceHelper.ToLogical(serviceProvider,
            Path.Combine(Folder, ChecksumSchemaFileName),
            Path.Combine(Folder, ChecksumTestStreamFileName),
            "checksumTest",
            writer);

        ctx.Should().NotBeNull();

        _output.WriteLine(ctx.RecordManager.ToString());
    }

    //[Fact]
    public void ChecksumWriteTest()
    {
        //var reader = new DictionaryBasedLogicalStub();
        //// fill reader fields
        //reader.AddValue<byte>("SysExData", 0, 0x41);
        //reader.AddValue<byte>("ChecksumData1", 0, 0x01);
        //reader.AddValue<byte>("ChecksumData2", 0, 0x02);
        //reader.AddValue<byte>("ChecksumData3", 0, 0x04);
        //reader.AddValue<byte>("ChecksumData4", 0, 0x08);

        //DeviceHelper.WritePhysical(ChecksumSchemaFileName, "checksumTest", reader);
    }
}