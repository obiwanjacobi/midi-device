using System.IO;
using CannedBytes.Midi.Device.IntegrationTests.Stubs;
using FluentAssertions;
using Xunit;

namespace CannedBytes.Midi.Device.IntegrationTests.StringConverterTest;

public class StringConverterTests
{
    public const string Folder = "StringConverterTest";
    public const string TestSchemaFileName = "StringConverterTestSchema.mds";
    public const string TestStreamFileName = "StringConverterTestStream.bin";

    private static void ReadLogical(string virtualRootName, IMidiLogicalWriter writer)
    {
        var serviceProvider = ServiceHelper.CreateServices();
        DeviceHelper.ToLogical(serviceProvider,
            Path.Combine(Folder, TestSchemaFileName),
            Path.Combine(Folder, TestStreamFileName), virtualRootName, writer);
    }

    private static DeviceDataContext WritePhysical(string virtualRootName, IMidiLogicalReader reader)
    {
        var serviceProvider = ServiceHelper.CreateServices();
        var ctx = DeviceHelper.ToPhysical(serviceProvider,
            Path.Combine(Folder, TestSchemaFileName), virtualRootName, reader);

        return ctx;
    }

    [Fact]
    public void Read_String12()
    {
        var writer = new DictionaryBasedLogicalStub();

        ReadLogical("stringTest", writer);

        writer[0].Value.Should().Be("Acoust Guitr");
    }

    [Fact]
    public void Write_String12()
    {
        var reader = new DictionaryBasedLogicalStub();
        reader.AddStub("urn:StringConverterTestSchema.mds:stringField", "Acoust Guitr");

        var ctx = WritePhysical("stringTest", reader);

        var stream = ctx.StreamManager.CurrentStream;
        using var expectedStream = File.OpenRead(Path.Combine(Folder, TestStreamFileName));

        StreamHelper.AssertStreamContentIsEquivalent(expectedStream, stream);
    }
}
