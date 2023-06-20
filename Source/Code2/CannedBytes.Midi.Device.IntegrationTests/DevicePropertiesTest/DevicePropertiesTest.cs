using CannedBytes.Midi.Device.IntegrationTests.Stubs;
using Xunit;
using FluentAssertions;
using Xunit.Abstractions;
using System.IO;

namespace CannedBytes.Midi.Device.IntegrationTests.DevicePropertiesTest;

public class DevicePropertiesTest
{
    public const string Folder = "DevicePropertiesTest/";
    public const string SchemaFileName = "DevicePropertiesTestSchema.mds";
    public const string StreamFileName = "DevicePropertiesTestStream.bin";

    private readonly ITestOutputHelper _output;

    private static DeviceDataContext ToLogical(IMidiLogicalWriter writer, string name)
    {
        var serviceProvider = ServiceHelper.CreateServices();
        var ctx = DeviceHelper.ToLogical(serviceProvider,
            Path.Combine(Folder, SchemaFileName),
            Path.Combine(Folder, StreamFileName), name, writer);

        return ctx;
    }

    public DevicePropertiesTest(ITestOutputHelper output)
        => _output = output;

    [Fact]
    public void ToLogical_SchemaWithProperties_LogicalPropertyValuesInContext()
    {
        DictionaryBasedLogicalStub writer = new();
        var ctx = ToLogical(writer, "RQ1");

        ctx.DeviceProperties.Count.Should().Be(4);
        ctx.DeviceProperties.Find("ManufacturerId").GetValue<int>().Should().Be(65);
        ctx.DeviceProperties.Find("SysExChannel").GetValue<int>().Should().Be(16);
        ctx.DeviceProperties.Find("ModelId").GetValue<int>().Should().Be(43);
        ctx.DeviceProperties.Find("CommandId").GetValue<int>().Should().Be(17);

        _output.WriteLine(ctx.LogManager.ToString());
    }
}